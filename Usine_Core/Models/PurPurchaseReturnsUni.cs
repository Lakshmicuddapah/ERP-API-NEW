using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PurPurchaseReturnsUni
    {
        public PurPurchaseReturnsUni()
        {
            PurPurchaseReturnTaxes = new HashSet<PurPurchaseReturnTaxes>();
            PurPurchaseReturnsDet = new HashSet<PurPurchaseReturnsDet>();
        }

        public int RecordId { get; set; }
        public string Seq { get; set; }
        public DateTime? Dat { get; set; }
        public DateTime? ApprovedDat { get; set; }
        public string Usr { get; set; }
        public string ApprovedUsr { get; set; }
        public string Transporter { get; set; }
        public int? RefMir { get; set; }
        public int? Vendorid { get; set; }
        public string Vendorname { get; set; }
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
        public string Webid { get; set; }
        public double? Baseamt { get; set; }
        public double? Discount { get; set; }
        public double? Taxes { get; set; }
        public double? Others { get; set; }
        public double? TotalAmt { get; set; }
        public int? Pos { get; set; }
        public int? Settlemode { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public double? CurrencyConversion { get; set; }
        public string CurrencySymbol { get; set; }

        public virtual ICollection<PurPurchaseReturnTaxes> PurPurchaseReturnTaxes { get; set; }
        public virtual ICollection<PurPurchaseReturnsDet> PurPurchaseReturnsDet { get; set; }
    }
}
