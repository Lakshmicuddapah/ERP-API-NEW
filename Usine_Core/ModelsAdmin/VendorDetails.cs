using System;
using System.Collections.Generic;

namespace Usine_Core.ModelsAdmin
{
    public partial class VendorDetails
    {
        public VendorDetails()
        {
            InverseMainVendorNavigation = new HashSet<VendorDetails>();
        }

        public int VendorCode { get; set; }
        public string VendorName { get; set; }
        public string ContactPerson { get; set; }
        public string Addr { get; set; }
        public string Country { get; set; }
        public string Stat { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Web { get; set; }
        public int? MainVendor { get; set; }
        public string Gst { get; set; }

        public virtual VendorDetails MainVendorNavigation { get; set; }
        public virtual ICollection<VendorDetails> InverseMainVendorNavigation { get; set; }
    }
}
