using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PpcProcessesMaster
    {
        public int RecordId { get; set; }
        public string ProcessName { get; set; }
        public int? QcRequired { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
