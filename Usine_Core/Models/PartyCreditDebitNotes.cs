using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PartyCreditDebitNotes
    {
        public long? RecordId { get; set; }
        public DateTime? Dat { get; set; }
        public int? TransactionId { get; set; }
        public string TransactionType { get; set; }
        public double? Amt { get; set; }
        public string Descriptio { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public string Seq { get; set; }
    }
}
