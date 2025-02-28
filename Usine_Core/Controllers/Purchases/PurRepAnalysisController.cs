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
    public class PurRepAnalysisInfomation
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
    public class PurRepAnalysisInfomationTotal
    {
        public List<PurRepAnalysisInfomation> details { get; set; }
        public string pdfFile { get; set; }
        public string excelFile { get; set; }
    }
    public class GeneralAnalysisInformation
    {
        public string fromdate1 { get; set; }
        public string todate1 { get; set; }
        public string fromdate2 { get; set; }
        public string todate2 { get; set; }
        public decimal? previousdays { get; set; }
        public decimal? nextdatys { get; set; }
        public UserInfo usr { get; set; }
    }
    
    public class PurRepAnalysisController : ControllerBase
    {
        private readonly IHostingEnvironment ho;
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        public PurRepAnalysisController(IHostingEnvironment hostingEnvironment)
        {
            ho = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurRepAnalysis/PurAnalysisPriceComparison")]
        public PurRepAnalysisInfomationTotal PurAnalysisPriceComparison([FromBody] GeneralAnalysisInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 6, 0))
            {
                General gg = new General();
                string dat1 = inf.fromdate1;
                string dat2 = gg.strDate(DateTime.Parse(inf.todate1).AddDays(1));


                string dat3 = inf.fromdate2;
                string dat4 = gg.strDate(DateTime.Parse(inf.todate2).AddDays(1));

                PurRepAnalysisInfomationTotal tot = new PurRepAnalysisInfomationTotal();
                tot.details = new List<PurRepAnalysisInfomation>();
                string quer = "";
                quer = quer + " select * from";
                quer = quer + " (select itemname, qty1, rat1, qty2, rat2,case when rat1 = 0 then 100 else round((rat2 - rat1) / rat1 * 100, 2) end diff, um from";
                quer = quer + " (select case when a.itemname is null then b.itemname else a.itemname end itemname,";
                quer = quer + " case when a.qty is null then 0 else a.qty end qty1,case when a.rat is null then 0 else a.rat end rat1,";
                quer = quer + " case when b.qty is null then 0 else b.qty end qty2,case when b.rat is null then 0 else b.rat end rat2,";
                quer = quer + " case when a.um is null then b.um else a.um end um from";
                quer = quer + " (select itemname, sum(qty) qty, sum(qty* rat)/ sum(qty) rat,um from";
                quer = quer + " (select b.itemname, a.qty, b.um, a.rat from";
                quer = quer + " (select a.itemid, a.qty* b.conversionFactor qty, b.stdum stdum, a.rat/ b.conversionFactor rat from";
                quer = quer + " (select* from purPurchasesDet where branchid= '" + inf.usr.bCode + "' and customercode = " + inf.usr.cCode + "";
                quer = quer + " and recordId in (select recordId from purpurchasesuni where branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + "";
                quer = quer + " and dat >= '" + dat1 + "' and dat< '" + dat2 + "'))a,";
                quer = quer + " (select * from invMaterialUnits where customerCode = " + inf.usr.cCode + ")b where a.um = b.um and a.itemId = b.recordId)a,";
                quer = quer + " (select * from invMaterialCompleteDetails_view where customerCode = " + inf.usr.cCode + ")b";
                quer = quer + " where a.itemId = b.recordId)x group by itemname, um)a full outer join";
                quer = quer + " (select itemname, sum(qty) qty, sum(qty* rat)/ sum(qty) rat,um from";
                quer = quer + " (select b.itemname, a.qty, b.um, a.rat from";
                quer = quer + " (select a.itemid, a.qty* b.conversionFactor qty, b.stdum stdum, a.rat/ b.conversionFactor rat from";
                quer = quer + " (select* from purPurchasesDet where branchid= '" + inf.usr.bCode + "' and customercode = " + inf.usr.cCode + "";
                quer = quer + " and recordId in (select recordId from purpurchasesuni where branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + "";
                quer = quer + " and dat >= '" + dat3 + "' and dat< '" + dat4 + "'))a,";
                quer = quer + " (select * from invMaterialUnits where customerCode = " + inf.usr.cCode + ")b where a.um = b.um and a.itemId = b.recordId)a,";
                quer = quer + " (select * from invMaterialCompleteDetails_view where customerCode = " + inf.usr.cCode + ")b";
                quer = quer + " where a.itemId = b.recordId)x group by itemname, um)b on a.itemname = b.itemname )x)x order by diff desc,itemname";
 
                DataBaseContext g = new DataBaseContext();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();

                while (dr.Read())
                {
                    tot.details.Add(new PurRepAnalysisInfomation
                    {
                        det1 = dr[0].ToString(),
                        det2 = dr[1].ToString(),
                        det3 = dr[2].ToString(),
                        det4 = dr[3].ToString(),
                        det5 = dr[4].ToString(),
                        det6 = dr[5].ToString(),
                        det7 = dr[6].ToString(),
                    });

                }
                dr.Close();
                g.db.Close();

                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
               dt.Columns.Add("item", typeof(string));
                dt.Columns.Add("qty1", typeof(string));
                dt.Columns.Add("rat1", typeof(string));
                dt.Columns.Add("qty2", typeof(string));
                dt.Columns.Add("rat2", typeof(string));
                dt.Columns.Add("diff", typeof(string));
                dt.Columns.Add("um", typeof(string));
                int i = 0;

                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i.ToString(), det.det1, det.det2, det.det3, det.det4, det.det5, det.det6, det.det7);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                 titles.Add("Item");
                titles.Add("Qty1");
                titles.Add("Rate1");
                titles.Add("Qty2");
                titles.Add("Rate2");
                titles.Add("Diffe");
                  titles.Add("UM");
           
                float[] widths = { 30f, 210f, 80f, 80f, 80f, 80f, 80f, 80f };
                int[] aligns = { 3, 1, 2,2,2,2,2,2};
                PDFExcelMake ep = new PDFExcelMake();

                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "PurStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfLandscapeConversion(filename, "Price Comparison of materials in range " + inf.fromdate1 + " to " + inf.todate1 + "  and  " + inf.fromdate2 + " to " + inf.todate2, inf.usr, dt, titles, widths, aligns, false);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Price Comparison of materials in range " + inf.fromdate1 + " to " + inf.todate1 + "  and  " + inf.fromdate2 + " to " + inf.todate2, inf.usr, dt, titles, widths, aligns, false);
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
        [Route("api/PurRepAnalysis/PurAnalysisPriceWithStandard")]
        public PurRepAnalysisInfomationTotal PurAnalysisPriceWithStandard([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 6, 0))
            {
                General gg = new General();
                string dat1 = inf.frmDate;
                string dat2 = gg.strDate(DateTime.Parse(inf.toDate).AddDays(1));


               
                PurRepAnalysisInfomationTotal tot = new PurRepAnalysisInfomationTotal();
                tot.details = new List<PurRepAnalysisInfomation>();
                string quer = "";
                quer = quer + " select itemname,qty,dbo.makcur(actualvalue) actualvalu,dbo.makcur(stdvalue) stdvalue,";
                quer = quer + " dbo.makcur(loss) loss1,dbo.makCur(profit) profit1 from";
                quer = quer + " (select a.itemname, a.qty, a.rat actualrat, b.stdRate stdrate, a.qty* a.rat actualvalue,";
                quer = quer + " a.qty* b.stdRate stdvalue,case when (a.qty* a.rat-a.qty * b.stdRate) >= 0 then a.qty* a.rat - a.qty * b.stdRate";
                quer = quer + " else 0 end loss,case when(a.qty * a.rat - a.qty * b.stdRate) < 0 then abs(a.qty* a.rat-a.qty * b.stdRate)";
                quer = quer + " else 0 end profit from";
                quer = quer + " (select itemname, sum(qty) qty, sum(qty * rat) / sum(qty) rat, um from";
                quer = quer + " (select b.itemname, a.qty, b.um, a.rat from";
                quer = quer + " (select a.itemid, a.qty * b.conversionFactor qty, b.stdum stdum, a.rat / b.conversionFactor rat from";
                quer = quer + " (select * from purPurchasesDet where branchid = '" + inf.usr.bCode + "' and customercode = " + inf.usr.cCode + "";
                quer = quer + " and recordId in (select recordId from purpurchasesuni where branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + "";
                quer = quer + " and dat >= '" + dat1 + "' and dat < '" + dat2 + "'))a,";
                quer = quer + " (select * from invMaterialUnits where customerCode = " + inf.usr.cCode + ")b where a.um = b.um and a.itemId = b.recordId)a,";
                quer = quer + " (select * from invMaterialCompleteDetails_view where customerCode = " + inf.usr.cCode + ")b";
                quer = quer + " where a.itemId = b.recordId)x group by itemname, um)a,";
                quer = quer + " (select * from invMaterials where customercode = " + inf.usr.cCode + ")b where a.itemname = b.itemName)xx";
                quer = quer + " order by loss desc,profit";

                   DataBaseContext g = new DataBaseContext();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();

                while (dr.Read())
                {
                    tot.details.Add(new PurRepAnalysisInfomation
                    {
                        det1 = dr[0].ToString(),
                        det2 = dr[1].ToString(),
                        det3 = dr[2].ToString(),
                        det4 = dr[3].ToString(),
                        det5 = dr[4].ToString(),
                        det6 = dr[5].ToString(),
                     });

                }
                dr.Close();
                g.db.Close();

                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("item", typeof(string));
                dt.Columns.Add("qty", typeof(string));
                 dt.Columns.Add("actualval", typeof(string));
                 dt.Columns.Add("stdval", typeof(string));
                dt.Columns.Add("loss", typeof(string));
                dt.Columns.Add("profit", typeof(string));
                int i = 0;

                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i.ToString(), det.det1, det.det2, det.det3, det.det4, det.det5, det.det6);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Item");
                titles.Add("Qty");
                titles.Add("Actual Val");
                titles.Add("Std Val");
                titles.Add("Loss");
                titles.Add("Profit");
             
                float[] widths = { 30f, 145f, 75f, 75f, 75f, 75f, 75f  };
                int[] aligns = { 3, 1, 2, 2, 2, 2, 2  };
                PDFExcelMake ep = new PDFExcelMake();

                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "PurStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfConversion(filename, "Price Comparison of materials actual to standard from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Price Comparison of materials actual to standard from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
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
        [Route("api/PurRepAnalysis/PurAnalysisReplinishment1")]
        public PurRepAnalysisInfomationTotal PurAnalysisReplinishment1([FromBody] GeneralAnalysisInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 6, 0))
            {
                General gg = new General();
                
                string dat1 = ac.strDate(ac.getFinancialStart(ac.getPresentDateTime(),inf.usr));
                string dat2 = ac.strDate(ac.getPresentDateTime().AddDays(1));

                decimal? factor = Math.Ceiling((decimal)(inf.nextdatys / inf.previousdays));

                PurRepAnalysisInfomationTotal tot = new PurRepAnalysisInfomationTotal();
                tot.details = new List<PurRepAnalysisInfomation>();
                string quer = "";
                quer = quer + " select itemname,grpname,projected,available,reqd,um,estimatedprice from";
                quer = quer + " (select b.itemname, b.grpname, convert(varchar(20), a.projected) projected, convert(varchar(20), a.available) available,";
                quer = quer + " convert(varchar(20), a.requd) reqd, b.um, dbo.makCur(a.estimatedprice) estimatedprice, 1 sno from";
                quer = quer + " (select itemname, projected, available, projected - available requd, (projected - available) * rat estimatedprice from";
                quer = quer + " (select a.itemname, projected,case when available is null then 0 else available end available,";
                quer = quer + " case when rat is null then 0 else rat end rat from";
                quer = quer + " (select itemname, sum(qtyout)*" + factor + " projected from invmaterialmanagement where datediff(DD, dat, sysdatetime()) <= " + inf.previousdays + "";
                quer = quer + " and qtyout > 0 and branchId = '" + inf.usr.bCode + "' and customercode = " + inf.usr.cCode + " group by itemname)a left outer join";
                quer = quer + " (select a.itemname, available,case when b.rat is null then 0 else b.rat end rat from";
                quer = quer + " (select itemname, sum(Qtyin-qtyout) available from invMaterialManagement where";
                quer = quer + " dat >= '" + dat1 + "' and dat< '" + dat2 + "' and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + " group by itemname)a left outer join";
                quer = quer + " (select itemname, max(rat) rat from invMaterialManagement  where";
                quer = quer + " transactionType < 100 and dat >= '" + dat1 + "' and dat< '" + dat2 + "' and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + " group by itemname)b";
                quer = quer + " on a.itemName = b.itemName )b on a.itemName = b.itemName)x where projected > available)a,";
                quer = quer + " (select * from invMaterialCompleteDetails_view where customerCode = " + inf.usr.cCode + ")b where a.itemName = b.recordId";
                quer = quer + " union all";
                quer = quer + " select ' ' itemname,' ' grpname, ' ' projected, ' ' available,' ' reqd,' ' um,' ' estimatedprice,";
                quer = quer + " 2 sno";
                quer = quer + " union all";
                quer = quer + " select  'Total' itemname,' ' grpname, ' ' projected, ' ' available,' ' reqd,' ' um,dbo.makCur(sum((projected - available) * rat)) estimatedprice,3 sno from";
                quer = quer + " (select a.itemname, projected,case when available is null then 0 else available end available,";
                quer = quer + " case when rat is null then 0 else rat end rat from";
                quer = quer + " (select itemname, sum(qtyout)*" + factor + " projected from invmaterialmanagement where datediff(DD, dat, sysdatetime()) <= 30";
                quer = quer + " and qtyout > 0 and branchId = '" + inf.usr.bCode + "' and customercode = " + inf.usr.cCode + " group by itemname)a left outer join";
                quer = quer + " (select a.itemname, available,case when b.rat is null then 0 else b.rat end rat from";
                quer = quer + " (select itemname, sum(Qtyin-qtyout) available from invMaterialManagement where";
                quer = quer + " dat >= '" + dat1 + "' and dat< '" + dat2 + "' and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + " group by itemname)a left outer join";
                quer = quer + " (select itemname, max(rat) rat from invMaterialManagement  where";
                quer = quer + " transactionType < 100 and dat >= '" + dat1 + "' and dat< '" + dat2 + "' and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + " group by itemname)b";
                quer = quer + " on a.itemName = b.itemName )b on a.itemName = b.itemName)x where projected > available)x order by sno, itemname";

                DataBaseContext g = new DataBaseContext();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();

                while (dr.Read())
                {
                    tot.details.Add(new PurRepAnalysisInfomation
                    {
                        det1 = dr[0].ToString(),
                        det2 = dr[1].ToString(),
                        det3 = dr[2].ToString(),
                        det4 = dr[3].ToString(),
                        det5 = dr[4].ToString(),
                        det6 = dr[5].ToString(),
                        det7 = dr[6].ToString()
                    });

                }
                dr.Close();
                g.db.Close();

                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("item", typeof(string));
                dt.Columns.Add("grp", typeof(string));
                dt.Columns.Add("projected", typeof(string));
                dt.Columns.Add("available", typeof(string));
                dt.Columns.Add("reqd", typeof(string));
                dt.Columns.Add("um", typeof(string));
                dt.Columns.Add("estimated", typeof(string));
                int i = 0;

                foreach (var det in tot.details)
                {
                    dt.Rows.Add( i< tot.details.Count()-2? i.ToString():" ", det.det1, det.det2, det.det3, det.det4, det.det5, det.det6,det.det7);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Item");
                titles.Add("Group");
                titles.Add("Projected");
                titles.Add("Available");
                titles.Add("Required");
                titles.Add("UM");
                titles.Add("Estimation");

                float[] widths = { 30f, 175f,150f, 75f, 75f, 75f, 65f, 75f };
                int[] aligns = { 3, 1, 1, 2, 2, 2, 2,2 };
                PDFExcelMake ep = new PDFExcelMake();

                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "PurStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfLandscapeConversion(filename, "Replinishment by considering last " + inf.previousdays  + " days and projecting next " + inf.nextdatys + " days" , inf.usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Replinishment by considering last " + inf.previousdays + " days and projecting next " + inf.nextdatys + " days", inf.usr, dt, titles, widths, aligns, true);
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
        [Route("api/PurRepAnalysis/PurAnalysisReplinishment2")]
        public PurRepAnalysisInfomationTotal PurAnalysisReplinishment2([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 2, 11, 6, 0))
            {
                General gg = new General();

                string dat1 = ac.strDate(ac.getFinancialStart(ac.getPresentDateTime(), usr));
                string dat2 = ac.strDate(ac.getPresentDateTime().AddDays(1));

              
                PurRepAnalysisInfomationTotal tot = new PurRepAnalysisInfomationTotal();
                tot.details = new List<PurRepAnalysisInfomation>();
                string quer = "";
                quer = quer + " select x.itemname,reorderQty,available,reqd,case when y.um is null then ' '  else y.um end um,estimatedCost from";
                quer = quer + " (select itemname, convert(varchar(20), reorderQty) reorderQty, convert(varchar(20), available) available,";
                quer = quer + " convert(varchar(20), reorderQty - available) reqd, dbo.makCur((reorderQty - available) * stdrate) estimatedcost, 1 sno from";
                quer = quer + " (select a.recordId, a.itemname, a.reorderQty, a.stdrate,case when b.available is null then 0 else b.available end available from";
                quer = quer + " (select* from invMaterials where customerCode= " + usr.cCode + ")a left outer join";
                quer = quer + " (select itemname, sum(qtyin-qtyout) available from invMaterialManagement";
                quer = quer + " where dat >= '" + dat1 + "' and dat< '" + dat2 + "' and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + "";
                quer = quer + " group by itemname)b on a.recordId = b.itemName)x where reOrderQty > available";
                quer = quer + " union all";
                quer = quer + " select ' ' itemname,' ' reorderQty,' ' available, ' ' reqd,  ' ' estimatedCost, 2 sno";
                quer = quer + " union all";
                quer = quer + " select 'Total' itemname,' ' reorderQty,' ' available, ' ' reqd, dbo.makCur(sum((reorderQty - available) * stdrate)) estimatedcost,3 sno from";
                quer = quer + " (select a.recordId, a.itemname, a.reorderQty, a.stdrate,case when b.available is null then 0 else b.available end available from";
                quer = quer + " (select* from invMaterials where customerCode= " + usr.cCode + ")a left outer join";
                quer = quer + " (select itemname, sum(qtyin-qtyout) available from invMaterialManagement";
                quer = quer + " where dat >= '" + dat1 + "' and dat< '" + dat2 + "' and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + "";
                quer = quer + " group by itemname)b on a.recordId = b.itemName)x where reOrderQty > available)x left outer join invMaterialCompleteDetails_view y on x.itemName =y.itemname ";
                quer = quer + " order by sno,x.itemname";

                DataBaseContext g = new DataBaseContext();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();

                while (dr.Read())
                {
                    tot.details.Add(new PurRepAnalysisInfomation
                    {
                        det1 = dr[0].ToString(),
                        det2 = dr[1].ToString(),
                        det3 = dr[2].ToString(),
                        det4 = dr[3].ToString(),
                        det5 = dr[4].ToString(),
                        det6 = dr[5].ToString(),
                    });

                }
                dr.Close();
                g.db.Close();

                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("item", typeof(string));
                 dt.Columns.Add("minlevel", typeof(string));
                dt.Columns.Add("available", typeof(string));
                dt.Columns.Add("reqd", typeof(string));
                dt.Columns.Add("um", typeof(string));
                dt.Columns.Add("estimated", typeof(string));
                int i = 0;

                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : " ", det.det1, det.det2, det.det3, det.det4, det.det5, det.det6);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Item");
                 titles.Add("Min Stock");
                titles.Add("Available");
                titles.Add("Required");
                titles.Add("UM");
                titles.Add("Estimation");

                float[] widths = { 30f, 155f,  75f, 75f, 75f, 65f, 75f };
                int[] aligns = { 3, 1, 2, 2, 2, 2, 2 };
                PDFExcelMake ep = new PDFExcelMake();

                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = usr.uCode + "PurStockRep" + dat +  usr.cCode +  usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfConversion(filename, "Replinishment by considering minimum stock levels as on " + ac.strDate(dats), usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = usr.uCode + "PurStockRep" + dat + usr.cCode + usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Replinishment by considering minimum stock levels as on " + ac.strDate(dats), usr, dt, titles, widths, aligns, true);
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
