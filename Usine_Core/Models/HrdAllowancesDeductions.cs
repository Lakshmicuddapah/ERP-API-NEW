using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Usine_Core.Models
{
    public partial class HrdAllowancesDeductions
    {
        public HrdAllowancesDeductions()
        {
            HrdAllowanceDeductionRanges = new HashSet<HrdAllowanceDeductionRanges>();
            HrdEmployeeAllowancesDeductions = new HashSet<HrdEmployeeAllowancesDeductions>();
        }

        public int RecordId { get; set; }
        public string Allowance { get; set; }
        public int? AllowanceCheck { get; set; }
        public int? CalcType { get; set; }
        public int? EffectAs { get; set; }
        public int? Statu { get; set; }
        public string Branchid { get; set; }
        public int? CustomerCode { get; set; }

        public virtual ICollection<HrdAllowanceDeductionRanges> HrdAllowanceDeductionRanges { get; set; }
        public virtual ICollection<HrdEmployeeAllowancesDeductions> HrdEmployeeAllowancesDeductions { get; set; }
    }
}
