using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PurPurchaseEnquiryUniHistory
    {
        public long AuditId { get; set; }
        public DateTime? AuditDat { get; set; }
        public int? AuditType { get; set; }
        public long? RecordId { get; set; }
        public string Seq { get; set; }
        public DateTime? Dat { get; set; }
        public string Usr { get; set; }
        public DateTime? ApprvedDat { get; set; }
        public string ApprovedUser { get; set; }
        public DateTime? Validity { get; set; }
        public string Reference { get; set; }
        public int? Supid { get; set; }
        public string Supplier { get; set; }
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
        public int? Pos { get; set; }
        public int? PrintCount { get; set; }
        public int? MailCount { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
