using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class UserPostingAccess
    {
        public long PostingId { get; set; }
        public DateTime? Dat { get; set; }
        public string UserToAccess { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual UserPostings Posting { get; set; }
    }
}
