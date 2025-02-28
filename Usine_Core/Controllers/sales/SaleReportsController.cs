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
    public class SalRepInformation
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
        public int? rec1 { get; set; }
    }
    public class SalRepInformationTotal
    {
        public List<SalRepInformation> details { get; set; }
        public string pdfFile { get; set; }
        public string excelFile { get; set; }
    }

    public class SaleReportsController : ControllerBase
    {

        private readonly IHostingEnvironment ho;
        public SaleReportsController(IHostingEnvironment hostingEnvironment)
        {
            ho = hostingEnvironment;
        }

        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        [HttpPost]
        [Authorize]
        [Route("api/SaleReports/SalRepListOfSales")]
        public SalRepInformationTotal SalRepListOfSales([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 5, 9, 1, 0))
            {
                SalRepInformationTotal tot = new SalRepInformationTotal();
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                General gg = new General();
                tot.details = (from a in db.SalSalesUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                               select new SalRepInformation
                               {
                                   det1 = a.Seq,
                                   det2 = gg.strDate(a.Dat.Value),
                                   det3 = a.PartyName,
                                   
                                   det4 = gg.makeCur((double)a.Baseamt,2),
                                   det5 = gg.makeCur((double)a.Discount, 2),
                                   det6 = gg.makeCur((double)a.Taxes, 2),
                                   det7 = gg.makeCur((double)a.Others, 2),
                                   det8 = gg.makeCur((double)a.TotalAmt, 2),
                                   val1=a.Baseamt,
                                   val2=a.Discount,
                                   val3=a.Taxes,
                                   val4=a.Others,
                                   val5=a.TotalAmt
                               }
                             ).OrderBy(b => b.det1).ToList();

                var bas = tot.details.Sum(a => a.val1);
                var dis = tot.details.Sum(b => b.val2);
                var oth = tot.details.Sum(b => b.val4);
                var tax = tot.details.Sum(b => b.val3);
                var tamt = tot.details.Sum(b => b.val5);

                tot.details.Add(new SalRepInformation
                {
                    det1 = "",

                });
                tot.details.Add(new SalRepInformation
                {
                    det3 = "Total",
                    det4 = gg.makeCur((double)bas, 2),
                    det5 = gg.makeCur((double)dis, 2),
                    det6 = gg.makeCur((double)tax, 2),
                    det7 = gg.makeCur((double)oth, 2),
                    det8 = gg.makeCur((double)tamt, 2),
                });


                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("seq", typeof(string));
                dt.Columns.Add("dat", typeof(string));
                dt.Columns.Add("customer", typeof(string));
                dt.Columns.Add("base", typeof(string));
                dt.Columns.Add("disc", typeof(string));
                dt.Columns.Add("others", typeof(string));
                dt.Columns.Add("taxes", typeof(string));
                dt.Columns.Add("total", typeof(string));
                int i = 1;
              
                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : "", det.det1, det.det2, det.det3, det.det4, det.det5, det.det6, det.det7, det.det8);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Seq");
                titles.Add("Date");
                titles.Add("Customer");
                titles.Add("Base");
                titles.Add("Discount");
                titles.Add("Others");
                titles.Add("Taxes");
                titles.Add("Total");

                float[] widths = { 30f, 60f, 80f, 180f, 70f, 70f, 70f, 70f, 90f };
                int[] aligns = { 3, 1, 1, 1, 2, 2, 2, 2, 2 };
                PDFExcelMake ep = new PDFExcelMake();

                AdminControl ac = new AdminControl();
                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "SalRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfLandscapeConversion(filename, "Sales from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Sales from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
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
        [Route("api/SaleReports/SalRepListOfSalesDetailed")]
        public SalRepInformationTotal SalRepListOfSalesDetailed([FromBody] GeneralInformation inf)
        {
            if(ac.screenCheck(inf.usr,5,9,1,0))
            {

                SalRepInformationTotal tot = new SalRepInformationTotal();
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                General gg = new General();
                var details = (from a in db.SalSalesUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                               select new SalRepInformation
                               {
                                   det1 = a.Seq,
                                   det2 = gg.strDate(a.Dat.Value),
                                   det3 = a.PartyName,

                                   det4=gg.makeCur((double)a.Baseamt,2 ),
                                   det5 = gg.makeCur((double)a.Discount, 2),
                                   det6 = gg.makeCur((double)a.Others, 2),
                                   det7 = gg.makeCur((double)a.Taxes, 2),
                                   det8 = gg.makeCur((double)a.TotalAmt, 2),

                                   val1 = a.Baseamt,
                                   val2 = a.Discount,
                                   val3 = a.Others,
                                   val4 = a.Taxes,
                                   val5 = a.TotalAmt,
                                   rec1 = a.RecordId
                               }
                             ).OrderBy(b => b.det1).ToList();

                tot.details = new List<SalRepInformation>();
                foreach (var det in details)
                {
                    tot.details.Add(det);
                    var subdet = (from a in db.SalSalesDet.Where(a => a.RecordId == det.rec1 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                  join b in db.InvUm.Where(a => a.CustomerCode == inf.usr.cCode) on a.Um equals b.RecordId
                                  select new SalRepInformation
                                  {
                                      det1 = "",
                                      det3 = a.ItemName,
                                      det5 = a.Qty.ToString(),
                                      det6 = b.Um,
                                      det7 = a.Rat.ToString(),
                                      det8 = gg.makeCur((double)( a.Qty * a.Rat),2),
                                      rec1 = a.Sno
                                  }
                                ).OrderBy(c => c.rec1).ToList();
                    tot.details.AddRange(subdet);
                }



                var bas = details.Sum(a => a.val1);
                var dis = details.Sum(b => b.val2);
                var oth = details.Sum(b => b.val3);
                var tax = details.Sum(b => b.val4);
                var tamt = details.Sum(b => b.val5);

                tot.details.Add(new SalRepInformation
                {
                    det1 = "",

                });
                tot.details.Add(new SalRepInformation
                {
                    det3 = "Total",
                    det4 =gg.makeCur((double)bas,2),
                    det5 = gg.makeCur((double)dis, 2),
                    det6 = gg.makeCur((double)oth, 2),
                    det7 = gg.makeCur((double)tax, 2),
                    det8 = gg.makeCur((double)tamt, 2),
                  });


                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("seq", typeof(string));
                dt.Columns.Add("dat", typeof(string));
                dt.Columns.Add("vendor", typeof(string));
                dt.Columns.Add("base", typeof(string));
                dt.Columns.Add("disc", typeof(string));
                dt.Columns.Add("others", typeof(string));
                dt.Columns.Add("taxes", typeof(string));
                dt.Columns.Add("total", typeof(string));
                int i = 1;
               
                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : "", det.det1,det.det2,det.det3,det.det4,det.det5,det.det6,det.det7,det.det8);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Seq");
                titles.Add("Date");
                titles.Add("Customer");
                titles.Add("Base");
                titles.Add("Discount");
                titles.Add("Others");
                titles.Add("Taxes");
                titles.Add("Total");


                float[] widths = { 30f, 60f, 80f, 180f, 70f, 70f, 70f, 70f, 90f };
                int[] aligns = { 3, 1, 1, 1, 2, 2, 2, 2, 2 };
                PDFExcelMake ep = new PDFExcelMake();

                AdminControl ac = new AdminControl();
                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfLandscapeConversion(filename, "Sales from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Sales from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
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
        [Route("api/SaleReports/SalRepListOfSalesConsolidated")]
        public SalRepInformationTotal SalRepListOfSalesConsolidated([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 5, 9, 1, 0))
            {
                SalRepInformationTotal tot = new SalRepInformationTotal();
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                General gg = new General();
                var details = (from a in db.SalSalesUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                               select new SalRepInformation
                               {
                                   det1 = gg.strDate(a.Dat.Value),
                                   val1 = a.Baseamt,
                                   val2 = a.Discount,
                                   val3 = a.Others,
                                   val4 = a.Taxes,
                                   val5= a.TotalAmt
                               }
                              ).ToList();

                tot.details = (from a in details.GroupBy(b => b.det1)
                               select new SalRepInformation
                               {
                                   det1 = a.Key,
                                   det2=gg.makeCur((double)a.Sum(b => b.val1),2),
                                   det3 = gg.makeCur((double)a.Sum(b => b.val2), 2),
                                   det4 = gg.makeCur((double)a.Sum(b => b.val3), 2),
                                   det5 = gg.makeCur((double)a.Sum(b => b.val4), 2),
                                   det6 = gg.makeCur((double)a.Sum(b => b.val5), 2),
                                
                               }).OrderBy(c => c.det1).ToList();


                var bas =  details.Sum(a => a.val1);
                var dis = details.Sum(b => b.val2);
                var oth =  details.Sum(b => b.val3);
                var tax =  details.Sum(b => b.val4);
                var tamt =  details.Sum(b => b.val5);

                tot.details.Add(new SalRepInformation
                {
                    det1 = "",

                });
                tot.details.Add(new SalRepInformation
                {
                    det1 = "Total",
                    det2 = gg.makeCur((double)bas, 2),
                    det3 = gg.makeCur((double)dis, 2),
                    det4 = gg.makeCur((double)oth, 2),
                    det5 = gg.makeCur((double)tax, 2),
                    det6 = gg.makeCur((double)tamt, 2),
                });


                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("dat", typeof(string));
                dt.Columns.Add("base", typeof(string));
                dt.Columns.Add("disc", typeof(string));
                dt.Columns.Add("others", typeof(string));
                dt.Columns.Add("taxes", typeof(string));
                dt.Columns.Add("total", typeof(string));
                int i = 1;


                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : "", det.det1,det.det2,det.det3,det.det4,det.det5,det.det6);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Date");
                titles.Add("Base");
                titles.Add("Discount");
                titles.Add("Others");
                titles.Add("Taxes");
                titles.Add("Total");

                float[] widths = { 50f, 100f, 80f, 80f, 80f, 80f, 80f };
                int[] aligns = { 3, 1, 2, 2, 2, 2, 2 };
                PDFExcelMake ep = new PDFExcelMake();

                AdminControl ac = new AdminControl();
                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfConversion(filename, "Consolidated Sales from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Consolidated Sales from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
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
        [Route("api/SaleReports/SalRepListOfSalesCustomerWise")]
        public SalRepInformationTotal SalRepListOfSalesCustomerWise([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 5, 9, 1, 0))
            {
                SalRepInformationTotal tot = new SalRepInformationTotal();
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                General gg = new General();
                var customer = db.PartyDetails.Where(a => a.RecordId == inf.recordId && a.PartyType == "CUS" && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).Select(b => b.PartyName).FirstOrDefault();
                tot.details = (from a in db.SalSalesUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.PartyId==inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                               select new SalRepInformation
                               {
                                   det1 = a.Seq,
                                   det2 = gg.strDate(a.Dat.Value),
                                   det3 = a.PartyName,

                                   det4 = gg.makeCur((double)a.Baseamt, 2),
                                   det5 = gg.makeCur((double)a.Discount, 2),
                                   det6 = gg.makeCur((double)a.Taxes, 2),
                                   det7 = gg.makeCur((double)a.Others, 2),
                                   det8 = gg.makeCur((double)a.TotalAmt, 2),
                                   val1 = a.Baseamt,
                                   val2 = a.Discount,
                                   val3 = a.Taxes,
                                   val4 = a.Others,
                                   val5 = a.TotalAmt
                               }
                             ).OrderBy(b => b.det1).ToList();

                var bas = tot.details.Sum(a => a.val1);
                var dis = tot.details.Sum(b => b.val2);
                var oth = tot.details.Sum(b => b.val4);
                var tax = tot.details.Sum(b => b.val3);
                var tamt = tot.details.Sum(b => b.val5);

                tot.details.Add(new SalRepInformation
                {
                    det1 = "",

                });
                tot.details.Add(new SalRepInformation
                {
                    det3 = "Total",
                    det4 = gg.makeCur((double)bas, 2),
                    det5 = gg.makeCur((double)dis, 2),
                    det6 = gg.makeCur((double)tax, 2),
                    det7 = gg.makeCur((double)oth, 2),
                    det8 = gg.makeCur((double)tamt, 2),
                });


                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("seq", typeof(string));
                dt.Columns.Add("dat", typeof(string));
                dt.Columns.Add("customer", typeof(string));
                dt.Columns.Add("base", typeof(string));
                dt.Columns.Add("disc", typeof(string));
                dt.Columns.Add("others", typeof(string));
                dt.Columns.Add("taxes", typeof(string));
                dt.Columns.Add("total", typeof(string));
                int i = 1;

                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : "", det.det1, det.det2, det.det3, det.det4, det.det5, det.det6, det.det7, det.det8);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Seq");
                titles.Add("Date");
                titles.Add("Customer");
                titles.Add("Base");
                titles.Add("Discount");
                titles.Add("Others");
                titles.Add("Taxes");
                titles.Add("Total");

                float[] widths = { 30f, 60f, 80f, 180f, 70f, 70f, 70f, 70f, 90f };
                int[] aligns = { 3, 1, 1, 1, 2, 2, 2, 2, 2 };
                PDFExcelMake ep = new PDFExcelMake();

                AdminControl ac = new AdminControl();
                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "SalRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfLandscapeConversion(filename, "Sale Details of Customer " + customer +" from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Sale Details of Customer " + customer + " from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
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
        [Route("api/SaleReports/SalRepListOfSalesCustomerWiseConsolidated")]
        public SalRepInformationTotal SalRepListOfSalesCustomerWiseConsolidated([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 5, 9, 1, 0))
            {
                SalRepInformationTotal tot = new SalRepInformationTotal();
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                General gg = new General();
                var details = (from a in db.SalSalesUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                               select new SalRepInformation
                               {
                                   det1 = a.PartyName,
                                   val1 = a.Baseamt,
                                   val2 = a.Discount,
                                   val3 = a.Others,
                                   val4 = a.Taxes,
                                   val5 = a.TotalAmt
                               }
                              ).ToList();

                tot.details = (from a in details.GroupBy(b => b.det1)
                               select new SalRepInformation
                               {
                                   det1 = a.Key,
                                   det2 = gg.makeCur((double)a.Sum(b => b.val1), 2),
                                   det3 = gg.makeCur((double)a.Sum(b => b.val2), 2),
                                   det4 = gg.makeCur((double)a.Sum(b => b.val3), 2),
                                   det5 = gg.makeCur((double)a.Sum(b => b.val4), 2),
                                   det6 = gg.makeCur((double)a.Sum(b => b.val5), 2),

                               }).OrderBy(c => c.det1).ToList();


                var bas = details.Sum(a => a.val1);
                var dis = details.Sum(b => b.val2);
                var oth = details.Sum(b => b.val3);
                var tax = details.Sum(b => b.val4);
                var tamt = details.Sum(b => b.val5);

                tot.details.Add(new SalRepInformation
                {
                    det1 = "",

                });
                tot.details.Add(new SalRepInformation
                {
                    det1 = "Total",
                    det2 = gg.makeCur((double)bas, 2),
                    det3 = gg.makeCur((double)dis, 2),
                    det4 = gg.makeCur((double)oth, 2),
                    det5 = gg.makeCur((double)tax, 2),
                    det6 = gg.makeCur((double)tamt, 2),
                });


                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("dat", typeof(string));
                dt.Columns.Add("base", typeof(string));
                dt.Columns.Add("disc", typeof(string));
                dt.Columns.Add("others", typeof(string));
                dt.Columns.Add("taxes", typeof(string));
                dt.Columns.Add("total", typeof(string));
                int i = 1;


                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : "", det.det1, det.det2, det.det3, det.det4, det.det5, det.det6);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Date");
                titles.Add("Base");
                titles.Add("Discount");
                titles.Add("Others");
                titles.Add("Taxes");
                titles.Add("Total");

                float[] widths = { 50f, 100f, 80f, 80f, 80f, 80f, 80f };
                int[] aligns = { 3, 1, 2, 2, 2, 2, 2 };
                PDFExcelMake ep = new PDFExcelMake();

                AdminControl ac = new AdminControl();
                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfConversion(filename, "Customer wise consolidated Sales from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Customer wise consolidated Sales from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
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
