using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class CrmTargetSettings
    {
        public long? Slno { get; set; }
        public DateTime? Dat { get; set; }
        public long? Empno { get; set; }
        public int? Yea { get; set; }
        public int? Mont { get; set; }
        public int? CategoryId { get; set; }
        public int? ProductId { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public double? Tgt { get; set; }
        public int? Calls { get; set; }
    }
}
