using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdAttendances
    {
        public int LineId { get; set; }
        public long? Empno { get; set; }
        public DateTime? Dat { get; set; }
        public string Attendance { get; set; }
        public string Ot { get; set; }
        public string Late { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
