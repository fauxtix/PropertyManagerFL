using PropertyManagerFL.Infrastructure.SqLiteGenerics;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Factories;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class TipoPropriedadeRepository : BaseRepository<TipoPropriedade>, ITipoPropriedadeRepository
    {
    }
}
