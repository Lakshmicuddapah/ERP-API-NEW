using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class FinAccountsAssign
    {
        public string Trans { get; set; }
        public string TraCode { get; set; }
        public int? Accname { get; set; }
        public string Branchid { get; set; }
        public int? Customercode { get; set; }
        public int Sno { get; set; }

        public virtual FinAccounts AccnameNavigation { get; set; }
    }
}
