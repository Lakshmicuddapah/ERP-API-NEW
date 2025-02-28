using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;

using Usine_Core.Controllers.Admin;
using Usine_Core.Controllers.CRM;
using Microsoft.AspNetCore.Authorization;
using Usine_Core.others;
using System.Data.SqlClient;

namespace Usine_Core.Controllers.crm
{
    public class PartyPaymentTotal
    {
        public PartyPaymentsuni header { get; set; }
        public List<PartyTransactions> lines { get; set; }
        public UserInfo usr { get; set; }
    }
    public class CRMReceiptsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/CRMReceipts/GetReceipts")]
        public List<PartyPaymentsuni> GetReceipts([FromBody] GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = dat1.AddDays(1);
            return (from a in db.PartyPaymentsuni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.VoucherType == "SAL_REC")
                    join b in db.PartyDetails.Where(a => a.CustomerCode == inf.usr.cCode) on a.PartyId equals b.RecordId
                    select new PartyPaymentsuni
                    {
                        Seq = a.Seq,
                        RecordId = a.RecordId,
                        PartyId = a.PartyId,
                        BaseAmt = a.BaseAmt,
                        Tds = a.Tds,
                        Rebate = a.Rebate,
                        Others = a.Others,
                        ReceiptAmt = a.ReceiptAmt,
                        ModeOfPayment = a.ModeOfPayment,
                        BranchId = b.PartyName
                    }).OrderBy(c => c.RecordId).ToList();

        }


        [HttpPost]
        [Authorize]
        [Route("api/CRMReceipts/SetPartyReceipt")]
        public TransactionResult SetPartyPayment([FromBody] PartyPaymentTotal tot)
        {

            string msg = "";
            if (ac.screenCheck(tot.usr, 2, 3, 4, 1))
            {
                try
                {
                    var seq = findSeq(tot.usr);
                    tot.header.Seq = seq;
                    tot.header.Dat = ac.getPresentDateTime();
                    tot.header.BranchId = tot.usr.bCode;
                    tot.header.CustomerCode = tot.usr.cCode;
                    tot.header.VoucherType = "SAL_REC";
                    db.PartyPaymentsuni.Add(tot.header);
                    db.SaveChanges();
                    foreach (var line in tot.lines)
                    {
                        line.TransactionId = (int)tot.header.RecordId;
                        line.Dat = tot.header.Dat;
                        line.Descriptio = "Payment against receipt " + seq;
                        line.Username = tot.usr.uCode;
                        line.BranchId = tot.usr.bCode;
                        line.CustomerCode = tot.usr.cCode;

                    }
                    db.PartyTransactions.AddRange(tot.lines);

                    makeAccounts(tot.header, tot.usr);
                    db.SaveChanges();
                    msg = "OK";
                }
                catch (Exception ee)
                {
                    msg = ee.Message;
                }
            }

            else
            {
                msg = "You are not authoriesed for this transaction";
            }
            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;
        }
        [HttpPost]
        [Authorize]
        [Route("api/CRMReceipts/DeletePartyReceipt")]
        public TransactionResult DeletePartyReceipt([FromBody] GeneralInformation inf)
        {
            string msg = "";
            if (ac.screenCheck(inf.usr, 2, 3, 4, 3))
            {
                try
                {
                    var header = db.PartyPaymentsuni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    var lines = db.PartyTransactions.Where(a => a.TransactionType == "REC" && a.TransactionId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                    var acchea = db.FinexecUni.Where(a => a.Traref == inf.recordId.ToString() && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if (acchea != null)
                    {
                        var acclines = db.FinexecDet.Where(a => a.RecordId == acchea.RecordId && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                        if (acclines.Count() > 0)
                        {
                            db.FinexecDet.RemoveRange(acclines);
                        }
                        db.FinexecUni.Remove(acchea);
                    }
                    if (lines.Count() > 0)
                    {
                        db.PartyTransactions.RemoveRange(lines);
                    }
                    if (header != null)
                    {
                        db.PartyPaymentsuni.Remove(header);
                    }

                    db.SaveChanges();
                    msg = "OK";
                }
                catch (Exception ee)
                {
                    msg = ee.Message;
                }
            }
            else
            {
                msg = "You are not authorised for this transaction";
            }
            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;
        }
        private string findSeq(UserInfo usr)
        {
            UsineContext db1 = new UsineContext();
            General gg = new General();
            int year = ac.getPresentDateTime().Year;
            var det = db1.PartyPaymentsuni.Where(a => a.VoucherType == "SAL_REC" && a.Dat.Value.Year == year && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.Seq);
            int x = 0;
            if (det != null)
            {
                x = gg.valInt(gg.right(det, 5));
            }
            x++;
            return "REC" + year.ToString() + "-" + gg.zeroMake(x, 5);
        }
        private void makeAccounts(PartyPaymentsuni header, UserInfo usr)
        {
            UsineContext db1 = new UsineContext();
            General gg = new General();
            var details = db1.AccountsAssign.Where(a => a.Module == "SAL" && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
            var chk = 0;
            int? creacc = 0, rebacc = 0, othacc = 0, revacc = 0;
            foreach (var det in details)
            {
                switch (det.Transcode)
                {
                    case "SAL_CRP":
                        creacc = det.Account;
                        break;
                    case "REC_REB":
                        rebacc = det.Account;
                        break;
                    case "REC_OTH":
                        othacc = det.Account;
                        break;
                }
            }
            if (header.BaseAmt > 0 && creacc == 0)
            {
                chk++;
            }
            if (header.Rebate > 0 && rebacc == 0)
            {
                chk++;
            }
            if (header.Others > 0 && othacc == 0)
            {
                chk++;
            }
            if (header.ReceiptAmt > 0 && header.RevAccount == 0)
            {
                chk++;
            }
            List<FinexecDet> lines = new List<FinexecDet>();
            if (chk == 0)
            {
                FinexecUni hea = new FinexecUni();

                hea.Seq = findAccountSeq(usr);
                hea.Dat = header.Dat;
                hea.Narr = "Sale Receipt details of Voucher " + header.Seq;
                hea.Tratype = "SAL_REC";
                hea.Traref = header.RecordId.ToString();
                hea.Vouchertype = "RECEIPT";
                hea.Branchid = usr.bCode;
                hea.CustomerCode = usr.cCode;
                hea.Usr = usr.uCode;
                db1.FinexecUni.Add(hea);
                db1.SaveChanges();
                if (header.BaseAmt > 0)
                {
                    lines.Add(new FinexecDet
                    {
                        RecordId = hea.RecordId,
                        Sno = 1,
                        Accname = creacc,
                        Cre = header.BaseAmt,
                        Deb = 0,
                        Branchid = usr.bCode,
                        CustomerCode = usr.cCode,
                        Dat = hea.Dat
                    });

                }
                if (header.Rebate > 0)
                {
                    lines.Add(new FinexecDet
                    {
                        RecordId = hea.RecordId,
                        Sno = 2,
                        Accname = rebacc,
                        Cre = 0,
                        Deb = header.Rebate,
                        Branchid = usr.bCode,
                        CustomerCode = usr.cCode,
                        Dat = hea.Dat
                    });
                }
                if (header.Others > 0)
                {
                    lines.Add(new FinexecDet
                    {
                        RecordId = hea.RecordId,
                        Sno = 3,
                        Accname = othacc,
                        Cre = 0,
                        Deb = header.Others,
                        Branchid = usr.bCode,
                        CustomerCode = usr.cCode,
                        Dat = hea.Dat
                    });
                }
                if (header.ReceiptAmt > 0)
                {
                    lines.Add(new FinexecDet
                    {
                        RecordId = hea.RecordId,
                        Sno = 4,
                        Accname = header.RevAccount,
                        Cre = 0,
                        Deb = header.ReceiptAmt,
                        Branchid = usr.bCode,
                        CustomerCode = usr.cCode,
                        Dat = hea.Dat
                    });
                }
                db1.FinexecDet.AddRange(lines);
                db1.SaveChanges();
            }


        }
        private int? findAccountSeq(UserInfo usr)
        {
            UsineContext db = new UsineContext();
            AdminControl ac = new AdminControl();
            DateTime dat1 = ac.getPresentDateTime();
            DateTime dat2 = dat1.AddDays(1);
            int? x = 0;
            var xx = db.FinexecUni.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode && a.Dat >= dat1 && a.Dat < dat2).FirstOrDefault();
            if (xx != null)
            {
                x = db.FinexecUni.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode && a.Dat >= dat1 && a.Dat < dat2).Max(b => b.Seq);
            }
            x++;
            return x;
        }

    }
}
