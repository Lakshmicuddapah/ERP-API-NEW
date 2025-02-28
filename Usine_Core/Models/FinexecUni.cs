using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class FinexecUni
    {
        public FinexecUni()
        {
            FinexecDet = new HashSet<FinexecDet>();
            FinexecUniHistory = new HashSet<FinexecUniHistory>();
        }

        public int? RecordId { get; set; }
        public int? Seq { get; set; }
        public DateTime? Dat { get; set; }
        public string Narr { get; set; }
        public string Tratype { get; set; }
        public string Traref { get; set; }
        public string Vouchertype { get; set; }
        public string BankDet { get; set; }
        public string Branchid { get; set; }
        public int? CustomerCode { get; set; }
        public string Usr { get; set; }
        public int? PrintCount { get; set; }
        public string detail1 { get; set; }
        public string detail2 { get; set; }
        public string detail3 { get; set; }
        public string detail4 { get; set; }
        public virtual ICollection<FinexecDet> FinexecDet { get; set; }
        public virtual ICollection<FinexecUniHistory> FinexecUniHistory { get; set; }
    }
}
