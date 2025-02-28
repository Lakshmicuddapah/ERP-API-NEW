using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class TransactionsAudit
    {
        public int Slno { get; set; }
        public int? TraId { get; set; }
        public string Descr { get; set; }
        public string Usr { get; set; }
        public int? Tratype { get; set; }
        public string Transact { get; set; }
        public string TraModule { get; set; }
        public string Syscode { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public DateTime? Dat { get; set; }
    }
}
