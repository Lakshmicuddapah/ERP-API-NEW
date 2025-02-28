using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Usine_Core.Models
{
    public partial class InvMaterialManagement
    {
        [Key]
        public int? TransactionId { get; set; }
        public int? Sno { get; set; }
        public string Gin { get; set; }
        public int? ItemName { get; set; }
        public DateTime? Dat { get; set; }
        public string BatchNo { get; set; }
        public DateTime? Manudate { get; set; }
        public DateTime? Expdate { get; set; }
        public int? Store { get; set; }
        public double? Qtyin { get; set; }
        public double? Qtyout { get; set; }
        public int? Stdum { get; set; }
        public double? Rat { get; set; }
        public int? Department { get; set; }
        public int? ProductBatchNo { get; set; }
        public string Descr { get; set; }
        public int? TransactionType { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public int? LineId { get; set; }
    }
}
