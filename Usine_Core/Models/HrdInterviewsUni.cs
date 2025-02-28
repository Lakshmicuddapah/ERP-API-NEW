using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdInterviewsUni
    {
        public HrdInterviewsUni()
        {
            HrdInterviewCandidates = new HashSet<HrdInterviewCandidates>();
            HrdInterviewsPanel = new HashSet<HrdInterviewsPanel>();
        }

        public int? RecordId { get; set; }
        public string Seq { get; set; }
        public DateTime? Dat { get; set; }
        public DateTime? InterviewDate { get; set; }
        public string Venue { get; set; }
        public string Descriptio { get; set; }
        public int? Designation { get; set; }
        public int? Pos { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual ICollection<HrdInterviewCandidates> HrdInterviewCandidates { get; set; }
        public virtual ICollection<HrdInterviewsPanel> HrdInterviewsPanel { get; set; }
    }
}
