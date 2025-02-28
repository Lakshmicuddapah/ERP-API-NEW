using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class MisCoveringLetterDetails
    {
        public int? Slno { get; set; }
        public string Typ { get; set; }
        public string Subjec { get; set; }
        public string Body { get; set; }
        public string Salutation { get; set; }
        public string Img { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
