using Dapper;
using PropertyManagerFL.Application.Interfaces.DapperContext;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Infrastructure.Repositories;
public class LetterTemplatesRepository : ILetterTemplatesRepository
{
    private readonly IDapperContext _context;

    public LetterTemplatesRepository(IDapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Template>> GetAllTemplatesAsync()
    {
        const string sql = "SELECT * FROM Templates";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Template>(sql);
    }

    public async Task<Template> GetTemplateByIdAsync(int id)
    {
        const string sql = "SELECT * FROM Templates WHERE Id = @Id";
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Template>(sql, new { Id = id });
    }

    public async Task<int> InsertAsync(Template template)
    {
        const string sql = "INSERT INTO Templates (FileName, CreatedAt) VALUES (@FileName, @CreatedAt); SELECT SCOPE_IDENTITY();";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstAsync<int>(sql, template);
    }

    public async Task<bool> UpdateAsync(Template template)
    {
        const string sql = "UPDATE Templates SET FileName = @FileName WHERE Id = @Id";
        using var connection = _context.CreateConnection();
        var affectedRows = await connection.ExecuteAsync(sql, template);
        return affectedRows > 0;
    }

}
