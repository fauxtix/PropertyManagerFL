USE [PropertyManagerDB]
GO
/****** Object:  StoredProcedure [dbo].[tools_CS_SPROC_Builder]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[tools_CS_SPROC_Builder] 
(
@objName nvarchar(100)
)
AS
/*
___________________________________________________________________
Name:  		CS SPROC Builder
Version: 	1
Date:    	10/09/2004
Author:  	Paul McKenzie
Description: 	Call this stored procedue passing the name of your 
  database object that you wish to insert/update
  from .NET (C#) and the code returns code to copy
  and paste into your application.  This version is
  for use with "Microsoft Data Application Block".
  
Version: 	1.1
Date:	 	17/02/2006
Author:	 	Paul McKenzie
Description:	a) Updated to include 'UniqueIdentifier' Data Type
		b) Support for 'ParameterDirection.Output'

*/
SET NOCOUNT ON
DECLARE @parameterCount int
DECLARE @errMsg varchar(100)
DECLARE @parameterAt varchar(1)
DECLARE @connName varchar(100)
DECLARE @outputValues varchar(100)
--Change the following variable to the name of your connection instance
SET @connName='conn.Connection'
SET @parameterAt=''
SET @outputValues=''
SELECT 
 	dbo.sysobjects.name AS ObjName, 
 	dbo.sysobjects.xtype AS ObjType,
 	dbo.syscolumns.name AS ColName, 
 	dbo.syscolumns.colorder AS ColOrder, 
 	dbo.syscolumns.length AS ColLen, 
 	dbo.syscolumns.colstat AS ColKey, 
 	dbo.syscolumns.isoutparam AS ColIsOut,
 	dbo.systypes.xtype
INTO #t_obj
FROM         
 	dbo.syscolumns INNER JOIN
 	dbo.sysobjects ON dbo.syscolumns.id = dbo.sysobjects.id INNER JOIN
 	dbo.systypes ON dbo.syscolumns.xtype = dbo.systypes.xtype
WHERE     
 	(dbo.sysobjects.name = @objName) 
 	AND 
 	(dbo.systypes.status <> 1) 
ORDER BY 
 	dbo.sysobjects.name, 
 	dbo.syscolumns.colorder

SET @parameterCount=(SELECT count(*) FROM #t_obj)
IF(@parameterCount<1) SET @errMsg='No Parameters/Fields found for ' + @objName
IF(@errMsg is null)
	BEGIN
  		PRINT 'try'
  		PRINT '   {'
  		PRINT '   SqlParameter[] paramsToStore = new SqlParameter[' + cast(@parameterCount as varchar) + '];'
  		PRINT ''
  
  		DECLARE @source_name nvarchar,@source_type varchar,
    			@col_name nvarchar(100),@col_order int,@col_type varchar(20),
    			@col_len int,@col_key int,@col_xtype int,@col_redef varchar(20), @col_isout tinyint
 
  		DECLARE cur CURSOR FOR
  		SELECT * FROM #t_obj
  		OPEN cur
  		-- Perform the first fetch.
  		FETCH NEXT FROM cur INTO @source_name,@source_type,@col_name,@col_order,@col_len,@col_key,@col_isout,@col_xtype
 
  			if(@source_type=N'U') SET @parameterAt='@'
  			-- Check @@FETCH_STATUS to see if there are any more rows to fetch.
  			WHILE @@FETCH_STATUS = 0
  				BEGIN
   				SET @col_redef=(SELECT CASE @col_xtype
					WHEN 34 THEN 'Image'
					WHEN 35 THEN 'Text'
					WHEN 36 THEN 'UniqueIdentifier'
					WHEN 48 THEN 'TinyInt'
					WHEN 52 THEN 'SmallInt'
					WHEN 56 THEN 'Int'
					WHEN 58 THEN 'SmallDateTime'
					WHEN 59 THEN 'Real'
					WHEN 60 THEN 'Money'
					WHEN 61 THEN 'DateTime'
					WHEN 62 THEN 'Float'
					WHEN 99 THEN 'NText'
					WHEN 104 THEN 'Bit'
					WHEN 106 THEN 'Decimal'
					WHEN 122 THEN 'SmallMoney'
					WHEN 127 THEN 'BigInt'
					WHEN 165 THEN 'VarBinary'
					WHEN 167 THEN 'VarChar'
					WHEN 173 THEN 'Binary'
					WHEN 175 THEN 'Char'
					WHEN 231 THEN 'NVarChar'
					WHEN 239 THEN 'NChar'
					ELSE '!MISSING'
					END AS C) 

				--Write out the parameter
				PRINT '   paramsToStore[' + cast(@col_order-1 as varchar) 
				    + '] = new SqlParameter("' + @parameterAt + @col_name
				    + '", SqlDbType.' + @col_redef
				    + ');'

				--Write out the parameter direction it is output
				IF(@col_isout=1)
					BEGIN
						PRINT '   paramsToStore['+ cast(@col_order-1 as varchar) +'].Direction=ParameterDirection.Output;'
						SET @outputValues=@outputValues+'   ?=paramsToStore['+ cast(@col_order-1 as varchar) +'].Value;'
					END
					ELSE
					BEGIN
						--Write out the parameter value line
   						PRINT '   paramsToStore['+ cast(@col_order-1 as varchar) + '].Value = ?;'
					END
				--If the type is a string then output the size declaration
				IF(@col_xtype=231)OR(@col_xtype=167)OR(@col_xtype=175)OR(@col_xtype=99)OR(@col_xtype=35)
					BEGIN
						PRINT '   paramsToStore[' + cast(@col_order-1 as varchar) + '].Size=' + cast(@col_len as varchar) + ';'
					END

				 -- This is executed as long as the previous fetch succeeds.
      			FETCH NEXT FROM cur INTO @source_name,@source_type,@col_name,@col_order, @col_len,@col_key,@col_isout,@col_xtype 
  	END
  PRINT ''
  PRINT '   SqlHelper.ExecuteNonQuery(' + @connName + ', CommandType.StoredProcedure,"' + @objName + '", paramsToStore);'
  PRINT @outputValues
  PRINT '   }'
  PRINT 'catch(Exception excp)'
  PRINT '   {'
  PRINT '   }'
  PRINT 'finally'
  PRINT '   {'
  PRINT '   ' + @connName + '.Dispose();'
  PRINT '   ' + @connName + '.Close();'
  PRINT '   }'  
  CLOSE cur
  DEALLOCATE cur
 END
if(LEN(@errMsg)>0) PRINT @errMsg
DROP TABLE #t_obj
SET NOCOUNT ON

GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_AutoRenewed]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_AutoRenewed] (@Id int)

AS
BEGIN
	SET NOCOUNT OFF;
	SELECT RenovacaoAutomatica FROM Arrendamentos WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_CheckForPayments]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_CheckForPayments] (@Id int) -- id da fração
AS

SET NOCOUNT ON;
SELECT	COUNT(1)
FROM    Recebimentos
WHERE	ID_Propriedade = @Id -- ID_Propriedade = Id da Fração
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Delete]
(
	@Id int
)
AS
	SET NOCOUNT OFF;
	DELETE FROM [Arrendamentos] WHERE [Id] = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_ExtendLeaseTerm]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_ExtendLeaseTerm]
(
	@Id int
)
AS
	SET NOCOUNT OFF;
	DECLARE @Prazo int
	DECLARE @NovaData DateTime
	DECLARE @Data_Fim DateTime
	SELECT @Prazo = Prazo, @Data_Fim = Data_Fim FROM Arrendamentos WHERE Id = @Id
	SELECT @NovaData = DATEADD(year, @Prazo, @Data_Fim)

	UPDATE [Arrendamentos] 
		SET Data_Fim = @NovaData
	WHERE [Id] = @Id 
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Get_ApplicableLaws]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Get_ApplicableLaws]
AS

SET NOCOUNT ON;
SELECT Id, DiplomaLegal Descricao 
FROM CoefientesAtualizacaoRendas
ORDER BY id DESC
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Get_CurrentRentCoefficient]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Get_CurrentRentCoefficient] (@Ano nvarchar(4))
AS

--- falta o select para 'Porta'!
SET NOCOUNT ON;
SELECT Coeficiente 
FROM CoefientesAtualizacaoRendas
WHERE Ano = @Ano
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Get_LastPaymentDate]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Get_LastPaymentDate]
(
	@Id int -- Unit Id

)
AS
BEGIN
	SET NOCOUNT OFF;
	SELECT [Data_Pagamento]
	FROM Arrendamentos
	WHERE [ID_Fracao] = @Id 
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Get_RentCoefficient_ById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Get_RentCoefficient_ById] (@Id int)
AS

--- falta o select para 'Porta'!
SET NOCOUNT ON;
SELECT	Id, Ano, Coeficiente, DiplomaLegal, UrlDiploma, 
		Lei, DataPublicacao
FROM CoefientesAtualizacaoRendas
WHERE Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Get_RentCoefficient_ByYear]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Get_RentCoefficient_ByYear] (@Year int)
AS

--- falta o select para 'Porta'!
SET NOCOUNT ON;
SELECT	Id, Ano, Coeficiente, DiplomaLegal, UrlDiploma, 
		Lei, DataPublicacao
FROM CoefientesAtualizacaoRendas
WHERE Ano = @Year
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Get_RentCoefficients]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Get_RentCoefficients] 
AS

--- falta o select para 'Porta'!
SET NOCOUNT ON;
SELECT	Id, Ano, Coeficiente, DiplomaLegal, UrlDiploma,
		Lei, DataPublicacao
FROM CoefientesAtualizacaoRendas
ORDER BY Ano Desc
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_GetAll]
AS
	SET NOCOUNT ON;
SELECT	A.Id, Data_Inicio, Data_Fim, Data_Pagamento, Fiador, 
		Prazo_Meses, Prazo, Valor_Renda, Doc_IRS, Doc_Vencimento, A.Notas, 
		ID_Fracao, ID_Inquilino, ID_Fiador, Caucao, ContratoEmitido, 
		EnvioCartaAtualizacaoRenda, DataEnvioCartaAtualizacao,
		DocumentoAtualizacaoGerado, DocumentoGerado, 
		EnvioCartaRevogacao, DocumentoRevogacaoGerado, DataEnvioCartaRevogacao, DataRespostaCartaRevogacao, RespostaCartaRevogacao,
		EnvioCartaAtrasoRenda, DocumentoAtrasoRendaGerado, DataEnvioCartaAtrasoRenda, RespostaCartaAtrasoRenda, DataRespostaCartaAtrasoRenda,
		Data_Saida, FormaPagamento, A.Ativo, A.LeiVigente, ArrendamentoNovo, EstadoPagamento, RenovacaoAutomatica,
		I.Nome NomeInquilino, F.Descricao Fracao
FROM    Arrendamentos A INNER JOIN Inquilinos I ON
			A.ID_Inquilino = I.Id
		INNER JOIN Fracoes F ON
			A.ID_Fracao = F.Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_GetById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_GetById] (@Id int)
AS

--- falta o select para 'Porta'!
SET NOCOUNT ON;
SELECT	A.Id, Data_Inicio, Data_Fim, Data_Pagamento, Fiador, 
		Prazo_Meses, Prazo, Valor_Renda, Doc_IRS, Doc_Vencimento, A.Notas, 
		EnvioCartaAtualizacaoRenda, DataEnvioCartaAtualizacao,
		EnvioCartaRevogacao, DocumentoRevogacaoGerado, DataEnvioCartaRevogacao, DataRespostaCartaRevogacao, RespostaCartaRevogacao,
		EnvioCartaAtrasoRenda, DocumentoAtrasoRendaGerado, DataEnvioCartaAtrasoRenda, RespostaCartaAtrasoRenda, DataRespostaCartaAtrasoRenda,
		DocumentoAtualizacaoGerado, ID_Fracao, ID_Inquilino, ID_Fiador, 
		Caucao, ContratoEmitido, DocumentoGerado, Data_Saida, LeiVigente,
		FormaPagamento, A.Ativo, ArrendamentoNovo, EstadoPagamento, RenovacaoAutomatica,
		I.Nome NomeInquilino, F.Descricao Fracao
FROM    Arrendamentos A INNER JOIN Inquilinos I ON
			A.ID_Inquilino = I.Id
		INNER JOIN Fracoes F ON
			A.ID_Fracao = F.Id
WHERE A.Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_GetGenerated_Document]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_GetGenerated_Document] (@Id int)

AS
BEGIN
	SET NOCOUNT OFF;
	SELECT DocumentoGerado
	FROM Arrendamentos 
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_GetLastId]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_GetLastId] 
AS

SET NOCOUNT ON;

SELECT	TOP 1 Id
FROM    Arrendamentos 
ORDER BY Id DESC
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_GetPropertyUnitsWithNoLease]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_GetPropertyUnitsWithNoLease] (@Id int) -- Id imóvel
AS

BEGIN
	SELECT	Fracoes.Id, Fracoes.Descricao, Imoveis.Id, Imoveis.Descricao
	FROM   Fracoes INNER JOIN Imoveis
		ON Fracoes.Id_Imovel = Imoveis.Id
	WHERE Fracoes.Id NOT IN(SELECT ID_Fracao  FROM Arrendamentos) 
		AND Imoveis.Id = @Id
END

GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_GetRevocation_Generated_Document]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_GetRevocation_Generated_Document] (@Id int)

AS
BEGIN
	SET NOCOUNT OFF;
	SELECT DocumentoRevogacaoGerado
	FROM Arrendamentos 
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_GetTenantDataToUpdateRents]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_GetTenantDataToUpdateRents] (@Id int)
AS

SET NOCOUNT ON;

SELECT Data_Inicio, ID_Fracao
FROM    Arrendamentos
WHERE ID_Inquilino = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_GetUnitId]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_GetUnitId] (@Id int)
AS

SET NOCOUNT ON;

SELECT	ID_Fracao
FROM    Arrendamentos 
WHERE Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_IncomeReceived]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_IncomeReceived]

AS
BEGIN
	SET NOCOUNT OFF;
	SELECT SUM(Valor_Renda)
	FROM Arrendamentos
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Insert]
(
	@Data_Inicio datetime,
	@Data_Fim datetime,
	@Data_Pagamento datetime,
	@SaldoInicial decimal(10,2),
	@Fiador bit,
	@Prazo_Meses int,
	@Prazo int,
	@Valor_Renda decimal(10, 2),
	@Doc_IRS bit,
	@Doc_Vencimento bit,
	@Notas text,
	@ID_Fracao int,
	@ID_Inquilino int,
	@ID_Fiador int,
	@Caucao bit,
	@ContratoEmitido bit,
	@DocumentoGerado varchar(255),
	@Data_Saida datetime,
	@FormaPagamento int,
	@Ativo bit,
	@ArrendamentoNovo bit,
	@EstadoPagamento varchar(15),
	@RenovacaoAutomatica bit,
	@LeiVigente varchar(100)

)
AS
	SET NOCOUNT OFF;
	INSERT INTO [Arrendamentos]
			(
				[Data_Inicio], [Data_Fim], [Data_Pagamento], [SaldoInicial], [Fiador], 
				[Prazo_Meses], [Prazo], [Valor_Renda], [Doc_IRS], [Doc_Vencimento], [Notas], 
				[ID_Fracao], [ID_Inquilino], [ID_Fiador], [Caucao], 
				[ContratoEmitido], [DocumentoGerado], 
				[Data_Saida], [FormaPagamento], [Ativo], [LeiVigente],
				[ArrendamentoNovo], [EstadoPagamento], [RenovacaoAutomatica]
			) 
	VALUES (	@Data_Inicio, @Data_Fim, @Data_Pagamento, @SaldoInicial, @Fiador, 
				@Prazo_Meses, @Prazo, @Valor_Renda, @Doc_IRS, @Doc_Vencimento, @Notas, 
				@ID_Fracao, @ID_Inquilino, @ID_Fiador,@Caucao,
				@ContratoEmitido, @DocumentoGerado,
				@Data_Saida, @FormaPagamento, @Ativo, @LeiVigente,
				@ArrendamentoNovo, @EstadoPagamento, @RenovacaoAutomatica);	

	SELECT CAST(SCOPE_IDENTITY() as int);
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Insert_RentCoefficient]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Insert_RentCoefficient]
(
	@Ano nvarchar(4),
	@Coeficiente float,
	@DiplomaLegal nvarchar(120),
	@UrlDiploma nvarchar(256),
	@Lei nvarchar(100), 
	@DataPublicacao nvarchar(30)
)
AS

BEGIN
	SET NOCOUNT ON;
	INSERT INTO CoefientesAtualizacaoRendas(Ano, Coeficiente, DiplomaLegal, 
				UrlDiploma, Lei, DataPublicacao)
	VALUES (@Ano, @Coeficiente, @DiplomaLegal, @UrlDiploma, 
			@Lei, @DataPublicacao)

	SELECT CAST(SCOPE_IDENTITY() as int);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Insert_UpdateRentsLetterProcess]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Insert_UpdateRentsLetterProcess]
(
	@Ano int,
	@DataProcessamento datetime
)
AS

SET NOCOUNT OFF;
INSERT INTO [ProcessamentoAtualizacaoRendas] 
	([Ano], [DataProcessamento])
VALUES 
	(@Ano, @DataProcessamento)
	
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_RentalExist]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_RentalExist] (@Id int)

AS
BEGIN
	SET NOCOUNT OFF;
	SELECT COUNT(1) FROM Arrendamentos 
	WHERE ID_Fracao = @Id AND Ativo = 1
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_SetLateRentPaymentLetter_Issued]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_SetLateRentPaymentLetter_Issued]
(
	@Id int,
	@GeneratedDocument varchar(256)
)
AS
BEGIN
	SET NOCOUNT OFF;
	UPDATE [Arrendamentos] SET EnvioCartaAtrasoRenda = 1, 
		DocumentoAtrasoRendaGerado = @GeneratedDocument
	WHERE [Id] = @Id 
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_SetLateRentPaymentLetter_NotIssued]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_SetLateRentPaymentLetter_NotIssued]
(
	@Id int,
	@GeneratedDocument varchar(256)
)
AS
BEGIN
	SET NOCOUNT OFF;
	UPDATE [Arrendamentos] SET EnvioCartaAtrasoRenda = 0, 
		DocumentoAtrasoRendaGerado = @GeneratedDocument
	WHERE [Id] = @Id 
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_SetLease_As_Ended]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_SetLease_As_Ended]
(
	@Id int,
	@LeaveDate datetime
)
AS
BEGIN
	SET NOCOUNT OFF;
	UPDATE [Arrendamentos] SET Data_Saida = @LeaveDate, Ativo = 0
	WHERE [Id] = @Id 
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_SetLease_As_Issued]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_SetLease_As_Issued]
(
	@Id int,
	@GeneratedDocument varchar(256)
)
AS
BEGIN
	SET NOCOUNT OFF;
	UPDATE [Arrendamentos] SET ContratoEmitido = 1, DocumentoGerado = @GeneratedDocument
	WHERE [Id] = @Id 
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_SetLease_NotIssued]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_SetLease_NotIssued] (@Id int)

AS
BEGIN
	SET NOCOUNT OFF;
	UPDATE [Arrendamentos] SET ContratoEmitido = 0
	WHERE [Id] = @Id 
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_SetUpdateRentsLetter_Issued]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_SetUpdateRentsLetter_Issued]
(
	@Id int,
	@GeneratedDocument varchar(256)
)
AS
BEGIN
	SET NOCOUNT OFF;
	UPDATE [Arrendamentos] SET EnvioCartaAtualizacaoRenda = 1, 
		DataEnvioCartaAtualizacao = GetDate(),
		DocumentoAtualizacaoGerado = @GeneratedDocument
	WHERE [Id] = @Id 
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_SetUpdateRentsLetter_NotIssued]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_SetUpdateRentsLetter_NotIssued]
(
	@Id int
)
AS
BEGIN
	SET NOCOUNT OFF;
	UPDATE [Arrendamentos] SET EnvioCartaAtualizacaoRenda = 0 
	WHERE [Id] = @Id 
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* 
	= 11/03/2023 =
	Retirada data do último pagamento (Data_Pag) da lista de parâmetros
	Caso não haja pagamentos, data vem como "01/01/0001" => inválida
	Data é atualizada quando há 'processamento mensal de rendas', p.exº.
*/
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Update]
(
	@Id int,
	@Data_Inicio datetime,
	@Data_Fim datetime,
	@Fiador bit,
	@Prazo_Meses int,
	@Prazo int,
	@Valor_Renda decimal(10, 2),
	@Doc_IRS bit,
	@Doc_Vencimento bit,
	@Notas text,
	@ID_Fracao int,
	@ID_Inquilino int,
	@ID_Fiador int,
	@Caucao bit,
	@ContratoEmitido bit,
	@DocumentoGerado varchar(255),
	@Data_Saida datetime,
	@FormaPagamento int,
	@Ativo bit,
	@LeiVigente varchar(100),
	@ArrendamentoNovo bit,
	@EstadoPagamento varchar(15),
	@RenovacaoAutomatica bit
)
AS
	SET NOCOUNT OFF;
UPDATE [Arrendamentos] 
SET [Data_Inicio] = @Data_Inicio, [Data_Fim] = @Data_Fim,
	[Fiador] = @Fiador, [Prazo_Meses] = @Prazo_Meses, [Prazo] = @Prazo,
	[Valor_Renda] = @Valor_Renda, [Doc_IRS] = @Doc_IRS, 
	[Doc_Vencimento] = @Doc_Vencimento, [Notas] = @Notas, [ID_Fracao] = @ID_Fracao, [ID_Inquilino] = @ID_Inquilino, 	
	[ID_Fiador] = @ID_Fiador, [Caucao] = @Caucao, [ContratoEmitido] = @ContratoEmitido, [DocumentoGerado] = @DocumentoGerado,
	[Data_Saida] = @Data_Saida, [FormaPagamento] = @FormaPagamento, [Ativo] = @Ativo, [ArrendamentoNovo] = @ArrendamentoNovo,
	[EstadoPagamento] = @EstadoPagamento, [RenovacaoAutomatica] = @RenovacaoAutomatica, LeiVigente = @LeiVigente
WHERE [Id] = @Id 
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Update_Issued_Flag__ToDelete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Update_Issued_Flag__ToDelete]
(
	@Id int,
	@Issued bit
)
AS
	SET NOCOUNT OFF;
	UPDATE [Arrendamentos] SET ContratoEmitido = @Issued
	WHERE [Id] = @Id 
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Update_LastPaymentDate]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Update_LastPaymentDate]
(
	@Id int, -- Unit Id
	@date datetime,
	@estadopagamento varchar(15)
)
AS
BEGIN
	SET NOCOUNT OFF;
	UPDATE [Arrendamentos] SET [Data_Pagamento] = @date,
	EstadoPagamento = @estadopagamento
	WHERE [ID_Fracao] = @Id 
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Update_LateRentPaymentLetter]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Update_LateRentPaymentLetter]
(
	@Id int,
	@EnvioCartaAtrasoRenda bit, 
	@DocumentoAtrasoRendaGerado varchar(255),
	@DataEnvioCartaAtrasoRenda datetime
)
AS
	SET NOCOUNT OFF;
	UPDATE [Arrendamentos] 
	SET [EnvioCartaAtrasoRenda] = @EnvioCartaAtrasoRenda, 
		[DocumentoAtrasoRendaGerado] = @DocumentoAtrasoRendaGerado,
		[DataEnvioCartaAtrasoRenda] = @DataEnvioCartaAtrasoRenda
	WHERE [Id] = @Id 
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Update_RentCoefficient]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Update_RentCoefficient]
(
	@Id int,
	@Ano nvarchar(4),
	@Coeficiente float,
	@DiplomaLegal nvarchar(120),
	@UrlDiploma nvarchar(256),
	@Lei nvarchar(100),
	@DataPublicacao nvarchar(30)

)
AS

BEGIN
	SET NOCOUNT ON;
	UPDATE CoefientesAtualizacaoRendas
	SET Ano = @Ano, Coeficiente = @Coeficiente, 
	DiplomaLegal = @DiplomaLegal, UrlDiploma = @UrlDiploma,
	Lei = @Lei, DataPublicacao = @DataPublicacao
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Update_RevocationLetter]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Update_RevocationLetter]
(
	@Id int,
	@EnvioCartaRevogacao bit, 
	@DocumentoRevogacaoGerado varchar(255),
	@DataEnvioCartaRevogacao datetime
)
AS
	SET NOCOUNT OFF;
	UPDATE [Arrendamentos] 
	SET [EnvioCartaRevogacao] = @EnvioCartaRevogacao, 
		[DocumentoRevogacaoGerado] = @DocumentoRevogacaoGerado,
		[DataEnvioCartaRevogacao] = @DataEnvioCartaRevogacao
	WHERE [Id] = @Id 
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_UpdateRevocationLetter_AnswerDate]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_UpdateRevocationLetter_AnswerDate]
(
	@Id int,
	@AnswerDate datetime
)
AS
BEGIN
	SET NOCOUNT OFF;
	UPDATE [Arrendamentos] SET DataRespostaCartaRevogacao = @AnswerDate
	WHERE [Id] = @Id 
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Was_LateRentPaymentLetter_Answered]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Was_LateRentPaymentLetter_Answered] (@Id int)

AS
BEGIN
	SET NOCOUNT OFF;
	SELECT RespostaCartaAtrasoRenda
	FROM Arrendamentos 
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Was_LateRentPaymentLetter_Issued]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Was_LateRentPaymentLetter_Issued] (@Id int)

AS
BEGIN
	SET NOCOUNT OFF;
	SELECT EnvioCartaAtrasoRenda
	FROM Arrendamentos 
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Was_Lease_Issued]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Was_Lease_Issued] (@Id int)

AS
BEGIN
	SET NOCOUNT OFF;
	SELECT ContratoEmitido 
	FROM Arrendamentos 
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Was_RevocationLetter_Answered]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Was_RevocationLetter_Answered] (@Id int)

AS
BEGIN
	SET NOCOUNT OFF;
	SELECT RespostaCartaRevogacao 
	FROM Arrendamentos 
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Was_RevocationLetter_Issued]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Was_RevocationLetter_Issued] (@Id int)

AS
BEGIN
	SET NOCOUNT OFF;
	SELECT EnvioCartaRevogacao 
	FROM Arrendamentos 
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Arrendamentos_Was_UpdateRentsLetter_Issued]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Arrendamentos_Was_UpdateRentsLetter_Issued] (@Ano int)

AS
BEGIN
	SET NOCOUNT OFF;
	SELECT COUNT(1)
	FROM ProcessamentoAtualizacaoRendas
	WHERE Ano = @Ano
END

GO
/****** Object:  StoredProcedure [dbo].[usp_CategoriaDespesa_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CategoriaDespesa_Delete]
(
	@Original_Id int,
	@Original_Descricao nvarchar(200)
)
AS
	SET NOCOUNT OFF;
DELETE FROM [CategoriaDespesa] WHERE (([Id] = @Original_Id) AND ([Descricao] = @Original_Descricao))
GO
/****** Object:  StoredProcedure [dbo].[usp_CategoriaDespesa_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CategoriaDespesa_GetAll]
AS
	SET NOCOUNT ON;
SELECT        *
FROM            CategoriaDespesa
GO
/****** Object:  StoredProcedure [dbo].[usp_CategoriaDespesa_GetById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CategoriaDespesa_GetById] (@Id int)
AS

SET NOCOUNT ON;
SELECT *
FROM	CategoriaDespesa
WHERE	Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_CategoriaDespesa_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CategoriaDespesa_Insert]
(
	@Descricao nvarchar(200)
)
AS
	SET NOCOUNT OFF;
	INSERT INTO [CategoriaDespesa] ([Descricao]) VALUES (@Descricao);
	
GO
/****** Object:  StoredProcedure [dbo].[usp_CategoriaDespesa_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CategoriaDespesa_Update]
(
	@Id int,
	@Descricao nvarchar(200)
)
AS
	SET NOCOUNT OFF;
	UPDATE [CategoriaDespesa] 
	SET [Id] = @Id, [Descricao] = @Descricao WHERE [Id] = @Id;
GO
/****** Object:  StoredProcedure [dbo].[usp_CC_Inquilinos_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CC_Inquilinos_Delete]
(
	@Id int
)
AS

SET NOCOUNT OFF;
DELETE FROM [CC_Inquilinos] 
WHERE Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_CC_Inquilinos_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CC_Inquilinos_GetAll]

AS

SET NOCOUNT OFF;
SELECT	CC_I.Id, DataMovimento, ValorPago, ValorEmDivida,
		Renda, ID_TipoRecebimento, CC_I.Notas,
		IdInquilino, I.Nome NomeInquilino, CC_I.Id TransactionId
FROM [CC_Inquilinos] CC_I
	INNER JOIN Inquilinos I ON
		CC_I.IdInquilino = I.Id
GO
/****** Object:  StoredProcedure [dbo].[usp_CC_Inquilinos_GetById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CC_Inquilinos_GetById]
(
	@Id int
)
AS

SET NOCOUNT OFF;
SELECT	CC_I.Id, DataMovimento, ValorPago, ValorEmDivida, 
		Renda, ID_TipoRecebimento,	CC_I.Notas,
		IdInquilino, I.Nome NomeInquilino, CC_I.Id TransactionId
FROM [CC_Inquilinos] CC_I
INNER JOIN Inquilinos I ON
	CC_I.IdInquilino = I.Id
WHERE CC_I.Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_CC_Inquilinos_GetByTenantId]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CC_Inquilinos_GetByTenantId]
(
	@TenantId int
)
AS

SET NOCOUNT OFF;
SELECT	CC_I.Id, DataMovimento, ValorPago, ValorEmDivida, 
		Renda, ID_TipoRecebimento,	CC_I.Notas,
		IdInquilino, I.Nome NomeInquilino, CC_I.Id TransactionId
FROM [CC_Inquilinos] CC_I
INNER JOIN Inquilinos I ON
	CC_I.IdInquilino = I.Id
WHERE CC_I.IdInquilino = @TenantId
GO
/****** Object:  StoredProcedure [dbo].[usp_CC_Inquilinos_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CC_Inquilinos_Insert]
(
	@DataMovimento datetime,
	@ValorPago decimal(10,2),
	@ValorEmDivida decimal(10,2),
	@Renda bit,
	@ID_TipoRecebimento int,
	@Notas varchar(512),
	@IdInquilino int,
	@TransactionId int
)
AS

SET NOCOUNT OFF;
INSERT INTO [CC_Inquilinos] 
	(DataMovimento, ValorPago, ValorEmDivida, Renda, ID_TipoRecebimento, Notas, IdInquilino, TransactionId)
VALUES 
	(@DataMovimento, @ValorPago, @ValorEmDivida, @Renda, @ID_TipoRecebimento, @Notas, @IdInquilino, @TransactionId);
	
SELECT CAST(SCOPE_IDENTITY() as int);
GO
/****** Object:  StoredProcedure [dbo].[usp_CC_Inquilinos_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CC_Inquilinos_Update]
(
	@Id int,
	@DataMovimento datetime,
	@ValorPago decimal(10,2),
	@ValorEmDivida decimal(10,2),
	@Renda bit,
	@ID_TipoRecebimento int,
	@Notas text,
	@IdInquilino int,
	@TransactionId int
)
AS

SET NOCOUNT OFF;
UPDATE [CC_Inquilinos] 
	SET DataMovimento = @DataMovimento, ValorPago = @ValorPago, ValorEmDivida = @ValorEmDivida,
	Renda = @Renda, ID_TipoRecebimento = @ID_TipoRecebimento,
	Notas = @Notas, IdInquilino = @IdInquilino,
	TransactionId = @TransactionId
WHERE Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Contactos_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Contactos_Delete]
(
	@Id int
)
AS
	SET NOCOUNT OFF;
	DELETE FROM [Contactos] WHERE [Id] = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Contactos_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Contactos_GetAll]
AS
	SET NOCOUNT ON;
SELECT  CO.Id, Nome, Morada, Localidade, Contacto, eMail, CO.Notas, ID_TipoContacto, TC.Descricao TipoContacto
FROM    Contactos CO
		INNER JOIN TipoContacto TC ON
			CO.ID_TipoContacto = TC.Id
		
GO
/****** Object:  StoredProcedure [dbo].[usp_Contactos_GetById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Contactos_GetById] (@Id int)
AS

SET NOCOUNT ON;
SELECT  CO.Id, Nome, Morada, Localidade, Contacto, eMail, CO.Notas, ID_TipoContacto, TC.Descricao TipoContacto
FROM    Contactos CO
		INNER JOIN TipoContacto TC ON
			CO.ID_TipoContacto = TC.Id
WHERE CO.Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Contactos_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Contactos_Insert]
(
	@Nome varchar(70),
	@Morada varchar(70),
	@Localidade varchar(50),
	@Contacto varchar(50),
	@eMail varchar(128),
	@Notas text,
	@ID_TipoContacto int
)
AS
SET NOCOUNT OFF;
INSERT INTO [Contactos] ([Nome], [Morada], [Localidade], [Contacto], 
		[eMail], [Notas], [ID_TipoContacto]) 
VALUES (@Nome, @Morada, @Localidade, @Contacto, 
		@eMail, @Notas, @ID_TipoContacto);
	
SELECT CAST(SCOPE_IDENTITY() as int);
GO
/****** Object:  StoredProcedure [dbo].[usp_Contactos_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Contactos_Update]
(
	@Id int,
	@Nome varchar(70),
	@Morada varchar(70),
	@Localidade varchar(50),
	@Contacto varchar(50),
	@eMail varchar(128),
	@Notas text,
	@ID_TipoContacto int
)
AS
SET NOCOUNT OFF;
UPDATE [Contactos] 
	SET [Nome] = @Nome, [Morada] = @Morada, [Localidade] = @Localidade, 
	[Contacto] = @Contacto, [eMail] = @eMail, [Notas] = @Notas,
	[ID_TipoContacto] = @ID_TipoContacto
WHERE [Id] = @Id;

SELECT  CO.Id, Nome, Morada, Localidade, Contacto, eMail, CO.Notas, ID_TipoContacto, TC.Descricao TipoContacto
FROM    Contactos CO
		INNER JOIN TipoContacto TC ON
			CO.ID_TipoContacto = TC.Id
WHERE CO.Id = @Id	
GO
/****** Object:  StoredProcedure [dbo].[usp_Contratos_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Contratos_Delete]
(
	@Id int
)
AS
	SET NOCOUNT OFF;
	DELETE FROM [Contratos] WHERE [Id] = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Contratos_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Contratos_GetAll]
AS

SET NOCOUNT ON;
SELECT	Id, DataCelebracao, Data_Inicio, Fiador, Prazo_Meses, Valor_Renda, 
		Doc_IRS, Doc_Vencimento, Notas, ID_Fracao, ID_Inquilino, ID_Fiador, Caucao
FROM	Contratos
GO
/****** Object:  StoredProcedure [dbo].[usp_Contratos_GetById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Contratos_GetById] (@Id int)
AS

SET NOCOUNT ON;
SELECT	Id, DataCelebracao, Data_Inicio, Fiador, Prazo_Meses, Valor_Renda, 
		Doc_IRS, Doc_Vencimento, Notas, ID_Fracao, ID_Inquilino, ID_Fiador, Caucao
FROM	Contratos
WHERE	Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Contratos_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Contratos_Insert]
(
	@DataCelebracao datetime,
	@Data_Inicio datetime,
	@Fiador bit,
	@Prazo_Meses int,
	@Valor_Renda decimal(10, 2),
	@Doc_IRS bit,
	@Doc_Vencimento bit,
	@Notas text,
	@ID_Fracao int,
	@ID_Inquilino int,
	@ID_Fiador int,
	@Caucao bit
)
AS

SET NOCOUNT OFF;
INSERT INTO [Contratos] 
	([DataCelebracao], [Data_Inicio], [Fiador], [Prazo_Meses], [Valor_Renda], [Doc_IRS], 
	[Doc_Vencimento], [Notas], [ID_Fracao], [ID_Inquilino], [ID_Fiador], [Caucao]) 
VALUES 
	(@DataCelebracao, @Data_Inicio, @Fiador, @Prazo_Meses, @Valor_Renda, @Doc_IRS, 
	@Doc_Vencimento, @Notas, @ID_Fracao, @ID_Inquilino, @ID_Fiador, @Caucao);
	
GO
/****** Object:  StoredProcedure [dbo].[usp_Contratos_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Contratos_Update]
(
	@Id int,
	@DataCelebracao datetime,
	@Data_Inicio datetime,
	@Fiador bit,
	@Prazo_Meses int,
	@Valor_Renda decimal(10, 2),
	@Doc_IRS bit,
	@Doc_Vencimento bit,
	@Notas text,
	@ID_Fracao int,
	@ID_Inquilino int,
	@ID_Fiador int,
	@Caucao bit
)
AS

SET NOCOUNT OFF;
UPDATE [Contratos] 
SET [DataCelebracao] = @DataCelebracao, [Data_Inicio] = @Data_Inicio, [Fiador] = @Fiador, 
	[Prazo_Meses] = @Prazo_Meses, [Valor_Renda] = @Valor_Renda, [Doc_IRS] = @Doc_IRS,
	[Doc_Vencimento] = @Doc_Vencimento, [Notas] = @Notas, [ID_Fracao] = @ID_Fracao,
	[ID_Inquilino] = @ID_Inquilino, [ID_Fiador] = @ID_Fiador, [Caucao] = @Caucao 
WHERE [Id] = @Id;
	
GO
/****** Object:  StoredProcedure [dbo].[usp_Despesas_Categorias_GetExpensesByYear]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[usp_Despesas_Categorias_GetExpensesByYear]
	@year int
AS
BEGIN
	SELECT MONTH(Datamovimento), DATENAME(month, d.DataMovimento) Descricao, 
	SUM(D.Valor_Pago) TotalDespesas, COUNT(1) NumeroMovimentos
	FROM Despesas D
	WHERE YEAR(D.DataMovimento) = @year
	GROUP BY MONTH(DataMovimento), DATENAME(month, d.DataMovimento) 
	ORDER BY MONTH(DataMovimento)
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Despesas_Categorias_GetSum]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[usp_Despesas_Categorias_GetSum]
	@year int
AS
BEGIN
	SELECT C.Descricao, SUM(D.Valor_Pago) TotalDespesas, COUNT(1) NumeroMovimentos
	FROM Despesas D
	INNER JOIN CategoriaDespesa C
	ON D.ID_CategoriaDespesa = C.Id
	WHERE YEAR(D.DataMovimento) = @year
	GROUP BY C.Descricao
	ORDER BY SUM(D.Valor_Pago) DESC
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Despesas_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Despesas_Delete]
(
	@Id int
)
AS
	SET NOCOUNT OFF;
	DELETE FROM [Despesas] WHERE [Id] = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Despesas_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Despesas_GetAll]
AS

BEGIN
	SET NOCOUNT ON;
	SELECT  D.Id, DataMovimento, Valor_Pago, ID_TipoDespesa, ID_ModoPagamento, 
			D.Id_CategoriaDespesa, NumeroDocumento, Notas,
			TD.Descricao TipoDespesa, CD.Descricao DescrImputacao, 
			FP.Descricao ModoPagamento
	FROM    Despesas D 
		INNER JOIN TipoDespesa TD ON
			D.ID_TipoDespesa = TD.Id
		INNER JOIN CategoriaDespesa CD ON
			TD.Id_CategoriaDespesa = CD.Id
		INNER JOIN FormaPagamento FP ON
			D.ID_ModoPagamento = FP.Id

END
GO
/****** Object:  StoredProcedure [dbo].[usp_Despesas_GetById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Despesas_GetById] (@Id int)
AS

BEGIN
	SET NOCOUNT ON;
	SELECT  D.Id, DataMovimento, Valor_Pago, ID_TipoDespesa, ID_ModoPagamento, NumeroDocumento,
			Notas,TD.Descricao TipoDespesa, CD.Descricao DescrImputacao, FP.Descricao ModoPagamento,
			D.Id_CategoriaDespesa
	FROM    Despesas D 
		INNER JOIN TipoDespesa TD ON
			D.ID_TipoDespesa = TD.Id
		INNER JOIN CategoriaDespesa CD ON
			TD.Id_CategoriaDespesa = CD.Id
		INNER JOIN FormaPagamento FP ON
			D.ID_ModoPagamento = FP.Id
						
	WHERE D.Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Despesas_GetMostSpendCategories_ByYear]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_Despesas_GetMostSpendCategories_ByYear]
(
	@year int
)
AS
BEGIN

	SELECT c.Descricao, count(1) NumeroMovimentos, sum(d.Valor_Pago) TotalDespesas
	FROM CategoriaDespesa c 
	INNER JOIN Despesas d
		ON c.Id = d.ID_CategoriaDespesa
	WHERE YEAR(d.DataMovimento) = @year
	GROUP BY c.Descricao
	ORDER BY sum(d.Valor_Pago) DESC
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Despesas_GetMostSpendCategory]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_Despesas_GetMostSpendCategory]

AS
BEGIN

	SELECT TOP 2
		c.Descricao, sum(d.Valor_Pago) as TotalDespesas, COUNT(1) NumeroMovimentos
	FROM CategoriaDespesa c 
	INNER JOIN Despesas d
		ON c.id = d.ID_CategoriaDespesa
	WHERE YEAR(d.DataMovimento) = YEAR(GetDate())
	GROUP BY c.Descricao
	ORDER BY sum(d.Valor_Pago) DESC
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Despesas_GetTipoDespesaByCategory]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Despesas_GetTipoDespesaByCategory] (@Id int)
AS

BEGIN
	SET NOCOUNT ON;
	SELECT id, Descricao 
	FROM TipoDespesa
	WHERE Id_CategoriaDespesa = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Despesas_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Despesas_Insert]
(
	@DataMovimento datetime,
	@Valor_Pago decimal(10, 2),
	@NumeroDocumento nvarchar(50),
	@ID_CategoriaDespesa int,
	@ID_TipoDespesa int,
	@ID_ModoPagamento int,
	@Notas text
)
AS

SET NOCOUNT OFF;
INSERT INTO [Despesas]
	([DataMovimento], [Valor_Pago], [ID_ModoPagamento], [NumeroDocumento],
	[ID_TipoDespesa], [ID_CategoriaDespesa], [Notas]) 
VALUES 
	(@DataMovimento, @Valor_Pago, @ID_ModoPagamento, @NumeroDocumento,
	@ID_TipoDespesa, @ID_CategoriaDespesa, @Notas);
	
SELECT CAST(SCOPE_IDENTITY() as int);
GO
/****** Object:  StoredProcedure [dbo].[usp_Despesas_TipoDespesa_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Despesas_TipoDespesa_Delete]
(
	@Id int
)
AS

SET NOCOUNT OFF;
DELETE FROM  TipoDespesa
WHERE Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Despesas_TipoDespesa_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Despesas_TipoDespesa_GetAll]

AS

SET NOCOUNT OFF;
SELECT TD.Id, TD.Descricao, TD.Id_CategoriaDespesa, CD.Descricao CategoriaDespesa
FROM  TipoDespesa TD 
INNER JOIN CategoriaDespesa CD ON
	TD.Id_CategoriaDespesa = CD.Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Despesas_TipoDespesa_GetById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Despesas_TipoDespesa_GetById]
(
	@Id int
)
AS

SET NOCOUNT OFF;
SELECT TD.Id, TD.Descricao, TD.Id_CategoriaDespesa, CD.Descricao CategoriaDespesa
FROM  TipoDespesa TD 
INNER JOIN CategoriaDespesa CD ON
	TD.Id_CategoriaDespesa = CD.Id
WHERE TD.Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Despesas_TipoDespesa_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Despesas_TipoDespesa_Insert]
(
	@Descricao nvarchar(255),
	@Id_CategoriaDespesa int
)
AS

SET NOCOUNT OFF;
INSERT INTO TipoDespesa
	(Descricao, [Id_CategoriaDespesa])
VALUES 
	(@Descricao, @Id_CategoriaDespesa);
	
SELECT CAST(SCOPE_IDENTITY() as int);
GO
/****** Object:  StoredProcedure [dbo].[usp_Despesas_TipoDespesa_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Despesas_TipoDespesa_Update]
(
	@Id int,
	@Descricao nvarchar(255),
	@Id_CategoriaDespesa int
)
AS

SET NOCOUNT OFF;
UPDATE TipoDespesa
	 SET [Descricao] = @Descricao, [Id_CategoriaDespesa] = @Id_CategoriaDespesa
WHERE Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Despesas_TipoDespesas_CheckDeleteConstraint]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Despesas_TipoDespesas_CheckDeleteConstraint] (@Id int, @OkToDelete bit OUTPUT)


AS

declare @CanDelete bit

SET NOCOUNT ON;
SELECT	@CanDelete = COUNT(1)
FROM    Despesas
WHERE	ID_TipoDespesa = @Id

SELECT @OkToDelete = @CanDelete
GO
/****** Object:  StoredProcedure [dbo].[usp_Despesas_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Despesas_Update]
(
	@Id int,
	@DataMovimento datetime,
	@Valor_Pago decimal(10, 2),
	@NumeroDocumento nvarchar(50),
	@ID_TipoDespesa int,
	@ID_CategoriaDespesa int,
	@ID_ModoPagamento int,
	@Notas text
)
AS

SET NOCOUNT OFF;
UPDATE [Despesas] 
SET [DataMovimento] = @DataMovimento, [Valor_Pago] = @Valor_Pago, 
	[NumeroDocumento] = @NumeroDocumento, [ID_ModoPagamento] = @ID_ModoPagamento,
	[ID_TipoDespesa] = @ID_TipoDespesa, [ID_CategoriaDespesa] = @ID_CategoriaDespesa,
	[Notas] = @Notas 
WHERE [Id] = @Id;
	
GO
/****** Object:  StoredProcedure [dbo].[usp_Development_CloneRecebimentos]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_Development_CloneRecebimentos]
AS
BEGIN
	INSERT INTO Recebimentos(DataMovimento, estado, ID_Propriedade, 
		ID_Inquilino, ID_TipoRecebimento, GeradoPeloPrograma, renda, ValorPrevisto, 
		ValorEmFalta, ValorRecebido, notas)
	SELECT DataMovimento, estado, ID_Propriedade, 
		ID_Inquilino, ID_TipoRecebimento, GeradoPeloPrograma, renda, ValorPrevisto, 
		ValorEmFalta, ValorRecebido, notas
		FROM RecebimentosTemp;

	INSERT INTO ProcessamentoRendas(Mes, Ano, DataProcessamento)
	SELECT Mes, Ano, DataProcessamento FROM ProcessamentoRendas;

END
GO
/****** Object:  StoredProcedure [dbo].[usp_Development_initializeRentPaymentTables]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Fausto Luis
-- Create date: 2023-01-09
-- Description:	Inicializa tabelas
-- =============================================
CREATE PROCEDURE [dbo].[usp_Development_initializeRentPaymentTables] 
AS
BEGIN
	SET NOCOUNT ON;
	truncate table PMlogs
	truncate table ProcessamentoRendas;
	truncate table ProcessamentoAtualizacaoRendas;
	truncate table CC_Inquilinos;
	truncate table DocumentosInquilino;
	truncate table Recebimentos;
	truncate table RecebimentosTemp;
	truncate table HistoricoAtualizacaoRendas;
	Update Inquilinos SET SaldoCorrente = 0, Notas = ''
	Update Arrendamentos SET 
	Data_Pagamento = Cast(Cast(dateadd(month, -1, GETDATE()) as Date) as Datetime),
	EnvioCartaAtrasoRenda = NULL, 
	DocumentoAtrasoRendaGerado = NULL, 
	DataEnvioCartaAtrasoRenda = NULL,
	DataEnvioCartaAtualizacao = NULL,
	DataEnvioCartaRevogacao = NULL,
	DocumentoAtualizacaoGerado = NULL,
	EnvioCartaAtualizacaoRenda = NULL,
	EstadoPagamento = 'Ok';
	update Fracoes set ValorRenda = 0 WHERE Id = 1; -- JJM - 3E
	update Fracoes set ValorRenda = 600 WHERE Id = 2; --JJM 3D
	update Fracoes set ValorRenda = 420 WHERE Id = 3; -- JJM 2E
	update Fracoes set ValorRenda = 275 WHERE Id = 8; -- escritório
	update Fracoes set ValorRenda = 450 WHERE Id = 9; -- CGG B
	update Fracoes set ValorRenda = 325 WHERE Id = 11; -- PBC 105
	update Fracoes set ValorRenda = 400 WHERE Id = 12; -- CGG C
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Development_initializeWorkTables]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Fausto Luis
-- Create date: 2023-01-09
-- Description:	Inicializa tabelas - exceções: Inquilinos, Fracoes, Proprietário
--				
-- =============================================
CREATE PROCEDURE [dbo].[usp_Development_initializeWorkTables] 
AS
BEGIN
	SET NOCOUNT ON;
	truncate table PMLogs;
	truncate table ProcessamentoRendas;
	truncate table ProcessamentoAtualizacaoRendas;
	truncate table CC_Inquilinos;
	TRUNCATE TABLE Despesas;
	TRUNCATE TABLE Recebimentos;
	TRUNCATE TABLE RecebimentosTemp;
	TRUNCATE TABLE DocumentosInquilino;
	TRUNCATE TABLE Contratos;
	TRUNCATE TABLE Arrendamentos;

	UPDATE Inquilinos SET SaldoCorrente = 0, Notas = ''
	UPDATE Fracoes SET Situacao = 2;

END
GO
/****** Object:  StoredProcedure [dbo].[usp_Development_ResetRentsYearUpdate]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Fausto Luis
-- Create date: 2023-01-09
-- Description:	Inicializa tabelas
-- =============================================
CREATE PROCEDURE [dbo].[usp_Development_ResetRentsYearUpdate] 
AS
BEGIN
	SET NOCOUNT ON;
	truncate table ProcessamentoAtualizacaoRendas;
	Update Arrendamentos SET EnvioCartaAtualizacaoRenda = 0, DocumentoAtualizacaoGerado = ''

	-- Atualizar manualmente o valor das rendas das frações

END
GO
/****** Object:  StoredProcedure [dbo].[usp_Documents_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Documents_Delete]
(
	@Id int
)
AS

SET NOCOUNT OFF;
DELETE FROM [Documents] WHERE [Id] = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Documents_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Documents_GetAll]
AS

SET NOCOUNT ON;
SELECT	D.Id, Title, D.[Description], IsPublic, [URL], D.CreatedBy, D.LocalUpload,
		D.CreatedOn, D.LastModifiedBy, D.LastModifiedOn, DocumentTypeId,
		DT.[Name] DocumentType, DTC.Descricao DocumentCategory
FROM	Documents D
	INNER JOIN DocumentTypes DT ON
		D.DocumentTypeId = DT.Id
	INNER JOIN DocumentTypeCategories DTC ON
		DT.TypeCategoryId = DTC.Id
ORDER BY D.CreatedOn DESC
GO
/****** Object:  StoredProcedure [dbo].[usp_Documents_GetById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Documents_GetById] (@Id int)
AS

SET NOCOUNT ON;
SELECT	D.Id, Title, D.[Description], IsPublic, [URL], D.CreatedBy, D.LocalUpload,
		D.CreatedOn, D.LastModifiedBy, D.LastModifiedOn, DocumentTypeId, DT.[Name]DocumentType,
		 DTC.Id [DocumentCategoryId]
FROM	Documents D
INNER JOIN DocumentTypes DT ON
		D.DocumentTypeId = DT.Id
			INNER JOIN DocumentTypeCategories DTC ON
		DT.TypeCategoryId = DTC.Id

WHERE	D.Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Documents_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Documents_Insert]
(
	@Title nvarchar(256),
	@Description nvarchar(512),
	@IsPublic bit,
	@URL nvarchar(256),
	@LocalUpload bit,
	@CreatedBy nvarchar(70),
	@CreatedOn datetime,
	@LastModifiedBy nvarchar(70),
	@LastModifiedOn datetime,
	@DocumentTypeId int,
	@DocumentCategoryId int
)
AS

SET NOCOUNT OFF;
INSERT INTO [Documents] 
	([Title], [Description], [IsPublic], [URL], [CreatedBy], [LocalUpload],
	[CreatedOn], [LastModifiedBy], [LastModifiedOn], [DocumentTypeId], [DocumentCategoryId])
VALUES 
	(@Title, @Description, @IsPublic, @URL, @CreatedBy, @LocalUpload,
	@CreatedOn, @LastModifiedBy, @LastModifiedOn, @DocumentTypeId, @DocumentCategoryId);
	
SELECT CAST(SCOPE_IDENTITY() as int);
GO
/****** Object:  StoredProcedure [dbo].[usp_Documents_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Documents_Update]
(
	@Id int,
	@Title nvarchar(256),
	@Description nvarchar(512),
	@URL nvarchar(256),
	@LocalUpload bit, 
	@IsPublic bit,
	@LastModifiedBy nvarchar(70),
	@LastModifiedOn datetime

)
AS

SET NOCOUNT OFF;
UPDATE [Documents] 
SET [Title] = @Title, [Description] = @Description, [IsPublic] = @IsPublic,
	[URL] = @URL, [LocalUpload] = @LocalUpload,
	[LastModifiedBy] = @LastModifiedBy, [LastModifiedOn] = @LastModifiedOn
WHERE [Id] = @Id;

SELECT	D.Id, Title, D.[Description], IsPublic, [URL], D.CreatedBy, d.LocalUpload,
		D.CreatedOn, D.LastModifiedBy, D.LastModifiedOn, DocumentTypeId, DT.Description DocumentType
FROM	Documents D
INNER JOIN DocumentTypes DT ON
		D.DocumentTypeId = DT.Id
WHERE	D.Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_DocumentTypes_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_DocumentTypes_Delete]
(
	@Id int
)
AS

SET NOCOUNT OFF;
DELETE FROM [DocumentTypes] WHERE [Id] = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_DocumentTypes_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_DocumentTypes_GetAll]
AS

SET NOCOUNT ON;
SELECT	Id, [Name], [Description], CreatedBy, CreatedOn, 
		LastModifiedBy, LastModifiedOn, TypeCategoryId
FROM   DocumentTypes
GO
/****** Object:  StoredProcedure [dbo].[usp_DocumentTypes_GetById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_DocumentTypes_GetById] (@Id int)
AS

SET NOCOUNT ON;
SELECT	Id, [Name], [Description], CreatedBy, CreatedOn, 
		LastModifiedBy, LastModifiedOn, TypeCategoryId
FROM	DocumentTypes
WHERE	Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_DocumentTypes_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_DocumentTypes_Insert]
(
	@Name nvarchar(128),
	@Description nvarchar(512),
	@CreatedBy nvarchar(70),
	@CreatedOn datetime,
	@LastModifiedBy nvarchar(70),
	@LastModifiedOn datetime
)
AS

SET NOCOUNT OFF;
INSERT INTO [DocumentTypes] 
	([Name], [Description], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn])
VALUES 
	(@Name, @Description, @CreatedBy, @CreatedOn, @LastModifiedBy, @LastModifiedOn);
	
GO
/****** Object:  StoredProcedure [dbo].[usp_DocumentTypes_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_DocumentTypes_Update]
(
	@Id int,
	@Name nvarchar(128),
	@Description nvarchar(512),
	@CreatedBy nvarchar(70),
	@CreatedOn datetime,
	@LastModifiedBy nvarchar(70),
	@LastModifiedOn datetime
)
AS

SET NOCOUNT OFF;
UPDATE [DocumentTypes] 
SET [Name] = @Name, [Description] = @Description, [CreatedBy] = @CreatedBy, 
	[CreatedOn] = @CreatedOn, [LastModifiedBy] = @LastModifiedBy, 
	[LastModifiedOn] = @LastModifiedOn WHERE [Id] = @Id;
	
GO
/****** Object:  StoredProcedure [dbo].[usp_EstadoCivil_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_EstadoCivil_Delete]
(
	@Id int
)
AS

SET NOCOUNT OFF;
DELETE FROM [EstadoCivil] WHERE [Id] = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_EstadoCivil_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_EstadoCivil_GetAll]
AS

SET NOCOUNT ON;
SELECT Id, Descricao
FROM   EstadoCivil
GO
/****** Object:  StoredProcedure [dbo].[usp_EstadoCivil_GetById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_EstadoCivil_GetById] (@Id int)
AS

SET NOCOUNT ON;
SELECT	Id, Descricao
FROM	EstadoCivil
WHERE	Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_EstadoCivil_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_EstadoCivil_Insert]
(
	@Descricao varchar(30)
)
AS

SET NOCOUNT OFF;
INSERT INTO [EstadoCivil] ([Descricao]) VALUES (@Descricao);
GO
/****** Object:  StoredProcedure [dbo].[usp_EstadoCivil_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_EstadoCivil_Update]
(
	@Id int,
	@Descricao varchar(30)
)
AS

SET NOCOUNT OFF;
UPDATE [EstadoCivil] SET [Descricao] = @Descricao WHERE [Id] = @Id;
	
GO
/****** Object:  StoredProcedure [dbo].[usp_EstadoConservacao_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_EstadoConservacao_Delete]
(
	@Id int
)
AS

SET NOCOUNT OFF;
DELETE FROM [EstadoConservacao] WHERE [Id] = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_EstadoConservacao_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_EstadoConservacao_GetAll]
AS

SET NOCOUNT ON;
SELECT Id, Descricao
FROM   EstadoConservacao
GO
/****** Object:  StoredProcedure [dbo].[usp_EstadoConservacao_GetById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_EstadoConservacao_GetById] (@Id int)
AS

SET NOCOUNT ON;
SELECT	Id, Descricao
FROM	EstadoConservacao
WHERE	Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_EstadoConservacao_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_EstadoConservacao_Insert]
(
	@Descricao varchar(30)
)
AS

SET NOCOUNT OFF;
INSERT INTO [EstadoConservacao] ([Descricao]) VALUES (@Descricao);
	
GO
/****** Object:  StoredProcedure [dbo].[usp_EstadoConservacao_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_EstadoConservacao_Update]
(
	@Id int,
	@Descricao varchar(30)
)
AS

SET NOCOUNT OFF;
UPDATE [EstadoConservacao] SET [Descricao] = @Descricao WHERE [Id] = @Id;
	
GO
/****** Object:  StoredProcedure [dbo].[usp_Fiadores_CheckDeleteConstraint]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- Antes de apagar fiador, verificar se há contratos associados ao mesmo
-- Se a operação resultar em 0 (zero), é válido apagar fiador

CREATE PROCEDURE [dbo].[usp_Fiadores_CheckDeleteConstraint] (@Id int, @OkToDelete bit OUTPUT)


AS

declare @CanDelete bit

SET NOCOUNT ON;
SELECT	@CanDelete = COUNT(1)
FROM    Arrendamentos
WHERE	ID_Fiador = @Id

SELECT @OkToDelete = @CanDelete
GO
/****** Object:  StoredProcedure [dbo].[usp_Fiadores_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_Fiadores_Delete]
(
	@Id int
)
AS

SET NOCOUNT OFF;
DELETE FROM [Fiadores] 
WHERE [Id] = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Fiadores_Get_ByIdExtended]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[usp_Fiadores_Get_ByIdExtended] (@Id int)
AS

SET NOCOUNT ON;
SELECT	I.Id, I.IdInquilino, Nome, Morada, I.ID_EstadoCivil, EC.Descricao EstadoCivil,
		Contacto1, Contacto2, NIF, Identificacao, ValidadeCC, 
		eMail, IRSAnual, Vencimento, Notas, Ativo
FROM    Fiadores I 
		INNER JOIN EstadoCivil EC ON
			I.ID_EstadoCivil = EC.Id		
WHERE	I.Id = @Id



GO
/****** Object:  StoredProcedure [dbo].[usp_Fiadores_Get_ByTenant]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[usp_Fiadores_Get_ByTenant] (@Id int) -- Id Inquilino
AS

SET NOCOUNT ON;
SELECT	F.Id, IdInquilino, Nome, Morada, ID_EstadoCivil, EC.Descricao EstadoCivil,
		Contacto1, Contacto2, NIF, Identificacao, ValidadeCC, 
		eMail, IRSAnual, Vencimento, Notas, Ativo
FROM    Fiadores F 
		INNER JOIN EstadoCivil EC ON
			F.ID_EstadoCivil = EC.Id		
WHERE	F.IdInquilino = @Id



GO
/****** Object:  StoredProcedure [dbo].[usp_Fiadores_Get_Disponiveis]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[usp_Fiadores_Get_Disponiveis]
AS

SET NOCOUNT ON;
SELECT inq.Id, inq.Nome Descricao 
FROM Fiadores inq 
WHERE inq.Id NOT IN(SELECT ID_Fiador FROM Arrendamentos) 
ORDER BY Inq.Nome


GO
/****** Object:  StoredProcedure [dbo].[usp_Fiadores_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_Fiadores_GetAll] (@Titular int) -- 0 = Fiadors e Fiadores, 1 = apenas Fiadors
AS

SET NOCOUNT ON;
SELECT	I.Id, I.IdInquilino, Nome, Morada, I.ID_EstadoCivil, EC.Descricao EstadoCivil,
		Contacto1, Contacto2, NIF, Identificacao, ValidadeCC, 
		eMail, IRSAnual, Vencimento, Notas, Ativo
FROM    Fiadores I INNER JOIN 
			EstadoCivil EC ON
				I.ID_EstadoCivil = ec.Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Fiadores_GetById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[usp_Fiadores_GetById] (@Id int)
AS

SET NOCOUNT ON;
SELECT	Id, IdInquilino, Nome, Morada, ID_EstadoCivil, Contacto1, Contacto2, eMail,
		IRSAnual, Vencimento, Notas, NIF, Identificacao, ValidadeCC
FROM    Fiadores
WHERE	Id = @Id

GO
/****** Object:  StoredProcedure [dbo].[usp_Fiadores_GetForLookup]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[usp_Fiadores_GetForLookup]
AS

SET NOCOUNT ON;

SELECT inq.Id, inq.Nome Descricao 
FROM Fiadores inq 
ORDER BY Inq.Nome


GO
/****** Object:  StoredProcedure [dbo].[usp_Fiadores_GetNome]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[usp_Fiadores_GetNome] (@Id int)
AS

SET NOCOUNT ON;
SELECT	Nome
FROM    Fiadores
WHERE	Id = @Id



GO
/****** Object:  StoredProcedure [dbo].[usp_Fiadores_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_Fiadores_Insert]
(
	@IdInquilino int,
	@Nome varchar(60),
	@Morada varchar(60),
	@ID_EstadoCivil int,
	@Contacto1 varchar(20),
	@Contacto2 varchar(20),
	@eMail varchar(128),
	@IRSAnual decimal(10, 2),
	@Vencimento decimal(10, 2),
	@Notas text,
	@NIF varchar(9),
	@Identificacao varchar(20),
	@ValidadeCC datetime
)
AS

SET NOCOUNT OFF;
INSERT INTO [Fiadores] 
	([IdInquilino], [Nome], [Morada], 
	[ID_EstadoCivil], [Contacto1], [Contacto2], [eMail],
	[IRSAnual], [Vencimento], [Notas], [NIF], [Identificacao], [ValidadeCC]) 
	VALUES (@IdInquilino, @Nome, @Morada, @ID_EstadoCivil, @Contacto1, @Contacto2, @eMail, 
	@IRSAnual, @Vencimento,	@Notas, @NIF, @Identificacao, @ValidadeCC);
	
SELECT CAST(SCOPE_IDENTITY() as int);



GO
/****** Object:  StoredProcedure [dbo].[usp_Fiadores_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_Fiadores_Update]
(
	@Id int,
	@IdInquilino int,
	@Nome varchar(60),
	@Morada varchar(60),
	@ID_EstadoCivil int,
	@Contacto1 varchar(20),
	@Contacto2 varchar(20),
	@eMail varchar(128),
	@IRSAnual decimal(10, 2),
	@Vencimento decimal(10, 2),
	@Notas text,
	@NIF varchar(9),
	@Identificacao varchar(20),
	@ValidadeCC datetime
)
AS

SET NOCOUNT OFF;
UPDATE [Fiadores] 
SET [IdInquilino] = @IdInquilino, [Nome] = @Nome, [Morada] = @Morada, [ID_EstadoCivil] = @ID_EstadoCivil, 
	[Contacto1] = @Contacto1, [Contacto2] = @Contacto2, [eMail] = @eMail, [IRSAnual] = @IRSAnual, 
	[Vencimento] = @Vencimento, [Notas] = @Notas, [NIF] = @NIF, [Identificacao] = @Identificacao, 
	[ValidadeCC] = @ValidadeCC
WHERE [Id] = @Id;
	
SELECT	I.Id, I.IdInquilino, Nome, Morada, I.ID_EstadoCivil, EC.Descricao EstadoCivil,
		Contacto1, Contacto2, NIF, Identificacao, ValidadeCC, 
		eMail, IRSAnual, Vencimento, Notas, Ativo
FROM    Fiadores I 
		INNER JOIN EstadoCivil EC ON
			I.ID_EstadoCivil = EC.Id
WHERE	(I.Id = @Id)
GO
/****** Object:  StoredProcedure [dbo].[usp_FormaPagamento_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_FormaPagamento_Delete]
(
	@Id int
)
AS

SET NOCOUNT OFF;
DELETE FROM [FormaPagamento] WHERE [Id] = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_FormaPagamento_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_FormaPagamento_GetAll]
AS

SET NOCOUNT ON;
SELECT  Id, Descricao
FROM    FormaPagamento
GO
/****** Object:  StoredProcedure [dbo].[usp_FormaPagamento_GetById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_FormaPagamento_GetById] (@Id int)
AS

SET NOCOUNT ON;
SELECT	Id, Descricao
FROM	FormaPagamento
WHERE	Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_FormaPagamento_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_FormaPagamento_Insert]
(
	@Descricao varchar(75)
)
AS

SET NOCOUNT OFF;
INSERT INTO [FormaPagamento] ([Descricao]) VALUES (@Descricao);
	
GO
/****** Object:  StoredProcedure [dbo].[usp_FormaPagamento_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_FormaPagamento_Update]
(
	@Id int,
	@Descricao varchar(75)
)
AS

SET NOCOUNT OFF;
UPDATE [FormaPagamento] 
SET [Descricao] = @Descricao WHERE [Id] = @Id;
	
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_CheckDeleteConstraint]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Antes de apagar fração, verificar se há contratos associados  à mesma
-- Se a operação resultar em 0 (zero), é válido apagar inquilino

CREATE PROCEDURE [dbo].[usp_Fracoes_CheckDeleteConstraint] (@Id int, @OkToDelete bit OUTPUT)


AS

declare @CanDelete bit

SET NOCOUNT ON;
SELECT	@CanDelete = COUNT(1)
FROM    Contratos
WHERE	ID_Fracao = @Id

SELECT @OkToDelete = @CanDelete
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_Delete]
(
	@Id int
)
AS

SET NOCOUNT OFF;
DELETE FROM [Fracoes] WHERE [Id] = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_DeleteImage]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_DeleteImage]
(	
	@Id int
)
AS

SET NOCOUNT OFF;
BEGIN
	DELETE FROM [ImagensFracoes] 
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_DeleteImages]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_DeleteImages]
(	
	@Id int
)
AS

SET NOCOUNT OFF;
BEGIN
	DELETE FROM [ImagensFracoes] 
	WHERE Id_Fracao = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_GetAll]
AS

SET NOCOUNT ON;
SELECT	F.Id, F.Ativa, F.Descricao, ValorRenda, AreaBrutaPrivativa, AreaBrutaDependente, CasasBanho, 
		Varanda, Terraco, Garagem, Arrecadacao, LugarEstacionamento, Fotos, F.Notas, 
		Tipologia, Matriz, Andar, Lado, AnoUltAvaliacao, ValorUltAvaliacao,
		LicencaHabitacao, DataEmissaoLicencaHabitacao,
		ID_CertificadoEnergetico, CE.Descricao CertificadoEnergetico,
		ID_TipoPropriedade, Id_Imovel, Situacao, F.Conservacao, F.GasCanalizado, F.CozinhaEquipada,
		EC.Descricao EstadoConservacao, SF.Descricao SituacaoFracao, 
		TF.Descricao TipologiaFracao, I.Descricao DescricaoImovel,
		TP.Descricao TipoPropriedade
FROM    Fracoes F 
			INNER JOIN Imoveis I ON
				F.Id_Imovel = I.Id
			INNER JOIN TipologiaFracao TF ON
				F.Tipologia = TF.Id
			INNER JOIN SituacaoFracao SF ON
				F.Situacao = SF.Id
			INNER JOIN EstadoConservacao EC ON
				F.Conservacao = EC.Id
			INNER JOIN TipoPropriedade TP ON
				F.ID_TipoPropriedade = TP.Id
			INNER JOIN TipoCertificadoEnergetico CE ON 
				ID_CertificadoEnergetico = CE.Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_GetAvailable]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_GetAvailable]
AS

BEGIN
	SET NOCOUNT ON;
    SELECT Id, Descricao
    FROM Fracoes
 	WHERE Id IN
		(SELECT ID_Fracao FROM Arrendamentos WHERE Arrendamentos.Ativo = 1) 
    ORDER BY Fracoes.Descricao
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_GetById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_GetById] (@Id int)
AS

SET NOCOUNT ON;
SELECT	F.Id, F.Ativa, F.Descricao, ValorRenda, AreaBrutaPrivativa, AreaBrutaDependente, CasasBanho, 
		Varanda, Terraco, Garagem, Arrecadacao, LugarEstacionamento, Fotos, F.Notas, 
		Tipologia, Matriz, Andar, Lado, AnoUltAvaliacao, ValorUltAvaliacao, 
		LicencaHabitacao, DataEmissaoLicencaHabitacao,
		ID_CertificadoEnergetico, CE.Descricao CertificadoEnergetico,
		ID_TipoPropriedade, Id_Imovel, Situacao, F.Conservacao, GasCanalizado, F.CozinhaEquipada,
		EC.Descricao EstadoConservacao, SF.Descricao SituacaoFracao, 
		TF.Descricao TipologiaFracao, I.Descricao DescricaoImovel,
		TP.Descricao TipoPropriedade
FROM    Fracoes F 
			INNER JOIN Imoveis I ON
				F.Id_Imovel = I.Id
			INNER JOIN TipologiaFracao TF ON
				F.Tipologia = TF.Id
			INNER JOIN SituacaoFracao SF ON
				F.Situacao = SF.Id
			INNER JOIN EstadoConservacao EC ON
				F.Conservacao = EC.Id
			INNER JOIN TipoPropriedade TP ON
				F.ID_TipoPropriedade = TP.Id
			INNER JOIN TipoCertificadoEnergetico CE ON 
				ID_CertificadoEnergetico = CE.Id
WHERE	F.Id = 
	CASE 
		WHEN  @Id > 0 THEN @Id 
		ELSE F.Id 
	END
ORDER BY Descricao
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_GetFracoes_Lookup]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_GetFracoes_Lookup] (@Id int) -- id do imóvel
AS

SET NOCOUNT ON;
SELECT	Id, Descricao
FROM    Fracoes
WHERE	Id_Imovel = 
	CASE 
		WHEN  @Id > 0 THEN @Id 
		ELSE Id_Imovel
	END	
ORDER BY Descricao
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_GetFracoes_WithDuePayments]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_GetFracoes_WithDuePayments]
AS

SET NOCOUNT ON;
SELECT	F.Id, F.Descricao
FROM    Fracoes F
INNER JOIN Recebimentos R ON
	F.Id = R.ID_Propriedade
WHERE R.Estado = 2
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_GetImage_ByUnitId]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_GetImage_ByUnitId] (@Id int)
AS

SET NOCOUNT ON;
SELECT id, Id_Fracao, Descricao, Foto
FROM ImagensFracoes
WHERE Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_GetImages_ByUnitId]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_GetImages_ByUnitId] (@Id int)
AS

SET NOCOUNT ON;
SELECT id, Id_Fracao, Descricao, Foto
FROM ImagensFracoes
WHERE Id_Fracao = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_GetPropertyUnits]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_GetPropertyUnits] (@id int)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT	F.Id, F.Ativa, F.Descricao, ValorRenda, AreaBrutaPrivativa, AreaBrutaDependente, CasasBanho, 
			Varanda, Terraco, Garagem, Arrecadacao, LugarEstacionamento, Fotos, F.Notas, 
			Tipologia, Matriz, Andar, Lado, AnoUltAvaliacao, ValorUltAvaliacao, 
			ID_TipoPropriedade, Id_Imovel, Situacao, F.Conservacao, GasCanalizado,F.CozinhaEquipada,
			EC.Descricao EstadoConservacao, SF.Descricao SituacaoFracao, 
			TF.Descricao TipologiaFracao, I.Descricao DescricaoImovel,
			TP.Descricao TipoPropriedade
	FROM    Fracoes F 
				INNER JOIN Imoveis I ON
					F.Id_Imovel = I.Id
				INNER JOIN TipologiaFracao TF ON
					F.Tipologia = TF.Id
				INNER JOIN SituacaoFracao SF ON
					F.Situacao = SF.Id
				INNER JOIN EstadoConservacao EC ON
					F.Conservacao = EC.Id
				INNER JOIN TipoPropriedade TP ON
					F.ID_TipoPropriedade = TP.Id
	WHERE F.Id_Imovel = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_GetUnitById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_GetUnitById] (@Id int)
AS

SET NOCOUNT ON;
SELECT	F.Id, F.Ativa, F.Descricao, ValorRenda, AreaBrutaPrivativa, AreaBrutaDependente, CasasBanho, 
		Varanda, Terraco, Garagem, Arrecadacao, LugarEstacionamento, GasCanalizado, F.CozinhaEquipada,
		Fotos, F.Notas, Tipologia, Matriz, Andar, Lado, AnoUltAvaliacao, ValorUltAvaliacao, 
		LicencaHabitacao, DataEmissaoLicencaHabitacao,
		ID_TipoPropriedade, Id_Imovel, Situacao, F.Conservacao 
		
FROM    Fracoes F 
WHERE	F.Id = 
	CASE 
		WHEN  @Id > 0 THEN @Id 
		ELSE F.Id 
	END
ORDER BY Descricao
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_GetUnitSituation_ByDescription]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_GetUnitSituation_ByDescription] (@Descricao varchar(50)) 
AS

SET NOCOUNT ON;
SELECT	Id
FROM    SituacaoFracao
WHERE	Descricao = @Descricao
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_GetUnitsWithoutLease]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_GetUnitsWithoutLease] (@Id int )
AS

BEGIN
	SET NOCOUNT ON;
    SELECT Id, Descricao
    FROM Fracoes
 	WHERE Id_Imovel = @Id AND Id NOT IN
		(SELECT ID_Fracao FROM Arrendamentos) 
    ORDER BY Fracoes.Descricao
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_Imovel]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_Imovel] (@Id int)
AS

BEGIN
	SET NOCOUNT ON;
	SELECT	Id, Ativa, Descricao, ValorRenda, AreaBrutaPrivativa, AreaBrutaDependente, CasasBanho, 
			LicencaHabitacao, DataEmissaoLicencaHabitacao,
			Alugada, Livre, Reservada, Vendida, Contencioso,
			Varanda, Terraco, Garagem, Arrecadacao, GasCanalizado, CozinhaEquipada,
			LugarEstacionamento, Fotos, Notas, Tipologia, Matriz, Andar, Lado, 
			AnoUltAvaliacao, ValorUltAvaliacao, ID_TipoPropriedade, Id_Imovel, Situacao, Conservacao
	FROM    Fracoes
	WHERE Id_Imovel = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_Insert]
(	
	@Ativa bit,
	@Descricao varchar(128),
	@ValorRenda decimal,
	@AreaBrutaPrivativa float,
	@AreaBrutaDependente float,
	@CasasBanho int,
	@Varanda bit,
	@Terraco bit,
	@Garagem bit,
	@Arrecadacao bit,
	@LugarEstacionamento bit,
	@GasCanalizado bit,
	@CozinhaEquipada bit, 
	@Fotos bit,
	@Notas text,
	@Tipologia int,
	@LicencaHabitacao varchar(30),
	@DataEmissaoLicencaHabitacao datetime,
	@ID_CertificadoEnergetico varchar(3),
	@Matriz varchar(50),
	@Andar varchar(20),
	@Lado varchar(20),
	@AnoUltAvaliacao varchar(4),
	@ValorUltAvaliacao decimal(12, 2),
	@ID_TipoPropriedade int,
	@Id_Imovel int,
	@Situacao int,
	@Conservacao int
)
AS

SET NOCOUNT OFF;
INSERT INTO [Fracoes] 
	([Ativa], [Descricao], [ValorRenda], [AreaBrutaPrivativa], [AreaBrutaDependente], [CasasBanho], 
	[Varanda], [Terraco], [Garagem], [GasCanalizado], [CozinhaEquipada], [Arrecadacao], [LugarEstacionamento], [Fotos], [Notas], 
	[Tipologia], [LicencaHabitacao], [DataEmissaoLicencaHabitacao], [ID_CertificadoEnergetico],
	[Matriz], [Andar], [Lado], [AnoUltAvaliacao], [ValorUltAvaliacao],
	[ID_TipoPropriedade], [Id_Imovel], [Situacao], [Conservacao])
VALUES 
	(@Ativa, @Descricao, @ValorRenda, @AreaBrutaPrivativa, @AreaBrutaDependente, @CasasBanho,
	@Varanda, @Terraco, @Garagem, @GasCanalizado, @CozinhaEquipada, @Arrecadacao, 
	@LugarEstacionamento, @Fotos, @Notas, @Tipologia, @LicencaHabitacao, @DataEmissaoLicencaHabitacao,
	@ID_CertificadoEnergetico, @Matriz, @Andar, @Lado, 
	@AnoUltAvaliacao, @ValorUltAvaliacao, @ID_TipoPropriedade, @Id_Imovel, 
	@Situacao, @Conservacao);
	
SELECT CAST(SCOPE_IDENTITY() as int);
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_InsertImage]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_InsertImage]
(	
	@Descricao varchar(128),
	@Foto varchar(256),
	@Id_Fracao int
)
AS

SET NOCOUNT OFF;
INSERT INTO [ImagensFracoes] 
	([Descricao], [Foto], [Id_Fracao])
VALUES 
	(@Descricao, @Foto, @Id_Fracao);
	
SELECT CAST(SCOPE_IDENTITY() as int);
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_IsUnitFreeToLease]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_IsUnitFreeToLease] (@Id int)
AS

BEGIN
	SET NOCOUNT ON;
	SELECT 1 
	FROM Fracoes 
	WHERE Id = @Id AND Situacao = 2 -- Livre
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_SetAsFree]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_SetAsFree] 
(
	@Id int
)

AS

BEGIN
	SET NOCOUNT ON;
	UPDATE Fracoes
	SET Situacao = 2 -- free
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_SetAsInDispute]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_SetAsInDispute] 
(
	@Id int
)

AS

BEGIN
	SET NOCOUNT ON;
	UPDATE Fracoes
	SET Situacao = 5
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_SetAsRented]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_SetAsRented] 
(
	@Id int
)

AS

BEGIN
	SET NOCOUNT ON;
	UPDATE Fracoes
	SET Situacao = 1 -- alugada
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_SetAsReserved]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_SetAsReserved] 
(
	@Id int
)

AS

BEGIN
	SET NOCOUNT ON;
	UPDATE Fracoes
	SET Situacao = 3
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_SetAsSold]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_SetAsSold] 
(
	@Id int
)

AS

BEGIN
	SET NOCOUNT ON;
	UPDATE Fracoes
	SET Situacao = 4
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_Update]
(
	@Id int,
	@Ativa bit,
	@Descricao varchar(128),
	@ValorRenda decimal,
	@AreaBrutaPrivativa float,
	@AreaBrutaDependente float,
	@CasasBanho int,
	@Varanda bit,
	@Terraco bit,
	@Garagem bit,
	@Arrecadacao bit,
	@GasCanalizado bit,
	@CozinhaEquipada bit, 
	@LugarEstacionamento bit,
	@Fotos bit,
	@Notas text,
	@Tipologia int,
	@LicencaHabitacao varchar(30),
	@DataEmissaoLicencaHabitacao datetime,
	@ID_CertificadoEnergetico varchar(3),
	@Matriz varchar(50),
	@Andar varchar(20),
	@Lado varchar(20),
	@AnoUltAvaliacao varchar(4),
	@ValorUltAvaliacao decimal(12, 2),
	@ID_TipoPropriedade int,
	@Id_Imovel int,
	@Situacao int,
	@Conservacao int
)
AS

SET NOCOUNT OFF;
UPDATE Fracoes 
SET Descricao = @Descricao, 
	Ativa = @Ativa, 
	ValorRenda = @ValorRenda, 
	AreaBrutaPrivativa = @AreaBrutaPrivativa, 
	AreaBrutaDependente = @AreaBrutaDependente, 
	CasasBanho = @CasasBanho, 
	Varanda = @Varanda, 
	CozinhaEquipada = @CozinhaEquipada,
	Terraco = @Terraco,
	Garagem = @Garagem, 
	Arrecadacao = @Arrecadacao, 
	GasCanalizado = @GasCanalizado,
	LugarEstacionamento = @LugarEstacionamento, 
	Fotos = @Fotos, 
	Notas = @Notas, 
	Tipologia = @Tipologia, 
	LicencaHabitacao = @LicencaHabitacao, DataEmissaoLicencaHabitacao = @DataEmissaoLicencaHabitacao,
	ID_CertificadoEnergetico = @ID_CertificadoEnergetico, Matriz = @Matriz, 
	Andar = @Andar,
	Lado = @Lado,
	AnoUltAvaliacao = @AnoUltAvaliacao,
	ValorUltAvaliacao = @ValorUltAvaliacao,
	ID_TipoPropriedade = @ID_TipoPropriedade, 
	Id_Imovel = @Id_Imovel, 
	Situacao = @Situacao, 
	Conservacao = @Conservacao 
WHERE Id = @Id;

SELECT	F.Id, F.Ativa, F.Descricao, ValorRenda, AreaBrutaPrivativa, AreaBrutaDependente, CasasBanho, 
		Varanda, Terraco, Garagem, Arrecadacao, LugarEstacionamento, GasCanalizado, F.CozinhaEquipada,
		Fotos, F.Notas, Tipologia, LicencaHabitacao, DataEmissaoLicencaHabitacao,
		Matriz, Andar, Lado, AnoUltAvaliacao, ValorUltAvaliacao, 
		ID_TipoPropriedade, Id_Imovel, Situacao, F.Conservacao 
		
FROM    Fracoes F 
WHERE	F.Id = 
	CASE 
		WHEN  @Id > 0 THEN @Id 
		ELSE F.Id 
	END

ORDER BY Descricao
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_UpdateImage]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_UpdateImage]
(	
	@Id int,
	@Descricao varchar(50),
	@Foto varchar(256),
	@Id_Fracao int
)
AS

SET NOCOUNT OFF;
BEGIN
	UPDATE [ImagensFracoes] 
	SET [Descricao] = @Descricao, [Foto] = @Foto, [Id_Fracao] = @Id_Fracao
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_UpdateRentValue]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_UpdateRentValue] (@Id int, @NewValue decimal(10, 2))
AS

BEGIN
	SET NOCOUNT ON;
	UPDATE Fracoes
	SET ValorRenda = @NewValue
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_UpdateStatus]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_UpdateStatus] 
(
	@Id int,
	@Status int
)

AS

BEGIN
	SET NOCOUNT ON;
	UPDATE Fracoes
	SET Situacao = @Status
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Fracoes_UpdateStatusBySituation_todelete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Fracoes_UpdateStatusBySituation_todelete] 
(
	@Id int, 
	@Alugada bit, @Livre bit, @Reservada bit, @Vendida bit, @Contencioso bit
)

AS

BEGIN
	SET NOCOUNT ON;
	UPDATE Fracoes
	SET Alugada = @Alugada, Livre = @Livre, Reservada = @Reservada, Vendida = @Vendida, Contencioso= @Contencioso
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetConcelhos]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[usp_GetConcelhos]
AS
BEGIN
	SELECT Id, CodConcelho, Descricao, IdDistrito, Coeficiente
	FROM Concelhos
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetConcelhosDeDistrito]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[usp_GetConcelhosDeDistrito](@IdDistrito int)
AS
BEGIN
	SELECT Id, CodConcelho, Descricao, IdDistrito, Coeficiente
	FROM Concelhos C 
	WHERE C.IdDistrito = @IdDistrito
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetDistritos]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[usp_GetDistritos]
AS
BEGIN
	SELECT Id, Descricao
	FROM Distritos D
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Imoveis_CheckDeleteConstraint]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Antes de apagar imóvel, verificar se há contratos associados ao mesmo
-- Se a operação resultar em 0 (zero), é válido apagar fração
CREATE PROCEDURE [dbo].[usp_Imoveis_CheckDeleteConstraint] (@Id int)

AS

BEGIN
	SET NOCOUNT ON;
	SELECT COUNT(1)
	FROM   Fracoes
	WHERE  Id_Imovel = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Imoveis_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Imoveis_Delete]
(
	@Id int
)
AS

SET NOCOUNT OFF;
DELETE FROM [Imoveis] WHERE [Id] = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Imoveis_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Imoveis_GetAll]
AS

SET NOCOUNT ON;
SELECT	I.Id, I.Descricao, Morada, Numero, CodPst, CodPstEx, 
		Freguesia FreguesiaImovel, Concelho ConcelhoImovel, DataUltimaInspecaoGas,
		AnoConstrucao, Elevador, Notas, Foto, Conservacao, EC.Descricao EstadoConservacao
FROM	Imoveis I 
		INNER JOIN EstadoConservacao EC ON
			I.Conservacao = EC.Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Imoveis_GetById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_Imoveis_GetById](@Id int)

AS
BEGIN
SELECT	I.Id, I.Descricao, Morada, Numero, CodPst, CodPstEx, 
		Freguesia FreguesiaImovel, Concelho ConcelhoImovel, DataUltimaInspecaoGas,
		AnoConstrucao, Elevador, Notas, Foto, Conservacao, EC.Descricao EstadoConservacao
FROM	Imoveis I 
		INNER JOIN EstadoConservacao EC ON
			I.Conservacao = EC.Id
		WHERE I.Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Imoveis_GetCodigoImovel]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_Imoveis_GetCodigoImovel](@Id int)

AS
BEGIN
			SELECT Id_Imovel 
			FROM Fracoes
			WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Imoveis_GetDescricaoImovel]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_Imoveis_GetDescricaoImovel](@Id int)

AS
BEGIN
	SELECT Descricao 
	FROM Imoveis
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Imoveis_GetImoveisAsLookupTable]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_Imoveis_GetImoveisAsLookupTable]

AS
BEGIN
SELECT	Id, Descricao
FROM   Imoveis 
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Imoveis_GetNumeroPorta]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_Imoveis_GetNumeroPorta](@Id int)

AS
BEGIN
	SELECT Numero FROM Imoveis WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Imoveis_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Imoveis_Insert]
(
	@Descricao varchar(60),
	@Numero varchar(4),
	@Morada varchar(60),
	@CodPst varchar(4),
	@CodPstEx varchar(3),
	@AnoConstrucao varchar(4),
	@FreguesiaImovel varchar(30),
	@ConcelhoImovel varchar(40),
	@Elevador bit,
	@Notas text,
	@Foto varchar(256),
	@Conservacao int,
	@DataUltimaInspecaoGas datetime

)
AS
SET NOCOUNT OFF;
INSERT INTO [Imoveis] 
	([Descricao], [Numero], [Morada], [CodPst], [CodPstEx], [AnoConstrucao], 
	[Freguesia], [Concelho], [Elevador], [Notas], [Foto], [Conservacao],
	[DataUltimaInspecaoGas]) 
VALUES 
	(@Descricao, @Numero, @Morada, @CodPst, @CodPstEx, @AnoConstrucao, 
	@FreguesiaImovel, @ConcelhoImovel, @Elevador, @Notas, @Foto, @Conservacao, 
	@DataUltimaInspecaoGas);	

SELECT CAST(SCOPE_IDENTITY() as int);
GO
/****** Object:  StoredProcedure [dbo].[usp_Imoveis_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Imoveis_Update]
(
	@Id int,
	@Descricao varchar(60),
	@Numero varchar(4),
	@Morada varchar(60),
	@CodPst varchar(4),
	@CodPstEx varchar(3),
	@AnoConstrucao varchar(4),
	@FreguesiaImovel varchar(30),
	@ConcelhoImovel varchar(40),
	@Elevador bit,
	@Notas text,
	@Foto varchar(256),
	@Conservacao int,
	@DataUltimaInspecaoGas datetime

)
AS

SET NOCOUNT OFF;
UPDATE [Imoveis] 
SET [Descricao] = @Descricao, [Numero] = @Numero, [Morada] = @Morada, [CodPst] = @CodPst, 
	[CodPstEx] = @CodPstEx, [AnoConstrucao] = @AnoConstrucao, [Freguesia] = @FreguesiaImovel, 
	[Concelho] = @ConcelhoImovel, [Elevador] = @Elevador, [Notas] = @Notas, [Foto] = @Foto,
	[Conservacao] = @Conservacao, [DataUltimaInspecaoGas] = @DataUltimaInspecaoGas
WHERE [Id] = @Id;
	
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_CheckDeleteConstraint]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Antes de apagar inquilino, verificar se há contratos associadosao mesmo
-- Se a operação resultar em 0 (zero), é válido apagar inquilino

CREATE PROCEDURE [dbo].[usp_Inquilinos_CheckDeleteConstraint] (@Id int, @OkToDelete bit OUTPUT)


AS

declare @CanDelete bit

SET NOCOUNT ON;
SELECT	@CanDelete = COUNT(1)
FROM    Arrendamentos
WHERE	ID_Inquilino = @Id

SELECT @OkToDelete = @CanDelete
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_CheckForPriorRentUpdates]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_CheckForPriorRentUpdates]
(
	@UnitId int
)

AS

BEGIN
	SET NOCOUNT OFF;
	SELECT COUNT(1) 
	FROM HistoricoAtualizacaoRendas
	WHERE UnitId = @UnitId AND
		YEAR(DateProcessed) = YEAR(GetDate())
END 
	
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_Delete]
(
	@Id int
)
AS

SET NOCOUNT OFF;
DELETE FROM [Inquilinos] 
WHERE [Id] = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_DeleteDocument]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_DeleteDocument]
(
	@Id int
)
AS

SET NOCOUNT OFF;
DELETE FROM [DocumentosInquilino] 
WHERE [Id] = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_GetAll] 
AS

SET NOCOUNT ON;
SELECT	I.Id, Nome, Morada, Naturalidade, I.ID_EstadoCivil, EC.Descricao EstadoCivil,
		DataNascimento, Contacto1, Contacto2, NIF, Identificacao, ValidadeCC, 
		eMail, IRSAnual, Vencimento, Titular, Notas, Ativo, SaldoCorrente, 
		dbo.fn_GetSaldoPrevisto_Inquilino(I.Id) SaldoPrevisto
FROM    Inquilinos I INNER JOIN 
			EstadoCivil EC ON
				I.ID_EstadoCivil = ec.Id

GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetAllDocuments]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_GetAllDocuments]
AS

SET NOCOUNT OFF;
SELECT DI.Id,[Descricao], [DocumentPath], [DocumentType], [TenantId], I.Nome NomeInquilino, 
		CreationDate, ReferralDate,
		[StorageType], [StorageFolder], DT.Name TipoDocumento
FROM [DocumentosInquilino] DI
	INNER JOIN Inquilinos I ON
		DI.TenantId = I.Id
	INNER JOIN DocumentTypes DT
		ON DI.DocumentType = DT.Id

GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetAllRentUpdates]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_GetAllRentUpdates] 
AS

SET NOCOUNT ON;
SELECT	H.Id, H.UnitId, H.DateProcessed, 
		H.PriorValue, H.UpdatedValue, 
		F.Descricao DescricaoFracao, I.Nome NomeInquilino
FROM    HistoricoAtualizacaoRendas H
	INNER JOIN Arrendamentos A ON
		H.UnitId = A.ID_Fracao
	INNER JOIN Inquilinos I ON
		A.ID_Inquilino = I.Id
	INNER JOIN Fracoes F ON
		H.UnitId = F.Id


GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetDocumentById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_GetDocumentById]
(
	@Id int
)
AS

SET NOCOUNT OFF;
SELECT DI.Id,[Descricao], [DocumentPath], [DocumentType], [TenantId], I.Nome NomeInquilino, ReferralDate,
		[StorageType], [StorageFolder], DT.Name TipoDocumento
FROM [DocumentosInquilino] DI
	INNER JOIN Inquilinos I ON
		DI.TenantId = I.Id
	INNER JOIN DocumentTypes DT
		ON DI.DocumentType = DT.Id

WHERE DI.Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetFiador_ById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Inquilinos_GetFiador_ById] (@Id int)
AS

SET NOCOUNT ON;
SELECT	Id, IdInquilino, Nome, Morada, ID_EstadoCivil, Contacto1, Contacto2, eMail, 
		IRSAnual, Vencimento, Notas, NIF, Identificacao, ValidadeCC
FROM    Fiadores
WHERE	IdInquilino = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetInquilino_ById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Inquilinos_GetInquilino_ById] (@Id int)
AS

SET NOCOUNT ON;
SELECT	Id, Nome, Morada, Naturalidade, ID_EstadoCivil, Contacto1, Contacto2, eMail, 
		IRSAnual, Vencimento, Notas, NIF, Identificacao, Titular, ValidadeCC, DataNascimento,
		SaldoCorrente, dbo.fn_GetSaldoPrevisto_Inquilino(Id) SaldoPrevisto
FROM    Inquilinos
WHERE	Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetInquilino_Exteded_ById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Inquilinos_GetInquilino_Exteded_ById] (@Id int)
AS

SET NOCOUNT ON;
SELECT	I.Id, Nome, Morada, Naturalidade, ID_EstadoCivil, 
		EC.Descricao EstadoCivil, Contacto1, Contacto2, eMail, 
		IRSAnual, Vencimento, Notas, NIF, Identificacao, 
		Titular, ValidadeCC, DataNascimento
FROM    Inquilinos I INNER JOIN EstadoCivil EC ON
		I.ID_EstadoCivil = EC.Id
WHERE	I.Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetInquilino_Extended_ById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Inquilinos_GetInquilino_Extended_ById] (@Id int)
AS

SET NOCOUNT ON;
SELECT	I.Id, Nome, Morada, Naturalidade, I.ID_EstadoCivil, EC.Descricao EstadoCivil,
		DataNascimento, Contacto1, Contacto2, NIF, Identificacao, ValidadeCC, 
		eMail, IRSAnual, Vencimento, Titular, Notas, Ativo, SaldoCorrente,
		dbo.fn_GetSaldoPrevisto_Inquilino(I.Id) SaldoPrevisto
FROM    Inquilinos I 
		INNER JOIN EstadoCivil EC ON
			I.ID_EstadoCivil = EC.Id
WHERE	I.Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetInquilinos_AsLookup]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Inquilinos_GetInquilinos_AsLookup]
AS

SET NOCOUNT ON;
SELECT inq.Id, inq.Nome Descricao 
FROM Inquilinos inq 
ORDER BY Inq.Nome

GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetInquilinos_Disponiveis]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Inquilinos_GetInquilinos_Disponiveis]
AS

SET NOCOUNT ON;
SELECT inq.Id, inq.Nome Descricao 
FROM Inquilinos inq 
WHERE inq.Id NOT IN(
	SELECT ID_Inquilino	FROM Arrendamentos) 
ORDER BY Inq.Nome

GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetInquilinos_Fiadores]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Inquilinos_GetInquilinos_Fiadores](@Titular bit)
AS

SET NOCOUNT ON;


SELECT inq.Id, inq.Nome Descricao 
FROM Inquilinos inq 
WHERE Titular = CASE WHEN @Titular = 1 THEN 1 ELSE 0 END
ORDER BY Inq.Nome

GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetInquilinos_SemContrato]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Inquilinos_GetInquilinos_SemContrato]
AS

SET NOCOUNT ON;

SET NOCOUNT ON;
SELECT Id, Nome Descricao
FROM Inquilinos
WHERE Id NOT IN
	(SELECT ID_Inquilino FROM Arrendamentos) 
ORDER BY Nome
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetNome]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Inquilinos_GetNome] (@Id int)
AS

SET NOCOUNT ON;
SELECT	Nome
FROM    Inquilinos
WHERE	Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetRentAdjustments]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_Inquilinos_GetRentAdjustments]

AS

BEGIN
	SELECT CC.Id, CC.TransactionId, CC.DataMovimento, CC.ValorPago, CC.UpdatedDate,
	f.ValorRenda ValorRenda, CC.ValorEmDivida, F.Descricao FracaoInquilino, I.Nome NomeInquilino, CC.Notas
	FROM CC_Inquilinos CC
	INNER JOIN arrendamentos A ON
		CC.IdInquilino = A.ID_Inquilino
	INNER JOIN Fracoes F ON
		A.ID_Fracao = F.Id
	INNER JOIN Inquilinos I ON
		CC.IdInquilino = I.id
	WHERE ValorEmDivida > 0 OR 
		ID_TipoRecebimento <> 99
	ORDER BY CC.DataMovimento
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetRentUpdates_ByTenantId]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_GetRentUpdates_ByTenantId] (@TenantId int)
AS

SET NOCOUNT ON;
SELECT	H.Id, H.UnitId, H.DateProcessed, 
		H.PriorValue, H.UpdatedValue, 
		F.Descricao DescricaoFracao, I.Nome NomeInquilino
FROM    HistoricoAtualizacaoRendas H
	INNER JOIN Arrendamentos A ON
		H.UnitId = A.ID_Fracao
	INNER JOIN Inquilinos I ON
		A.ID_Inquilino = I.Id
	INNER JOIN Fracoes F ON
		H.UnitId = F.Id
WHERE I.Id = @TenantId



GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetTenantDocuments]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_GetTenantDocuments] (@Id int)
AS

SET NOCOUNT OFF;
SELECT	DI.Id, [Descricao], [DocumentPath], [DocumentType], [TenantId], I.Nome NomeInquilino, 
		CreationDate, ReferralDate,
		[StorageType], [StorageFolder], DT.Description TipoDocumento
FROM [DocumentosInquilino] DI
	INNER JOIN Inquilinos I ON
		DI.TenantId = I.Id
	INNER JOIN DocumentTypes DT
		ON DI.DocumentType = DT.Id
WHERE DI.TenantId = @Id

GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetTenantPaymentsHistory]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_GetTenantPaymentsHistory] (@Id int)
AS

SET NOCOUNT OFF;
SELECT	Id, IdInquilino, DataMovimento, 
		ValorPago, ValorEmDivida, 
		Renda, ID_TipoRecebimento, 
		Notas, 
		TransactionId
FROM CC_Inquilinos
WHERE IdInquilino = @Id
ORDER BY TransactionId, DataMovimento

GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_GetValorRenda]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Inquilinos_GetValorRenda] (@Id int)
AS

SET NOCOUNT ON;
SELECT F.ValorRenda
FROM Arrendamentos A 
	INNER JOIN Fracoes F ON
		A.ID_Fracao = F.Id
WHERE A.ID_Inquilino = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_Insert]
(
	@Nome varchar(60),
	@Morada varchar(60),
	@Naturalidade varchar(50),
	@ID_EstadoCivil int,
	@Contacto1 varchar(20),
	@Contacto2 varchar(20),
	@eMail varchar(128),
	@IRSAnual decimal(10, 2),
	@Vencimento decimal(10, 2),
	@Notas text,
	@NIF varchar(9),
	@Identificacao varchar(20),
	@Titular bit,
	@ValidadeCC datetime,
	@DataNascimento datetime
)
AS

SET NOCOUNT OFF;
INSERT INTO [Inquilinos] 
	([Nome], [Morada], [Naturalidade], [ID_EstadoCivil], [Contacto1], [Contacto2], [eMail],
	[IRSAnual], [Vencimento], [Notas], [NIF], [Identificacao], [Titular], [ValidadeCC], [DataNascimento]) 
	VALUES (@Nome, @Morada, @Naturalidade, @ID_EstadoCivil, @Contacto1, @Contacto2, @eMail, 
	@IRSAnual, @Vencimento,	@Notas, @NIF, @Identificacao, @Titular, @ValidadeCC, @DataNascimento);
	
SELECT CAST(SCOPE_IDENTITY() as int);
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_InsertDocument]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_InsertDocument]
(
	@Descricao varchar(512),
	@DocumentPath varchar(256),
	@TenantId int,
	@ReferralDate datetime,
	@DocumentType int,
	@StorageType char(1),
	@StorageFolder varchar(50)
)
AS

SET NOCOUNT OFF;
INSERT INTO [DocumentosInquilino] 
	([Descricao], [DocumentPath], [DocumentType], [TenantId],[ReferralDate],[StorageType], [StorageFolder] )
	VALUES (@Descricao, @DocumentPath, @DocumentType, @TenantId, @ReferralDate, @StorageType, @StorageFolder);
	
SELECT CAST(SCOPE_IDENTITY() as int);
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_InsertRentUpdate]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_InsertRentUpdate]
(
	@UnitId int,
	@PriorValue decimal,
	@UpdatedValue decimal
)

AS

BEGIN
	SET NOCOUNT OFF;
	INSERT INTO HistoricoAtualizacaoRendas(UnitId, DateProcessed, PriorValue, UpdatedValue)
	VALUES( @UnitId, GetDate(), @PriorValue, @UpdatedValue)
END 
	
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_Stat_LatePaymentLetters]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_Stat_LatePaymentLetters]

AS
BEGIN
	SET LANGUAGE Portuguese
	select DI.Descricao Description, DI.CreationDate, 
		UPPER( DATENAME(mm, DI.ReferralDate)) + '-' + CAST(YEAR(DI.ReferralDate) AS VARCHAR(4)) RefersTo,
		I.Nome NomeInquilino
	from DocumentosInquilino DI
	inner join Inquilinos I ON
	DI.TenantId = I.Id
	WHERE DI.DocumentType = 18
	ORDER BY TenantId, CreationDate
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_Stat_RentUpdateLetters]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_Stat_RentUpdateLetters]

AS
BEGIN
	SET LANGUAGE Portuguese
	select I.Id, I.Nome, DI.Descricao Description, DI.CreationDate, 
		UPPER( DATENAME(mm, DI.ReferralDate)) + '-' + CAST(YEAR(DI.ReferralDate) AS VARCHAR(4)) RefersTo,
		I.Nome NomeInquilino
	from DocumentosInquilino DI
	inner join Inquilinos I ON
	DI.TenantId = I.Id
	WHERE DI.DocumentType = 16
	ORDER BY TenantId, CreationDate
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_Update]
(
	@Id int,
	@Nome varchar(60),
	@Morada varchar(60),
	@Naturalidade varchar(50),
	@ID_EstadoCivil int,
	@Contacto1 varchar(20),
	@Contacto2 varchar(20),
	@eMail varchar(128),
	@IRSAnual decimal(10, 2),
	@Vencimento decimal(10, 2),
	@Notas text,
	@NIF varchar(9),
	@Identificacao varchar(20),
	@Titular bit,
	@ValidadeCC datetime,
	@DataNascimento datetime
)
AS

SET NOCOUNT OFF;
UPDATE [Inquilinos] 
SET [Nome] = @Nome, [Morada] = @Morada, [Naturalidade] = @Naturalidade, [ID_EstadoCivil] = @ID_EstadoCivil, 
	[Contacto1] = @Contacto1, [Contacto2] = @Contacto2, [eMail] = @eMail, [IRSAnual] = @IRSAnual, 
	[Vencimento] = @Vencimento, [Notas] = @Notas, [NIF] = @NIF, [Identificacao] = @Identificacao, 
	[Titular] = @Titular, [ValidadeCC] = @ValidadeCC, [DataNascimento] = @DataNascimento 
WHERE [Id] = @Id;
	
SELECT	I.Id, Nome, Morada, Naturalidade, I.ID_EstadoCivil, EC.Descricao EstadoCivil,
		DataNascimento, Contacto1, Contacto2, NIF, Identificacao, ValidadeCC, 
		eMail, IRSAnual, Vencimento, Titular, Notas, Ativo, SaldoCorrente,
		dbo.fn_GetSaldoPrevisto_Inquilino(I.Id) SaldoPrevisto
FROM    Inquilinos I 
		INNER JOIN EstadoCivil EC ON
			I.ID_EstadoCivil = EC.Id
WHERE	(I.Id = @Id)
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_UpdateDocument]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_UpdateDocument]
(
	@Id int,
	@Descricao varchar(512),
	@DocumentPath varchar(256),
	@CreationDate datetime,
	@ReferralDate datetime,
	@TenantId int,
	@DocumentType int,
	@StorageType char(1),
	@StorageFolder varchar(50)

)
AS

BEGIN
	SET NOCOUNT OFF;
	UPDATE [DocumentosInquilino] 
	SET Descricao = @Descricao, DocumentPath = @DocumentPath, 
	TenantId = @TenantId, DocumentType = @DocumentType,
	CreationDate = @CreationDate, ReferralDate = @ReferralDate,
	[StorageType] = @StorageType, [StorageFolder] = @StorageFolder
	WHERE [Id] = @Id
END 
	
GO
/****** Object:  StoredProcedure [dbo].[usp_Inquilinos_UpdateSaldoCorrente]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inquilinos_UpdateSaldoCorrente]
(
	@TenantId int,
	@NovoSaldoCorrente decimal
)
AS

SET NOCOUNT OFF
BEGIN
	UPDATE Inquilinos
	SET SaldoCorrente = @NovoSaldoCorrente
	WHERE Id = @TenantId
END
GO
/****** Object:  StoredProcedure [dbo].[Usp_InsertAuditLogs]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Usp_InsertAuditLogs] 
    @UrlReferrer VARCHAR(500)
	,@ActionName VARCHAR(50)
	,@Area VARCHAR(50)
	,@ControllerName VARCHAR(50)
	,@LoginStatus VARCHAR(1)
	,@LoggedInAt VARCHAR(50)
	,@LoggedOutAt VARCHAR(50)
	,@PageAccessed VARCHAR(500)
	,@IPAddress VARCHAR(50)
	,@SessionID VARCHAR(50)
	,@UserID VARCHAR(10)
	,@RoleId VARCHAR(2)
	,@LangId VARCHAR(2)
	,@IsFirstLogin VARCHAR(2)

AS
BEGIN
	DECLARE @table VARCHAR(15)
		,@sql NVARCHAR(MAX)
		,@sqlcreate NVARCHAR(MAX)
		,@newtable VARCHAR(30)
		,@currentdate VARCHAR(23);

	SET @currentdate = (CONVERT(VARCHAR, getdate(), 20))
	SET @table = (
			SELECT REPLACE(CONVERT(VARCHAR(11), getdate(), 106), ' ', '_')
			)
	SET @newtable = 'Audit_' + @table

	SELECT @newtable

	IF (
			EXISTS (
				SELECT *
				FROM INFORMATION_SCHEMA.TABLES
				WHERE TABLE_NAME = @newtable
				)
			)
	BEGIN
		SET @sql = CONCAT (
				'INSERT INTO ['
				,@newtable
				,'] (Area,ControllerName,ActionName,LoginStatus,LoggedInAt,LoggedOutAt,PageAccessed,IPAddress,SessionID,UserID,RoleId,LangId,IsFirstLogin,CurrentDatetime) '
				,'VALUES ('''
				,@Area
				,''','''
				,@ControllerName
				,''','''
				,@ActionName
				,''','''
				,@LoginStatus
				,''','''
				,@LoggedInAt
				,''','''
				,@LoggedOutAt
				,''','''
				,@PageAccessed
				,''','''
				,@IPAddress
				,''','''
				,@SessionID
				,''','''
				,@UserID
				,''','''
				,@RoleId
				,''','''
				,@LangId
				,''','''
				,@IsFirstLogin
				,''','''
				,@currentdate
				
			
				,''')'
				);

		EXEC (@sql);
	END
	ELSE
	BEGIN
		SET @sqlcreate = 'CREATE TABLE ' + '[' + @newtable + ']' + '(
 [AuditId] [bigint] IDENTITY(1,1) NOT NULL,
 [Area] [varchar](50) NULL,
 [ControllerName] [varchar](50) NULL,
 [ActionName] [varchar](50) NULL,
 [LoginStatus] [varchar](1) NULL,
 [LoggedInAt] [varchar](23) NULL,
 [LoggedOutAt] [varchar](23) NULL,
 [PageAccessed] [varchar](500) NULL,
 [IPAddress] [varchar](50) NULL,
 [SessionID] [varchar](50) NULL,
 [UserID] [varchar](50) NULL,
 [RoleId] [varchar](2) NULL,
 [LangId] [varchar](2) NULL,
 [IsFirstLogin] [varchar](2) NULL,
 [CurrentDatetime] [varchar](23) NULL
 )'

		EXEC sp_executesql @sqlcreate;

		SET @sql = CONCAT (
				'INSERT INTO ['
				,@newtable
				,'] (Area,ControllerName,ActionName,LoginStatus,LoggedInAt,LoggedOutAt,PageAccessed,IPAddress,SessionID,UserID,RoleId,LangId,IsFirstLogin,CurrentDatetime) '
				,'VALUES ('''
				,@Area
				,''','''
				,@ControllerName
				,''','''
				,@ActionName
				,''','''
				,@LoginStatus
				,''','''
				,@LoggedInAt
				,''','''
				,@LoggedOutAt
				,''','''
				,@PageAccessed
				,''','''
				,@IPAddress
				,''','''
				,@SessionID
				,''','''
				,@UserID
				,''','''
				,@RoleId
				,''','''
				,@LangId
				,''','''
				,@IsFirstLogin
				,''','''
				,@currentdate
				
				,''')'
				);

		EXEC (@sql);
	END
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Log_Login_Create]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Fausto Luís
-- Create date: 28/01/2022
-- Description:	Cria registo de login na tabela de logins
-- =============================================
CREATE PROCEDURE [dbo].[usp_Log_Login_Create] 

@UserId nvarchar(450),
@SessionId nvarchar(250),
@LoginDate datetime

AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO tblLogLogins(UserId, LoginDate, SessionId)
	VALUES(@UserId, @LoginDate, @SessionId)
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Log_Login_Select]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Fausto Luís
-- Create date: 24/10/2022
-- Description:	visualiza registos de login/logout
-- =============================================
CREATE PROCEDURE [dbo].[usp_Log_Login_Select] 

AS
BEGIN
	SET NOCOUNT ON;
	SELECT Id, UserId, SessionId, LoginDate, LogoutDate
	FROM tblLogLogins
	ORDER BY Id DESC

END
GO
/****** Object:  StoredProcedure [dbo].[usp_Log_Login_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Fausto Luís
-- Create date: 28/01/2022
-- Description:	Atualiza Data Logout na tabela de logins
-- =============================================
CREATE PROCEDURE [dbo].[usp_Log_Login_Update] 
	@SessionId varchar(250),
	@LogoutDate DateTime
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE tblLogLogins
	SET LogoutDate = @LogoutDate
	WHERE SessionId = @SessionId
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Log_Operacoes_Create]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Fausto Luís
-- Create date: 28/01/2022
-- Description:	Cria registo de log (operações CRUD - C)
-- =============================================
CREATE PROCEDURE [dbo].[usp_Log_Operacoes_Create]
	@Tabela nvarchar(255),
	@IdReg int,
	@QuemCriou nvarchar(450),
	@DataCriacao datetime
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO tblLogOperacoes(Tabela, IdReg, QuemCriou, DataCriacao)
	VALUES(@Tabela, @IdReg, @QuemCriou, @DataCriacao)
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Log_Operacoes_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Fausto Luís
-- Create date: 28/01/2022
-- Description:	Apaga registo na tabela de logOperações
-- =============================================
CREATE PROCEDURE [dbo].[usp_Log_Operacoes_Delete] 
	@Tabela nvarchar(255),
	@IdReg int,
	@QuemApagou nvarchar(450),
	@DataAnulacao datetime

AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO tblLogOperacoes(Tabela, IdReg, QuemApagou, DataAnulacao)
	VALUES(@Tabela, @IdReg, @QuemApagou, @DataAnulacao)
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Log_Operacoes_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Fausto Luís
-- Create date: 28/01/2022
-- Description:	Atualiza registo na tabela de logOperações
-- =============================================
CREATE PROCEDURE [dbo].[usp_Log_Operacoes_Update] 
	@Tabela nvarchar(255),
	@IdReg int,
	@QuemModificou nvarchar(450),
	@DataUltimaAlteracao datetime

AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO tblLogOperacoes(Tabela, IdReg, QuemModificou, DataUltimaAlteracao)
	VALUES(@Tabela, @IdReg, @QuemModificou, @DataUltimaAlteracao)
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Logs_ById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Logs_ById] (@Id int)
AS
	SET NOCOUNT ON;
	SELECT Id, [Message], MessageTemplate, [Level], [TimeStamp], [Exception], [Properties]
	FROM    PMlogs
	WHERE Id = @Id;
GO
/****** Object:  StoredProcedure [dbo].[usp_Logs_DeleteAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Logs_DeleteAll]
AS
	SET NOCOUNT ON;
	TRUNCATE TABLE PMlogs
GO
/****** Object:  StoredProcedure [dbo].[usp_Logs_DeleteById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Logs_DeleteById] (@Id int)
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM PMLogs
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Logs_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Logs_GetAll]
AS
	SET NOCOUNT ON;
	SELECT	*
	FROM    PMlogs
GO
/****** Object:  StoredProcedure [dbo].[usp_Logs_GetLoginEntries]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Fausto Luís
-- Create date: 12/03/2023
-- Description:	visualiza registos de logs (logins)
-- =============================================
CREATE PROCEDURE [dbo].[usp_Logs_GetLoginEntries] 

AS
BEGIN
	SET NOCOUNT ON;
	SELECT Id, [Message], MessageTemplate, [Level], [TimeStamp], [Exception], [Properties]
	FROM PMLogs
	WHERE [Message] LIKE '%' + 'Logged' + '%'
	ORDER BY TimeStamp DESC

END
GO
/****** Object:  StoredProcedure [dbo].[usp_Logs_GetLogs]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Fausto Luís
-- Create date: 12/03/2023
-- Description:	visualiza registos de logs
-- =============================================
CREATE PROCEDURE [dbo].[usp_Logs_GetLogs] 

AS
BEGIN
	SET NOCOUNT ON;
	SELECT Id, [Message], MessageTemplate, [Level], [TimeStamp], [Exception], [Properties]
	FROM PMLogs
	ORDER BY TimeStamp DESC

END
GO
/****** Object:  StoredProcedure [dbo].[usp_Messages_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Fausto Luís
-- Create date: 24-04-2023
-- Description:	Apaga registo na tabela de mensagens
-- =============================================
CREATE PROCEDURE [dbo].[usp_Messages_Delete] 

@MessageId int

AS
BEGIN
	SET NOCOUNT ON;
	Delete From Messages
	Where MessageId = @MessageId
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Messages_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Fausto Luís
-- Create date: 24-04-2023
-- Description:	Seleciona todos os registos da tabela de mensagens
-- =============================================
CREATE PROCEDURE [dbo].[usp_Messages_GetAll] 

AS
BEGIN
	SET NOCOUNT ON;
	Select MessageId, DestinationEmail, SenderEmail, SubjectTitle, MessageContent,
	MessageType, MessageSentOn, MessageReceivedOn, TenantId, ReferenceId
	FROM Messages
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Messages_GetById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Fausto Luís
-- Create date: 24-04-2023
-- Description:	Seleciona registo na tabela de mensagens, por Id da mensagem
-- =============================================
CREATE PROCEDURE [dbo].[usp_Messages_GetById] 

@MessageId int 

AS
BEGIN
	SET NOCOUNT ON;
	Select MessageId, DestinationEmail, SenderEmail, SubjectTitle, MessageContent,
	MessageType, MessageSentOn, MessageReceivedOn,
		MessageType, MessageSentOn, MessageReceivedOn, TenantId, ReferenceId
	FROM Messages
	Where MessageId = @MessageId
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Messages_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Fausto Luís
-- Create date: 24-04-2023
-- Description:	Cria registo na tabela de mensagens
-- =============================================
CREATE PROCEDURE [dbo].[usp_Messages_Insert] 

@DestinationEmail nvarchar(250),
@SenderEmail nvarchar(250),
@SubjectTitle nvarchar(100),
@MessageContent nvarchar(512),
@MessageType int,
@MessageSentOn Date,
@MessageReceivedOn Date,
@TenantId int,
@ReferenceId varchar(50)


AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO Messages(DestinationEmail, SenderEmail, SubjectTitle, MessageContent,
		MessageType, MessageSentOn, MessageReceivedOn, TenantId, ReferenceId
)
	VALUES(@DestinationEmail, @SenderEmail, @SubjectTitle, @MessageContent,
		@MessageType, @MessageSentOn, @MessageReceivedOn, @TenantId, @ReferenceId
);

	SELECT CAST(SCOPE_IDENTITY() as int);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Messages_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Fausto Luís
-- Create date: 24-04-2023
-- Description:	Atualiza registo na tabela de mensagens
-- =============================================
CREATE PROCEDURE [dbo].[usp_Messages_Update] 

@MessageId int,
@DestinationEmail nvarchar(250),
@SenderEmail nvarchar(250),
@SubjectTitle nvarchar(100),
@MessageContent nvarchar(512),
@MessageType int,
@MessageSentOn Date,
@MessageReceivedOn Date,
@TenantId int, 
@ReferenceId varchar(50)

AS
BEGIN
	SET NOCOUNT ON;
	Update Messages
		SET DestinationEmail = @DestinationEmail, 
		SenderEmail = @SenderEmail, SubjectTitle = @SubjectTitle, 
		MessageContent = @MessageContent, MessageType = @MessageType,
		MessageSentOn = @MessageSentOn, MessageReceivedOn = @MessageReceivedOn,
		TenantId = @TenantId, ReferenceId = @ReferenceId
	Where MessageId = @MessageId
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Proprietarios_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Proprietarios_Delete]
(
	@Id int
)
AS

SET NOCOUNT OFF;
DELETE FROM Proprietarios 
WHERE [Id] = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Proprietarios_Existe]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Proprietarios_Existe]

AS

SET NOCOUNT OFF;
SELECT COUNT(1) FROM Proprietarios
GO
/****** Object:  StoredProcedure [dbo].[usp_Proprietarios_GetById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Proprietarios_GetById] (@Id int)
AS

SET NOCOUNT ON;
SELECT	P.Id, Nome, Morada, CodPostal, Naturalidade, ID_EstadoCivil, DataNascimento, Identificacao,
		ValidadeCC, NIF, Contacto, eMail, Notas, EC.Descricao EstadoCivil
FROM    Proprietarios P INNER JOIN
		EstadoCivil EC ON P.ID_EstadoCivil = EC.Id			
WHERE	P.Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Proprietarios_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Proprietarios_Insert]
/*
Id, Nome, Morada, Naturalidade, ID_EstadoCivil, DataNascimento, Identificacao,
		ValidadeCC, NIF, Contacto, eMail, Notas
*/
(
	@Nome varchar(60),
	@Morada varchar(60),
	@CodPostal varchar(50),
	@Naturalidade varchar(50),
	@ID_EstadoCivil int,
	@DataNascimento datetime,
	@Identificacao varchar(75),
	@ValidadeCC datetime,
	@NIF varchar(9),
	@Contacto varchar(20),
	@eMail varchar(128),
	@Notas text
)
AS

SET NOCOUNT OFF;
INSERT INTO [Proprietarios] 
	([Nome], [Morada], [CodPostal], [Naturalidade], [ID_EstadoCivil], [Contacto], [eMail],
	[Notas], [NIF], [Identificacao], [ValidadeCC], [DataNascimento]) 
VALUES (@Nome, @Morada, @CodPostal, @Naturalidade, @ID_EstadoCivil, @Contacto, 	@eMail, 
	@Notas, @NIF, @Identificacao, @ValidadeCC, @DataNascimento);
	
SELECT CAST(SCOPE_IDENTITY() as int);
GO
/****** Object:  StoredProcedure [dbo].[usp_Proprietarios_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Proprietarios_Update]

(
	@Id int,
	@Nome varchar(60),
	@Morada varchar(60),
	@CodPostal varchar(50),
	@Naturalidade varchar(50),
	@ID_EstadoCivil int,
	@DataNascimento datetime,
	@Identificacao varchar(75),
	@ValidadeCC datetime,
	@NIF varchar(9),
	@Contacto varchar(20),
	@eMail varchar(128),
	@Notas text
)
AS

SET NOCOUNT OFF;
UPDATE[Proprietarios] SET 
	[Nome] = @Nome, [Morada] = @Morada, CodPostal = @CodPostal, [Naturalidade] = @Naturalidade, [ID_EstadoCivil] = @ID_EstadoCivil, 
	[Contacto] = @Contacto, [eMail] = @eMail, [Notas] = @Notas, [NIF] = @NIF, [Identificacao] = @Identificacao,
	[ValidadeCC] = @ValidadeCC, [DataNascimento] = @DataNascimento
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_AcertaPagamentoRenda]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Recebimentos_AcertaPagamentoRenda]
(
	@Id int, -- transaction id
	@PaymentState int,
	@Notas varchar(50),
	@ValorAcerto decimal(10,2),
	@ValorEmDivida decimal(10,2),
	@SaldoCorrente decimal(10,2),
	@ValorRecebido decimal(10, 2),
	@TenantId int,
	@UpdatedDate datetime,
	@Success bit = 0 OUTPUT
)
AS

BEGIN

	DECLARE @CodTipoRecebimento int
	DECLARE @DataMovimento datetime

	SET NOCOUNT OFF;

	BEGIN TRY
	    BEGIN TRANSACTION ACERTO_RENDA;

			SELECT @DataMovimento = DataMovimento FROM Recebimentos WHERE Id = @Id;

			SELECT @CodTipoRecebimento = Id FROM TipoRecebimento
				WHERE Descricao LIKE 
					CASE 
						WHEN @PaymentState = 3 THEN '%atraso%' 
						ELSE '%certo%' 
					END;
			
			IF @PaymentState = 3 -- fully paid
			BEGIN
				SET @PaymentState = 1
			END

			IF @ValorAcerto = @ValorEmDivida
			BEGIN
				SET @ValorEmDivida = 0
			END

			UPDATE [Recebimentos] SET	
				[ValorRecebido] = @ValorRecebido,
				[ValorEmFalta] = @ValorEmDivida, 
				[Estado] = 1, -- acertos são sempre sobre valores totais em falta (1 = pago)
				[Renda] = 1,-- regularização de pagamentos - são sempre sobre rendas 
				[Notas] = @Notas
			WHERE Id = @Id;


			INSERT INTO [CC_Inquilinos] 
			(DataMovimento, UpdatedDate, ValorPago, ValorEmDivida, IdInquilino, Renda, ID_TipoRecebimento, Notas, TransactionId)
			VALUES 
			(@DataMovimento, @UpdatedDate, @ValorAcerto, @ValorEmDivida, @TenantId, 0, @CodTipoRecebimento, @Notas, @Id); 

			UPDATE [Inquilinos] 
				SET SaldoCorrente = @SaldoCorrente
			WHERE [Id] = @TenantId;

			-- 18-05-2023
			UPDATE Arrendamentos SET
				Data_Pagamento = @DataMovimento
			WHERE ID_Inquilino = @TenantId

			SELECT @Success = 1;

		COMMIT TRANSACTION ACERTO_RENDA;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN 
			SELECT @Success = 0;
			ROLLBACK TRANSACTION ACERTO_RENDA;                        
		END
		SELECT
			ERROR_NUMBER() AS ErrorNumber,
			ERROR_SEVERITY() AS ErrorSeverity,
			ERROR_STATE() AS ErrorState,
			ERROR_PROCEDURE() AS ErrorProcedure,
			ERROR_LINE() AS ErrorLine,
			ERROR_MESSAGE() AS ErrorMessage;
	END CATCH

END

GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_Delete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Recebimentos_Delete]
(
	@Id int
)
AS

SET NOCOUNT OFF;
DELETE FROM Recebimentos
WHERE [Id] = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_Delete_Temp]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Recebimentos_Delete_Temp]
AS

SET NOCOUNT OFF;
DELETE FROM RecebimentosTemp
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Recebimentos_GetAll]
AS

SET NOCOUNT ON;
SELECT	R.Id, DataMovimento, Estado, ValorPrevisto, ValorRecebido, Renda, ValorEmFalta, 
		ID_Propriedade, ID_TipoRecebimento, ID_Inquilino, GeradoPeloPrograma, R.Notas,
		F.Descricao Imovel, INQ.Nome Inquilino, TR.Descricao TipoRecebimento	
FROM	Recebimentos R 
		INNER JOIN Fracoes F ON
			R.ID_Propriedade = F.Id
		INNER JOIN Inquilinos INQ ON
			R.ID_Inquilino = INQ.Id
		LEFT JOIN TipoRecebimento TR ON
			R.ID_TipoRecebimento = TR.Id
-- WHERE Estado != 3 -- 3 = Não pago
ORDER BY R.DataMovimento Desc
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_GetLastPeriodProcessed]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Recebimentos_GetLastPeriodProcessed]
AS

SET NOCOUNT ON;

SELECT TOP 1 Mes, Ano
FROM ProcessamentoRendas
ORDER BY Ano Desc, Mes Desc
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_GetPaymentsByYear]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[usp_Recebimentos_GetPaymentsByYear]
	@year int
AS
BEGIN 
	SELECT 'Pagamento de Renda' Descricao,
	SUM(D.ValorRecebido) TotalPagamentos, COUNT(1) NumeroMovimentos
	FROM Recebimentos D
	WHERE D.Renda = 1 AND YEAR(D.DataMovimento) = @year
UNION
	SELECT TR.Descricao,
	SUM(D.ValorRecebido) TotalPagamentos, COUNT(1) NumeroMovimentos
	FROM Recebimentos D
	INNER JOIN TipoRecebimento TR
		ON D.ID_TipoRecebimento = TR.Id
	WHERE YEAR(D.DataMovimento) = @year
	GROUP BY TR.Descricao

END
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_GetRecebimento_ById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Recebimentos_GetRecebimento_ById] (@Id int)
AS

SET NOCOUNT ON;
SELECT	R.Id, DataMovimento, Estado, Renda, 
		ValorPrevisto, ValorRecebido, ValorEmFalta, ID_Propriedade, 
		ID_TipoRecebimento, ID_Inquilino, GeradoPeloPrograma, R.Notas, 
		F.Descricao Imovel, INQ.Nome Inquilino, TR.Descricao TipoRecebimento	
FROM	Recebimentos R 
		INNER JOIN Fracoes F ON
			R.ID_Propriedade = F.Id
		INNER JOIN Inquilinos INQ ON
			R.ID_Inquilino = INQ.Id
		LEFT JOIN TipoRecebimento TR ON
			R.ID_TipoRecebimento = TR.Id
WHERE	R.Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_GetRecebimento_Temp_ById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Recebimentos_GetRecebimento_Temp_ById] (@Id int)
AS

SET NOCOUNT ON;
SELECT	R.Id, DataMovimento, Estado, Renda, ValorPrevisto, ValorRecebido, ValorEmFalta, ID_Propriedade, 
		ID_TipoRecebimento, ID_Inquilino, GeradoPeloPrograma, R.Notas, 
		F.Descricao Imovel, INQ.Nome Inquilino, TR.Descricao TipoRecebimento	
FROM	RecebimentosTemp R 
		INNER JOIN Fracoes F ON
			R.ID_Propriedade = F.Id
		INNER JOIN Inquilinos INQ ON
			R.ID_Inquilino = INQ.Id
		LEFT JOIN TipoRecebimento TR ON
			R.ID_TipoRecebimento = TR.Id
WHERE	R.Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_GetRecebimento_TempById_ToDelete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Recebimentos_GetRecebimento_TempById_ToDelete] (@Id int)
AS

SET NOCOUNT ON;
SELECT	R.Id, DataMovimento, Estado, Renda, ValorRecebido, ValorEmFalta, ID_Propriedade, 
		ID_TipoRecebimento, ID_Inquilino, GeradoPeloPrograma, R.Notas, 
		F.Descricao Imovel, INQ.Nome Inquilino, TR.Descricao TipoRecebimento	
FROM	RecebimentosTemp R 
		INNER JOIN Fracoes F ON
			R.ID_Propriedade = F.Id
		INNER JOIN Inquilinos INQ ON
			R.ID_Inquilino = INQ.Id
		LEFT JOIN TipoRecebimento TR ON
			R.ID_TipoRecebimento = TR.Id
WHERE	R.Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_GetTotalByMonthAndYear]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Recebimentos_GetTotalByMonthAndYear] (@month int, @year int, @totalPaid decimal(10,2) out)
AS

SET NOCOUNT ON;

SELECT @totalPaid = sum(valorrecebido)
from RecebimentosTemp
WHERE Renda = 1 AND 
	Month(DataMovimento) = @month AND
	Year(DataMovimento) = @year
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_GetTotalReceived]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Recebimentos_GetTotalReceived] (@Id int = 0)
AS

SET NOCOUNT ON;

SELECT SUM(ValorRecebido)
FROM Recebimentos
WHERE ID_TipoRecebimento = 
	CASE 
		WHEN @Id > 0 THEN @Id ELSE ID_TipoRecebimento 
	END
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_GetValorUltimaRendaPaga_ById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Recebimentos_GetValorUltimaRendaPaga_ById] (@Id int)
AS

SET NOCOUNT ON;
SELECT TOP 1 ValorRecebido
FROM		Recebimentos
WHERE		ID_Inquilino = @Id AND
			Renda = 1
ORDER BY	DataMovimento Desc
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Recebimentos_Insert]
(
	@DataMovimento datetime,
	@Renda bit, 
	@Estado int,
	@ValorPrevisto decimal,
	@ValorRecebido decimal,
	@ValorEmFalta decimal,
	@ID_Propriedade int,
	@ID_TipoRecebimento int,
	@ID_Inquilino int,
	@GeradoPeloPrograma bit,
	@Notas varchar(512)
)
AS

SET NOCOUNT OFF;
INSERT INTO [Recebimentos] 
	([DataMovimento], [Estado], [Renda], [ValorPrevisto], [ValorRecebido], ValorEmFalta,
	[ID_Propriedade], [ID_TipoRecebimento], [ID_Inquilino], Notas)
VALUES 
	(@DataMovimento, @Estado, @Renda, @ValorPrevisto, @ValorRecebido, @ValorEmFalta,
	@ID_Propriedade, @ID_TipoRecebimento, @ID_Inquilino, @Notas)
	
SELECT CAST(SCOPE_IDENTITY() as int);
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_Insert_Temp]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Recebimentos_Insert_Temp]
(
	@DataMovimento datetime,
	@Renda bit, 
	@Estado int,
	@RendaAtualizada bit = 0,
	@ValorPrevisto decimal,
	@ValorRecebido decimal,
	@ValorEmFalta decimal,
	@ID_Propriedade int,
	@ID_TipoRecebimento int,
	@ID_Inquilino int,
	@GeradoPeloPrograma bit,
	@Notas varchar(512)
)
AS

SET NOCOUNT OFF;
INSERT INTO [RecebimentosTemp] 
	([DataMovimento], [Estado], [Renda], [RendaAtualizada], [ValorPrevisto], [ValorRecebido], [ValorEmFalta], 
	[ID_Propriedade], [ID_TipoRecebimento], [ID_Inquilino], Notas)
VALUES 
	(@DataMovimento, @Estado, @Renda, @RendaAtualizada, @ValorPrevisto, @ValorRecebido, @ValorEmFalta,
	@ID_Propriedade, @ID_TipoRecebimento, @ID_Inquilino, @Notas)
	
SELECT CAST(SCOPE_IDENTITY() as int);
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_MonthlyRentsProcessed]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Recebimentos_MonthlyRentsProcessed] (@year int)
AS

SET NOCOUNT ON;

SELECT Mes, Ano, DataProcessamento, TotalRecebido
FROM ProcessamentoRendas
WHERE Ano = @Year
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_OutrosRecebimentos_MaxValueAllowed_ByTenant]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Recebimentos_OutrosRecebimentos_MaxValueAllowed_ByTenant] (@Id int)
AS

SET NOCOUNT ON;

SELECT SUM(ValorPrevisto) - SUM(ValorRecebido)
FROM Recebimentos
WHERE ID_Inquilino = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_ProcessamentoAtualizacaoRendas_Insert_ToDelete]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Recebimentos_ProcessamentoAtualizacaoRendas_Insert_ToDelete]
(
	@Ano int,
	@DataProcessamento datetime
)
AS

SET NOCOUNT OFF;
INSERT INTO [ProcessamentoRendas] 
	([Ano], [DataProcessamento])
VALUES 
	(@Ano, @DataProcessamento)
	
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_ProcessamentoRendas_ById]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Recebimentos_ProcessamentoRendas_ById] (@Id int)
AS

SET NOCOUNT ON;
SELECT	Id, Mes, Ano, DataProcessamento, TotalRecebido
FROM	ProcessamentoRendas
WHERE	Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_ProcessamentoRendas_Check]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Recebimentos_ProcessamentoRendas_Check]
(
	@month int, 
	@year int
)
AS

SET NOCOUNT OFF;
SELECT COUNT(1)
FROM [ProcessamentoRendas] 
WHERE Mes = @month AND Ano = @year
	
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_ProcessamentoRendas_Insert]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Recebimentos_ProcessamentoRendas_Insert] (@month int, @year int, @TotalRecebido decimal(12,2))

AS

SET NOCOUNT OFF;
DECLARE @ProcessingDate date

SELECT @ProcessingDate = datefromparts(@year, @month, 9);

INSERT INTO [ProcessamentoRendas] 
	([Mes], [Ano], [DataProcessamento], [TotalRecebido])
VALUES 
	(@month, @year, @ProcessingDate, @TotalRecebido)
	
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_SetAsNotPaid]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Recebimentos_SetAsNotPaid]
(
	@Id int
)
AS

BEGIN
	SET NOCOUNT OFF;

	DECLARE @rentValue decimal 
	SET @rentValue = (SELECT ValorRecebido FROM Recebimentos WHERE Id = @Id);
	UPDATE Recebimentos SET Estado = 3, ValorEmFalta = ValorPrevisto, ValorRecebido = 0
	WHERE [Id] = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_Temp_GetAll]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Recebimentos_Temp_GetAll]
AS

SET NOCOUNT ON;
SELECT	R.Id, DataMovimento, Estado, ValorPrevisto, ValorRecebido, Renda, RendaAtualizada, ValorEmFalta, 
		ID_Propriedade, ID_TipoRecebimento, ID_Inquilino, GeradoPeloPrograma, R.Notas,
		F.Descricao Imovel, INQ.Nome Inquilino, TR.Descricao TipoRecebimento	
FROM	RecebimentosTemp R 
		INNER JOIN Fracoes F ON
			R.ID_Propriedade = F.Id
		INNER JOIN Inquilinos INQ ON
			R.ID_Inquilino = INQ.Id
		LEFT JOIN TipoRecebimento TR ON
			R.ID_TipoRecebimento = TR.Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_Temp_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Recebimentos_Temp_Update]
(
	@Id int,
	@DataMovimento datetime,
	@Estado int,
	@Renda bit,
	@RendaAtualizada bit = 0,
	@ValorPrevisto decimal,
	@ValorRecebido decimal,
	@ID_Propriedade int,
	@ID_TipoRecebimento int,
	@ID_Inquilino int,
	@Notas varchar(512),
	@ValorEmFalta decimal,
	@GeradoPeloPrograma bit
)
AS

SET NOCOUNT OFF;
UPDATE [RecebimentosTemp] 
SET	[DataMovimento] = @DataMovimento, [Estado] = @Estado, [Renda] = @Renda, [RendaAtualizada] = @RendaAtualizada,
	[ValorPrevisto] = @ValorPrevisto, [ValorRecebido] = @ValorRecebido, [ValorEmFalta] = @ValorEmFalta, 
	[Notas] = @Notas,[ID_Propriedade] = @ID_Propriedade, 
	[ID_TipoRecebimento] = @ID_TipoRecebimento,	[ID_Inquilino] = @ID_Inquilino
WHERE Id = @Id	
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_TotalExpected_ByTenant]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Recebimentos_TotalExpected_ByTenant] (@Id int)
AS

SET NOCOUNT ON;

SELECT SUM(ValorPrevisto)
FROM Recebimentos
WHERE ID_Inquilino = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_TotalReceived_ByTenant]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_Recebimentos_TotalReceived_ByTenant] (@Id int)
AS

SET NOCOUNT ON;

SELECT SUM(ValorRecebido)
FROM Recebimentos
WHERE ID_Inquilino = @Id
GO
/****** Object:  StoredProcedure [dbo].[usp_Recebimentos_Update]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Recebimentos_Update]
(
	@Id int,
	@DataMovimento datetime,
	@Estado int,
	@Renda bit,
	@ValorPrevisto decimal,
	@ValorRecebido decimal,
	@ID_Propriedade int,
	@ID_TipoRecebimento int,
	@ID_Inquilino int,
	@Notas varchar(512),
	@ValorEmFalta decimal,
	@GeradoPeloPrograma bit
)
AS

SET NOCOUNT OFF;
UPDATE [Recebimentos] 
SET	[DataMovimento] = @DataMovimento, [Estado] = @Estado, [Renda] = @Renda, 
	[ValorPrevisto] = @ValorPrevisto, [ValorRecebido] = @ValorRecebido, [ValorEmFalta] = @ValorEmFalta, 
	[Notas] = @Notas,[ID_Propriedade] = @ID_Propriedade, 
	[ID_TipoRecebimento] = @ID_TipoRecebimento,	[ID_Inquilino] = @ID_Inquilino
WHERE Id = @Id	
GO
/****** Object:  StoredProcedure [dbo].[usp_ShowStoredProcedures]    Script Date: 23/01/2024 10:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- display create datetime and last modified datetime
CREATE PROC [dbo].[usp_ShowStoredProcedures]
AS
BEGIN
	select 
	 [database name] = db_name() 
	,[schema name] =  SCHEMA_NAME([schema_id])
	,name [stored proc name]
	,create_date [create date]
	,modify_date [last modify date]
	from sys.objects
	where type = 'P'
	order by [stored proc name] asc
END
GO
