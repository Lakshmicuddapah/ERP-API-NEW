using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdEmpInOutDetails
    {
        public long? Lineid { get; set; }
        public long? EmpId { get; set; }
        public DateTime? Dat { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public int? Pos { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
