using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class QcTraTestsUni
    {
        public QcTraTestsUni()
        {
            QcTraTestsDet = new HashSet<QcTraTestsDet>();
        }

        public long RecordId { get; set; }
        public DateTime? Dat { get; set; }
        public int? Testid { get; set; }
        public long? InspectedBy { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public string Seq { get; set; }
        public string Typ { get; set; }
        public string Usrname { get; set; }

        public virtual QcTestings Test { get; set; }
        public virtual ICollection<QcTraTestsDet> QcTraTestsDet { get; set; }
    }
}
