using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;

namespace Usine_Core.Controllers.crm
{
    public class CRMAccountsAssign
    {
        public string transcode { get; set; }
        public string transdescription { get; set; }
        public int? accountId { get; set; }
        public string module { get; set; }

    }

    public class CRMAdvancesController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        private readonly IHostingEnvironment ho;
        public CRMAdvancesController(IHostingEnvironment hostingEnvironment)
        {
            ho = hostingEnvironment;
        }
        [HttpPost]
        [Authorize]
        [Route("api/CRMAdvances/GetSaleCRMAccounts")]
        public List<CRMAccountsAssign> GetSaleCRMAccounts([FromBody] UserInfo usr)
        {
            List<CRMAccountsAssign> lst = new List<CRMAccountsAssign>();

            lst.Add(new CRMAccountsAssign
            {
                transcode = "SAL_REC",
                transdescription = "Advance received from customers",
                accountId = 0,
                module = "SAL"
            });

             
            lst.Add(new CRMAccountsAssign
            {
                transcode = "SAL_SRT",
                transdescription = "Sale Returns",
                accountId = 0,
                module = "SAL"
            });
            lst.Add(new CRMAccountsAssign
            {
                transcode = "SAL_DNT",
                transdescription = "General Debit Note",
                accountId = 0,
                module = "SAL"
            });

           

            lst.Add(new CRMAccountsAssign
            {
                transcode = "SAL_REB",
                transdescription = "Receipt Rebates",
                accountId = 0,
                module = "SAL"
            });

            lst.Add(new CRMAccountsAssign
            {
                transcode = "SAL_OTH",
                transdescription = "Other Deductions in receipts",
                accountId = 0,
                module = "SAL"
            });


            var existedlst = (from a in db.AccountsAssign.Where(a => a.CustomerCode == usr.cCode && a.BranchId == usr.bCode && a.Module == "SAL")
                              select new CRMAccountsAssign
                              {
                                  transcode = a.Transcode,
                                  accountId = a.Account,
                                  module = "SAL"
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
                            select new CRMAccountsAssign
                            {
                                transcode = a.transcode,
                                transdescription = a.transdescription,
                                accountId = subdet == null ? 0 : subdet.accountId,
                                module = "SAL"
                            }).ToList();
                }
            }
            else
            {
                return lst;
            }




        }



    }
}
