using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PurPurchaseEnquiryDetHistory
    {
        public long AuditId { get; set; }
        public DateTime? AuditDat { get; set; }
        public int? AuditType { get; set; }
        public int Sno { get; set; }
        public long? RecordId { get; set; }
        public int? ItemId { get; set; }
        public string ItemDescription { get; set; }
        public double? Qty { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public int? uom { get; set; }
    }
}
