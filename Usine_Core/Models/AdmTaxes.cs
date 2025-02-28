using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class AdmTaxes
    {
        public int RecordId { get; set; }
        public string TaxCode { get; set; }
        public double? TaxPer { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
