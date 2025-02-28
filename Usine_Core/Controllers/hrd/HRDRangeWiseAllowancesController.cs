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
   public class HRDRangeWiseAllowancesTotal
    {
        public List<HrdAllowanceDeductionRanges> allowances { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class HRDRangeWiseAllowancesController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();


        [HttpPost]
        [Route("api/HRDRangeWiseAllowances/GetHRDRangeWiseAllowancesDeductions")]
        [Authorize]
        public dynamic GetHRDRangeWiseAllowancesDeductions([FromBody] UserInfo usr)
        {
            return (from a in db.HrdAllowanceDeductionRanges.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                    join b in db.HrdAllowancesDeductions.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                    on a.AllowanceId equals b.RecordId
                    select new
                    {
                        a.Lineid,
                        a.AllowanceId,
                        b.Allowance,
                        a.FromValue,
                        a.ToValue,
                        a.Valu
                    }).OrderBy(c => c.Lineid).ToList();
        }

        [HttpPost]
        [Route("api/HRDRangeWiseAllowances/SetHRDRangeWiseAllowancesDeductions")]
        [Authorize]
        public HRDRangeWiseAllowancesTotal SetHRDRangeWiseAllowancesDeductions([FromBody] HRDRangeWiseAllowancesTotal tot)
        {
            string msg = "";
            if (ac.screenCheck(tot.usr, 8, 1, 5, 0))
            {
                try
                {
                    foreach (HrdAllowanceDeductionRanges det in tot.allowances)
                    {
                        det.BranchId = tot.usr.bCode;
                        det.CustomerCode = tot.usr.cCode;
                    }
                    var lst = db.HrdAllowanceDeductionRanges.Where(a => a.AllowanceId == tot.allowances[0].AllowanceId).ToList();

                    db.HrdAllowanceDeductionRanges.RemoveRange(lst);
                    db.HrdAllowanceDeductionRanges.AddRange(tot.allowances);
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
            tot.result = msg;
            return tot;
           
        }
    }
}
