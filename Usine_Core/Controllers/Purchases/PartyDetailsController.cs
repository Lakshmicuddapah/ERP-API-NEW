using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;

using Usine_Core.Controllers.Admin;
using Usine_Core.Controllers.CRM;
using Microsoft.AspNetCore.Authorization;
using Usine_Core.others;
using System.Data.SqlClient;
 
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.Purchases
{
     public class PartyCompleteDetails
    {
        public int? RecordId { get; set; }
        public string PartyName { get; set; }
        public int? PartyGroup { get; set; }
        public string partyGroupName { get; set; }
       
        public string ContactPerson { get; set; }
        public string ContactDesignation { get; set; }
        public string ContactMobile { get; set; }
        public string ContactEmail { get; set; }
        public int? LeadTime { get; set; }
        public string PrefLanguage { get; set; }
        public int? CrDaysCheck { get; set; }
        public int? CrDay { get; set; }
        public int? CrAmtCheck { get; set; }
        public double? CrAmt { get; set; }
        public int? RestrictMode { get; set; }
       public long? Employee { get; set; }
        public string PartyType { get; set; }
        public int? DualType { get; set; }
        public int? EximCheck { get; set; }
        public string Pricelist { get; set; }
        public string Discountlist { get; set; }
        public string AirDestination { get; set; }
        public string SeaDestination { get; set; }
        public int? BankForSecurity { get; set; }
        public string PartyCode { get; set; }
        public string PartyUserName { get; set; }
        public string BranchId { get; set; }
        public string status { get; set; }
        public int? CustomerCode { get; set; }
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
        public int? MainCustomer { get; set; }

    }
    public class PartyDetailsTotal
    {
        public PartyDetails party { get; set; }
        public List<PartyAddresses> addresses { get; set; }
        public List<PartyDepartments> departments { get; set; }
        public int? traCheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class PartyOpeningDetailsTotal
    {
         public List<PartyTransactions> supports { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class PartyCompleteInfo
    {
        public List<PartyCompleteDetails> parties { get; set; }
        public List<PartyAddresses> addresses { get; set; }
        public List<PartyDepartments> departments { get; set; }
         public List<PurSupplierGroupsTree> partygroups { get; set; }
        public List<FinAccounts> banks { get; set; }
        public List<Purpurchasetypes> purtypes { get; set; }
    }
    public class PartyDetailsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/PartyDetails/GetCompleteInfoForParty")]
        public PartyCompleteInfo GetCompleteInfoForParty([FromBody] GeneralInformation inf)
        {
            PartyCompleteInfo tot = new PartyCompleteInfo();
            switch(inf.detail)
            {
                case "SUP":
                    PurSupplierGroupsController grp = new PurSupplierGroupsController();
                    tot.partygroups = grp.GetSupplierGroupsTreeView(inf.usr);
                    tot.parties = getPartySuppliersInformation(inf);
                     break;
                case "LEA":
                    CRMCustomerGroupsController grp1 = new CRMCustomerGroupsController();
                    tot.partygroups = grp1.GetCustomerGroupsTreeView(inf.usr);
                    tot.parties = getPartyCustomerInformation(inf);
                    break;
                case "AGE":
                    CRMCustomerGroupsController grp2 = new CRMCustomerGroupsController();
                    tot.partygroups = grp2.GetCustomerGroupsTreeView(inf.usr);
                    tot.parties = getPartyCustomerInformation(inf);
                    break;
                case "CUS":
                    CRMCustomerGroupsController grp3 = new CRMCustomerGroupsController();
                    tot.partygroups = grp3.GetCustomerGroupsTreeView(inf.usr);
                    tot.parties = getPartyCustomerInformation(inf);
                    
                    break;
            }
            tot.addresses = db.PartyAddresses.Where(a => a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ThenBy(c => c.Sno).ToList();
            tot.departments = db.PartyDepartments.Where(a => a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ThenBy(c => c.Sno).ToList();
            tot.purtypes = db.Purpurchasetypes.Where(a => a.CustomerCode == inf.usr.cCode).OrderBy(b => b.Purtype).ToList();
       

            tot.banks = db.FinAccounts.Where(a => a.CustomerCode == inf.usr.cCode && a.AcType == "BAN").OrderBy(b => b.Accname).ToList();
            return tot;
        }

        private List<PartyCompleteDetails> getPartySuppliersInformation(GeneralInformation inf)
        {
            return (from a in db.PartyDetails.Where(a => (a.PartyType == inf.detail) && a.CustomerCode == inf.usr.cCode)
                    join b in db.PurSupplierGroups.Where(a => a.CustomerCode == inf.usr.cCode)
                    on a.PartyGroup equals b.RecordId
                    select new PartyCompleteDetails
                    {
                        RecordId = a.RecordId,
                        PartyName = a.PartyName,
                        PartyGroup = a.PartyGroup,
                        partyGroupName = b.SGrp,
                        PrefLanguage = a .PrefLanguage ,
                        ContactPerson = a.ContactPerson,
                        ContactDesignation = a.ContactDesignation,
                        ContactMobile = a.ContactMobile,
                        ContactEmail = a.ContactEmail,
                        LeadTime = a.LeadTime,
                        CrAmtCheck = a.CrAmtCheck,
                        CrDaysCheck = a.CrDaysCheck,
                        CrDay = a.CrDay,
                        CrAmt = a.CrAmt,
                        RestrictMode = a.RestrictMode,

                        PartyType = a.PartyType,
                        DualType = a.DualType,
                        EximCheck = a.EximCheck,
                        AirDestination = a.AirDestination,
                        SeaDestination = a.SeaDestination,
                        BankForSecurity = a.BankForSecurity,
                        PartyCode = a.PartyCode,
                        status = a.Statu == 1 ? "Active" : "Inactive",
                        PartyUserName = a.PartyUserName,


                    }).OrderBy(b => b.PartyName).ToList();
        }
    
        private List<PartyCompleteDetails> getPartyCustomerInformation(GeneralInformation inf)
        {
            return (from a in db.PartyDetails.Where(a => (a.PartyType == inf.detail) && a.CustomerCode == inf.usr.cCode)
                    join b in db.SalcustomerGroups.Where(a => a.CustomerCode == inf.usr.cCode)
                    on a.PartyGroup equals b.RecordId
                    select new PartyCompleteDetails
                    {
                        RecordId = a.RecordId,
                        PartyName = a.PartyName,
                        PartyGroup = a.PartyGroup,
                        partyGroupName = b.SGrp,

                        ContactPerson = a.ContactPerson,
                        ContactDesignation = a.ContactDesignation,
                        ContactMobile = a.ContactMobile,
                        ContactEmail = a.ContactEmail,
                        PrefLanguage = a.PrefLanguage,
                        LeadTime = a.LeadTime,
                        CrAmtCheck = a.CrAmtCheck,
                        CrDaysCheck = a.CrDaysCheck,
                        CrDay = a.CrDay,
                        CrAmt = a.CrAmt,
                        RestrictMode = a.RestrictMode,
                        Employee=a.Employee,
                        PartyType = a.PartyType,
                        DualType = a.DualType,
                        EximCheck = a.EximCheck,
                        AirDestination = a.AirDestination,
                        SeaDestination = a.SeaDestination,
                        BankForSecurity = a.BankForSecurity,
                        PartyCode = a.PartyCode,
                        status = a.Statu == 1 ? "Active" : "Inactive",
                        PartyUserName = a.PartyUserName,
                        OrderReminder1 = a.OrderReminder1,
                        OrderReminder2 = a.OrderReminder2,
                        OrderReminder3= a.OrderReminder3,
                        PaymentReminder1 = a.PaymentReminder1,
                        PaymentReminder2= a.PaymentReminder2,
                        PaymentReminder3= a.PaymentReminder3,
                        DefaultPaymentMode = a.DefaultPaymentMode,
                        DefaultPurchaseorSaleType = a.DefaultPurchaseorSaleType,
                        KycAcbank=a.KycAcbank,
                        KycAcbranch=a.KycAcbranch,
                        KycAcholder=a.KycAcholder,
                        KycAcifsc  =a.KycAcifsc,
                        KycAcnumber=a.KycAcnumber,
                        Pricelist=a.Pricelist,
                        Discountlist=a.Discountlist,
                        MainCustomer=a.MainCustomer,
                        
                        

                    }).OrderBy(b => b.PartyName).ToList();
        }


            [HttpPost]
        [Authorize]
        [Route("api/PartyDetails/GetPartyDetails")]
        public PartyCompleteInfo GetPartyDetails([FromBody] GeneralInformation inf)
        {
            PartyCompleteInfo tot = new PartyCompleteInfo();
            tot.addresses = db.PartyAddresses.Where(a => a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ThenBy(c => c.Sno).ToList();
            tot.departments = db.PartyDepartments.Where(a => a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ThenBy(c => c.Sno).ToList();
            if (inf.detail =="SUP")
            {
                if (ac.screenCheck(inf.usr, 2, 1, 2, 0))
                {
                   tot.parties= getPartySuppliersInformation(inf);
                    return tot;
                    
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (ac.screenCheck(inf.usr, 7, 1, 4, 0))
                {
                    tot.parties = getPartyCustomerInformation(inf);
                    return tot;
                }
                else
                {
                    return null;
                }
            }
              
        }
        [HttpPost]
        [Authorize]
        [Route("api/PartyDetails/GetPartyDetail")]
        public PartyCompleteDetails GetPartyDetail([FromBody] GeneralInformation inf)
        {
            return (from a in db.PartyDetails.Where(a => a.PartyType == inf.detail && a.RecordId == inf.recordId && a.CustomerCode == inf.usr.cCode)
                    join b in db.PurSupplierGroups.Where(a => a.CustomerCode == inf.usr.cCode)
                    on a.PartyGroup equals b.RecordId
                    select new PartyCompleteDetails
                    {
                        RecordId = a.RecordId,
                        PartyName = a.PartyName,
                        PartyGroup = a.PartyGroup,
                        partyGroupName = b.SGrp,
                        
                        ContactPerson = a.ContactPerson,
                        ContactDesignation = a.ContactDesignation,
                        ContactMobile = a.ContactMobile,
                        ContactEmail = a.ContactEmail,
                        LeadTime = a.LeadTime,
                        CrAmtCheck = a.CrAmtCheck,
                        CrDaysCheck = a.CrDaysCheck,
                        CrDay = a.CrDay,
                        CrAmt = a.CrAmt,
                        RestrictMode = a.RestrictMode,
                        Employee=a.Employee,
                        PartyType = a.PartyType,
                        DualType = a.DualType,
                        EximCheck = a.EximCheck,
                        AirDestination = a.AirDestination,
                        SeaDestination = a.SeaDestination,
                        BankForSecurity = a.BankForSecurity,
                        PartyCode = a.PartyCode,
                        PartyUserName = a.PartyUserName,
                        




                    }).FirstOrDefault();
        }


        private Boolean duplicatePartyCheck(PartyDetailsTotal tot)
        {
            Boolean b = false;
            switch(tot.traCheck)
            {
                case 1:
                    var crecheck = db.PartyDetails.Where(a => a.PartyName == tot.party.PartyName && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(crecheck == null)
                    {
                        b = true;
                    }
                    break;
                case 2:
                    var updcheck = db.PartyDetails.Where(a => a.PartyName == tot.party.PartyName && a.RecordId != tot.party.RecordId && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (updcheck == null)
                    {
                        b = true;
                    }
                    break;
                case 3:
                    b = true;
                    break;
            }
            return b;
        }
        [HttpPost]
        [Authorize]
        [Route("api/PartyDetails/SetPartyDetail")]
        public TransactionResult SetPartyDetail([FromBody] PartyDetailsTotal tot)
        {
            string msg = "";
            int? recid = 0;
            AdminControl ac = new AdminControl();
            Boolean screencheck = false;
            switch (tot.party.PartyType)
            {
                case "SUP":
                    screencheck= ac.screenCheck(tot.usr, 2, 1, 2, (int)tot.traCheck);
                    break;
                case "CUS":
                    screencheck = ac.screenCheck(tot.usr, 7, 1, 4, (int)tot.traCheck);
                    break;
                case "LEA":
                     screencheck = ac.screenCheck(tot.usr, 7, 1, 5, (int)tot.traCheck);
                    break;
                case "AGE":
                    screencheck = ac.screenCheck(tot.usr, 7, 1, 7, (int)tot.traCheck);
                    break;
            }
             
            try
            {
                if (screencheck)
                {
                    if (duplicatePartyCheck(tot))
                    {
                        switch (tot.traCheck)
                        {
                            case 1:
                                using (var txn = db.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        PartyDetails partycre = new PartyDetails();
                                        partycre.PartyName = tot.party.PartyName;
                                        partycre.PartyGroup = tot.party.PartyGroup;

                                        partycre.ContactPerson = tot.party.ContactPerson;
                                        partycre.ContactDesignation = tot.party.ContactDesignation;
                                        partycre.ContactMobile = tot.party.ContactMobile;
                                        partycre.ContactEmail = tot.party.ContactEmail;
                                        partycre.LeadTime = tot.party.LeadTime;
                                        partycre.CrAmt = tot.party.CrAmt;
                                        partycre.CrAmtCheck = tot.party.CrAmtCheck;
                                        partycre.CrDay = tot.party.CrDay;
                                        partycre.CrDaysCheck = tot.party.CrDaysCheck;
                                        partycre.RestrictMode = tot.party.RestrictMode;

                                        partycre.PartyType = tot.party.PartyType;
                                        partycre.DualType = tot.party.DualType;
                                        partycre.EximCheck = tot.party.EximCheck;
                                        partycre.AirDestination = tot.party.AirDestination;
                                        partycre.SeaDestination = tot.party.SeaDestination;
                                        partycre.BankForSecurity = tot.party.BankForSecurity;
                                        partycre.PartyCode = tot.party.PartyCode;
                                        partycre.PartyUserName = tot.party.PartyUserName;
                                        partycre.BranchId = tot.usr.bCode;
                                        partycre.CustomerCode = tot.usr.cCode;
                                        partycre.Statu = tot.party.Statu;
                                        partycre.Pricelist = tot.party.Pricelist;
                                        partycre.Employee = tot.party.Employee;
                                        partycre.MainCustomer=tot.party.MainCustomer;
                                        partycre.PrefLanguage = tot.party.PrefLanguage;
                                        partycre.OrderReminder1 = tot.party.OrderReminder1;
                                        partycre.OrderReminder2 = tot.party.OrderReminder2;
                                        partycre.OrderReminder3 = tot.party.OrderReminder3;
                                        partycre.DefaultPurchaseorSaleType = tot.party.DefaultPurchaseorSaleType;
                                        partycre.PaymentReminder1 = tot.party.PaymentReminder1;
                                        partycre.PaymentReminder2 = tot.party.PaymentReminder2;
                                        partycre.PaymentReminder3 = tot.party.PaymentReminder3;
                                        partycre.DefaultPaymentMode = tot.party.DefaultPaymentMode;

                                        partycre.KycAcnumber = tot.party.KycAcnumber;
                                        partycre.KycAcbank = tot.party.KycAcbank;
                                        partycre.KycAcbranch = tot.party.KycAcbranch;
                                        partycre.KycAcholder = tot.party.KycAcholder;
                                        partycre.KycAcifsc = tot.party.KycAcifsc;

                                        partycre.Discountlist = tot.party.Discountlist;
                                        if (tot.party.PartyUserName != null)
                                        {
                                            if (tot.party.PartyUserName.Trim() != "")
                                            {
                                                partycre.PartyPwd = tot.party.PartyUserName.ToLower();
                                            }
                                        }
                                        db.PartyDetails.Add(partycre);
                                        db.SaveChanges();
                                        int sno = 1;
                                        foreach (var adr in tot.addresses)
                                        {
                                            adr.RecordId = partycre.RecordId;
                                            adr.Sno = sno;
                                            adr.BranchId = tot.usr.bCode;
                                            adr.CustomerCode = tot.usr.cCode;
                                            sno++;
                                        }
                                        db.PartyAddresses.AddRange(tot.addresses);
                                        sno = 1;
                                        foreach (var dep in tot.departments)
                                        {
                                            dep.RecordId = partycre.RecordId;
                                            dep.Sno = sno;
                                            dep.BranchId = tot.usr.bCode;
                                            dep.CustomerCode = tot.usr.cCode;
                                            sno++;
                                        }
                                        db.PartyDepartments.AddRange(tot.departments);


                                        db.SaveChanges();

                                        txn.Commit();
                                        recid = partycre.RecordId;
                                        msg = "OK";
                                    }
                                    catch (Exception ee)
                                    {
                                        msg = ee.Message;
                                        txn.Rollback();
                                    }
                                }
                               
                                break;
                            case 2:
                                var partyupd = db.PartyDetails.Where(a => a.RecordId == tot.party.RecordId && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                var olduser = partyupd.PartyUserName;
                                if (partyupd != null)
                                {
                                    partyupd.PartyName = tot.party.PartyName;
                                    partyupd.PartyGroup = tot.party.PartyGroup;
                                   
                                    partyupd.ContactPerson = tot.party.ContactPerson;
                                    partyupd.ContactDesignation = tot.party.ContactDesignation;
                                    partyupd.ContactMobile = tot.party.ContactMobile;
                                    partyupd.ContactEmail = tot.party.ContactEmail;
                                    partyupd.LeadTime = tot.party.LeadTime;
                                    partyupd.CrAmt = tot.party.CrAmt;
                                    partyupd.CrAmtCheck = tot.party.CrAmtCheck;
                                    partyupd.CrDay = tot.party.CrDay;
                                    partyupd.CrDaysCheck = tot.party.CrDaysCheck;
                                    partyupd.RestrictMode = tot.party.RestrictMode;
                                   partyupd.MainCustomer= tot.party.MainCustomer;
                                    partyupd.Statu = tot.party.Statu;
                                    partyupd.PartyType = tot.party.PartyType;
                                    partyupd.DualType = tot.party.DualType;
                                    partyupd.EximCheck = tot.party.EximCheck;
                                    partyupd.AirDestination = tot.party.AirDestination;
                                    partyupd.SeaDestination = tot.party.SeaDestination;
                                    partyupd.BankForSecurity = tot.party.BankForSecurity;
                                    partyupd.PartyCode = tot.party.PartyCode;
                                    partyupd.PartyUserName = tot.party.PartyUserName;
                                    partyupd.Pricelist = tot.party.Pricelist;
                                    partyupd.Discountlist = tot.party.Discountlist;
                                    partyupd.Employee = tot.party.Employee;
                                    partyupd.PrefLanguage = tot.party.PrefLanguage;
                                    partyupd.OrderReminder1 = tot.party.OrderReminder1;
                                    partyupd.OrderReminder2 = tot.party.OrderReminder2;
                                    partyupd.OrderReminder3 = tot.party.OrderReminder3;
                                    partyupd.DefaultPurchaseorSaleType = tot.party.DefaultPurchaseorSaleType;
                                    partyupd.PaymentReminder1 = tot.party.PaymentReminder1;
                                    partyupd.PaymentReminder2 = tot.party.PaymentReminder2;
                                    partyupd.PaymentReminder3 = tot.party.PaymentReminder3;
                                    partyupd.DefaultPaymentMode = tot.party.DefaultPaymentMode;

                                    partyupd.KycAcnumber = tot.party.KycAcnumber;
                                    partyupd.KycAcbank = tot.party.KycAcbank;
                                    partyupd.KycAcbranch = tot.party.KycAcbranch;
                                    partyupd.KycAcholder = tot.party.KycAcholder;
                                    partyupd.KycAcifsc = tot.party.KycAcifsc;



                                    if (olduser == null || olduser.Trim() == "")
                                    {
                                        if (tot.party.PartyUserName != null)
                                        {
                                            if (tot.party.PartyUserName.Trim() != "")
                                            {
                                                partyupd.PartyPwd = tot.party.PartyUserName.ToLower();
                                            }
                                        }
                                    }
                                     var addrupds = db.PartyAddresses.Where(a => a.RecordId == tot.party.RecordId && a.CustomerCode == tot.usr.cCode).ToList();
                                    db.PartyAddresses.RemoveRange(addrupds);
                                    var deptupds = db.PartyDepartments.Where(a => a.RecordId == tot.party.RecordId && a.CustomerCode == tot.usr.cCode).ToList();
                                    db.PartyDepartments.RemoveRange(deptupds);

                                    int? sno = db.PartyAddresses.Where(a => a.RecordId == tot.party.RecordId && a.CustomerCode == tot.usr.cCode).Max(b => b.Sno);
                                    if (sno == null)
                                    {
                                        sno = 0;
                                    }
                                    sno++;
                                    foreach (var adr in tot.addresses)
                                    {
                                        adr.RecordId = partyupd.RecordId;
                                        adr.Sno = sno;
                                        adr.BranchId = tot.usr.bCode;
                                        adr.CustomerCode = tot.usr.cCode;
                                        sno++;
                                    }
                                    db.PartyAddresses.AddRange(tot.addresses);
                                    sno = db.PartyDepartments.Where(a => a.RecordId == tot.party.RecordId && a.CustomerCode == tot.usr.cCode).Max(b => b.Sno);
                                    if (sno == null)
                                    {
                                        sno = 0;
                                    }
                                    sno++;
                                    foreach (var dep in tot.departments)
                                    {
                                        dep.RecordId = partyupd.RecordId;
                                        dep.Sno = sno;
                                        dep.BranchId = tot.usr.bCode;
                                        dep.CustomerCode = tot.usr.cCode;
                                        sno++;
                                    }

                                    db.PartyDepartments.AddRange(tot.departments);




                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "No record found";
                                }
                                break;
                            case 3:
                                try
                                {
                                    var partydel = db.PartyDetails.Where(a => a.RecordId == tot.party.RecordId && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                    var addrdels = db.PartyAddresses.Where(a => a.RecordId == tot.party.RecordId && a.CustomerCode == tot.usr.cCode).ToList();

                                    var deptdels = db.PartyDepartments.Where(a => a.RecordId == tot.party.RecordId && a.CustomerCode == tot.usr.cCode).ToList();



                                    if (partydel != null)
                                    {

                                        db.PartyAddresses.RemoveRange(addrdels);
                                        db.PartyDepartments.RemoveRange(deptdels);
                                        db.PartyDetails.Remove(partydel);
                                        db.SaveChanges();
                                        msg = "OK";
                                    }
                                    else
                                    {
                                        msg = "No record found";
                                    }
                                }
                                catch
                                {
                                    msg = "This Supplier is already in use";
                                }

                                break;
                        }
                    }

                    else
                    {
                        if (tot.traCheck == 3)
                        {
                            msg = "This " + (tot.party.PartyType == "SUP" ? "Supplier" : "Customer") + " is in use";
                        }
                        else
                        {
                            msg = "This name is already existed";
                        }
                    }
                }
                else
                {
                    msg = "You are not authorised for this transaction";
                }
            }
            catch(Exception ex)
            {
                msg = ex.Message;
            }

            TransactionResult result = new TransactionResult();
            result.recordId = recid;
            result.result = msg;
            return result;
        }

        [HttpPost]
        [Authorize]
        [Route("api/PartyDetails/GetPartiesOpeningBalances")]
        public List<PartyTransactions> GetPartiesOpeningBalances([FromBody]GeneralInformation inf)
        {
            DateTime dat = ac.getFinancialStart(DateTime.Now, inf.usr);
            string tratype = inf.detail == "SUP" ? "OPS" : "OPC";
            if (inf.detail == "SUP")
            {
                return (from a in (from p in db.PartyDetails.Where(a => a.Statu==1 && a.CustomerCode == inf.usr.cCode && a.PartyType == inf.detail)
                                   join q in db.PurSupplierGroups.Where(a => a.CustomerCode == inf.usr.cCode)
                                   on p.PartyGroup equals q.RecordId
                                   select new
                                   {
                                       RecordId = p.RecordId,
                                       p.PartyName,
                                        p.PartyGroup,
                                       q.SGrp
                                   })
                        join b in db.PartyTransactions.Where(a => a.Dat == dat && a.TransactionType == tratype && a.CustomerCode == inf.usr.cCode)
                               on a.RecordId equals b.PartyId into openings
                        from ope in openings.DefaultIfEmpty()
                        select new PartyTransactions
                        {
                            PartyId = a.RecordId,
                            PartyName = a.PartyName,
                            Descriptio = a.SGrp,
                            PendingAmount = ope.PendingAmount == null ? 0 : ope.PendingAmount

                        }
                             ).OrderBy(b => b.PartyName).ToList();
            }
            else
            {
                return (from a in (from p in db.PartyDetails.Where(a => a.Statu ==1 && a.CustomerCode == inf.usr.cCode && a.PartyType == inf.detail)
                                   join q in db.SalcustomerGroups.Where(a => a.CustomerCode == inf.usr.cCode)
                                   on p.PartyGroup equals q.RecordId
                                   select new
                                   {
                                       RecordId = p.RecordId,
                                       p.PartyName,
                                        p.PartyGroup,
                                       q.SGrp
                                   })
                        join b in db.PartyTransactions.Where(a => a.Dat == dat && a.TransactionType == tratype && a.CustomerCode == inf.usr.cCode)
                               on a.RecordId equals b.PartyId into openings
                        from ope in openings.DefaultIfEmpty()
                        select new PartyTransactions
                        {
                            PartyId = a.RecordId,
                            PartyName = a.PartyName,
                            Descriptio = a.SGrp,
                            PendingAmount = ope.PendingAmount == null ? 0 : ope.PendingAmount

                        }
                           ).OrderBy(b => b.PartyName).ToList();
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/PartyDetails/SetPartiesOpeningBalances")]
        public PartyOpeningDetailsTotal SetPartiesOpeningBalances([FromBody]PartyOpeningDetailsTotal tot)
        {
            string msg = "";
            DateTime dat = ac.getFinancialStart(DateTime.Now, tot.usr);
             
            try
            {
                if (ac.screenCheck(tot.usr, 2, 1, 3, 0))
                {
                    string partytype = "";
                    if (tot.supports.Count() > 0)
                    {
                        partytype = tot.supports[0].TransactionType;
                    }
                  //  Boolean b = partytype == "SUP" ? ac.screenCheck(tot.usr, 2, 1, 10, 0) : ac.screenCheck(tot.usr, 7, 1, 4, 0);
                    var openings = db.PartyTransactions.Where(a => a.TransactionType == partytype && a.Dat == dat && a.CustomerCode == tot.usr.cCode).ToList();
                    if (openings.Count > 0)
                    {
                        db.PartyTransactions.RemoveRange(openings);
                    }
                    foreach (PartyTransactions ope in tot.supports)
                    {
                        ope.CustomerCode = tot.usr.cCode;
                        ope.BranchId = tot.usr.bCode;
                        ope.TransactionId = -1;
                        ope.Username = tot.usr.uCode;
                        ope.Dat = dat;
                    }
                    db.PartyTransactions.AddRange(tot.supports);
                    db.SaveChanges();
                    msg = "OK";
                }
                else
                {
                    msg = "You are not authorised for this transaction";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            tot.result = msg;
            return tot;
        }
        [HttpPost]
        [Authorize]
        [Route("api/PartyDetails/GetSupplierPendingBalancesDetails")]
        public List<PartyTransactions> GetSupplierPendingBalancesDetails([FromBody] GeneralInformation inf)
        {
            string quer = "";
            quer = quer + " select *,pendingamount-returnamt-crnote-paid balance from";
            quer = quer + " (select a.transactionId, a.billno, a.dat, a.transactionamt, a.pendingamount,";
            quer = quer + " case when returnamt is null then 0 else returnamt end returnamt,";
            quer = quer + " case when crnote is null then 0 else crnote end crnote,";
            quer = quer + " case when paid is null then 0 else paid end paid from";
            quer = quer + " (select transactionId,'OPB' billno,dat,transactionamt,pendingAmount from partyTransactions where partyid = " + inf.recordId + " and transactiontype like 'OPS' and customerCode = " + inf.usr.cCode  + "";
            quer = quer + " union all select a.transactionId,b.seq billno, a.dat,a.transactionamt,a.pendingAmount from";
            quer = quer + " (select transactionId, dat, transactionamt, pendingAmount from partyTransactions where partyid= " + inf.recordId + " and transactiontype = 'PUR' and customerCode = " + inf.usr.cCode + ")a,";
            quer = quer + " (select * from purPurchasesUni where customerCode = " + inf.usr.cCode + ")b where a.transactionId = b.recordId )a left outer join";
            quer = quer + " (select onTraId transactionId, sum(returnamount) returnamt, sum(creditnote) crnote, sum(paymentAmount) paid from partyTransactions where customerCode = " + inf.usr.cCode + " group by onTraId)b on";
            quer = quer + " a.transactionId = b.transactionId  )x where pendingamount - returnamt - crnote - paid > 0 order by transactionid";

            SqlCommand dc = new SqlCommand();
            DataBaseContext g = new DataBaseContext();
            General gg = new General();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            List<PartyTransactions> lst = new List<PartyTransactions>();
            while (dr.Read())
            {
                lst.Add(new PartyTransactions
                {
                    TransactionId=gg.valInt(dr[0].ToString()),
                    BranchId=dr[1].ToString(),
                    Dat=DateTime.Parse(dr[2].ToString()),
                    TransactionAmt=gg.valNum(dr[3].ToString()),
                    PendingAmount=gg.valNum(dr[4].ToString()),
                    ReturnAmount=gg.valNum(dr[5].ToString()),
                    CreditNote=gg.valNum(dr[6].ToString()),
                    PaymentAmount=gg.valNum(dr[7].ToString()),
                    Username=dr[8].ToString()
                });
            }
            dr.Close();
            g.db.Close();

            return lst;
            
        }

        [HttpPost]
        [Authorize]
        [Route("api/PartyDetails/GetCustomerPendingBalancesDetails")]
        public List<PartyTransactions> GetCustomerPendingBalancesDetails([FromBody] GeneralInformation inf)
        {
            string quer = "";
            quer = quer + " select *,pendingamount-returnamt-crnote-paid balance from";
            quer = quer + " (select a.transactionId, a.billno, a.dat, a.transactionamt, a.pendingamount,";
            quer = quer + " case when returnamt is null then 0 else returnamt end returnamt,";
            quer = quer + " case when crnote is null then 0 else crnote end crnote,";
            quer = quer + " case when paid is null then 0 else paid end paid from";
            quer = quer + " (select transactionId,'OPB' billno,dat,transactionamt,pendingAmount from partyTransactions where partyid = " + inf.recordId + " and transactiontype like 'OPC' and customerCode = " + inf.usr.cCode + "";
            quer = quer + " union all select a.transactionId,b.seq billno, a.dat,a.transactionamt,a.pendingAmount from";
            quer = quer + " (select transactionId, dat, transactionamt, pendingAmount from partyTransactions where partyid= " + inf.recordId + " and transactiontype = 'SAL' and customerCode = " + inf.usr.cCode + ")a,";
            quer = quer + " (select * from salsalesuni where customerCode = " + inf.usr.cCode + ")b where a.transactionId = b.recordId )a left outer join";
            quer = quer + " (select onTraId transactionId, sum(returnamount) returnamt, sum(creditnote) crnote, sum(paymentAmount) paid from partyTransactions where customerCode = " + inf.usr.cCode + " group by onTraId)b on";
            quer = quer + " a.transactionId = b.transactionId  )x where pendingamount - returnamt - crnote - paid > 0 order by transactionid";

            SqlCommand dc = new SqlCommand();
            DataBaseContext g = new DataBaseContext();
            General gg = new General();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            List<PartyTransactions> lst = new List<PartyTransactions>();
            while (dr.Read())
            {
                lst.Add(new PartyTransactions
                {
                    TransactionId = gg.valInt(dr[0].ToString()),
                    BranchId = dr[1].ToString(),
                    Dat = DateTime.Parse(dr[2].ToString()),
                    TransactionAmt = gg.valNum(dr[3].ToString()),
                    PendingAmount = gg.valNum(dr[4].ToString()),
                    ReturnAmount = gg.valNum(dr[5].ToString()),
                    CreditNote = gg.valNum(dr[6].ToString()),
                    PaymentAmount = gg.valNum(dr[7].ToString()),
                    Username = dr[8].ToString()
                });
            }
            dr.Close();
            g.db.Close();

            return lst;

        }
    }
}
