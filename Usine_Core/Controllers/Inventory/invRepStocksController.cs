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

namespace Usine_Core.Controllers.Inventory
{
    public class InvRepStocksInfomation
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

        public double? val1 { get; set; }
        public double? val2 { get; set; }
        public double? val3 { get; set; }
        public double? val4 { get; set; }
        public double? val5 { get; set; }
    }
    public class InvRepStocksInfomationTotal
    {
        public List<InvRepStocksInfomation> details { get; set; }
        public string pdfFile { get; set; }
        public string excelFile { get; set; }
    }
    public class invRepStocksController : ControllerBase
    {
        private readonly IHostingEnvironment ho;
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        public invRepStocksController(IHostingEnvironment hostingEnvironment)
        {
            ho = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        [Route("api/invRepStocks/InvRepStocksClosingStocks")]
        public InvRepStocksInfomationTotal InvRepStocksClosingStocks([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 3, 9, 3, 0))
            {
                General gg = new General();
                string dat1 =gg.strDate(ac.getFinancialStart(ac.getPresentDateTime(), inf.usr));
                string dat2 = gg.strDate(DateTime.Parse(inf.toDate).AddDays(1));



                InvRepStocksInfomationTotal tot = new InvRepStocksInfomationTotal();
                tot.details = new List<InvRepStocksInfomation>();
                string quer = "";
                quer = quer + " select b.itemname,grpname,clstock,um, dbo.makCur(valu) valudet, valu from";
                quer = quer + " (select itemname, sum(closingStock) clstock, sum(closingStock * rat) valu from";
                quer = quer + " (select a.gin, itemname, closingstock, rat from";
                quer = quer + " (select gin, itemname, sum(qtyin - qtyout) closingstock from invMaterialManagement where";
                quer = quer + " dat >= '" + dat1 + "' and dat < '" + dat2 + "' and";
                quer = quer + " itemname in (select recordId from invMaterials where costingType > 1 and customerCode = " + inf.usr.cCode + ")";
                quer = quer + " and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + "  group by gin, itemname)a,";
                quer = quer + " (select gin, rat from invMaterialManagement where transactionType < 100 and";
                quer = quer + " branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b where a.gin = b.gin)x group by itemname";
                quer = quer + " union all";
                quer = quer + " select a.itemname, a.closingStock clstock, (a.closingStock * b.rat) valu from";
                quer = quer + " (select itemname, sum(qtyin-qtyout) closingstock from invMaterialManagement where";
                quer = quer + " dat >= '" + dat1 + "' and dat< '" + dat2 + "' and";
                quer = quer + " itemname in (select recordId from invMaterials where costingType = 1 and customerCode = " + inf.usr.cCode + ")";
                quer = quer + " and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + "  group by  itemname)a,";
                quer = quer + " (select itemname, sum(qtyin * rat) / sum(qtyin) rat from invMaterialManagement";
                quer = quer + " where transactionType < 100 and dat >= '" + dat1 + "' and dat< '" + dat2 + "' and";
                quer = quer + " branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + " group by itemname)b where a.itemName = b.itemName)a,";
                quer = quer + " (select * from invMaterialCompleteDetails_view where customercode = " + inf.usr.cCode + ")b where";
                quer = quer + " a.itemName = b.recordId order by itemname";


                DataBaseContext g = new DataBaseContext();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();

                while (dr.Read())
                {
                    tot.details.Add(new InvRepStocksInfomation
                    {
                        det1 = dr[0].ToString(),
                        det2 = dr[1].ToString(),
                        det3 = dr[2].ToString(),
                        det4 = dr[3].ToString(),
                        det5 = dr[4].ToString(),
                        val1=gg.valNum(dr[5].ToString())
                    });

                }
                dr.Close();
                g.db.Close();
                if(tot.details.Count() > 0)
                {
                    var total = tot.details.Sum(a => a.val1);
                    tot.details.Add(new InvRepStocksInfomation
                    {
                        det1=" ",
                        det2 = " ",
                        det3 = " ",
                        det4 = " ",
                        det5 = " ",

                    });
                    
                    tot.details.Add(new InvRepStocksInfomation
                    {
                        det1 = "Total",
                        det2 = " ",
                        det3 = " ",
                        det4 = " ",
                        det5 = gg.makeCur((double)total,2),

                    });
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("item", typeof(string));
                dt.Columns.Add("grp", typeof(string));
                dt.Columns.Add("clstock", typeof(string));
                dt.Columns.Add("um", typeof(string));
                dt.Columns.Add("valu", typeof(string));
                  int i = 0;

                foreach (var det in tot.details)
                {
                    dt.Rows.Add( i<tot.details.Count()-2 ? i.ToString():"", det.det1, det.det2, det.det3, det.det4, det.det5);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Item");
                titles.Add("Group");
                titles.Add("Stock");
                titles.Add("UM");
                titles.Add("Value");
            
                float[] widths = { 30f, 175f, 145f,  65f, 60f, 75f };
                int[] aligns = { 3, 1, 1, 2, 2, 2};
                PDFExcelMake ep = new PDFExcelMake();

                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "InvStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfConversion(filename, "Closing Stock as on " + inf.toDate , inf.usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "InvStockRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Closing Stock as on " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
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
