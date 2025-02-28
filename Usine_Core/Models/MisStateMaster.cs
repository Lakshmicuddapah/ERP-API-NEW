using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class MisStateMaster
    {
        public int RecordId { get; set; }
        public int? Cntname { get; set; }
        public string StateName { get; set; }
        public string GstStart { get; set; }
        public int? CustomerCode { get; set; }

        public virtual MisCountryMaster CntnameNavigation { get; set; }
    }
}
