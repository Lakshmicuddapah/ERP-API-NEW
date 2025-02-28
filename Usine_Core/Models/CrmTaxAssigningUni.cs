using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class CrmTaxAssigningUni
    {
        public CrmTaxAssigningUni()
        {
            CrmTaxAssigningDet = new HashSet<CrmTaxAssigningDet>();
        }

        public int RecordId { get; set; }
        public string TaxName { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual ICollection<CrmTaxAssigningDet> CrmTaxAssigningDet { get; set; }
    }
}
