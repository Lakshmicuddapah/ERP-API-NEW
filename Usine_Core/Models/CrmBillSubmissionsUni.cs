using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class CrmBillSubmissionsUni
    {
        public CrmBillSubmissionsUni()
        {
            CrmBillSubmissionsDet = new HashSet<CrmBillSubmissionsDet>();
        }

        public int? RecordId { get; set; }
        public string Seq { get; set; }
        public DateTime? Dat { get; set; }
        public int? CustomerId { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual ICollection<CrmBillSubmissionsDet> CrmBillSubmissionsDet { get; set; }
    }
}
