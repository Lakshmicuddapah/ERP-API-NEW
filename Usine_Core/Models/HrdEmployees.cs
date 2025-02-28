using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdEmployees
    {
        public HrdEmployees()
        {
            HrdEmployeeAllowancesDeductions = new HashSet<HrdEmployeeAllowancesDeductions>();
            HrdEmployeeCurriculum = new HashSet<HrdEmployeeCurriculum>();
            HrdEmployeeExperience = new HashSet<HrdEmployeeExperience>();
            HrdEmployeeFamilyDetails = new HashSet<HrdEmployeeFamilyDetails>();
            HrdEmployeeIdentifications = new HashSet<HrdEmployeeIdentifications>();
            HrdEmployeeLeaves = new HashSet<HrdEmployeeLeaves>();
            InverseMgrNavigation = new HashSet<HrdEmployees>();
            UserCompleteProfile = new HashSet<UserCompleteProfile>();
        }

        public long? RecordId { get; set; }
        public string Empno { get; set; }
        public string Empname { get; set; }
        public string Surname { get; set; }
        public string Fathername { get; set; }
        public int? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public int? ModeofPay { get; set; }
        public int? MaritalStatus { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Stat { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Webid { get; set; }
        public string Pan { get; set; }
        public string Aadhar { get; set; }
        public string Idtype { get; set; }
        public string Idno { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string BloodGrp { get; set; }
        public string Referenc { get; set; }
        public int? Department { get; set; }
        public int? Designation { get; set; }
        public DateTime? Doj { get; set; }
        public long? Mgr { get; set; }
        public double? BasicPay { get; set; }
        public double? GrandPay { get; set; }
        public int? BasicChk { get; set; }
        public string LeavesScheme { get; set; }
        public int? BankPay { get; set; }
        public string SbAc { get; set; }
        public string Bankifscno { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string Paddr { get; set; }
        public string Pcountry { get; set; }
        public string Pstat { get; set; }
        public string Pdist { get; set; }
        public string Pcity { get; set; }
        public string Pzip { get; set; }
        public string Pmobile { get; set; }
        public string Ptel { get; set; }
        public string Pfax { get; set; }
        public string Pemail { get; set; }
        public string Pwebid { get; set; }
        public string Pic { get; set; }
        public string Branchid { get; set; }
        public int? CustomerCode { get; set; }
        public string Esi { get; set; }
        public string Pf { get; set; }

        public virtual HrdDepartments DepartmentNavigation { get; set; }
        public virtual HrdDesignations DesignationNavigation { get; set; }
        public virtual HrdEmployees MgrNavigation { get; set; }
        public virtual ICollection<HrdEmployeeAllowancesDeductions> HrdEmployeeAllowancesDeductions { get; set; }
        public virtual ICollection<HrdEmployeeCurriculum> HrdEmployeeCurriculum { get; set; }
        public virtual ICollection<HrdEmployeeExperience> HrdEmployeeExperience { get; set; }
        public virtual ICollection<HrdEmployeeFamilyDetails> HrdEmployeeFamilyDetails { get; set; }
        public virtual ICollection<HrdEmployeeIdentifications> HrdEmployeeIdentifications { get; set; }
        public virtual ICollection<HrdEmployeeLeaves> HrdEmployeeLeaves { get; set; }
        public virtual ICollection<HrdEmployees> InverseMgrNavigation { get; set; }
        public virtual ICollection<UserCompleteProfile> UserCompleteProfile { get; set; }
    }
}
