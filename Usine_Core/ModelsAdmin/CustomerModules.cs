using System;
using System.Collections.Generic;

namespace Usine_Core.ModelsAdmin
{
    public partial class CustomerModules
    {
        public int CustomerId { get; set; }
        public int Sno { get; set; }
        public string ProductCode { get; set; }
        public string Module { get; set; }
        public DateTime? ExpDate { get; set; }
        public int? VendorId { get; set; }
    }
}
