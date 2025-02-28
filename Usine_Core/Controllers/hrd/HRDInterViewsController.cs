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

    public class HRDInterviewTotal
    {
        public HrdInterviewsUni header { get; set; }
        public List<HrdInterviewsPanel> panel { get; set; }
        public List<HrdInterviewCandidates> candidates { get; set; }
        public int? traCheck { get; set; }
        public UserInfo usr { get; set; }
    }
    public class HRDInterviewInformation
    {
        public HrdInterviewsUni header { get; set; }
        public dynamic candidates { get; set; }
        public dynamic panel { get; set; }
        public int? chk { get; set; }
    }
   

    public class HRDInterViewsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Route("api/HRDInterViews/GetHRDInterviewsList")]
        [Authorize]
        public dynamic GetHRDInterviewsList([FromBody] UserInfo usr)
        {
            var det1 = db.HrdInterviewsUni.Where(a => a.Pos == 1 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
            var det2 = (from a in db.HrdDesignations.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                        join b in db.HrdDepartments.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode) on a.Department equals b.RecordId
                        select new
                        {
                            a.RecordId,
                            a.Designation,
                            b.SGrp
                        }).ToList();
             return (from a in det1
                         join b in det2 on a.Designation equals b.RecordId
                         select new
                         {
                             a.RecordId,
                             a.Seq,
                             a.Dat,
                             a.InterviewDate,
                             a.Venue,
                             a.Pos,
                             b.Designation,
                             department = b.SGrp
                         }).OrderBy(b => b.RecordId).ToList();
        }
        [HttpPost]
        [Route("api/HRDInterViews/GetHRDPendingResume")]
        [Authorize]
        public List<HrdResumeUni> GetHRDPendingResume([FromBody] GeneralInformation inf)
        {
            DataBaseContext g = new DataBaseContext();
            General gg = new General();
            string quer = "";
            quer = quer + " select * from HrdResumeUni where designation=" + inf.recordId + " and branchid='" + inf.usr.bCode + "' and customercode=" + inf.usr.cCode;
            quer = quer + " and recordid not in (select resumeid from HrdInterviewCandidates where  resumeid is not null and branchid = '" + inf.usr.bCode + "' and customerCode=" + inf.usr.cCode + ")";
            SqlCommand dc = new SqlCommand();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            List<HrdResumeUni> resumes = new List<HrdResumeUni>();
            while(dr.Read())
            {
                resumes.Add(new HrdResumeUni
                {
                    RecordId=gg.valInt(dr[0].ToString()),
                    Seq=dr[1].ToString(),
                    Dat=DateTime.Parse(dr[2].ToString()),
                    AppDate=DateTime.Parse(dr[3].ToString()),
                    NameOfCandidate=dr[4].ToString() + " " + dr[5].ToString(),
                    FatherName=dr[6].ToString(),
                    Dob=DateTime.Parse(dr[7].ToString()),
                    Gender=gg.valInt(dr[8].ToString()),
                    ExpectedSalary=gg.valNum(dr[12].ToString()),
                    City=dr[17].ToString(),
                    Mobile=dr[19].ToString(),
                    Email=dr[22].ToString()

                });
                
            }
            dr.Close();
            g.db.Close();
            return resumes.OrderBy(a => a.RecordId).ToList();
        }
        private Boolean dupChk(GeneralInformation inf)
        {
            UsineContext db = new UsineContext();
            var det = db.HrdInterviewCandidates.Where(a => a.RecordId == inf.recordId && a.AppointmentStatus != null && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            if(det==null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        [Route("api/HRDInterViews/GetHRDInterviewInformation")]
        [Authorize]
        public HRDInterviewInformation GetHRDInterviewInformation([FromBody] GeneralInformation inf)
        {
            HRDInterviewInformation tot = new HRDInterviewInformation();
            if(dupChk(inf))
            {
                tot.header = db.HrdInterviewsUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                tot.candidates = (from a in db.HrdInterviewCandidates.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                  join b in db.HrdResumeUni.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.ResumeId equals b.RecordId
                                  select new
                                  {
                                      RecordId = a.ResumeId,
                                      Seq = b.Seq,
                                      Dat = b.Dat,
                                      AppDate = b.AppDate,
                                      NameOfCandidate = b.NameOfCandidate + " " + b.SurName,
                                      FatherName = b.FatherName,
                                      Dob = b.Dob,
                                      Gender = b.Gender,
                                      ExpectedSalary = b.ExpectedSalary,
                                      City = b.City,
                                      Mobile = b.Mobile,
                                      Email = b.Email
                                  }).ToList();
                tot.panel = (from a in db.HrdInterviewsPanel.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                             join b in db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.Empno equals b.RecordId
                             join c in db.HrdDepartments.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on b.Department equals c.RecordId
                             join d in db.HrdDesignations.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on b.Designation equals d.RecordId
                             select new
                             {
                                 recordId = b.RecordId,
                                 empno = b.Empno,
                                 empname = b.Empname,
                                 designation = d.Designation,
                                 department = c.SGrp,
                                 mobile = b.Mobile
                             }).ToList();
                tot.chk = 1;
            }
            else
            {
                tot.chk = 0;
            }

            return tot;
        }
        [HttpPost]
        [Route("api/HRDInterViews/setHRInterView")]
        [Authorize]
        public TransactionResult setHRInterView([FromBody] HRDInterviewTotal tot)
        {
            string msg = "";
            try
            {
                if(ac.screenCheck(tot.usr,8,2,2,(int)tot.traCheck))
                {
                    switch(tot.traCheck)
                    {
                        case 1:
                            using (var txn = db.Database.BeginTransaction())
                            {
                                try
                                {
                                    tot.header.Seq = findSeq(tot.usr);
                                    tot.header.Pos = 1;
                                    tot.header.Dat = ac.getPresentDateTime();
                                    tot.header.BranchId = tot.usr.bCode;
                                    tot.header.CustomerCode = tot.usr.cCode;
                                    db.HrdInterviewsUni.Add(tot.header);
                                    db.SaveChanges();
                                    int? sno = 1;
                                    foreach(var res in tot.candidates)
                                    {
                                        res.RecordId = tot.header.RecordId;
                                        res.Sno = sno;
                                        res.BranchId = tot.usr.bCode;
                                        res.CustomerCode = tot.usr.cCode;
                                        sno++;
                                    }
                                    if(tot.candidates.Count() > 0)
                                    {
                                        db.HrdInterviewCandidates.AddRange(tot.candidates);
                                    }
                                    sno = 1;
                                    foreach(var emp in tot.panel)
                                    {
                                        emp.RecordId = tot.header.RecordId;
                                        emp.Sno = sno;
                                        emp.BranchId = tot.usr.bCode;
                                        emp.CustomerCode = tot.usr.cCode;
                                        sno++;
                                    }
                                    if(tot.panel.Count() > 0)
                                    {
                                        db.HrdInterviewsPanel.AddRange(tot.panel);
                                    }
                                    db.SaveChanges();
                                     txn.Commit();
                                    msg = "OK";
                                }
                                catch(Exception ee)
                                {
                                    txn.Rollback();
                                    msg = ee.Message;
                                }
                            }
                                break;
                        case 2:
                            var headerupd = db.HrdInterviewsUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            if(headerupd != null)
                            {
                                headerupd.InterviewDate = tot.header.InterviewDate;
                                headerupd.Venue = tot.header.Venue;
                                headerupd.Descriptio = tot.header.Descriptio;
                                headerupd.Designation = tot.header.Designation;
                            }
                            var snoupd = db.HrdInterviewCandidates.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).Max(b => b.Sno);
                            if(snoupd == null)
                            {
                                snoupd = 0;
                            }
                            snoupd++;
                            var linecandidatesupd = db.HrdInterviewCandidates.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                            if(linecandidatesupd.Count() > 0)
                            {
                                db.HrdInterviewCandidates.RemoveRange(linecandidatesupd);
                            }
                            foreach (var res in tot.candidates)
                            {
                                res.RecordId = tot.header.RecordId;
                                res.Sno = snoupd;
                                res.BranchId = tot.usr.bCode;
                                res.CustomerCode = tot.usr.cCode;
                                snoupd++;
                            }
                            if (tot.candidates.Count() > 0)
                            {
                                db.HrdInterviewCandidates.AddRange(tot.candidates);
                            }
                            snoupd = db.HrdInterviewsPanel.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).Max(b => b.Sno);
                            if (snoupd == null)
                            {
                                snoupd = 0;
                            }
                            snoupd++;
                            var linepanelupd = db.HrdInterviewsPanel.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                            if(linepanelupd.Count() > 0)
                            {
                                db.HrdInterviewsPanel.RemoveRange(linepanelupd);
                            }
                            foreach (var emp in tot.panel)
                            {
                                emp.RecordId = tot.header.RecordId;
                                emp.Sno = snoupd;
                                emp.BranchId = tot.usr.bCode;
                                emp.CustomerCode = tot.usr.cCode;
                                snoupd++;
                            }
                            if (tot.panel.Count() > 0)
                            {
                                db.HrdInterviewsPanel.AddRange(tot.panel);
                            }
                            db.SaveChanges();
                            msg = "OK";
                            break;
                        case 3:
                            var headerdel = db.HrdInterviewsUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            var linecandidatesdel = db.HrdInterviewCandidates.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                            var linepaneldel = db.HrdInterviewsPanel.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();

                            headerdel.BranchId = headerdel.BranchId + "CANCEL";
                            foreach(var cand in linecandidatesdel)
                            {
                                cand.BranchId=cand.BranchId + "CANCEL";
                            }
                            foreach(var pane in linepaneldel)
                            {
                                pane.BranchId = pane.BranchId + "CANCEL";
                            }
                            db.SaveChanges();
                            msg = "OK";
                            break;
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
        private string findSeq(UserInfo usr)
        {
            
            var det = db.HrdInterviewsUni.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.Seq);
            int x = 0;

            General gg = new General();
            if(det!=null)
            {
                x = gg.valInt(gg.right(det, 6));
            }
            x++;
            return  "INT-"+  gg.zeroMake(x, 6);
        }


    }
}
