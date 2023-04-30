using PropertyManagerFL.Application.Interfaces.SqLiteGenerics;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.TipoDespesa;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface ITipoDespesaRepository
    {
        Task<int> InsereTipoDespesa(NovoTipoDespesa novoTipoDespesa);
        Task<bool> AtualizaTipoDespesa( AlteraTipoDespesa alteraTipoDespesa);
        Task ApagaTipoDespesa(int id);
        Task<TipoDespesaVM> GetTipoDespesa_ById(int id);
        Task<IEnumerable<TipoDespesaVM>> GetAll();
        Task<bool> CanRecordBeDeleted(int id);
    }
}
