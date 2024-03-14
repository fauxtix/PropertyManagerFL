using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.Interfaces.Services.Contract;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Application.ViewModels.Fiadores;
using PropertyManagerFL.Application.ViewModels.Fracoes;
using PropertyManagerFL.Application.ViewModels.Imoveis;
using PropertyManagerFL.Application.ViewModels.Inquilinos;
using PropertyManagerFL.Application.ViewModels.MailMerge;
using PropertyManagerFL.Application.ViewModels.Proprietarios;
using PropertyManagerFLApplication.Utilities;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public class WrapperContratos : IContratoService
    {
        private readonly ILogger<WrapperContratos> _logger;
        readonly IProprietarioService _svcProprietarios;
        readonly IInquilinoService _svcInquilinos;
        readonly IFiadorService _svcFiadores;
        readonly IImovelService _svcImoveis;
        readonly IFracaoService _svcFracoes;
        readonly IMailMergeService _MailMergeSvc;
        readonly ILookupTableService _svcLookupTable;

        ArrendamentoVM _arrendamento;

        public WrapperContratos(ILogger<WrapperContratos> logger,
                                IProprietarioService repoProprietarios,
                                IInquilinoService repoInquilinos,
                                IImovelService repoImoveis,
                                IFracaoService repoFracoes,
                                IMailMergeService mailMergeSvc,
                                ILookupTableService repoLookupTable,
                                IArrendamentoService svcArrendamentos,
                                IFiadorService svcFiadores)
        {
            _logger = logger;
            _svcProprietarios = repoProprietarios;
            _svcInquilinos = repoInquilinos;
            _svcImoveis = repoImoveis;
            _svcFracoes = repoFracoes;
            _MailMergeSvc = mailMergeSvc;
            _svcLookupTable = repoLookupTable;
            _svcFiadores = svcFiadores;
        }

        public async Task AtualizaSituacaoFracao(int IdFracao)
        {
            await _svcFracoes.MarcaFracaoComoAlugada(IdFracao);
        }

        public async Task<string> EmiteContrato(Contrato? contrato)
        {
            string[] aCampos = new string[] {
                "NomeOutorgante_1", "NomeOutorgante_2", "NomeOutorgante_3",
                "DtNasc_O_1", "DtNasc_O_2", "DtNasc_O_3",
                "MoradaOutorgante_1", "MoradaOutorgante_2", "MoradaOutorgante_3",
                "NaturalidadeOutorgante_1", "NaturalidadeOutorgante_2", "NaturalidadeOutorgante_3",
                "EC_Outorgante_1", "EC_Outorgante_2", "EC_Outorgante_3",
                "CC_Outorgante_1", "CC_Outorgante_2", "CC_Outorgante_3",
                "ValidadeCC_Outorgante_1", "ValidadeCC_Outorgante_2", "ValidadeCC_Outorgante_3",
                "NIF_Outorgante_1", "NIF_Outorgante_2", "NIF_Outorgante_3",
                "Letra", "Andar", "Lado", 
                "MoradaImovel", "Freguesia", "Concelho",
                "Artigo", "LicencaHabitacao", "DataLicencaHabitacao", "Quartos", "MesesContrato", 
                "DataInicioContrato", "DiaInicioContrato", "MesInicioContrato", "AnoInicioContrato",
                "ValorRenda", "ValorRendaExtenso"
            };


            string[] aDados = new string[] {
                contrato.Proprietario.Nome,
                contrato.Inquilino.Nome,
                contrato.Fiador.Nome,

                contrato.Proprietario.DataNascimento.ToShortDateString(),
                contrato.Inquilino.DataNascimento.ToShortDateString(),
                contrato.Fiador.DataNascimento.ToShortDateString(),

                contrato.Proprietario.Morada,
                contrato.Inquilino.Morada,
                contrato.Fiador.Morada,

                contrato.Proprietario.Naturalidade,
                contrato.Inquilino.Naturalidade,
                contrato.Fiador.Naturalidade,

                contrato.Proprietario.EstadoCivil,
                contrato.Inquilino.EstadoCivil,
                contrato.Fiador.EstadoCivil,

                contrato.Proprietario.Identificação,
                contrato.Inquilino.Identificação,
                contrato.Fiador.Identificação,

                contrato.Proprietario.Validade_CC.ToShortDateString(),
                contrato.Inquilino.Validade_CC.ToShortDateString(),
                contrato.Fiador.Validade_CC.ToShortDateString(),

                contrato.Proprietario.NIF,
                contrato.Inquilino.NIF,
                contrato.Fiador.NIF,

                contrato.LetraFracao,
                contrato.Andar,
                contrato.Lado,

                contrato.MoradaImovel + ", " + contrato.Numero,
                contrato.FreguesiaFracao,
                contrato.ConcelhoFracao,

                contrato.Artigo,
                contrato.LicencaHabitacao,
                contrato.DataEmissaoLicencaHabitacao.ToShortDateString(),
                contrato.Quartos,
                "36",

                contrato.Inicio.ToShortDateString(),
                contrato.Inicio.ToString("dd"),
                contrato.Inicio.ToString("MMM"),
                contrato.Inicio.ToString("yyyy"),

                contrato.Valor_Renda.ToString(),
                contrato.Valor_Renda_Extenso
            };

            try
            {
                if (!contrato.ContratoEmitido) 
                {
                    var mergeModel = new MailMergeModel()
                    {
                        CodContrato = _arrendamento.Id,
                        TipoDocumentoEmitido = DocumentoEmitido.ContratoArrendamento,
                        DocumentHeader = "",
                        MergeFields = aCampos,
                        ValuesFields = aDados,
                        WordDocument = "ContratoArrendamento.dotx",
                        SaveFile = true,
                        Referral = true
                    };

                    var result = await _MailMergeSvc.MailMergeLetter(mergeModel); // returns the file full path generated (docx)

                    return result; ;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return "Error";
            }
        }

        public async Task<Contrato> GetDadosContrato(ArrendamentoVM DadosArrendamento)
        {
            _arrendamento = DadosArrendamento;

            int IdProprietario = await _svcProprietarios.GetFirstId(); // nesta versão da aplicação, só existe um proprietário...
            ProprietarioVM DadosProprietario = await _svcProprietarios.GetProprietario_ById(IdProprietario);

            InquilinoVM DadosInquilino = await _svcInquilinos.GetInquilino_ById(_arrendamento.ID_Inquilino);


            FracaoVM DadosFracao = await _svcFracoes.GetFracao_ById(_arrendamento.ID_Fracao);
            ImovelVM DadosImovel = await _svcImoveis.GetImovel_ById(DadosFracao.Id_Imovel);

            FiadorVM DadosFiador = await _svcFiadores.GetFiador_ById(_arrendamento.ID_Fiador);

            string tipologia = await NormalizeLookupDescription(DadosFracao.Tipologia, "TipologiaFracao"); 
            string estadoCivilProprietario = await NormalizeLookupDescription(DadosProprietario.ID_EstadoCivil, "EstadoCivil");
            string estadoCivilInquilino = await NormalizeLookupDescription(DadosInquilino.ID_EstadoCivil, "EstadoCivil"); 
            string estadoCivilFiador = await NormalizeLookupDescription(DadosFiador.ID_EstadoCivil, "EstadoCivil"); 

            string quartos = tipologia.Substring(1); // T1, T2, ...

            Contrato contrato = new Contrato()
            {
                Proprietario = new DadosOutorgante()
                {
                    Nome = DadosProprietario.Nome,
                    DataNascimento = DadosProprietario.DataNascimento,
                    Morada = DadosProprietario.Morada,
                    Naturalidade = DadosProprietario.Naturalidade,
                    EstadoCivil = estadoCivilProprietario,
                    Identificação = DadosProprietario.Identificacao,
                    Validade_CC = DadosProprietario.ValidadeCC,
                    NIF = DadosProprietario.NIF
                },
                Inquilino = new DadosOutorgante()
                {
                     Id = DadosInquilino.Id,
                    Nome = DadosInquilino.Nome,
                    DataNascimento = DadosInquilino.DataNascimento,
                    Morada = DadosInquilino.Morada,
                    Naturalidade = DadosInquilino.Naturalidade,
                    EstadoCivil = estadoCivilInquilino,
                    Identificação = DadosInquilino.Identificacao,
                    Validade_CC = DadosInquilino.ValidadeCC,
                    NIF = DadosInquilino.NIF
                },
                Fiador = new DadosOutorgante()
                {
                    Nome = DadosFiador.Nome,
                    Morada = DadosFiador.Morada,
                    EstadoCivil = estadoCivilFiador,
                    Identificação = DadosFiador.Identificacao,
                    Validade_CC = DadosFiador.ValidadeCC,
                    NIF = DadosFiador.NIF
                },
                IdFracao = DadosFracao.Id,
                LetraFracao = DadosFracao.Lado,  // TODO - não existe esta informação, será necessária?
                Andar = DadosFracao.Andar,
                Lado = DadosFracao.Lado,
                MoradaImovel = DadosImovel.Morada,
                Numero = DadosImovel.Numero,
                FreguesiaFracao = DadosImovel.FreguesiaImovel,
                ConcelhoFracao = DadosImovel.ConcelhoImovel,
                CamaraEmissoraLicencaHabitacao = DadosImovel.ConcelhoImovel,
                EmissorCertificadoEnergetico = DadosImovel.ConcelhoImovel,
                CertificadoEnergetico = DadosFracao.CertificadoEnergetico, 
                ValidadeEmissaoCertificadoEnergetico = DateTime.UtcNow.ToShortDateString(),  // TODO - não existe esta informação, adaptar bd e front-end
                Artigo = DadosFracao.Matriz, // TODO alterar bd, criar entrada para recolha desta informação                
                MatrizPredial = DadosFracao.Matriz,
                Quartos = quartos,
                LicencaHabitacao = DadosFracao.LicencaHabitacao,
                DataEmissaoLicencaHabitacao = DadosFracao.DataEmissaoLicencaHabitacao,
                Prazo = DadosArrendamento.Prazo_Meses,
                Inicio = DadosArrendamento.Data_Inicio,
                Termo = DadosArrendamento.Data_Fim,
                ContratoEmitido = DadosArrendamento.ContratoEmitido,
                Valor_Renda = DadosArrendamento.Valor_Renda,
                Valor_Renda_Extenso = Utilitarios.ValorPorExtenso(DadosArrendamento.Valor_Renda),
                Valor_Caucao = DadosArrendamento.Valor_Renda, // Igual TODO rever?
                NIB = "1234567890123456789 10"  // TODO incluir na base de dados?
            };

            return contrato;
        }

        private async Task<string> NormalizeLookupDescription(int fkId, string tableToSearch)
        {
            var normalizedString = await _svcLookupTable.GetDescription(fkId, tableToSearch);
            normalizedString = normalizedString.Replace("\"", "").Replace("\\\\", "\\");
            return normalizedString;
        }
    }
}
