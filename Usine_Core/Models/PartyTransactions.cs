using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PartyTransactions
    {
        public int? RecordId { get; set; }
        public int? TransactionId { get; set; }
        public string TransactionType { get; set; }
        public DateTime? Dat { get; set; }
        public int? PartyId { get; set; }
        public string PartyName { get; set; }
        public double? TransactionAmt { get; set; }
        public double? PendingAmount { get; set; }
        public double? ReturnAmount { get; set; }
        public double? CreditNote { get; set; }
        public double? PaymentAmount { get; set; }
        public string Descriptio { get; set; }
        public string Username { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public int? OnTraId { get; set; }
    }
}
