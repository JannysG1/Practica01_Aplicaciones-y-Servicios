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

### 4. Gestión Dinámica de Roles (`api/Roles`)

| Método | Ruta | Autorización | Función |
|---|---|---|---|
| `GET` | `/api/Roles` | Requiere Rol `Admin` o `SuperAdmin` | Obtiene el catálogo de roles disponibles en el sistema. |
| `POST` | `/api/Roles` | Requiere Rol `Admin` o `SuperAdmin` | Crea un nuevo rol dinámico en Identity validando unicidad. |
| `POST` | `/api/Roles/asignar` | Requiere Rol `Admin` o `SuperAdmin` | Asocia un rol existente a un usuario especificado. |
| `GET` | `/api/Roles/usuario/{usuario}` | Requiere Rol `Admin` o `SuperAdmin` | Consulta la lista de roles asignados a un usuario específico. |

### 5. Cómputo de Tablas Matemáticas y Trigonométricas (`api/TablasCalculo`)

| Método | Ruta | Autorización | Función |
|---|---|---|---|
| `GET` | `/api/TablasCalculo/potencias` | Pública (`[AllowAnonymous]`) | Genera tabla de potencias (base y rango de exponentes por query params). |
| `POST` | `/api/TablasCalculo/potencias` | Pública (`[AllowAnonymous]`) | Genera tabla de potencias a partir de un cuerpo JSON validado. |
| `GET` | `/api/TablasCalculo/potencias/texto` | Pública (`[AllowAnonymous]`) | Retorna la tabla de potencias en texto plano ASCII tabulado. |
| `GET` | `/api/TablasCalculo/trigonometria` | Pública (`[AllowAnonymous]`) | Genera tabla de ángulos, radianes, senos y cosenos (por query params). |
| `POST` | `/api/TablasCalculo/trigonometria` | Pública (`[AllowAnonymous]`) | Genera tabla trigonométrica a partir de un cuerpo JSON con paso y precisión. |
| `GET` | `/api/TablasCalculo/trigonometria/texto` | Pública (`[AllowAnonymous]`) | Retorna la tabla trigonométrica en texto plano ASCII tabulado. |

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

---

## Examen Práctico Corto (PPP04) - Módulo Proveedores & Evidencia de Persistencia

### 1. Requerimientos Implementados
- **Entidad `Models/Proveedor`**: Con propiedades `Id` (int identity), `Nombre` (nvarchar(100) not null), `Telefono` (nvarchar(20) null) y `Activo` (bit not null).
- **`CafeteriaDbContext`**: Registrada la propiedad `public DbSet<Proveedor> Proveedores => Set<Proveedor>();` y configuradas longitudes en `OnModelCreating` (`Nombre: HasMaxLength(100)`, `Telefono: HasMaxLength(20)`).
- **Migración Code-First**: Migración `AgregarProveedores` creada y aplicada en base de datos `CafeteriaDb`.
- **`ProveedoresController`**: Endpoint `POST /api/Proveedores` protegido con `[Authorize(Roles = "Admin")]`.
- **Persistencia Directa**: Guarda en base de datos mediante `_context.Proveedores.Add` y `_context.SaveChangesAsync()`, retornando `201 CreatedAtAction(...)`.

### 2. Guion de Prueba Oficial y Evidencia de Persistencia

| Paso | Acción | Resultado Esperado | Resultado Obtenido | Estado |
|:---:|:---|:---|:---|:---:|
| **1** | Login `admin / 1234` &rarr; Authorize | Token JWT emitido con rol `Admin` | Token generado exitosamente y cargado en Swagger | ✅ Aprobado |
| **2** | `POST /api/Proveedores` con `{"nombre":"Distribuidora La Costa","telefono":"0991234567","activo":true}` | **201 Created** con `"id": 1` | **201 Created** con payload JSON e Id generado | ✅ Aprobado |
| **3** | SQL Server Object Explorer &rarr; `dbo.Proveedores` &rarr; Ver datos | 1 fila con los datos enviados | 1 fila visible en SQL Server con Id=1 | ✅ Aprobado |

#### Evidencia de Respuesta Swagger (201 Created):
```json
{
  "id": 1,
  "nombre": "Distribuidora La Costa",
  "telefono": "0991234567",
  "activo": true
}
```

#### Evidencia de Consulta en SQL Server (`dbo.Proveedores`):
```text
Id          Nombre                                                                                               Telefono             Activo
----------- ---------------------------------------------------------------------------------------------------- -------------------- ------
          1 Distribuidora La Costa                                                                               0991234567                1

(1 rows affected)
```

---

## Deberes / Ejercicios: Generación de Tablas de Potencias y Funciones Trigonométricas

En conformidad con las directrices académicas institucionales de la **Escuela Politécnica Nacional (EPN)**, se incorporó al proyecto una arquitectura limpia y desacoplada para el cálculo numérico, generación de tablas de potencias y funciones trigonométricas (seno y coseno), cumpliendo con todas las normas de entrega y buenas prácticas de ingeniería de software.

### 1. Directrices de Entrega Cumplidas
- **Inclusión de comentarios de alto nivel orientados al propósito**: Todos los comentarios en el código explican **qué hace** y cuál es la intención de diseño o semántica matemática, absteniéndose de describir mecánicamente el **cómo lo hace** la sintaxis del lenguaje (apego estricto a las normas de *Clean Code*).
- **Inclusión de cambios y mejoras solicitadas**: Se implementó el cómputo de potencias con base y rangos de exponentes configurables, así como el barrido angular con evaluación trigonométrica (seno, coseno y tangente con detección de asíntotas).
- **Exclusión estricta de artefactos generados automáticamente**: Las carpetas `bin/` y `obj/` se encuentran estrictamente ignoradas a través del archivo `.gitignore` y no forman parte del control de versiones de Git.
- **Documentación central en `README.md`**: Toda la especificación, contratos de API, principios aplicados y ejemplos de prueba se integran de manera formal en el repositorio de GitHub.

### 2. Encabezado Académico Institucional (EPN)
El archivo principal del servicio de cálculo (`Service/TablasCalculoService.cs`) incluye en su encabezado la estructura canónica requerida:

```text
# **********************************************************
# Deber/Ejercicio 01
# Tu Nombre va aquí: Jannys Garrido
# Fecha de realización: 24/11/2026
# Fecha de entrega: 24/11/2026
# Resultados:
#  * El código permita generar una tabla con potencias y una tabla con ángulos y los
#    valores de las funciones seno y coseno
# Conclusiones:
#  * La abstracción mediante la interfaz ITablasCalculoService desacopla el cálculo matemático
#    de las capas de transporte y presentación, cumpliendo rigurosamente con SRP y DIP.
#  * El tratamiento numérico con normalización de residuos infinitesimales en múltiplos de 90°
#    garantiza exactitud física y consistencia matemática frente al estándar IEEE 754.
#  * La generación dual (estructurada en objetos DTO y renderizada en texto ASCII tabular)
#    optimiza tanto el consumo de clientes API como la legibilidad en consolas y reportes.
# Recomendaciones:
#  * Restringir los rangos de entrada en los DTOs para impedir anomalías por desbordamiento numérico
#    o degradación en tiempos de respuesta por tamaños excesivos de paso angular.
#  * Enfocar la documentación y comentarios del código en la intención del negocio y la semántica
#    del dominio, omitiendo descripciones mecánicas de sintaxis para preservar el principio Clean Code.
#  * Centralizar el cálculo de funciones trigonométricas en métodos puros e inmutables para facilitar
#    su prueba unitaria y futura extensión a otras funciones matemáticas.
# Bibliografía:
#  * Martin, R. C. (2008). Clean Code: A Handbook of Agile Software Craftsmanship. Prentice Hall.
#  * Microsoft Learn. Documentación de .NET: Operaciones matemáticas y diseño de APIs REST en C#.
#  * IEEE Standard for Floating-Point Arithmetic (IEEE 754-2019).
# **********************************************************
```

### 3. Arquitectura y Principios SOLID Implementados

1. **SRP (Single Responsibility Principle - Principio de Responsabilidad Única)**:
   - `TablasCalculoService`: Responsable única y exclusivamente del algoritmo de cálculo matemático, normalización de coma flotante y renderizado de tablas.
   - `TablasCalculoController`: Responsable única y exclusivamente del transporte HTTP, negociación de contenido y respuesta de códigos de estado.
   - `TablasCalculoDTOs`: Responsables de transportar datos y validar restricciones de entrada (`Range`).
2. **OCP (Open/Closed Principle - Principio de Abierto/Cerrado)**:
   - Los contratos e interfaces permiten incorporar nuevas evaluaciones numéricas o formatos de exportación (ej. CSV, Markdown, LaTeX) extendiendo la interfaz sin modificar el núcleo de cálculo existente.
3. **LSP (Liskov Substitution Principle - Principio de Sustitución de Liskov)**:
   - Cualquier implementación que cumpla con `ITablasCalculoService` puede sustituir a `TablasCalculoService` sin alterar el comportamiento esperado por los consumidores.
4. **ISP (Interface Segregation Principle - Principio de Segregación de Interfaces)**:
   - `ITablasCalculoService` define métodos cohesivos orientados únicamente al cómputo y formateo tabular, sin mezclar dependencias de persistencia relacional o identidad.
5. **DIP (Dependency Inversion Principle - Principio de Inversión de Dependencias)**:
   - `TablasCalculoController` depende exclusivamente de la abstracción `ITablasCalculoService`. La inyección se resuelve a través del contenedor IoC de ASP.NET Core (`builder.Services.AddScoped<ITablasCalculoService, TablasCalculoService>()`).

### 4. Muestras de Resultados y Visualización ASCII

#### Tabla de Potencias (Base 2, Exponentes 0 a 10):
```text
+------------+------------+--------------------+------------------------------+
| Exponente  |    Base    |     Resultado      | Expresión Matemática         |
+------------+------------+--------------------+------------------------------+
|          0 |       2.00 |             1.0000 | 2 ^ 0 = 1                    |
|          1 |       2.00 |             2.0000 | 2 ^ 1 = 2                    |
|          2 |       2.00 |             4.0000 | 2 ^ 2 = 4                    |
|          3 |       2.00 |             8.0000 | 2 ^ 3 = 8                    |
|          4 |       2.00 |            16.0000 | 2 ^ 4 = 16                   |
|          5 |       2.00 |            32.0000 | 2 ^ 5 = 32                   |
|          6 |       2.00 |            64.0000 | 2 ^ 6 = 64                   |
|          7 |       2.00 |           128.0000 | 2 ^ 7 = 128                  |
|          8 |       2.00 |           256.0000 | 2 ^ 8 = 256                  |
|          9 |       2.00 |           512.0000 | 2 ^ 9 = 512                  |
|         10 |       2.00 |         1,024.0000 | 2 ^ 10 = 1024                |
+------------+------------+--------------------+------------------------------+
```

#### Tabla Trigonométrica (0° a 360°, paso 45°):
```text
+-------------+-------------+------------+------------+------------+--------------------------+
| Ángulo (°)  | Ángulo(rad) |    Seno    |   Coseno   |  Tangente  | Observación              |
+-------------+-------------+------------+------------+------------+--------------------------+
|         0.0 |      0.0000 |     0.0000 |     1.0000 |     0.0000 | Cero exacto              |
|        45.0 |      0.7854 |     0.7071 |     0.7071 |     1.0000 | Definido                 |
|        90.0 |      1.5708 |     1.0000 |     0.0000 |        N/D | Indeterminado (asíntota) |
|       135.0 |      2.3562 |     0.7071 |    -0.7071 |    -1.0000 | Definido                 |
|       180.0 |      3.1416 |     0.0000 |    -1.0000 |     0.0000 | Cero exacto              |
|       225.0 |      3.9270 |    -0.7071 |    -0.7071 |     1.0000 | Definido                 |
|       270.0 |      4.7124 |    -1.0000 |     0.0000 |        N/D | Indeterminado (asíntota) |
|       315.0 |      5.4978 |    -0.7071 |     0.7071 |    -1.0000 | Definido                 |
|       360.0 |      6.2832 |     0.0000 |     1.0000 |     0.0000 | Cero exacto              |
+-------------+-------------+------------+------------+------------+--------------------------+
```

