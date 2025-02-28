using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdShifts
    {
        public int RecordId { get; set; }
        public string ShiftName { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
