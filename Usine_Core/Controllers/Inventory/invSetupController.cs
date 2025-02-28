using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace Usine_Core.Controllers.Inventory
{
    
    public class invSetupController : Controller
    {
        UsineContext db = new UsineContext();

        [HttpPost]
        [Authorize]
        [Route("api/invSetup/invCostingInfo")]
        public InvSetup invCostingInfo([FromBody] UserInfo usr)
        {
            AdminControl ac = new AdminControl();
            if (ac.screenCheck(usr, 3, 12, 1, 0))
            {
                try
                {
                    return db.InvSetup.Where(a => a.CustomerCode == usr.cCode && a.SetupCode == "inv_iss").FirstOrDefault();

                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/invSetup/setInvCostingInfo")]
        public GeneralInformation SetInvCostingInfo([FromBody] GeneralInformation inf)
        {
            AdminControl ac = new AdminControl();
            string msg = "";
            if (ac.screenCheck(inf.usr, 3, 12, 1, 0))
            {

                try
                {
                    var det = db.InvSetup.Where(a => a.SetupCode == "inv_iss" && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if(det != null)
                    {
                        det.SetupValue = inf.recordId.ToString();
                        det.Dat = ac.getPresentDateTime();
                    }
                    else
                    {
                        InvSetup newdet = new InvSetup();
                        newdet.SetupValue = inf.recordId.ToString();
                        newdet.Dat = ac.getPresentDateTime();
                        newdet.SetupCode = "inv_iss";
                        newdet.SetupDescription = "1 for -ve 2 for FIFO 3 for LIFO 4 for Batch 5 for GIN";
                        newdet.BranchId = inf.usr.bCode;
                        newdet.CustomerCode = inf.usr.cCode;
                        db.InvSetup.Add(newdet);


                    }
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
                msg = "You are not authorised to add settings";
            }
            inf.detail = msg;
            return inf;
        }

        [HttpPost]
        [Authorize]
        [Route("api/invSetup/invTransVerification")]
        public InvMaterialManagement invTransVerification([FromBody] UserInfo usr)
        {
            AdminControl ac = new AdminControl();
            try
            {
                return db.InvMaterialManagement.Where(a => a.CustomerCode == usr.cCode ).FirstOrDefault();

            }
            catch
            {
                return null;
            }
        }

    }
}
