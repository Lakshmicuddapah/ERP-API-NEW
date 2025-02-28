using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class Admroles
    {
        public string RoleName { get; set; }
        public int? ModuleCode { get; set; }
        public int? MenuCode { get; set; }
        public int? ScreenCode { get; set; }
        public int? TransCode { get; set; }
        public int? Pos { get; set; }
        public int? CustomerCode { get; set; }
        public int Sno { get; set; }
    }
}
