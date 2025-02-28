using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class CrmPriceListDet
    {
        public long? RecordId { get; set; }
        public int? Sno { get; set; }
        public int? ProductId { get; set; }
        public double? Price { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public int? TaxId { get; set; }

        public virtual CrmPriceListUni Record { get; set; }
    }
}
