using System;
using System.Collections.Generic;

namespace Usine_Core.ModelsAdmin
{
    public partial class Usraut
    {
        public string Username { get; set; }
        public string Rolename { get; set; }
        public string Pwd { get; set; }
        public int? Pos { get; set; }
        public string MainUser { get; set; }
        public int VendorCode { get; set; }
    }
}
