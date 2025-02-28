using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class ProItemWiseAttachmentsUni
    {
        public int ItemId { get; set; }
        public double? MinBatchQty { get; set; }
        public double? MaxBatchQty { get; set; }
        public int? UmId { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
