using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PartyPaymentsuni
    {
        public long? RecordId { get; set; }
        public string Seq { get; set; }
        public DateTime? Dat { get; set; }
        public int? PartyId { get; set; }
        public double? BaseAmt { get; set; }
        public double? Tds { get; set; }
        public double? Rebate { get; set; }
        public double? Others { get; set; }
        public double? ReceiptAmt { get; set; }
        public string ModeOfPayment { get; set; }
        public int? RevAccount { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public string VoucherType { get; set; }
    }
}
