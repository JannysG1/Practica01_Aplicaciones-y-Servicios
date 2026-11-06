# Diseño de Aplicaciones P01

## ¿Qué es el proyecto?

Este proyecto es una API REST desarrollada con ASP.NET Core sobre .NET 8. Su función principal es administrar usuarios mediante operaciones HTTP de consulta, búsqueda, registro y eliminación.

La API no contiene una interfaz gráfica. Expone endpoints que pueden ser consumidos por aplicaciones web, aplicaciones móviles, clientes de escritorio o herramientas como Swagger y Postman.

## ¿Qué hace?

La API administra usuarios con la siguiente información:

- Identificador del usuario.
- Nombre.
- Correo electrónico.
- Contraseña.
- Estado activo o inactivo.

Los endpoints disponibles pertenecen a la ruta `api/Usuarios`:

| Método | Ruta | Función |
|---|---|---|
| `GET` | `/api/Usuarios` | Obtiene todos los usuarios. |
| `GET` | `/api/Usuarios/{id}` | Busca un usuario por su identificador. |
| `POST` | `/api/Usuarios` | Registra un usuario nuevo. |
| `DELETE` | `/api/Usuarios/{id}` | Elimina un usuario por su identificador. |

## ¿Cómo funciona operativamente?

Cuando la aplicación inicia, ASP.NET Core configura los controladores, Swagger, Entity Framework Core y la conexión con SQL Server.

Cuando un cliente realiza una solicitud HTTP, el flujo operativo es el siguiente:

1. La solicitud llega a `UsuariosController` mediante la ruta correspondiente.
2. El controlador recibe una instancia de `IUsuarioRepository` mediante inyección de dependencias.
3. El controlador delega la operación al repositorio.
4. `UsuarioRepository` utiliza `ApplicationDbContext` para comunicarse con SQL Server.
5. Entity Framework Core convierte las operaciones del repositorio en consultas SQL.
6. SQL Server ejecuta la consulta sobre la tabla `Usuarios`.
7. El resultado regresa al repositorio y posteriormente al controlador.
8. El controlador devuelve una respuesta HTTP al cliente.

El controlador no accede directamente a la base de datos. Esta separación permite mantener el código modular y cambiar la implementación del repositorio sin modificar los endpoints.

## Organización de los componentes

### `Controllers/UsuarioController.cs`

Define los endpoints HTTP de usuarios. Controla las solicitudes, las respuestas y los códigos HTTP, como `200 OK`, `204 No Content` y `404 Not Found`.

### `recursos/IUsuarioRepository.cs`

Define el contrato que debe cumplir cualquier repositorio de usuarios. El controlador depende de esta interfaz y no de una implementación concreta.

### `recursos/repos_usuario.cs`

Contiene la implementación `UsuarioRepository`. Ejecuta las operaciones de consulta, inserción y eliminación mediante Entity Framework Core.

### `Data/ApplicationDbContext.cs`

Es el contexto de Entity Framework Core. Representa la conexión entre la aplicación y la base de datos y define el mapeo de la entidad `Usuario` con la tabla `Usuarios`.

### `model/Usuario.cs`

Representa la entidad de usuario utilizada por la API y mapeada a la tabla de SQL Server.

### `Program.cs`

Configura la aplicación, registra los servicios y establece la inyección de dependencias:

- `ApplicationDbContext` se registra para usar SQL Server.
- `IUsuarioRepository` se vincula con `UsuarioRepository`.
- Los controladores se habilitan para atender las solicitudes HTTP.

## Conexión con SQL Server

La conexión se configura en `appsettings.json` mediante `DefaultConnection`:

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AplicacionesP01Db;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

La cadena utiliza:

- `Server=(localdb)\\MSSQLLocalDB`: instancia local de SQL Server LocalDB.
- `Database=AplicacionesP01Db`: base de datos utilizada por la API.
- `Trusted_Connection=True`: autenticación de Windows.
- `TrustServerCertificate=True`: permite la conexión local sin validar un certificado del servidor.

En `Program.cs`, Entity Framework Core lee esta cadena y configura el proveedor de SQL Server:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

El contexto se inyecta en `UsuarioRepository`. Por esta razón, cada operación del repositorio trabaja directamente con la base de datos configurada en `DefaultConnection`.

## Base de datos

El script `Diseño de Aplicaciones_P01/Scripts/InicializarBaseDeDatos.sql` crea la base de datos `AplicacionesP01Db` y la tabla `Usuarios` con la estructura esperada por la aplicación.

La tabla contiene:

- `IdUsuario`: clave primaria autoincremental.
- `NombreUsuario`: nombre del usuario.
- `CorreoElectronicousuario`: correo electrónico.
- `Contrasenausuario`: contraseña almacenada.
- `Activo`: estado del usuario.

El script también agrega tres registros iniciales para que la API pueda consultar datos desde el primer uso. Si la tabla `Usuarios` ya existe, el script la elimina y la vuelve a crear para garantizar que sus columnas coincidan con el modelo de la aplicación.

## Persistencia de datos

Las operaciones se almacenan permanentemente en SQL Server:

- Las consultas utilizan `AsNoTracking()` cuando los datos solo se leen.
- Los nuevos usuarios se agregan mediante `DbSet.Add` y `SaveChanges()`.
- Los usuarios se eliminan mediante `DbSet.Remove` y `SaveChanges()`.
- Las consultas dejan de depender de una lista en memoria y utilizan la tabla `Usuarios`.

## Tecnologías utilizadas

- .NET 8.
- ASP.NET Core Web API.
- Entity Framework Core 8.
- SQL Server LocalDB.
- Swagger/OpenAPI.
- Inyección de dependencias.
- Patrón repositorio mediante interfaces.
