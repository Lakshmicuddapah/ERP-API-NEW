using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdInterviewCandidates
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public int? ResumeId { get; set; }
        public int? AppointmentStatus { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public DateTime? MaxDateToJoin { get; set; }
        public int? JoiningStatus { get; set; }
        public DateTime? JoiningDate { get; set; }
        public long? RefEmpNo { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public string InterviewerComments { get; set; }
        public string AppointmentComments { get; set; }
        public string JoiningComments { get; set; }
        public virtual HrdInterviewsUni Record { get; set; }
    }
}
