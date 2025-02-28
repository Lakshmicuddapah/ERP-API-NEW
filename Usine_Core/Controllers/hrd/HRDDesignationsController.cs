using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Usine_Core.Controllers.Admin;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.HRD
{

    public class HRDDesignationAllowancesDetails
    {
        public int? recordId { get; set; }
        public int? lineId { get; set; }
        public int? allowanceId { get; set; }   
        public string allowance { get; set; }
        public double? valu { get; set; }
    }
    public class HRDDesignationLeavesDetails
    {
        public int? recordId { get; set; }
        public int? lineId { get; set; }
        public int? leaveId { get; set; }
        public string leave { get; set; }
        public int? valu { get; set; }
    }

    public class HRDDesignationsTotal
    {
        public HrdDesignations designation { get; set; }
        public List<HRDDesignationAllowancesDetails> desigallowances { get; set; }
        public List<HRDDesignationLeavesDetails> leaves { get; set; }
        public int? traCheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }

    }
    
    public class HRDDesignationsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Route("api/HRDDesignations/GetHRDDesignations")]
        [Authorize]
        public string GetHRDDesignations([FromBody] UserInfo usr)
        {
            try
            {
                
                    var details = (from a in db.HrdDesignations.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                                   join b in db.HrdDepartments.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode) on a.Department equals b.RecordId
                                   select new
                                   {
                                       a.RecordId,
                                       a.Designation,
                                       departmentid = b.RecordId,
                                       department = b.SGrp
                                   }).OrderBy(c => c.departmentid).ThenBy(d => d.RecordId).ToList();
                    return JsonConvert.SerializeObject(details);
                
            }
            catch(Exception ee)
            {
                return ee.Message;
            }
        }

        [HttpPost]
        [Route("api/HRDDesignations/GetHRDDesignation")]
        [Authorize]
        public HRDDesignationsTotal GetHRDDesignation([FromBody] GeneralInformation inf)
        {
            HRDDesignationsTotal tot = new HRDDesignationsTotal();
            tot.designation = db.HrdDesignations.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            tot.desigallowances = (from a in db.HrdDesignationsAllowances.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                   join b in db.HrdAllowancesDeductions.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                   on a.Allowance equals b.RecordId
                                   select new HRDDesignationAllowancesDetails
                                   {
                                       recordId = a.RecordId,
                                       lineId = a.LineId,
                                       allowanceId = a.Allowance,
                                       allowance = b.Allowance,
                                       valu = a.Valu
                                   }

                                 ).OrderBy(b => b.lineId).ToList();

            tot.leaves = (from a in db.HrdDesignationsLeaves.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                          join b in db.HrdLeaves.Where(a => a.BranchId == inf.usr.bCode && a.Customercode == inf.usr.cCode)
                          on a.LeaveId equals b.RecordId
                          select new HRDDesignationLeavesDetails
                          {
                              recordId = a.RecordId,
                              lineId = a.Lineid,
                              leaveId = a.LeaveId,
                              leave = b.LeaveCode,
                              valu = a.Valu
                          }

                                 ).OrderBy(b => b.lineId).ToList();

            return tot;

        }


        private Boolean duplicateDesignationCheck(HRDDesignationsTotal tot)
        {
            Boolean b=false;
            switch(tot.traCheck)
            {
                case 1:
                    var cre = db.HrdDesignations.Where(a => a.Designation == tot.designation.Designation && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(cre == null)
                    {
                        b=true;
                    }
                    break;
                case 2:
                    var upd = db.HrdDesignations.Where(a => a.Designation == tot.designation.Designation && a.RecordId != tot.designation.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (upd == null)
                    {
                        b = true;
                    }
                    break;
                case 3:
                    var del = db.HrdEmployees.Where(a => a.Department == tot.designation.RecordId  && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (del == null)
                    {
                        b = true;
                    }
                    break;
            }

            return b;
        }


        [HttpPost]
        [Route("api/HRDDesignations/setHRDDesignation")]
        [Authorize]
        public HRDDesignationsTotal setHRDDesignation([FromBody]HRDDesignationsTotal tot)
        {
            string msg = "";
            try
            {

                if(ac.screenCheck(tot.usr,8,1,4,(int)tot.traCheck))
                {
                    if(duplicateDesignationCheck(tot))
                    {
                        int sno = 1;
                        switch (tot.traCheck)
                        {
                            case 1:
                                using (var txn = db.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        List<HrdDesignationsAllowances> allowances = new List<HrdDesignationsAllowances>();
                                        List<HrdDesignationsLeaves> leaves = new List<HrdDesignationsLeaves>();
                                        var headercre = new HrdDesignations();
                                        headercre.Designation = tot.designation.Designation;
                                        headercre.Department = tot.designation.Department;
                                        headercre.BranchId = tot.usr.bCode;
                                        headercre.CustomerCode = tot.usr.cCode;
                                        db.HrdDesignations.Add(headercre);
                                        db.SaveChanges();

                                        sno = 1;
                                        foreach(HRDDesignationAllowancesDetails det in tot.desigallowances)
                                        {
                                            allowances.Add(new HrdDesignationsAllowances
                                            {
                                                RecordId = headercre.RecordId,
                                                Allowance = det.allowanceId,
                                                Valu = det.valu,
                                                BranchId = tot.usr.bCode,
                                                CustomerCode = tot.usr.cCode
                                            });
                                         
                                        }
                                        db.HrdDesignationsAllowances.AddRange(allowances);

                                        foreach(HRDDesignationLeavesDetails det in tot.leaves)
                                        {
                                            leaves.Add(new HrdDesignationsLeaves
                                            {
                                                RecordId= headercre.RecordId,
                                                LeaveId = det.leaveId,
                                                Valu    =det.valu,
                                                BranchId=tot.usr.bCode,
                                                CustomerCode=tot.usr.cCode
                                            });
                                        }
                                        db.HrdDesignationsLeaves.AddRange(leaves);
                                        db.SaveChanges();
                                        txn.Commit();
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
                                var hrdupd = db.HrdDesignations.Where(a => a.RecordId == tot.designation.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                int updchk = 0;
                                if(hrdupd!=null)
                                {
                                    hrdupd.Designation = tot.designation.Designation;
                                    updchk = 1;
                                }
                                if(updchk==1)
                                {
                                    List<HrdDesignationsAllowances> allowancesupd = new List<HrdDesignationsAllowances>();
                                    List<HrdDesignationsLeaves> leavesupd = new List<HrdDesignationsLeaves>();

                                    var listallowance = db.HrdDesignationsAllowances.Where(a => a.RecordId == tot.designation.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                  if(listallowance.Count() > 0)
                                    db.HrdDesignationsAllowances.RemoveRange(listallowance);
                                    var listleave = db.HrdDesignationsLeaves.Where(a => a.RecordId == tot.designation.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if (listleave.Count() > 0)
                                        db.HrdDesignationsLeaves.RemoveRange(listleave);

                                    foreach (HRDDesignationAllowancesDetails det in tot.desigallowances)
                                    {
                                        allowancesupd.Add(new HrdDesignationsAllowances
                                        {
                                            RecordId = tot.designation.RecordId,
                                            Allowance = det.allowanceId,
                                            Valu = det.valu,
                                            BranchId = tot.usr.bCode,
                                            CustomerCode = tot.usr.cCode
                                        });

                                    }
                                    db.HrdDesignationsAllowances.AddRange(allowancesupd);

                                    foreach (HRDDesignationLeavesDetails det in tot.leaves)
                                    {
                                        leavesupd.Add(new HrdDesignationsLeaves
                                        {
                                            RecordId = tot.designation.RecordId,
                                            LeaveId = det.leaveId,
                                            Valu = det.valu,
                                            BranchId = tot.usr.bCode,
                                            CustomerCode = tot.usr.cCode
                                        });
                                    }
                                    db.HrdDesignationsLeaves.AddRange(leavesupd);
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "No record found";
                                }

                                break;
                            case 3:
                                var hrddel = db.HrdDesignations.Where(a => a.RecordId == tot.designation.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                int delchk = 0;
                                if (hrddel != null)
                                {
                                    
                                    delchk = 1;
                                }
                                if (delchk == 1)
                                {
                                    List<HrdDesignationsAllowances> allowancesupd = new List<HrdDesignationsAllowances>();
                                    List<HrdDesignationsLeaves> leavesupd = new List<HrdDesignationsLeaves>();

                                    var listallowance = db.HrdDesignationsAllowances.Where(a => a.RecordId == tot.designation.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    db.HrdDesignationsAllowances.RemoveRange(listallowance);
                                    var listleave = db.HrdDesignationsLeaves.Where(a => a.RecordId == tot.designation.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    db.HrdDesignationsLeaves.RemoveRange(listleave);

                                    db.HrdDesignations.Remove(hrddel);

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
                        msg = tot.traCheck == 3 ? "This designation is already in use deletion is not possible" : "This name is already existed";
                    }
                }
                else
                {
                    msg = "You are not authorised for this transaction";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            tot.result = msg;
            return tot;
        }










    }
}
