using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class MisTasks
    {
        public int TaskId { get; set; }
        public DateTime? Dat { get; set; }
        public string ToUsr { get; set; }
        public string FrmUsr { get; set; }
        public string Subjec { get; set; }
        public string Descriptio { get; set; }
        public DateTime? RequiredBy { get; set; }
        public int? Priority { get; set; }
        public int? Pos { get; set; }
        public DateTime? ReadDat { get; set; }
        public DateTime? ClearDat { get; set; }
        public string Branchid { get; set; }
        public int? CustomerCode { get; set; }
    }
}
