using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Usine_Core.Controllers.Admin;
 
namespace Usine_Core.Controllers.HRD
{
    public class HRDShiftsTotal
    {
        public HrdShifts shift { get; set; }
        public int? traCheck { get; set; }
        public UserInfo usr { get; set; }
    }
   
    public class HRDShiftsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Route("api/HRDShifts/GetHrdShifts")]
        [Authorize]
        public List<HrdShifts> GetHrdShifts([FromBody] UserInfo usr)
        {
            return db.HrdShifts.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
        }

        [HttpPost]
        [Route("api/HRDShifts/SetHrdShifts")]
        [Authorize]
        public TransactionResult SetHrdShifts([FromBody] HRDShiftsTotal tot)
        {
            string msg = "";
            try
            {
                Boolean b = ac.screenCheck(tot.usr, 8, 1, 7, (int)tot.traCheck);
                if(b)
                {
                    if(dupCheck(tot))
                    {
                        switch(tot.traCheck)
                        {
                            case 1:
                                tot.shift.BranchId = tot.usr.bCode;
                                tot.shift.CustomerCode = tot.usr.cCode;
                                db.HrdShifts.Add(tot.shift);
                                db.SaveChanges();
                                msg = "OK";
                                break;
                            case 2:
                                var upd = db.HrdShifts.Where(a => a.RecordId == tot.shift.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if(upd != null)
                                {
                                    upd.ShiftName = tot.shift.ShiftName;
                                    upd.FromTime = tot.shift.FromTime;
                                    upd.ToTime = tot.shift.ToTime;
                                    db.SaveChanges();
                                    msg="OK";
                                }
                                else
                                {
                                    msg = "No record found to update";
                                }
                                break;
                            case 3:
                                var del = db.HrdShifts.Where(a => a.RecordId == tot.shift.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (del != null)
                                {
                                    db.HrdShifts.Remove(del);
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "No record found to delete";
                                }
                                break;
                        }
                    }
                    else
                    {
                        msg = "This Shift is already existed";
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
            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result; 
        }
        private Boolean dupCheck(HRDShiftsTotal tot)
        {
            Boolean b = true;
            UsineContext db1 = new UsineContext();
            switch(tot.traCheck)
            {
                case 1:
                    var cre = db1.HrdShifts.Where(a => a.ShiftName.ToUpper() == tot.shift.ShiftName.ToUpper() && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(cre != null)
                    {
                        b = false;
                    }
                    break;
                case 2:
                    var upd = db1.HrdShifts.Where(a => a.RecordId != tot.shift.RecordId && a.ShiftName.ToUpper() == tot.shift.ShiftName.ToUpper() && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (upd != null)
                    {
                        b = false;
                    }
                    break;
            }
            return b;
        }
    }
}
