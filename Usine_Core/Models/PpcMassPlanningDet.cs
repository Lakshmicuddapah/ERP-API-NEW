using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PpcMassPlanningDet
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public int? ItemId { get; set; }
        public double? Qty { get; set; }
        public int? Um { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual PpcMassPlanningUni Record { get; set; }
    }
}
