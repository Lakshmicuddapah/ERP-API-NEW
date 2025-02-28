using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class FinexecDet
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public int? Accname { get; set; }
        public double? Cre { get; set; }
        public double? Deb { get; set; }
        public string Branchid { get; set; }
        public int? CustomerCode { get; set; }
        public DateTime? Dat { get; set; }

        public virtual FinAccounts AccnameNavigation { get; set; }
        public virtual FinexecUni Record { get; set; }
    }
}
