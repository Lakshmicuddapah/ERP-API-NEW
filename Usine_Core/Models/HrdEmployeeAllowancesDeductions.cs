using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdEmployeeAllowancesDeductions
    {
        public long? RecordId { get; set; }
        public long LineId { get; set; }
        public int? Allowance { get; set; }
        public double? Valu { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual HrdAllowancesDeductions AllowanceNavigation { get; set; }
        public virtual HrdEmployees Record { get; set; }
    }
}
