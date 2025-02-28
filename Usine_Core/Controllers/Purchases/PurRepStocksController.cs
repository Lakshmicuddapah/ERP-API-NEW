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
namespace Usine_Core.Controllers.Purchases
{
    public class PurRepStocksInfomation
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
    public class PurRepStocksInformationTotal
    {
        public List<PurRepStocksInfomation> details { get; set; }
        public string pdfFile { get; set; }
        public string excelFile { get; set; }
    }
    public class PurRepStocksController : ControllerBase
    {
        private readonly IHostingEnvironment ho;
        public PurRepStocksController(IHostingEnvironment hostingEnvironment)
        {
            ho = hostingEnvironment;
        }

        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/PurRepStocks/PurRepPurchasesDayBook")]
        public PurRepStocksInformationTotal PurRepPurchasesDayBook([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 5, 0))
            {
                General gg = new General();
                string dat1 = inf.frmDate;
                string dat2 = gg.strDate(DateTime.Parse(inf.frmDate).AddDays(1));
                PurRepStocksInformationTotal tot = new PurRepStocksInformationTotal();
                tot.details = new List<PurRepStocksInfomation>();
                string quer = "";
                quer = quer + " select seq,tim,itemname,convert(varchar(10),purqty) purqty,b.um purum,dbo.makCur(purvalue) purvalu,";
                quer = quer + " ' ' retqty,' ' retum,' ' retvalue from";
                quer = quer + " (select b.seq, dbo.strDateTime(b.dat) tim, a.itemName, a.qty purqty, a.um purum, a.qty* rat purvalue from";
                quer = quer + " (select* from purPurchasesDet where branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
                quer = quer + " (select * from purpurchasesuni where dat >= '" + dat1 + "' and dat< '" + dat2 + "'";
                quer = quer + " and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b where a.recordId = b.recordId)a,";
                quer = quer + " (select * from invum where customerCode = " + inf.usr.cCode + ")b where a.purum = b.recordId";
                quer = quer + " union all select seq, tim, itemname,' ' purqty,' ' purum,' ' purvalu, ";
                quer = quer + " convert(varchar(10), purqty) retqty,b.um retum, dbo.makCur(purvalue) retvalue from";
                quer = quer + " (select b.seq, dbo.strDateTime(b.dat) tim, a.itemName, a.qty purqty, a.um purum, a.qty* rat purvalue from";
                quer = quer + " (select* from purPurchaseReturnsDet where branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
                quer = quer + " (select * from purPurchaseReturnsUni where dat >= '" + dat1 + "' and dat< '" + dat2 + "'";
                quer = quer + " and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b where a.recordId = b.recordId)a,";
                quer = quer + " (select * from invum where customerCode = " + inf.usr.cCode + ")b where a.purum = b.recordId";


                  DataBaseContext g = new DataBaseContext();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();

                while (dr.Read())
                {
                    tot.details.Add(new PurRepStocksInfomation
                    {
                        det1 = dr[0].ToString(),
                        det2 = dr[1].ToString(),
                        det3 = dr[2].ToString(),
                        det4 = dr[3].ToString(),
                        det5 = dr[4].ToString(),
                        det6 = dr[5].ToString(),
                        det7 = dr[6].ToString(),
                        det8 = dr[7].ToString(),
                        det9 = dr[8].ToString()
                    });

                }
                dr.Close();
                g.db.Close();

                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("seq", typeof(string));
                dt.Columns.Add("tim", typeof(string));
                dt.Columns.Add("item", typeof(string));
                dt.Columns.Add("pqty", typeof(string));
                dt.Columns.Add("pum", typeof(string));
                dt.Columns.Add("pval", typeof(string));
                dt.Columns.Add("prqty", typeof(string));
                dt.Columns.Add("prum", typeof(string));
                dt.Columns.Add("prval", typeof(string));
                int i = 0;

                foreach (var det in tot.details)
                {
                    dt.Rows.Add(  i.ToString()  , det.det1, det.det2, det.det3, det.det4, det.det5, det.det6, det.det7, det.det8, det.det9);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Seq");
                titles.Add("Time");
                titles.Add("Item");
                titles.Add("Pur Qty");
                titles.Add("UM");
                titles.Add("Pur Value");
                titles.Add("Ret Qty");
                titles.Add("UM");
                titles.Add("Ret Value");
                float[] widths = { 30f, 70f, 70f,130f, 70f, 60f, 80f, 70f, 60f, 80f };
                int[] aligns = { 3, 1, 1,1, 2, 1, 2, 2,1, 2 };
                PDFExcelMake ep = new PDFExcelMake();

                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "PurStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfLandscapeConversion(filename, "Purchases Day book as on " + inf.frmDate  , inf.usr, dt, titles, widths, aligns, false);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Purchases Day book as on " + inf.frmDate, inf.usr, dt, titles, widths, aligns, false);
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
        [Route("api/PurRepStocks/PurRepItemWisePurchasesConsolidations")]
        public PurRepStocksInformationTotal PurRepItemWisePurchasesConsolidations([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 5, 0))
            {
                General gg = new General();
                string dat1 = inf.frmDate;
                string dat2 = gg.strDate(DateTime.Parse(inf.toDate).AddDays(1));
                PurRepStocksInformationTotal tot = new PurRepStocksInformationTotal();
                tot.details = new List<PurRepStocksInfomation>();
                string quer = "";
                quer = quer + " select itemname,qty,um,valu from";
                quer = quer + " (select itemname, convert(varchar(20), qty) qty, b.um um, dbo.makcur(valu) valu, 1 sno from";
                quer = quer + " (select itemname, sum(qty) qty, stdum, sum(qty * rat) valu from";
                quer = quer + " (select itemname, qty * conversionFactor qty, stdum, rat / conversionFactor rat from";
                quer = quer + " (select * from purpurchasesdet where branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + "";
                quer = quer + " and recordId in";
                quer = quer + " (select recordId from purPurchasesUni where dat >= '" + dat1 + "' and dat < '" + dat2 + "'";
                quer = quer + " and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + "))a,";
                quer = quer + " (select * from invMaterialUnits where customerCode = " + inf.usr.cCode + ")b where a.itemid = b.recordId and a.um = b.um)x";
                quer = quer + " group by itemname, stdum)a,";
                quer = quer + " (select * from invum where customerCode = " + inf.usr.cCode + ")b where a.stdUm = b.recordId";
                quer = quer + " union all";
                quer = quer + " select ' ' itemname,' ' qty,' ' um,' ' value,2 sno";
                quer = quer + " union all";
                quer = quer + " select 'Total' itemname,' ' qty,' ' um, dbo.makcur(sum(qty * rat)) valu,3 sno from purpurchasesdet where branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + "";
                quer = quer + " and recordId in";
                quer = quer + " (select recordId from purPurchasesUni where dat >= '" + dat1 + "' and dat< '" + dat2 + "'";
                quer = quer + " and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + "))X order by sno, itemname";


                DataBaseContext g = new DataBaseContext();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();

                while (dr.Read())
                {
                    tot.details.Add(new PurRepStocksInfomation
                    {
                        det1 = dr[0].ToString(),
                        det2 = dr[1].ToString(),
                        det3 = dr[2].ToString(),
                        det4 = dr[3].ToString(),
                       
                    });

                }
                dr.Close();
                g.db.Close();

                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("item", typeof(string));
                dt.Columns.Add("qty", typeof(string));
                dt.Columns.Add("um", typeof(string));
                dt.Columns.Add("valu", typeof(string));
             
                int i = 0;

                foreach (var det in tot.details)
                {
                    dt.Rows.Add( i<tot.details.Count()-2? i.ToString():" ", det.det1, det.det2, det.det3, det.det4 );
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Item");
                titles.Add("Qty");
                titles.Add("UM");
                titles.Add("Value");
               
                float[] widths = { 30f, 250f, 90f, 70f, 110f};
                int[] aligns = { 3, 1,  2, 1, 2 };
                PDFExcelMake ep = new PDFExcelMake();

                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "PurStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfLandscapeConversion(filename, "Item wise Consolidation Purchases from " + inf.frmDate + " to " + inf.toDate ,  inf.usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Item wise Consolidation Purchases from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
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
        [Route("api/PurRepStocks/PurRepTop10ItemsPurchased")]
        public PurRepStocksInformationTotal PurRepTop10ItemsPurchased([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 5, 0))
            {
                General gg = new General();
                string dat1 = inf.frmDate;
                string dat2 = gg.strDate(DateTime.Parse(inf.toDate).AddDays(1));
                PurRepStocksInformationTotal tot = new PurRepStocksInformationTotal();
                tot.details = new List<PurRepStocksInfomation>();
                string quer = "";
                quer = quer + " select top 10 itemname,qty,um,valu from";
                quer = quer + " (select itemname, convert(varchar(20), qty) qty, b.um um, dbo.makcur(valu) valu, valu value1 from";
                quer = quer + " (select itemname, sum(qty) qty, stdum, sum(qty * rat) valu from";
                quer = quer + " (select itemname, qty * conversionFactor qty, stdum, rat / conversionFactor rat from";
                quer = quer + " (select * from purpurchasesdet where branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + "";
                quer = quer + " and recordId in";
                quer = quer + " (select recordId from purPurchasesUni where dat >= '" + dat1 + "' and dat < '" + dat2 + "'";
                quer = quer + " and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + "))a,";
                quer = quer + " (select * from invMaterialUnits where customerCode = " + inf.usr.cCode + ")b where a.itemid = b.recordId and a.um = b.um)x";
                quer = quer + " group by itemname, stdum)a,";
                quer = quer + " (select * from invum where customerCode = " + inf.usr.cCode + ")b where a.stdUm = b.recordId)x order by value1 desc";



                DataBaseContext g = new DataBaseContext();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();

                while (dr.Read())
                {
                    tot.details.Add(new PurRepStocksInfomation
                    {
                        det1 = dr[0].ToString(),
                        det2 = dr[1].ToString(),
                        det3 = dr[2].ToString(),
                        det4 = dr[3].ToString(),

                    });

                }
                dr.Close();
                g.db.Close();

                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("item", typeof(string));
                dt.Columns.Add("qty", typeof(string));
                dt.Columns.Add("um", typeof(string));
                dt.Columns.Add("valu", typeof(string));

                int i = 0;

                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : " ", det.det1, det.det2, det.det3, det.det4);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Item");
                titles.Add("Qty");
                titles.Add("UM");
                titles.Add("Value");

                float[] widths = { 30f, 250f, 90f, 70f, 110f };
                int[] aligns = { 3, 1, 2, 1, 2 };
                PDFExcelMake ep = new PDFExcelMake();

                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "PurStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfLandscapeConversion(filename, "Top 10 Items Purchased from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Top 10 Items Purchased  from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
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
        [Route("api/PurRepStocks/PurRepLeast10ItemsPurchased")]
        public PurRepStocksInformationTotal PurRepLeast10ItemsPurchased([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 5, 0))
            {
                General gg = new General();
                string dat1 = inf.frmDate;
                string dat2 = gg.strDate(DateTime.Parse(inf.toDate).AddDays(1));
                PurRepStocksInformationTotal tot = new PurRepStocksInformationTotal();
                tot.details = new List<PurRepStocksInfomation>();
                string quer = "";
                quer = quer + " select top 10 itemname,qty,um,valu from";
                quer = quer + " (select itemname, convert(varchar(20), qty) qty, b.um um, dbo.makcur(valu) valu, valu value1 from";
                quer = quer + " (select itemname, sum(qty) qty, stdum, sum(qty * rat) valu from";
                quer = quer + " (select itemname, qty * conversionFactor qty, stdum, rat / conversionFactor rat from";
                quer = quer + " (select * from purpurchasesdet where branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + "";
                quer = quer + " and recordId in";
                quer = quer + " (select recordId from purPurchasesUni where dat >= '" + dat1 + "' and dat < '" + dat2 + "'";
                quer = quer + " and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + "))a,";
                quer = quer + " (select * from invMaterialUnits where customerCode = " + inf.usr.cCode + ")b where a.itemid = b.recordId and a.um = b.um)x";
                quer = quer + " group by itemname, stdum)a,";
                quer = quer + " (select * from invum where customerCode = " + inf.usr.cCode + ")b where a.stdUm = b.recordId)x order by value1 asc";



                DataBaseContext g = new DataBaseContext();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();

                while (dr.Read())
                {
                    tot.details.Add(new PurRepStocksInfomation
                    {
                        det1 = dr[0].ToString(),
                        det2 = dr[1].ToString(),
                        det3 = dr[2].ToString(),
                        det4 = dr[3].ToString(),

                    });

                }
                dr.Close();
                g.db.Close();

                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("item", typeof(string));
                dt.Columns.Add("qty", typeof(string));
                dt.Columns.Add("um", typeof(string));
                dt.Columns.Add("valu", typeof(string));

                int i = 0;

                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : " ", det.det1, det.det2, det.det3, det.det4);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Item");
                titles.Add("Qty");
                titles.Add("UM");
                titles.Add("Value");

                float[] widths = { 30f, 250f, 90f, 70f, 110f };
                int[] aligns = { 3, 1, 2, 1, 2 };
                PDFExcelMake ep = new PDFExcelMake();

                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "PurStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfLandscapeConversion(filename, "Least 10 Items Purchased from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Least 10 Items Purchased  from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
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
        [Route("api/PurRepStocks/PurRepNoPurchaseItems")]
        public PurRepStocksInformationTotal PurRepNoPurchaseItems([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 5, 0))
            {
                General gg = new General();
                string dat1 = inf.frmDate;
                string dat2 = gg.strDate(DateTime.Parse(inf.toDate).AddDays(1));
                PurRepStocksInformationTotal tot = new PurRepStocksInformationTotal();
                tot.details = new List<PurRepStocksInfomation>();
                string quer = "";
                quer = quer + " select itemid,itemname,grpname from invMaterialCompleteDetails_view where recordId not in";
                quer = quer + " (select distinct itemid from purPurchasesDet where branchid = '" + inf.usr.bCode + "' and customercode = " + inf.usr.cCode + " and";
                quer = quer + " recordId in (select recordId from purPurchasesUni where dat >= '" + dat1 + "' and dat < '" + dat2 + "'";
                quer = quer + " and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")) order by itemname";



                   DataBaseContext g = new DataBaseContext();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();

                while (dr.Read())
                {
                    tot.details.Add(new PurRepStocksInfomation
                    {
                        det1 = dr[0].ToString(),
                        det2 = dr[1].ToString(),
                        det3 = dr[2].ToString(),
                      

                    });

                }
                dr.Close();
                g.db.Close();

                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("itemid", typeof(string));
                dt.Columns.Add("itemname", typeof(string));
                dt.Columns.Add("grp", typeof(string));
          
                int i = 0;

                foreach (var det in tot.details)
                {
                    dt.Rows.Add( i.ToString() , det.det1, det.det2, det.det3);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Item Id");
                titles.Add("Item");
                titles.Add("Group");
            
                float[] widths = { 30f, 100f, 220f, 200f};
                int[] aligns = { 3, 1, 1, 1};
                PDFExcelMake ep = new PDFExcelMake();

                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "PurStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfLandscapeConversion(filename, "No Items Purchased from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "No Items Purchased  from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
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
