using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class UsrAut
    {
        public string UsrName { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public string Pwd { get; set; }
        public int? Pos { get; set; }
        public int CustomerCode { get; set; }
        public int? WebFreeEnable { get; set; }
        public int? MobileFreeEnable { get; set; }
    }
}
