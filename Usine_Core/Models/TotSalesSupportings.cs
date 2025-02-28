using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class TotSalesSupportings
    {
        public int? BillNo { get; set; }
        public string GuestName { get; set; }
        public string Addr { get; set; }
        public string Mobile { get; set; }
        public double? Amt { get; set; }
        public string Descript { get; set; }
        public string BankDetails { get; set; }
        public int? AccName { get; set; }
        public int? RoomCheckinid { get; set; }
        public string BillType { get; set; }
        public string SettleMode { get; set; }
        public string Usrname { get; set; }
        public DateTime? SettleDate { get; set; }
        public string Branchid { get; set; }
        public int? CustomerCode { get; set; }
        public int Slno { get; set; }
    }
}
