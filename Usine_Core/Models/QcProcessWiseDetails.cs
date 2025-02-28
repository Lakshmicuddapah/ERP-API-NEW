using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class QcProcessWiseDetails
    {
        public int RecordId { get; set; }
        public int? ProcessId { get; set; }
        public DateTime? Dat { get; set; }
        public DateTime? Fromdate { get; set; }
        public DateTime? Todate { get; set; }
        public long? QcIncharge { get; set; }
        public int? Test { get; set; }
        public double? SamplesCollected { get; set; }
        public double? Rectified { get; set; }
        public double? Rejected { get; set; }
        public double? RectificationValue { get; set; }
        public double? RejectedValue { get; set; }
        public string Descriptio { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public int? BatchNo { get; set; }
    }
}
