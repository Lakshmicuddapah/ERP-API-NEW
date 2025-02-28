using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class MaiEquipment
    {
        public MaiEquipment()
        {
            MaiEquipmentInsurances = new HashSet<MaiEquipmentInsurances>();
            MaiEquipmentPmdetails = new HashSet<MaiEquipmentPmdetails>();
        }

        public int? RecordId { get; set; }
        public string EquipmentCode { get; set; }
        public string EquipmentName { get; set; }
        public int? EquipmentGroup { get; set; }
        public int? PreferableServiceSupplier { get; set; }
        public int? PreferableSparesSupplier { get; set; }
        public int? MobileCheck { get; set; }
        public int? Roomno { get; set; }
        public DateTime? AmcDate { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public DateTime? LastPmdate { get; set; }
        public int? MaxHrs { get; set; }

        public virtual MaiEquipGroups EquipmentGroupNavigation { get; set; }
        public virtual ICollection<MaiEquipmentInsurances> MaiEquipmentInsurances { get; set; }
        public virtual ICollection<MaiEquipmentPmdetails> MaiEquipmentPmdetails { get; set; }
    }
}
