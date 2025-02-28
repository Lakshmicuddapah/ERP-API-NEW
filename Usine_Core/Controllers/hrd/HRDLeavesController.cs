using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Usine_Core.Controllers.Admin;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.HRD
{
    public class HRDLeavesTotal
    {
        public HrdLeaves leave { get; set; }
        public int? traCheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
     
    public class HRDLeavesController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();


        [HttpPost]
        [Route("api/HRDLeaves/GetHRDLeaves")]
        [Authorize]
        public List<HrdLeaves> GetHRDLeaves([FromBody]UserInfo usr)
        {
            return db.HrdLeaves.Where(a => a.BranchId == usr.bCode && a.Customercode == usr.cCode).OrderBy(b => b.RecordId).ToList();
        }

        public Boolean dupLeaveCheck(HRDLeavesTotal tot)
        {
            Boolean b = false;
            switch(tot.traCheck)
            {
                case 1:
                    var cre = db.HrdLeaves.Where(a => a.LeaveCode == tot.leave.LeaveCode && a.BranchId == tot.usr.bCode && a.Customercode == tot.usr.cCode).FirstOrDefault();
                    if (cre == null)
                    {
                        b=true;
                    }
                    break;
                case 2:
                    var upd = db.HrdLeaves.Where(a => a.LeaveCode == tot.leave.LeaveCode && a.RecordId != tot.leave.RecordId && a.BranchId == tot.usr.bCode && a.Customercode == tot.usr.cCode).FirstOrDefault();
                    if (upd == null)
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
        [Route("api/HRDLeaves/SetHRDLeave")]
        [Authorize]
        public HRDLeavesTotal SetHRDLeave([FromBody]HRDLeavesTotal tot)
        {
            string msg = "";
            try
            {
                if(ac.screenCheck(tot.usr,8,1,3,(int)tot.traCheck))
                {
                    switch(tot.traCheck)
                    {
                        case 1:
                            HrdLeaves lea = new HrdLeaves();
                            lea.LeaveCode= tot.leave.LeaveCode;
                            lea.LeaveDescription= tot.leave.LeaveDescription;
                            lea.PayType= tot.leave.PayType;
                            lea.ForwardType= tot.leave.ForwardType;
                            lea.BranchId=tot.usr.bCode;
                            lea.Customercode=tot.usr.cCode;
                            db.HrdLeaves.Add(lea);
                            db.SaveChanges();
                            msg = "OK";
                            break;
                        case 2:
                            HrdLeaves leau = db.HrdLeaves.Where(a => a.RecordId == tot.leave.RecordId && a.BranchId == tot.usr.bCode && a.Customercode == tot.usr.cCode).FirstOrDefault();
                            leau.LeaveCode = tot.leave.LeaveCode;
                            leau.LeaveDescription = tot.leave.LeaveDescription;
                            leau.PayType = tot.leave.PayType;
                            leau.ForwardType = tot.leave.ForwardType;
                            db.SaveChanges();
                            msg = "OK";
                            break;
                        case 3:
                            HrdLeaves lead = db.HrdLeaves.Where(a => a.RecordId == tot.leave.RecordId && a.BranchId == tot.usr.bCode && a.Customercode == tot.usr.cCode).FirstOrDefault();
                            if (lead != null)
                            {
                                db.HrdLeaves.Remove(lead);
                                db.SaveChanges();
                                msg="OK";
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
                    msg = "You are not authorised for this transaction";
                }
            }
            catch(Exception ex)
            {
                msg = ex.Message;
            }

            tot.result = msg;
            return tot;

        }

    }
}
