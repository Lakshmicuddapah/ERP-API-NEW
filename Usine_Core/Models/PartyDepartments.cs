using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PartyDepartments
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public string Department { get; set; }
        public string DepartmentDetails { get; set; }
        public int? Statu { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
