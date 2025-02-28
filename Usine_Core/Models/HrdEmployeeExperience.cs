using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdEmployeeExperience
    {
        public long? RecordId { get; set; }
        public long LineId { get; set; }
        public int? Frmyear { get; set; }
        public int? Toyear { get; set; }
        public string Designation { get; set; }
        public string Organisation { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual HrdEmployees Record { get; set; }
    }
}
