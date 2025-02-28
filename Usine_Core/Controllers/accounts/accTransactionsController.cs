using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;

namespace Usine_Core.Controllers.Accounts
{
    public class AccountTransactionTotal
    {
        public FinexecUni header { get; set; }
        public List<FinexecDet> lines { get; set; }
        public int? tracheck { get; set; }
        public String result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class AccountTransactionsCompleteDetails
    {
        public int? recordid { get; set; }
        public int? seq { get; set; }
        public String username { get; set; }
        public String details { get; set; }
        public double? amt { get; set; }
        public String narration { get; set; }
        public int? printCount { get; set; }
    }
    public class VoucherResult
    {
        public string filename { get; set; }
        public string result { get; set; }
    }
    public class accTransactionsController : Controller
    {
        UsineContext db = new UsineContext();
        private readonly IHostingEnvironment ho;
        public accTransactionsController(IHostingEnvironment hostingEnvironment)
        {
            
            ho = hostingEnvironment;
        }
       
        [HttpPost]
        [Authorize]
        [Route("api/accTransactions/getTransactionsList")]
        public List<AccountTransactionsCompleteDetails> getTransactionsList([FromBody] GeneralInformation inf)
        {
            try
            {
                AdminControl am = new AdminControl();
                DateTime dat1 = DateTime.Parse( inf.frmDate);
                DateTime dat2 = dat1.AddDays(1);


                var basedet = (from a in db.FinexecUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode && a.Vouchertype == inf.detail)
                               join b in db.FinexecDet.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.RecordId equals b.RecordId
                              select new
                              {
                                  a.RecordId,
                                  a.Seq,
                                  a.Narr,
                                  b.Accname,
                                  b.Deb,
                                  b.Cre,
                                  a.Usr,
                                  a.PrintCount
                              }).ToList();
                var masterdet =  db.FinAccounts.Where(a => a.CustomerCode == inf.usr.cCode).ToList();


                var det1 = (from a in basedet
                            join b in masterdet on a.Accname equals b.Recordid
                            select new
                            {
                                a.RecordId,
                                a.Seq,
                                a.Narr,
                                a.Accname,
                                a.Deb,
                                a.Cre,
                                a.Usr,
                                a.PrintCount,
                                detail = b.Accname + (a.Deb > 0 ? " Dr" : " Cr") + Math.Abs((double)(a.Deb - a.Cre)).ToString(),

                            }).ToList();

                return  det1.GroupBy(c => new { c.RecordId,c.Narr,c.Usr,c.PrintCount }).Select(g =>
                                new AccountTransactionsCompleteDetails { recordid = g.Key.RecordId,narration=g.Key.Narr,username=g.Key.Usr, amt = 0, details = String.Join(", ", g.Select(a => a.detail)) ,printCount=g.Key.PrintCount})
                                .OrderBy(b => b.recordid).ToList();

          
                
            }
            catch(Exception ee)
            {
                return null;
            }

        }

        [HttpPost]
        [Authorize]
        [Route("api/accTransactions/getTransactionDetail")]
        public AccountTransactionTotal getTransactionDetail([FromBody] GeneralInformation  inf)
        {
            AccountTransactionTotal det = new AccountTransactionTotal();
            det.header = (from a in db.FinexecUni.Where(a => a.RecordId == inf.recordId && a.CustomerCode == inf.usr.cCode)
                          select new FinexecUni
                          {
                              RecordId= a.RecordId,
                              Seq=a.Seq,
                              Dat=a.Dat,
                              Narr=a.Narr,
                              Tratype=a.Tratype,
                              Traref=a.Traref,
                              Vouchertype=a.Vouchertype,
                              BankDet=a.BankDet,
                              PrintCount=a.PrintCount,
                              detail1=a.detail1,
                              detail2=a.detail2,
                              detail3=a.detail3,
                              detail4=a.detail4
                          }).FirstOrDefault();
            det.lines = (from a in db.FinexecDet.Where(a => a.RecordId == inf.recordId && a.CustomerCode == inf.usr.cCode)
                         select new FinexecDet
                         {
                             RecordId=a.RecordId,
                             Sno=a.Sno,
                             Accname=a.Accname,
                             Cre=a.Cre,
                             Deb=a.Deb,
                             Dat=a.Dat
                         }).OrderBy(b => b.Sno).ToList();


            return det;

        }


       

        [HttpPost]
        [Authorize]
        [Route("api/accTransactions/setTransaction")]
        public TransactionResult setTransaction([FromBody] AccountTransactionTotal tot)
        {
            String msg = "";
            try
            {
                int screenChk = 0;
                int sno = 1;
                AdminControl ac = new AdminControl();
                String tmsg = ac.transactionCheck("Accounts", tot.header.Dat, tot.usr);
                if (tmsg == "OK")
                {
                    switch(tot.header.Vouchertype)
                    {
                        case "Payment":
                            screenChk = 1;
                            break;
                        case "Receipt":
                            screenChk = 2;
                            break;
                        case "Contra":
                            screenChk = 3;
                            break;
                        case "Transfer":
                            screenChk = 4;
                            break;
                    }
                    switch(tot.tracheck)
                    {
                        case 1:
                            if(ac.screenCheck(tot.usr, 1,2,screenChk,1))
                            {
                                using (var txn = db.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        tot.header.Seq = findSeq(tot);
                                        tot.header.Dat = ac.DateAdjustFromFrontEnd(tot.header.Dat.Value);
                                        tot.header.Branchid = tot.usr.bCode;
                                        tot.header.CustomerCode = tot.usr.cCode;
                                        tot.header.Usr = tot.usr.uCode;
                                        tot.header.PrintCount = 0;
                                        db.FinexecUni.Add(tot.header);
                                        db.SaveChanges();
                                        
                                        foreach (FinexecDet line in tot.lines)
                                        {
                                            line.RecordId = tot.header.RecordId;
                                            line.Sno = sno;
                                            sno++;
                                            line.Dat = tot.header.Dat;
                                            line.Branchid = tot.usr.bCode;
                                            line.CustomerCode = tot.usr.cCode;
                                        }
                                        db.FinexecDet.AddRange(tot.lines);

                                        FinexecUniHistory audheader_cre = new FinexecUniHistory();
                                        audheader_cre.RecordId = tot.header.RecordId;
                                        audheader_cre.Seq = tot.header.Seq;
                                        audheader_cre.Dat = tot.header.Dat;
                                        audheader_cre.Branchid = tot.usr.bCode;
                                        audheader_cre.CustomerCode = tot.usr.cCode;
                                        audheader_cre.Narr=tot.header.Narr;
                                        audheader_cre.Tratype= tot.header.Tratype;
                                        audheader_cre.Traref= tot.header.Traref;
                                        audheader_cre.Vouchertype= tot.header.Vouchertype;
                                        audheader_cre.BankDet= tot.header.BankDet;
                                        audheader_cre.Usr= tot.header.Usr;
                                        audheader_cre.AuditDate = ac.getPresentDateTime();
                                        audheader_cre.AuditType = 1;
                                        db.FinexecUniHistory.Add(audheader_cre);
                                        db.SaveChanges();
                                        sno = 1;
                                        List<FinexecDetHistory> audlines_cre = new List<FinexecDetHistory>();
                                        foreach(FinexecDet line in tot.lines)
                                        {
                                            audlines_cre.Add( new FinexecDetHistory{
                                                AuditId=audheader_cre.AuditId,
                                                Sno=sno,
                                                RecordId=tot.header.RecordId,
                                                Accname=line.Accname,
                                                Cre = line.Cre,
                                                Deb=line.Deb,
                                                Branchid=tot.usr.bCode,
                                                CustomerCode=tot.usr.cCode,
                                                AuditDate=audheader_cre.AuditDate,
                                                AuditType=1,

                                            });
                                            sno++;
                                        }
                                        db.FinexecDetHistory.AddRange(audlines_cre);
                                        


                                        TransactionsAudit aud = new TransactionsAudit();
                                        aud.TraId = tot.header.RecordId;
                                        var amt = tot.lines.Sum(a => a.Cre);
                                        aud.Descr = tot.header.Tratype + " transaction of " + amt.ToString() + " has been done";
                                        aud.Usr = tot.usr.uCode;
                                        aud.Tratype = 1;
                                        aud.Transact = tot.header.Tratype;
                                        aud.TraModule = "FIN";
                                        aud.Syscode = " ";
                                        aud.BranchId = tot.usr.bCode;
                                        aud.CustomerCode = tot.usr.cCode;
                                        aud.Dat = ac.getPresentDateTime();
                                        db.TransactionsAudit.Add(aud);
                                        db.SaveChanges();
                                        txn.Commit();
                                        msg = "OK";
                                    }
                                    catch (Exception ee)
                                    {
                                        msg = ee.Message;
                                        txn.Rollback();
                                    }
                                }
                            }
                            else
                            {
                                msg = "You are not authorised for this transaction";
                            }
                            break;
                        case 2:
                            if (ac.screenCheck(tot.usr, 1, 2, screenChk, 2))
                            {

                                int? rec = tot.header.RecordId;
                              
                                var oldamt = db.FinexecDet.Where(a => a.RecordId == rec && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).Sum(b => b.Cre);

                                var hea = db.FinexecUni.Where(a => a.RecordId == rec && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                var lin = db.FinexecDet.Where(a => a.RecordId == rec && a.Branchid == tot.usr.bCode && a.CustomerCode==tot.usr.cCode);
                                 db.FinexecDet.RemoveRange(lin);
                                db.SaveChanges();
                                hea.Dat =ac.DateAdjustFromFrontEnd( tot.header.Dat.Value);
                                hea.Tratype = tot.header.Tratype;
                                hea.Narr = tot.header.Narr;
                                hea.BankDet = tot.header.BankDet;
                                 
                                tot.header.Usr = tot.usr.uCode;
                                sno = 1;

                                foreach (FinexecDet line in tot.lines)
                                {
                                    line.Dat=ac.DateAdjustFromFrontEnd( line.Dat.Value);
                                    line.RecordId = rec;
                                    line.Sno = sno;
                                    line.Branchid = tot.usr.bCode;
                                    line.CustomerCode = tot.usr.cCode;
                                    sno++;
                                }
                                db.FinexecDet.AddRange(tot.lines);



                                TransactionsAudit aud = new TransactionsAudit();
                                aud.TraId = rec;
                                var amt = tot.lines.Sum(a => a.Cre);
                                aud.Descr = tot.header.Tratype + " transaction of " + oldamt.ToString() + " has been changed to " + amt.ToString();
                                aud.Usr = tot.usr.uCode;
                                aud.Tratype = 2;
                                aud.Transact = tot.header.Tratype;
                                aud.TraModule = "FIN";
                                aud.Syscode = " ";
                                aud.BranchId = tot.usr.bCode;
                                aud.CustomerCode = tot.usr.cCode;
                                aud.Dat = ac.getPresentDateTime();
                                db.TransactionsAudit.Add(aud);
                                db.SaveChanges();
                                msg = "OK";

                                FinexecUniHistory audheader_upd = new FinexecUniHistory();
                                audheader_upd.RecordId = hea.RecordId;
                                audheader_upd.Seq = hea.Seq;
                                audheader_upd.Dat = hea.Dat;
                                audheader_upd.Branchid = tot.usr.bCode;
                                audheader_upd.CustomerCode = tot.usr.cCode;
                                audheader_upd.Narr = hea.Narr;
                                audheader_upd.Tratype = hea.Tratype;
                                audheader_upd.Traref = hea.Traref;
                                audheader_upd.Vouchertype = hea.Vouchertype;
                                audheader_upd.BankDet = hea.BankDet;
                                audheader_upd.Usr = hea.Usr;
                                audheader_upd.AuditDate = ac.getPresentDateTime();
                                audheader_upd.AuditType = 2;
                                db.FinexecUniHistory.Add(audheader_upd);
                                db.SaveChanges();
                                sno = 1;
                                List<FinexecDetHistory> audlines_upd = new List<FinexecDetHistory>();
                                foreach (FinexecDet line in tot.lines)
                                {
                                    audlines_upd.Add(new FinexecDetHistory
                                    {
                                        AuditId = audheader_upd.AuditId,
                                        Sno = sno,
                                        RecordId = tot.header.RecordId,
                                        Accname = line.Accname,
                                        Cre = line.Cre,
                                        Deb = line.Deb,
                                        Branchid = tot.usr.bCode,
                                        CustomerCode = tot.usr.cCode,
                                        AuditDate = audheader_upd.AuditDate,
                                        AuditType = 1,

                                    });
                                    sno++;
                                }
                                db.FinexecDetHistory.AddRange(audlines_upd);

                                db.SaveChanges();


                            }
                            else
                            {
                                msg = "You are not authorised for this transaction";
                            }
                                break;
                        case 3:
                            if (ac.screenCheck(tot.usr, 1, 2, screenChk, 3))
                            {

                                int? rec = tot.header.RecordId;
                                int? seq = db.FinexecUni.Where(a => a.RecordId == rec && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).Select(b => b.Seq).FirstOrDefault();

                                var oldamt = db.FinexecDet.Where(a => a.RecordId == rec && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).Sum(b => b.Cre);

                                var hea = db.FinexecUni.Where(a => a.RecordId == rec && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                var lin = db.FinexecDet.Where(a => a.RecordId == rec && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode);
                              
                                
                               


                             

                                TransactionsAudit aud = new TransactionsAudit();
                                aud.TraId = rec;
                                var amt = tot.lines.Sum(a => a.Cre);
                                aud.Descr = tot.header.Tratype + " transaction of " + oldamt.ToString() + " has been deleted";
                                aud.Usr = tot.usr.uCode;
                                aud.Tratype = 3;
                                aud.Transact = tot.header.Tratype;
                                aud.TraModule = "FIN";
                                aud.Syscode = " ";
                                aud.BranchId = tot.usr.bCode;
                                aud.CustomerCode = tot.usr.cCode;
                                aud.Dat = ac.getPresentDateTime();
                                db.TransactionsAudit.Add(aud);
                              

                                FinexecUniHistory audheader_del = new FinexecUniHistory();
                                audheader_del.RecordId = hea.RecordId;
                                audheader_del.Seq = hea.Seq;
                                audheader_del.Dat = hea.Dat;
                                audheader_del.Branchid = tot.usr.bCode;
                                audheader_del.CustomerCode = tot.usr.cCode;
                                audheader_del.Narr = hea.Narr;
                                audheader_del.Tratype = hea.Tratype;
                                audheader_del.Traref = hea.Traref;
                                audheader_del.Vouchertype = hea.Vouchertype;
                                audheader_del.BankDet = hea.BankDet;
                                audheader_del.Usr = hea.Usr;
                                audheader_del.AuditDate = ac.getPresentDateTime();
                                audheader_del.AuditType = 3;
                                db.FinexecUniHistory.Add(audheader_del);
                                db.SaveChanges();
                                sno = 1;
                                List<FinexecDetHistory> audlines_del = new List<FinexecDetHistory>();
                                foreach (FinexecDet line in tot.lines)
                                {
                                    audlines_del.Add(new FinexecDetHistory
                                    {
                                        AuditId = audheader_del.AuditId,
                                        Sno = sno,
                                        RecordId = tot.header.RecordId,
                                        Accname = line.Accname,
                                        Cre = line.Cre,
                                        Deb = line.Deb,
                                        Branchid = tot.usr.bCode,
                                        CustomerCode = tot.usr.cCode,
                                        AuditDate = audheader_del.AuditDate,
                                        AuditType = 1,

                                    });
                                    sno++;
                                }
                                db.FinexecDetHistory.AddRange(audlines_del);

                                db.FinexecUni.Remove(hea);
                                db.FinexecDet.RemoveRange(lin);
                                db.SaveChanges();
                                msg = "OK";

                            }
                            else
                            {
                                msg = "You are not authorised for this transaction";
                            }
                            break;
                    }
                }
                else
                {
                    msg = tmsg;
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

        private int? findTransactionId()
        {
            UsineContext db = new UsineContext();
            int? x = 0;
            var xx = db.FinexecUni.FirstOrDefault();
            if(xx!=null)
            {
                x = db.FinexecUni.Max(a => a.RecordId);
            }
            x++;
            return x;
        }
        private int? findSeq(AccountTransactionTotal tot)
        {
            UsineContext db = new UsineContext();
            AdminControl ac = new AdminControl();
            DateTime dat1 = DateTime.Parse( ac.strDate((DateTime)tot.header.Dat));
            DateTime dat2 = dat1.AddDays(1);
            int? x = 0;
            var xx = db.FinexecUni.Where(a => a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode && a.Dat >= dat1 && a.Dat < dat2).FirstOrDefault();
            if (xx != null)
            {
                x = db.FinexecUni.Where(a => a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode && a.Dat >= dat1 && a.Dat < dat2).Max(b => b.Seq);
            }
            x++;
            return x;
        }

        [HttpPost]
        [Authorize]
        [Route("api/accTransactions/accPrintVoucher")]
        public VoucherResult accPrintVoucher([FromBody]GeneralInformation inf)
        {
            VoucherResult vr = new VoucherResult();
            try
            {
                
                String quer = "select seq,dbo.strdate(dat) dats,narr,substring(tratype,5,10) tratype,vouchertype from finexecuni where recordId = " + inf.recordId.ToString() + " and customerCode=" + inf.usr.cCode.ToString();
                DataBaseContext g = new DataBaseContext();


                String seq = "", dats = "", narr = "", tratype = "", vtype = "";

                General gg = new General();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();
                if (dr.Read())
                {
                    seq = dr[0].ToString();
                    dats =   dr[1].ToString();
                    narr = dr[2].ToString();
                    tratype = dr[3].ToString();
                    vtype = dr[4].ToString();
                }
                dr.Close();
                quer = "";


                quer = quer + " select sno, b.accname, amt from";
                if (vtype == "Payment")
                    quer = quer + " (select sno, accname, deb amt from finexecdet  where recordId = " + inf.recordId.ToString() + " and deb > 0 and customerCode = " + inf.usr.cCode.ToString() + ")a,";
                else
                    quer = quer + " (select sno, accname, cre amt from finexecdet  where recordId = " + inf.recordId.ToString() + " and cre > 0 and customerCode = " + inf.usr.cCode.ToString() + ")a,";


                quer = quer + " (select * from finaccounts where customerCode = " + inf.usr.cCode.ToString() + ")b where a.accname = b.recordID order by sno";

                dc.CommandText = quer;
                dr = dc.ExecuteReader();
                String str = ho.WebRootPath + "     " + ho.ContentRootPath;
                String filename = ho.WebRootPath + "\\Reps\\" + inf.usr.uCode + "ACCVOUCHER" + inf.usr.cCode + inf.usr.bCode + ".pdf";
                LoginControlController ll = new LoginControlController();
                UserAddress addr = ll.makeBranchAddress( inf.usr.bCode, inf.usr.cCode);
                using (FileStream ms = new FileStream(filename, FileMode.Append, FileAccess.Write))

                {
                    Document document = new Document(PageSize.A4, 75f, 40f, 20f, 0f);
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);

                    document.Open();
                    PdfPTable ptot = new PdfPTable(1);
                    float[] widths = new float[] { 550f };
                    ptot.SetWidths(widths);
                    ptot.TotalWidth = 550f;
                    ptot.LockedWidth = true;
                    iTextSharp.text.Font fn;
                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 14, Font.BOLD));
                    PdfPCell plC = new PdfPCell(new Phrase(addr.branchName, fn));
                    plC.BorderWidth = 0f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot.AddCell(plC);

                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.BOLD));
                    plC = new PdfPCell(new Phrase(addr.address + ", " + addr.city, fn));
                    plC.BorderWidth = 0f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot.AddCell(plC);
                    plC = new PdfPCell(new Phrase("Ph: " + addr.tel, fn));
                    plC.BorderWidth = 0f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot.AddCell(plC);

                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.BOLD));


                     plC = new PdfPCell(new Phrase(vtype=="Payment"?"VOUCHER":"RECEIPT", fn));
                    plC.BorderWidth = 1f;
                    plC.BackgroundColor = iTextSharp.text.BaseColor.LightGray;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot.AddCell(plC);


                    plC = new PdfPCell(new Phrase("\n", fn));
                    plC.BorderWidth = 0f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot.AddCell(plC);


                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.BOLD));
                    PdfPTable pdftab = new PdfPTable(2);
                    pdftab.SetWidths(new float[] {250f,250f });
                    PdfPCell pl1 = new PdfPCell(new Phrase((vtype == "Payment" ? "Voucher # : " : "Receipt # : ") + seq, fn));
                    pl1.BorderWidth = 0f;
                    pl1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    pl1.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    pdftab.AddCell(pl1);
                    pl1 = new PdfPCell(new Phrase("Date : " + dats, fn));
                    pl1.BorderWidth = 0f;
                    pl1.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    pl1.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    pdftab.AddCell(pl1);
                    plC = new PdfPCell(pdftab);
                    plC.BorderWidth = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot.AddCell(plC);

                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.NORMAL));


                    plC = new PdfPCell(new Phrase(vtype == "Payment" ? "Paid to" : "Received with thanks from", fn));
                    plC.BorderWidth = 0f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot.AddCell(plC);

                    PdfPTable ptot1 = new PdfPTable(3);
                    float[] widths1 = new float[] { 40f,410f,100f };
                    ptot1.SetWidths(widths1);
                    ptot1.TotalWidth = 550f;
                    ptot1.LockedWidth = true;


                    int x = 1;
                    AdminControl ac = new AdminControl();
                  
                    double tot = 0;
                    while (dr.Read())
                    {
                        plC = new PdfPCell(new Phrase(dr[0].ToString(), fn));
                        plC.BorderWidth = 0f;
                        plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        ptot1.AddCell(plC);
                        plC = new PdfPCell(new Phrase(dr[1].ToString(), fn));
                        plC.BorderWidth = 0f;
                        plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        ptot1.AddCell(plC);
                        
                        plC = new PdfPCell(new Phrase( gg.makeCur(gg.valNum(dr[2].ToString()),2)  , fn));
                        plC.BorderWidth = 0f;
                        plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        ptot1.AddCell(plC);

                        tot = tot + gg.valNum(dr[2].ToString());

                        plC = new PdfPCell(ptot1);
                        plC.BorderWidth = 0f;
                        plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        ptot.AddCell(plC);
                        ptot1 = new PdfPTable(3);
                        ptot1.SetWidths(widths1);
                        ptot1.TotalWidth = 550f;
                        ptot1.LockedWidth = true;
                        x++;
                    }
                    
                     while(x <=10)
                    {
                        plC = new PdfPCell(new Phrase(" ", fn));
                        plC.BorderWidth = 0f;
                        plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        ptot1.AddCell(plC);
                        plC = new PdfPCell(new Phrase(" ", fn));
                        plC.BorderWidth = 0f;
                        plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        ptot1.AddCell(plC);
                        plC = new PdfPCell(new Phrase(" ", fn));
                        plC.BorderWidth = 0f;
                        plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        ptot1.AddCell(plC);
                        plC = new PdfPCell(ptot1);
                        plC.BorderWidth = 0f;
                        plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        ptot.AddCell(plC);
                        ptot1 = new PdfPTable(3);
                        ptot1.SetWidths(widths1);
                        ptot1.TotalWidth = 550f;
                        ptot1.LockedWidth = true;
                        x++;
                    }

                

                

                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 9, Font.BOLD));
                    plC = new PdfPCell(new Phrase(gg.makeCur(tot, 2), fn));
                    plC.BorderWidth = 0f;
                    plC.BorderWidthTop = 1;
                    plC.BorderWidthBottom = 1;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot.AddCell(plC);

                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 9, Font.ITALIC));
                    plC = new PdfPCell(new Phrase(gg.numWord(long.Parse(tot.ToString())) + " rupees only", fn));
                    plC.BorderWidth = 0f;
                    plC.BorderWidthTop = 0;
                    plC.BorderWidthBottom = 0;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot.AddCell(plC);

                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.NORMAL));
                    plC = new PdfPCell(new Phrase("Mode : " + tratype, fn));
                    plC.BorderWidth = 0f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot.AddCell(plC);

                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.NORMAL));
                    plC = new PdfPCell(new Phrase("Towards", fn));
                    plC.BorderWidth = 0f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot.AddCell(plC);
                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.ITALIC));
                    plC = new PdfPCell(new Phrase(narr, fn));
                    plC.BorderWidth = 0f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot.AddCell(plC);

                    for(var i=1;i<=10;i++)
                    {
                        plC = new PdfPCell(new Phrase("", fn));
                        plC.BorderWidth = 0f;
                        plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        ptot.AddCell(plC);

                    }

                    ptot1 = new PdfPTable(2);
                    widths1 = new float[] { 275f,275f };
                    ptot1.SetWidths(widths1);
                    ptot1.TotalWidth = 550f;
                    ptot1.LockedWidth = true;

                    plC = new PdfPCell(new Phrase("Cashier", fn));
                    plC.BorderWidth = 0f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot1.AddCell(plC);

                    plC = new PdfPCell(new Phrase(vtype == "Payment" ? "Received by" : " ", fn));
                    plC.BorderWidth = 0f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot1.AddCell(plC);

                    plC = new PdfPCell(ptot1);
                    plC.BorderWidth = 0f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot.AddCell(plC);

                    PdfPTable ptotal = new PdfPTable(1);

                    ptotal.SetWidths(widths);
                    ptotal.TotalWidth = 550f;
                    ptotal.LockedWidth = true;

                    plC = new PdfPCell(ptot);
                    plC.BorderWidth = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptotal.AddCell(plC);

                    document.Add(ptotal);

                    document.Close();
                   
                }


                dr.Close();
                g.db.Close();
                UsineContext db1 = new UsineContext();
                var header = db1.FinexecUni.Where(a => a.RecordId == inf.recordId && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                if(header != null)
                {
                    header.PrintCount=header.PrintCount+1;
                    db1.SaveChanges();
                }
               
                vr.filename = inf.usr.uCode + "ACCVOUCHER" + inf.usr.cCode + inf.usr.bCode + ".pdf";
                vr.result = "OK";

            }
            catch(Exception ee)
            {
                vr.filename = "";
                vr.result = ee.Message;
            }
            return vr;

        }

    }
}
