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
namespace Usine_Core.Controllers.quality
{
    public class QCMIRReportsInfomation
    {
        public string det1 { get; set; }
        public string det2 { get; set; }
        public string det3 { get; set; }
        public string det4 { get; set; }
        public string det5 { get; set; }
        public string det6 { get; set; }
        public string det7 { get; set; }
        public string det8 { get; set; }
        public string det9 { get; set; }
    }
    public class QCMIRReportsInfomationTotal
    {
        public List<QCMIRReportsInfomation> details { get; set; }
        public string pdfFile { get; set; }
        public string excelFile { get; set; }
    }

    public class QCMIRReportsController : ControllerBase
    {
        private readonly IHostingEnvironment ho;
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        public QCMIRReportsController(IHostingEnvironment hostingEnvironment)
        {
            ho = hostingEnvironment;
        }
        [HttpPost]
        [Authorize]
        [Route("api/QCMIRReports/QCMIRRepPendings")]
        public QCMIRReportsInfomationTotal QCMIRRepPendings([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 11, 9, 1, 0))
            {
                try
                {
                    QCMIRReportsInfomationTotal tot = new QCMIRReportsInfomationTotal();
                    General gg = new General();
                    tot.details = (from a in db.PurPurchasesUni.Where(a => a.QcCheck == 1 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                                   select new QCMIRReportsInfomation
                                   {
                                       det1 = a.Seq,
                                       det2 = gg.strDate(a.Dat.Value),
                                       det3 = a.Vendorname,
                                       det4 = gg.makeCur((double)a.Baseamt, 2),
                                       det5=gg.makeCur((double)a.TotalAmt,2)
                                   }).OrderBy(b => b.det1).ToList();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("sno", typeof(string));
                    dt.Columns.Add("mir", typeof(string));
                    dt.Columns.Add("dat", typeof(string));
                    dt.Columns.Add("vendor", typeof(string));
                    dt.Columns.Add("base", typeof(string));
                    dt.Columns.Add("total", typeof(string));
                     int i = 0;

                    foreach (var det in tot.details)
                    {
                        dt.Rows.Add(i.ToString(), det.det1, det.det2, det.det3, det.det4, det.det5 );
                        i++;
                    }

                    List<string> titles = new List<string>();
                    titles.Add("#");
                    titles.Add("MIR");
                    titles.Add("Date");
                    titles.Add("Supplier");
                    titles.Add("Base");
                    titles.Add("Total");
       
                    float[] widths = { 30f,70f,80f, 210f, 80f, 80f};
                    int[] aligns = { 3, 1,1,1, 2, 2};
                    PDFExcelMake ep = new PDFExcelMake();

                    DateTime dats = DateTime.Now;
                    string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                    string fname = usr.uCode + "QCMIRRep" + dat + usr.cCode + usr.bCode + ".pdf";
                    string filename = ho.WebRootPath + "\\Reps\\" + fname;
                    string msg = "";

                    msg = ep.pdfLandscapeConversion(filename, "Pending MIR To be Tested ", usr, dt, titles, widths, aligns, false);
                    if (msg == "OK")
                    {
                        tot.pdfFile = fname;
                    }
                    string fname1 =  usr.uCode + "PurStockRep" + dat + usr.cCode + usr.bCode + ".xlsx";
                    string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                    msg = "";
                    msg = ep.makeExcelConversion(filename1, "Pending MIR To be Tested ", usr, dt, titles, widths, aligns, false);
                    if (msg == "OK")
                    {
                        tot.excelFile = fname1;
                    }

                    return tot;



                }
                catch (Exception ee)
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
        [Route("api/QCMIRReports/QCMIRRepTestDetails")]
        public QCMIRReportsInfomationTotal QCMIRRepTestDetails([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 11, 9, 1, 0))
            {
                General gg = new General();
                string dat1 = inf.frmDate;
                string dat2 = gg.strDate(DateTime.Parse(inf.toDate).AddDays(1));



                QCMIRReportsInfomationTotal tot = new QCMIRReportsInfomationTotal();
                tot.details = new List<QCMIRReportsInfomation>();
                string quer = "";
                quer = quer + " select b.seq,dbo.strDate(dat) dat,a.testname,b.vendorname,b.itemname,b.qty,dbo.makcur(rectificationCost) rectif,dbo.makcur(valueOfItem) rejected from";
                quer = quer + " (select a.sno, a.lotno, dat, a.transactionid, a.checkedqty, a.rectificationCost, a.valueOfItem, b.testname from";
                quer = quer + " (select a.recordId, a.sno, dat, a.lotno, a.transactionid, checkedQty, rectificationCost, testid, valueOfItem from";
                quer = quer + " (select* from qcTraTestsDet  where branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
                quer = quer + " (select * from qcTraTestsuni where typ = 'MAT' and dat >= '" + dat1 + "' and dat<'" + dat2 + "'";
                quer = quer + " and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + " )b where a.recordId = b.recordId)a,";
                quer = quer + " (select * from qcTestings where customerCode = " + inf.usr.cCode + ")b where a.testid = b.recordId)a,";
                quer = quer + " (select b.seq,b.vendorId,b.vendorname,a.itemname,a.qty,a.recordId,a.sno from";
                quer = quer + " (select* from purPurchasesDet where branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
                quer = quer + " (select * from purpurchasesuni where branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b where a.recordId = b.recordId )b";
                quer = quer + " where a.transactionId = b.recordId and a.lotno = b.sno order by seq";

                     DataBaseContext g = new DataBaseContext();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();

                while (dr.Read())
                {
                    tot.details.Add(new QCMIRReportsInfomation
                    {
                        det1 = dr[0].ToString(),
                        det2 = dr[1].ToString(),
                        det3 = dr[2].ToString(),
                        det4 = dr[3].ToString(),
                        det5 = dr[4].ToString(),
                        det6 = dr[5].ToString(),
                        det7 = dr[6].ToString(),
                        det8 = dr[7].ToString()
                    });

                }
                dr.Close();
                g.db.Close();

                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("mir", typeof(string));
                dt.Columns.Add("dat", typeof(string));
                dt.Columns.Add("test", typeof(string));
                dt.Columns.Add("supplier", typeof(string));
                dt.Columns.Add("item", typeof(string));
                dt.Columns.Add("qty", typeof(string));
                dt.Columns.Add("rect", typeof(string));
                dt.Columns.Add("reject", typeof(string));
                int i = 0;

                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i.ToString(), det.det1, det.det2, det.det3, det.det4, det.det5, det.det6,det.det7,det.det8);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("MIR");
                titles.Add("Date");
                titles.Add("Test");
                titles.Add("Supplier");
                titles.Add("Item");
                titles.Add("Qty");
                titles.Add("Rectif.");
                titles.Add("Reject.");
               

                float[] widths = { 30f, 70f,70f,   110f,120f,110f, 70f, 70f, 70f };
                int[] aligns = { 3, 1, 1, 1, 1, 1, 2,2,2 };
                PDFExcelMake ep = new PDFExcelMake();

                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "QCMIRRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfLandscapeConversion(filename, "Quality Material Testing Results from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "QCMIRRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Quality Material Testing Results from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
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
        [Route("api/QCMIRReports/QCMIRRepSupplierRanking")]
        public QCMIRReportsInfomationTotal QCMIRRepSupplierRanking([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 11, 9, 1, 0))
            {
                General gg = new General();
                string dat1 = inf.frmDate;
                string dat2 = gg.strDate(DateTime.Parse(inf.toDate).AddDays(1));



                QCMIRReportsInfomationTotal tot = new QCMIRReportsInfomationTotal();
                tot.details = new List<QCMIRReportsInfomation>();
                string quer = "";
                quer = quer + " select vendorname,dbo.makCur(sum(purchasesAmt)) purchaseAmt, dbo.makCur(sum(rectification)) rectification,dbo.makCur(sum(rejected)) rejected,";
                quer = quer + " round(sum(rectification + rejected) / sum(purchasesAmt) * 100, 2) inaccur from";
                quer = quer + " (select vendorname, baseAmt purchasesAmt,";
                quer = quer + " case when rectification is null then 0 else rectification end rectification,";
                quer = quer + " case when rejected is null then 0 else rejected end rejected from";
                quer = quer + " (select* from purpurchasesuni where dat >= '" + dat1 + "' and dat< '" + dat2 + "'";
                quer = quer + " and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a left outer join";
                quer = quer + " (select transactionId, sum(rectificationCost) rectification, sum(valueOfItem) rejected";
                quer = quer + " from qcTraTestsDet where branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + " group by transactionId)b on";
                quer = quer + " a.recordId = b.transactionid)x group by vendorname order by round(sum(rectification + rejected) / sum(purchasesAmt) * 100, 2) desc";

                         DataBaseContext g = new DataBaseContext();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();

                while (dr.Read())
                {
                    tot.details.Add(new QCMIRReportsInfomation
                    {
                        det1 = dr[0].ToString(),
                        det2 = dr[1].ToString(),
                        det3 = dr[2].ToString(),
                        det4 = dr[3].ToString(),
                        det5 = dr[4].ToString(),
                    });

                }
                dr.Close();
                g.db.Close();

                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("supplier", typeof(string));
                dt.Columns.Add("purchase", typeof(string));
                dt.Columns.Add("rectif", typeof(string));
                dt.Columns.Add("reject", typeof(string));
                dt.Columns.Add("inaccur", typeof(string));
            
                int i = 0;

                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i.ToString(), det.det1, det.det2, det.det3, det.det4, det.det5);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Supplier");
                titles.Add("Purchase");
                titles.Add("Rectification");
                titles.Add("Rejection");
                titles.Add("Inaccuracy%");
              

                float[] widths = { 30f,200f,80f,80f,80f,80f  };
                int[] aligns = { 3, 1,   2, 2, 2,2 };
                PDFExcelMake ep = new PDFExcelMake();

                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "QCMIRRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfConversion(filename, "Supplier Rating from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "QCMIRRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Supplier Rating from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
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
