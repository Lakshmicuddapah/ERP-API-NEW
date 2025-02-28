using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdInterviewsPanel
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public long? Empno { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual HrdInterviewsUni Record { get; set; }
    }
}
