using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PpcBatchProcessWiseDetailsUni
    {
        public PpcBatchProcessWiseDetailsUni()
        {
            PpcBatchProcessWiseDetailsDet = new HashSet<PpcBatchProcessWiseDetailsDet>();
        }

        public int? RecordId { get; set; }
        public string JobCardNo { get; set; }
        public DateTime? Dat { get; set; }
        public double? Qty { get; set; }
        public int? Um { get; set; }
        public long? ProcessIncharge { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public int? ProcessId { get; set; }
        public virtual ICollection<PpcBatchProcessWiseDetailsDet> PpcBatchProcessWiseDetailsDet { get; set; }
    }
}
