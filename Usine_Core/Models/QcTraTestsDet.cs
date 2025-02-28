using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class QcTraTestsDet
    {
        public long RecordId { get; set; }
        public int Sno { get; set; }
        public int? Lotno { get; set; }
        public string Lottype { get; set; }
        public string Comments { get; set; }
        public double? CheckedQty { get; set; }
        public double? RectifiedQty { get; set; }
        public double? RejectedQty { get; set; }
        public double? ValueOfItem { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public int? TransactionId { get; set; }
        public double? RectificationCost { get; set; }

        public virtual QcTraTestsUni Record { get; set; }
    }
}
