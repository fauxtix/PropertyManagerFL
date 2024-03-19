using PropertyManagerFL.Core.Entities;
using System.Collections.Generic;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager;
public interface ILetterTemplatesService
{
    Task<int> AddTemplateAsync(Template template);
    Task<IEnumerable<Template>> GetAllTemplatesAsync();
    Task<Template> GetTemplateByIdAsync(int id);
    Task<string> GetTemplateFromServer(string templateName);
    Task<IList<string>> GetTemplatesFilenamesFromServer(string culture);
    Task<bool> UpdateTemplateAsync(Template template);
}