using System;
using System.Collections.Generic;

namespace Usine_Core.ModelsAdmin
{
    public partial class CrmOrderApproval
    {
        public int RecordId { get; set; }
        public int Sno { get; set; }
        public string Module { get; set; }
        public string Descriptio { get; set; }
        public DateTime? Trainingdays { get; set; }
        public double? Price { get; set; }
        public int? VendorId { get; set; }
    }
}
