
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.Application
{
    public class LogCRUDModel
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string UserId { get; set; }
        public OpcaoCRUD Action { get; set; }
    }
}
