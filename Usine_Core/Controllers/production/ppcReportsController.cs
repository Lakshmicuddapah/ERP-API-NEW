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

namespace Usine_Core.Controllers.production
{
    public class PPCReportDetails
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
        public double? val6 { get; set; }
    }
    public class PPCReportDetailsTotal
    {
        public string pdfFile { get; set; }
        public string excelFile { get; set; }
        public List<PPCReportDetails> details { get; set; }
    }
    public class ppcReportsController : ControllerBase
    {
        private readonly IHostingEnvironment ho;
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        public ppcReportsController(IHostingEnvironment hostingEnvironment)
        {
            ho = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        [Route("api/ppcReports/ppcRepMassAchievements")]
        public PPCReportDetailsTotal ppcRepMassAchievements([FromBody] UserInfo usr)
        {
            if(ac.screenCheck(usr,10,9,1,0))
            {
                var rec = db.PpcMassPlanningUni.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.RecordId);
                if(rec != null)
                {
                    var header = db.PpcMassPlanningUni.Where(a => a.RecordId == rec && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).FirstOrDefault();
                    var lines = db.PpcMassPlanningDet.Where(a => a.RecordId == rec && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
                    var batches = (from a in db.PpcBatchPlanningUni.Where(a => a.Dat >= header.FromDate && a.Dat <= header.ToDate && a.Pos==0&& a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).GroupBy(b => b.ItemId)
                                   select new
                                   {
                                       itemid = a.Key,
                                       qty = a.Sum(b => b.Qty)
                                   }).ToList();

                    var details = (from a in lines
                                   join b in batches on a.ItemId equals b.itemid into gj
                                   from subdet in gj.DefaultIfEmpty()
                                   select new
                                   {
                                       itemid = a.ItemId,
                                       plannedQty = a.Qty,
                                       clearedQty = subdet == null ? 0 : subdet.qty
                                   }).ToList();

                    var itemdetails = db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode).ToList();
                    PPCReportDetailsTotal tot = new PPCReportDetailsTotal();
                    tot.details = (from a in details
                                   join b in itemdetails on a.itemid equals b.RecordId
                                   select new PPCReportDetails
                                   {
                                       det1 = b.Itemname,
                                       det2 = b.Grpname,
                                       det3 = a.plannedQty.ToString(),
                                       det4 = a.clearedQty.ToString(),
                                       det5 = (a.plannedQty > a.clearedQty ? a.plannedQty - a.clearedQty : 0).ToString(),
                                       det6 = b.Um

                                   }).OrderBy(a => a.det1).ToList();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("sno", typeof(string));
                    dt.Columns.Add("itemname", typeof(string));
                    dt.Columns.Add("grp", typeof(string));
                    dt.Columns.Add("planned", typeof(string));
                    dt.Columns.Add("cleared", typeof(string));
                    dt.Columns.Add("pending", typeof(string));
                    dt.Columns.Add("um", typeof(string));
                 
                    int i = 1;
                    General gg = new General();
                    foreach (var det in tot.details)
                    {
                        dt.Rows.Add(i.ToString(), det.det1, det.det2, det.det3, det.det4,det.det5, det.det6);
                        i++;
                    }

                    List<string> titles = new List<string>();
                    titles.Add("#");
                    titles.Add("Item");
                    titles.Add("Group");
                    titles.Add("Planned");
                    titles.Add("Cleared");
                    titles.Add("Pending");
                    titles.Add("UM");
                    

                    float[] widths = { 40f, 140f, 110f, 70f,70f,70f,60f };
                    int[] aligns = { 3, 1, 1, 2, 2, 2, 2};
                    PDFExcelMake ep = new PDFExcelMake();
                    DateTime dats = DateTime.Now;
                    string title = "Pending production details as per estimation at " + gg.strDateTime(dats);
                  
                    string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                    string fname =  usr.uCode + "ppcRep" + dat + usr.cCode + usr.bCode + ".pdf";
                    string filename = ho.WebRootPath + "\\Reps\\" + fname;
                    string msg = "";

                    msg = ep.pdfLandscapeConversion(filename, title, usr, dt, titles, widths, aligns, false);
                    if (msg == "OK")
                    {
                        tot.pdfFile = fname;
                    }
                    string fname1 = usr.uCode + "ppcRep" + dat + usr.cCode + usr.bCode + ".xlsx";
                    string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                    msg = "";
                    msg = ep.makeExcelConversion(filename1, title, usr, dt, titles, widths, aligns, false);
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
            else
            {
                return null;
            }
        }



        [HttpPost]
        [Authorize]
        [Route("api/ppcReports/ppcRepMaterialRequirements")]
        public PPCReportDetailsTotal ppcRepMaterialRequirements([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 10, 9, 1, 0))
            {
                var rec = db.PpcMassPlanningUni.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.RecordId);
                if (rec != null)
                {
                    var header = db.PpcMassPlanningUni.Where(a => a.RecordId == rec && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).FirstOrDefault();
                    var lines = db.PpcMassPlanningDet.Where(a => a.RecordId == rec && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
                    var batches = (from a in db.PpcBatchPlanningUni.Where(a => a.Dat >= header.FromDate && a.Dat <= header.ToDate && a.Pos == 0 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).GroupBy(b => b.ItemId)
                                   select new
                                   {
                                       itemid = a.Key,
                                       qty = a.Sum(b => b.Qty)
                                   }).ToList();

                    var details = (from a in lines
                                   join b in batches on a.ItemId equals b.itemid into gj
                                   from subdet in gj.DefaultIfEmpty()
                                   select new
                                   {
                                       itemid = a.ItemId,
                                       plannedQty = a.Qty,
                                       pendingQty = a.Qty-(subdet == null ? 0 : subdet.qty)
                                   }).ToList();

                    var matdetails = db.ProItemWiseIngredients.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
                    var materials = (from a in details
                                     join b in matdetails on a.itemid equals b.ItemId
                                     select new
                                     {
                                         itemid = b.Ingredient,
                                         qty = a.pendingQty * b.Qty
                                     }).ToList();

                    var itemdetails = db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode).ToList();
                    PPCReportDetailsTotal tot = new PPCReportDetailsTotal();
                    tot.details = (from a in materials
                                   join b in itemdetails on a.itemid equals b.RecordId
                                   select new PPCReportDetails
                                   {
                                       det1 = b.Itemname,
                                       det2 = b.Grpname,
                                       det3 = a.qty.ToString(),
                                       det4 = b.Um

                                   }).OrderBy(a => a.det1).ToList();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("sno", typeof(string));
                    dt.Columns.Add("itemname", typeof(string));
                    dt.Columns.Add("grp", typeof(string));
                    dt.Columns.Add("qty", typeof(string));
                      dt.Columns.Add("um", typeof(string));

                    int i = 1;
                    General gg = new General();
                    foreach (var det in tot.details)
                    {
                        dt.Rows.Add(i.ToString(), det.det1, det.det2, det.det3, det.det4);
                        i++;
                    }

                    List<string> titles = new List<string>();
                    titles.Add("#");
                    titles.Add("Item");
                    titles.Add("Group");
                    titles.Add("Qty");
                    
                    titles.Add("UM");


                    float[] widths = { 40f, 240f, 140f,   70f, 60f };
                    int[] aligns = { 3, 1, 1, 2, 2};
                    PDFExcelMake ep = new PDFExcelMake();
                    DateTime dats = DateTime.Now;
                    string title = "Material Requirement details as per estimation at " + gg.strDateTime(dats);

                    string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                    string fname = usr.uCode + "ppcRep" + dat + usr.cCode + usr.bCode + ".pdf";
                    string filename = ho.WebRootPath + "\\Reps\\" + fname;
                    string msg = "";

                    msg = ep.pdfLandscapeConversion(filename, title, usr, dt, titles, widths, aligns, false);
                    if (msg == "OK")
                    {
                        tot.pdfFile = fname;
                    }
                    string fname1 = usr.uCode + "ppcRep" + dat + usr.cCode + usr.bCode + ".xlsx";
                    string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                    msg = "";
                    msg = ep.makeExcelConversion(filename1, title, usr, dt, titles, widths, aligns, false);
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
            else
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/ppcReports/ppcRepRunningBatchInfo")]
        public PPCReportDetailsTotal ppcRepRunningBatchInfo([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 10, 9, 1, 0))
            {
                PPCReportDetailsTotal tot = new PPCReportDetailsTotal();
                ProductionGeneral pg = new ProductionGeneral();
                var dets = pg.pendingProcessList(usr);
                tot.details = new List<PPCReportDetails>();
                foreach(var de in dets)
                {
                    tot.details.Add(new PPCReportDetails
                    {
                        det1=de.batchno,
                        det2=de.itemname,
                        det3=de.qty.ToString(),
                        det4=de.um,
                        det5=de.process
                    });
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("batch", typeof(string));
                dt.Columns.Add("item", typeof(string));
                dt.Columns.Add("qty", typeof(string));
                dt.Columns.Add("um", typeof(string));
                dt.Columns.Add("process", typeof(string));
                int i = 1;
                General gg = new General();
                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i.ToString(), det.det1, det.det2, det.det3, det.det4,det.det5);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Batch");
                titles.Add("Item");
               
                titles.Add("Qty");

                titles.Add("UM");
                titles.Add("Process");

                float[] widths = { 40f, 120f, 140f, 70f, 60f,120f };
                int[] aligns = { 3, 1, 1, 2, 3,1 };
                PDFExcelMake ep = new PDFExcelMake();
                DateTime dats = DateTime.Now;
                string title = "Production processes details at " + gg.strDateTime(dats);

                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = usr.uCode + "ppcRep" + dat + usr.cCode + usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfLandscapeConversion(filename, title, usr, dt, titles, widths, aligns, false);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = usr.uCode + "ppcRep" + dat + usr.cCode + usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, title, usr, dt, titles, widths, aligns, false);
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
        [Route("api/ppcReports/ppcRepRunningBatchMaterialDetails")]
        public PPCReportDetailsTotal ppcRepRunningBatchMaterialDetails([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 10, 9, 1, 0))
            {
                General gg = new General();
                PPCReportDetailsTotal tot = new PPCReportDetailsTotal();
                ProductionGeneral pg = new ProductionGeneral();
                var dets = pg.GetMaterialsUsedForPresentBatches(usr);
                tot.details = new List<PPCReportDetails>();

                foreach (var de in dets)
                {
                    tot.details.Add(new PPCReportDetails
                    {
                        det1 = de.materialname,
                        det2 = de.grpname,
                        det3 = de.qty.ToString(),
                        det4 = de.um,
                        det5 = gg.makeCur( (double)de.valu,2)
                    });
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
               dt.Columns.Add("item", typeof(string));
                dt.Columns.Add("grp", typeof(string));

                dt.Columns.Add("qty", typeof(string));
                dt.Columns.Add("um", typeof(string));
                dt.Columns.Add("valu", typeof(string));
                int i = 1;
               
                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i.ToString(), det.det1, det.det2, det.det3, det.det4, det.det5);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
               
                titles.Add("Item");
                titles.Add("Group");
                titles.Add("Qty");

                titles.Add("UM");
                titles.Add("Value");

                float[] widths = { 40f,  160f, 130f, 70f, 60f, 90f };
                int[] aligns = { 3, 1, 1, 2, 2, 2 };
                PDFExcelMake ep = new PDFExcelMake();
                DateTime dats = DateTime.Now;
                string title = "Material Used in Current Production at " + gg.strDateTime(dats);

                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = usr.uCode + "ppcRep" + dat + usr.cCode + usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfLandscapeConversion(filename, title, usr, dt, titles, widths, aligns, false);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = usr.uCode + "ppcRep" + dat + usr.cCode + usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, title, usr, dt, titles, widths, aligns, false);
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
        [Route("api/ppcReports/ppcRepMaterialWastageDetails")]
        public PPCReportDetailsTotal ppcRepMaterialWastageDetails([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 10, 9, 1, 0))
            {
                PPCReportDetailsTotal tot = new PPCReportDetailsTotal();
                DataBaseContext g = new DataBaseContext();
                General gg = new General();
                string dat1 = inf.frmDate;
                string dat2 = gg.strDate(DateTime.Parse(inf.toDate).AddDays(1));
               
                string quer = "";
                quer = quer + " select a.itemid,b.itemname,b.grpname, a.actualrequired,a.consumed,";
                quer = quer + " case when consumed >= actualrequired then consumed-actualrequired else 0 end wastage,";
                quer = quer + " case when consumed <= actualrequired then actualrequired-consumed else 0 end saving,";
                quer = quer + " b.um from";
                quer = quer + " (select case when a.itemname is null then b.ingredient else a.itemname end itemid,";
                quer = quer + " case when consumed is null then 0 else consumed end consumed,";
                quer = quer + " case when qty is null then 0 else qty end actualrequired from";
                quer = quer + " (select b.itemName, b.qtyout consumed from";
                quer = quer + " (select* from invMaterialManagement where transactionType = 3 and dat >= '" + dat1 + "'";
                quer = quer + " and dat< '" + dat2 + "' and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
                quer = quer + " (select * from invMaterialManagement where transactionType = 103 and branchid = '" + inf.usr.bCode + "'";
                quer = quer + " and customerCode = " + inf.usr.cCode + ")b where a.transactionId = b.productBatchNo)a full outer join";
                quer = quer + " (select b.ingredient, b.qty* a.qtyin qty from";
                quer = quer + " (select* from invMaterialManagement where transactionType = 3 and dat >= '" + dat1 + "'";
                quer = quer + " and dat< '" + dat2 + "' and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
                quer = quer + " (select * from proItemWiseIngredients where branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b where a.itemName = b.itemid)b";
                quer = quer + " on a.itemName = b.ingredient)a,";
                quer = quer + " (select * from invMaterialCompleteDetails_view where customerCode = " + inf.usr.cCode + " )b where a.itemid = b.recordId";

                tot.details = new List<PPCReportDetails>();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();
                int sno = 1;
                while(dr.Read())
                {
                    tot.details.Add(new PPCReportDetails
                    {

                        det1=dr[1].ToString(),
                        det2=dr[2].ToString(),
                        det3=dr[3].ToString(),
                        det4=dr[4].ToString(),
                        det5=dr[5].ToString(),
                        det6=dr[6].ToString(),
                        det7=dr[7].ToString(),
                      
                    });
                }
                dr.Close();
                g.db.Close();


                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("item", typeof(string));
                dt.Columns.Add("grp", typeof(string));

                dt.Columns.Add("reqd", typeof(string));
                dt.Columns.Add("consumed", typeof(string));
                dt.Columns.Add("wasted", typeof(string));
                dt.Columns.Add("saved", typeof(string));
                dt.Columns.Add("um", typeof(string));
                int i = 1;

                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i.ToString(), det.det1, det.det2, det.det3, det.det4, det.det5, det.det6, det.det7);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");

                titles.Add("Item");
                titles.Add("Group");
                titles.Add("Required");
                titles.Add("Consumed");
                titles.Add("Wasted");
                titles.Add("Saved");
                titles.Add("UM");
             
                float[] widths = { 40f, 190f, 150f, 70f, 70f, 70f,70f,60f };
                int[] aligns = { 3, 1, 1, 2, 2, 2,2,2 };
                PDFExcelMake ep = new PDFExcelMake();
                DateTime dats = DateTime.Now;
                string title = "Material Wastage/Savings in production from" + inf.frmDate + " to " + inf.toDate;

                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "ppcRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfLandscapeConversion(filename, title, inf.usr, dt, titles, widths, aligns, false);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "ppcRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, title, inf.usr, dt, titles, widths, aligns, false);
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
