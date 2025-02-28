using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class QcTestings
    {
        public QcTestings()
        {
            QcTraTestsUni = new HashSet<QcTraTestsUni>();
        }

        public int RecordId { get; set; }
        public string Testname { get; set; }
        public string TestArea { get; set; }
        public int? MicroCheck { get; set; }
        public int? CheckingType { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual ICollection<QcTraTestsUni> QcTraTestsUni { get; set; }
    }
}
