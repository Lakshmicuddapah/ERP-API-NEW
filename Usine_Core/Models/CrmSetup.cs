using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class CrmSetup
    {
        public string SetupCode { get; set; }
        public string SetupDescription { get; set; }
        public string Pos { get; set; }
        public string PosDescription { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public int Sno { get; set; }
    }
}
