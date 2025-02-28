using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdLeaveApplications
    {
        public int RecordId { get; set; }
        public long? Empno { get; set; }
        public DateTime? Dat { get; set; }
        public DateTime? LeaveFrom { get; set; }
        public DateTime? LeaveTo { get; set; }
        public int? ApprovalStatus { get; set; }
        public string ApprovedBy { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
