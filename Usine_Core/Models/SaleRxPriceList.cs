using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class SaleRxPriceList
    {
        public int? Sno { get; set; }
        public string Product { get; set; }
        public string Coat { get; set; }
        public string Taxtype { get; set; }
        public double? Price { get; set; }
        public string PriceListName { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
