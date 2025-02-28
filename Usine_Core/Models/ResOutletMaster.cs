using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class ResOutletMaster
    {
        public string RestaCode { get; set; }
        public string RestaName { get; set; }
        public int? Outlettype { get; set; }
        public int BillingGroup { get; set; }
        public int? AutoSettleChck { get; set; }
        public string Branchid { get; set; }
        public int CustomerCode { get; set; }
    }
}
