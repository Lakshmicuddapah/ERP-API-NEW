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

namespace Usine_Core.Controllers.Purchases
{
    public class TotalAdvancesTotal
    {
        public TotAdvanceDetails advance{ get; set; }
         public int? tracheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
        public string fname { get; set; }
         
    }
     public class TotalAdvanceRequirement
    {
        public string voucher { get; set; }
        public List<FinAccounts> accounts { get; set; }
    }
    
    public class TotalAdvancesController : Controller
    {

        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        private readonly IHostingEnvironment ho;
        public TotalAdvancesController(IHostingEnvironment hostingEnvironment)
        {
            ho = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        [Route("api/purSetup/GetTotalAdvanceRequirements")]
        public TotalAdvanceRequirement GetTotalAdvanceRequirements([FromBody]UserInfo usr)
        {
            try
            {
                TotalAdvancesTotal tot = new TotalAdvancesTotal();
                tot.advance = new TotAdvanceDetails();
                tot.advance.Dat = ac.getPresentDateTime();
                tot.usr = usr;
                TotalAdvanceRequirement req = new TotalAdvanceRequirement();
                req.voucher=findAdvanceSeq(tot);
                req.accounts = db.FinAccounts.Where(a => (a.AcType == "CAS" || a.AcType == "BAN" || a.AcType == "MOB") && a.CustomerCode == usr.cCode).OrderBy(b => b.Accname).ToList();    
                return req;
            }
            catch(Exception ee)
            {
                return null; 
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/purSetup/GetPendingOrders")]
        public List<PurPurchaseOrderUni> GetPendingOrders([FromBody] GeneralInformation inf)
        {
            try
            {
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                DateTime dat = ac.getPresentDateTime();
               if(inf.recordId ==1)
                {
                    return db.PurPurchaseOrderUni.Where(a => a.Dat >= dat1 && a.Pos <=3 && a.Dat <= dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();
                }
               else
                {
                    return db.PurPurchaseOrderUni.Where(a => a.Dat >= dat1 && a.Validity >= dat && a.Pos <= 3 && a.Dat <= dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();
                }
            }
            catch (Exception ee)
            {
                return null;
            }
        }
        [HttpPost]
        [Authorize]
        [Route("api/purSetup/GetPendingSaleOrders")]
        public List<CrmSaleOrderUni> GetPendingSaleOrders([FromBody] GeneralInformation inf)
        {
            try
            {
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                DateTime dat = ac.getPresentDateTime();
                if (inf.recordId == 1)
                {
                    return db.CrmSaleOrderUni.Where(a => a.Dat >= dat1 && a.Pos <= 3 && a.Dat <= dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();
                }
                else
                {
                    return db.CrmSaleOrderUni.Where(a => a.Dat >= dat1 && a.Validity >= dat && a.Pos <= 3 && a.Dat <= dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();
                }
            }
            catch (Exception ee)
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/purSetup/GetTotalAdvanceDetails")]
        public List<TotAdvanceDetails> GetTotalAdvanceDetails([FromBody]GeneralInformation inf)
        {

            return db.TotAdvanceDetails.Where(a => a.TransactionId == inf.recordId && a.Tratype == inf.detail && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();

        }



        [HttpPost]
        [Authorize]
        [Route("api/purSetup/SetTotalAdvanceDetails")]
        public TotalAdvancesTotal SetTotalAdvanceDetails([FromBody]TotalAdvancesTotal tot)
        {
            string msg = "";
            try
            {
                string tramsg = ac.transactionCheck("Inventory", tot.advance.Dat, tot.usr);
                if (tramsg == "OK")
                {
                    if (ac.screenCheck(tot.usr, 2, 3, 4, (int)tot.tracheck))
                    {
                        switch (tot.tracheck)
                        {
                            case 1:
                                using (var txn = db.Database.BeginTransaction())
                                {
                                    try
                                    {

                                        tot.advance.Seq = findAdvanceSeq(tot);
                                        tot.advance.UsrName = tot.usr.uCode;
                                        tot.advance.BranchId = tot.usr.bCode;
                                        tot.advance.CustomerCode = tot.usr.cCode;
                                        tot.advance.PrintCount = 0;
                                        db.TotAdvanceDetails.Add(tot.advance);

                                        db.SaveChanges();

                                        TransactionsAudit aud = new TransactionsAudit();
                                        aud.TraId = tot.advance.RecordId;

                                        aud.Descr = "An advance for Blank order of " + tot.advance.Amt.ToString() + " has been created";
                                        aud.Usr = tot.usr.uCode;
                                        aud.Tratype = 1;
                                        aud.Transact = "BLA_VOU";
                                        aud.TraModule = "PUR";
                                        aud.Syscode = " ";
                                        aud.BranchId = tot.usr.bCode;
                                        aud.CustomerCode = tot.usr.cCode;
                                        aud.Dat = ac.getPresentDateTime();
                                        db.TransactionsAudit.Add(aud);

                                        db.SaveChanges();
                                        txn.Commit();
                                        msg = "OK";
                                    }
                                    catch (Exception eee)
                                    {
                                        txn.Rollback();
                                        msg = eee.Message;
                                    }
                                }
                                break;
                            case 2:
                                var advupd = db.TotAdvanceDetails.Where(a => a.RecordId == tot.advance.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                var prevamt = advupd.Amt;
                                var premode = advupd.Paymentmode;
                                if (advupd != null)
                                {
                                    advupd.Dat = tot.advance.Dat;
                                    advupd.Amt = tot.advance.Amt;
                                    advupd.UsrName = tot.usr.uCode;
                                    advupd.Bankdetails = tot.advance.Bankdetails;
                                    advupd.Paymentmode = tot.advance.Paymentmode;


                                    TransactionsAudit audupd = new TransactionsAudit();
                                    audupd.TraId = tot.advance.RecordId;

                                    audupd.Descr = "An advance of " + advupd.Seq + " receipt number changed from  " + prevamt.ToString() + " to " + tot.advance.Amt.ToString() + " and from " + premode + " to " + tot.advance.Paymentmode;
                                    audupd.Usr = tot.usr.uCode;
                                    audupd.Tratype = 2;
                                    audupd.Transact = "BLA_ADV";
                                    audupd.TraModule = "PUR";
                                    audupd.Syscode = " ";
                                    audupd.BranchId = tot.usr.bCode;
                                    audupd.CustomerCode = tot.usr.cCode;
                                    audupd.Dat = ac.getPresentDateTime();
                                    db.TransactionsAudit.Add(audupd);
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "No record found";
                                }
                                break;
                            case 3:
                                var advdel = db.TotAdvanceDetails.Where(a => a.RecordId == tot.advance.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                var delamt = advdel.Amt;
                                var delmode = advdel.Paymentmode;
                                if (advdel != null)
                                {
                                    db.TotAdvanceDetails.Remove(advdel);

                                    TransactionsAudit auddel = new TransactionsAudit();
                                    auddel.TraId = tot.advance.RecordId;

                                    auddel.Descr = "An advance of " + advdel.Seq + " receipt number cancelled payment of " + delamt.ToString() + " and mode of " + delmode ;
                                    auddel.Usr = tot.usr.uCode;
                                    auddel.Tratype = 3;
                                    auddel.Transact = "BLA_ADV";
                                    auddel.TraModule = "PUR";
                                    auddel.Syscode = " ";
                                    auddel.BranchId = tot.usr.bCode;
                                    auddel.CustomerCode = tot.usr.cCode;
                                    auddel.Dat = ac.getPresentDateTime();
                                    db.TransactionsAudit.Add(auddel);
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
                else
                {
                    msg = tramsg;
                }
            }
            catch(Exception ee)
            {
                msg = ee.Message;
            }

            tot.result = msg;
            return tot;
        }
        
        private string findAdvanceSeq(TotalAdvancesTotal tot)
        {
            General g = new General();
            DateTime d1 = ac.getFinancialStart(tot.advance.Dat.Value, tot.usr);
            DateTime d2 = d1.AddYears(1).AddDays(-1);
            string title = tot.advance.Tratype == "PUR_VOU" ? "ADVO" : "ADRE";
            string pref=title + d1.Year.ToString().Substring(2,2)   + d2.Year.ToString().Substring(2, 2) + "_";
            int x = 0;
            var detail = db.TotAdvanceDetails.Where(a => a.Dat >= d1 && a.Dat <= d2 && a.Seq.Contains(pref) && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).Max(a => a.Seq);
            if(detail != null)
            {
                x = int.Parse( detail.Substring(9, 6));
            }
            x++;
            return pref + g.zeroMake(x, 6);
        }



        [HttpPost]
        [Authorize]
        [Route("api/purSetup/PrintAdvanceInfo")]
        public TotalAdvancesTotal PrintAdvanceInfo([FromBody] TotalAdvancesTotal tot)
        {
            string msg = "";

            try
            {
                DateTime dat = DateTime.Now;
                 
                string fna = tot.usr.uCode + "ADVVOUCHER" + dat.Hour.ToString() + dat.Minute.ToString() + dat.Second.ToString() + tot.usr.cCode + tot.usr.bCode + ".pdf";
                String filename = ho.WebRootPath + "\\Reps\\" + fna;
                LoginControlController ll = new LoginControlController();
                UserAddress addr = ll.makeBranchAddress(tot.usr.bCode, tot.usr.cCode);
                string party = "", seq = "" ;
                switch(tot.advance.Tratype)
                {
                    case "PUR_VOU":
                        var blaord = db.PurPurchaseOrderUni.Where(a => a.RecordId == tot.advance.TransactionId).FirstOrDefault();
                        party = blaord.Vendorname;
                        seq=blaord.Seq;
                        break;
                    case "SAL_REC":
                        var sord = db.CrmSaleOrderUni.Where(a => a.RecordId == tot.advance.TransactionId).FirstOrDefault();
                        party = sord.PartyName;
                        seq = sord.Seq;
                        break;
                }
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
                    General g = new General();
                    string vtype =g.right( tot.advance.Tratype,3);
                    plC = new PdfPCell(new Phrase(vtype == "VOU" ? "VOUCHER" : "RECEIPT", fn));
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
                    pdftab.SetWidths(new float[] { 250f, 250f });
                    PdfPCell pl1 = new PdfPCell(new Phrase((vtype == "VOU" ? "Voucher # : " : "Receipt # : ") + tot.advance.Seq, fn));
                    pl1.BorderWidth = 0f;
                    pl1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    pl1.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    pdftab.AddCell(pl1);
                    string dats = g.strDate(tot.advance.Dat.Value);
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


                    plC = new PdfPCell(new Phrase(vtype == "VOU" ? "Paid to" : "Received with thanks from", fn));
                    plC.BorderWidth = 0f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot.AddCell(plC);

                    PdfPTable ptot1 = new PdfPTable(3);
                    float[] widths1 = new float[] { 40f, 410f, 100f };
                    ptot1.SetWidths(widths1);
                    ptot1.TotalWidth = 550f;
                    ptot1.LockedWidth = true;
                    int x = 1;
                    AdminControl ac = new AdminControl();

                    plC = new PdfPCell(new Phrase(" ", fn));
                    plC.BorderWidth = 0f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot1.AddCell(plC);
                    plC = new PdfPCell(new Phrase(party, fn));
                    plC.BorderWidth = 0f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot1.AddCell(plC);

                    plC = new PdfPCell(new Phrase(g.makeCur((double)tot.advance.Amt, 2), fn));
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



                    while (x <= 10)
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
                    plC = new PdfPCell(new Phrase(g.makeCur((double)tot.advance.Amt, 2), fn));
                    plC.BorderWidth = 0f;
                    plC.BorderWidthTop = 1;
                    plC.BorderWidthBottom = 1;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot.AddCell(plC);

                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 9, Font.ITALIC));
                    plC = new PdfPCell(new Phrase(g.numWord(long.Parse(tot.advance.Amt.ToString())) + " rupees only", fn));
                    plC.BorderWidth = 0f;
                    plC.BorderWidthTop = 0;
                    plC.BorderWidthBottom = 0;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot.AddCell(plC);

                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.NORMAL));
                    plC = new PdfPCell(new Phrase("Mode : " + tot.advance.Paymentmode, fn));
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
                    plC = new PdfPCell(new Phrase((vtype != "VOU" ? "Received " : "Paid ") + " advance amount for our " + (tot.advance.Tratype=="PUR_VOU"?"Purchase":"Sale") + " order no " + seq, fn));
                    plC.BorderWidth = 0f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot.AddCell(plC);


                    for (var i = 1; i <= 10; i++)
                    {
                        plC = new PdfPCell(new Phrase("", fn));
                        plC.BorderWidth = 0f;
                        plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        ptot.AddCell(plC);

                    }

                    ptot1 = new PdfPTable(2);
                    widths1 = new float[] { 275f, 275f };
                    ptot1.SetWidths(widths1);
                    ptot1.TotalWidth = 550f;
                    ptot1.LockedWidth = true;

                    plC = new PdfPCell(new Phrase("Cashier", fn));
                    plC.BorderWidth = 0f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot1.AddCell(plC);

                    plC = new PdfPCell(new Phrase(vtype == "VOU" ? "Received by" : "Paid To", fn));
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


                    tot.fname = fna;
                
                msg = "OK";
                var det = db.TotAdvanceDetails.Where(a => a.RecordId == tot.advance.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                if(det != null)
                {
                    det.PrintCount = det.PrintCount + 1;
                    db.SaveChanges();
                }
            }
            catch (Exception ee)
            {
                msg = ee.Message;
            }

            tot.result= msg;
            return tot;

        }

    }
}
