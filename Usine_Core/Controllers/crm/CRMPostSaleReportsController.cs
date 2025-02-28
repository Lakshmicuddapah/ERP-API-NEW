using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Controllers.Others;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Controllers.CRM;
using Usine_Core.Controllers.Purchases;

namespace Usine_Core.Controllers.crm
{
    
    public class CRMPostSaleReportsController : ControllerBase
    {
        private readonly IHostingEnvironment ho;
        public CRMPostSaleReportsController(IHostingEnvironment hostingEnvironment)
        {

            ho = hostingEnvironment;
        }

        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        [HttpPost]
        [Authorize]
        [Route("api/RMPostSaleReports/GetSaleReturns")]
        public CRMReportDetailsTotal GetSaleReturns([FromBody] GeneralInformation inf)
        {
            var dat1 = DateTime.Parse(inf.frmDate);
            var dat2 = DateTime.Parse(inf.toDate).AddDays(1);
            General gg = new General();
            CRMReportDetailsTotal tot = new CRMReportDetailsTotal();
            tot.details = (from a in db.SalSaleReturnsUni.Where(a => a.Dat >= dat1 && a.Dat < dat2)
                           select new CRMRepDetails
                           {
                               det1=a.Seq,
                               det2=ac.strDate(a.Dat.Value),
                               det3=a.PartyName,
                               det4=gg.makeCur((double)a.Baseamt,2),
                               det5=gg.makeCur((double)a.TotalAmt,2),
                               val1=a.Baseamt,
                               val2=a.TotalAmt
                           }).ToList();

            var ba = tot.details.Sum(a => a.val1);
            var tam = tot.details.Sum(a => a.val2);
            if(tot.details.Count() > 0)
            {
                tot.details.Add(new CRMRepDetails
                {
                    det1 = "",
                    det3 = "",
                });
                tot.details.Add(new CRMRepDetails
                {
                    det3="Total",
                    det4=gg.makeCur((double)ba,2),
                    det5 = gg.makeCur((double)tam, 2),

                });
            }
            return tot;

        }
        [HttpPost]
        [Authorize]
        [Route("api/CRMPostSaleReports/CRMRepCustomerSnapShot")]
        public PurSupplierLedgerTotal CRMRepCustomerSnapShot([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 2, 11, 4, 0))
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
                dt.Columns.Add("purchase", typeof(string));
                dt.Columns.Add("preturn", typeof(string));
                dt.Columns.Add("crnote", typeof(string));
                dt.Columns.Add("payment", typeof(string));
                dt.Columns.Add("clb", typeof(string));
                int i = 1;

                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i.ToString(), det.dat, det.opb, det.pur, det.ret, det.cno, det.pay, det.clb);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Supplier");
                titles.Add("Opening");
                titles.Add("Purchase");
                titles.Add("Return");
                titles.Add("Cr Note");
                titles.Add("Payment");
                titles.Add("Closing");
                float[] widths = { 30f, 100f, 70f, 70f, 70f, 70f, 70f, 70f };
                int[] aligns = { 3, 1, 2, 2, 2, 2, 2, 2 };
                PDFExcelMake ep = new PDFExcelMake();

                DateTime dats = ac.getPresentDateTime();
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = usr.uCode + "PurCostRep" + dat + usr.cCode + usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfLandscapeConversion(filename, "Supplier Snap Shot as on " + gg.strDateTime(dats), usr, dt, titles, widths, aligns, false);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = usr.uCode + "PurCostRep" + dat + usr.cCode + usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Supplier Snap Shot as on " + gg.strDateTime(dats), usr, dt, titles, widths, aligns, false);
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
        [Route("api/CRMPostSaleReports/CRMRepCustomerAgingDetails")]
        public PurSupplierLedgerTotal PurRepCustomerAgingDetails([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 2, 11, 4, 0))
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
                titles.Add("Supplier");
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

                msg = ep.pdfConversion(filename, "Supplier aging details as on " + gg.strDateTime(dats), usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = usr.uCode + "PurCostRep" + dat + usr.cCode + usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Supplier aging details as on " + gg.strDateTime(dats), usr, dt, titles, widths, aligns, true);
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
