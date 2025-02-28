using System;
using System.Collections.Generic;

namespace Usine_Core.ModelsAdmin
{
    public partial class CrmFollowup
    {
        public int RecordId { get; set; }
        public int? EnquiryId { get; set; }
        public DateTime? Dat { get; set; }
        public string Usrname { get; set; }
        public string CustomerComments { get; set; }
        public string CallerComments { get; set; }
        public DateTime? NextCallDate { get; set; }
        public string NextCallMode { get; set; }
        public int? Statu { get; set; }

        public virtual CrmEnquiries Enquiry { get; set; }
    }
}
