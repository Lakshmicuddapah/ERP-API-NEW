using System;
using System.Collections.Generic;

namespace Usine_Core
{
    public partial class PartyDetails
    {
        public int? RecordId { get; set; }
        public string PartyName { get; set; }
        public int? PartyGroup { get; set; }
      
        public string ContactPerson { get; set; }
        public string ContactDesignation { get; set; }
        public string ContactMobile { get; set; }
        public string ContactEmail { get; set; }
        public int? LeadTime { get; set; }
        public int? CrDaysCheck { get; set; }
        public int? CrDay { get; set; }
        public int? CrAmtCheck { get; set; }
        public double? CrAmt { get; set; }
        public int? RestrictMode { get; set; }
       
        public string PartyType { get; set; }
        public int? DualType { get; set; }
        public int? EximCheck { get; set; }
        public string AirDestination { get; set; }
        public string SeaDestination { get; set; }
        public int? BankForSecurity { get; set; }
        public string PartyCode { get; set; }
        public string PartyUserName { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
        public int? Statu { get; set; }
        public int? MainCustomer { get; set; }
        public string Pricelist { get; set; }
        public string Discountlist { get; set; }
        public string PartyPwd { get; set; }


        public string PrefLanguage { get; set; }
        public int? OrderReminder1 { get; set; }
        public int? OrderReminder2 { get; set; }
        public int? OrderReminder3 { get; set; }
        public string DefaultPurchaseorSaleType { get; set; }
        public int? PaymentReminder1 { get; set; }
        public int? PaymentReminder2 { get; set; }
        public int? PaymentReminder3 { get; set; }
        public string DefaultPaymentMode { get; set; }
        public string KycAcnumber { get; set; }
        public string KycAcholder { get; set; }
        public string KycAcbank { get; set; }
        public string KycAcbranch { get; set; }
        public string KycAcifsc { get; set; }
        public long? Employee { get; set; }

    }
}
