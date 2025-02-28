using System;
using System.Collections.Generic;

namespace Usine_Core.ModelsAdmin
{
    public partial class CustomerBranches
    {
        public int CustomerId { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; }
        public string BranchHead { get; set; }
        public string Addr { get; set; }
        public string Country { get; set; }
        public string Stat { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Web { get; set; }
        public int? Outlets { get; set; }
    }
}
