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

namespace Usine_Core.Controllers.hrd
{
    public class HRDAppintmentTotalInformation
    {
        public List<HrdInterviewsUni> header { get; set; }
        public dynamic employees { get; set; }
        public dynamic candidates { get; set; }
    }
    public class HRDAppintmentsTotal
    {
        public  HrdInterviewsUni  header { get; set; }
        public List<HrdInterviewCandidates> candidates { get; set; }
         public UserInfo usr { get; set; }
    }
    public class HRDAppointmentsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        [HttpPost]
        [Route("api/HRDAppointments/GetHRDAppintmentList")]
        [Authorize]
        public HRDAppintmentTotalInformation GetHRDAppintmentList([FromBody] UserInfo usr)
        {
          
            HRDAppintmentTotalInformation tot = new HRDAppintmentTotalInformation();
            tot.header = (from a in db.HrdInterviewsUni.Where(a => a.Pos == 1 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                          join b in db.HrdDesignations.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on a.Designation equals b.RecordId
                          select new HrdInterviewsUni
                          {
                              RecordId = a.RecordId,
                              Seq = a.Seq,
                              Dat = a.Dat,
                              InterviewDate = a.InterviewDate,
                              Venue = a.Venue,
                              Descriptio = a.Descriptio,
                              Designation = a.Designation,
                              BranchId = b.Designation
                          }).OrderBy(b => b.RecordId).ToList();
            tot.candidates = (from a in db.HrdInterviewsUni.Where(a => a.Pos == 1 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                              join b in db.HrdInterviewCandidates.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on a.RecordId equals b.RecordId
                              join c in db.HrdResumeUni.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on b.ResumeId equals c.RecordId
                              select new
                              {
                                  recordid = a.RecordId,
                                  seq=a.Seq,
                                  resumeid=c.RecordId,
                                  resumeseq=c.Seq,
                                  nameofcandidate=c.NameOfCandidate,
                                  gender=c.Gender==1?"Male":"Female",
                                  mobile=c.Mobile,
                                  email=c.Email,
                                  expectedsalary=c.ExpectedSalary

                              }).OrderBy(b => b.recordid).ToList();
            tot.employees = (from a in db.HrdInterviewsUni.Where(a => a.Pos == 1 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                             join b in db.HrdInterviewsPanel.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on a.RecordId equals b.RecordId
                             join c in db.HrdEmployees.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode) on b.Empno equals c.RecordId
                             join d in db.HrdDesignations.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on c.Designation equals d.RecordId
                             select new
                             {
                                 recordid = a.RecordId,
                                 seq = a.Seq,
                                 empno = b.Empno,
                                 empname = c.Empname,
                                 designation = d.Designation
                             }).OrderBy(e => e.recordid).ToList();

            return tot;
        }
        [HttpPost]
        [Route("api/HRDAppointments/SetHrdAppintment")]
        [Authorize]
        public TransactionResult SetHrdAppintment([FromBody] HRDAppintmentsTotal tot)
        {
            string msg = "";
            if(ac.screenCheck(tot.usr,8,2,3,0))
            {
                try
                {
                    var header = db.HrdInterviewsUni.Where(a => a.RecordId == tot.header.RecordId).FirstOrDefault();
                    header.Pos = 2;
                    var lines = db.HrdInterviewCandidates.Where(a => a.RecordId == tot.header.RecordId).ToList();
                    foreach(var line in lines)
                    {
                        //    var ln = db.HrdInterviewCandidates.Where(a => a.RecordId == tot.header.RecordId && a.ResumeId == line.ResumeId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                        var ln = tot.candidates.Where(a => a.ResumeId == line.ResumeId).FirstOrDefault();
                        
                        if(ln != null)
                        {
                            line.AppointmentStatus = ln.AppointmentStatus;
                            line.AppointmentDate = ac.getPresentDateTime();
                            line.MaxDateToJoin = ln.MaxDateToJoin;
                            line.RefEmpNo = ln.RefEmpNo;
                            line.AppointmentComments = ln.AppointmentComments;
                        }
                    }
                    db.SaveChanges();
                    msg = "OK";
                }
                catch(Exception ee)
                {
                    msg = ee.Message;
                }
            }
            else
            {
                msg = "You are not authorised for this trqansaction";
            }
            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;
        }


    }
}
