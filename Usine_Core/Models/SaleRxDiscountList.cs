using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class SaleRxDiscountList
    {
        public int Sno { get; set; }
        public string Product { get; set; }
        public string Coat { get; set; }
        public double? Discount { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public string PriceListName { get; set; }
    }
}
