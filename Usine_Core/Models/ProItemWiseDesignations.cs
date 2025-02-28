using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class ProItemWiseDesignations
    {
        public long Slno { get; set; }
        public int? Itemid { get; set; }
        public int? DesigId { get; set; }
        public int? Manhrs { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
