using System.Text;
using System.Security.Claims;
using Diseño_de_Aplicaciones_P01.Data;
using Diseño_de_Aplicaciones_P01.Models;
using Diseño_de_Aplicaciones_P01.recursos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// ---------- SERVICIOS ----------
builder.Services.AddControllers();

// Base de datos CafeteriaDbContext (Entity Framework Core con SQL Server e Identity)
builder.Services.AddDbContext<CafeteriaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CafeteriaDb")));

// Base de datos ApplicationDbContext (existente para usuarios)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositorios (AddScoped: una instancia por petición HTTP para trabajar con DbContext)
builder.Services.AddScoped<ProductoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Registro de ASP.NET Core Identity Core (Paso 3)
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        // Reglas de contraseña (Paso 3)
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;

        // Reglas de usuario
        options.User.RequireUniqueEmail = true;

        // Bloqueo de cuenta tras intentos fallidos
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<CafeteriaDbContext>();

// Autenticación: "los usuarios se identifican con un JWT"
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // valida quién emitió el token
            ValidateAudience = true, // valida para quién es
            ValidateLifetime = true, // rechaza tokens vencidos
            ValidateIssuerSigningKey = true, // valida la firma
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name
        };
    });

// Habilita las reglas de permisos ([Authorize], roles, políticas)
builder.Services.AddAuthorization();

// Soporte CORS para clientes SPA / Blazor WebAssembly (Punto 12)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger con soporte para JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega solo el token, sin la palabra Bearer"
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

// Inicialización de bases de datos existentes y productos
DbInitializer.Initialize(app.Services);

// Paso 4: Sembrado de roles y usuario Administrador inicial
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
}

// ---------- PIPELINE ----------
app.UseHttpsRedirection();

// Habilitar política CORS
app.UseCors("AllowAll");

// Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseAuthentication(); // 1.º: ¿quién eres? (lee el token)
app.UseAuthorization();  // 2.º: ¿tienes permiso?

app.MapControllers();

app.Run();
