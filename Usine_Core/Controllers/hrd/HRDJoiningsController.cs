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
using System.Data;
using System.Data.SqlClient;
using Usine_Core.Controllers.HRD;

namespace Usine_Core.Controllers.hrd
{
    public class HRDJoiningsTotal
    {
        public string empno { get; set; }
        public HrdInterviewCandidates candidate { get; set; }
        public UserInfo usr { get; set; }
    }
    public class HRDJoiningsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Route("api/HRDJoinings/GetHRDJoiningsList")]
        [Authorize]
        public dynamic GetHRDJoiningsList([FromBody] UserInfo usr)
        {
            var det= (from a in db.HrdInterviewCandidates.Where(a => a.AppointmentStatus == 1 && a.JoiningStatus == null && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                       join b in db.HrdResumeUni.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on a.ResumeId equals b.RecordId
                       join c in db.HrdDesignations.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on b.Designation equals c.RecordId
                       select new
                       {
                           a.RecordId,
                           a.Sno,
                           a.ResumeId,
                           b.Seq,
                           b.NameOfCandidate,
                           b.FatherName,
                           b.Mobile,
                           b.ExpectedSalary,
                           c.Designation,
                           a.MaxDateToJoin,
                           a.AppointmentComments,
                           chk=a.MaxDateToJoin >= ac.getPresentDateTime()?1:0
                       }).OrderBy(b => b.RecordId).ToList();
            return det;

        }

        [HttpPost]
        [Route("api/HRDJoinings/SetJoining")]
        [Authorize]
        public TransactionResult SetJoining([FromBody] HRDJoiningsTotal tot)
        {
            string msg = "";
            try
            {
                if (ac.screenCheck(tot.usr, 8, 2, 4, 0))
                {
                    if (duplicateCheck(tot.empno, tot.usr))
                    {
                        var det = db.HrdInterviewCandidates.Where(a => a.ResumeId == tot.candidate.ResumeId && a.RecordId == tot.candidate.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                        det.JoiningDate = ac.getPresentDateTime();
                        det.JoiningStatus = 1;
                        det.JoiningComments = tot.candidate.JoiningComments;

                        var resume = db.HrdResumeUni.Where(a => a.RecordId == tot.candidate.ResumeId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                        var resumecurriculum = db.HrdResumeCurriculum.Where(a => a.RecordId == tot.candidate.ResumeId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).OrderBy(b => b.Sno).ToList();
                        var resumeexperience = db.HrdResumeExperience.Where(a => a.RecordId == tot.candidate.ResumeId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).OrderBy(b => b.Sno).ToList();
                        var department = db.HrdDesignations.Where(a => a.RecordId == resume.Designation && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).Select(b => b.Department).FirstOrDefault();

                        HrdEmployees emp = new HrdEmployees();
                        emp.Empno = tot.empno;
                        emp.Empname = resume.NameOfCandidate;
                        emp.Surname = resume.SurName;
                        emp.Fathername = resume.FatherName;
                        emp.Gender = resume.Gender;
                        emp.Dob = resume.Dob;
                        emp.ModeofPay = 3;
                        emp.MaritalStatus = resume.MaritalStatus;
                        emp.Address = resume.Addr;
                        emp.Country = resume.Country;
                        emp.Stat = resume.Stat;
                        emp.District = resume.District;
                        emp.City = resume.City;
                        emp.Zip = resume.Zip;
                        emp.Mobile = resume.Mobile;
                        emp.Tel = resume.Tel;
                        emp.Aadhar = resume.Fax;
                        emp.Email = resume.Email;
                        emp.Designation = resume.Designation;
                        emp.Department = department;
                        emp.Doj = ac.getPresentDateTime();
                        emp.BasicPay = resume.ExpectedSalary;
                        emp.Branchid = tot.usr.bCode;
                        emp.CustomerCode = tot.usr.cCode;
                        List<HrdEmployeeCurriculum> empcurriculum = new List<HrdEmployeeCurriculum>();
                        foreach (var lst in resumecurriculum)
                        {
                            empcurriculum.Add(new HrdEmployeeCurriculum
                            {
                                Frmyear = lst.FromYear,
                                Toyear = lst.ToYead,
                                Qual = lst.Qualification,
                                Board = lst.Board
                            });
                        }
                        List<HrdEmployeeExperience> empexperience = new List<HrdEmployeeExperience>();
                        foreach (var lst in resumeexperience)
                        {
                            empexperience.Add(new HrdEmployeeExperience
                            {
                                Frmyear = lst.FromYear,
                                Toyear = lst.ToYead,
                                Designation = lst.Designation,
                                Organisation = lst.Organisation
                            });
                        }

                        HRDEmployeeTotal emptot = new HRDEmployeeTotal();
                        emptot.usr = tot.usr;
                        emptot.employee = emp;
                        emptot.curriculum = empcurriculum;
                        emptot.experience = empexperience;
                        makeEmployee(emptot);
                        db.SaveChanges();
                        msg = "OK";
                    }
                    else
                    {
                        msg = "This employee numner is already existed";
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
        private Boolean duplicateCheck(string empno,UserInfo usr)
        {
            UsineContext db = new UsineContext();
            var det = db.HrdEmployees.Where(a => a.Empno == empno && a.Branchid == usr.bCode && a.CustomerCode == usr.cCode).FirstOrDefault();
            if(det==null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void makeEmployee(HRDEmployeeTotal tot)
        {
            List<HrdEmployeeAllowancesDeductions> allows = new List<HrdEmployeeAllowancesDeductions>();
            List<HrdEmployeeLeaves> leaves = new List<HrdEmployeeLeaves>();
            using (var txn = db.Database.BeginTransaction())
            {
                try
                {

                    tot.employee.Branchid = tot.usr.bCode;
                    tot.employee.CustomerCode = tot.usr.cCode;
                    db.HrdEmployees.Add(tot.employee);

                    db.SaveChanges();

                   
                 
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
                   
                    
                   
                    if (tot.curriculum.Count() > 0)
                    {
                        db.HrdEmployeeCurriculum.AddRange(tot.curriculum);
                    }
                    if (tot.experience.Count() > 0)
                    {
                        db.HrdEmployeeExperience.AddRange(tot.experience);
                    }
                    
                    db.SaveChanges();
                    txn.Commit();

                    db.SaveChanges();
                    

                }
                catch (Exception ex)
                {
                    
                    txn.Rollback();
                }

            }
        }
    }
}
