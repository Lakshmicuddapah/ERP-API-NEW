using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class MaiEquipmentInsurances
    {
        public long RecordId { get; set; }
        public int? Equipid { get; set; }
        public DateTime? Dat { get; set; }
        public int? VendorId { get; set; }
        public string VendorName { get; set; }
        public string Descriptio { get; set; }
        public DateTime? InsureFrom { get; set; }
        public DateTime? InsureTo { get; set; }
        public double? InsureAmt { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual MaiEquipment Equip { get; set; }
        public virtual PartyDetails Vendor { get; set; }
    }
}
