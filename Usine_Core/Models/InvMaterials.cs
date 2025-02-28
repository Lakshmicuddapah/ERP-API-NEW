using System;
using System.Collections.Generic;

namespace Usine_Core
{
    public partial class InvMaterials
    {
        public InvMaterials()
        {
            InvMaterialUnits = new HashSet<InvMaterialUnits>();
        }

        public int? RecordId { get; set; }
        public string Itemid { get; set; }
        public string ItemName { get; set; }
        public int? Grp { get; set; }
        public double? StdRate { get; set; }
        public double? ReOrderQty { get; set; }
        public int? ShelfLifeReqd { get; set; }
        public int? InventoryReqd { get; set; }
        public int? Statu { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public string Pic { get; set; }
        public int? VendorId { get; set; }
        public int? CostingType { get; set; }

        public virtual InvGroups GrpNavigation { get; set; }
        public virtual ICollection<InvMaterialUnits> InvMaterialUnits { get; set; }
    }
}
