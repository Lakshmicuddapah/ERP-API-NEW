using System;
using System.Collections.Generic;

namespace Usine_Core.ModelsAdmin
{
    public partial class CustomerReceiptsUni
    {
        public int RecordId { get; set; }
        public string ReceiptNo { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? Dat { get; set; }
        public double? ReceiptAmount { get; set; }
        public int? ModeofPayment { get; set; }

        public virtual CustomerRegistrations Customer { get; set; }
    }
}
