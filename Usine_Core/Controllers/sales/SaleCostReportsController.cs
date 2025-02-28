using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Controllers.Others;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
namespace Usine_Core.Controllers.sales
{
   
    
        public class PurSupplierDayBookInfo
        {
            public string seq { get; set; }
            public DateTime? dat { get; set; }
            public string vendor { get; set; }
            public double? purchase { get; set; }
            public double? preturn { get; set; }
            public double? crnote { get; set; }
            public double? payment { get; set; }
        }
        public class PurSupplierDayBookTotal
        {
            public List<PurSupplierDayBookInfo> details { get; set; }
            public string pdfFile { get; set; }
            public string excelFile { get; set; }
        }
        public class PurSupplierLedgerInfo
        {
            public string dat { get; set; }
            public string opb { get; set; }
            public string pur { get; set; }
            public string ret { get; set; }
            public string cno { get; set; }
            public string pay { get; set; }
            public string clb { get; set; }
        }
        public class PurSupplierLedgerTotal
        {
            public List<PurSupplierLedgerInfo> details { get; set; }
            public string pdfFile { get; set; }
            public string excelFile { get; set; }
        }
        public class SaleCostReportsController : ControllerBase
        {
            private readonly IHostingEnvironment ho;
            public SaleCostReportsController(IHostingEnvironment hostingEnvironment)
            {
                ho = hostingEnvironment;
            }

            UsineContext db = new UsineContext();
            AdminControl ac = new AdminControl();

            [HttpPost]
            [Authorize]
            [Route("api/SaleCostReports/SalRepCustomerDayBook")]
            public PurSupplierDayBookTotal SalRepCustomerDayBook([FromBody] GeneralInformation inf)
            {
                if (ac.screenCheck(inf.usr, 5, 9, 1, 0))
                {
                    try
                    {
                        DateTime dat1 = DateTime.Parse(inf.frmDate);
                        DateTime dat2 = DateTime.Parse(inf.frmDate).AddDays(1);

                        PurSupplierDayBookTotal tot = new PurSupplierDayBookTotal();
                        tot.details = new List<PurSupplierDayBookInfo>();
                        List<PurSupplierDayBookInfo> details = new List<PurSupplierDayBookInfo>();
                          var det1 = (from a in db.SalSalesUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                    select new PurSupplierDayBookInfo
                                    {
                                        seq = a.Seq,
                                        dat = a.Dat.Value,
                                        vendor = a.PartyName,
                                        purchase = a.TotalAmt,
                                        preturn = 0,
                                        crnote = 0,
                                        payment = 0
                                    }).ToList();
                        if (det1.Count() > 0)
                        {
                            details.AddRange(det1);
                        }

                        var det2 = (from a in db.SalSaleReturnsUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                    select new PurSupplierDayBookInfo
                                    {
                                        seq = a.Seq,
                                        dat = a.Dat.Value,
                                        vendor = a.PartyName,
                                        purchase = 0,
                                        preturn = a.TotalAmt,
                                        crnote = 0,
                                        payment = 0
                                    }).ToList();
                        if (det2.Count() > 0)
                        {
                            details.AddRange(det2);
                        }

                        var det3 = (from a in db.PartyCreditDebitNotes.Where(a => a.TransactionType == "SDR" && a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                    join b in db.SalSalesUni.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.TransactionId equals b.RecordId
                                    select new PurSupplierDayBookInfo
                                    {
                                        seq = a.Seq,
                                        dat = a.Dat,
                                        vendor = b.PartyName,
                                        purchase = 0,
                                        preturn = 0,
                                        crnote = a.Amt,
                                        payment = 0
                                    }).ToList();
                        if (det3.Count() > 0)
                        {
                            details.AddRange(det3);
                        }
                        var det4 = (from a in db.PartyPaymentsuni.Where(a => a.VoucherType == "SAL_REC" && a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                    join b in db.PartyDetails.Where(a => a.CustomerCode == inf.usr.cCode) on a.PartyId equals b.RecordId
                                    select new PurSupplierDayBookInfo
                                    {
                                        seq = a.Seq,
                                        dat = a.Dat,
                                        vendor = b.PartyName,
                                        purchase = 0,
                                        preturn = 0,
                                        crnote = 0,
                                        payment = a.BaseAmt

                                    }).ToList();
                        if (det4.Count() > 0)
                        {
                            details.AddRange(det4);
                        }
                        if (details.Count() > 0)
                        {
                            tot.details.AddRange(details);
                        }
                        tot.details.Add(new PurSupplierDayBookInfo
                        {
                            seq = "",
                            vendor = ""
                        });
                        tot.details.Add(new PurSupplierDayBookInfo
                        {
                            seq = "",
                            vendor = "Total",
                            purchase = details.Sum(a => a.purchase),
                            preturn = details.Sum(a => a.preturn),
                            crnote = details.Sum(a => a.crnote),
                            payment = details.Sum(a => a.payment)
                        });

                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));
                        dt.Columns.Add("seq", typeof(string));
                        dt.Columns.Add("vendor", typeof(string));
                        dt.Columns.Add("dat", typeof(string));
                        dt.Columns.Add("purchase", typeof(string));
                        dt.Columns.Add("preturn", typeof(string));
                        dt.Columns.Add("crnote", typeof(string));
                        dt.Columns.Add("payment", typeof(string));
                        int i = 1;

                        General gg = new General();
                        foreach (var det in tot.details)
                        {
                            dt.Rows.Add(i <= tot.details.Count() - 2 ? i.ToString() : "", det.seq, det.vendor, det.dat == null ? "" : gg.strTime(det.dat.Value), det.purchase == null ? " " : gg.makeCur((double)det.purchase, 2), det.preturn == null ? " " : gg.makeCur((double)det.preturn, 2), det.crnote == null ? " " : gg.makeCur((double)det.crnote, 2), det.payment == null ? " " : gg.makeCur((double)det.payment, 2));
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Seq");
                        titles.Add("Supplier");
                        titles.Add("Time");
                        titles.Add("Purchase");
                        titles.Add("Return");
                        titles.Add("Cr Note");
                        titles.Add("Payment");
                        float[] widths = { 30f, 90f, 200f, 80f, 80f, 80f, 80f, 80f };
                        int[] aligns = { 3, 1, 1, 1, 2, 2, 2, 2 };
                        PDFExcelMake ep = new PDFExcelMake();

                        DateTime dats = DateTime.Now;
                        string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname = inf.usr.uCode + "SalCostRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                        string filename = ho.WebRootPath + "\\Reps\\" + fname;
                        string msg = "";

                        msg = ep.pdfLandscapeConversion(filename, "Sales Daybook on " + inf.frmDate, inf.usr, dt, titles, widths, aligns, true);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname;
                        }
                        string fname1 = inf.usr.uCode + "SalCostRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, "Sales Daybook on " + inf.frmDate, inf.usr, dt, titles, widths, aligns, true);
                        if (msg == "OK")
                        {
                            tot.excelFile = fname1;
                        }

                        return tot;


                    }
                    catch
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }

            [HttpPost]
            [Authorize]
            [Route("api/SaleCostReports/SalRepCustomerLedger")]
            public PurSupplierLedgerTotal SalRepCustomerLedger([FromBody] GeneralInformation inf)
            {
                if (ac.screenCheck(inf.usr, 5, 9, 1, 0))
                {
                    General gg = new General();
                    string dat1 = inf.frmDate;
                    string dat2 = gg.strDate(DateTime.Parse(inf.toDate).AddDays(1));
                    PurSupplierLedgerTotal tot = new PurSupplierLedgerTotal();
                    tot.details = new List<PurSupplierLedgerInfo>();
                    string quer = "";
                    quer = quer + " select dat,dbo.makCur(opb) opb,dbo.makCur(puramt) puramt,dbo.makcur(retamt) retamt,";
                    quer = quer + " dbo.makcur(crenote) crenot,dbo.makcur(payment) payment,dbo.makcur(closing) closing from";
                    quer = quer + " (select 'Opening Balance' dat,sum(pendingamount - returnamount - creditnote - paymentamount) opb,0 puramt,0 retamt,0 crenote,0 payment,0 closing from partyTransactions";
                    quer = quer + " where partyId = " + inf.recordId + " and dat< '" + dat1 + "' and branchId = '" + inf.usr.bCode + "' and customercode = " + inf.usr.cCode + "";
                    quer = quer + " union all";
                    quer = quer + " select dbo.strDate(dat) dat,0 opb, pendingamount puramt, returnamount retamt,creditnote crenote, paymentamount payment,0 closing from partyTransactions";
                    quer = quer + " where partyId = " + inf.recordId + " and dat >= '" + dat1 + "' and dat< '" + dat2 + "' and branchId = '" + inf.usr.bCode + "' and customercode = " + inf.usr.cCode + "";
                    quer = quer + " union all";
                    quer = quer + " select 'Closing Balance' dat,0 opb,0 puramt,0 retamt,0 crenote,0 payment,sum(pendingamount - returnamount - creditnote - paymentamount)  closing from partyTransactions";
                    quer = quer + " where partyId = " + inf.recordId + " and dat< '" + dat2 + "' and branchId = '" + inf.usr.bCode + "' and customercode = " + inf.usr.cCode + ")x";
                    DataBaseContext g = new DataBaseContext();
                    SqlCommand dc = new SqlCommand();
                    dc.Connection = g.db;
                    g.db.Open();
                    dc.CommandText = quer;
                    SqlDataReader dr = dc.ExecuteReader();

                    while (dr.Read())
                    {
                        tot.details.Add(new PurSupplierLedgerInfo
                        {
                            dat = dr[0].ToString(),
                            opb = dr[1].ToString(),
                            pur = dr[2].ToString(),
                            ret = dr[3].ToString(),
                            cno = dr[4].ToString(),
                            pay = dr[5].ToString(),
                            clb = dr[6].ToString()
                        });

                    }
                    dr.Close();
                    g.db.Close();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("sno", typeof(string));
                    dt.Columns.Add("dat", typeof(string));
                    dt.Columns.Add("opb", typeof(string));
                    dt.Columns.Add("sale", typeof(string));
                    dt.Columns.Add("sreturn", typeof(string));
                    dt.Columns.Add("drnote", typeof(string));
                    dt.Columns.Add("receipt", typeof(string));
                    dt.Columns.Add("clb", typeof(string));
                    int i = 0;

                    foreach (var det in tot.details)
                    {
                        dt.Rows.Add(i <= tot.details.Count() - 1 && i > 0 ? i.ToString() : "", det.dat, det.opb, det.pur, det.ret, det.cno, det.pay, det.clb);
                        i++;
                    }

                    List<string> titles = new List<string>();
                    titles.Add("#");
                    titles.Add("Date");
                    titles.Add("Opening");
                    titles.Add("Sale");
                    titles.Add("Return");
                    titles.Add("Dr Note");
                    titles.Add("Receipt");
                    titles.Add("Closing");
                    float[] widths = { 30f, 100f, 70f, 70f, 70f, 70f, 70f, 70f };
                    int[] aligns = { 3, 1, 2, 2, 2, 2, 2, 2 };
                    PDFExcelMake ep = new PDFExcelMake();

                    DateTime dats = DateTime.Now;
                    string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                    string fname = inf.usr.uCode + "PurCostRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                    string filename = ho.WebRootPath + "\\Reps\\" + fname;
                    string msg = "";

                    msg = ep.pdfLandscapeConversion(filename, "Customer Ledger from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
                    if (msg == "OK")
                    {
                        tot.pdfFile = fname;
                    }
                    string fname1 = inf.usr.uCode + "PurCostRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                    string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                    msg = "";
                    msg = ep.makeExcelConversion(filename1, "Customer Ledger from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
                    if (msg == "OK")
                    {
                        tot.excelFile = fname1;
                    }

                    return tot;


                }
                else
                {
                    return null;
                }

            }

            [HttpPost]
            [Authorize]
            [Route("api/SaleCostReports/SalRepCustomerSnapShot")]
            public PurSupplierLedgerTotal SalRepCustomerSnapShot([FromBody] UserInfo usr)
            {
                if (ac.screenCheck(usr, 5, 9, 1, 0))
                {
                    General gg = new General();
                    string dat1 = gg.strDate(ac.getFinancialStart(ac.getPresentDateTime(), usr));
                    DataBaseContext g = new DataBaseContext();
                    PurSupplierLedgerTotal tot = new PurSupplierLedgerTotal();
                    tot.details = new List<PurSupplierLedgerInfo>();
                    string quer = "";
                    quer = quer + " select b.partyname,dbo.makcur(opb) opb,dbo.makcur(puramt) puramt,dbo.makcur(retamt) retamt,";
                    quer = quer + " dbo.makcur(crenote) crenote,dbo.makcur(payment) payment,dbo.makcur(clb) clb from";
                    quer = quer + " (select partyid, sum(opb) opb, sum(puramt) puramt, sum(retamt) retamt, sum(crenote) crenote, sum(payment) payment,";
                    quer = quer + " sum(opb+puramt - retamt - crenote - payment) clb from";
                    quer = quer + " (select partyid, sum(pendingamount -returnamount - creditnote - paymentamount) opb,0 puramt,0 retamt,0 crenote,0 payment from partyTransactions";
                    quer = quer + " where dat <= '" + dat1 + "' and customerCode = " + usr.cCode + " group by partyid  union all";
                    quer = quer + " select partyid,0 opb,sum(pendingamount) puramt,sum(returnamount) retamt,sum(creditnote) crenote,";
                    quer = quer + " sum(paymentamount) payment from partyTransactions where dat > '" + dat1 + "' and customerCode = " + usr.cCode + " group by partyid)x group by partyid)a,";
                    quer = quer + " (select * from partydetails where partyType = 'CUS' and customerCode = " + usr.cCode + ")b where a.partyId = b.recordId order by partyname";

                    SqlCommand dc = new SqlCommand();
                    dc.Connection = g.db;
                    g.db.Open();
                    dc.CommandText = quer;
                    SqlDataReader dr = dc.ExecuteReader();

                    while (dr.Read())
                    {
                        tot.details.Add(new PurSupplierLedgerInfo
                        {
                            dat = dr[0].ToString(),
                            opb = dr[1].ToString(),
                            pur = dr[2].ToString(),
                            ret = dr[3].ToString(),
                            cno = dr[4].ToString(),
                            pay = dr[5].ToString(),
                            clb = dr[6].ToString()
                        });

                    }
                    dr.Close();
                    g.db.Close();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("sno", typeof(string));
                    dt.Columns.Add("sup", typeof(string));
                    dt.Columns.Add("opb", typeof(string));
                    dt.Columns.Add("sale", typeof(string));
                    dt.Columns.Add("sreturn", typeof(string));
                    dt.Columns.Add("drnote", typeof(string));
                    dt.Columns.Add("receipt", typeof(string));
                    dt.Columns.Add("clb", typeof(string));
                    int i = 1;

                    foreach (var det in tot.details)
                    {
                        dt.Rows.Add(i.ToString(), det.dat, det.opb, det.pur, det.ret, det.cno, det.pay, det.clb);
                        i++;
                    }

                    List<string> titles = new List<string>();
                    titles.Add("#");
                    titles.Add("Customer");
                    titles.Add("Opening");
                    titles.Add("Sale");
                    titles.Add("Return");
                    titles.Add("Dr Note");
                    titles.Add("Receipt");
                    titles.Add("Closing");
                    float[] widths = { 30f, 100f, 70f, 70f, 70f, 70f, 70f, 70f };
                    int[] aligns = { 3, 1, 2, 2, 2, 2, 2, 2 };
                    PDFExcelMake ep = new PDFExcelMake();

                    DateTime dats = ac.getPresentDateTime();
                    string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                    string fname = usr.uCode + "PurCostRep" + dat + usr.cCode + usr.bCode + ".pdf";
                    string filename = ho.WebRootPath + "\\Reps\\" + fname;
                    string msg = "";

                    msg = ep.pdfLandscapeConversion(filename, "Customer Snap Shot as on " + gg.strDateTime(dats), usr, dt, titles, widths, aligns, false);
                    if (msg == "OK")
                    {
                        tot.pdfFile = fname;
                    }
                    string fname1 = usr.uCode + "PurCostRep" + dat + usr.cCode + usr.bCode + ".xlsx";
                    string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                    msg = "";
                    msg = ep.makeExcelConversion(filename1, "Customer Snap Shot as on " + gg.strDateTime(dats), usr, dt, titles, widths, aligns, false);
                    if (msg == "OK")
                    {
                        tot.excelFile = fname1;
                    }

                    return tot;


                }
                else
                {
                    return null;
                }
            }


            [HttpPost]
            [Authorize]
            [Route("api/SaleCostReports/SalRepCustomerAgingDetails")]
            public PurSupplierLedgerTotal SalRepCustomerAgingDetails([FromBody] UserInfo usr)
            {
                if (ac.screenCheck(usr, 5, 9, 1, 0))
                {
                    General gg = new General();
                    string dat1 = gg.strDate(ac.getFinancialStart(ac.getPresentDateTime(), usr));
                    DataBaseContext g = new DataBaseContext();
                    PurSupplierLedgerTotal tot = new PurSupplierLedgerTotal();
                    tot.details = new List<PurSupplierLedgerInfo>();
                    string quer = "";
                    quer = quer + " select b.partyname,dbo.makCur(balance1) balance1,dbo.makCur(balance2) balance2,";
                    quer = quer + " dbo.makCur(balance3) balance3,dbo.makCur(balance4) balance4,";
                    quer = quer + " dbo.makCur(balance1 + balance2 + balance3 + balance4) totbalance from";
                    quer = quer + " (select partyid, sum(balance1) balance1, sum(balance2) balance2, sum(balance3) balance3, sum(balance4) balance4 from";
                    quer = quer + " (select a.transactionId, partyid, pendingAmount-case when cleared is null then 0 else cleared end balance1,";
                    quer = quer + " 0 balance2,0 balance3,0 balance4 from";
                    quer = quer + " (select transactionId, partyid, pendingAmount from partyTransactions where transactionType= 'SAL' and";
                    quer = quer + "  DATEDIFF(DD, dat, sysdatetime()) <= 30 )a left outer join";
                    quer = quer + " (select onTraid, sum(returnAmount+creditnote + paymentamount) cleared from";
                    quer = quer + " partyTransactions where customerCode = " + usr.cCode + " group by onTraId)b on a.transactionId = b.onTraId";
                    quer = quer + " union all";
                    quer = quer + " select a.transactionId,partyid,0 balance1, pendingAmount -case when cleared is null then 0 else cleared end balance2,";
                    quer = quer + " 0 balance3,0 balance4 from";
                    quer = quer + " (select transactionId, partyid, pendingAmount from partyTransactions where transactionType= 'SAL' and";
                    quer = quer + " DATEDIFF(DD, dat, sysdatetime()) > 30 and DATEDIFF(DD, dat, sysdatetime()) <= 60 )a left outer join";
                    quer = quer + " (select onTraid, sum(returnAmount+creditnote + paymentamount) cleared from";
                    quer = quer + " partyTransactions where customerCode = " + usr.cCode + " group by onTraId)b on a.transactionId = b.onTraId";
                    quer = quer + " union all";
                    quer = quer + " select a.transactionId,partyid,0 balance1, 0 balance2,";
                    quer = quer + " pendingAmount -case when cleared is null then 0 else cleared end balance3, 0 balance4 from";
                    quer = quer + " (select transactionId, partyid, pendingAmount from partyTransactions where transactionType= 'SAL' and";
                    quer = quer + " DATEDIFF(DD, dat, sysdatetime()) > 60 and DATEDIFF(DD, dat, sysdatetime()) <= 90 )a left outer join";
                    quer = quer + " (select onTraid, sum(returnAmount+creditnote + paymentamount) cleared from";
                    quer = quer + " partyTransactions where customerCode = " + usr.cCode + " group by onTraId)b on a.transactionId = b.onTraId";
                    quer = quer + " union all";
                    quer = quer + " select a.transactionId,partyid,0 balance1, 0 balance2,";
                    quer = quer + " pendingAmount -case when cleared is null then 0 else cleared end balance3, 0 balance4 from";
                    quer = quer + " (select transactionId, partyid, pendingAmount from partyTransactions where transactionType= 'SAL' and";
                    quer = quer + " DATEDIFF(DD, dat, sysdatetime()) > 90   )a left outer join";
                    quer = quer + " (select onTraid, sum(returnAmount+creditnote + paymentamount) cleared from";
                    quer = quer + " partyTransactions where customerCode = " + usr.cCode + " group by onTraId)b on a.transactionId = b.onTraId)x group by partyid)a,";
                    quer = quer + " (select * from partyDetails where partytype = 'CUS' and customerCode = " + usr.cCode + ")b where a.partyId = b.recordId";
                    quer = quer + " order by partyname";
                    SqlCommand dc = new SqlCommand();
                    dc.Connection = g.db;
                    g.db.Open();
                    dc.CommandText = quer;
                    SqlDataReader dr = dc.ExecuteReader();
                    var details = new List<PurSupplierLedgerInfo>();
                    while (dr.Read())
                    {
                        details.Add(new PurSupplierLedgerInfo
                        {
                            dat = dr[0].ToString(),
                            opb = dr[1].ToString(),
                            pur = dr[2].ToString(),
                            ret = dr[3].ToString(),
                            cno = dr[4].ToString(),
                            pay = dr[5].ToString(),
                        });

                    }



                    dr.Close();
                    g.db.Close();
                    if (details.Count() > 0)
                        tot.details.AddRange(details);
                    double bal1 = 0, bal2 = 0, bal3 = 0, bal4 = 0, tbal = 0;
                    foreach (var det in details)
                    {

                        bal1 = bal1 + gg.valNum(gg.removeKama(det.opb));
                        bal2 = bal2 + gg.valNum(gg.removeKama(det.pur));
                        bal3 = bal3 + gg.valNum(gg.removeKama(det.ret));
                        bal4 = bal4 + gg.valNum(gg.removeKama(det.cno));
                        tbal = tbal + gg.valNum(gg.removeKama(det.pay));

                    }
                    tot.details.Add(new PurSupplierLedgerInfo
                    {
                        dat = ""
                    });
                    tot.details.Add(new PurSupplierLedgerInfo
                    {
                        dat = "Total",
                        opb = gg.makeCur(bal1, 2),
                        pur = gg.makeCur(bal2, 2),
                        ret = gg.makeCur(bal3, 2),
                        cno = gg.makeCur(bal4, 2),
                        pay = gg.makeCur(tbal, 2),
                    });

                    DataTable dt = new DataTable();
                    dt.Columns.Add("sno", typeof(string));
                    dt.Columns.Add("sup", typeof(string));
                    dt.Columns.Add("bal1", typeof(string));
                    dt.Columns.Add("bal2", typeof(string));
                    dt.Columns.Add("bal3", typeof(string));
                    dt.Columns.Add("bal4", typeof(string));
                    dt.Columns.Add("tbal", typeof(string));
                    int i = 1;

                    foreach (var det in tot.details)
                    {
                        dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : "", det.dat, det.opb, det.pur, det.ret, det.cno, det.pay);
                        i++;
                    }

                    List<string> titles = new List<string>();
                    titles.Add("#");
                    titles.Add("Customer");
                    titles.Add("0-30");
                    titles.Add("30-60");
                    titles.Add("60-90");
                    titles.Add(">90");
                    titles.Add("Total");
                    float[] widths = { 30f, 145f, 75f, 75f, 75f, 75f, 75f };
                    int[] aligns = { 3, 1, 2, 2, 2, 2, 2 };
                    PDFExcelMake ep = new PDFExcelMake();

                    DateTime dats = ac.getPresentDateTime();
                    string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                    string fname = usr.uCode + "PurCostRep" + dat + usr.cCode + usr.bCode + ".pdf";
                    string filename = ho.WebRootPath + "\\Reps\\" + fname;
                    string msg = "";

                    msg = ep.pdfConversion(filename, "Customer aging details as on " + gg.strDateTime(dats), usr, dt, titles, widths, aligns, true);
                    if (msg == "OK")
                    {
                        tot.pdfFile = fname;
                    }
                    string fname1 = usr.uCode + "PurCostRep" + dat + usr.cCode + usr.bCode + ".xlsx";
                    string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                    msg = "";
                    msg = ep.makeExcelConversion(filename1, "Customer aging details as on " + gg.strDateTime(dats), usr, dt, titles, widths, aligns, true);
                    if (msg == "OK")
                    {
                        tot.excelFile = fname1;
                    }

                    return tot;


                }
                else
                {
                    return null;
                }
            }

            [HttpPost]
            [Authorize]
            [Route("api/SaleCostReports/SalRepCustomerClosingBalances")]
            public PurSupplierLedgerTotal SalRepCustomerClosingBalances([FromBody] UserInfo usr)
            {
                if (ac.screenCheck(usr, 5, 9, 1, 0))
                {
                    PurSupplierLedgerTotal tot = new PurSupplierLedgerTotal();
                    General gg = new General();
                    tot.details = new List<PurSupplierLedgerInfo>();
                    var lst1 = (from a in db.PartyTransactions.Where(a => a.CustomerCode == usr.cCode).GroupBy(b => b.PartyId)
                                select new
                                {
                                    partyid = a.Key,
                                    balance = a.Sum(b => b.PendingAmount - b.ReturnAmount - b.CreditNote - b.PaymentAmount)
                                }).ToList();
                    var lst2 = db.PartyDetails.Where(a => a.PartyType == "CUS" && a.CustomerCode == usr.cCode).ToList();
                    var details = (from a in lst1
                                   join b in lst2 on a.partyid equals b.RecordId
                                   select new PurSupplierLedgerInfo
                                   {
                                       dat = b.PartyName,
                                       pur = gg.makeCur((double)a.balance, 2),
                                       clb = a.balance.ToString()
                                   }).OrderBy(b => b.dat).ToList();
                    if (details.Count() > 0)
                    {
                        tot.details.AddRange(details);
                    }
                    tot.details.Add(new PurSupplierLedgerInfo
                    {
                        dat = ""
                    });
                    tot.details.Add(new PurSupplierLedgerInfo
                    {
                        dat = "Total",
                        pur = gg.makeCur(details.Sum(b => double.Parse(b.clb)), 2)
                    });

                    DataTable dt = new DataTable();
                    dt.Columns.Add("sno", typeof(string));
                    dt.Columns.Add("sup", typeof(string));
                    dt.Columns.Add("bal1", typeof(string));

                    int i = 1;

                    foreach (var det in tot.details)
                    {
                        dt.Rows.Add(i < tot.details.Count() - 1 ? i.ToString() : "", det.dat, det.pur);
                        i++;
                    }

                    List<string> titles = new List<string>();
                    titles.Add("#");
                    titles.Add("Customer");
                    titles.Add("Balance");

                    float[] widths = { 30f, 300f, 220f };
                    int[] aligns = { 3, 1, 2 };
                    PDFExcelMake ep = new PDFExcelMake();

                    DateTime dats = ac.getPresentDateTime();
                    string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                    string fname = usr.uCode + "PurCostRep" + dat + usr.cCode + usr.bCode + ".pdf";
                    string filename = ho.WebRootPath + "\\Reps\\" + fname;
                    string msg = "";

                    msg = ep.pdfConversion(filename, "Customer Closing Balances as on " + gg.strDateTime(dats), usr, dt, titles, widths, aligns, true);
                    if (msg == "OK")
                    {
                        tot.pdfFile = fname;
                    }
                    string fname1 = usr.uCode + "PurCostRep" + dat + usr.cCode + usr.bCode + ".xlsx";
                    string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                    msg = "";
                    msg = ep.makeExcelConversion(filename1, "Customer Closing Balances as on " + gg.strDateTime(dats), usr, dt, titles, widths, aligns, true);
                    if (msg == "OK")
                    {
                        tot.excelFile = fname1;
                    }

                    return tot;


                }
                else
                {
                    return null;
                }
            }

        }
    }
