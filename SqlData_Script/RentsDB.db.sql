BEGIN TRANSACTION;
INSERT INTO HelpIndex_Parent (Id,NomeProjeto,NomeExe,NomeWord,NomePdf,Descricao,dCriacao) VALUES (1,'HouseRentalSoft','C:\GitProjects\HouseRentalSoft\HouseRentalSoft\bin\Debug\PropertyRentalSoft.exe','','','Gestão de Arrendamentos','2020-01-08 11:45:00');
INSERT INTO HelpIndex (Id,NomeForm,Titulo,Descricao,Pagina,ID_Parent) VALUES (1,'FrmArrendamento','Gestão de Arrendamentos','{\rtf1\ansi\ansicpg1252\deff0\deflang2070{\fonttbl{\f0\fnil\fcharset0 Calibri;}}

\viewkind4\uc1\pard\f0\fs23 Gest\''e3o de arrendamentos.\par

}

',0,1);
INSERT INTO HelpIndex (Id,NomeForm,Titulo,Descricao,Pagina,ID_Parent) VALUES (2,'frmDespesas','Gestão de Despesas','{\rtf1\ansi\ansicpg1252\deff0{\fonttbl{\f0\fnil\fcharset0 Segoe UI;}}

\viewkind4\uc1\pard\lang2070\f0\fs23 Texto de ajuda para despesas.\par

}

',0,1);
INSERT INTO HelpIndex (Id,NomeForm,Titulo,Descricao,Pagina,ID_Parent) VALUES (3,'FrmFracoes','Gestão de Frações','{\rtf1\ansi\deff0{\fonttbl{\f0\fnil\fcharset0 Segoe UI;}}

\viewkind4\uc1\pard\lang2070\f0\fs23 Texto de Ajuda de fra\''e7\''f5es\par

}

',0,1);
INSERT INTO TipoContacto (Id,Descricao,Notas) VALUES (1,'Pinturas','Notas sobre pinturas');
INSERT INTO TipoContacto (Id,Descricao,Notas) VALUES (2,'Limpezas','Notas sobre limpezas');
INSERT INTO TipoContacto (Id,Descricao,Notas) VALUES (3,'Imobiliária','');
INSERT INTO TipoContacto (Id,Descricao,Notas) VALUES (4,'Advogado','');
INSERT INTO TipoContacto (Id,Descricao,Notas) VALUES (5,'Obras - Geral','');
INSERT INTO TipoContacto (Id,Descricao,Notas) VALUES (6,'Úteis','');
INSERT INTO TipologiaFracao (Id,Descricao) VALUES (1,'T0');
INSERT INTO TipologiaFracao (Id,Descricao) VALUES (2,'T1');
INSERT INTO TipologiaFracao (Id,Descricao) VALUES (3,'T2');
INSERT INTO TipologiaFracao (Id,Descricao) VALUES (4,'T3');
INSERT INTO TipologiaFracao (Id,Descricao) VALUES (5,'T4');
INSERT INTO TipologiaFracao (Id,Descricao) VALUES (6,'T5');
INSERT INTO TipoPropriedade (Id,Descricao) VALUES (1,'Outro tipo de propriedade');
INSERT INTO TipoPropriedade (Id,Descricao) VALUES (2,'Apartamento');
INSERT INTO TipoPropriedade (Id,Descricao) VALUES (3,'Escritório');
INSERT INTO TipoPropriedade (Id,Descricao) VALUES (4,'Loja');
INSERT INTO TipoPropriedade (Id,Descricao) VALUES (5,'Terreno');
INSERT INTO TipoPropriedade (Id,Descricao) VALUES (6,'Vivenda');
INSERT INTO TipoRecebimento (Id,Descricao) VALUES (1,'Pagamento de multa');
INSERT INTO TipoRecebimento (Id,Descricao) VALUES (2,'Acerto de pagamento atrasado');
INSERT INTO TipoRecebimento (Id,Descricao) VALUES (5,'Pagamento de renda');
INSERT INTO TipoRecebimento (Id,Descricao) VALUES (7,'Não classificado');
INSERT INTO SituacaoFracao (Id,Descricao) VALUES (1,'Alugada');
INSERT INTO SituacaoFracao (Id,Descricao) VALUES (2,'Livre');
INSERT INTO SituacaoFracao (Id,Descricao) VALUES (3,'Reservada');
INSERT INTO SituacaoFracao (Id,Descricao) VALUES (4,'Vendida');
INSERT INTO EstadoConservacao (Id,Descricao) VALUES (1,'Bom');
INSERT INTO EstadoConservacao (Id,Descricao) VALUES (2,'A precisar de intervenção');
INSERT INTO EstadoConservacao (Id,Descricao) VALUES (3,'Degradado');
INSERT INTO EstadoCivil (Id,Descricao) VALUES (1,'Solteiro');
INSERT INTO EstadoCivil (Id,Descricao) VALUES (2,'Casado');
INSERT INTO EstadoCivil (Id,Descricao) VALUES (3,'Divorciado');
INSERT INTO EstadoCivil (Id,Descricao) VALUES (4,'Viúvo');
INSERT INTO EstadoCivil (Id,Descricao) VALUES (5,'Separado');
INSERT INTO EstadoCivil (Id,Descricao) VALUES (6,'Desconhecido');
--INSERT INTO CategoriaDespesa (Id,Descricao) VALUES (1,'Impostos');
--INSERT INTO CategoriaDespesa (Id,Descricao) VALUES (2,'Obras de beneficiação no Imóvel');
--INSERT INTO CategoriaDespesa (Id,Descricao) VALUES (3,'Obras de beneficiação na fração');
--INSERT INTO CategoriaDespesa (Id,Descricao) VALUES (4,'Despesas correntes no Imóvel');
--INSERT INTO CategoriaDespesa (Id,Descricao) VALUES (5,'Comissões');
--INSERT INTO CategoriaDespesa (Id,Descricao) VALUES (6,'Sem classificação');
INSERT INTO TipoDespesa (id, descricao, Id_CategoriaDespesa) VALUES (1,'Eletricidade',4);
INSERT INTO TipoDespesa (id, descricao, Id_CategoriaDespesa) VALUES (2,'Limpeza da escada',4);
INSERT INTO TipoDespesa (id, descricao, Id_CategoriaDespesa) VALUES (3,'IMI',1);
INSERT INTO TipoDespesa (id, descricao, Id_CategoriaDespesa) VALUES (4,'IRS',1);
INSERT INTO TipoDespesa (id, descricao, Id_CategoriaDespesa) VALUES (5,'Água',4);
INSERT INTO TipoDespesa (id, descricao, Id_CategoriaDespesa) VALUES (6,'Obras em frações',3);
INSERT INTO TipoDespesa (id, descricao, Id_CategoriaDespesa) VALUES (7,'Condomínio',4);
INSERT INTO TipoDespesa (id, descricao, Id_CategoriaDespesa) VALUES (8,'Consumíveis',4);
INSERT INTO TipoDespesa (id, descricao, Id_CategoriaDespesa) VALUES (9,'Obras em Imóvel',2);
INSERT INTO TipoDespesa (id, descricao, Id_CategoriaDespesa) VALUES (10,'Advogados',6);
INSERT INTO TipoDespesa (id, descricao, Id_CategoriaDespesa) VALUES (11,'Não classificada',6);
INSERT INTO Contactos (id, nome, morada, Localidade, Contacto, email, notas, ID_TipoContacto) VALUES (1,'aYSF2iG3Be0WAsGn4qnPVpBszcBtWYda','ouoxj6K0np8=','ouoxj6K0np8=','iogxK3yWWyOn6epaDynveQ==','','T8UeasAFUCPjSQGmWzqduWJW1wEuqaBggayMn+5/VPChf+dZWIB49Blqnhc5q1mVhEwAMc+HeJU=',5);
INSERT INTO Contactos (id, nome, morada, Localidade, Contacto, email, notas, ID_TipoContacto) VALUES (2,'IHMe9MuKyy7cnMckyJAX9Q==','ouoxj6K0np8=','ouoxj6K0np8=','jn9UDEdYVwLMrreAHjJhRw==','','ONsY5wApq2GWMX5pYEVFtqwMLGsOESDM',3);
INSERT INTO Contactos (id, nome, morada, Localidade, Contacto, email, notas, ID_TipoContacto) VALUES (3,'yUe8/qlUnjyuqdJQdKNtGg==','MCEol1zLKI2ZcJw7f9U64AN9FY+ntMBt/D/hqG+K+gA=','ouoxj6K0np8=','ccRHGCeXGOa6rfMtZ/RCtA==','','fsEuappzI7zQjlyWYPEH15rv9kuv44g3syB8lQru5WijkUKXPmbWQh8ckeXaK92q',3);
INSERT INTO Contactos (id, nome, morada, Localidade, Contacto, email, notas, ID_TipoContacto) VALUES (4,'RGcVyGGEgb8=','9BZvBUlCcXkMJ+giTZbrh1Fug8/PrfuMr4ET7mX9ULU=','upsmYCU6sOaR7gvfafr6zQ==','q4ZBSpNVrUhaFukz9dAc2Q==','dNvs/aVkUFgZwZ/BgiwXFPEu7zZ/3le+','B4VmyduBX9AGSzTig8OBRKlDRYTWn79/Nxjd7F/CrzoDh6y9BFYWTOnTTDht1ISQJDgVyTeXjT1IgLp+hAtYH2YMlcQvMV7nub1FNx5h2mI4MPF6WUjme67Xy/gCKXfW',6);
INSERT INTO Contactos (id, nome, morada, Localidade, Contacto, email, notas, ID_TipoContacto) VALUES (5,'1LuQv9BoYz9vTn4dn1q1yTYgBpAcnterMSKoR5/I3Zk=','Q5nbhK1lsgP+Za/XZB9giniDJ4b9Qw1LOv3zrDKiEns=','3N0FTZ27m691qQeekA1pxFrliSdprWi/','uU2Z6TJKRzz5855Z2xPcfA==','','zXhKOzKmX6DblSkVfGcTQV/wCuKQnIx/',3);
INSERT INTO Imoveis (Id,Descricao,Numero,Morada,CodPst,CodPstEx,AnoConstrucao,Freguesia,Concelho,Elevador,Notas,Conservacao,LicencaHabitacao,DataEmissaoLicencaHabitacao) VALUES (1,'R. C. G. Guerra','58','Montijo','2870','000','2001','Montijo e Afonsoeiro','Montijo',0,'',1,'abc12345','1999-09-29 12:50:41');
INSERT INTO Imoveis (Id,Descricao,Numero,Morada,CodPst,CodPstEx,AnoConstrucao,Freguesia,Concelho,Elevador,Notas,Conservacao,LicencaHabitacao,DataEmissaoLicencaHabitacao) VALUES (2,'Rua José Joaquim Marques','4','Montijo','2870','348','2001','Montijo e Afonsoeiro','Montijo',0,'',1,'6d586d','1999-09-29 12:50:41');
INSERT INTO Imoveis (Id,Descricao,Numero,Morada,CodPst,CodPstEx,AnoConstrucao,Freguesia,Concelho,Elevador,Notas,Conservacao,LicencaHabitacao,DataEmissaoLicencaHabitacao) VALUES (3,'Rua Professor Bernardo da Costa','104','Montijo','2870','011','2003','Montijo e Afonsoeiro','Montijo',0,'Prédio precisa de pintura.

Já foi feito orçamento, mas aguarda (há bastante tempo) por decisão do condomínio.',2,'abc2113','2003-09-29 12:50:41');
INSERT INTO Fracoes VALUES (1,'3º Dto JJM',200.4,44.5,0,1,0,0,0,'',2,5,1,'6783','2015',133060,2,2,1,'3º','Dto');
INSERT INTO Fracoes VALUES (2,'Escritório do 1º Fte (JJM)',95.34,21.185,0,0,0,0,0,'Massagista.',3,3,1,'99991','2015',67650,2,2,1,'1º','Fte');
INSERT INTO Fracoes VALUES (3,'2º Dto (JJM)',140.0,30.0,0,0,0,0,0,'',2,5,1,'99992','2015',137500,2,2,1,'2º','Dto');
INSERT INTO Fracoes VALUES (4,'Fração autónoma B (R/C) CCG',105.95,38.4,0,1,0,0,0,'Tem terraço.

Barbeiro.

Foi baixada a renda em 50 euros...',2,4,1,'7645','2015',71360,1,1,1,'R/C','---');
INSERT INTO Fracoes VALUES (5,'Fração autónoma C (1º andar) CGG',105.9,49.5,0,1,0,0,0,'Nídia',2,4,1,'11049','2015',73500,1,1,1,'1º','---');
INSERT INTO Fracoes VALUES (6,'2º Andar PBC',80.35,0.0,0,0,0,0,0,'Área dependente não está informada; será por não ter arrecadação? Veificar se tem varanda.

',2,3,0,'6166','2015',45590,3,2,1,'2º','Dto');
INSERT INTO Fracoes VALUES (7,'3º Esq (JJM)',92.25,59.7,0,1,0,0,0,'Habitada por mim desde Março 2013.',2,3,1,'6798','2015',72730,2,2,1,'3º','Esq');
INSERT INTO Fracoes VALUES (8,'3º piso (CGG)',110.0,12.0,0,1,0,0,0,'',2,4,1,'424124','2015',71000,1,2,1,'2','---');
INSERT INTO Fracoes VALUES (9,'Escritório do 2º Fte (JJM)',45.0,0.0,0,0,0,0,0,'',3,3,0,'8767','2015',67650,2,2,1,'2º','Fte');
INSERT INTO Proprietarios (Id,Nome,Morada,Naturalidade,EstadoCivil,DataNascimento,Identificacao,ValidadeCC,NIF,Contacto,eMail,Notas) VALUES (4,'Laura de Jesus','Rua Quinta das Palmeiras, 89 - 4 Dto','Oeiras',4,'1930-01-02 02:32:25','1122334','2025-09-29 02:32:25','136126820','214571743','','');
INSERT INTO Proprietarios (Id,Nome,Morada,Naturalidade,EstadoCivil,DataNascimento,Identificacao,ValidadeCC,NIF,Contacto,eMail,Notas) VALUES (5,'gdfgsg','gsdgdfg','dgsgsdg',3,'1960-12-01 02:35:36','525452','2025-09-29 02:35:36','122623878','','','');
INSERT INTO RolesDetail (Id,Descricao) VALUES (1,'Admin');
INSERT INTO RolesDetail (Id,Descricao) VALUES (2,'Utilizador');
INSERT INTO FormaPagamento (Id,Descricao) VALUES (1,'Dinheiro');
INSERT INTO FormaPagamento (Id,Descricao) VALUES (2,'Transferência');
INSERT INTO FormaPagamento (Id,Descricao) VALUES (3,'Cheque');
INSERT INTO Inquilinos (Id,Nome,Morada,Naturalidade,EstadoCivil,Contacto1,Contacto2,eMail,IRSAnual,Vencimento,Notas,NIF,Identificacao,Titular,ValidadeCC,DataNascimento,Ativo,SaldoCorrente) VALUES (1,'Fausto Luís JJM 3 Esq','Av D. Dinis, 106 - 2º Dto - Odivelas','Oleiros',3,'937262753','','fauxtix.luix@hotmail.com',27000,1200,'','122623878','43625894',1,'2018-09-28 13:06:54','1960-12-01 13:06:54',1,0);
INSERT INTO Inquilinos (Id,Nome,Morada,Naturalidade,EstadoCivil,Contacto1,Contacto2,eMail,IRSAnual,Vencimento,Notas,NIF,Identificacao,Titular,ValidadeCC,DataNascimento,Ativo,SaldoCorrente) VALUES (2,'Ana Filipa Oliveira Luís 3 Esq','Ribeirada (Odivelas)','Lisboa',2,'397001002','','FilipaL29@gmail.com',25000,1500,'Filha do Autor deste pequeno programa...','122623878','545465',0,'2025-10-20 14:03:16','1989-09-26 14:03:16',1,0);
INSERT INTO Inquilinos (Id,Nome,Morada,Naturalidade,EstadoCivil,Contacto1,Contacto2,eMail,IRSAnual,Vencimento,Notas,NIF,Identificacao,Titular,ValidadeCC,DataNascimento,Ativo,SaldoCorrente) VALUES (3,'Jose Antonio Bonifacio JJM 2 Fte','Morada do Inquilino da JJM 2 Fte','Lisboa',2,'931258741','','',20000,1200,'','122623878','11223344',1,'2025-10-24 20:52:29','1975-11-01 20:52:29',1,0);
INSERT INTO Inquilinos (Id,Nome,Morada,Naturalidade,EstadoCivil,Contacto1,Contacto2,eMail,IRSAnual,Vencimento,Notas,NIF,Identificacao,Titular,ValidadeCC,DataNascimento,Ativo,SaldoCorrente) VALUES (4,'Nome do fiador JJM 2 Fte','Morada do fiador JJM 2 Fte','Lisboa',2,'937258471','','',17500,1100,'','122623878','4554654',0,'2021-10-24 20:54:30','1989-10-01 20:54:30','true',0);
INSERT INTO Inquilinos (Id,Nome,Morada,Naturalidade,EstadoCivil,Contacto1,Contacto2,eMail,IRSAnual,Vencimento,Notas,NIF,Identificacao,Titular,ValidadeCC,DataNascimento,Ativo,SaldoCorrente) VALUES (5,'Nome do Inquilino do 1 fte JJM','Morada do Inquilino do 1 fte JJM','Lisboa',5,'932582129','','',15000,920,'','122623878','363656',1,'2022-10-26 16:44:39','1970-12-01 16:44:39','true',0);
INSERT INTO Inquilinos (Id,Nome,Morada,Naturalidade,EstadoCivil,Contacto1,Contacto2,eMail,IRSAnual,Vencimento,Notas,NIF,Identificacao,Titular,ValidadeCC,DataNascimento,Ativo,SaldoCorrente) VALUES (6,'Nome do Fiador do 1 fte JJM','Morada do Inquilino do 1 fte JJM','Lisboa',5,'931247852','','',20000,1000,'','122623878','6546546',0,'2028-10-26 16:47:38','1989-05-14 16:47:38','true',0);
INSERT INTO Inquilinos (Id,Nome,Morada,Naturalidade,EstadoCivil,Contacto1,Contacto2,eMail,IRSAnual,Vencimento,Notas,NIF,Identificacao,Titular,ValidadeCC,DataNascimento,Ativo,SaldoCorrente) VALUES (7,'Nome do Inquilino do 3º dto JJM','Morada do Inquilino do 3º dto','Coimbra',2,'931584781','','',18500,1000,'','122623878','65464645',1,'2019-11-01 18:54:56.8437898','1989-11-01 18:54:56',1,0);
INSERT INTO Inquilinos (Id,Nome,Morada,Naturalidade,EstadoCivil,Contacto1,Contacto2,eMail,IRSAnual,Vencimento,Notas,NIF,Identificacao,Titular,ValidadeCC,DataNascimento,Ativo,SaldoCorrente) VALUES (8,'Nome do fiador do 3º dto','Morada do fiador do 3º dto','Leiria',4,'931025847','','',25000,1780,'','122623878','4654564',0,'2025-11-01 18:56:26','1959-10-01 18:56:26','true',0);
INSERT INTO Inquilinos (Id,Nome,Morada,Naturalidade,EstadoCivil,Contacto1,Contacto2,eMail,IRSAnual,Vencimento,Notas,NIF,Identificacao,Titular,ValidadeCC,DataNascimento,Ativo,SaldoCorrente) VALUES (9,'Inquilino do 2 Dto JJM','Morada anterior','Brasil',2,'931555777','','',25000,1500,'','122623878','23132132',1,'2025-11-06 22:42:34','1960-11-06 22:42:34','true',0);
INSERT INTO Inquilinos (Id,Nome,Morada,Naturalidade,EstadoCivil,Contacto1,Contacto2,eMail,IRSAnual,Vencimento,Notas,NIF,Identificacao,Titular,ValidadeCC,DataNascimento,Ativo,SaldoCorrente) VALUES (10,'Fiador do Inquilino do 2 Dto JJM','Morada anterior','Lisboa',1,'931222666','','',18500,1000,'','122623878','4679843',0,'2022-11-06 22:44:36','1989-11-06 22:44:36','true',0);
INSERT INTO Inquilinos (Id,Nome,Morada,Naturalidade,EstadoCivil,Contacto1,Contacto2,eMail,IRSAnual,Vencimento,Notas,NIF,Identificacao,Titular,ValidadeCC,DataNascimento,Ativo,SaldoCorrente) VALUES (11,'Inquilino do 1ª piso - CCG','Morada anterior deste Inquilino','Naturalidade',3,'937262910','','',22500,1100,'','122623878','11223344',1,'2025-01-10 11:59:37','1955-01-10 11:59:37',1,0);
INSERT INTO Inquilinos (Id,Nome,Morada,Naturalidade,EstadoCivil,Contacto1,Contacto2,eMail,IRSAnual,Vencimento,Notas,NIF,Identificacao,Titular,ValidadeCC,DataNascimento,Ativo,SaldoCorrente) VALUES (12,'Fiador do piso 1 da CGG','Morada do Fiador do piso 1 - CCG','Naturalidade',2,'931258963','','',19500,850,'','122623878','7989456',0,'2020-01-10 12:02:55.1196133','1981-05-10 12:02:55',1,0);
INSERT INTO Inquilinos (Id,Nome,Morada,Naturalidade,EstadoCivil,Contacto1,Contacto2,eMail,IRSAnual,Vencimento,Notas,NIF,Identificacao,Titular,ValidadeCC,DataNascimento,Ativo,SaldoCorrente) VALUES (13,'Nídia','Morada anterior do Inquilino do 2. piso da CCG','Naturalidade',2,'931023123','','',22500,1100,'','122623878','654654',1,'2025-01-10 19:07:47','1981-05-01 19:07:47',1,0);
INSERT INTO Inquilinos (Id,Nome,Morada,Naturalidade,EstadoCivil,Contacto1,Contacto2,eMail,IRSAnual,Vencimento,Notas,NIF,Identificacao,Titular,ValidadeCC,DataNascimento,Ativo,SaldoCorrente) VALUES (14,'Fiador do piso 2 da CCG','Morada anterior do Fiador do 2º piso da CCG','Naturalidade',1,'932874569','','',17500,890,'','122623878','87841321',0,'2025-01-10 19:10:06','1990-08-08 19:10:06',1,0);
INSERT INTO Inquilinos (Id,Nome,Morada,Naturalidade,EstadoCivil,Contacto1,Contacto2,eMail,IRSAnual,Vencimento,Notas,NIF,Identificacao,Titular,ValidadeCC,DataNascimento,Ativo,SaldoCorrente) VALUES (15,'Inquilino do piso 3 da CCG','Morada','Naturalidade',2,'935125785','','',22000,1200,'','122623878','654654',1,'2025-01-10 19:40:10','1990-01-10 19:40:10',1,0);
INSERT INTO Recebimentos VALUES (30,'2021-01-08 10:16:01.9948851',400,4,5,13,'Pagamento de renda (3/2021',0,0);
INSERT INTO Recebimentos VALUES (31,'2022-11-23 20:37:34.6970759',400,4,5,13,'Pagamento de renda (4/2021',0,0);
INSERT INTO Arrendamento (Id,Data_Inicio,Data_Fim,Data_Pagamento,Fiador,Prazo_Meses,Valor_Renda,Doc_IRS,Doc_Vencimento,Notas,ID_Fracao,ID_Inquilino,ID_Fiador,Caucao,ContratoEmitido,DocumentoGerado,Data_Saida,FormaPagamento,Ativo,ArrendamentoNovo,EstadoPagamento,RenovacaoAutomatica) VALUES (15,'2021-01-01 00:00:00','2022-01-01 00:00:00','2021-04-01 00:00:00',1,240,400,1,1,'',4,13,14,1,1,'C:\GitProjects\HouseRentalSoft\HouseRentalSoft\Reports\Docs\Contratos\Ctr__15_080120211013.docx','0001-01-01 00:00:00',1,1,0,'Pendente',1);
INSERT INTO User_Info (Id,RoleId,User_Name,Pwd,First_Name,EMail,Mobile,Last_Login_Date,Password_Change_Date,IsActive) VALUES (1,1,'lDPQ+XQ5EHk=','XVG3oDwA2Cw=','Administrador','5WBs7UUEleoQG/NkDJgXEUqZAzyAeupW','eQIa71kxdoovDqeyME/Acw==','2022-11-25 12:17:10.9191485','2019-09-15 07:06:09.2655881Z',1);
INSERT INTO User_Info (Id,RoleId,User_Name,Pwd,First_Name,EMail,Mobile,Last_Login_Date,Password_Change_Date,IsActive) VALUES (8,2,'F2jDPZEVpTZZV2Y4ZiXtkA==','sgDwpd/ibvJM+vELHM6e1w==','Fausto Luis','5WBs7UUEleoQG/NkDJgXEUqZAzyAeupW','eQIa71kxdoovDqeyME/Acw==','0001-01-01 00:00:00','0001-01-01 00:00:00',1);
COMMIT;
