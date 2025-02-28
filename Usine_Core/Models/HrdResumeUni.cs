using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdResumeUni
    {
        public HrdResumeUni()
        {
            HrdResumeCurriculum = new HashSet<HrdResumeCurriculum>();
            HrdResumeExperience = new HashSet<HrdResumeExperience>();
        }

        public int? RecordId { get; set; }
        public string Seq { get; set; }
        public DateTime? Dat { get; set; }
        public DateTime? AppDate { get; set; }
        public string NameOfCandidate { get; set; }
        public string SurName { get; set; }
        public string FatherName { get; set; }
        public DateTime? Dob { get; set; }
        public int? Gender { get; set; }
        public int? MaritalStatus { get; set; }
        public string Reference { get; set; }
        public int? Designation { get; set; }
        public double? ExpectedSalary { get; set; }
        public string Addr { get; set; }
        public string Country { get; set; }
        public string Stat { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string PermenentId { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual ICollection<HrdResumeCurriculum> HrdResumeCurriculum { get; set; }
        public virtual ICollection<HrdResumeExperience> HrdResumeExperience { get; set; }
    }
}
