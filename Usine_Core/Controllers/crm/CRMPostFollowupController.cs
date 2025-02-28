using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;

namespace Usine_Core.Controllers.crm
{
     public class CRMPostFollowupTotal
    {
        public CrmPostTeleCalling call { get; set; }
        public UserInfo usr { get; set; }
    }
    public class CRMPostFollowupRequirments
    {
        public dynamic details { get; set; }
        public string seq { get; set; }
    }
    public class CRMPostFollowupController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/CRMPostFollowup/GetTodayFollowup")]
        public List<CrmPostTeleCalling> GetTodayFollowup([FromBody] GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = dat1.AddDays(1);
            return db.CrmPostTeleCalling.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMPostFollowup/GetFollowUpRequirements")]
        public CRMPostFollowupRequirments GetFollowUpRequirements([FromBody] UserInfo usr)
        {
            var lst1 = (from a in db.PartyTransactions.Where(a => a.CustomerCode == usr.cCode).GroupBy(b => b.PartyId)
                        select new
                        {
                            partyid = a.Key,
                            pending = a.Sum(b => b.PendingAmount - b.ReturnAmount - b.CreditNote - b.PaymentAmount)
                        }).Where(a => a.pending > 0).ToList();
            var lst2 = db.PartyDetails.Where(a => a.PartyType == "CUS" && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();

            var sublst31 = (from a in db.CrmPostTeleCalling.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).GroupBy(a => a.CustomerId)
                        select new
                        {
                            customerid = a.Key,
                            callid = a.Max(b => b.RecordId)
                        }).ToList();
            var sublst32 = db.CrmPostTeleCalling.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
            var lst3 = (from a in sublst31
                        join b in sublst32 on a.callid equals b.RecordId
                        select new
                        {
                            partyid = a.customerid,
                              callercomments = b.CallerComments,
                            dat = b.Dat,
                            seq = b.Seq,
                            prevmode = b.CallType == 1 ? "Telecall" : "Personal Visit",
                            prevuser = b.UserName,
                            previouscomments = b.CustomerComments
                        }).ToList();

            var details1 = (from a in lst1
                            join b in lst2 on a.partyid equals b.RecordId
                            select new
                            {
                                customerid = a.partyid,
                                customername = b.PartyName,
                                mobile = b.ContactMobile,
                                email = b.ContactEmail,
                                pendingamt=a.pending

                            }).ToList();
            CRMPostFollowupRequirments tot = new CRMPostFollowupRequirments();
           tot.details= (from a in details1
                           join b in lst3 on a.customerid equals b.partyid
                           into gj
                           from subdet in gj.DefaultIfEmpty()
                           select new
                           {
                               customerid = a.customerid,
                               customername = a.customername,
                               mobile = a.mobile,
                               email = a.email,
                               pendingamt = a.pendingamt,
                               prevcommnets = subdet == null ? "" : subdet.previouscomments,
                               prevseq = subdet == null ? "" : subdet.seq,
                               prevdate = subdet == null ? null : subdet.dat,
                               prevmode = subdet == null ? "" : subdet.prevmode,
                               prevcaller = subdet == null ? "" : subdet.prevuser,

                           }).OrderBy(b => b.customername).ToList();
            tot.seq = findSeq(usr);
            return tot;


        }
        private string findSeq(UserInfo usr)
        {
            UsineContext db = new UsineContext();
            General gg = new General();
            int x = 0;
            var det = db.CrmPostTeleCalling.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.Seq);
            if(det != null)
            {
                x =gg.valInt( gg.right(det, 6));
            }
            x++;
            return "CFO-" + gg.zeroMake(x, 6);
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMPostFollowup/SetCRMPostFollowup")]
        public TransactionResult SetCRMPostFollowup([FromBody] CRMPostFollowupTotal tot)
        {
            string msg = "";
            try
            {
                if(ac.screenCheck(tot.usr,7,2,2,1))
                {
                    tot.call.Seq = findSeq(tot.usr);
                    tot.call.Dat = ac.getPresentDateTime();
                    tot.call.UserName = tot.usr.uCode;
                    tot.call.BranchId = tot.usr.bCode;
                    tot.call.CustomerCode = tot.usr.cCode;
                    db.CrmPostTeleCalling.Add(tot.call);
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

    }
}
