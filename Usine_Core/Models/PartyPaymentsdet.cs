using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PartyPaymentsdet
    {
        public long RecordId { get; set; }
        public int Sno { get; set; }
        public int? Billno { get; set; }
        public string BillType { get; set; }
        public double? PaymentAmt { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
