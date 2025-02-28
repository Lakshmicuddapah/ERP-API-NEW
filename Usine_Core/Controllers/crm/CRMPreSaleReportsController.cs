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

namespace Usine_Core.Controllers.crm
{
    public class CRMRepDetails
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
    public class CRMReportDetailsTotal
    {
        public string pdfFile { get; set; }
        public string excelFile { get; set; }
        public List<CRMRepDetails> details { get; set; }
    }
    public class CRMPreSaleReportsController : ControllerBase
    {
        private readonly IHostingEnvironment ho;
        public CRMPreSaleReportsController(IHostingEnvironment hostingEnvironment)
        {

            ho = hostingEnvironment;
        }

        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/CRMPreSaleReports/GetCRMListofTeleCalls")]
        public CRMReportDetailsTotal GetCRMListofTeleCalls([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 7, 9, 2, 0))
            {
                try
                {
                    DateTime dat1 = DateTime.Parse(inf.frmDate);
                    DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                    General gg = new General();
                    CRMReportDetailsTotal tot = new CRMReportDetailsTotal();
                    tot.details = (from a in db.CrmTeleCallingRx.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                   join b in db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.Callerid equals b.RecordId into gj
                                   from subdet in gj.DefaultIfEmpty()
                                   select new CRMRepDetails
                                   {
                                       det1 = a.Seq,
                                       det2 = gg.strDate(a.Dat.Value),
                                       det3 = subdet==null?a.Username: subdet.Empname,
                                       det4 = a.Customer,
                                       det5 = a.Mobile,
                                       det6 = a.CustomerComments,
                                       det7 = a.CallPosition == 1 ? "Hot" : (a.CallPosition == 2 ? "Warm" : (a.CallPosition == 3 ? "Cool" : "Declined"))
                                   }
                                 ).OrderBy(b => b.det1).ToList();

                    if(tot.details.Count() > 0)
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));
                        dt.Columns.Add("seq", typeof(string));
                        dt.Columns.Add("dat", typeof(string));
                        dt.Columns.Add("caller", typeof(string));
                        dt.Columns.Add("customer", typeof(string));
                        dt.Columns.Add("mobile", typeof(string));
                        dt.Columns.Add("comments", typeof(string));
                        dt.Columns.Add("pos", typeof(string));

                        int i = 1;

                        foreach (var det in tot.details)
                        {
                            dt.Rows.Add(i.ToString(), det.det1, det.det2,det.det3,det.det4,det.det5,det.det6,det.det7);
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Seq");
                        titles.Add("Date");
                        titles.Add("Caller");
                        titles.Add("Customer");
                        titles.Add("Mobile");
                        titles.Add("Comments");
                        titles.Add("Status");



                        float[] widths = { 40f,70f,90f,90f, 100f,100f, 150f,90f };
                        int[] aligns = { 3, 1, 1, 1,1,1,1,1 };
                        PDFExcelMake ep = new PDFExcelMake();

                        AdminControl ac = new AdminControl();
                        DateTime dats = DateTime.Now;
                        string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname = inf.usr.uCode + "CRMPreReps" + dat + inf.usr.cCode + inf.usr.bCode;
                        string filename = ho.WebRootPath + "\\Reps\\" + fname + ".pdf"; 
                        string msg = "";
                        string title = "List of Tele Calls from " + inf.frmDate + " to " + inf.toDate;
                        msg = ep.pdfConversion(filename, title, inf.usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname + ".pdf";
                        }
                          string filename1 = ho.WebRootPath + "\\Reps\\" + fname +".xlsx"; 
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, title, inf.usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.excelFile = fname + ".xlsx";
                        }
                        
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
        [Route("api/CRMPreSaleReports/GetCRMPendingTeleCalls")]
        public CRMReportDetailsTotal GetCRMPendingTeleCalls([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 7, 9, 2, 0))
            {
                try
                {
                    CRMReportDetailsTotal tot = new CRMReportDetailsTotal();
                    tot.details = new List<CRMRepDetails>();
                    DateTime dat = ac.getPresentDateTime();
                    var det1 = db.CrmTeleCallingRx.Where(a => a.NextCallDate <= dat && a.NextCallMode == "1" && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
                    var det2 = db.CrmEnquiriesRx.Where(a => a.NextCallDate <= dat && a.NextCallMode == 1 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
                    foreach (CrmTeleCallingRx det in det1)
                    {
                        tot.details.Add(new CRMRepDetails
                        {
                            det1 = det.Customer,
                            det2 = det.Mobile,
                            det4 = "TeleCall",
                            det3 = ac.strDate(det.NextCallDate.Value),
                            det5 = det.Username,
                            det6 = det.CustomerComments
                        });
                    }
                    foreach (CrmEnquiriesRx det in det2)
                    {
                        tot.details.Add(new CRMRepDetails
                        {
                            det1 = det.Customer,
                            det2 = det.Mobile,
                            det4 = "Enquiry",
                            det3 = ac.strDate(det.NextCallDate.Value),
                            det5 = det.username,
                            det6 = det.CustomerComments
                        });
                    }
                    if (tot.details.Count() > 0)
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));
                        dt.Columns.Add("customer", typeof(string));
                        dt.Columns.Add("mobile", typeof(string));
                        dt.Columns.Add("dat", typeof(string));
                        dt.Columns.Add("Prev Mode", typeof(string));
                        dt.Columns.Add("User", typeof(string));
                        

                        int i = 1;

                        foreach (var det in tot.details)
                        {
                            dt.Rows.Add(i.ToString(), det.det1, det.det2, det.det3, det.det4, det.det5);
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Customer");
                        titles.Add("Mobile");
                        titles.Add("Scheduled");
                        titles.Add("Prev Mode");
                        titles.Add("User");
                     


                        float[] widths = { 40f, 150f,100f,80f,80f,100f };
                        int[] aligns = { 3, 1, 1, 1, 1, 1  };
                        PDFExcelMake ep = new PDFExcelMake();

                        AdminControl ac = new AdminControl();
                        DateTime dats = DateTime.Now;
                        string no = ac.strDate(dats);
                        string datss = no + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname = usr.uCode + "CRMPreReps" + datss +  usr.cCode +  usr.bCode;
                        string filename = ho.WebRootPath + "\\Reps\\" + fname + ".pdf";
                        string msg = "";
                        string title = "List of Pending Tele Calls as of " + no;
                        msg = ep.pdfConversion(filename, title, usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname + ".pdf";
                        }
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname + ".xlsx";
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, title, usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.excelFile = fname + ".xlsx";
                        }

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
        [Route("api/CRMPreSaleReports/GetCRMListofEnquiries")]
        public CRMReportDetailsTotal GetCRMListofEnquiries([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 7, 9, 2, 0))
            {
                try
                {
                    DateTime dat1 = DateTime.Parse(inf.frmDate);
                    DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                    General gg = new General();
                    CRMReportDetailsTotal tot = new CRMReportDetailsTotal();
                    tot.details = (from a in db.CrmEnquiriesRx.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                   join b in db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.Callerid equals b.RecordId into gj
                                   from subdet in gj.DefaultIfEmpty()
                                   select new CRMRepDetails
                                   {
                                       det1 = a.Seq,
                                       det2 = gg.strDate(a.Dat.Value),
                                       det3 = subdet == null ? a.username : subdet.Empname,
                                       det4 = a.Customer,
                                       det5 = a.Mobile,
                                       det6 = a.CustomerComments,
                                       det7 = a.CallPosition == 1 ? "Hot" : (a.CallPosition == 2 ? "Warm" : (a.CallPosition == 3 ? "Cool" : "Declined"))
                                   }
                                 ).OrderBy(b => b.det1).ToList();

                    if (tot.details.Count() > 0)
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));
                        dt.Columns.Add("seq", typeof(string));
                        dt.Columns.Add("dat", typeof(string));
                        dt.Columns.Add("caller", typeof(string));
                        dt.Columns.Add("customer", typeof(string));
                        dt.Columns.Add("mobile", typeof(string));
                        dt.Columns.Add("comments", typeof(string));
                        dt.Columns.Add("pos", typeof(string));

                        int i = 1;

                        foreach (var det in tot.details)
                        {
                            dt.Rows.Add(i.ToString(), det.det1, det.det2, det.det3, det.det4, det.det5, det.det6, det.det7);
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Seq");
                        titles.Add("Date");
                        titles.Add("Caller");
                        titles.Add("Customer");
                        titles.Add("Mobile");
                        titles.Add("Comments");
                        titles.Add("Pos");



                        float[] widths = { 40f, 70f, 90f, 90f, 100f, 100f, 150f, 90f };
                        int[] aligns = { 3, 1, 1, 1, 1, 1, 1, 1 };
                        PDFExcelMake ep = new PDFExcelMake();

                        AdminControl ac = new AdminControl();
                        DateTime dats = DateTime.Now;
                        string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname = inf.usr.uCode + "CRMPreReps" + dat + inf.usr.cCode + inf.usr.bCode;
                        string filename = ho.WebRootPath + "\\Reps\\" + fname + ".pdf";
                        string msg = "";
                        string title = "List of Enquiries from " + inf.frmDate + " to " + inf.toDate;
                        msg = ep.pdfConversion(filename, title, inf.usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname + ".pdf";
                        }
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname + ".xlsx";
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, title, inf.usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.excelFile = fname + ".xlsx";
                        }

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
        [Route("api/CRMPreSaleReports/GetCRMPendingEnquiries")]
        public CRMReportDetailsTotal GetCRMPendingEnquiries([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 7, 9, 2, 0))
            {
                try
                {
                    CRMReportDetailsTotal tot = new CRMReportDetailsTotal();
                    tot.details = new List<CRMRepDetails>();
                    DateTime dat = ac.getPresentDateTime();
                    var det1 = db.CrmTeleCallingRx.Where(a => a.NextCallDate <= dat && a.NextCallMode == "2" && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
                    var det2 = db.CrmEnquiriesRx.Where(a => a.NextCallDate <= dat && a.NextCallMode == 2 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
                    foreach (CrmTeleCallingRx det in det1)
                    {
                        tot.details.Add(new CRMRepDetails
                        {
                            det1 = det.Customer,
                            det2 = det.Mobile,
                            det4 = "TeleCall",
                            det3 = ac.strDate(det.NextCallDate.Value),
                            det5 = det.Username,
                            det6 = det.CustomerComments
                        });
                    }
                    foreach (CrmEnquiriesRx det in det2)
                    {
                        tot.details.Add(new CRMRepDetails
                        {
                            det1 = det.Customer,
                            det2 = det.Mobile,
                            det4 = "Enquiry",
                            det3 = ac.strDate(det.NextCallDate.Value),
                            det5 = det.username,
                            det6 = det.CustomerComments
                        });
                    }
                    if (tot.details.Count() > 0)
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));
                        dt.Columns.Add("customer", typeof(string));
                        dt.Columns.Add("mobile", typeof(string));
                        dt.Columns.Add("dat", typeof(string));
                        dt.Columns.Add("Prev Mode", typeof(string));
                        dt.Columns.Add("User", typeof(string));


                        int i = 1;

                        foreach (var det in tot.details)
                        {
                            dt.Rows.Add(i.ToString(), det.det1, det.det2, det.det3, det.det4, det.det5);
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Customer");
                        titles.Add("Mobile");
                        titles.Add("Scheduled");
                        titles.Add("Prev Mode");
                        titles.Add("User");



                        float[] widths = { 40f, 150f, 100f, 80f, 80f, 100f };
                        int[] aligns = { 3, 1, 1, 1, 1, 1 };
                        PDFExcelMake ep = new PDFExcelMake();

                        AdminControl ac = new AdminControl();
                        DateTime dats = DateTime.Now;
                        string no = ac.strDate(dats);
                        string datss = no + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname = usr.uCode + "CRMPreReps" + datss + usr.cCode + usr.bCode;
                        string filename = ho.WebRootPath + "\\Reps\\" + fname + ".pdf";
                        string msg = "";
                        string title = "List of Pending Enquiries as of " + no;
                        msg = ep.pdfConversion(filename, title, usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname + ".pdf";
                        }
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname + ".xlsx";
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, title, usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.excelFile = fname + ".xlsx";
                        }

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
        [Route("api/CRMPreSaleReports/GetCRMCallerwiseCalls")]
        public CRMReportDetailsTotal GetCRMCallerwiseCalls([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 7, 9, 2, 0))
            {
                CRMReportDetailsTotal tot = new CRMReportDetailsTotal();
                tot.details = new List<CRMRepDetails>();
                string dat1 = inf.frmDate;

                string dat2 = ac.strDate(DateTime.Parse(inf.toDate).AddDays(1));
                string quer = "";
                quer = quer + " select case when a.username is null then b.username else a.username end username,";
                quer = quer + " case when a.cnt is null then 0 else a.cnt end telecalls,";
                quer = quer + " case when b.cnt is null then 0 else b.cnt end enquiries from";
                quer = quer + " (select username, count(*) cnt From CrmTeleCallingRx where dat >= '" + dat1 + "' and dat< '" + dat2 + "'";
                quer = quer + " and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + " group by username)a full outer join";
                quer = quer + " (select username, count(*) cnt From crmEnquiriesRX where dat >= '" + dat1 + "' and dat< '" + dat2 + "'";
                quer = quer + " and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + " group by username)b on a.username = b.username";
                DataBaseContext g = new DataBaseContext();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();
                while (dr.Read())
                {
                    tot.details.Add(new CRMRepDetails
                    {
                        det1 = dr[0].ToString(),
                        det2 = dr[1].ToString(),
                        det3 = dr[2].ToString()
                    });
                }
                dr.Close();
                g.db.Close();

                if (tot.details.Count() > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("sno", typeof(string));
                    dt.Columns.Add("user", typeof(string));
                    dt.Columns.Add("calls", typeof(string));
                    dt.Columns.Add("enquiries", typeof(string));
                 

                    int i = 1;

                    foreach (var det in tot.details)
                    {
                        dt.Rows.Add(i.ToString(), det.det1, det.det2, det.det3 );
                        i++;
                    }

                    List<string> titles = new List<string>();
                    titles.Add("#");
                    titles.Add("User");
                    titles.Add("Calls");
                    titles.Add("Enquiries");
                    



                    float[] widths = { 50f, 250f, 125f,125f};
                    int[] aligns = { 3, 1, 2, 2};
                    PDFExcelMake ep = new PDFExcelMake();

                    AdminControl ac = new AdminControl();
                    DateTime dats = DateTime.Now;
                    string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                    string fname = inf.usr.uCode + "CRMPreReps" + dat + inf.usr.cCode + inf.usr.bCode;
                    string filename = ho.WebRootPath + "\\Reps\\" + fname + ".pdf";
                    string msg = "";
                    string title = "User wise Calls from " + inf.frmDate + " to " + inf.toDate;
                    msg = ep.pdfConversion(filename, title, inf.usr, dt, titles, widths, aligns, false);
                    if (msg == "OK")
                    {
                        tot.pdfFile = fname + ".pdf";
                    }
                    string filename1 = ho.WebRootPath + "\\Reps\\" + fname + ".xlsx";
                    msg = "";
                    msg = ep.makeExcelConversion(filename1, title, inf.usr, dt, titles, widths, aligns, false);
                    if (msg == "OK")
                    {
                        tot.excelFile = fname + ".xlsx";
                    }

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
        [Route("api/CRMPreSaleReports/GetCRMListofSaleOrders")]
        public CRMReportDetailsTotal GetCRMListofSaleOrders([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 7, 9, 2, 0))
            {
                try
                {
                    DateTime dat1 = DateTime.Parse(inf.frmDate);
                    DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                    General gg = new General();
                    CRMReportDetailsTotal tot = new CRMReportDetailsTotal();
                    tot.details = (from a in db.CrmSaleOrderUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                     select new CRMRepDetails
                                   {
                                       det1 = a.Seq,
                                       det2 = gg.strDate(a.Dat.Value),
                                       det3 = a.PartyName,
                                       det4 = a.Mobile,
                                       det5 = gg.makeCur((double)a.Baseamt,2),
                                       det6 = gg.makeCur((double)a.Discount, 2),
                                         det7 = gg.makeCur((double)a.Taxes, 2),
                                         det8 = gg.makeCur((double)a.Others, 2),
                                         det9 = gg.makeCur((double)a.TotalAmt, 2),

                                         val1=a.Baseamt,
                                         val2=a.Discount,
                                         val3=a.Taxes,
                                         val4=a.Others,
                                         val5=a.TotalAmt
                                         
                                     }
                                 ).OrderBy(b => b.det1).ToList();



                    if (tot.details.Count() > 0)
                    {
                        var v1 = tot.details.Sum(a => a.val1);
                        var v2 = tot.details.Sum(a => a.val2);
                        var v3 = tot.details.Sum(a => a.val3);
                        var v4 = tot.details.Sum(a => a.val4);
                        var v5 = tot.details.Sum(a => a.val5);
                        tot.details.Add(new CRMRepDetails
                        {
                            det1 = "",
                            det2 = "",
                            det3 = "",
                            det4 = "",
                            det5 = "",
                            det6 = "",
                            det7 = "",
                            det8 = "",
                            det9 = ""
                        });
                        tot.details.Add(new CRMRepDetails
                        {
                            det1 = "",
                            det2 = "",
                            det3 = "Total",
                            det4 = "",
                            det5 = gg.makeCur((double)v1, 2),
                            det6 = gg.makeCur((double)v2, 2),
                            det7 = gg.makeCur((double)v3, 2),
                            det8 = gg.makeCur((double)v4, 2),
                            det9 = gg.makeCur((double)v5, 2),
                        });

                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));
                        dt.Columns.Add("so", typeof(string));
                        dt.Columns.Add("dat", typeof(string));
                         dt.Columns.Add("customer", typeof(string));
                        dt.Columns.Add("mobile", typeof(string));
                        dt.Columns.Add("base", typeof(string));
                        dt.Columns.Add("disc", typeof(string));
                        dt.Columns.Add("taxes", typeof(string));
                        dt.Columns.Add("others", typeof(string));
                        dt.Columns.Add("total", typeof(string));
                        int i = 1;

                        foreach (var det in tot.details)
                        {
                            dt.Rows.Add( i<tot.details.Count()-2? i.ToString():"", det.det1, det.det2, det.det3, det.det4, det.det5, det.det6, det.det7, det.det8, det.det9);
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Seq");
                        titles.Add("Date");
                           titles.Add("Customer");
                        titles.Add("Mobile");
                        titles.Add("Base");
                        titles.Add("Discount");
                        titles.Add("Taxes");
                        titles.Add("Others");
                        titles.Add("Total");


                        float[] widths = { 40f, 60f, 80f, 160f, 90f, 60f, 60f, 60f, 60f, 60f};
                        int[] aligns = { 3, 1, 1, 1, 1, 2, 2, 2, 2, 2 };
                        PDFExcelMake ep = new PDFExcelMake();

                        AdminControl ac = new AdminControl();
                        DateTime dats = DateTime.Now;
                        string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname = inf.usr.uCode + "CRMPreReps" + dat + inf.usr.cCode + inf.usr.bCode;
                        string filename = ho.WebRootPath + "\\Reps\\" + fname + ".pdf";
                        string msg = "";
                        string title = "List of Orders from " + inf.frmDate + " to " + inf.toDate;
                        msg = ep.pdfLandscapeConversion(filename, title, inf.usr, dt, titles, widths, aligns, true);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname + ".pdf";
                        }
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname + ".xlsx";
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, title, inf.usr, dt, titles, widths, aligns, true);
                        if (msg == "OK")
                        {
                            tot.excelFile = fname + ".xlsx";
                        }

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
        [Route("api/CRMPreSaleReports/GetCRMCustomerWiseSaleOrders")]
        public CRMReportDetailsTotal GetCRMCustomerWiseSaleOrders([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 7, 9, 2, 0))
            {
                try
                {
                    DateTime dat1 = DateTime.Parse(inf.frmDate);
                    DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                    General gg = new General();
                    CRMReportDetailsTotal tot = new CRMReportDetailsTotal();
                    var dets = db.CrmSaleOrderUni.Where(a => a.PartyId == inf.recordId && a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                    string customer = "";
                    if(dets.Count() > 0)
                    {
                        customer = dets.Select(b => b.PartyName).FirstOrDefault();
                    }

                    tot.details = (from a in dets
                                   select new CRMRepDetails
                                   {
                                       det1 = a.Seq,
                                       det2 = gg.strDate(a.Dat.Value),
                                       det3 = a.PartyName,
                                       det4 = a.Mobile,
                                       det5 = gg.makeCur((double)a.Baseamt, 2),
                                       det6 = gg.makeCur((double)a.Discount, 2),
                                       det7 = gg.makeCur((double)a.Taxes, 2),
                                       det8 = gg.makeCur((double)a.Others, 2),
                                       det9 = gg.makeCur((double)a.TotalAmt, 2),

                                       val1 = a.Baseamt,
                                       val2 = a.Discount,
                                       val3 = a.Taxes,
                                       val4 = a.Others,
                                       val5 = a.TotalAmt

                                   }
                                 ).OrderBy(b => b.det1).ToList();



                    if (tot.details.Count() > 0)
                    {
                        var v1 = tot.details.Sum(a => a.val1);
                        var v2 = tot.details.Sum(a => a.val2);
                        var v3 = tot.details.Sum(a => a.val3);
                        var v4 = tot.details.Sum(a => a.val4);
                        var v5 = tot.details.Sum(a => a.val5);
                        tot.details.Add(new CRMRepDetails
                        {
                            det1 = "",
                            det2 = "",
                            det3 = "",
                            det4 = "",
                            det5 = "",
                            det6 = "",
                            det7 = "",
                            det8 = "",
                            det9 = ""
                        });
                        tot.details.Add(new CRMRepDetails
                        {
                            det1 = "",
                            det2 = "",
                            det3 = "Total",
                            det4 = "",
                            det5 = gg.makeCur((double)v1, 2),
                            det6 = gg.makeCur((double)v2, 2),
                            det7 = gg.makeCur((double)v3, 2),
                            det8 = gg.makeCur((double)v4, 2),
                            det9 = gg.makeCur((double)v5, 2),
                        });

                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));
                        dt.Columns.Add("so", typeof(string));
                        dt.Columns.Add("dat", typeof(string));
                        dt.Columns.Add("customer", typeof(string));
                        dt.Columns.Add("mobile", typeof(string));
                        dt.Columns.Add("base", typeof(string));
                        dt.Columns.Add("disc", typeof(string));
                        dt.Columns.Add("taxes", typeof(string));
                        dt.Columns.Add("others", typeof(string));
                        dt.Columns.Add("total", typeof(string));
                        int i = 1;

                        foreach (var det in tot.details)
                        {
                            dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : "", det.det1, det.det2, det.det3, det.det4, det.det5, det.det6, det.det7, det.det8, det.det9);
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Seq");
                        titles.Add("Date");
                        titles.Add("Customer");
                        titles.Add("Mobile");
                        titles.Add("Base");
                        titles.Add("Discount");
                        titles.Add("Taxes");
                        titles.Add("Others");
                        titles.Add("Total");


                        float[] widths = { 40f, 60f, 80f, 160f, 90f, 60f, 60f, 60f, 60f, 60f };
                        int[] aligns = { 3, 1, 1, 1, 1, 2, 2, 2, 2, 2 };
                        PDFExcelMake ep = new PDFExcelMake();

                        AdminControl ac = new AdminControl();
                        DateTime dats = DateTime.Now;
                        string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname = inf.usr.uCode + "CRMPreReps" + dat + inf.usr.cCode + inf.usr.bCode;
                        string filename = ho.WebRootPath + "\\Reps\\" + fname + ".pdf";
                        string msg = "";
                        string title = "List of Orders of customer " + customer + " from " + inf.frmDate + " to " + inf.toDate;
                        msg = ep.pdfLandscapeConversion(filename, title, inf.usr, dt, titles, widths, aligns, true);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname + ".pdf";
                        }
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname + ".xlsx";
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, title, inf.usr, dt, titles, widths, aligns, true);
                        if (msg == "OK")
                        {
                            tot.excelFile = fname + ".xlsx";
                        }

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
        [Route("api/CRMPreSaleReports/GetCRMListofPendingSaleOrders")]
        public CRMReportDetailsTotal GetCRMListofPendingSaleOrders([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 7, 9, 2, 0))
            {
                try
                {
                   
                    General gg = new General();
                    CRMReportDetailsTotal tot = new CRMReportDetailsTotal();
                    tot.details = (from a in db.CrmSaleOrderUni.Where(a => a.Dat >= DateTime.Parse("1-Apr-24") && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                                   select new CRMRepDetails
                                   {
                                       det1 = a.Seq,
                                       det2 = gg.strDate(a.Dat.Value),
                                       det3 = a.PartyName,
                                       det4 = a.Mobile,
                                       det5 = gg.makeCur((double)a.Baseamt, 2),
                                       det6 = gg.makeCur((double)a.Discount, 2),
                                       det7 = gg.makeCur((double)a.Taxes, 2),
                                       det8 = gg.makeCur((double)a.Others, 2),
                                       det9 = gg.makeCur((double)a.TotalAmt, 2),

                                       val1 = a.Baseamt,
                                       val2 = a.Discount,
                                       val3 = a.Taxes,
                                       val4 = a.Others,
                                       val5 = a.TotalAmt

                                   }
                                 ).OrderBy(b => b.det1).ToList();



                    if (tot.details.Count() > 0)
                    {
                        var v1 = tot.details.Sum(a => a.val1);
                        var v2 = tot.details.Sum(a => a.val2);
                        var v3 = tot.details.Sum(a => a.val3);
                        var v4 = tot.details.Sum(a => a.val4);
                        var v5 = tot.details.Sum(a => a.val5);
                        tot.details.Add(new CRMRepDetails
                        {
                            det1 = "",
                            det2 = "",
                            det3 = "",
                            det4 = "",
                            det5 = "",
                            det6 = "",
                            det7 = "",
                            det8 = "",
                            det9 = ""
                        });
                        tot.details.Add(new CRMRepDetails
                        {
                            det1 = "",
                            det2 = "",
                            det3 = "Total",
                            det4 = "",
                            det5 = gg.makeCur((double)v1, 2),
                            det6 = gg.makeCur((double)v2, 2),
                            det7 = gg.makeCur((double)v3, 2),
                            det8 = gg.makeCur((double)v4, 2),
                            det9 = gg.makeCur((double)v5, 2),
                        });

                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));
                        dt.Columns.Add("so", typeof(string));
                        dt.Columns.Add("dat", typeof(string));
                        dt.Columns.Add("customer", typeof(string));
                        dt.Columns.Add("mobile", typeof(string));
                        dt.Columns.Add("base", typeof(string));
                        dt.Columns.Add("disc", typeof(string));
                        dt.Columns.Add("taxes", typeof(string));
                        dt.Columns.Add("others", typeof(string));
                        dt.Columns.Add("total", typeof(string));
                        int i = 1;

                        foreach (var det in tot.details)
                        {
                            dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : "", det.det1, det.det2, det.det3, det.det4, det.det5, det.det6, det.det7, det.det8, det.det9);
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Seq");
                        titles.Add("Date");
                        titles.Add("Customer");
                        titles.Add("Mobile");
                        titles.Add("Base");
                        titles.Add("Discount");
                        titles.Add("Taxes");
                        titles.Add("Others");
                        titles.Add("Total");


                        float[] widths = { 40f, 60f, 80f, 160f, 90f, 60f, 60f, 60f, 60f, 60f };
                        int[] aligns = { 3, 1, 1, 1, 1, 2, 2, 2, 2, 2 };
                        PDFExcelMake ep = new PDFExcelMake();

                        AdminControl ac = new AdminControl();
                        DateTime dats = DateTime.Now;
                        string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname = usr.uCode + "CRMPreReps" + dat + usr.cCode + usr.bCode;
                        string filename = ho.WebRootPath + "\\Reps\\" + fname + ".pdf";
                        string msg = "";
                        string title = "List of Pending Orders  as on " + ac.getPresentDateTime().ToLongDateString()  ;
                        msg = ep.pdfLandscapeConversion(filename, title, usr, dt, titles, widths, aligns, true);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname + ".pdf";
                        }
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname + ".xlsx";
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, title, usr, dt, titles, widths, aligns, true);
                        if (msg == "OK")
                        {
                            tot.excelFile = fname + ".xlsx";
                        }

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
        [Route("api/CRMPreSaleReports/GetCRMItemWisePendingSaleOrders")]
        public CRMReportDetailsTotal GetCRMItemWisePendingSaleOrders([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 7, 9, 2, 0))
            {
                try
                {

                    General gg = new General();
                    CRMReportDetailsTotal tot = new CRMReportDetailsTotal();
                    var lst1 = (from a in db.CrmSaleOrderDet.Where(a =>    a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).GroupBy(c => c.ItemName)
                                   select new CRMRepDetails
                                   {
                                       det1 = a.Key,
                                        val1=a.Sum(b => b.Qty),
                                       val2 = a.Sum(b => (b.Qty * b.Rat))
                                                       }
                                 ).OrderBy(b => b.det1).ToList();
                    var lst2 = db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode).ToList();

                    tot.details = (from a in lst1
                                   join b in lst2 on a.det1 equals b.Itemname
                                   select new CRMRepDetails
                                   {
                                       det1=a.det1,
                                       det5=b.Um,
                                       val1=a.val1,
                                       val2=a.val2
                                   }).OrderBy(b => b.det1).ToList();

                        if (tot.details.Count() > 0)
                    {
                        var v1 = tot.details.Sum(a => a.val2);
                      
                        tot.details.Add(new CRMRepDetails
                        {
                            det1 = "",
                            det2 = "",
                            det3 = "",
                            det5=""
                          
                        });
                        tot.details.Add(new CRMRepDetails
                        {
                            det1 = "Total",
                            det2 = "",
                            det3="",
                            det5="",
                           val2 =v1
                          
                        });
                        foreach(var de in tot.details)
                        {
                            de.det2 = de.val1 == null ? "" : de.val1.ToString();
                            de.det3 = de.val2 == null ? "" : gg.makeCur((double)de.val2, 2);
                        }
                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));
                        dt.Columns.Add("item", typeof(string));
                        dt.Columns.Add("qty", typeof(string));
                        dt.Columns.Add("um", typeof(string));
                        dt.Columns.Add("valu", typeof(string));
                        
                        int i = 1;

                        foreach (var det in tot.details)
                        {
                            dt.Rows.Add(i < tot.details.Count() - 2 ? i.ToString() : "", det.det1, det.det2, det.det5, det.det3);
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Item");
                        titles.Add("Qty");
                        titles.Add("UOM");
                        titles.Add("Value");
                     


                        float[] widths = { 50f, 250f, 80f,90f,90f};
                        int[] aligns = { 3, 1,   2, 3,2  };
                        PDFExcelMake ep = new PDFExcelMake();

                        AdminControl ac = new AdminControl();
                        DateTime dats = DateTime.Now;
                        string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname = usr.uCode + "CRMPreReps" + dat + usr.cCode + usr.bCode;
                        string filename = ho.WebRootPath + "\\Reps\\" + fname + ".pdf";
                        string msg = "";
                        string title = "Item wise Pending Orders  as on " + ac.getPresentDateTime().ToLongDateString();
                        msg = ep.pdfConversion(filename, title, usr, dt, titles, widths, aligns, true);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname + ".pdf";
                        }
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname + ".xlsx";
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, title, usr, dt, titles, widths, aligns, true);
                        if (msg == "OK")
                        {
                            tot.excelFile = fname + ".xlsx";
                        }

                    }

                    return tot;
                }
                catch(Exception ee)
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
        [Route("api/CRMPreSaleReports/crmRepOrderPendingAdvances")]
        public CRMReportDetailsTotal crmRepOrderPendingAdvances([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 7, 9, 2, 0))
            {
                try
                {
                    General gg = new General();
                    CRMReportDetailsTotal tot = new CRMReportDetailsTotal();

                    string quer = "";
                    quer = quer + " select seq,dats,vendorname,mobile,dbo.makcur(amt) amt,paymentmode,sno from  ";
                    quer = quer + " (select right(a.seq,4) seq,dbo.strDAte(a.dat) dats,b.partyName,b.mobile,amt,a.paymentmode,1 sno from";
                    quer = quer + " (select * from totAdvanceDetails where branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a,";
                    quer = quer + " (select * from crmSaleOrderUni where pos >= 1 and branchId = '" + usr.bCode + "' and customerCode =  " + usr.cCode + ")b where a.transactionid = b.recordId";

                    quer = quer + " union all";
                    quer = quer + " select ' ' seq,' ' dats,' ' vendorname,' ' mobile,0 amt,' ' paymentmode,2 sno";
                    quer = quer + " union all  select ' '  seq,' '  dats,'Total',' ' mobile, sum(a.amt) amt,' ' paymentmode,3 sno from";
                    quer = quer + " (select * from totAdvanceDetails where branchId= '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a,";
                    quer = quer + " (select * from crmSaleOrderUni where pos >= 1 and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b where a.transactionid = b.recordId)x order by sno, seq";




                    tot.details = new List<CRMRepDetails>();
                    DataBaseContext g = new DataBaseContext();
                    SqlCommand dc = new SqlCommand();
                    dc.Connection = g.db;
                    g.db.Open();
                    dc.CommandText = quer;
                    SqlDataReader dr = dc.ExecuteReader();
                    while (dr.Read())
                    {
                        tot.details.Add(new CRMRepDetails
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

                    if (tot.details.Count > 0)
                    {

                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));

                        dt.Columns.Add("voucher", typeof(string));
                        dt.Columns.Add("dat", typeof(string));
                        dt.Columns.Add("vendor", typeof(string));
                        dt.Columns.Add("mobile", typeof(string));
                        dt.Columns.Add("amt", typeof(string));
                        dt.Columns.Add("paymentmode", typeof(string));
                        int i = 1;

                        foreach (var det in tot.details)
                        {
                            dt.Rows.Add(i < tot.details.Count() - 1 ? i.ToString() : "", det.det1, det.det2, det.det3, det.det4, det.det5, det.det6);
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Voucher");
                        titles.Add("Date");
                        titles.Add("Customer");
                        titles.Add("Mobile");
                        titles.Add("Amt");
                        titles.Add("Mode");

                        float[] widths = { 40f, 50f, 60f, 160f, 100f, 80f, 70f };
                        int[] aligns = { 3, 1, 1, 1, 1, 2, 1 };
                        PDFExcelMake ep = new PDFExcelMake();

                        AdminControl ac = new AdminControl();
                        DateTime dats = DateTime.Now;
                        string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname = usr.uCode + "pendingadvances" + dat + usr.cCode + usr.bCode + ".pdf";
                        string filename = ho.WebRootPath + "\\Reps\\" + fname;
                        string msg = "";
                        string datss = ac.strDate(ac.getPresentDateTime());
                        msg = ep.pdfConversion(filename, "Pending Advances as on " + datss, usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname;
                        }
                        string fname1 = usr.uCode + "pendingadvances" + dat + usr.cCode + usr.bCode + ".xlsx";
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, "Pending Advances as on " + datss, usr, dt, titles, widths, aligns, false);
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

