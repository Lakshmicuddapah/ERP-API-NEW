using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PurPurchaseEnquiryNotesHistory
    {
        public long AuditId { get; set; }
        public DateTime? AuditDat { get; set; }
        public int? AuditType { get; set; }
        public int Sno { get; set; }
        public long? RecordId { get; set; }
        public string Note { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
