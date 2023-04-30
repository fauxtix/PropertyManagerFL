using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;

namespace PropertyManagerFL.Application.Interfaces.Services.Contract
{
    public interface IContratoService
    {
        Task AtualizaSituacaoFracao(int IdFracao);
        Task<string> EmiteContrato(Contrato contrato);
        Task<Contrato> GetDadosContrato(ArrendamentoVM DadosArrendamento);
    }
}