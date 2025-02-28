using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class FinassetsView
    {
        public int RecordId { get; set; }
        public string AssetName { get; set; }
        public decimal? Depreciation { get; set; }
        public string Opvalue { get; set; }
        public DateTime? Opedate { get; set; }
        public string Presetnvalue { get; set; }
        public string Branchid { get; set; }
        public int? CustomerCode { get; set; }
    }
}
