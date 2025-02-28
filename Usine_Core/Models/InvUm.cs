using System;
using System.Collections.Generic;

namespace Usine_Core
{
    public partial class InvUm
    {
        public InvUm()
        {
            InvMaterialUnits = new HashSet<InvMaterialUnits>();
        }

        public int? RecordId { get; set; }
        public string Um { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual ICollection<InvMaterialUnits> InvMaterialUnits { get; set; }
    }
}
