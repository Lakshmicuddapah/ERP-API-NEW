using System;
using System.Collections.Generic;

namespace Usine_Core.ModelsAdmin
{
    public partial class ProductModules
    {
        public string ProductCode { get; set; }
        public int Sno { get; set; }
        public string ModuleName { get; set; }
        public string ModuleDescription { get; set; }
        public int? TrainingDays { get; set; }
        public double? Price { get; set; }

        public virtual ProductDetails ProductCodeNavigation { get; set; }
    }
}
