using System;

namespace Usine_Core.Models
{ 
    public partial class CrmTeleCallingRx
    {
        public int? RecordId { get; set; }
        public string Seq { get; set; }
        public DateTime? Dat { get; set; }
        public int? Callerid { get; set; }
        public string Customer { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public int? PrevcallId { get; set; }
        public string PrevCallMode { get; set; }
        public string CustomerComments { get; set; }
        public string CallerComments { get; set; }
        public int? CallPosition { get; set; }
        public DateTime? NextCallDate { get; set; }
        public string NextCallMode { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public string Username { get; set; }
        public int? nextCallId { get; set; }
    }
}
