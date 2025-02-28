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
    public class PurRepPurchasesTotal
    {
        public string pdfFile { get; set; }
        public string excelFile { get; set; }
        public  List<PurPurchasesUni>  details { get; set; }
    }
    public class PurCreditNotesTotal
    {
        public List<PartyCreditDebitNotes> details { get; set; }
        public string pdfFile { get; set; }
        public string excelFile { get; set; }
    }
    public class PurRepPurchasesController : ControllerBase
    {
        private readonly IHostingEnvironment ho;
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        public PurRepPurchasesController(IHostingEnvironment hostingEnvironment)
        {
            ho = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurRepPurchases/purRepPurchasesListofPurchases")]
        public PurRepPurchasesTotal purRepPurchasesListofPurchases([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 3, 0))
            {
                PurRepPurchasesTotal tot = new PurRepPurchasesTotal();
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);

                tot.details = (from a in db.PurPurchasesUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                               select new PurPurchasesUni
                               {
                                 Seq=  a.Seq,
                                  Dat= a.Dat,
                                  Vendorname= a.Vendorname,
                                   Baseamt = a.Baseamt,
                                   Discount= a.Discount,
                                   Others = a.Others,
                                   Taxes=a.Taxes,
                                  TotalAmt= a.TotalAmt
                               }
                             ).OrderBy(b => b.Dat).ThenBy(c => c.Seq).ToList();

                var bas = tot.details.Sum(a => a.Baseamt);
                var dis = tot.details.Sum(b => b.Discount);
                var oth = tot.details.Sum(b => b.Others);
                var tax = tot.details.Sum(b => b.Taxes);
                var tamt = tot.details.Sum(b => b.TotalAmt);

                tot.details.Add(new PurPurchasesUni
                {
                    Seq = "",
                    
                }) ;
                tot.details.Add(new PurPurchasesUni
                {
                    Vendorname="Total",
                    Baseamt=bas,
                    Discount=dis,
                    Others=oth,
                    Taxes=tax,
                    TotalAmt=tamt
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
                General gg = new General();

                foreach (var det in tot.details)
                {
                    dt.Rows.Add( i <tot.details.Count()-2? i.ToString():"",  det.Seq,det.Dat==null?" " :gg.strDateTime(det.Dat.Value), det.Vendorname,det.Baseamt==null?"":gg.makeCur((double)det.Baseamt,2), det.Discount == null ? "" : gg.makeCur((double)det.Discount, 2), det.Others == null ? "" : gg.makeCur((double)det.Others, 2), det.Taxes == null ? "" : gg.makeCur((double)det.Taxes, 2), det.TotalAmt == null ? "" : gg.makeCur((double)det.TotalAmt, 2));
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Seq");
                titles.Add("Date");
                titles.Add("Supplier");
                titles.Add("Base");
                titles.Add("Discount");
                titles.Add("Others");
                titles.Add("Taxes");
                titles.Add("Total");
                
                float[] widths = { 30f, 60f, 80f, 180f, 70f,70f,70f,70f,90f};
                int[] aligns = { 3, 1, 1, 1 ,2,2,2,2,2 };
                PDFExcelMake ep = new PDFExcelMake();

                AdminControl ac = new AdminControl();
                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfLandscapeConversion(filename, "Purchases from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Purchases from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
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
        [Route("api/PurRepPurchases/purRepPurchasesListofPurchasesConsolidated")]
        public PurRepPurchasesTotal purRepPurchasesListofPurchasesConsolidated([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 3, 0))
            {
                PurRepPurchasesTotal tot = new PurRepPurchasesTotal();
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                General gg = new General();
               var details = (from a in db.PurPurchasesUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                               select new PurPurchasesUni
                               {
                                   Seq = gg.strDate(a.Dat.Value),
                                   Baseamt = a.Baseamt,
                                   Discount = a.Discount,
                                   Others = a.Others,
                                   Taxes = a.Taxes,
                                   TotalAmt = a.TotalAmt
                               }
                             ).ToList();

                tot.details = (from a in details.GroupBy(b => b.Seq)
                               select new PurPurchasesUni
                               {
                                   Seq = a.Key,
                                   Baseamt = a.Sum(b => b.Baseamt),
                                   Discount = a.Sum(b => b.Discount),
                                   Others = a.Sum(b => b.Others),
                                   Taxes = a.Sum(b => b.Taxes),
                                   TotalAmt = a.Sum(b => b.TotalAmt)
                               }).OrderBy(c => c.Seq).ToList();


                var bas = tot.details.Sum(a => a.Baseamt);
                var dis = tot.details.Sum(b => b.Discount);
                var oth = tot.details.Sum(b => b.Others);
                var tax = tot.details.Sum(b => b.Taxes);
                var tamt = tot.details.Sum(b => b.TotalAmt);

                tot.details.Add(new PurPurchasesUni
                {
                    Seq = "",

                });
                tot.details.Add(new PurPurchasesUni
                {
                    Vendorname = "Total",
                    Baseamt = bas,
                    Discount = dis,
                    Others = oth,
                    Taxes = tax,
                    TotalAmt = tamt
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
                    dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : "", det.Seq, det.Baseamt == null ? "" : gg.makeCur((double)det.Baseamt, 2), det.Discount == null ? "" : gg.makeCur((double)det.Discount, 2), det.Others == null ? "" : gg.makeCur((double)det.Others, 2), det.Taxes == null ? "" : gg.makeCur((double)det.Taxes, 2), det.TotalAmt == null ? "" : gg.makeCur((double)det.TotalAmt, 2));
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

                float[] widths = { 50f, 100f,   80f, 80f, 80f, 80f, 80f};
                int[] aligns = { 3, 1,  2,2, 2, 2, 2 };
                PDFExcelMake ep = new PDFExcelMake();

                AdminControl ac = new AdminControl();
                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfConversion(filename, "Consolidated Purchases from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Consolidated Purchases from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
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
        [Route("api/PurRepPurchases/purRepPurchasesListofPurchasesDetailed")]
        public PurRepPurchasesTotal purRepPurchasesListofPurchasesDetailed([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 3, 0))
            {
                PurRepPurchasesTotal tot = new PurRepPurchasesTotal();
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);

                var details = (from a in db.PurPurchasesUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                               select new PurPurchasesUni
                               {
                                   Seq = a.Seq,
                                   Dat = a.Dat,
                                   Vendorname = a.Vendorname,
                                   Baseamt =  a.Baseamt,
                                   Discount = a.Discount,
                                   Others = a.Others,
                                   Taxes = a.Taxes,
                                   TotalAmt = a.TotalAmt,
                                   RecordId=a.RecordId
                               }
                             ).OrderBy(b => b.Dat).ThenBy(c => c.Seq).ToList();

                tot.details = new List<PurPurchasesUni>();
                foreach (var det in details)
                {
                    tot.details.Add(det);
                    var subdet = (from a in db.PurPurchasesDet.Where(a => a.RecordId == det.RecordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                  join b in db.InvUm.Where(a => a.CustomerCode == inf.usr.cCode) on a.Um equals b.RecordId
                                  select new PurPurchasesUni
                                  {
                                      Seq = "",
                                      Vendorname = a.ItemName,
                                      Discount = a.Qty,
                                      BranchId = b.Um,
                                      Others = a.Rat,
                                      TotalAmt = a.Qty * a.Rat,
                                      RecordId = a.Sno
                                  }
                                ).OrderBy(c => c.RecordId).ToList();
                    tot.details.AddRange(subdet);
                }



                var bas =  details.Sum(a => a.Baseamt);
                var dis =  details.Sum(b => b.Discount);
                var oth =  details.Sum(b => b.Others);
                var tax = details.Sum(b => b.Taxes);
                var tamt = details.Sum(b => b.TotalAmt);

                tot.details.Add(new PurPurchasesUni
                {
                    Seq = "",

                });
                tot.details.Add(new PurPurchasesUni
                {
                    Vendorname = "Total",
                    Baseamt = bas,
                    Discount = dis,
                    Others = oth,
                    Taxes = tax,
                    TotalAmt = tamt
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
                General gg = new General();

                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : "", det.Seq, det.Dat == null ? " " : gg.strDateTime(det.Dat.Value), det.Vendorname, det.Baseamt == null ? "" : gg.makeCur((double)det.Baseamt, 2), det.Discount == null ? "" : gg.makeCur((double)det.Discount, 2), det.Others == null ? "" : gg.makeCur((double)det.Others, 2), det.Taxes == null ? det.BranchId : gg.makeCur((double)det.Taxes, 2), det.TotalAmt == null ? "" : gg.makeCur((double)det.TotalAmt, 2));
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Seq");
                titles.Add("Date");
                titles.Add("Supplier");
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

                msg = ep.pdfLandscapeConversion(filename, "Purchases from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Purchases from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
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
        [Route("api/PurRepPurchases/purRepPurchasesListofPurchasesSupplierWise")]
        public PurRepPurchasesTotal purRepPurchasesListofPurchasesSupplierWise([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 3, 0))
            {
                PurRepPurchasesTotal tot = new PurRepPurchasesTotal();
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);

                tot.details = (from a in db.PurPurchasesUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.Vendorname==inf.detail && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                               select new PurPurchasesUni
                               {
                                   Seq = a.Seq,
                                   Dat = a.Dat,
                                   Vendorname = a.Vendorname,
                                   Baseamt = a.Baseamt,
                                   Discount = a.Discount,
                                   Others = a.Others,
                                   Taxes = a.Taxes,
                                   TotalAmt = a.TotalAmt
                               }
                             ).OrderBy(b => b.Dat).ThenBy(c => c.Seq).ToList();

                var bas = tot.details.Sum(a => a.Baseamt);
                var dis = tot.details.Sum(b => b.Discount);
                var oth = tot.details.Sum(b => b.Others);
                var tax = tot.details.Sum(b => b.Taxes);
                var tamt = tot.details.Sum(b => b.TotalAmt);

                tot.details.Add(new PurPurchasesUni
                {
                    Seq = "",

                });
                tot.details.Add(new PurPurchasesUni
                {
                    Vendorname = "Total",
                    Baseamt = bas,
                    Discount = dis,
                    Others = oth,
                    Taxes = tax,
                    TotalAmt = tamt
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
                General gg = new General();

                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : "", det.Seq, det.Dat == null ? " " : gg.strDateTime(det.Dat.Value), det.Vendorname, det.Baseamt == null ? "" : gg.makeCur((double)det.Baseamt, 2), det.Discount == null ? "" : gg.makeCur((double)det.Discount, 2), det.Others == null ? "" : gg.makeCur((double)det.Others, 2), det.Taxes == null ? "" : gg.makeCur((double)det.Taxes, 2), det.TotalAmt == null ? "" : gg.makeCur((double)det.TotalAmt, 2));
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Seq");
                titles.Add("Date");
                titles.Add("Supplier");
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

                msg = ep.pdfLandscapeConversion(filename, "Purchases of supplier " + inf.detail + " from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Purchases of supplier " + inf.detail + " from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
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
        [Route("api/PurRepPurchases/purRepPurchasesListofPurchasesSupplierConsolidated")]
        public PurRepPurchasesTotal purRepPurchasesListofPurchasesSupplierConsolidated([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 3, 0))
            {
                PurRepPurchasesTotal tot = new PurRepPurchasesTotal();
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                General gg = new General();
                var details = (from a in db.PurPurchasesUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                               select new PurPurchasesUni
                               {
                                   Seq = a.Vendorname,
                                   Baseamt = a.Baseamt,
                                   Discount = a.Discount,
                                   Others = a.Others,
                                   Taxes = a.Taxes,
                                   TotalAmt = a.TotalAmt
                               }
                              ).ToList();

                tot.details = (from a in details.GroupBy(b => b.Seq)
                               select new PurPurchasesUni
                               {
                                   Seq = a.Key,
                                   Baseamt = a.Sum(b => b.Baseamt),
                                   Discount = a.Sum(b => b.Discount),
                                   Others = a.Sum(b => b.Others),
                                   Taxes = a.Sum(b => b.Taxes),
                                   TotalAmt = a.Sum(b => b.TotalAmt)
                               }).OrderBy(c => c.Seq).ToList();


                var bas = tot.details.Sum(a => a.Baseamt);
                var dis = tot.details.Sum(b => b.Discount);
                var oth = tot.details.Sum(b => b.Others);
                var tax = tot.details.Sum(b => b.Taxes);
                var tamt = tot.details.Sum(b => b.TotalAmt);

                tot.details.Add(new PurPurchasesUni
                {
                    Seq = "",

                });
                tot.details.Add(new PurPurchasesUni
                {
                    Vendorname = "Total",
                    Baseamt = bas,
                    Discount = dis,
                    Others = oth,
                    Taxes = tax,
                    TotalAmt = tamt
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
                    dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : "", det.Seq, det.Baseamt == null ? "" : gg.makeCur((double)det.Baseamt, 2), det.Discount == null ? "" : gg.makeCur((double)det.Discount, 2), det.Others == null ? "" : gg.makeCur((double)det.Others, 2), det.Taxes == null ? "" : gg.makeCur((double)det.Taxes, 2), det.TotalAmt == null ? "" : gg.makeCur((double)det.TotalAmt, 2));
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Supplier");
                titles.Add("Base");
                titles.Add("Discount");
                titles.Add("Others");
                titles.Add("Taxes");
                titles.Add("Total");

                float[] widths = { 30f, 170f, 70f, 70f, 70f, 70f, 70f };
                int[] aligns = { 3, 1, 2, 2, 2, 2,2 };
                PDFExcelMake ep = new PDFExcelMake();

                AdminControl ac = new AdminControl();
                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfConversion(filename, "Supplier wise Consolidated Purchases from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Supplier wise Consolidated Purchases from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
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
        [Route("api/PurRepPurchases/purRepPurchasesListofPurchaseReturns")]
        public PurRepPurchasesTotal purRepPurchasesListofPurchaseReturns([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 3, 0))
            {
                PurRepPurchasesTotal tot = new PurRepPurchasesTotal();
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);

                tot.details = (from a in db.PurPurchaseReturnsUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                               select new PurPurchasesUni
                               {
                                   Seq = a.Seq,
                                   Dat = a.Dat,
                                   Vendorname = a.Vendorname,
                                   Baseamt = a.Baseamt,
                                   Discount = a.Discount,
                                   Others = a.Others,
                                   Taxes = a.Taxes,
                                   TotalAmt = a.TotalAmt
                               }
                             ).OrderBy(b => b.Dat).ThenBy(c => c.Seq).ToList();

                var bas = tot.details.Sum(a => a.Baseamt);
                var dis = tot.details.Sum(b => b.Discount);
                var oth = tot.details.Sum(b => b.Others);
                var tax = tot.details.Sum(b => b.Taxes);
                var tamt = tot.details.Sum(b => b.TotalAmt);

                tot.details.Add(new PurPurchasesUni
                {
                    Seq = "",

                });
                tot.details.Add(new PurPurchasesUni
                {
                    Vendorname = "Total",
                    Baseamt = bas,
                    Discount = dis,
                    Others = oth,
                    Taxes = tax,
                    TotalAmt = tamt
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
                General gg = new General();

                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : "", det.Seq, det.Dat == null ? " " : gg.strDateTime(det.Dat.Value), det.Vendorname, det.Baseamt == null ? "" : gg.makeCur((double)det.Baseamt, 2), det.Discount == null ? "" : gg.makeCur((double)det.Discount, 2), det.Others == null ? "" : gg.makeCur((double)det.Others, 2), det.Taxes == null ? "" : gg.makeCur((double)det.Taxes, 2), det.TotalAmt == null ? "" : gg.makeCur((double)det.TotalAmt, 2));
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Seq");
                titles.Add("Date");
                titles.Add("Supplier");
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

                msg = ep.pdfLandscapeConversion(filename, "Purchase Returns from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Purchase Returns from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
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
        [Route("api/PurRepPurchases/purRepPurchasesPurchaseReturnsConsolidated")]
        public PurRepPurchasesTotal purRepPurchasesPurchaseReturnsConsolidated([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 3, 0))
            {
                PurRepPurchasesTotal tot = new PurRepPurchasesTotal();
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                General gg = new General();
                var details = (from a in db.PurPurchaseReturnsUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                               select new PurPurchasesUni
                               {
                                   Seq = gg.strDate(a.Dat.Value),
                                   Baseamt = a.Baseamt,
                                   Discount = a.Discount,
                                   Others = a.Others,
                                   Taxes = a.Taxes,
                                   TotalAmt = a.TotalAmt
                               }
                              ).ToList();

                tot.details = (from a in details.GroupBy(b => b.Seq)
                               select new PurPurchasesUni
                               {
                                   Seq = a.Key,
                                   Baseamt = a.Sum(b => b.Baseamt),
                                   Discount = a.Sum(b => b.Discount),
                                   Others = a.Sum(b => b.Others),
                                   Taxes = a.Sum(b => b.Taxes),
                                   TotalAmt = a.Sum(b => b.TotalAmt)
                               }).OrderBy(c => c.Seq).ToList();


                var bas = tot.details.Sum(a => a.Baseamt);
                var dis = tot.details.Sum(b => b.Discount);
                var oth = tot.details.Sum(b => b.Others);
                var tax = tot.details.Sum(b => b.Taxes);
                var tamt = tot.details.Sum(b => b.TotalAmt);

                tot.details.Add(new PurPurchasesUni
                {
                    Seq = "",

                });
                tot.details.Add(new PurPurchasesUni
                {
                    Vendorname = "Total",
                    Baseamt = bas,
                    Discount = dis,
                    Others = oth,
                    Taxes = tax,
                    TotalAmt = tamt
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
                    dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : "", det.Seq, det.Baseamt == null ? "" : gg.makeCur((double)det.Baseamt, 2), det.Discount == null ? "" : gg.makeCur((double)det.Discount, 2), det.Others == null ? "" : gg.makeCur((double)det.Others, 2), det.Taxes == null ? "" : gg.makeCur((double)det.Taxes, 2), det.TotalAmt == null ? "" : gg.makeCur((double)det.TotalAmt, 2));
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

                msg = ep.pdfConversion(filename, "Consolidated Purchase Returns from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Consolidated Purchase Returns from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
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
        [Route("api/PurRepPurchases/purRepPurchaseReturnsSupplierConsolidated")]
        public PurRepPurchasesTotal purRepPurchaseReturnsSupplierConsolidated([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 3, 0))
            {
                PurRepPurchasesTotal tot = new PurRepPurchasesTotal();
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                General gg = new General();
                var details = (from a in db.PurPurchaseReturnsUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                               select new PurPurchasesUni
                               {
                                   Vendorname = a.Vendorname,
                                   Baseamt = a.Baseamt,
                                   Discount = a.Discount,
                                   Others = a.Others,
                                   Taxes = a.Taxes,
                                   TotalAmt = a.TotalAmt
                               }
                              ).ToList();

                tot.details = (from a in details.GroupBy(b => b.Vendorname)
                               select new PurPurchasesUni
                               {
                                   Vendorname = a.Key,
                                   Baseamt = a.Sum(b => b.Baseamt),
                                   Discount = a.Sum(b => b.Discount),
                                   Others = a.Sum(b => b.Others),
                                   Taxes = a.Sum(b => b.Taxes),
                                   TotalAmt = a.Sum(b => b.TotalAmt)
                               }).OrderBy(c => c.Seq).ToList();


                var bas = tot.details.Sum(a => a.Baseamt);
                var dis = tot.details.Sum(b => b.Discount);
                var oth = tot.details.Sum(b => b.Others);
                var tax = tot.details.Sum(b => b.Taxes);
                var tamt = tot.details.Sum(b => b.TotalAmt);

                tot.details.Add(new PurPurchasesUni
                {
                    Vendorname = "",

                });
                tot.details.Add(new PurPurchasesUni
                {
                    Vendorname = "Total",
                    Baseamt = bas,
                    Discount = dis,
                    Others = oth,
                    Taxes = tax,
                    TotalAmt = tamt
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
                    dt.Rows.Add(i <= tot.details.Count() - 2 ? i.ToString() : "", det.Vendorname, det.Baseamt == null ? "" : gg.makeCur((double)det.Baseamt, 2), det.Discount == null ? "" : gg.makeCur((double)det.Discount, 2), det.Others == null ? "" : gg.makeCur((double)det.Others, 2), det.Taxes == null ? "" : gg.makeCur((double)det.Taxes, 2), det.TotalAmt == null ? "" : gg.makeCur((double)det.TotalAmt, 2));
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Supplier");
                titles.Add("Base");
                titles.Add("Discount");
                titles.Add("Others");
                titles.Add("Taxes");
                titles.Add("Total");

                float[] widths = { 30f, 170f, 70f, 70f, 70f, 70f, 70f };
                int[] aligns = { 3, 1, 2, 2, 2, 2, 2 };
                PDFExcelMake ep = new PDFExcelMake();

                AdminControl ac = new AdminControl();
                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfConversion(filename, "Supplier wise Consolidated Purchase Returns from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Supplier wise Consolidated Purchase Returns from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
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
        [Route("api/PurRepPurchases/PurRepPurchasesToReturns")]
        public PurRepPurchasesTotal PurRepPurchasesToReturns([FromBody] GeneralInformation inf)
        {

            AdminControl ac = new AdminControl();
            if (ac.screenCheck(inf.usr, 2, 11, 3, 0))
            {
                string dat1 = inf.frmDate;
                string dat2 = ac.strDate(DateTime.Parse(inf.toDate).AddDays(1));
                DataBaseContext g = new DataBaseContext();
                string quer = "";
                quer = quer + " select * from ";
                quer = quer + " (select  b.partyname, dbo.makCur(purchaseAmt) purAmt, dbo.makCur(Returnamt) retamt, Returnper, 1 sno from";
                quer = quer + " (select vendorid, purchaseAmt, ReturnAmt, round(case when purchaseAmt = 0 and ReturnAmt = 0 then 0 else";
                quer = quer + " case when purchaseAmt = 0 and ReturnAmt > 0 then 100 else ReturnAmt / PurchaseAmt * 100 end end,2) ReturnPer from";
                quer = quer + " (select case when a.vendorid is null then b.vendorid else a.vendorid end vendorid,";
                quer = quer + " case when purchaseAmt is null then 0 else purchaseAmt end purchaseamt,";
                quer = quer + " case when ReturnAmt is null then 0 else ReturnAmt end ReturnAmt from";
                quer = quer + " (select vendorid, sum(totalAmt) purchaseAmt from purpurchasesuni where dat >= '" + dat1 + "' and dat< '" + dat2 + "' and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + "";
                quer = quer + " group by vendorid)a full outer join";
                quer = quer + " (select vendorid, sum(totalAmt) ReturnAmt from purPurchaseReturnsUni where dat >= '" + dat1 + "' and dat< '" + dat2 + "' and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + "";
                quer = quer + " group by vendorid)b on a.vendorid = b.vendorid)x)a,";
                quer = quer + " (select * from partyDetails where customercode = " + inf.usr.cCode + ")b where a.vendorid = b.recordId";
                quer = quer + " union all select ' ' partyname,' ' puramt,' ' retamt,null returnPer,2 sno";
                quer = quer + " union all";
                quer = quer + " select 'Total' partyname,dbo.makCur(purchaseAmt) purAmt,dbo.makCur(ReturnAmt) retamt,";
                quer = quer + " case when purchaseAmt = 0 and ReturnAmt = 0 then 0 else";
                quer = quer + " case when purchaseAmt = 0 and ReturnAmt> 0 then 100 else";
                quer = quer + " round(ReturnAmt / purchaseAmt * 100, 2) end end Returnper, 3 sno from";
                quer = quer + " (select purchaseAmt, ReturnAmt from";
                quer = quer + " (select  sum(totalAmt) purchaseAmt from purpurchasesuni where dat >= '" + dat1 + "' and dat < '" + dat2 + "' and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
                quer = quer + " (select  sum(totalAmt) ReturnAmt from purPurchaseReturnsUni where dat >= '" + dat1 + "' and dat < '" + dat2 + "' and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b)x)x";
                quer = quer + " order by sno asc,returnper desc";

                PurRepPurchasesTotal tot = new PurRepPurchasesTotal();
                tot.details = new List<PurPurchasesUni>();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();
                while (dr.Read())
                {
                    tot.details.Add(new PurPurchasesUni
                    {
                        Vendorname = dr[0].ToString(),
                        Zip = dr[1].ToString(),
                        Mobile = dr[2].ToString(),
                        Tel = dr[3].ToString()
                    });
                }
                dr.Close();
                g.db.Close();


                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("vendor", typeof(string));
                dt.Columns.Add("puramt", typeof(string));
                dt.Columns.Add("retamt", typeof(string));
                dt.Columns.Add("retper", typeof(string));
                int i = 1;


                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i <= tot.details.Count() - 2 ? i.ToString() : "", det.Vendorname, det.Zip, det.Mobile, det.Tel);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Supplier");
                titles.Add("Purchase");
                titles.Add("Return");
                titles.Add("Return%");


                float[] widths = { 30f, 250f, 90f, 90f, 90f };
                int[] aligns = { 3, 1, 2, 2, 2 };
                PDFExcelMake ep = new PDFExcelMake();

                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfConversion(filename, "Purchase to Return Comparison from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Purchase to Return Comparison from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
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
        [Route("api/PurRepPurchases/PurRepListOfCreditNotes")]
        public PurCreditNotesTotal PurRepListOfCreditNotes([FromBody] GeneralInformation inf)
        {

            AdminControl ac = new AdminControl();
            if (ac.screenCheck(inf.usr, 2, 11, 3, 0))
            {
                try
                {
                    DateTime dat1 = DateTime.Parse(inf.frmDate);
                    DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                    var details = (from a in db.PartyCreditDebitNotes.Where(a => a.TransactionType == "PCR" && a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                   join b in db.PurPurchasesUni.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.TransactionId equals b.RecordId
                                   select new PartyCreditDebitNotes
                                   {
                                       Seq = a.Seq,
                                       BranchId = b.Vendorname,
                                       Dat = a.Dat,
                                       Amt = a.Amt
                                   }).OrderBy(b => b.Seq).ToList();
                    PurCreditNotesTotal tot = new PurCreditNotesTotal();
                    tot.details = new List<PartyCreditDebitNotes>();
                    if (details.Count() > 0)
                    {
                        tot.details.AddRange(details);
                    }
                    tot.details.Add(new PartyCreditDebitNotes
                    {
                        Seq = "",
                        BranchId = "",
                    });

                    tot.details.Add(new PartyCreditDebitNotes
                    {
                        Seq = "",
                        BranchId = "Total",
                        Amt = details.Sum(b => b.Amt)
                    });

                    DataTable dt = new DataTable();
                    dt.Columns.Add("sno", typeof(string));
                    dt.Columns.Add("seq", typeof(string));
                    dt.Columns.Add("vendor", typeof(string));
                    dt.Columns.Add("dat", typeof(string));
                    dt.Columns.Add("amt", typeof(string));
                    int i = 1;

                    General gg = new General();
                    foreach (var det in tot.details)
                    {
                        dt.Rows.Add(i <= tot.details.Count() - 2 ? i.ToString() : "", det.Seq, det.BranchId, det.Dat == null ? " " : ac.strDate(det.Dat.Value), det.Amt == null ? " " : gg.makeCur((double)det.Amt.Value, 2));
                        i++;
                    }

                    List<string> titles = new List<string>();
                    titles.Add("#");
                    titles.Add("Seq");
                    titles.Add("Supplier");
                    titles.Add("Date");
                    titles.Add("Amount");


                    float[] widths = { 30f, 90f, 250f, 90f, 90f };
                    int[] aligns = { 3, 1, 1, 1, 2 };
                    PDFExcelMake ep = new PDFExcelMake();

                    DateTime dats = DateTime.Now;
                    string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                    string fname = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                    string filename = ho.WebRootPath + "\\Reps\\" + fname;
                    string msg = "";

                    msg = ep.pdfConversion(filename, "List of Credit Notes from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
                    if (msg == "OK")
                    {
                        tot.pdfFile = fname;
                    }
                    string fname1 = inf.usr.uCode + "PurRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                    string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                    msg = "";
                    msg = ep.makeExcelConversion(filename1, "List of Credit Notes from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
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
    }
}
