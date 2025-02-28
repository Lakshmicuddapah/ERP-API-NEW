using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdDesignationsAllowances
    {
        public int? RecordId { get; set; }
        public int LineId { get; set; }
        public int? Allowance { get; set; }
        public double? Valu { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual HrdDesignations Record { get; set; }
    }
}
