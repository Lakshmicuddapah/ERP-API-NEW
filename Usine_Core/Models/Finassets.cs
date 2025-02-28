using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class Finassets
    {
        public int RecordId { get; set; }
        public string AssetName { get; set; }
        public decimal? Depreciation { get; set; }
        public decimal? OpeningValue { get; set; }
        public DateTime? Date { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
