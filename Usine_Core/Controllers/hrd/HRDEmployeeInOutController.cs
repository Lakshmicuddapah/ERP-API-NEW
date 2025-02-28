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
  public class HRDEmpInoutTotal
    {
        public HrdEmpInOutDetails inout { get; set; }
        public int? traCheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class HRDEmployeeInOutController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Route("api/HRDEmployeeInOut/GetTodayInoutDetails")]
        [Authorize]
        public dynamic GetTodayInoutDetails([FromBody]GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 =dat1.AddDays(1);
            return (from a in db.HrdEmpInOutDetails.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                       join b in db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.EmpId equals b.RecordId
                       select new
                       {
                           lineid=a.Lineid,
                           recordid = a.EmpId,
                           empno = b.Empno,
                           empname = b.Empname,
                           fromTime = a.FromTime,
                           toTime = a.ToTime,
                           pos = a.Pos,
                           mobile=b.Mobile,
                           email=b.Email
                           
                       }
                     ).OrderBy(b => b.empname).ToList();
        }
        [HttpPost]
        [Route("api/HRDEmployeeInOut/SetTodayInoutDetails")]
        [Authorize]
        public HRDEmpInoutTotal SetTodayInoutDetails([FromBody]HRDEmpInoutTotal tot)
        {
            string msg = "";
            switch(tot.traCheck)
            {
                case 1:
                    var det = new HrdEmpInOutDetails();
                    det.EmpId = tot.inout.EmpId;
                    det.Dat = ac.DateAdjustFromFrontEnd(tot.inout.Dat.Value);
                    det.FromTime =  tot.inout.FromTime ;
                    det.Pos = 1;
                    det.BranchId = tot.usr.bCode;
                    det.CustomerCode = tot.usr.cCode;
                    db.HrdEmpInOutDetails.Add(det);
                    db.SaveChanges();
                    msg = "OK";
                        
                    break;
                case 2:
                    var detupd = db.HrdEmpInOutDetails.Where(a => a.Lineid == tot.inout.Lineid && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(detupd != null)
                    {
                        detupd.ToTime = tot.inout.ToTime;
                        detupd.Pos = 0;
                        db.SaveChanges();
                        msg = "OK";
                    }
                    else
                    {
                        msg = "No record found";
                    }
                    
                    break;
            }

            tot.result = msg;
            return tot;
        }
    }
}
