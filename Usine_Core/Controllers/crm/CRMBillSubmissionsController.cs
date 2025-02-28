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
    public class CRMBillSubmissionTotal
    {
        public CrmBillSubmissionsUni header { get; set; }
        public List<CrmBillSubmissionsDet> lines { get; set; }
        public UserInfo usr { get; set; }
    }
     
    public class CRMBillSubmissionsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        [HttpPost]
        [Authorize]
        [Route("api/CRMBillSubmissions/GetSubmittedBills")]
        public dynamic GetSubmittedBills([FromBody] GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = dat1.AddDays(1);
            var lst1 = db.CrmBillSubmissionsUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
            var lst = (from a in db.CrmBillSubmissionsDet.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                        join b in db.SalSalesUni.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.Billno equals b.RecordId
                        select new
                        {
                            recordid = a.RecordId,
                            baseamt = b.Baseamt,
                            totalamt = b.TotalAmt
                        }).ToList();

            var lst2 = (from a in lst.GroupBy(b => b.recordid)
                        select new
                        {
                            recordid = a.Key,
                            baseamt = a.Sum(c => c.baseamt),
                            totalamt = a.Sum(c => c.totalamt)
                        }).ToList();
            var lst3 = db.PartyDetails.Where(a => a.CustomerCode == inf.usr.cCode);


            return (from a in lst1
                       join b in lst2 on a.RecordId equals b.recordid
                       join c in lst3 on a.CustomerId equals c.RecordId
                        select new
                       {
                           recordid = a.RecordId,
                           seq = a.Seq,
                           customerid = a.CustomerId,
                           customername = c.PartyName,
                           totalbase=b.baseamt,
                           totalamt=b.totalamt
                       }).OrderBy(b => b.recordid).ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMBillSubmissions/GetBillsForSubmission")]
        public List<SalSalesUni> GetBillsForSubmission([FromBody] UserInfo usr)
        {
            var det= (from a in db.SalSalesUni.Where(a => a.BranchId==usr.bCode && a.CustomerCode == usr.cCode)
                    join b in db.CrmBillSubmissionsDet  on a.RecordId equals b.Billno
                    into gj
                    from subdet in gj.DefaultIfEmpty()
                    select new SalSalesUni
                    {
                        RecordId=a.RecordId,
                        Seq=a.Seq,
                        Dat=a.Dat,
                        PartyId=a.PartyId,
                        PartyName=a.PartyName,
                        Baseamt=a.Baseamt,
                        TotalAmt=a.TotalAmt,
                        Pos=subdet==null?1:0
                    }).ToList();
            return det.Where(a => a.Pos == 1).OrderBy(b => b.RecordId).ToList();
         }
        [HttpPost]
        [Authorize]
        [Route("api/CRMBillSubmissions/SetBillSubmission")]
        public TransactionResult SetBillSubmission([FromBody] CRMBillSubmissionTotal tot)
        {
            string msg = "";
            try
            {
                if(ac.screenCheck(tot.usr,7,2,2,0))
                {
                    var rec = findRecordId();
                    var seq = findSeq(tot.usr);
                    tot.header.RecordId = rec;
                    tot.header.Seq = seq;
                    tot.header.Dat = ac.getPresentDateTime();
                    tot.header.BranchId = tot.usr.bCode;
                    tot.header.CustomerCode = tot.usr.cCode;
                    db.CrmBillSubmissionsUni.Add(tot.header);
                    int sno = 1;
                    foreach(var lin in tot.lines)
                    {
                        lin.RecordId = rec;
                        lin.Sno = sno;
                        lin.BranchId = tot.usr.bCode;
                        lin.CustomerCode = tot.usr.cCode;
                        sno++;
                    }
                        if(tot.lines.Count() > 0)
                    {
                        db.CrmBillSubmissionsDet.AddRange(tot.lines);
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

        private int? findRecordId()
        {
            UsineContext db = new UsineContext();
            int? x = 0;
            var det = db.CrmBillSubmissionsUni.Max(b => b.RecordId);
            if(det != null)
            {
                x = det;
            }
            x++;
            return x;
        }
        private string findSeq(UserInfo usr)
        {
              General gg = new General();
            int x = 0;
            var det = db.CrmBillSubmissionsUni.Where(a => a.BranchId==usr.bCode && a.CustomerCode==usr.cCode).Max(b => b.Seq);
            if (det != null)
            {
                x = gg.valInt( gg.right(det,6));
            }
            x++;
            return "SUB" + gg.zeroMake(x,6);
        }

    }
}
