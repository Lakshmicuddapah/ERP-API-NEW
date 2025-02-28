using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdAllowanceDeductionRanges
    {
        public int? AllowanceId { get; set; }
        public long Lineid { get; set; }
        public double? FromValue { get; set; }
        public double? ToValue { get; set; }
        public double? Valu { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual HrdAllowancesDeductions Allowance { get; set; }
    }
}
