using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class CrmEnquiriesRx
    {
        public int? RecordId { get; set; }
        public string Seq { get; set; }
        public DateTime? Dat { get; set; }
        public int? Callerid { get; set; }
        public string Customer { get; set; }
        public int? MainCustomerId { get; set; }
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
        public int? PrevcallId { get; set; }
        public string PrevCallMode { get; set; }
        public string CustomerComments { get; set; }
        public string CallerComments { get; set; }
        public int? CallPosition { get; set; }
        public DateTime? NextCallDate { get; set; }
        public int? NextCallMode { get; set; }
        public int? Statu { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public string username { get; set; }
        public string PriceList { get; set; }
        public string DiscountList { get; set; }
        public int? nextCallId { get; set; }
        public int? Type { get; set; }
        public string Reference { get; set; }
    }
}
