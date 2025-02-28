using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class InvLosses
    {
        public int RecordId { get; set; }
        public string LossName { get; set; }
        public double? Allowableper { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
