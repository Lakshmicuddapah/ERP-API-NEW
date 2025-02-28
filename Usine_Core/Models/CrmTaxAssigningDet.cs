using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class CrmTaxAssigningDet
    {
        public int RecordId { get; set; }
        public int Sno { get; set; }
        public string TaxCode { get; set; }
        public double? Taxper { get; set; }
        public string TaxOn { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual CrmTaxAssigningUni Record { get; set; }
    }
}
