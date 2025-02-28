using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class MaiEquipmentPreventiveMaintenances
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public string Prevmaintenance { get; set; }
        public int? FrequencyInDays { get; set; }
        public int? Chk { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
