USE [PropertyManagerDB]
GO
/****** Object:  User [IIS APPPOOL\DefaultAppPool]    Script Date: 23/01/2024 10:48:52 ******/
CREATE USER [IIS APPPOOL\DefaultAppPool] FOR LOGIN [IIS APPPOOL\DefaultAppPool] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [IIS APPPOOL\PropertyManagerAPI]    Script Date: 23/01/2024 10:48:52 ******/
CREATE USER [IIS APPPOOL\PropertyManagerAPI] FOR LOGIN [IIS APPPOOL\PropertyManagerAPI] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [IIS APPPOOL\PropertyManagerFL]    Script Date: 23/01/2024 10:48:52 ******/
CREATE USER [IIS APPPOOL\PropertyManagerFL] FOR LOGIN [IIS APPPOOL\PropertyManagerFL] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [IIS APPPOOL\DefaultAppPool]
GO
ALTER ROLE [db_owner] ADD MEMBER [IIS APPPOOL\PropertyManagerAPI]
GO
ALTER ROLE [db_owner] ADD MEMBER [IIS APPPOOL\PropertyManagerFL]
GO
/****** Object:  Schema [Identity]    Script Date: 23/01/2024 10:48:52 ******/
CREATE SCHEMA [Identity]
GO
