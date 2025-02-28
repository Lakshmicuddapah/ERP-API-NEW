using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PurPurchaseRequestUniHistory
    {
        public long AuditId { get; set; }
        public DateTime? AuditDat { get; set; }
        public int RecordId { get; set; }
        public string Seq { get; set; }
        public DateTime? Dat { get; set; }
        public string Descriptio { get; set; }
        public string Usr { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public int? Department { get; set; }
        public int? AuditType { get; set; }
        public int? Empno { get; set; }
    }
}
