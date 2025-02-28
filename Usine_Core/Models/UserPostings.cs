using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class UserPostings
    {
        public UserPostings()
        {
            UserPostingAccess = new HashSet<UserPostingAccess>();
            UserPostingsComments = new HashSet<UserPostingsComments>();
            UserPostingsLikes = new HashSet<UserPostingsLikes>();
        }

        public long PostingId { get; set; }
        public DateTime? Dat { get; set; }
        public string PostSubject { get; set; }
        public string Postdetail { get; set; }
        public int? Postlikes { get; set; }
        public int? Postcomments { get; set; }
        public int? PostcommentsEnable { get; set; }
        public string UserCode { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual ICollection<UserPostingAccess> UserPostingAccess { get; set; }
        public virtual ICollection<UserPostingsComments> UserPostingsComments { get; set; }
        public virtual ICollection<UserPostingsLikes> UserPostingsLikes { get; set; }
    }
}
