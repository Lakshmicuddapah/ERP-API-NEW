using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class HrdDesignations
    {
        public HrdDesignations()
        {
            HrdDesignationsAllowances = new HashSet<HrdDesignationsAllowances>();
            HrdDesignationsLeaves = new HashSet<HrdDesignationsLeaves>();
            HrdEmployees = new HashSet<HrdEmployees>();
        }

        public int RecordId { get; set; }
        public string Designation { get; set; }
        public int? Department { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual HrdDepartments DepartmentNavigation { get; set; }
        public virtual ICollection<HrdDesignationsAllowances> HrdDesignationsAllowances { get; set; }
        public virtual ICollection<HrdDesignationsLeaves> HrdDesignationsLeaves { get; set; }
        public virtual ICollection<HrdEmployees> HrdEmployees { get; set; }
    }
}
