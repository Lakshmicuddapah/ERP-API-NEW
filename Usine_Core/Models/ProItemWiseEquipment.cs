using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class ProItemWiseEquipment
    {
        public long Slno { get; set; }
        public int? Itemid { get; set; }
        public int? EquipmentId { get; set; }
        public int? Manhrs { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
