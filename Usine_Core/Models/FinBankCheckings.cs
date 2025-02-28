using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class FinBankCheckings
    {
        public int RecordId { get; set; }
        public int Sno { get; set; }
        public DateTime? Dat { get; set; }
        public string Details { get; set; }
        public int? Bank { get; set; }
        public double? Amt { get; set; }
        public string Description { get; set; }
        public int? Typ { get; set; }
        public int? Pos { get; set; }
        public string Usrname { get; set; }
        public DateTime? ClearedDat { get; set; }
        public string Clearedby { get; set; }
        public string Branchid { get; set; }
        public int? CustomerCode { get; set; }
    }
}
