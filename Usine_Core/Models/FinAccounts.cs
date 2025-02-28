using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class FinAccounts
    {
        public FinAccounts()
        {
            FinAccountsAssign = new HashSet<FinAccountsAssign>();
            FinexecDet = new HashSet<FinexecDet>();
        }

        public int? Recordid { get; set; }
        public string Accname { get; set; }
        public int? Accgroup { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Pin { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string WebId { get; set; }
        public string AcType { get; set; }
        public int? AcChk { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual ICollection<FinAccountsAssign> FinAccountsAssign { get; set; }
        public virtual ICollection<FinexecDet> FinexecDet { get; set; }
    }
}
