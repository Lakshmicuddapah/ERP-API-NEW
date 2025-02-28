using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PpcBatchPlanningProcesses
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public int? ProcessId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? QcRequired { get; set; }
        public long? ProcessIncharge { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual PpcBatchPlanningUni Record { get; set; }
    }
}
