using System;
using System.Collections.Generic;

namespace Usine_Core
{
    public partial class InvGroups
    {
        public InvGroups()
        {
            InvMaterials = new HashSet<InvMaterials>();
        }

        public int RecordId { get; set; }
        public string MGrp { get; set; }
        public string SGrp { get; set; }
        public int? Sno { get; set; }
        public int? Chk { get; set; }
        public string GroupCode { get; set; }
        public string GrpTag { get; set; }
        public int? Statu { get; set; }
        public string Branchid { get; set; }
        public int? CustomerCode { get; set; }
        public string Pic { get; set; }

        public virtual ICollection<InvMaterials> InvMaterials { get; set; }
    }
}
