using PropertyManagerFL.Application.ViewModels.Audit;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface IAuditRepository
    {
        void InsertAuditLogs(AuditModel objauditmodel);
    }
}