using System;
using System.Collections.Generic;

namespace Usine_Core.ModelsAdmin
{
    public partial class CustomerReceiptsDet
    {
        public int RecordId { get; set; }
        public int Sno { get; set; }
        public string Module { get; set; }
        public double? Amount { get; set; }
    }
}
