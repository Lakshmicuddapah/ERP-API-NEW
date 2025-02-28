using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class MakeSessions
    {
        public int Sno { get; set; }
        public string UserName { get; set; }
        public int? CustomerCode { get; set; }
        public string KCode { get; set; }
        public DateTime? LogDate { get; set; }
        public DateTime? SessionClose { get; set; }
        public int? Pos { get; set; }
    }
}
