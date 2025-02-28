using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PpcMassPlanningUni
    {
        public PpcMassPlanningUni()
        {
            PpcMassPlanningDet = new HashSet<PpcMassPlanningDet>();
        }

        public int? RecordId { get; set; }
        public string Seq { get; set; }
        public DateTime? Dat { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual ICollection<PpcMassPlanningDet> PpcMassPlanningDet { get; set; }
    }
}
