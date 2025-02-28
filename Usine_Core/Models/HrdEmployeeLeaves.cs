using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdEmployeeLeaves
    {
        public long? RecordId { get; set; }
        public long LineId { get; set; }
        public int? LeaveId { get; set; }
        public int? Valu { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual HrdLeaves Leave { get; set; }
        public virtual HrdEmployees Record { get; set; }
    }
}
