using  System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Complete_Solutions_Core.Controllers.Accounts;
using Usine_Core.others;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.Admin
{
    public class TotAdvanceDetailsTotal
    {
        public TotAdvanceDetails adva { get; set; }
        public int tracheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class PartyAdvancesController : ControllerBase
    {
        UsineContext db = new UsineContext();
        [HttpPost]
        [Route("api/partyadvances/GetAdvances")]
        public List<TotAdvanceDetails> GetAdvances([FromBody] GeneralInformation inf)
        {
            General g = new General();

            return db.TotAdvanceDetails.Where(a => a.TransactionId == inf.recordId && a.Tratype == inf.detail && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();
        }
        private int? findAccountTransactionId()
        {
            UsineContext db = new UsineContext();
            int? x = 0;
            var det = db.FinexecUni.Max(a => a.RecordId);
            if (det != null)
            {
                x = det;
            }
            x++;
            return x;
        }
        private int? findSeq(DateTime dat, UserInfo usr)
        {
            UsineContext db = new UsineContext();
            AdminControl ac = new AdminControl();
            DateTime dat1 = DateTime.Parse(ac.strDate(dat));
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
        [Route("api/partyadvances/setAdvance")]
        public TotAdvanceDetailsTotal setAdvance([FromBody] TotAdvanceDetailsTotal tot)
        {
            AdminControl ac = new AdminControl();
            UsineContext db1 = new UsineContext();
            String msg = "";
            int paychk = 0;
            string narration = "";
            string module = "";
            Boolean b = false;
            switch (tot.adva.Tratype)
            {
                
                case "PUR_VOU":
                   
                    b = ac.screenCheck(tot.usr, 2, 2, 6, (int)tot.tracheck);
                    paychk = 1;
                    narration = "Purchase order Advance paid";
                    module = "Inventory";
           
                    break;
                case "SAL_REC":
                    paychk = 2;
                    b = ac.screenCheck(tot.usr, 7, 2, 5, (int)tot.tracheck);
                    narration = "Sale order Advance received";
                    module = "Sales";
                    break;
               
            }
            String tramsg = ac.transactionCheck(module, tot.adva.Dat, tot.usr);
            if (tramsg == "OK")
            {

               
                try
                {
                    if (b)
                    {
                        switch (tot.tracheck)
                        {
                            case 1:


                                TotAdvanceDetails ad = new TotAdvanceDetails();
                                ad.Seq = findseq(tot);
                                ad.TransactionId = tot.adva.TransactionId;
                                ad.Tratype = tot.adva.Tratype;
                                ad.Dat = tot.adva.Dat;
                                ad.Amt = tot.adva.Amt;
                                ad.Paymentmode = tot.adva.Paymentmode;
                                ad.Remarks = tot.adva.Remarks;
                                ad.Bankdetails = tot.adva.Bankdetails;
                                ad.PartyId = tot.adva.PartyId;
                                ad.AccountId = tot.adva.AccountId;
                                ad.Detail1 = tot.adva.Detail1;
                                ad.Detail2 = tot.adva.Detail2;
                                ad.Detail3 = tot.adva.Detail3;
                                ad.UsrName = tot.usr.uCode;
                                ad.BranchId = tot.usr.bCode;
                                ad.CustomerCode = tot.usr.cCode;
                                ad.PrintCount = 0;
                                db.TotAdvanceDetails.Add(ad);
                                db.SaveChanges();
                                if (ac.transactionCheck("Accounts", tot.adva.Dat, tot.usr) == "OK")
                                {

                                    var acc = db1.AccountsAssign.Where(a => a.Transcode == tot.adva.Tratype && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                    if (acc != null)
                                    {
                                       // var rec = findAccountTransactionId();
                                        var seq = findSeq(tot.adva.Dat.Value, tot.usr);
                                        FinexecUni header = new FinexecUni();
                                       // header.RecordId = rec;
                                        header.Seq = seq;
                                        header.Dat = tot.adva.Dat.Value;
                                        header.Narr = narration;
                                        header.Tratype = tot.adva.Tratype;
                                        header.Traref = ad.RecordId.ToString();
                                        header.Vouchertype = paychk == 1 ? "Payment" : "Receipt";
                                        header.BankDet = " ";
                                        header.Branchid = tot.usr.bCode;
                                        header.CustomerCode = tot.usr.cCode;
                                        header.Usr = tot.usr.uCode;
                                        db.FinexecUni.Add(header);
                                        db.SaveChanges();
                                        var rec = header.RecordId;
                                        List<FinexecDet> lines = new List<FinexecDet>();


                                        lines.Add(new FinexecDet
                                        {
                                            RecordId = rec,
                                            Sno = 1,
                                            Accname = acc.Account,
                                            Cre = paychk == 1 ? 0 : tot.adva.Amt,
                                            Deb = paychk == 1 ? tot.adva.Amt : 0,
                                            Branchid = tot.usr.bCode,
                                            CustomerCode = tot.usr.cCode,
                                            Dat = tot.adva.Dat.Value
                                        });
                                        lines.Add(new FinexecDet
                                        {
                                            RecordId = rec,
                                            Sno = 2,
                                            Accname = tot.adva.AccountId,
                                            Cre = paychk == 1 ? tot.adva.Amt : 0,
                                            Deb = paychk == 1 ? 0 : tot.adva.Amt,
                                            Branchid = tot.usr.bCode,
                                            CustomerCode = tot.usr.cCode,
                                            Dat = tot.adva.Dat.Value
                                        });
                                        db.FinexecDet.AddRange(lines);

                                    }
                                    db.SaveChanges();
                                }
                               TransactionsAudit audit = new TransactionsAudit();
                                audit.TraId = ad.RecordId;
                                audit.Descr = narration + " with id of " + ad.Seq + " with an amount of " + tot.adva.Amt.ToString();
                                audit.Usr = tot.usr.uCode;
                                audit.Tratype = 1;
                                audit.Transact = tot.adva.Tratype;
                                audit.TraModule = module == "Sales" ? "SAL" : "PUR";
                                audit.Syscode = " ";
                                audit.BranchId = tot.usr.bCode;
                                audit.CustomerCode = tot.usr.cCode;
                                audit.Dat = ac.getPresentDateTime();
                                db.TransactionsAudit.Add(audit);
                                db.SaveChanges();
                                msg = "OK";


                                break;
                            case 2:
                                var adupd = db.TotAdvanceDetails.Where(a => a.RecordId == tot.adva.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                var oldamt = adupd.Amt;
                                if (adupd != null)
                                {


                                    adupd.Dat = tot.adva.Dat;
                                    adupd.Amt = tot.adva.Amt;
                                    adupd.Paymentmode = tot.adva.Paymentmode;
                                    adupd.Remarks = tot.adva.Remarks;
                                    adupd.Bankdetails = tot.adva.Bankdetails;
                                    adupd.PartyId = tot.adva.PartyId;
                                    adupd.AccountId = tot.adva.AccountId;
                                    adupd.Detail1 = tot.adva.Detail1;
                                    adupd.Detail2 = tot.adva.Detail2;
                                    adupd.Detail3 = tot.adva.Detail3;
                                    adupd.UsrName = tot.usr.uCode;
                                   
                                    if (ac.transactionCheck("Accounts", tot.adva.Dat, tot.usr) == "OK")
                                    {


                                        var delu2 = db1.FinexecDet.Where(a => a.RecordId ==
                                        (db1.FinexecUni.Where(a => a.Traref == tot.adva.RecordId.ToString() && a.Tratype == tot.adva.Tratype && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).Select(b => b.RecordId).FirstOrDefault())
                                        && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                        var delu1 = db1.FinexecUni.Where(a => a.Traref == tot.adva.RecordId.ToString() && a.Tratype == tot.adva.Tratype && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                     if(delu2.Count > 0)
                                        db.FinexecDet.RemoveRange(delu2);
                                     if(delu1!= null)
                                        db.FinexecUni.Remove(delu1);

                                        db.SaveChanges();

                                        var acc = db1.AccountsAssign.Where(a => a.Transcode == tot.adva.Tratype && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                        if (acc != null)
                                        {
                                            

                                          //  var rec = findAccountTransactionId();
                                            var seq = findSeq(tot.adva.Dat.Value, tot.usr);
                                            FinexecUni header = new FinexecUni();
                                           // header.RecordId = rec;
                                            header.Seq = seq;
                                            header.Dat = tot.adva.Dat.Value;
                                            header.Narr = narration;
                                            header.Tratype = tot.adva.Tratype;
                                            header.Traref = tot.adva.RecordId.ToString();
                                            header.Vouchertype = paychk == 1 ? "Payment" : "Receipt";
                                            header.BankDet = " ";
                                            header.Branchid = tot.usr.bCode;
                                            header.CustomerCode = tot.usr.cCode;
                                            header.Usr = tot.usr.uCode;
                                            db.FinexecUni.Add(header);
                                            List<FinexecDet> lines = new List<FinexecDet>();
                                            db.SaveChanges();

                                            lines.Add(new FinexecDet
                                            {
                                                RecordId = header.RecordId,
                                                Sno = 1,
                                                Accname = acc.Account,
                                                Cre = paychk == 1 ? 0 : tot.adva.Amt,
                                                Deb = paychk == 1 ? tot.adva.Amt : 0,
                                                Branchid = tot.usr.bCode,
                                                CustomerCode = tot.usr.cCode,
                                                Dat = tot.adva.Dat.Value
                                            });
                                            lines.Add(new FinexecDet
                                            {
                                                RecordId = header.RecordId,
                                                Sno = 2,
                                                Accname = tot.adva.AccountId,
                                                Cre = paychk == 1 ? tot.adva.Amt : 0,
                                                Deb = paychk == 1 ? 0 : tot.adva.Amt,
                                                Branchid = tot.usr.bCode,
                                                CustomerCode = tot.usr.cCode,
                                                Dat = tot.adva.Dat.Value
                                            });
                                            db.FinexecDet.AddRange(lines);

                                        }
                                    }
                                   

                                    TransactionsAudit audit1 = new TransactionsAudit();
                                    audit1.TraId = tot.adva.RecordId;
                                    audit1.Descr = "Change in " + narration + " with id of " + adupd.Seq + " from " + oldamt.ToString() + " to " + tot.adva.Amt.ToString();
                                    audit1.Usr = tot.usr.uCode;
                                    audit1.Tratype = 2;
                                    audit1.Transact = tot.adva.Tratype;
                                    audit1.TraModule = module == "Sales" ? "SAL" : "PUR";
                                    audit1.Syscode = " ";
                                    audit1.BranchId = tot.usr.bCode;
                                    audit1.CustomerCode = tot.usr.cCode;
                                    audit1.Dat = ac.getPresentDateTime();
                                    db.TransactionsAudit.Add(audit1);
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                break;
                            case 3:
                                var addel = db.TotAdvanceDetails.Where(a => a.RecordId == tot.adva.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                var oldamdel = addel.Amt;
                                db.TotAdvanceDetails.Remove(addel);
                                var del2 = db1.FinexecDet.Where(a => a.RecordId ==
                                                                      (db1.FinexecUni.Where(a => a.Traref == tot.adva.RecordId.ToString() && a.Tratype == tot.adva.Tratype && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).Select(b => b.RecordId).FirstOrDefault())
                                                                      && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                var del1 = db1.FinexecUni.Where(a => a.Traref == tot.adva.RecordId.ToString() && a.Tratype == tot.adva.Tratype && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                db.FinexecDet.RemoveRange(del2);
                                db.FinexecUni.Remove(del1);


                                TransactionsAudit audit2 = new TransactionsAudit();
                                audit2.TraId = tot.adva.RecordId;
                                audit2.Descr = "Deletion of " + narration + " with id of " + addel.Seq + " with amount " + oldamdel.ToString();
                                audit2.Usr = tot.usr.uCode;
                                audit2.Tratype = 3;
                                audit2.Transact = tot.adva.Tratype;
                                audit2.TraModule = module == "Sales" ? "SAL" : "PUR";
                                audit2.Syscode = " ";
                                audit2.BranchId = tot.usr.bCode;
                                audit2.CustomerCode = tot.usr.cCode;
                                audit2.Dat = ac.getPresentDateTime();
                                db.TransactionsAudit.Add(audit2);
                                db.SaveChanges();



                                break;
                        }
                    }
                    else
                    {
                        msg = "You are not authorised for this transaction";
                    }
                }
                catch (Exception ee)
                {
                    msg = ee.Message;
                }
            }
            else
            {
                msg = tramsg;
            }


            tot.result = msg;
            return tot;
        }
        private String findseq(TotAdvanceDetailsTotal inf)
        {
            int x = 0;
            AdminControl ac = new AdminControl();
            General g = new General();
            DateTime dat = inf.adva.Dat.Value;
            DateTime dat1 = ac.getFinancialStart(dat, inf.usr);
            DateTime dat2 = dat1.AddYears(1);

            var det = db.TotAdvanceDetails.Where(a => a.Tratype == inf.adva.Tratype && a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).Max(b => b.Seq);
            if (det != null)
            {
                x = g.valInt(g.right(det, 7));
            }
            x++;
            return "AD" + (g.right(inf.adva.Tratype,3) != "REC" ? "VO" : "RE") + dat1.Year.ToString().Substring(2,2) + dat2.Year.ToString().Substring(2,2) + "-" + g.zeroMake(x, 7);

        }
    }
}
