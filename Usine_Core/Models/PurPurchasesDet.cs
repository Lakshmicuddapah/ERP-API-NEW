using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PurPurchasesDet
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public int? Store { get; set; }
        public int? ItemId { get; set; }
        public string ItemName { get; set; }
        public string Batchno { get; set; }
        public DateTime? Manudate { get; set; }
        public DateTime? Expdate { get; set; }
        public double? Qty { get; set; }
        public int? Um { get; set; }
        public double? Rat { get; set; }
        public double? Mrp { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public string Gin { get; set; }
    }
}
