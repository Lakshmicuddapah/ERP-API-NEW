using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;
using Usine_Core.Controllers.HRD;
using Usine_Core.others;

namespace Usine_Core.Controllers.CRM
{
    public class CRMTeleCallRxTotal
    {
        public CrmTeleCallingRx call { get; set; }
        public int traCheck { get; set; }
        public UserInfo usr { get; set; }
        public string result { get; set; }
    }
    public class CRMPendingsList
    {
        public int? id { get; set; }
        public string seq { get; set; }
        public int? callerId { get; set; }
        public String customer { get; set; }
        public String mobile { get; set; }
        public String mode { get; set; }
        public DateTime dat { get; set; }
        public string customercomments { get; set; }
        public string username { get; set; }
    }
    public class CRMTeleCallRequirements
    {
        public String seq { get; set; }
        public List<Employees> employees { get; set; }
        public List<CRMPendingsList> pendings { get; set; }
    }
    public class CRMTeleCallingRXController : Controller
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();


        [HttpPost]
        [Authorize]
        [Route("api/CRMRx/GetCRMRxTelecallRequirements")]
        public CRMTeleCallRequirements GetCRMRxTelecallRequirements([FromBody] UserInfo usr)
        {
            hrdDepartmentsController hr = new hrdDepartmentsController();
            CRMTeleCallRequirements tot = new CRMTeleCallRequirements();
            tot.seq = findSeq(usr);
            GeneralInformation inf = new GeneralInformation();
            inf.detail = "Sales";
            inf.usr = usr;
            tot.employees = hr.getEmployeesByDepartment(inf);
            GeneralInformation inf1 = new GeneralInformation();
            inf1.recordId = 1;
            inf1.detail = "TELECALL";
            inf1.usr = usr;
            tot.pendings = getPendingCallsList(inf1);
            return tot;
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMRx/GetCRMRxTelecalls")]
        public List<CrmTeleCallingRx> GetCRMRxTelecalls([FromBody]GeneralInformation inf)
        {

            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
            UsineContext db1 = new UsineContext();
            var det= (from a in db.CrmTeleCallingRx.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                    join b in db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                    on a.Callerid equals b.RecordId into gj
                    from subdet in gj.DefaultIfEmpty()
                                           select new CrmTeleCallingRx
                       {
                           RecordId=a.RecordId,
                           Customer=a.Customer,
                           Seq=a.Seq,
                           Email=subdet==null? " ":subdet.Empname,
                           Mobile=a.Mobile,
                           CallPosition=a.CallPosition


                       }).OrderBy(e => e.Seq).ToList();

            return det;
          
            
        }
        [HttpPost]
        [Authorize]
        [Route("api/CRMRx/GetCRMRxTelecallById")]
        public dynamic GetCRMRxTelecallById([FromBody] GeneralInformation inf)
        {

            var recid = (int?)inf.recordId;

            UsineContext db1 = new UsineContext();
            var list1 = db.CrmTeleCallingRx.Where(a => a.RecordId == recid && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
            var list2_01 = (from a in db.CrmTeleCallingRx.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                            join b in db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.Callerid equals b.RecordId
                            select new CrmTeleCallingRx
                            {
                                RecordId = a.RecordId,
                                Seq = a.Seq,
                                Dat = a.Dat,
                                BranchId = "TeleCall",
                                PrevCallMode = "1",
                                Mobile = a.Mobile,
                                Email = b.Empname,
                                Username = a.Username,
                                CustomerComments = a.CustomerComments
                            }
                          ).ToList();
            var list2_02 = (from a in db.CrmEnquiriesRx.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                            join b in db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.Callerid equals b.RecordId
                            select new CrmTeleCallingRx
                            {
                                RecordId = a.RecordId,
                                Seq = a.Seq,
                                Dat = a.Dat,
                                BranchId = "Enquiry",
                                PrevCallMode = "2",
                                Mobile = a.Mobile,
                                Email = b.Empname,
                                Username = a.username,
                                CustomerComments = a.CustomerComments
                            }
                       ).ToList();
             
            var list2 = new List<CrmTeleCallingRx>();
            if(list2_01.Count > 0 && list2_02.Count > 0)
            {
                var list = list2_01.Union(list2_02);
                foreach(var x in list)
                {
                    list2.Add(new CrmTeleCallingRx
                    {
                        RecordId = x.RecordId,
                        Seq = x.Seq,
                        Dat = x.Dat,
                        BranchId = x.BranchId,
                        PrevCallMode = x.PrevCallMode,
                        Mobile = x.Mobile,
                        Email = x.Email,
                        Username = x.Username,
                        CustomerComments = x.CustomerComments
                    });
                }
            }
            else
            {
                if(list2_01.Count > 0)
                {
                    var list = list2_01;
                    foreach (var x in list)
                    {
                        list2.Add(new CrmTeleCallingRx
                        {
                            RecordId = x.RecordId,
                            Seq = x.Seq,
                            Dat = x.Dat,
                            BranchId = x.BranchId,
                            PrevCallMode = x.PrevCallMode,
                            Mobile = x.Mobile,
                            Email = x.Email,
                            Username = x.Username,
                            CustomerComments = x.CustomerComments
                        });
                    }
                }
                if(list2_02.Count > 0)
                {
                    var list = list2_02;
                    foreach (var x in list)
                    {
                        list2.Add(new CrmTeleCallingRx
                        {
                            RecordId = x.RecordId,
                            Seq = x.Seq,
                            Dat = x.Dat,
                            BranchId = x.BranchId,
                            PrevCallMode = x.PrevCallMode,
                            Mobile = x.Mobile,
                            Email = x.Email,
                            Username = x.Username,
                            CustomerComments = x.CustomerComments
                        });
                    }
                }
            }
            var det = (from a in list1
                       join b in list2
                       on new { id = a.PrevcallId, callmode = a.PrevCallMode } equals new { id = b.RecordId, callmode = b.PrevCallMode } into gj
                       from subdet in gj.DefaultIfEmpty()
                       select new
                       {
                           RecordId = a.RecordId,
                           Customer = a.Customer,
                           Seq = a.Seq,
                           Mobile = a.Mobile,
                           Email = a.Email,
                           PrevcallId = subdet == null ? null : subdet.RecordId,
                           prevseq = subdet == null ? null : subdet.Seq,
                           prevDat = subdet == null ? null : subdet.Dat,
                           prevCallMode = subdet == null ? null : subdet.BranchId,
                           prevMobile = subdet == null ? null : subdet.Mobile,
                           prevComments = subdet == null ? null : subdet.CustomerComments,
                           prevemployee=subdet== null ? null : subdet.Email,
                           prevuser=subdet == null ? null : subdet.Username,
                           customerComments = a.CustomerComments,
                           callerCommnets = a.CallerComments,
                           pos = a.CallPosition,
                           nextcalldate = a.NextCallDate,
                           nextcallmode = a.NextCallMode

                       }).FirstOrDefault();

            return det;


        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMRx/SetCRMRXTeleCall")]
        public CRMTeleCallRxTotal SetCRMRXTeleCall([FromBody]CRMTeleCallRxTotal tot)
        {
            String msg = "";
            String res = ac.transactionCheck("CRM", tot.call.Dat, tot.usr);
            StringConversions sc = new StringConversions();
            DateTime dat = ac.getPresentDateTime(); // DateTime.Parse(ac.strDate(tot.call.Dat.Value) + " " + ac.strTime(ac.getPresentDateTime()));
            if (res == "OK")
            {
                try
                {
                    switch (tot.traCheck)
                    {
                        case 1:
                            if (ac.screenCheck(tot.usr, 7, 2, 2, 1))
                            {
                                var usrname = sc.makeStringToAscii(tot.usr.uCode.ToLower());
                                var empp = db.UserCompleteProfile.Where(a => a.UsrName == usrname && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                
                                CrmTeleCallingRx call = new CrmTeleCallingRx();
                                call.Seq = findSeq(tot.usr);
                                call.Dat = dat;
                                 call.Customer = tot.call.Customer;
                                if (empp != null)
                                {
                                    var emp = db.HrdEmployees.Where(a => a.RecordId == empp.EmployeeNo && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                   if(emp != null)
                                    {
                                        call.Callerid = (int?)emp.RecordId;
                                    }
                                }
                                call.Mobile = tot.call.Mobile;
                                call.Email = tot.call.Email;
                                 call.PrevcallId = tot.call.PrevcallId;
                                //call.PrevCallMode = tot.call.PrevCallMode;
                                call.CustomerComments = tot.call.CustomerComments;
                                call.CallerComments = tot.call.CallerComments;
                                call.CallPosition = tot.call.CallPosition;
                                call.NextCallDate = tot.call.NextCallDate;
                                call.NextCallMode = tot.call.NextCallMode;
                                call.Username = tot.usr.uCode;
                                call.BranchId = tot.usr.bCode;
                                call.CustomerCode = tot.usr.cCode;
                                db.CrmTeleCallingRx.Add(call);
                                db.SaveChanges();

                                if (tot.call.PrevcallId !=null)
                                {
                                    if(tot.call.PrevCallMode=="TELECALL")
                                    {
                                        var prevcall = db.CrmTeleCallingRx.Where(a => a.RecordId == tot.call.PrevcallId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                        prevcall.nextCallId = call.RecordId;
                                     //   prevcall.PrevCallMode = "TELECALL";
                                    }
                                    else
                                    {
                                        var prevcall1 = db.CrmEnquiriesRx.Where(a => a.RecordId == tot.call.PrevcallId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                        prevcall1.nextCallId = call.RecordId;
                                      //  prevcall1.PrevCallMode = "TELECALL";
                                    }
                                    db.SaveChanges();
                                }
                                msg = "OK";
                            }
                            else
                            {
                                msg = "You are not authorised for tele calling";
                            }
                            break;
                        case 2:
                            if (ac.screenCheck(tot.usr, 7, 2, 2, 2))
                            {
                                var call = db.CrmTeleCallingRx.Where(a => a.RecordId == tot.call.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (call != null)
                                {
                                    call.Dat = tot.call.Dat;
                                    call.Callerid = tot.call.Callerid;
                                    call.Customer = tot.call.Customer;
                                    call.Mobile = tot.call.Mobile;
                                    call.Email = tot.call.Email;
                                    call.PrevcallId = tot.call.PrevcallId;
                                    call.PrevCallMode = tot.call.PrevCallMode;
                                    call.CustomerComments = tot.call.CustomerComments;
                                    call.CallerComments = tot.call.CallerComments;
                                    call.CallPosition = tot.call.CallPosition;
                                    call.NextCallDate = tot.call.NextCallDate;
                                    call.NextCallMode = tot.call.NextCallMode;
                                    call.Username = tot.usr.uCode;
                                    call.BranchId = tot.usr.bCode;
                                    call.CustomerCode = tot.usr.cCode;
                                    
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "No record found";
                                }
                            }
                            else
                            {
                                msg = "You are not authorised for tele calling";
                            }
                            break;
                        case 3:
                            if (ac.screenCheck(tot.usr, 7, 2, 2, 3))
                            {
                                var call = db.CrmTeleCallingRx.Where(a => a.RecordId == tot.call.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (call != null)
                                {
                                   
                                    db.CrmTeleCallingRx.Remove(call);
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "No record found";
                                }
                            }
                            else
                            {
                                msg = "You are not authorised for tele calling";
                            }
                            break;
                    }

                }
                catch (Exception ee)
                {
                    msg = ee.Message;
                }
            }
            else
            {
                msg = res;
            }

            tot.result = msg;
            return tot;
        }
        

        private String findSeq(UserInfo usr)
        {
            DateTime dat = ac.getPresentDateTime();
            General g = new General();
            String seq = db.CrmTeleCallingRx.Where(a => a.Dat.Value.Year == dat.Year && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.Seq);
            int x = 0;
            if (seq != null)
            {
                x = int.Parse(seq.Substring(5, 7));
            }
            x++;
            return "TC" + dat.Year.ToString().Substring(2, 2) + "-" + g.zeroMake(x, 7);
        }

        public List<CRMPendingsList> getPendingCallsList(GeneralInformation inf)
        {
            List<CRMPendingsList> lst = new List<CRMPendingsList>();
            DateTime dat = ac.getPresentDateTime();
            var det1 = db.CrmTeleCallingRx.Where(a => a.NextCallDate <= dat && a.nextCallId ==null && a.NextCallMode == inf.recordId.ToString() && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
            var det2 = db.CrmEnquiriesRx.Where(a => a.NextCallDate <= dat && a.nextCallId==null && a.NextCallMode == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
            foreach(CrmTeleCallingRx det in det1)
            {
                lst.Add(new CRMPendingsList
                {
                    id = det.RecordId,
                    seq=det.Seq,
                    callerId=det.Callerid,
                    customer = det.Customer,
                    mobile = det.Mobile,
                    mode = "TeleCall",
                    dat = det.Dat.Value,
                    customercomments=det.CustomerComments,
                    username=det.Username
                });
            }
            foreach (CrmEnquiriesRx det in det2)
            {
                lst.Add(new CRMPendingsList
                {
                    id = det.RecordId,
                    seq=det.Seq,
                    callerId=det.Callerid,
                    customer = det.Customer,
                    mobile = det.Mobile,
                    mode = "Enquiry",
                    dat = det.Dat.Value,
                    customercomments=det.CustomerComments,
                    username=det.username
                    
                });
            }
            return lst;
        }

    }
}
