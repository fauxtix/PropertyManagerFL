using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.Interfaces.Services.Contract;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Core.Shared.ViewModels.Fracoes;
using PropertyManagerFL.Core.Shared.ViewModels.Imoveis;
using PropertyManagerFL.Core.Shared.ViewModels.Inquilinos;

namespace PropertyManagerFL.Infrastructure.Services.ContractServices
{
    public class ContratoService : IContratoService
    {
        private readonly IProprietarioRepository _repoProprietarios;
        private readonly IInquilinoRepository _repoInquilinos;
        private readonly IImovelRepository _repoImoveis;
        private readonly IFracaoRepository _repoFracoes;
        private readonly IMailMergeService _MailMergeSvc;
        private readonly ILookupTableRepository _repoLookupTables;

        protected Arrendamento? _arrendamento { get; set; }
        public ContratoService(IProprietarioRepository repoProprietarios,
                               IInquilinoRepository repoInquilinos,
                               IImovelRepository repoImoveis,
                               IFracaoRepository repoFracoes,
                               IMailMergeService mailMergeSvc)
        {
            _MailMergeSvc = mailMergeSvc;
            _repoProprietarios = repoProprietarios;
            _repoInquilinos = repoInquilinos;
            _repoImoveis = repoImoveis;
            _repoFracoes = repoFracoes;
        }

        public async Task<Contrato> GetDadosContrato(Arrendamento DadosArrendamento)
        {
            _arrendamento = DadosArrendamento;

            int IdProprietario = _repoProprietarios.GetFirstId();
            Proprietario DadosProprietario = await _repoProprietarios.GetProprietario_ById(IdProprietario);

            InquilinoVM DadosInquilino = await _repoInquilinos.GetInquilino_ById(_arrendamento.ID_Inquilino);
            InquilinoVM DadosFiador = await _repoInquilinos.GetInquilino_ById(_arrendamento.ID_Fiador);

            FracaoVM DadosFracao = await _repoFracoes.GetFracao_ById(_arrendamento.ID_Fracao);

            ImovelVM DadosImovel = await _repoImoveis.GetImovel_ById(DadosFracao.Id_Imovel);

            var sTipologia = _repoLookupTables.GetDescription(DadosFracao.Tipologia, "TipologiaFracao");
            string sQuartos = sTipologia.Substring(1); // T1, T2, ...

            Contrato contrato = new Contrato()
            {
                Proprietario = new DadosOutorgante()
                {
                    Nome = DadosProprietario.Nome,
                    DataNascimento = DadosProprietario.DataNascimento,
                    Morada = DadosProprietario.Morada,
                    Naturalidade = DadosProprietario.Naturalidade,
                    EstadoCivil = _repoLookupTables.GetDescription(DadosProprietario.EstadoCivil, "EstadoCivil"),
                    Identificação = DadosProprietario.Identificacao,
                    Validade_CC = DadosProprietario.ValidadeCC,
                    NIF = DadosProprietario.NIF
                },
                Inquilino = new DadosOutorgante()
                {
                    Nome = DadosInquilino.Nome,
                    DataNascimento = DadosInquilino.DataNascimento,
                    Morada = DadosInquilino.Morada,
                    Naturalidade = DadosInquilino.Naturalidade,
                    EstadoCivil = _repoLookupTables.GetDescription(DadosInquilino.ID_EstadoCivil, "EstadoCivil"),
                    Identificação = DadosInquilino.Identificacao,
                    Validade_CC = DadosInquilino.ValidadeCC,
                    NIF = DadosInquilino.NIF
                },
                Fiador = new DadosOutorgante()
                {
                    Nome = DadosFiador.Nome,
                    DataNascimento = DadosFiador.DataNascimento,
                    Morada = DadosFiador.Morada,
                    Naturalidade = DadosFiador.Naturalidade,
                    EstadoCivil = _repoLookupTables.GetDescription(DadosFiador.ID_EstadoCivil, "EstadoCivil"),
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
                Freguesia = DadosImovel.Freguesia,
                Concelho = DadosImovel.Concelho,
                LicencaHabitacao = DadosImovel.LicencaHabitacao,
                DataEmissaoLicencaHabitacao = DadosImovel.DataEmissaoLicencaHabitacao,
                Quartos = sQuartos,
                Prazo = DadosArrendamento.Prazo_Meses,
                Inicio = DadosArrendamento.Data_Inicio,
                Termo = DadosArrendamento.Data_Inicio.AddYears(1),
                ContratoEmitido = DadosArrendamento.ContratoEmitido,
                Valor_Renda = DadosArrendamento.Valor_Renda,
                Valor_Caucao = DadosArrendamento.Valor_Renda, // Igual TODO rever?
                NIB = "1234567890123456789 10"  // TODO incluir na base de dados?
            };

            return contrato;
        }

        /// <summary>
        /// Faz o merge com o documento/Template de word 
        /// </summary>
        /// <param name="contrato"></param>
        /// <returns>nome do documento gerado</returns>
        public async Task<string> EmiteContrato(Contrato contrato)
        {
            // TODO falta acrescentar restantes campos do contrato - 01/12
            string[] aCampos = new string[] {
                "NomeOutorgante_1", "NomeOutorgante_2", "NomeOutorgante_3",
                "DtNasc_O_1", "DtNasc_O_2", "DtNasc_O_3",
                "MoradaOutorgante_1", "MoradaOutorgante_2", "MoradaOutorgante_3",
                "NaturalidadeOutorgante_1", "NaturalidadeOutorgante_2", "NaturalidadeOutorgante_3",
                "EC_Outorgante_1", "EC_Outorgante_2", "EC_Outorgante_3",
                "CC_Outorgante_1", "CC_Outorgante_2", "CC_Outorgante_3",
                "ValidadeCC_Outorgante_1", "ValidadeCC_Outorgante_2", "ValidadeCC_Outorgante_3",
                "NIF_Outorgante_1", "NIF_Outorgante_2", "NIF_Outorgante_3"
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
                contrato.Fiador.NIF
            };

            try
            {
                if (!contrato.ContratoEmitido) // repo_Fracao.FracaoEstaLivre(contrato.IdFracao))
                {
                    // TODO - variável _arrendamento deverá ser passado como parâmetro
                    string docGerado = _MailMergeSvc.GeraContrato(_arrendamento.Id,
                        "ContratoArrendamento.dotx", aCampos, aDados, "", true, true);

                    //esta opção abaixo deixa de ser necessária, ao criar registo de 'Arrendamento', situação é logo alterada
                    //AtualizaSituacaoFracao(contrato.IdFracao);

                    return docGerado;
                }
                else
                {
                    return "";
                }
            }
            catch
            {

                throw;
            }
        }


        /// <summary>
        /// Marca fração como alugada
        /// </summary>
        /// <param name="IdFracao"></param>
        public void AtualizaSituacaoFracao(int IdFracao)
        {
            _repoFracoes.MarcaFracaoComoAlugada(IdFracao);
        }
    }

}

