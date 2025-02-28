using System;
using System.Collections.Generic;

namespace Usine_Core.ModelsAdmin
{
    public partial class CrmTickets
    {
        public int TicketId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? Dat { get; set; }
        public string CustomerIssue { get; set; }
        public string ServiceDescription { get; set; }
        public int? Statu { get; set; }
        public DateTime? ClearedDat { get; set; }

        public virtual CustomerRegistrations Customer { get; set; }
    }
}
