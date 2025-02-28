using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdAdvances
    {
        public int RecordId { get; set; }
        public long? Empno { get; set; }
        public DateTime? Dat { get; set; }
        public double? AdvanceCredit { get; set; }
        public double? AdvanceDebit { get; set; }
        public double? AdvanceCutOff { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
