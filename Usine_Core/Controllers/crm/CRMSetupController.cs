using System;
using System.Collections.Generic;
 
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;

namespace Usine_Core.Controllers.CRM
{ 
    public class CRMSetUpTotal
    {
        public List<CrmSetup> list { get; set; }
        public UserInfo usr { get; set; }
    }
    public class CRMSetupController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        [HttpPost]
        [Authorize]
        [Route("api/CRMSetup/GetCRMSetupDetails")]
        public List<CrmSetup> GetCRMSetupDetails([FromBody] UserInfo usr)
        {
           
            try
            {
                  return db.CrmSetup.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMSetup/SetCRMSetupDetails")]
        public TransactionResult SetCRMSetupDetails([FromBody] CRMSetUpTotal tot)
        {
            string msg = "";
            try
            {
                if(ac.screenCheck(tot.usr,7,10,2,0))
                {
                     var details = db.CrmSetup.Where(a => a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                    db.CrmSetup.RemoveRange(details);
                    foreach(var det in tot.list)
                    {
                        det.BranchId = tot.usr.bCode;
                        det.CustomerCode = tot.usr.cCode;
                    }
                    db.CrmSetup.AddRange(tot.list);
                    db.SaveChanges();
                    msg = "OK";
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
            result.result = msg;
            return result;
        }
    }
}
