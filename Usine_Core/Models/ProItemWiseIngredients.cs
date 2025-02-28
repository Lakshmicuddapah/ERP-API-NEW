using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class ProItemWiseIngredients
    {
        public int? ItemId { get; set; }
        public int? Ingredient { get; set; }
        public double? Qty { get; set; }
        public int? Um { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public long Slno { get; set; }
    }
}
