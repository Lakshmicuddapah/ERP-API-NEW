using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PpcBatchPlanningUni
    {
        public PpcBatchPlanningUni()
        {
            PpcBatchPlanningEmployeeAssignings = new HashSet<PpcBatchPlanningEmployeeAssignings>();
            PpcBatchPlanningProcesses = new HashSet<PpcBatchPlanningProcesses>();
        }

        public int? RecordId { get; set; }
        public string BatchNo { get; set; }
        public DateTime? Dat { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? ProductionIncharge { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public int? ItemId { get; set; }
        public double? Qty { get; set; }
        public int? Pos { get; set; }
        public virtual ICollection<PpcBatchPlanningEmployeeAssignings> PpcBatchPlanningEmployeeAssignings { get; set; }
        public virtual ICollection<PpcBatchPlanningProcesses> PpcBatchPlanningProcesses { get; set; }
    }
}
