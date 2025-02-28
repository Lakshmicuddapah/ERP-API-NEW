using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Hosting;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.HRD
{
    public class HRDEmployeeAllowancesDetails
    {
        public long? lineid { get; set; }    
        public int? allowanceId { get; set; }
        public string allowance { get; set; }
        public double? valu { get; set; }

    }
    public class HRDEmployeeLeavesDetails
    {
        public long? lineid { get; set; }
        public int? leaveId { get; set; }
        public string leave { get; set; }
        public int? valu { get; set; }
        public int? payType { get; set; }

    }
    public class HRDEmployeeRequirements
    {
        public List<HrdDepartmentGroupsTree> departmentTree { get; set; }
        public List<HrdDesignations> designations { get; set; }
        public List<HrdDesignationsAllowances> desigallowances { get; set; }
        public List<HrdDesignationsLeaves> desigleaves { get; set; }
        public List<HrdAllowancesDeductions> allowancesDeductions { get; set; }
        public List<HrdLeaves> leaves { get; set; }


    }
     
    public class HRDEmployeeTotal
    {
         public HrdEmployees employee { get; set; }
        public List<HRDEmployeeAllowancesDetails> allowances { get; set; }
        public List<HRDEmployeeLeavesDetails> leaves { get; set; }
        public List<HrdEmployeeIdentifications> identifications { get; set; }
        public List<HrdEmployeeCurriculum> curriculum { get; set; }
        public List<HrdEmployeeExperience> experience { get; set; }
        public List<HrdEmployeeFamilyDetails> family { get; set; }
        public List<FileUploadAttribute> imgs { get; set; }
        public int? traCheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    
    public class HRDEmployeesController : ControllerBase
    {

        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        private readonly IHostingEnvironment ho;
        
        public HRDEmployeesController(IHostingEnvironment hostingEnvironment)
        {

            ho = hostingEnvironment;
        }

        [HttpPost]
        [Route("api/HRDEmployees/GetHRDEmployeeRequirements")]
        [Authorize]
        public HRDEmployeeRequirements GetHRDEmployeeRequirements([FromBody]UserInfo usr)
        {
            HRDEmployeeRequirements tot = new HRDEmployeeRequirements();
            hrdDepartmentsController con = new hrdDepartmentsController();
            tot.departmentTree = con.GetHrdDepartmentsTreeView(usr);
            tot.designations = (from a in db.HrdDesignations.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                                select new HrdDesignations
                                {
                                    RecordId=a.RecordId,
                                    Department=a.Department,
                                    Designation=a.Designation
                                }).OrderBy(b => b.Designation).ToList();
            tot.desigallowances = (from a in db.HrdDesignationsAllowances.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                                   select new HrdDesignationsAllowances
                                   {
                                       RecordId=a.RecordId,
                                       LineId=a.LineId,
                                       Allowance=a.Allowance,
                                       Valu=a.Valu
                                   }).OrderBy(b => b.RecordId).ThenBy(c => c.LineId).ToList();
            tot.desigleaves = (from a in db.HrdDesignationsLeaves.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                               select new HrdDesignationsLeaves
                               {
                                   RecordId=a.RecordId,
                                   Lineid=a.Lineid,
                                   LeaveId=a.LeaveId,
                                   Leave=a.Leave,
                                   Valu=a.Valu
                               }).OrderBy(b => b.RecordId).ThenBy(c => c.Lineid).ToList();
            tot.allowancesDeductions = (from a in db.HrdAllowancesDeductions.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                                        select new HrdAllowancesDeductions
                                        {
                                            RecordId=a.RecordId,
                                            Allowance=a.Allowance,
                                            AllowanceCheck=a.AllowanceCheck,
                                            CalcType=a.CalcType,
                                            EffectAs=a.EffectAs
                                        }).OrderBy(b => b.RecordId).ToList();
            tot.leaves = (from a in db.HrdLeaves.Where(a => a.BranchId == usr.bCode && a.Customercode == usr.cCode)
                          select new HrdLeaves
                          {
                              RecordId=a.RecordId,
                              LeaveCode=a.LeaveCode,
                              LeaveDescription=a.LeaveDescription
                          }).OrderBy(b => b.RecordId).ToList();


            return tot;
        }



        [HttpPost]
        [Route("api/HRDEmployees/GetHRDEmployees")]
        [Authorize]
        public dynamic GetHRDEmployees([FromBody]UserInfo usr)
        {
            return (from empdepts in (from emps in db.HrdEmployees.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                                             join depts in db.HrdDepartments.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                                             on emps.Department equals depts.RecordId
                                             select new
                                             {
                                                 emps.RecordId,
                                                 emps.Empno,
                                                 emps.Empname,
                                                 emps.Mobile,
                                                 emps.Email,
                                                 emps.Pic,
                                                 emps.Designation,
                                                 department = depts.SGrp
                                             }
                           )
                           join desigs in db.HrdDesignations.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                           on empdepts.Designation equals desigs.RecordId
                           select new
                           {
                               empdepts.RecordId,
                               empdepts.Empno,
                               empdepts.Empname,
                               empdepts.Mobile,
                               empdepts.Email,
                               empdepts.department,
                               desigs.Designation,
                               empdepts.Pic
                           }).OrderBy(a => a.Empname).ToList();
        }
        [HttpPost]
        [Route("api/HRDEmployees/GetHRDEmployee")]
        [Authorize]
        public HRDEmployeeTotal GetHRDEmployee([FromBody]GeneralInformation inf)
        {
            HRDEmployeeTotal tot = new HRDEmployeeTotal();
            tot.employee = db.HrdEmployees.Where(a => a.RecordId == inf.recordId && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            tot.allowances=(from a in db.HrdEmployeeAllowancesDeductions.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                            join b in db.HrdAllowancesDeductions.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                            on a.Allowance equals b.RecordId 
                            select new HRDEmployeeAllowancesDetails
                            {
                                lineid=a.LineId,
                                allowanceId=a.Allowance,
                                allowance=b.Allowance,
                                valu=a.Valu
                            }).OrderBy(b => b.lineid).ToList();
            tot.leaves = (from a in db.HrdEmployeeLeaves.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                          join b in db.HrdLeaves.Where(a => a.BranchId == inf.usr.bCode && a.Customercode == inf.usr.cCode)
                          on a.LeaveId equals b.RecordId
                          select new HRDEmployeeLeavesDetails
                          {
                              lineid=a.LineId,
                              leaveId=a.LeaveId,
                              leave=b.LeaveCode,
                              valu=a.Valu,
                              payType = b.PayType

                          }).OrderBy(b => b.lineid).ToList();
            tot.identifications =(from a in db.HrdEmployeeIdentifications.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                  select new HrdEmployeeIdentifications
                                  {
                                      RecordId=a.RecordId,
                                      LineId=a.LineId,
                                      Identit=a.Identit
                                  }).OrderBy(b => b.LineId).ToList();
            tot.curriculum = (from a in db.HrdEmployeeCurriculum.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                              select new HrdEmployeeCurriculum
                {
                                  RecordId=a.RecordId,
                                  LineId=a.LineId,
                                  Frmyear=a.Frmyear,
                                  Toyear=a.Toyear,
                                  Qual=a.Qual,
                                  Board=a.Board
            }).OrderBy(b => b.LineId).ToList();

            tot.experience= (from a in db.HrdEmployeeExperience.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                             select new HrdEmployeeExperience
                             {
                                 RecordId = a.RecordId,
                                 LineId = a.LineId,
                                 Frmyear = a.Frmyear,
                                 Toyear = a.Toyear,
                                 Designation=a.Designation,
                                 Organisation=a.Organisation
                             }).OrderBy(b => b.LineId).ToList();
            tot.family =(from a in  db.HrdEmployeeFamilyDetails.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                         select new HrdEmployeeFamilyDetails
                         {
                             RecordId = a.RecordId,
                             LineId = a.LineId,
                             Relativename=a.Relativename,
                             Relation=a.Relation
                         }).OrderBy(b => b.LineId).ToList();
            return tot;
        }

        [HttpPost]
        [Route("api/HRDEmployees/EmployeeServiceClosing")]
        [Authorize]
        public TransactionResult EmployeeServiceClosing([FromBody] GeneralInformation inf)
        {
            string msg = "";
            try
            {
                if (ac.screenCheck(inf.usr, 8, 2, 5, 0))
                {
                    var emp = db.HrdEmployees.Where(a => a.RecordId == inf.recordId && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if(emp != null)
                    {
                        emp.Dob = ac.getPresentDateTime();
                        emp.Branchid = emp.Branchid + inf.detail;
                    }
                    db.SaveChanges();
                    msg = "OK";
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

        private Boolean duplicateEmpCheck(HRDEmployeeTotal tot)
        {
            Boolean b = false;
            switch(tot.traCheck)
            {
                case 1:
                    var cre = db.HrdEmployees.Where(a => a.Empno == tot.employee.Empno && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(cre == null)
                    {
                        b= true;
                    }
                    break;
                case 2:
                    var upd = db.HrdEmployees.Where(a => a.Empno == tot.employee.Empno && a.RecordId != tot.employee.RecordId && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
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
        [Route("api/HRDEmployees/SetHRDEmployee")]
        [Authorize]
        public TransactionResult SetHRDEmployee([FromBody] HRDEmployeeTotal tot)
        {
            string msg = "";
            DataBaseContext ggg = new DataBaseContext();
            if (ac.screenCheck(tot.usr, 8, 1, 6, (int)tot.traCheck))
            {
                if (duplicateEmpCheck(tot))
                {
                    List<HrdEmployeeAllowancesDeductions> allows = new List<HrdEmployeeAllowancesDeductions>();
                    List<HrdEmployeeLeaves> leaves = new List<HrdEmployeeLeaves>();


                    try
                    {
                        switch (tot.traCheck)
                        {
                            case 1:
                                using (var txn = db.Database.BeginTransaction())
                                {
                                    try
                                    {
                                      
                                        tot.employee.Branchid = tot.usr.bCode;
                                        tot.employee.CustomerCode = tot.usr.cCode;
                                        db.HrdEmployees.Add(tot.employee);

                                        db.SaveChanges();

                                        foreach (var det in tot.identifications)
                                        {
                                            det.RecordId = tot.employee.RecordId;
                                            det.BranchId = tot.usr.bCode;
                                            det.CustomerCode = tot.usr.cCode;
                                        }
                                        foreach (var det in tot.allowances)
                                        {
                                            allows.Add(
                                               new HrdEmployeeAllowancesDeductions
                                               {
                                                   RecordId = tot.employee.RecordId,
                                                   Allowance = det.allowanceId,
                                                   Valu = det.valu,
                                                   BranchId = tot.usr.bCode,
                                                   CustomerCode = tot.usr.cCode
                                               });
                                        }
                                        foreach (var det in tot.leaves)
                                        {
                                            leaves.Add(new HrdEmployeeLeaves
                                            {
                                                RecordId = tot.employee.RecordId,
                                                LeaveId = det.leaveId,
                                                Valu = det.valu,
                                                BranchId = tot.usr.bCode,
                                                CustomerCode = tot.usr.cCode
                                            });
                                        }
                                        foreach (var det in tot.curriculum)
                                        {
                                            det.RecordId = tot.employee.RecordId;
                                            det.BranchId = tot.usr.bCode;
                                            det.CustomerCode = tot.usr.cCode;
                                        }
                                        foreach (var det in tot.experience)
                                        {
                                            det.RecordId = tot.employee.RecordId;
                                            det.BranchId = tot.usr.bCode;
                                            det.CustomerCode = tot.usr.cCode;
                                        }
                                        foreach (var det in tot.family)
                                        {
                                            det.RecordId = tot.employee.RecordId;
                                            det.BranchId = tot.usr.bCode;
                                            det.CustomerCode = tot.usr.cCode;
                                        }
                                        if (tot.identifications.Count() > 0)
                                        {
                                            db.HrdEmployeeIdentifications.AddRange(tot.identifications);
                                        }
                                        if (allows.Count() > 0)
                                        {
                                            db.HrdEmployeeAllowancesDeductions.AddRange(allows);
                                        }
                                        if (leaves.Count() > 0)
                                        {
                                            db.HrdEmployeeLeaves.AddRange(leaves);
                                        }
                                        if (tot.curriculum.Count() > 0)
                                        {
                                            db.HrdEmployeeCurriculum.AddRange(tot.curriculum);
                                        }
                                        if (tot.experience.Count() > 0)
                                        {
                                            db.HrdEmployeeExperience.AddRange(tot.experience);
                                        }
                                        if (tot.family.Count() > 0)
                                        {
                                            db.HrdEmployeeFamilyDetails.AddRange(tot.family);
                                        }
                                        db.SaveChanges();
                                        txn.Commit();
                                       
                                        db.SaveChanges();
                                        string fn = "EMP_PIC_" + tot.employee.RecordId + "_" + tot.usr.bCode + "_" + tot.usr.cCode + ".jpg";
                                        var imgresult = ggg.imageUploadGeneric(tot.imgs, ho.WebRootPath + "\\Attachments\\" + "hrd\\" + fn);

                                        msg = "OK";

                                    }
                                    catch (Exception ex)
                                    {
                                        msg = ex.Message;
                                        txn.Rollback();
                                    }

                                }
                                break;
                            case 2:
                                var empupd = db.HrdEmployees.Where(a => a.RecordId == tot.employee.RecordId && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                int updchk = 0;
                                if (empupd != null)
                                {
                                    updchk = 1;
                                  
                                }
                                if(updchk==1)
                                {
                                    empupd.Empno = tot.employee.Empno;
                                    empupd.Empname = tot.employee.Empname;
                                    empupd.Surname = tot.employee.Surname;
                                    empupd.Fathername= tot.employee.Fathername;
                                    empupd.Gender= tot.employee.Gender;
                                    empupd.Dob= tot.employee.Dob;
                                    empupd.ModeofPay= tot.employee.ModeofPay;
                                    empupd.MaritalStatus = tot.employee.MaritalStatus;
                                    empupd.Address = tot.employee.Address;
                                    empupd.Country = tot.employee.Country;
                                    empupd.Stat = tot.employee.Stat;
                                    empupd.District = tot.employee.District;
                                    empupd.City=tot.employee.City;
                                    empupd.Zip=tot.employee.Zip;
                                    empupd.Mobile= tot.employee.Mobile;
                                    empupd.Tel= tot.employee.Tel;
                                    empupd.Fax= tot.employee.Fax;
                                    empupd.Email= tot.employee.Email;
                                    empupd.Webid= tot.employee.Webid;
                                    empupd.Pan = tot.employee.Pan;
                                    empupd.Aadhar= tot.employee.Aadhar;
                                    empupd.Idtype= tot.employee.Idtype;
                                    empupd.Idno= tot.employee.Idno;
                                    empupd.Height= tot.employee.Height;
                                    empupd.Weight= tot.employee.Weight;
                                    empupd.BloodGrp= tot.employee.BloodGrp;
                                    empupd.Referenc= tot.employee.Referenc;
                                    empupd.Department= tot.employee.Department;
                                    empupd.Designation= tot.employee.Designation;
                                    empupd.Doj= tot.employee.Doj;
                                    empupd.BasicPay= tot.employee.BasicPay;
                                    empupd.GrandPay= tot.employee.GrandPay;
                                    empupd.BasicChk= tot.employee.BasicChk;
                                    empupd.Mgr  = tot.employee.Mgr;
                                    empupd.LeavesScheme= tot.employee.LeavesScheme;
                                    empupd.BankPay= tot.employee.BankPay;
                                    empupd.SbAc= tot.employee.SbAc;
                                    empupd.Bankifscno= tot.employee.Bankifscno;
                                    empupd.BankName= tot.employee.BankName;
                                    empupd.BankBranch= tot.employee.BankBranch;
                                    empupd.Pic= tot.employee.Pic;
                                    empupd.Esi= tot.employee.Esi;
                                    empupd.Pf= tot.employee.Pf;

                                    var updidentifications = db.HrdEmployeeIdentifications.Where(a => a.RecordId == tot.employee.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if(updidentifications.Count() > 0)
                                    {
                                        db.HrdEmployeeIdentifications.RemoveRange(updidentifications);
                                    }
                                    var updallowdeducs = db.HrdEmployeeAllowancesDeductions.Where(a => a.RecordId == tot.employee.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if (updallowdeducs.Count() > 0)
                                    {
                                        db.HrdEmployeeAllowancesDeductions.RemoveRange(updallowdeducs);
                                    }
                                    var updleaves = db.HrdEmployeeLeaves.Where(a => a.RecordId == tot.employee.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if (updleaves.Count() > 0)
                                    {
                                        db.HrdEmployeeLeaves.RemoveRange(updleaves);
                                    }
                                    var updcurriculs = db.HrdEmployeeCurriculum.Where(a => a.RecordId == tot.employee.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if (updcurriculs.Count() > 0)
                                    {
                                        db.HrdEmployeeCurriculum.RemoveRange(updcurriculs);
                                    }
                                    var updexperiences = db.HrdEmployeeExperience.Where(a => a.RecordId == tot.employee.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if (updexperiences.Count() > 0)
                                    {
                                        db.HrdEmployeeExperience.RemoveRange(updexperiences);
                                    }
                                    var upfamilies = db.HrdEmployeeFamilyDetails.Where(a => a.RecordId == tot.employee.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if (upfamilies.Count() > 0)
                                    {
                                        db.HrdEmployeeFamilyDetails.RemoveRange(upfamilies);
                                    }

                                    foreach (var det in tot.identifications)
                                    {
                                        det.RecordId = tot.employee.RecordId;
                                        det.BranchId = tot.usr.bCode;
                                        det.CustomerCode = tot.usr.cCode;
                                    }
                                    foreach (var det in tot.allowances)
                                    {
                                        allows.Add(
                                           new HrdEmployeeAllowancesDeductions
                                           {
                                               RecordId = tot.employee.RecordId,
                                               Allowance = det.allowanceId,
                                               Valu = det.valu,
                                               BranchId = tot.usr.bCode,
                                               CustomerCode = tot.usr.cCode
                                           });
                                    }
                                    foreach (var det in tot.leaves)
                                    {
                                        leaves.Add(new HrdEmployeeLeaves
                                        {
                                            RecordId = tot.employee.RecordId,
                                            LeaveId = det.leaveId,
                                            Valu = det.valu,
                                            BranchId = tot.usr.bCode,
                                            CustomerCode = tot.usr.cCode
                                        });
                                    }
                                    foreach (var det in tot.curriculum)
                                    {
                                        det.RecordId = tot.employee.RecordId;
                                        det.BranchId = tot.usr.bCode;
                                        det.CustomerCode = tot.usr.cCode;
                                    }
                                    foreach (var det in tot.experience)
                                    {
                                        det.RecordId = tot.employee.RecordId;
                                        det.BranchId = tot.usr.bCode;
                                        det.CustomerCode = tot.usr.cCode;
                                    }
                                    foreach (var det in tot.family)
                                    {
                                        det.RecordId = tot.employee.RecordId;
                                        det.BranchId = tot.usr.bCode;
                                        det.CustomerCode = tot.usr.cCode;
                                    }
                                    if (tot.identifications.Count() > 0)
                                    {
                                        db.HrdEmployeeIdentifications.AddRange(tot.identifications);
                                    }
                                    if (allows.Count() > 0)
                                    {
                                        db.HrdEmployeeAllowancesDeductions.AddRange(allows);
                                    }
                                    if (leaves.Count() > 0)
                                    {
                                        db.HrdEmployeeLeaves.AddRange(leaves);
                                    }
                                    if (tot.curriculum.Count() > 0)
                                    {
                                        db.HrdEmployeeCurriculum.AddRange(tot.curriculum);
                                    }
                                    if (tot.experience.Count() > 0)
                                    {
                                        db.HrdEmployeeExperience.AddRange(tot.experience);
                                    }
                                    if (tot.family.Count() > 0)
                                    {
                                        db.HrdEmployeeFamilyDetails.AddRange(tot.family);
                                    }
                                    db.SaveChanges();
                                    string fn = "EMP_PIC_" + tot.employee.RecordId + "_" + tot.usr.bCode + "_" + tot.usr.cCode + ".jpg";
                                    var imgresult = ggg.imageUploadGeneric(tot.imgs, ho.WebRootPath + "\\Attachments\\" + "hrd\\" + fn);


                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "No record found to update";
                                }
                                break;
                            case 3:
                                var empdel = db.HrdEmployees.Where(a => a.RecordId == tot.employee.RecordId && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                int delchk = 0;
                                if (empdel != null)
                                {
                                    delchk = 1;

                                }
                                if (delchk == 1)
                                {

                                    db.HrdEmployees.Remove(empdel);

                                    var updidentifications = db.HrdEmployeeIdentifications.Where(a => a.RecordId == tot.employee.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if (updidentifications.Count() > 0)
                                    {
                                        db.HrdEmployeeIdentifications.RemoveRange(updidentifications);
                                    }
                                    var updallowdeducs = db.HrdEmployeeAllowancesDeductions.Where(a => a.RecordId == tot.employee.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if (updallowdeducs.Count() > 0)
                                    {
                                        db.HrdEmployeeAllowancesDeductions.RemoveRange(updallowdeducs);
                                    }
                                    var updleaves = db.HrdEmployeeLeaves.Where(a => a.RecordId == tot.employee.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if (updallowdeducs.Count() > 0)
                                    {
                                        db.HrdEmployeeLeaves.RemoveRange(updleaves);
                                    }
                                    var updcurriculs = db.HrdEmployeeCurriculum.Where(a => a.RecordId == tot.employee.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if (updallowdeducs.Count() > 0)
                                    {
                                        db.HrdEmployeeCurriculum.RemoveRange(updcurriculs);
                                    }
                                    var updexperiences = db.HrdEmployeeExperience.Where(a => a.RecordId == tot.employee.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if (updallowdeducs.Count() > 0)
                                    {
                                        db.HrdEmployeeExperience.RemoveRange(updexperiences);
                                    }
                                    var upfamilies = db.HrdEmployeeFamilyDetails.Where(a => a.RecordId == tot.employee.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if (updallowdeducs.Count() > 0)
                                    {
                                        db.HrdEmployeeFamilyDetails.RemoveRange(upfamilies);
                                    }
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
                            catch (Exception ex)
                            {
                                msg = ex.Message;
                            }
                            
                      
                    
                }
                else
                {
                    msg = "This employee number is already existed";
                }
            }
            else
            {
                msg = "You are not authorised for this transaction";
            }
            TransactionResult res = new TransactionResult();
                res.result = msg;
            return res;
        }

    }
}
