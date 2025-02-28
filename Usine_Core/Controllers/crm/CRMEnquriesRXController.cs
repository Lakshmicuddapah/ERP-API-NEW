using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Controllers.HRD;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Usine_Core.Controllers.CRM
{
    public class CRMEnquiryRxTotal
    {
        public CrmEnquiriesRx call { get; set; }
        public int traCheck { get; set; }
        public UserInfo usr { get; set; }
        public string result { get; set; }
    }
   
    public class CRMEnquiryRXRequirements
    {
        public String seq { get; set; }
        public List<Employees> employees { get; set; }
        public List<CRMPendingsList> pendings { get; set; }
        public List<MisCountryMaster> countries { get; set; }
        public List<MisStateMaster> states { get; set; }
    }
    public class CRMEnquriesRXController : Controller
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();


        [HttpPost]
        [Authorize]
        [Route("api/CRMRx/GetCRMRxEnquiryRequirements")]
        public CRMEnquiryRXRequirements GetCRMRxEnquiryRequirements([FromBody]UserInfo usr)
        {
            hrdDepartmentsController hr = new hrdDepartmentsController();
            CRMEnquiryRXRequirements tot = new CRMEnquiryRXRequirements();
            tot.seq = findSeq(usr);
            GeneralInformation inf = new GeneralInformation();
            inf.detail = "Sales";
            inf.usr = usr;
          //  tot.employees = hr.getEmployeesByDepartment(inf);
            GeneralInformation inf1 = new GeneralInformation();
            inf.recordId = 2;
            inf.usr = usr;
            tot.pendings = getPendingCallsList(inf);

            tot.countries = db.MisCountryMaster.Where(a => a.CustomerCode == usr.cCode).ToList();
            tot.states = db.MisStateMaster.Select(x => new MisStateMaster
            {
                StateName = x.StateName,
                CustomerCode = x.CustomerCode,
                Cntname = x.Cntname,
                RecordId = x.RecordId

            }).Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.StateName).ToList();

            //tot.states = db.MisStateMaster.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.StateName).ToList();
            return tot;
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMRx/GetCRMRxEnquiries")]
        public List<CrmEnquiriesRx> GetCRMRxEnquiries([FromBody]GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);

            var det= db.CrmEnquiriesRx.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();
            return det;
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMRx/GetCRMRxOrders")]
        public List<CrmOrdersRxuni> GetCRMRxOrders([FromBody] GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);


            return  (from a in db.CrmOrdersRxuni.Where(a => a.Dat >= dat1 && a.Dat <= dat2)
                     join b in (from p in db.CrmOrdersRxdet.Where(a => a.Dat >= dat1 && a.Dat <=dat2).GroupBy(b => b.RecordId)
                                select new
                                {
                                    recordId=p.Key,
                                    qty=p.Sum(b => b.Qty)
                                }) on a.RecordId equals b.recordId
                                select new CrmOrdersRxuni
                                {
                                    RecordId=a.RecordId,
                                    Seq=a.Seq,
                                    Dat=a.Dat,
                                    Customer=a.Customer,
                                    BaseAmt=b.qty
                                }).OrderBy(d => d.RecordId).ToList();


             

           // return db.CrmEnquiriesRx.Where(a => a.Dat >= inf.frmDate.Value && a.Dat <= inf.toDate.Value && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();
        }







        [HttpPost]
        [Authorize]
        [Route("api/CRMRx/SetCRMRXEnquiry")]
        public CRMEnquiryRxTotal SetCRMRXEnquiry([FromBody] CRMEnquiryRxTotal tot)
        {
            String msg = "";
            String res = ac.transactionCheck("CRM", tot.call.Dat, tot.usr);
            if (res == "OK")
            {
                try
                {
                    switch (tot.traCheck)
                    {
                        case 1:
                            if (ac.screenCheck(tot.usr, 7, 2, 2, 1))
                            {
                                var existingRecord = db.CrmEnquiriesRx
                                    .Where(a => a.RecordId == tot.call.RecordId
                                        && a.BranchId == tot.usr.bCode
                                        && a.CustomerCode == tot.usr.cCode)
                                    .FirstOrDefault();

                                if (existingRecord == null) // Insert New Record
                                {
                                    CrmEnquiriesRx call = new CrmEnquiriesRx();
                                    call.Seq = findSeq(tot.usr);
                                    call.Dat = ac.getPresentDateTime();
                                    call.Callerid = tot.call.Callerid;
                                    call.Customer = tot.call.Customer;
                                    call.Addr = tot.call.Addr;
                                    call.Country = tot.call.Country;
                                    call.Stat = tot.call.Stat;
                                    call.District = tot.call.District;
                                    call.City = tot.call.City;
                                    call.Zip = tot.call.Zip;
                                    call.Mobile = tot.call.Mobile;
                                    call.Tel = tot.call.Tel;
                                    call.Fax = tot.call.Fax;
                                    call.Webid = tot.call.Webid;
                                    call.Email = tot.call.Email;
                                    call.PrevcallId = tot.call.PrevcallId;
                                    call.PrevCallMode = tot.call.PrevCallMode;
                                    call.CustomerComments = tot.call.CustomerComments;
                                    call.CallerComments = tot.call.CallerComments;
                                    call.CallPosition = tot.call.CallPosition;
                                    call.NextCallDate = tot.call.NextCallDate;
                                    call.NextCallMode = tot.call.NextCallMode;
                                    call.Reference = tot.call.Reference;
                                    call.Type = tot.call.Type;
                                    call.BranchId = tot.usr.bCode;
                                    call.CustomerCode = tot.usr.cCode;
                                    call.username = tot.usr.uCode;
                                    db.CrmEnquiriesRx.Add(call);
                                    /*  if (tot.call.PrevcallId == null)
                                {
                                    PartyDetails party = new PartyDetails();
                                    party.PartyName = tot.call.Customer;
                                    party.Addr = tot.call.Addr;
                                    party.City = tot.call.City;
                                    party.Mobile = tot.call.Mobile;
                                    party.Email = tot.call.Email;
                                    party.Web = tot.call.Webid;
                                    party.Zip = tot.call.Zip;
                                    party.PartyGroup = 120;
                                    party.PartyType = "CUS";
                                    party.Statu = 2;
                                    party.BranchId = tot.usr.bCode;
                                    party.CustomerCode = tot.usr.cCode;
                                    db.PartyDetails.Add(party);
                                }*/
                                }
                                else // Update Existing Record
                                {
                                    existingRecord.Callerid = tot.call.Callerid;
                                    existingRecord.Customer = tot.call.Customer;
                                    existingRecord.Addr = tot.call.Addr;
                                    existingRecord.Country = tot.call.Country;
                                    existingRecord.Stat = tot.call.Stat;
                                    existingRecord.District = tot.call.District;
                                    existingRecord.City = tot.call.City;
                                    existingRecord.Zip = tot.call.Zip;
                                    existingRecord.Mobile = tot.call.Mobile;
                                    existingRecord.Tel = tot.call.Tel;
                                    existingRecord.Fax = tot.call.Fax;
                                    existingRecord.Webid = tot.call.Webid;
                                    existingRecord.Email = tot.call.Email;
                                    existingRecord.PrevcallId = tot.call.PrevcallId;
                                    existingRecord.PrevCallMode = tot.call.PrevCallMode;
                                    existingRecord.CustomerComments = tot.call.CustomerComments;
                                    existingRecord.CallerComments = tot.call.CallerComments;
                                    existingRecord.CallPosition = tot.call.CallPosition;
                                    existingRecord.NextCallDate = tot.call.NextCallDate;
                                    existingRecord.NextCallMode = tot.call.NextCallMode;
                                    existingRecord.Reference = tot.call.Reference;
                                    existingRecord.Type = tot.call.Type;
                                }

                                db.SaveChanges();
                                msg = "OK";
                            }
                            else
                            {
                                msg = "You are not authorised for Enquiries";
                            }
                            break;
                        case 2: // Fetch Enquiry Data
                            if (ac.screenCheck(tot.usr, 7, 2, 2, 2))
                            {
                                var call = db.CrmEnquiriesRx
                                    .Where(a => a.RecordId == tot.call.RecordId
                                        && a.BranchId == tot.usr.bCode
                                        && a.CustomerCode == tot.usr.cCode)
                                    .FirstOrDefault();

                                if (call != null)
                                {
                                    // Assign fetched data to tot.call
                                    tot.call = new CrmEnquiriesRx
                                    {
                                        RecordId = call.RecordId,
                                        Callerid = call.Callerid,
                                        Seq = call.Seq,
                                        Customer = call.Customer,
                                        Addr = call.Addr,
                                        Country = call.Country,
                                        Stat = call.Stat,
                                        District = call.District,
                                        City = call.City,
                                        Zip = call.Zip,
                                        Mobile = call.Mobile,
                                        Tel = call.Tel,
                                        Fax = call.Fax,
                                        Webid = call.Webid,
                                        Email = call.Email,
                                        PrevcallId = call.PrevcallId,
                                        PrevCallMode = call.PrevCallMode,
                                        CustomerComments = call.CustomerComments,
                                        CallerComments = call.CallerComments,
                                        CallPosition = call.CallPosition,
                                        NextCallDate = call.NextCallDate,
                                        NextCallMode = call.NextCallMode,
                                        Reference = call.Reference,
                                        Type = call.Type,
                                     
                                    };

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
                            if (ac.screenCheck(tot.usr, 7, 2, 1, 3))
                            {
                                var call = db.CrmEnquiriesRx.Where(a => a.RecordId == tot.call.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (call != null)
                                {

                                    db.CrmEnquiriesRx.Remove(call);
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
                                msg = "You are not authorised for enquiry";
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
            String seq = db.CrmEnquiriesRx.Where(a => a.Dat.Value.Year == dat.Year && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.Seq);
            int x = 0;
            if (seq != null)
            {
                x = int.Parse(seq.Substring(5, 7));
            }
            x++;
            return "SE" + dat.Year.ToString().Substring(2, 2) + "-" + g.zeroMake(x, 7);
        }

        public List<CRMPendingsList> getPendingCallsList(GeneralInformation inf)
        {
            List<CRMPendingsList> lst = new List<CRMPendingsList>();
            DateTime dat = ac.getPresentDateTime();
            var det1 = db.CrmTeleCallingRx.Where(a => a.NextCallDate <= dat && a.NextCallMode == "2" && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
            var det2 = db.CrmEnquiriesRx.Where(a => a.NextCallDate <= dat && a.NextCallMode == 2  && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
            foreach (CrmTeleCallingRx det in det1)
            {
                lst.Add(new CRMPendingsList
                {
                    id = det.RecordId,
                    customer = det.Customer,
                    mobile = det.Mobile,
                    mode = "TeleCall",
                    dat = det.Dat.Value,
                    customercomments=det.CustomerComments
                });
            }
            foreach (CrmEnquiriesRx det in det2)
            {
                lst.Add(new CRMPendingsList
                {
                    id = det.RecordId,
                    customer = det.Customer,
                    mobile = det.Mobile,
                    mode = "Enquiry",
                    dat = det.Dat.Value,
                    customercomments=det.CustomerComments
                });
            }
            return lst;
        }
       


    }
}
