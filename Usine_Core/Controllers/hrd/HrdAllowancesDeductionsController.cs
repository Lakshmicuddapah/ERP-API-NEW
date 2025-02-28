using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Usine_Core.Controllers.Admin;
using Usine_Core.others.dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.HRD
{
   public class HrdAllowancesDeductionsTotal
    {
        public HrdAllowancesDeductions header { get; set; }
        public List<HrdAllowancesDeductionsEffectsComplete> lines { get; set; }
        public int? traCheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class HrdAllowancesDeductionsEffectsComplete
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public int? AllowanceOn { get; set; }
        public string allowanceName { get; set; }
    }
    public class HRDAllowancesDeductionsRangeRequirements
    {
        public List<HrdAllowancesDeductions> allowancesDeductions { get; set; }
        public List<HrdAllowanceDeductionRanges> allowanceDeductionRanges { get; set; }
    }
    public class HRDAllowancesDeductionsRangeTotal
    {
        public List<HrdAllowanceDeductionRanges> range { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class HrdAllowancesDeductionsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Route("api/HrdAllowancesDeductions/GetHrdAllowancesDeductions")]
        [Authorize]
        public List<HrdAllowancesDeductions> GetHrdAllowancesDeductions([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 8, 1, 2, 0))
            {
                return db.HrdAllowancesDeductions.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        [Route("api/HrdAllowancesDeductions/GetHrdAllowancesDeduction")]
        [Authorize]
        public HrdAllowancesDeductionsTotal GetHrdAllowancesDeduction([FromBody] GeneralInformation inf)
        {
            HrdAllowancesDeductionsTotal tot = new HrdAllowancesDeductionsTotal();
            tot.header = db.HrdAllowancesDeductions.Where(a => a.RecordId == inf.recordId && a.CustomerCode == inf.usr.cCode).FirstOrDefault();


            var lines1=db.HrdAllowancesDeductionsEffects.Where(a => a.RecordId == inf.recordId && a.CustomerCode == inf.usr.cCode).ToList();


            List<HrdAllowancesDeductionsEffectsComplete> lines2 = new List<HrdAllowancesDeductionsEffectsComplete>();
            lines2.Add(new HrdAllowancesDeductionsEffectsComplete
            {
                RecordId=-1,
                Sno=1,
                AllowanceOn=-1,
                allowanceName="Basic"
            });

            lines2.Add(new HrdAllowancesDeductionsEffectsComplete
            {
                RecordId = -2,
                Sno = 2,
                AllowanceOn = -2,
                allowanceName = "LOP"
            });
            lines2.Add(new HrdAllowancesDeductionsEffectsComplete
            {
                RecordId = -3,
                Sno = 3,
                AllowanceOn = -3,
                allowanceName = "Late"
            });
            lines2.Add(new HrdAllowancesDeductionsEffectsComplete
            {
                RecordId = -4,
                Sno = 4,
                AllowanceOn = -4,
                allowanceName = "OT"
            });

            var firstlines = (from a in lines1
                           join b in lines2 on a.AllowanceOn equals b.AllowanceOn
                           select new HrdAllowancesDeductionsEffectsComplete
                           {
                               RecordId=a.RecordId,
                               Sno=a.Sno,
                               AllowanceOn=b.AllowanceOn,
                               allowanceName=b.allowanceName
                           }
                         ).ToList();

          var secondlines = (from a in db.HrdAllowancesDeductionsEffects.Where(a => a.RecordId == inf.recordId && a.CustomerCode == inf.usr.cCode)
                         join b in db.HrdAllowancesDeductions.Where(a=> a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.AllowanceOn equals b.RecordId
                         select new HrdAllowancesDeductionsEffectsComplete
                         {
                             RecordId = a.RecordId,
                             Sno = a.Sno,
                             AllowanceOn = a.AllowanceOn,
                             allowanceName = b.Allowance
                         }
                       ).OrderBy(b => b.Sno).ToList();


            
            tot.lines=firstlines.Union(secondlines).OrderBy(a => a.Sno).ToList();
             return tot;
        }

        public Boolean dupHrdAllowanceCheck([FromBody]HrdAllowancesDeductionsTotal tot)
        {
            Boolean b = false;
            switch(tot.traCheck)
            {
                case 1:
                    var cre = db.HrdAllowancesDeductions.Where(a => a.Allowance.ToUpper() == tot.header.Allowance.ToUpper() && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(cre == null)
                    {
                        b = true;
                    }
                    break;
                case 2:
                    var upd = db.HrdAllowancesDeductions.Where(a => a.Allowance.ToUpper() == tot.header.Allowance.ToUpper() && a.RecordId != tot.header.RecordId   && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (upd == null)
                    {
                        b = true;
                    }
                    break;
                case 3:

                    var del1 = db.HrdAllowancesDeductionsEffects.Where(a => a.AllowanceOn == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    var del2 = db.HrdDesignationsAllowances.Where(a => a.Allowance == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    var del3 = db.HrdEmployeeAllowancesDeductions.Where(a => a.Allowance == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();

                    if(del1 == null && del2==null && del3 == null)
                    {
                        b = true;
                    }
                     break;

            }

            return b;
        }
        [HttpPost]
        [Route("api/HrdAllowancesDeductions/SetHrdAllowancesDeduction")]
        [Authorize]
        public HrdAllowancesDeductionsTotal SetHrdAllowancesDeduction([FromBody] HrdAllowancesDeductionsTotal tot)
        {
            string msg = "";
            try
            {
                if(dupHrdAllowanceCheck(tot))
                {
                    if(ac.screenCheck(tot.usr,8,1,2,(int)tot.traCheck))
                    {
                        int sno = 1;
                        List<HrdAllowancesDeductionsEffects> list = new List<HrdAllowancesDeductionsEffects>();
                        switch (tot.traCheck)
                        {
                            case 1:
                                using (var txn = db.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        list = new List<HrdAllowancesDeductionsEffects>();
                                        var headercre = new HrdAllowancesDeductions();
                                        headercre.Allowance = tot.header.Allowance;
                                        headercre.AllowanceCheck = tot.header.AllowanceCheck;
                                        headercre.CalcType = tot.header.CalcType;
                                        headercre.EffectAs = tot.header.EffectAs;
                                        headercre.Branchid = tot.usr.bCode;
                                        headercre.CustomerCode = tot.usr.cCode;
                                        db.HrdAllowancesDeductions.Add(headercre);
                                        db.SaveChanges();
                                        sno = 1;
                                        
                                        foreach(HrdAllowancesDeductionsEffectsComplete det in tot.lines)
                                        {
                                            list.Add(new HrdAllowancesDeductionsEffects
                                            {
                                                RecordId = headercre.RecordId,
                                                Sno=sno,
                                                AllowanceOn=det.AllowanceOn,
                                                BranchId=tot.usr.bCode,
                                                CustomerCode=tot.usr.cCode, 
                                            });
                                            sno++;
                                        }
                                        if (list.Count > 0)
                                        {
                                            db.HrdAllowancesDeductionsEffects.AddRange(list);
                                            db.SaveChanges();
                                        }
                                        txn.Commit();
                                        tot.header.RecordId = headercre.RecordId;
                                        msg = "OK";
                                    }
                                    catch(Exception ex)
                                    {
                                        txn.Rollback();
                                        msg = ex.Message;
                                    }
                                }
                                    break;
                            case 2:
                                var headerupd = db.HrdAllowancesDeductions.Where(a => a.RecordId == tot.header.RecordId && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (headerupd != null)
                                {
                                    headerupd.Allowance = tot.header.Allowance;
                                    headerupd.AllowanceCheck = tot.header.AllowanceCheck;
                                    headerupd.EffectAs=tot.header.EffectAs;
                                    headerupd.Statu = tot.header.Statu;
                                }
                                sno = (int)db.HrdAllowancesDeductionsEffects.Where(a => a.RecordId == tot.header.RecordId && a.CustomerCode == tot.usr.cCode).Max(b => b.Sno);
                                sno++;
                                var lines =db.HrdAllowancesDeductionsEffects.Where(a => a.RecordId== tot.header.RecordId && a.CustomerCode == tot.header.CustomerCode).ToList();
                               if(lines.Count > 0)  
                                db.HrdAllowancesDeductionsEffects.RemoveRange(lines);
                              
                               list = new List<HrdAllowancesDeductionsEffects>();
                               
                                foreach (HrdAllowancesDeductionsEffectsComplete det in tot.lines)
                                {
                                    list.Add(new HrdAllowancesDeductionsEffects
                                    {
                                        RecordId = tot.header.RecordId,
                                        Sno = sno,
                                        AllowanceOn = det.AllowanceOn,
                                        BranchId = tot.usr.bCode,
                                        CustomerCode = tot.usr.cCode,
                                    });
                                    sno++;
                                }
                                if (list.Count > 0)
                                {
                                    db.HrdAllowancesDeductionsEffects.AddRange(list);
                                    
                                }
                                db.SaveChanges();
                                msg = "OK";
                                break;
                            case 3:
                                var headerdel = db.HrdAllowancesDeductions.Where(a => a.RecordId == tot.header.RecordId && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (headerdel != null)
                                {
                                     db.HrdAllowancesDeductions.Remove(headerdel);
                                }
                                var linesdel = db.HrdAllowancesDeductionsEffects.Where(a => a.RecordId == tot.header.RecordId && a.CustomerCode == tot.header.CustomerCode).ToList();
                                if (linesdel.Count > 0)
                                    db.HrdAllowancesDeductionsEffects.RemoveRange(linesdel);
                              
                                db.SaveChanges();
                                msg="OK";
                                break;
                        }
                    }
                    else
                    {
                        msg = "You are not authorised for this transaction";
                    }
                }
                else
                {
                    if(tot.traCheck==3)
                    {
                        msg = "This allowance is in use you can not delete";
                    }
                    else
                    {
                        msg = "This name is already existed";
                    }
                }
            }
            catch(Exception ex)
            {
                msg = ex.Message;
            }


            tot.result = msg;
            return tot;
        }

       
        [HttpPost]
        [Route("api/HrdAllowancesDeductions/GetHRDAllowancesDeductionRangeRequirements")]
        [Authorize]
        public HRDAllowancesDeductionsRangeRequirementsDTO GetHRDAllowancesDeductionRangeRequirements([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 8, 1, 5, 0))
            {
                HRDAllowancesDeductionsRangeRequirementsDTO result = new HRDAllowancesDeductionsRangeRequirementsDTO();

                result.AllowancesDeductions = db.HrdAllowancesDeductions
                    .Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                    .OrderBy(b => b.Allowance)
                    .Select(a => new HrdAllowancesDeductionsDTO
                    {
                        RecordId = a.RecordId,
                        Allowance = a.Allowance,
                        AllowanceCheck = a.AllowanceCheck,
                        CalcType = a.CalcType,
                        EffectAs = a.EffectAs,
                        Statu = a.Statu,
                        Branchid = usr.bCode,
                        CustomerCode = usr.cCode,
                    })
                    .ToList();

                result.AllowanceDeductionRanges = db.HrdAllowanceDeductionRanges
                    .Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                    .OrderBy(b => b.AllowanceId)
                    .ThenBy(b => b.Lineid)
                    .Select(r => new HrdAllowanceDeductionRangesDTO
                    {
                        AllowanceId = r.AllowanceId,
                        Lineid = r.Lineid,
                        FromValue = r.FromValue,
                        ToValue = r.ToValue,
                        Valu = r.Valu,
                        CustomerCode = r.CustomerCode
                    })
                    .ToList();

                return result;
            }
            return null;
        }

        [HttpPost]
        [Route("api/HrdAllowancesDeductions/setHRDAllowanceDeductionsRange")]
        [Authorize]
        public HRDAllowancesDeductionsRangeTotal setHRDAllowanceDeductionsRange([FromBody] HRDAllowancesDeductionsRangeTotal tot)
        {
            string msg = "";
            try
            {
                if (ac.screenCheck(tot.usr, 8, 1, 5, 0))
                {
                    int? allid = 0;
                    if(tot.range.Count > 0)
                    {
                        allid = tot.range[0].AllowanceId;
                    }
                    var list = db.HrdAllowanceDeductionRanges.Where(a => a.AllowanceId==allid && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                    if(list.Count > 0)
                    {
                        db.HrdAllowanceDeductionRanges.RemoveRange(list);
                    }
                    foreach(var lst in tot.range)
                    {
                        lst.BranchId= tot.usr.bCode;
                        lst.CustomerCode = tot.usr.cCode;
                    }
                    db.HrdAllowanceDeductionRanges.AddRange(tot.range);
                    db.SaveChanges();
                    msg = "OK";
                }
                else
                {
                    msg = "Please verify your autorisation";
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
