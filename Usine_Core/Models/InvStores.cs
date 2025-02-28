using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class InvStores
    {
        public int RecordId { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string StoreIncharge { get; set; }
        public int? Pos { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
