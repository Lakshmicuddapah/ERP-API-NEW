using System;
using System.Collections.Generic;

namespace Usine_Core
{
    public partial class InvMaterialUnits
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public int? Um { get; set; }
        public int? StdUm { get; set; }
        public double? ConversionFactor { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual InvMaterials Record { get; set; }
        public virtual InvUm UmNavigation { get; set; }
    }
}
