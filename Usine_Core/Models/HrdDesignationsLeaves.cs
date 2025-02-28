using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdDesignationsLeaves
    {
        public int? RecordId { get; set; }
        public int Lineid { get; set; }
        public int? LeaveId { get; set; }
        public int? Valu { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual HrdLeaves Leave { get; set; }
        public virtual HrdDesignations Record { get; set; }
    }
}
