using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;

using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;
using Usine_Core.others;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.Purchases
{
    public class PurchaseTypesComplete
    {
        public Purpurchasetypes ptype { get; set; }
        public int? traCheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class PurAccountsAssign
    {
        public string transcode { get; set; }
        public string transdescription { get; set; }
        public int? accountId { get; set; }
        public string module { get; set; }

    }
    public class PurAccountsAssinTotal
    {
        public List<AccountsAssign> list { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    
    public class PurchaseTypesController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/PurchaseTypes/GetPurchaseTypes")]
        public List<Purpurchasetypes> GetPurchaseTypes([FromBody] UserInfo usr)
        {
            return db.Purpurchasetypes.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.Slno).ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurchaseTypes/SetPurchaseType")]
        public PurchaseTypesComplete SetPurchaseType([FromBody] PurchaseTypesComplete tot)
        {
            string msg = "";
            try
            {
                if(ac.screenCheck(tot.usr,2,8,1,0))
                {
                    if(duplicateCheck(tot))
                    {
                        switch(tot.traCheck)
                        {
                            case 1:
                                Purpurchasetypes ptycre = new Purpurchasetypes();
                                ptycre.Purtype = tot.ptype.Purtype;
                                ptycre.ImportType= tot.ptype.ImportType;
                                ptycre.CustomerCode = tot.usr.cCode;
                                db.Purpurchasetypes.Add(ptycre);
                                db.SaveChanges();
                                msg = "OK";
                                break;
                            case 2:
                                var ptyupd = db.Purpurchasetypes.Where(a => a.Purtype == tot.ptype.Purtype && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if(ptyupd != null)
                                {
                                    ptyupd.ImportType = tot.ptype.ImportType;
                                    db.SaveChanges();
                                    msg="OK";
                                }
                                else
                                {
                                    msg = "No record found";
                                }
                                break;
                            case 3:
                                var ptydel = db.Purpurchasetypes.Where(a => a.Purtype == tot.ptype.Purtype && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (ptydel != null)
                                {
                                    db.Purpurchasetypes.Remove(ptydel);
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "No record found";
                                }
                                break;
                        }
                    }
                    else
                    {
                        msg = tot.traCheck == 3 ? "This type is already in use deletion not possible" : "This type is already existed";
                    }
                }
                else
                {
                    msg = "You are not authorised";
                }
            }
            catch(Exception ex)
            {
                msg = ex.Message;
            }
            tot.result = msg;
            return tot;
        }
        private bool duplicateCheck(PurchaseTypesComplete tot)
        {
            bool b = false;
            switch(tot.traCheck)
            {
                case 1:
                    var ptycre = db.Purpurchasetypes.Where(a => a.Purtype == tot.ptype.Purtype && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(ptycre == null)
                    {
                        b = true;
                    }
                    break;
                case 2:
                    b = true;
                    break;
                case 3:
                    b = true;
                    break;
            }


            return b;
        }



        [HttpPost]
        [Authorize]
        [Route("api/PurchaseTypes/GetPurchaseAccounts")]
        public List<PurAccountsAssign> GetPurchaseAccounts([FromBody] UserInfo usr)
        {
            List<PurAccountsAssign> lst = new List<PurAccountsAssign>();
        
            lst.Add(new PurAccountsAssign
            {
                transcode = "PUR_VOU",
                transdescription = "Advance paid suppliers",
                accountId = 0,
                module = "PUR"
            });


           /* var invgroups = db.InvGroups.Where(a => a.CustomerCode == usr.cCode).ToList();
            foreach(var grp in invgroups)
            {
                lst.Add(new PurAccountsAssign
                {
                    transcode = "PUR_GRP_" + grp.RecordId.ToString(),
                    transdescription = grp.SGrp + " Purchases",
                    accountId = 0,
                    module = "PUR"
                });
            }*/


            lst.Add(new PurAccountsAssign
            {
                transcode = "PUR_PUR",
                transdescription = "Default Purchases",
                accountId = 0,
                module = "PUR"
            });
            lst.Add(new PurAccountsAssign
            {
                transcode = "PUR_DIS",
                transdescription = "General Purchase Discounts",
                accountId = 0,
                module = "PUR"
            });
            lst.Add(new PurAccountsAssign
            {
                transcode = "PUR_OTH",
                transdescription = "General Purchase Other Amounts",
                accountId = 0,
                module = "PUR"
            });
            lst.Add(new PurAccountsAssign
            {
                transcode = "PUR_PRT",
                transdescription = "Purchase Returns",
                accountId = 0,
                module = "PUR"
            });
            lst.Add(new PurAccountsAssign
            {
                transcode = "PUR_CNT",
                transdescription = "General Credit Note",
                accountId = 0,
                module = "PUR"
            });

            /*   var supgrps = db.PurSupplierGroups.Where(a => a.CustomerCode == usr.cCode).ToList();
               foreach(var grp in supgrps)
               {
                   lst.Add(new PurAccountsAssign
                   {
                       transcode = "PUR_SUP" + grp.RecordId,
                       transdescription =  grp.SGrp + " Credit Purchases",
                       accountId = 0,
                       module = "PUR"
                   });
               }*/

            lst.Add(new PurAccountsAssign
            {
                transcode = "PAY_REB",
                transdescription = "Payment Rebates",
                accountId = 0,
                module = "PUR"
            });

            lst.Add(new PurAccountsAssign
            {
                transcode = "PAY_OTH",
                transdescription = "Other Deductions in payments",
                accountId = 0,
                module = "PUR"
            });

            lst.Add(new PurAccountsAssign
            {
                transcode = "PUR_CRP",
                transdescription = "Default Credit Purchases",
                accountId = 0,
                module = "PUR"
            });

            var taxes = (from a in db.AdmTaxes.Where(a => a.CustomerCode == usr.cCode)
                         select new PurAccountsAssign
                         {
                             transcode="PUR_" + a.TaxCode+"@" + a.TaxPer.ToString(),
                             transdescription= "Tax on " + a.TaxCode + " @ " + a.TaxPer.ToString() +"%",
                             accountId=0,
                             module ="PUR"
                         }
                       ).ToList();
            if(taxes.Count > 0)
            lst.AddRange(taxes);


            var existedlst = (from a in db.AccountsAssign.Where(a => a.CustomerCode == usr.cCode && a.BranchId == usr.bCode  && a.Module=="PUR")
                              select new PurAccountsAssign
                              {
                                  transcode=a.Transcode,
                                  accountId=a.Account,
                                  module ="PUR"
                              }).ToList();
            if (existedlst != null)
            {
                if (existedlst.Count == 0)
                {
                    return lst;
                }
                else
                {
                    return (from a in lst
                            join b in existedlst on a.transcode equals b.transcode
                            into gj
                            from subdet in gj.DefaultIfEmpty()
                            select new PurAccountsAssign
                            {
                                transcode = a.transcode,
                                transdescription = a.transdescription,
                                accountId = subdet == null ? 0 : subdet.accountId,
                                module = "PUR"
                            }).ToList();
                }
            }
            else
            {
                return lst;
            }

        }

        [HttpPost]
        [Authorize]
        [Route("api/PurchaseTypes/SetPurchaseAccounts")]
        public TransactionResult SetPurchaseAccounts(PurAccountsAssinTotal tot)
        {
            string msg = "";
            if (ac.screenCheck(tot.usr, 2, 8, 4, 0))
            {
                try
                {
                    var lst = db.AccountsAssign.Where(a => a.Module == "PUR" && a.CustomerCode == tot.usr.cCode && a.BranchId == tot.usr.bCode).ToList();
                    if (lst.Count > 0)
                    {
                        db.AccountsAssign.RemoveRange(lst);
                    }

                    foreach (AccountsAssign acc in tot.list)
                    {
                        acc.BranchId = tot.usr.bCode;
                        acc.CustomerCode = tot.usr.cCode;
                    }
                    db.AccountsAssign.AddRange(tot.list);
                    db.SaveChanges();
                    msg = "OK";

                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }
            else
            {
                msg = "You are not authorised for this transaction";
            }
            TransactionResult res = new TransactionResult();
            res.result = msg;
            return res;
        }
    }
}
