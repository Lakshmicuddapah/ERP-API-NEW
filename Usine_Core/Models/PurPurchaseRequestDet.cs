using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PurPurchaseRequestDet
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public DateTime? Dat { get; set; }
        public int? ItemId { get; set; }
        public string ItemDescription { get; set; }
        public string Purpose { get; set; }
        public int? SuggestedSupplier { get; set; }
        public double? Qty { get; set; }
        public double? ApprovedQty { get; set; }
        public int? Um { get; set; }
        public DateTime? ReqdBy { get; set; }
        public string Usr { get; set; }
        public string ApprovedUsr { get; set; }
        public int? Pos { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public int? Department { get; set; }

        public virtual PurPurchaseRequestUni Record { get; set; }
    }
}
