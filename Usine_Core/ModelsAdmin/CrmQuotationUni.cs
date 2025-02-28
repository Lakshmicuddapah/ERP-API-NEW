using System;
using System.Collections.Generic;

namespace Usine_Core.ModelsAdmin
{
    public partial class CrmQuotationUni
    {
        public CrmQuotationUni()
        {
            CrmOrderUni = new HashSet<CrmOrderUni>();
            CrmQuotationTaxes = new HashSet<CrmQuotationTaxes>();
            CrmQuotationTerms = new HashSet<CrmQuotationTerms>();
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
        public int? EnquiryId { get; set; }
        public int? Statu { get; set; }
        public string Username { get; set; }
        public int? VendorId { get; set; }
        public string ProductCode { get; set; }
        public double? BaseAmt { get; set; }
        public double? Discount { get; set; }
        public double? Taxes { get; set; }
        public double? Totalamt { get; set; }

        public virtual CrmEnquiries Enquiry { get; set; }
        public virtual ICollection<CrmOrderUni> CrmOrderUni { get; set; }
        public virtual ICollection<CrmQuotationTaxes> CrmQuotationTaxes { get; set; }
        public virtual ICollection<CrmQuotationTerms> CrmQuotationTerms { get; set; }
    }
}
