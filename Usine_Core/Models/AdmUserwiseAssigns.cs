using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class AdmUserwiseAssigns
    {
        public string UserName { get; set; }
        public string AssignedId { get; set; }
        public string AssignedTyp { get; set; }
        public int? CustomerCode { get; set; }
        public int? sno { get; set; }
    }
}
