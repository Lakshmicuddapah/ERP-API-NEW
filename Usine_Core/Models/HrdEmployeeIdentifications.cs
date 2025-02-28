using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdEmployeeIdentifications
    {
        public long? RecordId { get; set; }
        public long LineId { get; set; }
        public string Identit { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual HrdEmployees Record { get; set; }
    }
}
