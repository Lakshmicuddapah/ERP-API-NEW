using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PpcBatchProcessWiseDetailsDet
    {
        public int? RecordId { get; set; }
        public int? Batchno { get; set; }
        public int? LineId { get; set; }
        public int? JobCardNo { get; set; }
        public double? Qty { get; set; }
        public int? Um { get; set; }
        public int? Pos { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual PpcBatchProcessWiseDetailsUni JobCardNoNavigation { get; set; }
    }
}
