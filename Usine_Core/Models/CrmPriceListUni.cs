using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class CrmPriceListUni
    {
        public CrmPriceListUni()
        {
            CrmPriceListDet = new HashSet<CrmPriceListDet>();
        }

        public long RecordId { get; set; }
        public string PriceListName { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public int? Pos { get; set; }
        public int? MrpCheck { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public int? Currency { get; set; }

        public virtual ICollection<CrmPriceListDet> CrmPriceListDet { get; set; }
    }
}
