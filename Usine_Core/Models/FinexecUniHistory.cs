using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class FinexecUniHistory
    {
        public long? AuditId { get; set; }
        public int? RecordId { get; set; }
        public int? Seq { get; set; }
        public DateTime? Dat { get; set; }
        public string Narr { get; set; }
        public string Tratype { get; set; }
        public string Traref { get; set; }
        public string Vouchertype { get; set; }
        public string BankDet { get; set; }
        public string Branchid { get; set; }
        public int? CustomerCode { get; set; }
        public string Usr { get; set; }
        public int? PrintCount { get; set; }
        public DateTime? AuditDate { get; set; }
        public int? AuditType { get; set; }

        public virtual FinexecUni Record { get; set; }
    }
}
