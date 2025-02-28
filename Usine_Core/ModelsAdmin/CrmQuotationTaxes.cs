using System;
using System.Collections.Generic;

namespace Usine_Core.ModelsAdmin
{
    public partial class CrmQuotationTaxes
    {
        public int RecordId { get; set; }
        public int Sno { get; set; }
        public string TaxCode { get; set; }
        public double? TaxPer { get; set; }
        public double? TaxValue { get; set; }

        public virtual CrmQuotationUni Record { get; set; }
    }
}
