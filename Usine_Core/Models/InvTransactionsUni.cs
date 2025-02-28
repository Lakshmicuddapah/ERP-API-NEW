using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class InvTransactionsUni
    {
        public InvTransactionsUni()
        {
            InvTransactionsDet = new HashSet<InvTransactionsDet>();
        }

        public int? RecordId { get; set; }
        public DateTime? Dat { get; set; }
        public string TraType { get; set; }
        public string Descriptio { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual ICollection<InvTransactionsDet> InvTransactionsDet { get; set; }
    }
}
