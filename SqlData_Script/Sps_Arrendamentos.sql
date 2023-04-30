IF EXISTS (SELECT * FROM sysobjects WHERE name = 'usp_Arrendamento_GetAll' AND user_name(uid) = 'dbo')
	DROP PROCEDURE [dbo].usp_Arrendamento_GetAll
GO

CREATE PROCEDURE [dbo].usp_Arrendamento_GetAll
AS
	SET NOCOUNT ON;
SELECT        Id, Data_Inicio, Data_Fim, Data_Pagamento, Fiador, Prazo_Meses, Valor_Renda, Doc_IRS, Doc_Vencimento, Notas, ID_Fracao, ID_Inquilino, ID_Fiador, Caucao, ContratoEmitido, DocumentooGerado, Data_Saida, 
                         FormaPagamento, Ativo, ArrendamentoNovo, EstadoPagamento, RenovacaoAutomatica
FROM            Arrendamento
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'usp_Arrendamento_Insert' AND user_name(uid) = 'dbo')
	DROP PROCEDURE [dbo].usp_Arrendamento_Insert
GO

CREATE PROCEDURE [dbo].usp_Arrendamento_Insert
(
	@Data_Inicio datetime,
	@Data_Fim datetime,
	@Data_Pagamento datetime,
	@Fiador bit,
	@Prazo_Meses int,
	@Valor_Renda decimal(10, 2),
	@Doc_IRS bit,
	@Doc_Vencimento bit,
	@Notas text,
	@ID_Fracao int,
	@ID_Inquilino int,
	@ID_Fiador int,
	@Caucao bit,
	@ContratoEmitido bit,
	@DocumentooGerado varchar(255),
	@Data_Saida datetime,
	@FormaPagamento int,
	@Ativo bit,
	@ArrendamentoNovo bit,
	@EstadoPagamento varchar(15),
	@RenovacaoAutomatica bit
)
AS
	SET NOCOUNT OFF;
INSERT INTO [Arrendamento] ([Data_Inicio], [Data_Fim], [Data_Pagamento], [Fiador], [Prazo_Meses], [Valor_Renda], [Doc_IRS], [Doc_Vencimento], [Notas], [ID_Fracao], [ID_Inquilino], [ID_Fiador], [Caucao], [ContratoEmitido], [DocumentooGerado], [Data_Saida], [FormaPagamento], [Ativo], [ArrendamentoNovo], [EstadoPagamento], [RenovacaoAutomatica]) VALUES (@Data_Inicio, @Data_Fim, @Data_Pagamento, @Fiador, @Prazo_Meses, @Valor_Renda, @Doc_IRS, @Doc_Vencimento, @Notas, @ID_Fracao, @ID_Inquilino, @ID_Fiador, @Caucao, @ContratoEmitido, @DocumentooGerado, @Data_Saida, @FormaPagamento, @Ativo, @ArrendamentoNovo, @EstadoPagamento, @RenovacaoAutomatica);
	
SELECT Id, Data_Inicio, Data_Fim, Data_Pagamento, Fiador, Prazo_Meses, Valor_Renda, Doc_IRS, Doc_Vencimento, Notas, ID_Fracao, ID_Inquilino, ID_Fiador, Caucao, ContratoEmitido, DocumentooGerado, Data_Saida, FormaPagamento, Ativo, ArrendamentoNovo, EstadoPagamento, RenovacaoAutomatica FROM Arrendamento WHERE (Id = SCOPE_IDENTITY())
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'usp_Arrendamento_Update' AND user_name(uid) = 'dbo')
	DROP PROCEDURE [dbo].usp_Arrendamento_Update
GO

CREATE PROCEDURE [dbo].usp_Arrendamento_Update
(
	@Data_Inicio datetime,
	@Data_Fim datetime,
	@Data_Pagamento datetime,
	@Fiador bit,
	@Prazo_Meses int,
	@Valor_Renda decimal(10, 2),
	@Doc_IRS bit,
	@Doc_Vencimento bit,
	@Notas text,
	@ID_Fracao int,
	@ID_Inquilino int,
	@ID_Fiador int,
	@Caucao bit,
	@ContratoEmitido bit,
	@DocumentooGerado varchar(255),
	@Data_Saida datetime,
	@FormaPagamento int,
	@Ativo bit,
	@ArrendamentoNovo bit,
	@EstadoPagamento varchar(15),
	@RenovacaoAutomatica bit,
	@Original_Id int,
	@Original_Data_Inicio datetime,
	@Original_Data_Fim datetime,
	@Original_Data_Pagamento datetime,
	@Original_Fiador bit,
	@Original_Prazo_Meses int,
	@Original_Valor_Renda decimal(10, 2),
	@Original_Doc_IRS bit,
	@Original_Doc_Vencimento bit,
	@Original_ID_Fracao int,
	@Original_ID_Inquilino int,
	@Original_ID_Fiador int,
	@IsNull_Caucao Int,
	@Original_Caucao bit,
	@IsNull_ContratoEmitido Int,
	@Original_ContratoEmitido bit,
	@IsNull_DocumentooGerado Int,
	@Original_DocumentooGerado varchar(255),
	@IsNull_Data_Saida Int,
	@Original_Data_Saida datetime,
	@Original_FormaPagamento int,
	@IsNull_Ativo Int,
	@Original_Ativo bit,
	@Original_ArrendamentoNovo bit,
	@Original_EstadoPagamento varchar(15),
	@Original_RenovacaoAutomatica bit,
	@Id int
)
AS
	SET NOCOUNT OFF;
UPDATE [Arrendamento] SET [Data_Inicio] = @Data_Inicio, [Data_Fim] = @Data_Fim, [Data_Pagamento] = @Data_Pagamento, [Fiador] = @Fiador, [Prazo_Meses] = @Prazo_Meses, [Valor_Renda] = @Valor_Renda, [Doc_IRS] = @Doc_IRS, [Doc_Vencimento] = @Doc_Vencimento, [Notas] = @Notas, [ID_Fracao] = @ID_Fracao, [ID_Inquilino] = @ID_Inquilino, [ID_Fiador] = @ID_Fiador, [Caucao] = @Caucao, [ContratoEmitido] = @ContratoEmitido, [DocumentooGerado] = @DocumentooGerado, [Data_Saida] = @Data_Saida, [FormaPagamento] = @FormaPagamento, [Ativo] = @Ativo, [ArrendamentoNovo] = @ArrendamentoNovo, [EstadoPagamento] = @EstadoPagamento, [RenovacaoAutomatica] = @RenovacaoAutomatica WHERE (([Id] = @Original_Id) AND ([Data_Inicio] = @Original_Data_Inicio) AND ([Data_Fim] = @Original_Data_Fim) AND ([Data_Pagamento] = @Original_Data_Pagamento) AND ([Fiador] = @Original_Fiador) AND ([Prazo_Meses] = @Original_Prazo_Meses) AND ([Valor_Renda] = @Original_Valor_Renda) AND ([Doc_IRS] = @Original_Doc_IRS) AND ([Doc_Vencimento] = @Original_Doc_Vencimento) AND ([ID_Fracao] = @Original_ID_Fracao) AND ([ID_Inquilino] = @Original_ID_Inquilino) AND ([ID_Fiador] = @Original_ID_Fiador) AND ((@IsNull_Caucao = 1 AND [Caucao] IS NULL) OR ([Caucao] = @Original_Caucao)) AND ((@IsNull_ContratoEmitido = 1 AND [ContratoEmitido] IS NULL) OR ([ContratoEmitido] = @Original_ContratoEmitido)) AND ((@IsNull_DocumentooGerado = 1 AND [DocumentooGerado] IS NULL) OR ([DocumentooGerado] = @Original_DocumentooGerado)) AND ((@IsNull_Data_Saida = 1 AND [Data_Saida] IS NULL) OR ([Data_Saida] = @Original_Data_Saida)) AND ([FormaPagamento] = @Original_FormaPagamento) AND ((@IsNull_Ativo = 1 AND [Ativo] IS NULL) OR ([Ativo] = @Original_Ativo)) AND ([ArrendamentoNovo] = @Original_ArrendamentoNovo) AND ([EstadoPagamento] = @Original_EstadoPagamento) AND ([RenovacaoAutomatica] = @Original_RenovacaoAutomatica));
	
SELECT Id, Data_Inicio, Data_Fim, Data_Pagamento, Fiador, Prazo_Meses, Valor_Renda, Doc_IRS, Doc_Vencimento, Notas, ID_Fracao, ID_Inquilino, ID_Fiador, Caucao, ContratoEmitido, DocumentooGerado, Data_Saida, FormaPagamento, Ativo, ArrendamentoNovo, EstadoPagamento, RenovacaoAutomatica FROM Arrendamento WHERE (Id = @Id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'usp_Arrendamento_Delete' AND user_name(uid) = 'dbo')
	DROP PROCEDURE [dbo].usp_Arrendamento_Delete
GO

CREATE PROCEDURE [dbo].usp_Arrendamento_Delete
(
	@Original_Id int,
	@Original_Data_Inicio datetime,
	@Original_Data_Fim datetime,
	@Original_Data_Pagamento datetime,
	@Original_Fiador bit,
	@Original_Prazo_Meses int,
	@Original_Valor_Renda decimal(10, 2),
	@Original_Doc_IRS bit,
	@Original_Doc_Vencimento bit,
	@Original_ID_Fracao int,
	@Original_ID_Inquilino int,
	@Original_ID_Fiador int,
	@IsNull_Caucao Int,
	@Original_Caucao bit,
	@IsNull_ContratoEmitido Int,
	@Original_ContratoEmitido bit,
	@IsNull_DocumentooGerado Int,
	@Original_DocumentooGerado varchar(255),
	@IsNull_Data_Saida Int,
	@Original_Data_Saida datetime,
	@Original_FormaPagamento int,
	@IsNull_Ativo Int,
	@Original_Ativo bit,
	@Original_ArrendamentoNovo bit,
	@Original_EstadoPagamento varchar(15),
	@Original_RenovacaoAutomatica bit
)
AS
	SET NOCOUNT OFF;
DELETE FROM [Arrendamento] WHERE (([Id] = @Original_Id) AND ([Data_Inicio] = @Original_Data_Inicio) AND ([Data_Fim] = @Original_Data_Fim) AND ([Data_Pagamento] = @Original_Data_Pagamento) AND ([Fiador] = @Original_Fiador) AND ([Prazo_Meses] = @Original_Prazo_Meses) AND ([Valor_Renda] = @Original_Valor_Renda) AND ([Doc_IRS] = @Original_Doc_IRS) AND ([Doc_Vencimento] = @Original_Doc_Vencimento) AND ([ID_Fracao] = @Original_ID_Fracao) AND ([ID_Inquilino] = @Original_ID_Inquilino) AND ([ID_Fiador] = @Original_ID_Fiador) AND ((@IsNull_Caucao = 1 AND [Caucao] IS NULL) OR ([Caucao] = @Original_Caucao)) AND ((@IsNull_ContratoEmitido = 1 AND [ContratoEmitido] IS NULL) OR ([ContratoEmitido] = @Original_ContratoEmitido)) AND ((@IsNull_DocumentooGerado = 1 AND [DocumentooGerado] IS NULL) OR ([DocumentooGerado] = @Original_DocumentooGerado)) AND ((@IsNull_Data_Saida = 1 AND [Data_Saida] IS NULL) OR ([Data_Saida] = @Original_Data_Saida)) AND ([FormaPagamento] = @Original_FormaPagamento) AND ((@IsNull_Ativo = 1 AND [Ativo] IS NULL) OR ([Ativo] = @Original_Ativo)) AND ([ArrendamentoNovo] = @Original_ArrendamentoNovo) AND ([EstadoPagamento] = @Original_EstadoPagamento) AND ([RenovacaoAutomatica] = @Original_RenovacaoAutomatica))
GO

