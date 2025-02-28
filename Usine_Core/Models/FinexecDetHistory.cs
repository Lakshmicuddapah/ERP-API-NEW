using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class FinexecDetHistory
    {
        public long? AuditId { get; set; }
        public int? Sno { get; set; }
        public int? RecordId { get; set; }
        public int? Accname { get; set; }
        public double? Cre { get; set; }
        public double? Deb { get; set; }
        public string Branchid { get; set; }
        public int? CustomerCode { get; set; }
        public DateTime? Dat { get; set; }
        public DateTime? AuditDate { get; set; }
        public int? AuditType { get; set; }
    }
}
