using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class UserPostingsComments
    {
        public long CommentId { get; set; }
        public long? PostingId { get; set; }
        public string UserName { get; set; }
        public DateTime? Dat { get; set; }
        public string Comment { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual UserPostings Posting { get; set; }
    }
}
