using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdResumeExperience
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public int? FromYear { get; set; }
        public int? ToYead { get; set; }
        public string Designation { get; set; }
        public string Organisation { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual HrdResumeUni Record { get; set; }
    }
}
