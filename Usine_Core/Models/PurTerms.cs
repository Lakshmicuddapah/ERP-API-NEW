using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PurTerms
    {
        public int Slno { get; set; }
        public string Term { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
