using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PartyPaymentDetails
    {
        public long Lineid { get; set; }
        public int? TransactionId { get; set; }
        public string TransactionType { get; set; }
        public DateTime? TransactionDate { get; set; }
        public int? PartyId { get; set; }
        public string PartyName { get; set; }
        public double? TotalAmt { get; set; }
        public double? AdvanceAmt { get; set; }
        public double? CreditNote { get; set; }
        public double? OtherAmounts { get; set; }
        public double? PaymentAmount { get; set; }
        public string Description { get; set; }
        public string Usrname { get; set; }
        public string BranchId { get; set; }
        public string CompanyId { get; set; }
    }
}
