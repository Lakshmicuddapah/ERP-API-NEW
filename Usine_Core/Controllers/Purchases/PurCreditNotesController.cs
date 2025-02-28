using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.Purchases
{
    public class CreditNoteTotal
    {
        public PartyCreditDebitNotes note { get; set; }
        public int tracheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
     
    public class PurCreditNotesController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/PurCreditNotes/GetCreditNotes")]
    public dynamic GetCreditNotes([FromBody] GeneralInformation inf)
    {
        DateTime dat1 = DateTime.Parse(inf.frmDate);
        DateTime dat2 = dat1.AddDays(1);
            var details= (from a in db.PartyCreditDebitNotes.Where(a => a.TransactionType == "PCR" && a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                    join b in db.PurPurchasesUni.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.TransactionId equals b.RecordId
                    select new
                    {
                        RecordId=a.RecordId,
                        Seq=a.Seq,
                        Refmir=b.Seq,
                        Refinvoice=b.Invoiceno,
                        vendorname=b.Vendorname,
                        mobile=b.Mobile,
                        amt=a.Amt,
                        descriptio=a.Descriptio

                    }).ToList();
            return  details;
    }
        [HttpPost]
        [Authorize]
        [Route("api/PurCreditNotes/SetCreditNote")]
        public TransactionResult SetCreditNote([FromBody]CreditNoteTotal tot)
        {
            string msg = "";
            UsineContext db1 = new UsineContext();
            var purheader = db1.PurPurchasesUni.Where(a => a.RecordId == tot.note.TransactionId && a.BranchId==tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (ac.screenCheck(tot.usr, 2, 3, 3, 1))
                    {
                        try
                        {
                    tot.note.Dat = ac.getPresentDateTime();
                    tot.note.Seq = findSeq(tot.usr);
                    tot.note.BranchId = tot.usr.bCode;
                    tot.note.CustomerCode = tot.usr.cCode;
                    db.PartyCreditDebitNotes.Add(tot.note);
                    db.SaveChanges();
                    PartyTransactions note = new PartyTransactions();
                    note.TransactionId = (int)tot.note.RecordId;
                    note.TransactionType = tot.note.TransactionType;
                    note.Dat = ac.getPresentDateTime();
                    note.PartyId = purheader.Vendorid;
                    note.PartyName = purheader.Vendorname;
                    note.TransactionAmt = tot.note.Amt;
                    note.PendingAmount = 0;
                    note.ReturnAmount = 0;
                    note.CreditNote = tot.note.Amt;
                    note.PaymentAmount = 0;
                    note.Descriptio = tot.note.Descriptio;
                    note.Username = tot.usr.uCode;
                    note.BranchId = tot.usr.bCode;
                    note.CustomerCode = tot.usr.cCode;
                    note.OnTraId = tot.note.TransactionId;
                    db.PartyTransactions.Add(note);
                            db.SaveChanges();
                             var accounts = db1.AccountsAssign.Where(a => a.Module == "PUR" && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                            int? acc = 0, rev = 0;
                            foreach(var ac in accounts)
                            {
                                if(ac.Transcode=="PUR_CNT")
                                {
                                    acc = ac.Account;
                                }
                                if(ac.Transcode=="PUR_CRP")
                                {
                                    rev = ac.Account;
                                }
                            }
                         //   var seq = db.PurPurchasesUni.Where(a => a.RecordId == note.TransactionId).Select(b => b.Seq).FirstOrDefault();
                            if(acc > 0 && rev > 0)
                            {
                                FinexecUni acheader = new FinexecUni();
                                acheader.Seq = finAccSeq(tot.usr, tot.note.Dat.Value);
                                acheader.Dat = tot.note.Dat.Value;
                                acheader.Narr = "Credit Note Detail of Seq " + tot.note.Seq + " on MIR " + purheader.Seq;
                                acheader.Tratype = "PUR_PCR";
                                acheader.Traref = tot.note.RecordId.ToString();
                                acheader.Vouchertype = "CREDIT NOTE";
                                acheader.Branchid = tot.usr.bCode;
                                acheader.CustomerCode = tot.usr.cCode;
                                acheader.Usr = tot.usr.uCode;
                                db1.FinexecUni.Add(acheader);
                                db1.SaveChanges();

                                FinexecDet det1 = new FinexecDet();
                                det1.RecordId = acheader.RecordId;
                                det1.Sno = 1;
                                det1.Accname = acc;
                                det1.Cre = tot.note.Amt;
                                det1.Deb = 0;
                                det1.Branchid = tot.usr.bCode;
                                det1.CustomerCode = tot.usr.cCode;
                                det1.Dat = acheader.Dat;
                                db1.FinexecDet.Add(det1);
                                FinexecDet det2 = new FinexecDet();
                                det2.RecordId = acheader.RecordId;
                                det2.Sno = 2;
                                det2.Accname = rev;
                                det2.Cre = 0;
                                det2.Deb = tot.note.Amt;
                                det2.Branchid = tot.usr.bCode;
                                det2.CustomerCode = tot.usr.cCode;
                                det2.Dat = acheader.Dat;
                                db1.FinexecDet.Add(det2);
                                db1.SaveChanges();
                            }
 
                            msg = "OK";
                        }
                        catch (Exception ex)
                        {
                            msg = ex.Message;
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
        public String findSeq(UserInfo usr)
        {
            UsineContext db = new UsineContext();
            General g = new General();
            int x = 0;
            AdminControl ac = new AdminControl();
            DateTime dat = ac.getPresentDateTime();
            var xx = db.PartyCreditDebitNotes.Where(a => a.Dat.Value.Month == dat.Month && a.Dat.Value.Year == dat.Year && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.Seq);
            if (xx != null)
            {
                x = g.valInt(g.right(xx, 4));
            }
            x++;
            return "PCR" + dat.Year.ToString().Substring(2, 2) + "-" + (dat.Month < 10 ? "0" : "") + dat.Month.ToString() + "/" + g.zeroMake(x, 4);
        }
        private int? finAccSeq(UserInfo usr, DateTime dat)
        {
            UsineContext db = new UsineContext();
            AdminControl ac = new AdminControl();
            DateTime dat1 = dat;
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


        [HttpPost]
        [Authorize]
        [Route("api/PurCreditNotes/DeleteCreditNote")]
        public TransactionResult DeleteCreditNote([FromBody] GeneralInformation inf)
        {
            string msg = "";

            if (ac.screenCheck(inf.usr, 2, 3, 3, 3))
            {
                try
                {
                    var crnote = db.PartyCreditDebitNotes.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if(crnote != null)
                    {
                        db.PartyCreditDebitNotes.Remove(crnote);
                    }
                    var note = db.PartyTransactions.Where(a => a.TransactionId == inf.recordId && a.TransactionType == "PCR" && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    var head = db.FinexecUni.Where(a => a.Traref == inf.recordId.ToString() && a.Tratype == "PUR_PCR" && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if(head != null)
                    {
                        var lines = db.FinexecDet.Where(a => a.RecordId == head.RecordId && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                        if(lines.Count() > 0)
                        {
                            db.FinexecDet.RemoveRange(lines);
                        }
                        db.FinexecUni.Remove(head);
                    }
                    if(note != null)
                    {
                        db.PartyTransactions.Remove(note);
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
                msg = "You are not authorised for this transaction";
            }
            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;

        }



    }
}
