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

namespace Usine_Core.Controllers.hrd
{
     public class HRDResumeTotal
    {
        public HrdResumeUni header { get; set; }
        public List<HrdResumeCurriculum> curriculum { get; set; }
        public List<HrdResumeExperience> experience { get; set; }
        public List<FileUploadAttribute> imgs { get; set; }
        public int? traCheck { get; set; }
        public UserInfo usr { get; set; }
    }
    public class HRDResumeController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        private readonly IHostingEnvironment ho;
        public HRDResumeController(IHostingEnvironment hostingEnvironment)
        {
            ho = hostingEnvironment;
        }

        [HttpPost]
        [Route("api/HRDResume/GetHRDResumeList")]
        [Authorize]
        public List<HrdResumeUni> GetHRDResumeList([FromBody] GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);

            return (from a in db.HrdResumeUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                    join b in db.HrdDesignations.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.Designation equals b.RecordId
                    select new HrdResumeUni
                    {
                        RecordId = a.RecordId,
                        Seq=a.Seq,
                        NameOfCandidate=a.NameOfCandidate,
                        Dat=a.Dat,
                        Mobile=a.Mobile,
                        Email=a.Email,
                        Zip=b.Designation

                    }).OrderBy(c => c.Dat).ThenBy(d => d.RecordId).ToList();
        }

        [HttpPost]
        [Route("api/HRDResume/GetHRDResume")]
        [Authorize]
        public HRDResumeTotal GetHRDResume([FromBody] GeneralInformation inf)
        {
            HRDResumeTotal tot = new HRDResumeTotal();
            if (dupResumeCheck(inf))
            {
                tot.header = db.HrdResumeUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                tot.curriculum = (from a in db.HrdResumeCurriculum.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                  select new HrdResumeCurriculum
                                  {
                                      RecordId = a.RecordId,
                                      Sno = a.Sno,
                                      FromYear = a.FromYear,
                                      ToYead = a.ToYead,
                                      Qualification = a.Qualification,
                                      Board = a.Board
                                  }).OrderBy(b => b.Sno).ToList();

                tot.experience = (from a in db.HrdResumeExperience.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                  select new HrdResumeExperience
                                  {
                                      RecordId = a.RecordId,
                                      Sno = a.Sno,
                                      FromYear = a.FromYear,
                                      ToYead = a.ToYead,
                                      Designation = a.Designation,
                                      Organisation = a.Organisation
                                  }).OrderBy(b => b.Sno).ToList();
                tot.traCheck = 1;
            }
            else
            {
                tot.traCheck = 0;
            }
            return tot;
        }
        private Boolean dupResumeCheck(GeneralInformation inf)
        {
            Boolean b = true;
            UsineContext db1 = new UsineContext();
            var detail = db1.HrdInterviewCandidates.Where(a => a.ResumeId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            if(detail==null)
            {
                b = true;
            }
            else
            {
                b = false;
            }
            return b;
        }

        [HttpPost]
        [Route("api/HRDResume/SetHRDResume")]
        [Authorize]
        public TransactionResult SetHRDResume([FromBody] HRDResumeTotal tot)
        {
            string msg = "";
            DataBaseContext ggg = new DataBaseContext();

            try
            {
                if(ac.screenCheck(tot.usr,8,2,1,(int)tot.traCheck))
                {
                    switch(tot.traCheck)
                    {
                        case 1:
                            using (var txn = db.Database.BeginTransaction())
                            {
                                try
                                {
                                    tot.header.Seq = findSeq(tot.usr);
                                    tot.header.Dat = ac.getPresentDateTime();
                                    tot.header.BranchId = tot.usr.bCode;
                                    tot.header.CustomerCode = tot.usr.cCode;
                                    db.HrdResumeUni.Add(tot.header);
                                    db.SaveChanges();

                                    int? sno1 = 1;
                                    foreach(var cur in tot.curriculum)
                                    {
                                        cur.RecordId = tot.header.RecordId;
                                        cur.Sno = sno1;
                                        cur.BranchId = tot.usr.bCode;
                                        cur.CustomerCode = tot.usr.cCode;
                                        sno1++;
                                    }
                                    if(tot.curriculum.Count() > 0)
                                    {
                                        db.HrdResumeCurriculum.AddRange(tot.curriculum);
                                    }
                                    sno1 = 1;
                                    foreach(var exp in tot.experience)
                                    {
                                        exp.RecordId = tot.header.RecordId;
                                        exp.Sno = sno1;
                                        exp.BranchId = tot.usr.bCode;
                                        exp.CustomerCode = tot.usr.cCode;

                                        sno1++;
                                    }
                                    if(tot.experience.Count() > 0)
                                    {
                                        db.HrdResumeExperience.AddRange(tot.experience);
                                    }

                                    TransactionsAudit aud1 = new TransactionsAudit();
                                    aud1.TraId = tot.header.RecordId;
                                    aud1.Descr = "A Resume of Seq " + tot.header.Seq + " has been created";
                                    aud1.Usr = tot.usr.uCode;
                                    aud1.Tratype = 1;
                                    aud1.Transact = "HRD_RES";
                                    aud1.TraModule = "RESUME";
                                    aud1.Syscode = " ";
                                    aud1.BranchId = tot.usr.bCode;
                                    aud1.CustomerCode = tot.usr.cCode;
                                    aud1.Dat = ac.getPresentDateTime();
                                    db.TransactionsAudit.Add(aud1);

                                    db.SaveChanges();



                                    txn.Commit();
                                     string fn = "RES_PIC" + tot.header.RecordId + "_" + tot.usr.bCode + "_" + tot.usr.cCode + ".jpg";
                                    var imgresult = ggg.imageUploadGeneric(tot.imgs, ho.WebRootPath + "\\Attachments\\" + "hrd\\" + fn);

                                    msg = "OK";
                                }
                                catch (Exception ee)
                                {
                                    txn.Rollback();
                                    msg = ee.Message;
                                }
                            }
                                    break;
                        case 2:
                            var headerupd = db.HrdResumeUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            if(headerupd != null)
                            {
                                headerupd.AppDate = tot.header.AppDate;
                                headerupd.NameOfCandidate = tot.header.NameOfCandidate;
                                headerupd.SurName = tot.header.SurName;
                                headerupd.FatherName = tot.header.FatherName;
                                headerupd.Dob = tot.header.Dob;
                                headerupd.Gender = tot.header.Gender;
                                headerupd.MaritalStatus = tot.header.MaritalStatus;
                                headerupd.Reference = tot.header.Reference;
                                headerupd.Designation = tot.header.Designation;
                                headerupd.ExpectedSalary = tot.header.ExpectedSalary;
                                headerupd.Addr = tot.header.Addr;
                                headerupd.Country = tot.header.Country;
                                headerupd.Stat = tot.header.Stat;
                                headerupd.District = tot.header.District;
                                headerupd.City = tot.header.City;
                                headerupd.Zip = tot.header.Zip;
                                headerupd.Mobile = tot.header.Mobile;
                                headerupd.Tel = tot.header.Tel;
                                headerupd.Fax = tot.header.Fax;
                                headerupd.Email = tot.header.Email;
                                headerupd.PermenentId = tot.header.PermenentId;


                                var currUpd = db.HrdResumeCurriculum.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                int? snoupd = 0;
                                if(currUpd.Count() > 0)
                                {
                                    snoupd = currUpd.Max(a => a.Sno);
                                }
                                snoupd++;
                                foreach(var curupd in tot.curriculum)
                                {
                                    curupd.RecordId = tot.header.RecordId;
                                    curupd.Sno = snoupd;
                                    curupd.BranchId = tot.usr.bCode;
                                    curupd.CustomerCode = tot.usr.cCode;
                                    snoupd++;
                                }
                                if (currUpd.Count() > 0)
                                {
                                    db.HrdResumeCurriculum.RemoveRange(currUpd);
                                }
                                if (tot.curriculum.Count() > 0)
                                {
                                    db.HrdResumeCurriculum.AddRange(tot.curriculum);
                                }

                                var expUpd = db.HrdResumeExperience.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                snoupd = 0;
                                if (expUpd.Count() > 0)
                                {
                                    snoupd = expUpd.Max(a => a.Sno);
                                }
                                snoupd++;
                                foreach (var expupd in tot.experience)
                                {
                                    expupd.RecordId = tot.header.RecordId;
                                    expupd.Sno = snoupd;
                                    expupd.BranchId = tot.usr.bCode;
                                    expupd.CustomerCode = tot.usr.cCode;
                                    snoupd++;
                                }
                                if (expUpd.Count() > 0)
                                {
                                    db.HrdResumeExperience.RemoveRange(expUpd);
                                }
                                if (tot.experience.Count() > 0)
                                {
                                    db.HrdResumeExperience.AddRange(tot.experience);
                                }

                                string fn = "RES_PIC" + tot.header.RecordId + "_" + tot.usr.bCode + "_" + tot.usr.cCode + ".jpg";
                                string imgresult1 = "";
                                if(tot.imgs != null)
                                 imgresult1 = ggg.imageUploadGeneric(tot.imgs, ho.WebRootPath + "\\Attachments\\" + "hrd\\" + fn);


                                TransactionsAudit aud2 = new TransactionsAudit();
                                aud2.TraId = tot.header.RecordId;
                                aud2.Descr = "A Resume of Seq " + tot.header.Seq + " has been modified";
                                aud2.Usr = tot.usr.uCode;
                                aud2.Tratype = 2;
                                aud2.Transact = "HRD_RES";
                                aud2.TraModule = "RESUME";
                                aud2.Syscode = " ";
                                aud2.BranchId = tot.usr.bCode;
                                aud2.CustomerCode = tot.usr.cCode;
                                aud2.Dat = ac.getPresentDateTime();
                                db.TransactionsAudit.Add(aud2);

                                db.SaveChanges();

                                msg = "OK";
                            }
                            else
                            {
                                msg = "No record found";
                            }
                            break;
                        case 3:
                            var headerDel = db.HrdResumeUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            var currDel = db.HrdResumeCurriculum.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                            var expDel = db.HrdResumeExperience.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                            if (headerDel != null)
                            {
                                if (currDel.Count() > 0)
                                {
                                    db.HrdResumeCurriculum.RemoveRange(currDel);
                                }
                                if (expDel.Count() > 0)
                                {
                                    db.HrdResumeExperience.RemoveRange(expDel);
                                }
                                db.HrdResumeUni.Remove(headerDel);
                                TransactionsAudit aud3 = new TransactionsAudit();
                                aud3.TraId = tot.header.RecordId;
                                aud3.Descr = "A Resume of Seq " + tot.header.Seq + " has been deleted";
                                aud3.Usr = tot.usr.uCode;
                                aud3.Tratype = 3;
                                aud3.Transact = "HRD_RES";
                                aud3.TraModule = "RESUME";
                                aud3.Syscode = " ";
                                aud3.BranchId = tot.usr.bCode;
                                aud3.CustomerCode = tot.usr.cCode;
                                aud3.Dat = ac.getPresentDateTime();
                                db.TransactionsAudit.Add(aud3);

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
            int? x = 0;
            General gg = new General();
            var str = "RES_" + ac.getPresentDateTime().Year.ToString() + "-";
            var detail = db.HrdResumeUni.Where(a => a.Seq.Contains(str) && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.Seq);
            if(detail != null)
            {
                x = gg.valInt(gg.right(detail, 5));
            }
            x++;
            return str + gg.zeroMake((int)x, 5);
        }
    }
}
