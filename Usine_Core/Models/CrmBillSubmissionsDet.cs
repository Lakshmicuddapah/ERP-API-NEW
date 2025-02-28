using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class CrmBillSubmissionsDet
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public int? Billno { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual CrmBillSubmissionsUni Record { get; set; }
    }
}
