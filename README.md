# Diseño de Aplicaciones P01 - API REST con ASP.NET Core & JWT

## ¿Qué es el proyecto?

Este proyecto es una API REST desarrollada con ASP.NET Core sobre .NET 8. Proporciona administración de usuarios conectada a SQL Server y un sistema de autenticación y autorización mediante **JSON Web Tokens (JWT)** con control de acceso basado en roles para proteger recursos sensibles (como el catálogo de productos).

La API no contiene una interfaz gráfica tradicional; expone endpoints HTTP consumibles por aplicaciones web (SPA/Blazor), aplicaciones móviles, clientes de escritorio o herramientas como Swagger UI y Postman.

---

## Endpoints Disponibles

### 1. Autenticación (`api/Auth`)

| Método | Ruta | Autorización | Función |
|---|---|---|---|
| `POST` | `/api/Auth/login` | Pública (`[AllowAnonymous]`) | Recibe credenciales (`usuario`, `password`), valida y genera el token JWT con sus claims correspondientes. |

### 2. Gestión de Productos (`api/Productos`)

| Método | Ruta | Autorización | Función |
|---|---|---|---|
| `GET` | `/api/Productos` | Requiere Token JWT (`[Authorize]`) | Obtiene la lista de productos disponibles. |
| `GET` | `/api/Productos/{id}` | Requiere Token JWT (`[Authorize]`) | Obtiene un producto por su identificador. |
| `POST` | `/api/Productos` | Requiere Rol `Admin` (`[Authorize(Roles = "Admin")]`) | Agrega un nuevo producto al catálogo. |
| `DELETE` | `/api/Productos/{id}` | Requiere Rol `Admin` (`[Authorize(Roles = "Admin")]`) | Elimina un producto por su identificador. |

### 3. Gestión de Usuarios (`api/Usuarios`)

| Método | Ruta | Autorización | Función |
|---|---|---|---|
| `GET` | `/api/Usuarios` | Pública | Obtiene todos los usuarios registrados en SQL Server. |
| `GET` | `/api/Usuarios/{id}` | Pública | Busca un usuario por su identificador. |
| `POST` | `/api/Usuarios` | Pública | Registra un usuario nuevo en la base de datos. |
| `DELETE` | `/api/Usuarios/{id}` | Pública | Elimina un usuario por su identificador. |

---

## Autenticación y Autorización con JWT

### ¿Cómo funciona el flujo de JWT?

1. **Petición de Login**: El cliente envía sus credenciales (`Usuario` y `Password`) al endpoint `POST /api/Auth/login`.
2. **Generación del Token**: Si las credenciales son válidas, la API genera un token JWT firmado mediante el algoritmo `HmacSha256` con una clave secreta simétrica. Dentro del token se almacenan los **claims**:
   - `sub`: Identificador/nombre del usuario.
   - `http://schemas.microsoft.com/ws/2008/06/identity/claims/role`: Rol asignado (ej. `Admin`).
   - `jti`: Identificador único del token (GUID) para prevenir ataques de repetición.
3. **Uso del Token en Peticiones**: En las peticiones posteriores, el cliente envía el token en el encabezado HTTP:
   ```http
   Authorization: Bearer <token_jwt>
   ```
4. **Validación Automática**: El middleware de autenticación (`JwtBearer`) en `Program.cs` intercepta la petición, verifica la firma con la clave simétrica, el emisor (`Issuer`), la audiencia (`Audience`) y la fecha de expiración (`Lifetime`). Si es válido, extrae los claims del usuario y permite la ejecución; de lo contrario, responde automáticamente con un código de error:
   - **`401 Unauthorized`**: Si la petición no tiene token, está vencido o la firma es inválida.
   - **`403 Forbidden`**: Si el token es válido pero el usuario no tiene el rol exigido por la acción (ej. un usuario sin rol `Admin` intentando ejecutar `DELETE`).

### Configuración en `appsettings.json`

Los parámetros del token se configuran en el archivo `appsettings.json` para facilitar el cambio entre entornos:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AplicacionesP01Db;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Key": "esta-es-una-clave-secreta-de-mas-de-32-caracteres-123",
    "Issuer": "CafeteriaBackend",
    "Audience": "CafeteriaClientes"
  }
}
```

---

## Configuración de Swagger con Soporte para JWT

Swagger UI incluye soporte para esquemas de seguridad HTTP `Bearer`, lo que permite probar los endpoints directamente desde el navegador:

1. Al abrir la aplicación en modo desarrollo, la URL raíz `/` redirige automáticamente a `/swagger`.
2. En la interfaz se presenta el botón **Authorize**.
3. Al ingresar el token recibido en el login (únicamente la cadena del token, sin la palabra "Bearer"), Swagger lo añade a todas las peticiones salientes en el encabezado `Authorization`.

---

## Organización de los Componentes

### Controladores (`Controllers/`)
- **`AuthController.cs`**: Controlador que gestiona la autenticación, validación de credenciales y generación del token JWT firmado con claims y expiración.
- **`ProductosController.cs`**: Controlador protegido mediante atributos `[Authorize]` y `[Authorize(Roles = "Admin")]` para ilustrar el control de acceso en la API.
- **`UsuarioController.cs`**: Controlador que gestiona las operaciones CRUD de los usuarios comunicándose con el repositorio.

### Modelos (`model/`)
- **`Usuario.cs`**: Representa la entidad de usuario mapeada a la tabla `Usuarios` en SQL Server.
- **`Producto.cs`**: Representa la entidad de producto para el catálogo.

### Repositorios y Recursos (`recursos/`)
- **`IUsuarioRepository.cs`**: Contrato para el acceso y manipulación de datos de usuarios.
- **`repos_usuario.cs`**: Implementación del repositorio de usuarios utilizando `ApplicationDbContext`.
- **`ProductoRepository.cs`**: Repositorio de productos registrado como Singleton para demostración de operaciones en memoria.

### Acceso a Datos (`Data/`)
- **`ApplicationDbContext.cs`**: Contexto de Entity Framework Core que configura y gestiona el mapeo de entidades con SQL Server LocalDB.

### Configuración Global (`Program.cs`)
- Registro de controladores y repositorios en el contenedor de inyección de dependencias.
- Registro del contexto de base de datos con SQL Server.
- Configuración del servicio de autenticación JWT (`AddAuthentication` y `AddJwtBearer`).
- Configuración de autorización (`AddAuthorization`).
- Configuración de Swagger con esquemas de seguridad OpenAPI (`Bearer`).
- Definición del orden de los middlewares en el pipeline HTTP (`UseHttpsRedirection`, Swagger, `UseAuthentication`, `UseAuthorization`, `MapControllers`).

---

## Persistencia y Base de Datos SQL Server

La aplicación utiliza SQL Server LocalDB mediante Entity Framework Core 8:
- Cadena de conexión: `DefaultConnection` apuntando a `(localdb)\MSSQLLocalDB` y base de datos `AplicacionesP01Db`.
- Script de inicialización: `Diseño de Aplicaciones_P01/Scripts/InicializarBaseDeDatos.sql` para crear la base de datos y la tabla `Usuarios` con datos de prueba iniciales.

---

## Tecnologías Utilizadas

- **.NET 8.0 SDK**
- **ASP.NET Core Web API**
- **Microsoft.AspNetCore.Authentication.JwtBearer (8.0.13)**
- **System.IdentityModel.Tokens.Jwt (8.0.1)**
- **Entity Framework Core 8 con SQL Server**
- **Swashbuckle.AspNetCore (10.3.1)**
- **SQL Server LocalDB**
- **Inyección de dependencias y patrón repositorio**
