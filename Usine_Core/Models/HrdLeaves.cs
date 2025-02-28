using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdLeaves
    {
        public HrdLeaves()
        {
            HrdDesignationsLeaves = new HashSet<HrdDesignationsLeaves>();
            HrdEmployeeLeaves = new HashSet<HrdEmployeeLeaves>();
        }

        public int RecordId { get; set; }
        public string LeaveCode { get; set; }
        public string LeaveDescription { get; set; }
        public int? PayType { get; set; }
        public int? ForwardType { get; set; }
        public string BranchId { get; set; }
        public int? Customercode { get; set; }

        public virtual ICollection<HrdDesignationsLeaves> HrdDesignationsLeaves { get; set; }
        public virtual ICollection<HrdEmployeeLeaves> HrdEmployeeLeaves { get; set; }
    }
}
