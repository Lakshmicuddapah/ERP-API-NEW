using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class TotAdvanceDetails
    {
        public int? RecordId { get; set; }
        public string Seq { get; set; }
        public int? TransactionId { get; set; }
        public string Tratype { get; set; }
        public DateTime? Dat { get; set; }
        public double? Amt { get; set; }
        public string Paymentmode { get; set; }
        public string Remarks { get; set; }
        public string Bankdetails { get; set; }
        public string UsrName { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public int? PartyId { get; set; }
        public int? AccountId { get; set; }
        public string Detail1 { get; set; }
        public string Detail2 { get; set; }
        public string Detail3 { get; set; }
        public int? PrintCount { get; set; }
    }
}
