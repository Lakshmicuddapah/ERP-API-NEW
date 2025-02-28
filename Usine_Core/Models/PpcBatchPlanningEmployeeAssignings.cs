using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PpcBatchPlanningEmployeeAssignings
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public long? Employee { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual PpcBatchPlanningUni Record { get; set; }
    }
}
