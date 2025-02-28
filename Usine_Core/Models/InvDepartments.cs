using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class InvDepartments
    {
        public int RecordId { get; set; }
        public string Department { get; set; }
        public string Area { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
