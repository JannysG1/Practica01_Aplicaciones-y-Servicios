using Diseño_de_Aplicaciones_P01.model;
using Microsoft.EntityFrameworkCore;

namespace Diseño_de_Aplicaciones_P01.Data;

public static class DbInitializer
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var appDb = services.GetRequiredService<ApplicationDbContext>();
            var cafeteriaDb = services.GetRequiredService<CafeteriaDbContext>();

            // Asegurar que las bases de datos existen
            appDb.Database.EnsureCreated();
            cafeteriaDb.Database.EnsureCreated();

            // 1. CONDICIONES PREVIAS Y VALIDACIONES PARA TABLA USUARIOS
            appDb.Database.ExecuteSqlRaw(@"
                IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Usuarios
                    (
                        IdUsuario INT IDENTITY(1,1) NOT NULL,
                        NombreUsuario NVARCHAR(100) NOT NULL,
                        CorreoElectronicousuario NVARCHAR(150) NOT NULL,
                        Contrasenausuario NVARCHAR(255) NOT NULL,
                        Activo BIT NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT (1),
                        CONSTRAINT PK_Usuarios PRIMARY KEY (IdUsuario),
                        CONSTRAINT UQ_Usuarios_Correo UNIQUE (CorreoElectronicousuario),
                        CONSTRAINT CK_Usuarios_Nombre CHECK (LEN(TRIM(NombreUsuario)) >= 2),
                        CONSTRAINT CK_Usuarios_Correo CHECK (CorreoElectronicousuario LIKE '%_@__%.__%'),
                        CONSTRAINT CK_Usuarios_Contrasena CHECK (LEN(Contrasenausuario) >= 4)
                    );
                END
                ELSE
                BEGIN
                    -- Si la tabla ya existía, asegurar que las restricciones de validación estén presentes
                    IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UQ_Usuarios_Correo')
                    BEGIN
                        ALTER TABLE dbo.Usuarios ADD CONSTRAINT UQ_Usuarios_Correo UNIQUE (CorreoElectronicousuario);
                    END

                    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Usuarios_Nombre')
                    BEGIN
                        ALTER TABLE dbo.Usuarios ADD CONSTRAINT CK_Usuarios_Nombre CHECK (LEN(TRIM(NombreUsuario)) >= 2);
                    END

                    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Usuarios_Correo')
                    BEGIN
                        ALTER TABLE dbo.Usuarios ADD CONSTRAINT CK_Usuarios_Correo CHECK (CorreoElectronicousuario LIKE '%_@__%.__%');
                    END

                    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Usuarios_Contrasena')
                    BEGIN
                        ALTER TABLE dbo.Usuarios ADD CONSTRAINT CK_Usuarios_Contrasena CHECK (LEN(Contrasenausuario) >= 4);
                    END
                END
            ");

            // Sembrar usuarios iniciales si la tabla está vacía
            if (!appDb.Usuarios.Any())
            {
                appDb.Usuarios.AddRange(
                    new Usuario { NombreUsuario = "Administrador Principal", CorreoElectronicousuario = "admin@cafeteria.com", Contrasenausuario = "admin123", Activo = true },
                    new Usuario { NombreUsuario = "Ana Torres", CorreoElectronicousuario = "ana@correo.com", Contrasenausuario = "ana123", Activo = true },
                    new Usuario { NombreUsuario = "Marco Aurelio", CorreoElectronicousuario = "marco@correo.com", Contrasenausuario = "marco123", Activo = true },
                    new Usuario { NombreUsuario = "Maria Antonieta", CorreoElectronicousuario = "maria@correo.com", Contrasenausuario = "maria123", Activo = true },
                    new Usuario { NombreUsuario = "Carlos Perez", CorreoElectronicousuario = "carlos@correo.com", Contrasenausuario = "1234", Activo = true }
                );
                appDb.SaveChanges();
            }

            // 2. CONDICIONES PREVIAS Y VALIDACIONES PARA TABLA PRODUCTOS
            cafeteriaDb.Database.ExecuteSqlRaw(@"
                IF OBJECT_ID(N'dbo.Productos', N'U') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Productos_Precio')
                    BEGIN
                        ALTER TABLE dbo.Productos ADD CONSTRAINT CK_Productos_Precio CHECK (Precio >= 0);
                    END

                    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Productos_Nombre')
                    BEGIN
                        ALTER TABLE dbo.Productos ADD CONSTRAINT CK_Productos_Nombre CHECK (LEN(TRIM(Nombre)) >= 2);
                    END
                END
            ");

            // Sembrar productos iniciales si la tabla está vacía
            if (!cafeteriaDb.Productos.Any())
            {
                cafeteriaDb.Productos.AddRange(
                    new Producto { Nombre = "Café Americano", Precio = 1.50m, Activo = true },
                    new Producto { Nombre = "Capuchino", Precio = 2.50m, Activo = true },
                    new Producto { Nombre = "Espresso Doble", Precio = 2.80m, Activo = true },
                    new Producto { Nombre = "Pastel de Chocolate", Precio = 3.20m, Activo = true }
                );
                cafeteriaDb.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetService<ILogger<ApplicationDbContext>>();
            logger?.LogError(ex, "Ocurrió un error al inicializar las bases de datos.");
        }
    }
}
