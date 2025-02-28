using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class CrmPostTeleCalling
    {
        public CrmPostTeleCalling()
        {
            InverseNextCall = new HashSet<CrmPostTeleCalling>();
        }

        public int? RecordId { get; set; }
        public string Seq { get; set; }
        public DateTime? Dat { get; set; }
        public int? Callerid { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerComments { get; set; }
        public string CallerComments { get; set; }
        public int? CallType { get; set; }
        public DateTime? NextCallDate { get; set; }
        public int? NextCallMode { get; set; }
        public string UserName { get; set; }
        public int? NextCallId { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual CrmPostTeleCalling NextCall { get; set; }
        public virtual ICollection<CrmPostTeleCalling> InverseNextCall { get; set; }
    }
}
