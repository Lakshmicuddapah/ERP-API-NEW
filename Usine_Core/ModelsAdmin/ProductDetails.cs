using System;
using System.Collections.Generic;

namespace Usine_Core.ModelsAdmin
{
    public partial class ProductDetails
    {
        public ProductDetails()
        {
            CustomerRegistrations = new HashSet<CustomerRegistrations>();
            ProductModules = new HashSet<ProductModules>();
        }

        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int? MaxInstallationTime { get; set; }
        public int? MaxTrainingTime { get; set; }
        public string ProductType { get; set; }
        public string ProductDescription { get; set; }
        public int? PriceType { get; set; }
        public double? Price { get; set; }

        public virtual ICollection<CustomerRegistrations> CustomerRegistrations { get; set; }
        public virtual ICollection<ProductModules> ProductModules { get; set; }
    }
}
