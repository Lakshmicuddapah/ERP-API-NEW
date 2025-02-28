using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PurPurchaseOrderDet
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public int? ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public double? Qty { get; set; }
        public int? Um { get; set; }
        public double? Rat { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public int? PurRequest { get; set; }
        public DateTime? ReqdBy { get; set; }

        public virtual PurPurchaseRequestUni PurRequestNavigation { get; set; }
    }
}
