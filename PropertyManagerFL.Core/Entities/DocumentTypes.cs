using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagerFL.Core.Entities
{
    public class DocumentTypes
    {
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastModifiedOn { get; }
    }
}
