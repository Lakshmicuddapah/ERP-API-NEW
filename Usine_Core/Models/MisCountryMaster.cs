using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class MisCountryMaster
    {
        public MisCountryMaster()
        {
            MisStateMaster = new HashSet<MisStateMaster>();
        }

        public int RecordId { get; set; }
        public string Cntname { get; set; }
        public string Curr { get; set; }
        public string CurrSymbol { get; set; }
        public double? ConversionFactor { get; set; }
        public int? Statu { get; set; }
        public int? CustomerCode { get; set; }
        public string Coins { get; set; }

        public virtual ICollection<MisStateMaster> MisStateMaster { get; set; }
    }
}
