using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PpcBatchPlanningSaleOrders
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public int? Soid { get; set; }
        public int? LineId { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
