using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class SalSalesUni
    {
        public SalSalesUni()
        {
            SalSalesDet = new HashSet<SalSalesDet>();
            SalSalesTaxes = new HashSet<SalSalesTaxes>();
        }

        public int RecordId { get; set; }
        public string Seq { get; set; }
        public DateTime? Dat { get; set; }
        public DateTime? ApprovedDat { get; set; }
        public string Usr { get; set; }
        public string ApprovedUsr { get; set; }
        public string Dcno { get; set; }
        public DateTime? Dcdat { get; set; }
        public string Transporter { get; set; }
        public string SaleType { get; set; }
        public int? RefSoid { get; set; }
        public int? PartyId { get; set; }
        public string PartyName { get; set; }
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
        public int? PassCodeCheck { get; set; }
        public string PassCode { get; set; }

        public virtual ICollection<SalSalesDet> SalSalesDet { get; set; }
        public virtual ICollection<SalSalesTaxes> SalSalesTaxes { get; set; }
    }
}
