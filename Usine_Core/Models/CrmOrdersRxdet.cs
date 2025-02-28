using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class CrmOrdersRxdet
    {
        public int RecordId { get; set; }
        public int Sno { get; set; }
        public DateTime? Dat { get; set; }
        public string Product { get; set; }
        public string Coating { get; set; }
        public string Dia { get; set; }
        public string Spherical { get; set; }
        public string Cylindrical { get; set; }
        public string Additional { get; set; }
        public string Side { get; set; }
        public string Shade { get; set; }
        public int? Qty { get; set; }
        public int? Rat { get; set; }
        public int? Rxqty { get; set; }
        public int? Rxqc { get; set; }
        public int? Hcqty { get; set; }
        public int? Hmcqty { get; set; }
        public int? TintQty { get; set; }
        public int? FinalQc { get; set; }
        public int? DespatchedQty { get; set; }
        public int? Typ { get; set; }
        public int? Pos { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public string blankBase { get; set; }
        public int? blanksInvQty { get; set; }
    }
}
