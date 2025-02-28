using System;
using System.Collections.Generic;

namespace Usine_Core.ModelsAdmin
{
    public partial class CrmEnquiries
    {
        public CrmEnquiries()
        {
            CrmFollowup = new HashSet<CrmFollowup>();
            CrmQuotationUni = new HashSet<CrmQuotationUni>();
        }

        public int RecordId { get; set; }
        public string Seq { get; set; }
        public DateTime? Dat { get; set; }
        public string Customer { get; set; }
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
        public string CustomerComments { get; set; }
        public string CallerComments { get; set; }
        public DateTime? NextCallDate { get; set; }
        public int? Statu { get; set; }
        public string Username { get; set; }
        public int? VenodrId { get; set; }
        public string ProductCode { get; set; }
        public string Reference { get; set; }
        public DateTime? Validity { get; set; }

        public virtual ICollection<CrmFollowup> CrmFollowup { get; set; }
        public virtual ICollection<CrmQuotationUni> CrmQuotationUni { get; set; }
    }
}
