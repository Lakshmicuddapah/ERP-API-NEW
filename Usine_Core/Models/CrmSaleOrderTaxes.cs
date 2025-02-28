using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class CrmSaleOrderTaxes
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public string Taxcode { get; set; }
        public double? Taxper { get; set; }
        public double? Taxvalue { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public int? LineId { get; set; }
        public virtual CrmSaleOrderUni Record { get; set; }
    }
}
