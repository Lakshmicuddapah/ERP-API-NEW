using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PurEmails
    {
        public int Sno { get; set; }
        public string SetupCode { get; set; }
        public string SetupValue { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
