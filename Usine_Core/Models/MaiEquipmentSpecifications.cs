﻿using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class MaiEquipmentSpecifications
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public string Specification { get; set; }
        public string Valu { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
