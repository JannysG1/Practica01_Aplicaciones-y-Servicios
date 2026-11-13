# Diseño de Aplicaciones P01 - API REST con ASP.NET Core Identity & JWT

## ¿Qué es el proyecto?

Este proyecto es una API REST profesional desarrollada con **ASP.NET Core** sobre **.NET 8**. Integra un sistema robusto de autenticación y autorización combinando **ASP.NET Core Identity** con **JSON Web Tokens (JWT)** y persistencia en **SQL Server** mediante **Entity Framework Core**. El sistema implementa Control de Acceso Basado en Roles (**RBAC**) para proteger recursos del catálogo de productos y la gestión de usuarios, garantizando el cumplimiento de los principios de diseño de software **SOLID** y **DRY**.

La API expone endpoints HTTP consumibles por clientes web (Single Page Applications y **Blazor WebAssembly**), aplicaciones móviles, clientes de escritorio y herramientas como **Swagger UI** y **Postman**.

---

## Endpoints Disponibles

### 1. Autenticación y Registro (`api/Auth`)

| Método | Ruta | Autorización | Función |
|---|---|---|---|
| `POST` | `/api/Auth/login` | Pública (`[AllowAnonymous]`) | Recibe credenciales (`usuario` o `correo`, `password`), valida contra Identity (con hash y control de bloqueo), y genera el token JWT con sus claims de rol. |
| `POST` | `/api/Auth/register` | Pública (`[AllowAnonymous]`) | Registra nuevos usuarios en Identity con validación estricta de contraseñas, unicidad de correo/usuario y asignación automática del rol `Usuario`. |

### 2. Gestión de Productos (`api/Productos`)

| Método | Ruta | Autorización | Función |
|---|---|---|---|
| `GET` | `/api/Productos` | Requiere Token JWT (`[Authorize]`) | Obtiene la lista de productos disponibles desde SQL Server (`CafeteriaDb`). Permite acceso a `Admin` y `Usuario`. |
| `GET` | `/api/Productos/{id}` | Requiere Token JWT (`[Authorize]`) | Obtiene un producto por su identificador. |
| `POST` | `/api/Productos` | Requiere Rol `Admin` (`[Authorize(Roles = "Admin")]`) | Agrega un nuevo producto a la base de datos (validado por DTO). Si accede `Usuario`, responde `403 Forbidden`. |
| `PUT` | `/api/Productos/{id}` | Requiere Rol `Admin` (`[Authorize(Roles = "Admin")]`) | Actualiza un producto existente en la base de datos. Si accede `Usuario`, responde `403 Forbidden`. |
| `DELETE` | `/api/Productos/{id}` | Requiere Rol `Admin` (`[Authorize(Roles = "Admin")]`) | Elimina un producto por su identificador en SQL Server. Si accede `Usuario`, responde `403 Forbidden`. |

### 3. Gestión de Usuarios (`api/Usuarios`)

| Método | Ruta | Autorización | Función |
|---|---|---|---|
| `GET` | `/api/Usuarios` | Requiere Token JWT (`[Authorize(Roles = "Admin,Usuario")]`) | Consulta la lista de todos los usuarios registrados. |
| `GET` | `/api/Usuarios/{id}` | Requiere Token JWT (`[Authorize(Roles = "Admin,Usuario")]`) | Consulta un usuario por su identificador. |
| `POST` | `/api/Usuarios` | Requiere Rol `Admin` (`[Authorize(Roles = "Admin")]`) | Solo `Admin` puede registrar usuarios por esta vía administrativa. |
| `PUT` | `/api/Usuarios/{id}` | Requiere Rol `Admin` (`[Authorize(Roles = "Admin")]`) | Solo `Admin` puede actualizar usuarios existentes. |
| `DELETE` | `/api/Usuarios/{id}` | Requiere Rol `Admin` (`[Authorize(Roles = "Admin")]`) | Solo `Admin` puede eliminar usuarios. |

---

## Implementación Detallada: Guía 4 - ASP.NET Core Identity con JWT

A continuación se detalla la implementación paso a paso de los 12 puntos descritos en el documento de la práctica:

### 1. Teoría Básica: Identity y JWT
- **ASP.NET Core Identity**: Gestiona la persistencia de usuarios, contraseñas hasheadas de forma segura con PBKDF2/SHA256, reglas de complejidad, bloqueo temporal por intentos fallidos y administración de roles. Actúa únicamente al momento de la autenticación (`login`) y registro (`register`).
- **JWT**: Viaja en cada petición HTTP en el encabezado `Authorization: Bearer <token>` y es validado en memoria por el middleware `JwtBearer` (firma criptográfica, emisor, audiencia y vigencia), sin consultar la base de datos en cada llamada.
- **`AddIdentityCore` vs `AddIdentity`**: Se utilizó `AddIdentityCore<ApplicationUser>` ya que está diseñado para APIs REST con JWT; no impone esquemas de autenticación basados en cookies que interferirían con JWT.

### 2. Paso 1: Paquete y Clase `ApplicationUser`
- Paquete instalado: `Microsoft.AspNetCore.Identity.EntityFrameworkCore` versión `8.0.13` (compatible y alineada con los paquetes de EF Core y .NET 8).
- Clase de usuario personalizada: `model/ApplicationUser.cs` hereda de `IdentityUser`, permitiendo extender las propiedades predeterminadas de Identity con `NombreCompleto` y manteniendo `Id` de tipo string (GUID).

### 3. Paso 2: Conectar Identity al `DbContext` y Migraciones
- `Data/CafeteriaDbContext.cs` se modificó para heredar de `IdentityDbContext<ApplicationUser>`, asegurando que `base.OnModelCreating(modelBuilder)` se ejecute siempre en primer lugar.
- Migración creada y aplicada:
  ```powershell
  dotnet ef migrations add AgregarIdentity --context CafeteriaDbContext
  dotnet ef database update --context CafeteriaDbContext
  ```
- Tablas generadas en SQL Server: `AspNetRoles`, `AspNetUsers`, `AspNetUserRoles`, `AspNetUserClaims`, `AspNetRoleClaims`, `AspNetUserLogins`, `AspNetUserTokens`, manteniendo intacta la tabla `Productos`.

### 4. Paso 3: Registrar Identity en `Program.cs`
Se registraron los servicios de Identity con reglas estrictas de seguridad:
- Longitud mínima de contraseña: 8 caracteres.
- Requisitos: Dígito numérico, letra mayúscula y letra minúscula.
- Caracteres no alfanuméricos opcionales (`RequireNonAlphanumeric = false`).
- Correo único obligatorio (`RequireUniqueEmail = true`).
- Bloqueo por intentos fallidos: Habilitado para nuevos usuarios (`AllowedForNewUsers = true`), máximo 5 intentos fallidos (`MaxFailedAccessAttempts = 5`), duración del bloqueo de 5 minutos (`DefaultLockoutTimeSpan = 5 min`).
- Soporte para roles (`AddRoles<IdentityRole>()`) y almacenamiento con EF Core (`AddEntityFrameworkStores<CafeteriaDbContext>()`).

### 5. Paso 4: Roles y Administrador Inicial (`IdentitySeeder`)
- Se implementó `Data/IdentitySeeder.cs`, el cual inicializa idempotentemente:
  1. Los roles indispensables: `Admin` y `Usuario`.
  2. El usuario administrador principal si no existe.
  3. La asignación del rol `Admin` al usuario inicial.
- Se configuraron las credenciales seguras fuera del código fuente utilizando **User Secrets**:
  ```powershell
  dotnet user-secrets init
  dotnet user-secrets set "SeedAdmin:Email" "admin@cafeteria.com"
  dotnet user-secrets set "SeedAdmin:Password" "Admin123"
  ```
- En `Program.cs`, el seeder se ejecuta al arranque de la aplicación dentro de un `IServiceScope`.

### 6. Paso 5: Login contra la Base de Datos
- `Controllers/AuthController.cs` fue actualizado para eliminar usuarios fijos en memoria.
- `UserManager<ApplicationUser>` busca al usuario por nombre de usuario (`FindByNameAsync`) o por correo electrónico (`FindByEmailAsync`).
- Detección de cuentas bloqueadas mediante `IsLockedOutAsync(user)`.
- Verificación segura del hash mediante `CheckPasswordAsync(user, request.Password)`.
- Incremento del contador de fallos con `AccessFailedAsync(user)` y reinicio al tener éxito con `ResetAccessFailedCountAsync(user)`.
- Lectura dinámica de roles desde SQL Server con `GetRolesAsync(user)` y construcción del token JWT con claims (`Sub`, `Name`, `Jti`, y `ClaimTypes.Role`).

### 7. Paso 6: Registro de Usuarios
- Endpoint `POST /api/Auth/register` accesible sin token (`[AllowAnonymous]`).
- Crea nuevos usuarios mediante `userManager.CreateAsync(user, request.Password)`. Si la contraseña no cumple las reglas o el usuario/correo está duplicado, retorna `400 Bad Request` detallando los errores.
- Asigna automáticamente en el servidor el rol `Usuario` (`AddToRoleAsync`), garantizando que ningún cliente pueda autoasignarse roles privilegiados.

### 8. Verificación en SQL Server (`sqlcmd`)
Se verificó el estado de la base de datos en SQL Server LocalDB (`CafeteriaDb`):
```powershell
# Verificar tablas generadas
sqlcmd -S "(localdb)\MSSQLLocalDB" -d CafeteriaDb -E -Q "SELECT name FROM sys.tables ORDER BY name"

# Verificar historial de migraciones
sqlcmd -S "(localdb)\MSSQLLocalDB" -d CafeteriaDb -E -Q "SELECT MigrationId FROM __EFMigrationsHistory"

# Verificar roles creados
sqlcmd -S "(localdb)\MSSQLLocalDB" -d CafeteriaDb -E -Q "SELECT Name FROM AspNetRoles"

# Verificar usuarios y sus roles asignados
sqlcmd -S "(localdb)\MSSQLLocalDB" -d CafeteriaDb -E -Q "SELECT u.UserName, u.Email, r.Name AS Rol FROM AspNetUsers u JOIN AspNetUserRoles ur ON ur.UserId = u.Id JOIN AspNetRoles r ON r.Id = ur.RoleId"

# Comprobar almacenamiento seguro con hash
sqlcmd -S "(localdb)\MSSQLLocalDB" -d CafeteriaDb -E -Q "SELECT UserName, LEFT(PasswordHash, 12) AS HashInicio FROM AspNetUsers"
```

### 9. Pruebas Funcionales y de Seguridad
Todas las pruebas del documento fueron ejecutadas y validadas exitosamente:
1. **Login Admin con usuario**: `admin` / `Admin123` -> `200 OK`, token generado y `roles: ["Admin"]`.
2. **Acceso a Productos con Admin**: Con token Admin, `GET /api/Productos` responde `200 OK` y `POST /api/Productos` responde `201 Created`.
3. **Login con credenciales fijas antiguas**: `admin` / `1234` -> `401 Unauthorized`.
4. **Login Admin con correo**: `admin@cafeteria.com` / `Admin123` -> `200 OK`.
5. **Registro de cliente**: `POST /api/Auth/register` con `cliente1` y contraseña `Cliente123` -> `200 OK`.
6. **Validación de roles (RBAC)**: Login con `cliente1` obtiene `roles: ["Usuario"]`. Con su token, `GET /api/Productos` responde `200 OK`, pero `POST /api/Productos` responde `403 Forbidden`.
7. **Reglas de contraseña y duplicados**: Intento de registro con contraseña `1234` responde `400 Bad Request` con lista de reglas violadas; intento de registro con usuario repetido responde `400 Bad Request`.
8. **Bloqueo de cuenta (Lockout)**: Tras 5 intentos fallidos consecutivos de contraseña, el 6.º intento responde:
   `401 Unauthorized` con el mensaje `"Cuenta bloqueada temporalmente. Intenta más tarde."`.

### 10. Errores Comunes y Prevención
- **requires a primary key al migrar**: Resuelto ejecutando `base.OnModelCreating(modelBuilder)` al inicio del método.
- **La migración incluye CreateTable de Productos**: Resuelto asegurando que la migración base `Inicial` ya existía.
- **Contraseña de Admin no cumple reglas**: Garantizado usando `Admin123` (8+ caracteres, mayúscula, minúscula, dígito).
- **Desbloqueo manual en pruebas**:
  ```sql
  SET QUOTED_IDENTIFIER ON;
  UPDATE AspNetUsers SET LockoutEnd = NULL, AccessFailedCount = 0 WHERE UserName = 'usuario';
  ```

### 11. Glosario de Conceptos Clave
- **Identity**: Framework integral de ASP.NET Core para autenticación, gestión de credenciales y roles.
- **UserManager / RoleManager**: Servicios principales para manipulación y consulta de identidades.
- **PasswordHash**: Hash criptográfico unidireccional irreversible para almacenamiento de contraseñas.
- **Claim**: Declaración de identidad o atributos empaquetados en el JWT (identificador, nombre, rol).
- **Seeder**: Mecanismo de inicialización de datos base al arrancar la aplicación.
- **User Secrets**: Almacén seguro de configuraciones locales de desarrollo fuera del árbol del repositorio Git.
- **Scoped**: Ciclo de vida por petición HTTP utilizado por los repositorios y contextos de base de datos.
- **Lockout**: Mecanismo de defensa contra ataques de fuerza bruta que bloquea temporalmente una cuenta.
- **AddIdentityCore**: Configuración liviana de Identity para APIs desacoplada de cookies.

### 12. Siguiente Etapa: Integración con Cliente Blazor WebAssembly
Para que el cliente **Blazor WebAssembly** consuma de manera nativa esta API sin problemas de seguridad de origen cruzado:
1. **CORS Habilitado en la API**: Se incorporó en `Program.cs` la política de CORS (`app.UseCors("AllowAll")`), permitiendo que el cliente Blazor corriendo en el navegador (en un puerto o dominio distinto) pueda invocar los endpoints sin restricciones de navegador.
2. **Consumo de Endpoints**: Blazor consumirá `POST /api/Auth/login` enviando las credenciales, recibiendo el token JWT y almacenándolo en el almacenamiento local del navegador (`localStorage`).
3. **`AuthenticationStateProvider` Personalizado**: Blazor deserializa el payload del token JWT, extrayendo los claims `name` y `role`.
4. **Vistas Protegidas con `<AuthorizeView>`**:
   - Elementos visibles para todos los autenticados: catálogo de productos con precios.
   - Botones de "Agregar Producto", "Editar" y "Eliminar": envueltos en `<AuthorizeView Roles="Admin">`, manteniéndose ocultos para usuarios estándar y visibles exclusivamente para administradores.
5. **Formulario de Registro**: Vista en Blazor conectada a `POST /api/Auth/register` para permitir el alta pública de nuevos clientes.

---

## Archivo de Pruebas HTTP (`Diseño de Aplicaciones_P01.http`)

El proyecto incluye el archivo `Diseño de Aplicaciones_P01.http` completamente actualizado con los 15 casos de prueba de Identity, JWT, RBAC y validaciones, listo para ser ejecutado con un clic desde Visual Studio o VS Code.

---

## Tecnologías Utilizadas

- **.NET 8.0 SDK**
- **ASP.NET Core Web API**
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.13)**
- **Microsoft.AspNetCore.Authentication.JwtBearer (8.0.13)**
- **System.IdentityModel.Tokens.Jwt (8.0.1)**
- **Microsoft.EntityFrameworkCore.SqlServer (8.0.25)**
- **Microsoft.EntityFrameworkCore.Design (8.0.13)**
- **Entity Framework Core .NET Command-line Tools (dotnet-ef 10.0.14)**
- **Swashbuckle.AspNetCore (10.3.1)**
- **SQL Server LocalDB (MSSQLLocalDB)**
- **User Secrets Manager**
