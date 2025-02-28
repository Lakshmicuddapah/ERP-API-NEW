using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class CrmDiscountListDet
    {
        public long RecordId { get; set; }
        public int Sno { get; set; }
        public int? ProductId { get; set; }
        public double? Discount { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual CrmDiscountListUni Record { get; set; }
    }
}
