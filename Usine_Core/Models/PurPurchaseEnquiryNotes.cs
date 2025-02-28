using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PurPurchaseEnquiryNotes
    {
        public long? RecordId { get; set; }
        public int? Sno { get; set; }
        public string Note { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
