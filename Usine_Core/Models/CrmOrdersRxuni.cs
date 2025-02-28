using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class CrmOrdersRxuni
    {
        public int RecordId { get; set; }
        public string Seq { get; set; }
        public DateTime? Dat { get; set; }
        public DateTime? Approveddat { get; set; }
        public string Usr { get; set; }
        public string ApprovedUsr { get; set; }
        public int? CustomerId { get; set; }
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
        public string Webid { get; set; }
        public int? TypeofSale { get; set; }
        public double? BaseAmt { get; set; }
        public double? Discount { get; set; }
        public double? Taxes { get; set; }
        public double? Roundoff { get; set; }
        public double? TotalAmt { get; set; }
        public DateTime? ValidityDate { get; set; }
        public int? Typ { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
