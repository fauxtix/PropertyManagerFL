using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Repositories;
public interface ILetterTemplatesRepository
{
    Task<IEnumerable<Template>> GetAllTemplatesAsync();
    Task<Template> GetTemplateByIdAsync(int id);
    Task<int> InsertAsync(Template template);
    Task<bool> UpdateAsync(Template template);
}
