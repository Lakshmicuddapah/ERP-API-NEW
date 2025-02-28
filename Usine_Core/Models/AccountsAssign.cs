using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class AccountsAssign
    {
        public int Slno { get; set; }
        public string Transcode { get; set; }
        public int? Account { get; set; }
        public string Module { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
