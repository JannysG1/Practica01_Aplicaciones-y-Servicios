IF DB_ID(N'AplicacionesP01Db') IS NULL
BEGIN
	CREATE DATABASE AplicacionesP01Db;
END
GO

USE AplicacionesP01Db;
GO

IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NOT NULL
BEGIN
	DROP TABLE dbo.Usuarios;
END
GO

CREATE TABLE dbo.Usuarios
(
	IdUsuario INT IDENTITY(1,1) NOT NULL,
	NombreUsuario NVARCHAR(100) NOT NULL,
	CorreoElectronicousuario NVARCHAR(150) NOT NULL,
	Contrasenausuario NVARCHAR(255) NOT NULL,
	Activo BIT NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT (1),
	CONSTRAINT PK_Usuarios PRIMARY KEY (IdUsuario)
);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE CorreoElectronicousuario = N'ana@correo.com')
BEGIN
	INSERT INTO dbo.Usuarios
		(NombreUsuario, CorreoElectronicousuario, Contrasenausuario, Activo)
	VALUES
		(N'Ana Torres', N'ana@correo.com', N'ana123', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE CorreoElectronicousuario = N'marco@correo.com')
BEGIN
	INSERT INTO dbo.Usuarios
		(NombreUsuario, CorreoElectronicousuario, Contrasenausuario, Activo)
	VALUES
		(N'Marco Aurelio', N'marco@correo.com', N'marco123', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE CorreoElectronicousuario = N'maria@correo.com')
BEGIN
	INSERT INTO dbo.Usuarios
		(NombreUsuario, CorreoElectronicousuario, Contrasenausuario, Activo)
	VALUES
		(N'Maria Antonieta', N'maria@correo.com', N'maria123', 1);
END
GO

SELECT * FROM dbo.Usuarios;
GO
