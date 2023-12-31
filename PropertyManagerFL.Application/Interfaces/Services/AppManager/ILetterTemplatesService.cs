using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager;
public interface ILetterTemplatesService
{
    Task<int> AddTemplateAsync(Template template);
    Task<IEnumerable<Template>> GetAllTemplatesAsync();
    Task<Template> GetTemplateByIdAsync(int id);
    Task<string> GetTemplateFromServer(string templateName);
    Task<bool> UpdateTemplateAsync(Template template);
}