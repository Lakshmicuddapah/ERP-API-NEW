using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PartyDeaprtmentDetails
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public string Department { get; set; }
        public string Details { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual PartyDetails Record { get; set; }
    }
}
