using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Usine_Core.Controllers.Inventory
{

    public class invUMTotal
    {
        public InvUm um { get; set; }
        public UserInfo usr { get; set; }
        public int traCheck { get; set; }
        public string result { get; set; }
    }
     
    
    public class InvUnitsController : Controller
    {

        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Route("api/inventory/GetInventoryUnits")]
        public IQueryable<InvUm> GetInvUnits([FromBody] UserInfo usr)
        {
            return db.InvUm.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.Um);
        }

      

        [HttpPost]
        [Route("api/inventory/GetInventoryUnit")]
        public InvUm GetInvUnit([FromBody] GeneralInformation inf)
        {
            return db.InvUm.Where(a => a.RecordId == inf.recordId && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
        }

        [HttpPost]
        [Route("api/inventory/SetInventoryUnits")]
        public invUMTotal setInvUM([FromBody]  invUMTotal det)
        {
            String msg = "";
            GeneralInformation inf = new GeneralInformation();
            inf.recordId = det.um.RecordId;
            inf.detail = det.um.Um;
            inf.usr = det.usr;
            inf.traCheck = det.traCheck;
            AdminGeneral av = new AdminGeneral();
            try
            {
                if (invUMDuplicateCheck(inf))
                {
                    switch (inf.traCheck)
                    {
                        case 1:
                            if (ac.screenCheck(det.usr, 3, 1, 1, 1))
                            {
                                det.um.BranchId = det.usr.bCode;
                                det.um.CustomerCode = det.usr.cCode;
                                db.InvUm.Add(det.um);
                                db.SaveChanges();
                                msg = "OK";
                            }
                            else
                            {
                                msg = "You are not authorised to create Units";
                            }
                            break;
                        case 2:

                            if (ac.screenCheck(det.usr, 3, 1, 1, 2))
                            {
                                var um = db.InvUm.Where(a => a.RecordId == det.um.RecordId && a.CustomerCode == det.usr.cCode).FirstOrDefault();
                                um.Um = det.um.Um;
                                db.SaveChanges();
                                msg = "OK";
                            }
                            else
                            {
                                msg = "You are not authorised to modify Units";
                            }
                            break;
                        case 3:
                            if (ac.screenCheck(det.usr, 3, 1, 1, 3))
                            {

                                var um = db.InvUm.Where(a => a.RecordId == det.um.RecordId && a.CustomerCode == det.usr.cCode).FirstOrDefault();
                                db.InvUm.Remove(um);
                                db.SaveChanges();
                                msg = "OK";
                            }
                            else
                            {
                                msg = "You are not authorised to delete Units";
                            }
                            break;
                    }
                }
                else
                {
                    if (det.traCheck == 3)
                        msg = "This unit is in use deletion is not possible";
                    else
                        msg = "Unit name is already existed";
                }
            }
            catch (Exception ee)
            {
            msg=  det.traCheck==3? "This unit is in use deletion is not possible":ee.Message;
            }
            det.result = msg;
            return det;
        }

      
        private Boolean invUMDuplicateCheck(GeneralInformation inf)
        {
            Boolean b = false;
            switch (inf.traCheck)
            {
                case 1:
                    var x = db.InvUm.Where(a => a.Um == inf.detail && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if (x == null)
                    {
                        b = true;
                    }
                    else
                    {
                        b = false;
                    }
                    break;
                case 2:
                    var y = db.InvUm.Where(a => a.RecordId != inf.recordId && a.Um == inf.detail && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if (y == null)
                    {
                        b = true;
                    }
                    else
                    {
                        b = false;
                    }
                    break;
                case 3:
                    b = true;
                    break;
            }

            return b;
        }
    }
}
