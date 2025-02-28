using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PurPurchaseRequestUni
    {
        public PurPurchaseRequestUni()
        {
            PurPurchaseOrderDet = new HashSet<PurPurchaseOrderDet>();
            PurPurchaseRequestDet = new HashSet<PurPurchaseRequestDet>();
        }

        public int RecordId { get; set; }
        public string Seq { get; set; }
        public DateTime? Dat { get; set; }
        public string Descriptio { get; set; }
        public string Usr { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public int? Department { get; set; }
        public long? Empno { get; set; }
        public int? PrintCount { get; set; }
        public int? Statu { get; set; }

        public virtual ICollection<PurPurchaseOrderDet> PurPurchaseOrderDet { get; set; }
        public virtual ICollection<PurPurchaseRequestDet> PurPurchaseRequestDet { get; set; }
    }
}
