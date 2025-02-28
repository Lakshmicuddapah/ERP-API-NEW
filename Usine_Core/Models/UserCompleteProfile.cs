using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class UserCompleteProfile
    {
        public string UsrName { get; set; }
        public string UserProfileName { get; set; }
        public long? EmployeeNo { get; set; }
        public string AboutUser { get; set; }
        public string BannerImage { get; set; }
        public string ThumbImage { get; set; }
        public string BranchId { get; set; }
        public int CustomerCode { get; set; }

        public virtual HrdEmployees EmployeeNoNavigation { get; set; }
    }
}
