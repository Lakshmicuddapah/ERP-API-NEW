using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class Purpurchasetypes
    {
        public int Slno { get; set; }
        public string Purtype { get; set; }
        public int? ImportType { get; set; }
        public int? CustomerCode { get; set; }
    }
}
