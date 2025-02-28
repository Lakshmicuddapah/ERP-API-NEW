using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class InvTransactionsDet
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public int? ItemId { get; set; }
        public double? Qtyin { get; set; }
        public double? Qtyout { get; set; }
        public int? Um { get; set; }
        public double? Conversion { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual InvMaterials Item { get; set; }
        public virtual InvTransactionsUni Record { get; set; }
    }
}
