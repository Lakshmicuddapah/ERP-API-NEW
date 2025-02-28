using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PartyAddresses
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public string AddressName { get; set; }
        public string Addres { get; set; }
        public string Country { get; set; }
        public string Stat { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Webid { get; set; }
        public int? Statu { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
