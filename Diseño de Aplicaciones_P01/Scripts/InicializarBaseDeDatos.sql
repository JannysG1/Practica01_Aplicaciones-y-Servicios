-- ============================================================================
-- SCRIPT DE INICIALIZACIÓN, CONDICIONES PREVIAS Y VALIDACIONES DE BASE DE DATOS
-- Base de datos: CafeteriaDb (y opcionalmente AplicacionesP01Db)
-- ============================================================================

-- 1. CREACIÓN DE BASE DE DATOS
IF DB_ID(N'CafeteriaDb') IS NULL
BEGIN
	CREATE DATABASE CafeteriaDb;
END
GO

USE CafeteriaDb;
GO

-- 2. TABLA: Usuarios CON RESTRICCIONES Y VALIDACIONES A NIVEL DE BDD
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
	-- Asegurar restricciones si la tabla ya existía
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
GO

-- 3. TABLA: Productos CON RESTRICCIONES Y VALIDACIONES
IF OBJECT_ID(N'dbo.Productos', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Productos
	(
		Id INT IDENTITY(1,1) NOT NULL,
		Nombre NVARCHAR(100) NOT NULL,
		Precio DECIMAL(18,2) NOT NULL,
		Activo BIT NOT NULL CONSTRAINT DF_Productos_Activo DEFAULT (1),
		CONSTRAINT PK_Productos PRIMARY KEY (Id),
		CONSTRAINT CK_Productos_Precio CHECK (Precio >= 0),
		CONSTRAINT CK_Productos_Nombre CHECK (LEN(TRIM(Nombre)) >= 2)
	);
END
ELSE
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
GO

-- 4. SEMBRADO DE USUARIOS INICIALES
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE CorreoElectronicousuario = N'admin@cafeteria.com')
BEGIN
	INSERT INTO dbo.Usuarios (NombreUsuario, CorreoElectronicousuario, Contrasenausuario, Activo)
	VALUES (N'Administrador General', N'admin@cafeteria.com', N'admin123', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE CorreoElectronicousuario = N'ana@correo.com')
BEGIN
	INSERT INTO dbo.Usuarios (NombreUsuario, CorreoElectronicousuario, Contrasenausuario, Activo)
	VALUES (N'Ana Torres', N'ana@correo.com', N'ana123', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE CorreoElectronicousuario = N'marco@correo.com')
BEGIN
	INSERT INTO dbo.Usuarios (NombreUsuario, CorreoElectronicousuario, Contrasenausuario, Activo)
	VALUES (N'Marco Aurelio', N'marco@correo.com', N'marco123', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE CorreoElectronicousuario = N'maria@correo.com')
BEGIN
	INSERT INTO dbo.Usuarios (NombreUsuario, CorreoElectronicousuario, Contrasenausuario, Activo)
	VALUES (N'Maria Antonieta', N'maria@correo.com', N'maria123', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE CorreoElectronicousuario = N'carlos@correo.com')
BEGIN
	INSERT INTO dbo.Usuarios (NombreUsuario, CorreoElectronicousuario, Contrasenausuario, Activo)
	VALUES (N'Carlos Perez', N'carlos@correo.com', N'1234', 1);
END
GO

-- 5. SEMBRADO DE PRODUCTOS INICIALES
IF NOT EXISTS (SELECT 1 FROM dbo.Productos WHERE Nombre = N'Café Americano')
BEGIN
	INSERT INTO dbo.Productos (Nombre, Precio, Activo)
	VALUES (N'Café Americano', 1.50, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Productos WHERE Nombre = N'Capuchino')
BEGIN
	INSERT INTO dbo.Productos (Nombre, Precio, Activo)
	VALUES (N'Capuchino', 2.50, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Productos WHERE Nombre = N'Espresso Doble')
BEGIN
	INSERT INTO dbo.Productos (Nombre, Precio, Activo)
	VALUES (N'Espresso Doble', 2.80, 1);
END
GO

-- 6. VERIFICACIÓN FINAL
SELECT 'Usuarios' AS Tabla, COUNT(*) AS TotalRegistros FROM dbo.Usuarios
UNION ALL
SELECT 'Productos' AS Tabla, COUNT(*) AS TotalRegistros FROM dbo.Productos;
GO
