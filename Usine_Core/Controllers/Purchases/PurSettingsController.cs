using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;

namespace Usine_Core.Controllers.Purchases
{
     public class PurSetupTotal
    {
        public List<PurSetup> sets { get; set; }
        public UserInfo usr { get; set; }
    }
    public class PurEmailTotal
    {
        public List<PurEmails> mails { get; set; }
        public UserInfo usr { get; set; }
    }
    public class PurSettingsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        [HttpPost]
        [Authorize]
        [Route("api/PurSettings/GetPurSetupDetails")]
        public List<PurSetup> GetPurSetupDetails([FromBody] UserInfo usr)
        {
            return db.PurSetup.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
        }
        [HttpPost]
        [Authorize]
        [Route("api/PurSettings/SetPurSetupDetails")]
        public TransactionResult SetPurSetupDetails([FromBody] PurSetupTotal tot)
        {
            string msg = "";
          if(ac.screenCheck(tot.usr,2,8,3,0))
            {
                try
                {
                    var lst = db.PurSetup.Where(a => a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                    if(lst.Count() > 0)
                    {
                        db.PurSetup.RemoveRange(lst);
                    }
                    foreach(var l in tot.sets)
                    {
                        l.BranchId = tot.usr.bCode;
                        l.CustomerCode = tot.usr.cCode;
                    }
                    db.PurSetup.AddRange(tot.sets);
                    db.SaveChanges();
                    msg = "OK";
                }
                catch(Exception ee)
                {
                    msg = ee.Message;
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

        [HttpPost]
        [Authorize]
        [Route("api/PurSettings/GetPurMailSettings")]
        public List<PurEmails> GetPurMailSettings([FromBody] UserInfo usr)
        {
            return db.PurEmails.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurSettings/SetPurMailSettings")]
        public TransactionResult SetPurMailSettings([FromBody] PurEmailTotal tot)
        {
            string msg = "";
            if(ac.screenCheck(tot.usr,2,8,5,0))
            {
                try
                {
                    var lst = db.PurEmails.Where(a => a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                    if (lst.Count() > 0)
                    {
                        db.PurEmails.RemoveRange(lst);
                    }
                    foreach (var l in tot.mails)
                    {
                        l.BranchId = tot.usr.bCode;
                        l.CustomerCode = tot.usr.cCode;
                    }
                    db.PurEmails.AddRange(tot.mails);
                    db.SaveChanges();

                    msg = "OK";
                }
                catch(Exception ee)
                {
                    msg = ee.Message;
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
