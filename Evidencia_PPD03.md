# Evidencia de Persistencia y Pruebas - Guía ITID803_02 (PPD03)

## Arquitectura en Capas, Interfaces y Persistencia

- **Estudiante**: Jannys Garrido
- **Institución**: Escuela Politécnica Nacional (EPN)
- **Materia**: ITID803_02 – Diseño de Aplicaciones y Servicios Web
- **Fecha**: 24/11/2026

---

## 1. Cumplimiento de la Estructura en Capas

El proyecto implementa la separación estricta de responsabilidades requerida en la rúbrica del documento PPD03:

```text
Diseño de Aplicaciones_P01/
├── Interfaces/
│   ├── IProveedorRepository.cs  (GetAllAsync, GetByIdAsync, AddAsync, SaveChangesAsync)
│   ├── IProveedorService.cs     (CrearProveedorAsync, GetAllAsync, GetByIdAsync)
│   └── ...
├── Repositories/
│   └── ProveedorRepository.cs   (Implementa IProveedorRepository usando CafeteriaDbContext)
├── Services/
│   └── ProveedorService.cs      (Implementa IProveedorService inyectando IProveedorRepository)
├── Controllers/
│   └── ProveedoresController.cs (Inyecta IProveedorService, POST con [Authorize(Roles = "Admin")])
└── Program.cs                   (Registro con builder.Services.AddScoped)
```

---

## 2. Registro de Inyección de Dependencias (`Program.cs`)

En `Program.cs` se registraron las interfaces y sus clases concretas:

```csharp
// Requerimiento Guía ITID803_02 (PPD03): Registro desacoplado de Repositorio y Servicio de Proveedores
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IProveedorService, ProveedorService>();
```

---

## 3. Guion de Pruebas y Evidencias de Persistencia

### Paso 1: Autenticación como Administrador
Se envía petición `POST /api/Auth/login` con credenciales de administrador para obtener el token JWT:
- **Respuesta**: HTTP `200 OK` con token portador conteniendo el claim `"role": "Admin"`.

### Paso 2: Creación de Proveedor desde Swagger / HTTP Client
- **Método**: `POST /api/Proveedores`
- **Encabezado**: `Authorization: Bearer <Token_JWT_Admin>`
- **Payload Enviado**:
```json
{
  "nombre": "Distribuidora La Costa",
  "telefono": "0991234567",
  "activo": true
}
```

- **Respuesta Obtenida**: HTTP `201 Created`
- **Header Location**: `https://localhost:7083/api/Proveedores/1`
- **Cuerpo de Respuesta**:
```json
{
  "id": 1,
  "nombre": "Distribuidora La Costa",
  "telefono": "0991234567",
  "activo": true
}
```

### Paso 3: Evidencia de Persistencia en SQL Server (`dbo.Proveedores`)
Consulta directa ejecutada sobre la base de datos `CafeteriaDb` en `(localdb)\MSSQLLocalDB`:

```sql
SELECT Id, Nombre, Telefono, Activo FROM dbo.Proveedores;
```

**Resultado de la consulta**:
```text
Id          Nombre                                                                                               Telefono             Activo
----------- ---------------------------------------------------------------------------------------------------- -------------------- ------
          1 Distribuidora La Costa                                                                               0991234567                1

(1 rows affected)
```

---

## 4. Verificación de Reglas de Negocio en la Capa de Servicio

1. **Unicidad de Nombre (409 Conflict)**:
   Al intentar registrar nuevamente un proveedor con el nombre `"Distribuidora La Costa"`, `ProveedorService` detecta la duplicidad y retorna:
   ```json
   {
     "mensaje": "Ya existe un proveedor registrado con el nombre 'Distribuidora La Costa'."
   }
   ```
2. **Validación de Longitud de Nombre (400 Bad Request)**:
   Si el nombre tiene menos de 2 caracteres, el servicio rechaza la solicitud antes de interactuar con la base de datos:
   ```json
   {
     "mensaje": "El nombre del proveedor debe tener al menos 2 caracteres."
   }
   ```
3. **Control de Autorización (403 Forbidden / 401 Unauthorized)**:
   - Usuarios sin token reciben `401 Unauthorized`.
   - Usuarios con rol de cliente/usuario reciben `403 Forbidden`.
   - Únicamente usuarios con rol `Admin` pueden realizar el registro y modificación.
