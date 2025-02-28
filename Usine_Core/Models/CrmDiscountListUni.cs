using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class CrmDiscountListUni
    {
        public CrmDiscountListUni()
        {
            CrmDiscountListDet = new HashSet<CrmDiscountListDet>();
        }

        public long RecordId { get; set; }
        public string DiscountListName { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public int? Pos { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual ICollection<CrmDiscountListDet> CrmDiscountListDet { get; set; }
    }
}
