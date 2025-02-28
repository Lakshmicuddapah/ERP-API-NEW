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
    public class InvLossesTotal
    {
        public InvLosses loss { get; set; }
        public int traCheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    
    public class invLossesController : Controller
    {
        UsineContext db = new UsineContext();

        [HttpPost]
        [Authorize]
        [Route("api/invlosses/getInvLosses")]
        public List<InvLosses> getInvLosses([FromBody]UserInfo usr)
        {
            return db.InvLosses.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.LossName).ToList();
        }


        [HttpPost]
        [Authorize]
        [Route("api/invlosses/setInvLoss")]
        public InvLossesTotal setInvLoss([FromBody]InvLossesTotal tot)
        {
            String msg = "";
            AdminControl ac = new AdminControl();

            try
            {
                if (ac.screenCheck(tot.usr, 3, 1, 7, (int)tot.traCheck))
                    {
                    if (dupCheck(tot))
                    {
                        switch (tot.traCheck)
                        {
                            case 1:
                                InvLosses los = new InvLosses();
                                los.LossName = tot.loss.LossName;
                                los.Allowableper = tot.loss.Allowableper;
                                los.BranchId = tot.usr.bCode;
                                los.CustomerCode = tot.usr.cCode;
                                db.InvLosses.Add(los);
                                db.SaveChanges();
                                msg = "OK";
                                break;
                            case 2:
                                var losdet = db.InvLosses.Where(a => a.RecordId == tot.loss.RecordId && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (losdet != null)
                                {
                                    losdet.LossName = tot.loss.LossName;
                                    losdet.Allowableper = tot.loss.Allowableper;
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "No record found";
                                }
                                break;
                            case 3:
                                var losdet1 = db.InvLosses.Where(a => a.RecordId == tot.loss.RecordId && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (losdet1 != null)
                                {
                                    db.InvLosses.Remove(losdet1);
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
                        if (tot.traCheck == 3)
                        {
                            msg = "This loss name is already in use deletion is not possible";
                        }
                        else
                        {
                            msg = "This loss name is already existed";
                        }
                    }
                }
                else
                {
                    msg = "You are not authorised for this transaction";
                }
            }
            catch(Exception ee)
            {
                msg = ee.Message;
            }

            tot.result = msg;
            return tot;
        }


        private Boolean dupCheck(InvLossesTotal tot)
        {
            Boolean b = false;
            switch(tot.traCheck)
            {
                case 1:
                    var x = db.InvLosses.Where(a => a.LossName == tot.loss.LossName && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(x==null)
                    {
                        b = true;
                    }
                    break;
                case 2:
                    var y = db.InvLosses.Where(a => a.LossName == tot.loss.LossName && a.RecordId != tot.loss.RecordId  && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (y == null)
                    {
                        b = true;
                    }
                    break;
                case 3:
                    var z = db.InvMaterialManagement.Where(a => a.Department == tot.loss.RecordId && a.TransactionType == 17 && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(z==null)
                    {
                        b = true;
                    }
                    break;
            }


            return b;
        }
    }
}
