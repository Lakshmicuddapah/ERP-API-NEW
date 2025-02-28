using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class CrmSaleOrderDet
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public int? ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public double? Qty { get; set; }
        public int? Um { get; set; }
        public double? Rat { get; set; }
        public double? DiscPer { get; set; }
        public DateTime? ReqdBy { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual CrmSaleOrderUni Record { get; set; }
    }
}
