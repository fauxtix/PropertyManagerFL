using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Inquilinos;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface ICC_InquilinoRepository
    {
        Task<int> Insert(CC_InquilinoNovo entity);
        Task<bool> Update(CC_InquilinoAltera entity);
        Task Delete(int id);
        Task<bool> EntradaExiste_BD(string campo, string str);
        Task<int> GetFirstId();
        Task<int> GetLastId();
        Task<IEnumerable<CC_InquilinoVM>> GetAll();
        Task<CC_InquilinoVM> Query_ById(int Id);
        Task<bool> RecInUse(int Id);
        Task<bool> TableHasData();
    }
}