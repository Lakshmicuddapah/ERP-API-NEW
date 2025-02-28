using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class MaiEquipmentPmdetails
    {
        public long RecordId { get; set; }
        public int? Equipid { get; set; }
        public int? Sno { get; set; }
        public DateTime? Dat { get; set; }
        public string Descriptio { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual MaiEquipment Equip { get; set; }
    }
}
