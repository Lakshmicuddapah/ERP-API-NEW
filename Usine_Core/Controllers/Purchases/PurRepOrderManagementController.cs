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
    public class PurchaseRepRequestCompleteDetails
    {
        public string seq { get; set; }
        public string dat { get; set; }
        public string itemdescription { get; set; }
        public string purpose { get; set; }
        public string empname { get; set; }
        public string department { get; set; }
        public string qty { get; set; }
        public string approvedqty { get; set; }
        public string uom { get; set; }
        public string approvedby { get; set; }
        public string statu { get; set; }
        public int? pos { get; set; }
    }
    public class PurRepExpectedMaterialInfo
    {
        public string itemname { get; set; }
        public double? qty { get; set; }
        public string um { get; set; }
        public double? valu { get; set; }
    }
    public class PurRepOrderManagementTotal
    {
        public string pdfFile { get; set; }
        public string excelFile { get; set; }
        public List<PurchaseRepRequestCompleteDetails> details { get; set; }
    }
    public class PurRepExpectedMaterialTotal
    {
        public string pdfFile { get; set; }
        public string excelFile { get; set; }
        public List<PurRepExpectedMaterialInfo> details { get; set; }
    }
    public class PurRepExpiredRequestsInformation
    {
        public string seq { get; set; }
        public string dat { get; set; }
        public string item { get; set; }
        public string usr { get; set; }
        public string reqdby { get; set; }
        public string employee { get; set; }
        public string qty { get; set; }
        public string um { get; set; }
    }
    public class PurRepRequestsExpired
    {
        public string pdfFile { get; set; }
        public string excelFile { get; set; }
        public List<PurRepExpiredRequestsInformation> details { get; set; }
    }
    public class PurRepOrdersTotal
    {
        public string pdfFile { get; set; }
        public string excelFile { get; set; }
        public List<PurPurchaseOrderUni> details { get; set; }
    }
    public class PurRepEnquiriesTotal
    {
        public string pdfFile { get; set; }
        public string excelFile { get; set; }
        public List<PurPurchaseEnquiryUni> details { get; set; }
    }
    public class PurRepOrderManagementController : ControllerBase
    {
        private readonly IHostingEnvironment ho;
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        public PurRepOrderManagementController(IHostingEnvironment hostingEnvironment)
        {
            ho = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurRepOrderManagement/purRepOrderRequests")]
        public PurRepOrderManagementTotal purRepOrderRequests([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 2, 0))
            {
                try
                {
                    General gg = new General();
                    PurRepOrderManagementTotal tot = new PurRepOrderManagementTotal();
                    string dat1 = inf.frmDate;
                    string dat2= ac.strDate( DateTime.Parse(inf.toDate).AddDays(1));
                    string quer = "";
                    quer = quer + " select b.seq,b.dats,a.itemDescription,a.purpose,empname,b.department, a.qty,a.approvedQty,a.um,a.approvedusr,b.stau,b.statu from";
                    quer = quer + " (select a.recordID, a.sno, a.dat, a.itemId, a.itemDescription, a.purpose, a.qty, a.approvedQty, approvedusr, b.um  from";
                    quer = quer + " (select * from purPurchaseRequestDet where dat >= '" + dat1 + "' and dat < '" + dat2 + "'";
                    quer = quer + " and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
                    quer = quer + " (select * from invum where customerCode = " + inf.usr.cCode + ")b where a.um = b.recordId)a,";
                    quer = quer + " (select a.recordId,a.seq,dats,usr,a.department,b.empname,stau,statu from";
                    quer = quer + " (select a.recordId, right(seq,5) seq,dbo.strdate(dat) dats,usr,b.department,empno,case when statu = 1 then 'Pending' else 'Approved' end stau,";
                    quer = quer + " statu from";
                    quer = quer + " (select * from purPurchaseRequestUni where dat >= '" + dat1 + "' and dat < '" + dat2 + "'";
                    quer = quer + " and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
                    quer = quer + " (select * from invdepartments where branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b where a.department = b.recordId)a,";
                    quer = quer + " (select * from hrdEmployees where branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b where a.empno = b.recordId)b where a.recordId = b.recordId";
                    
                    if(inf.recordId != 0)
                    {
                      //  quer = quer + " and statu=" + inf.recordId.ToString();
                    }
                    quer = quer + " order by seq";

                    tot.details = new List<PurchaseRepRequestCompleteDetails>();
                    DataBaseContext g = new DataBaseContext();
                    SqlCommand dc = new SqlCommand();
                    dc.Connection = g.db;
                    g.db.Open();
                    dc.CommandText = quer;
                    SqlDataReader dr = dc.ExecuteReader();
                    while(dr.Read())
                    {
                        tot.details.Add(new PurchaseRepRequestCompleteDetails
                        {
                            seq=dr[0].ToString(),
                            dat=dr[1].ToString(),
                            itemdescription=dr[2].ToString(),
                            purpose=dr[3].ToString(),
                            empname=dr[4].ToString(),
                            department=dr[5].ToString(),
                            qty=dr[6].ToString(),
                            approvedqty=dr[7].ToString(),
                            uom=dr[8].ToString(),
                            approvedby=dr[9].ToString(),
                            statu=dr[10].ToString(),
                            pos=gg.valInt(dr[11].ToString())
                        });
                    }
                    dr.Close();
                    g.db.Close();
                    
                    if (tot.details.Count > 0)
                    {

                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));
                        dt.Columns.Add("seq", typeof(string));
                        dt.Columns.Add("dat", typeof(string));
                        dt.Columns.Add("item", typeof(string));
                        dt.Columns.Add("purpose", typeof(string));
                        dt.Columns.Add("emp", typeof(string));
                        dt.Columns.Add("dept", typeof(string));
                        dt.Columns.Add("qty", typeof(string));
                        dt.Columns.Add("aprqty", typeof(string));
                        dt.Columns.Add("um", typeof(string));
                        dt.Columns.Add("approvedby", typeof(string));
                        dt.Columns.Add("statu", typeof(string));
                        int i = 1;

                        foreach (var det in tot.details)
                        {
                            dt.Rows.Add(i.ToString(), det.seq,det.dat,det.itemdescription,det.purpose,det.empname,det.department,det.qty,det.approvedby,det.uom,det.approvedby,det.statu);
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Seq");
                        titles.Add("Date");
                        titles.Add("Item");
                        titles.Add("Purpose");
                        titles.Add("Employee");
                        titles.Add("Department");
                        titles.Add("Qty");
                        titles.Add("Approved");
                        titles.Add("UOM");
                        titles.Add("ApprovedBy");
                        titles.Add("Status");
                        float[] widths = { 30f,60f,80f,120f,120f,120f,100f,60f,60f,60f,100f,100f };
                        int[] aligns = { 3, 1, 1, 1,1,1,1,1,1,1,1,1 };
                        PDFExcelMake ep = new PDFExcelMake();

                        AdminControl ac = new AdminControl();
                        DateTime dats = DateTime.Now;
                        string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname = inf.usr.uCode + "RequestsRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                        string filename = ho.WebRootPath + "\\Reps\\" + fname;
                        string msg = "";

                        msg = ep.pdfLandscapeConversion(filename, "Purchase Requests from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname;
                        }
                        string fname1 = inf.usr.uCode + "RequestsRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, "Purchase Requests from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
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
        [Route("api/PurRepOrderManagement/purRepOrderPendingRequests")]
        public PurRepOrderManagementTotal purRepOrderPendingRequests([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 2, 0))
            {
                try
                {
                    General gg = new General();
                    PurRepOrderManagementTotal tot = new PurRepOrderManagementTotal();
                    string dat1 = inf.frmDate;
                    string dat2 = ac.strDate(DateTime.Parse(inf.toDate).AddDays(1));
                    string quer = "";
                    quer = quer + " select* from (select a.seq, a.dats, a.itemDescription, a.purpose, empname, department, a.qty,a.um, b.purrequest from";



                    quer = quer + " (select a.recordID,b.seq,b.dats,a.itemDescription,a.purpose,empname,b.department, a.qty,a.approvedQty,a.um,a.approvedusr,b.stau,b.statu from";
                    quer = quer + " (select a.recordID, a.sno, a.dat, a.itemId, a.itemDescription, a.purpose, a.qty, a.approvedQty, approvedusr, b.um  from";
                    quer = quer + " (select * from purPurchaseRequestDet where dat >= '" + dat1 + "' and dat < '" + dat2 + "'";
                    quer = quer + " and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
                    quer = quer + " (select * from invum where customerCode = " + inf.usr.cCode + ")b where a.um = b.recordId)a,";
                    quer = quer + " (select a.recordId,a.seq,dats,usr,a.department,b.empname,stau,statu from";
                    quer = quer + " (select a.recordId, right(seq,5) seq,dbo.strdate(dat) dats,usr,b.department,empno,case when statu = 1 then 'Pending' else 'Approved' end stau,";
                    quer = quer + " statu from";
                    quer = quer + " (select * from purPurchaseRequestUni where dat >= '" + dat1 + "' and dat < '" + dat2 + "'";
                    quer = quer + " and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
                    quer = quer + " (select * from invdepartments where branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b where a.department = b.recordId)a,";
                    quer = quer + " (select * from hrdEmployees where branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b where a.empno = b.recordId)b where a.recordId = b.recordId";


                    quer = quer + " and statu >= 2)a left outer join";
                    quer = quer + " (select* from purPurchaseOrderDet where branchid= '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b on a.recordId = b.purRequest and a.itemDescription = b.itemDescription)x";
                    quer = quer + " where purrequest is null order by seq";

                    tot.details = new List<PurchaseRepRequestCompleteDetails>();
                    DataBaseContext g = new DataBaseContext();
                    SqlCommand dc = new SqlCommand();
                    dc.Connection = g.db;
                    g.db.Open();
                    dc.CommandText = quer;
                    SqlDataReader dr = dc.ExecuteReader();
                    while (dr.Read())
                    {
                        tot.details.Add(new PurchaseRepRequestCompleteDetails
                        {
                            seq = dr[0].ToString(),
                            dat = dr[1].ToString(),
                            itemdescription = dr[2].ToString(),
                            purpose = dr[3].ToString(),
                            empname = dr[4].ToString(),
                            department = dr[5].ToString(),
                            qty = dr[6].ToString(),
                            uom = dr[7].ToString(),
                           
                        });
                    }
                    dr.Close();
                    g.db.Close();

                    if (tot.details.Count > 0)
                    {

                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));
                        dt.Columns.Add("seq", typeof(string));
                        dt.Columns.Add("dat", typeof(string));
                        dt.Columns.Add("item", typeof(string));
                        dt.Columns.Add("qty", typeof(string));
                         dt.Columns.Add("um", typeof(string));
                         int i = 1;

                        foreach (var det in tot.details)
                        {
                            dt.Rows.Add(i.ToString(), det.seq, det.dat, det.itemdescription, det.qty,  det.uom);
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Seq");
                        titles.Add("Date");
                        titles.Add("Item");
                        
                        titles.Add("Qty");
                       
                        titles.Add("UOM");
                       
                        float[] widths = { 40f, 60f, 80f,  170f,100f, 100f };
                        int[] aligns = { 3, 1, 1, 1, 1, 1 };
                        PDFExcelMake ep = new PDFExcelMake();

                        AdminControl ac = new AdminControl();
                        DateTime dats = DateTime.Now;
                        string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname = inf.usr.uCode + "RequestsRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                        string filename = ho.WebRootPath + "\\Reps\\" + fname;
                        string msg = "";

                        msg = ep.pdfConversion(filename, "Pending Requests from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname;
                        }
                        string fname1 = inf.usr.uCode + "RequestsRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, "Pending Requests from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
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
        [HttpPost]
        [Authorize]
        [Route("api/PurRepOrderManagement/purRepItemWisePendingRequests")]
        public PurRepOrderManagementTotal purRepItemWisePendingRequests([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 2, 0))
            {
                DataBaseContext g = new DataBaseContext();
            General gg = new General();
            PurRepOrderManagementTotal tot = new PurRepOrderManagementTotal();
            try
            {
                string quer = "";
                if (inf.recordId == 0)
                {
                    quer = quer + " select itemdescription,qty,b.um from (";
                    quer = quer + " select itemDescription,sum(approvedQty) qty,um from";
                    quer = quer + " (select * From purPurchaseRequestDet where pos >= 2 and recordId not in";
                    quer = quer + " (select purrequest from purPurchaseOrderDet where purRequest is not null and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")";
                    quer = quer + " and reqdBy > SYSDATETIME() and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")x";
                    quer = quer + " group by itemDescription,um";
                    quer = quer + " )a, (select * from invum where customerCode=" + inf.usr.cCode + ")b where a.UM =b.recordId order by itemdescription";
                }
                else
                {
                    quer = quer + " select itemdescription,qty,b.um from (";
                    quer = quer + " select itemDescription,sum(approvedQty) qty,um from";
                    quer = quer + " (select * From purPurchaseRequestDet where pos >= 2 and recordId not in";
                    quer = quer + " (select purrequest from purPurchaseOrderDet where purRequest is not null and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")";
                    quer = quer + " and branchId = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")x";
                    quer = quer + " group by itemDescription,um";
                    quer = quer + " )a, (select * from invum where customerCode=" + inf.usr.cCode + ")b where a.UM =b.recordId order by itemdescription";
                }

                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();
                tot.details = new List<PurchaseRepRequestCompleteDetails>();
                while (dr.Read())
                {
                    tot.details.Add(new PurchaseRepRequestCompleteDetails
                    {
                        itemdescription = dr[0].ToString(),
                        qty = dr[1].ToString(),
                        uom = dr[2].ToString()
                    });
                }
                dr.Close();
                g.db.Close();

                if (tot.details.Count > 0)
                {

                    DataTable dt = new DataTable();
                    dt.Columns.Add("sno", typeof(string));
                    dt.Columns.Add("item", typeof(string));
                    dt.Columns.Add("qty", typeof(string));
                    dt.Columns.Add("um", typeof(string));
                    int i = 1;

                    foreach (var det in tot.details)
                    {
                        dt.Rows.Add(i.ToString(), det.itemdescription, det.qty, det.uom);
                        i++;
                    }

                    List<string> titles = new List<string>();
                    titles.Add("#");

                    titles.Add("Item");

                    titles.Add("Qty");

                    titles.Add("UOM");

                    float[] widths = { 40f, 210f, 100f, 100f };
                    int[] aligns = { 3, 1, 2, 1 };
                    PDFExcelMake ep = new PDFExcelMake();

                    AdminControl ac = new AdminControl();
                    DateTime dats = DateTime.Now;
                    string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                    string fname = inf.usr.uCode + "RequestsRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                    string filename = ho.WebRootPath + "\\Reps\\" + fname;
                    string msg = "";

                    msg = ep.pdfConversion(filename, " Material wise requests to be ordered  as on " + gg.strDateTime(ac.getPresentDateTime()), inf.usr, dt, titles, widths, aligns, false);
                    if (msg == "OK")
                    {
                        tot.pdfFile = fname;
                    }
                    string fname1 = inf.usr.uCode + "RequestsRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                    string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                    msg = "";
                    msg = ep.makeExcelConversion(filename1, " Material wise requests to be ordered as on " + gg.strDateTime(ac.getPresentDateTime()), inf.usr, dt, titles, widths, aligns, false);
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
        [Route("api/PurRepOrderManagement/PurRepRequestsExpired")]
        public PurRepRequestsExpired PurRepRequestsExpired([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 2, 11, 2, 0))
            {
                DataBaseContext g = new DataBaseContext();
                General gg = new General();
                var dat = ac.strDate(ac.getPresentDateTime());
                string quer = "";
                quer = quer + " select a.seq,dbo.strDate(a.dat) dat,a.itemDescription,a.approvedQty,a.usr, dbo.strDate(a.reqdby) reqdby,a.empname,a.department,a.um from";
                quer = quer + " (select a.recordID, a.sno, a.seq, a.dat, a.itemId, a.itemDescription, a.approvedQty, a.usr,";
                quer = quer + " a.reqdby, a.empname, a.department, b.um from";
            quer = quer + " (select a.recordID, a.sno, a.seq, a.dat, a.itemId, a.itemDescription, a.approvedQty, a.usr, a.reqdby, a.empname, b.department, a.um from";
                quer = quer + " (select a.recordID, a.sno, a.seq, a.dat, a.itemId, a.itemDescription, a.approvedQty, a.usr, a.reqdby, b.empname, a.department, a.um from";
                quer = quer + " (select a.recordId, a.sno, b.seq, a.dat, a.itemDescription, a.itemId, a.approvedQty, a.usr, a.reqdBy, b.empno, b.department, a.UM from";
                quer = quer + " (select * From purpurchaserequestdet where branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + "";
                quer = quer + " and reqdBy < '" + dat + "' and pos >= 2)a, (select * from purPurchaseRequestUni where branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b where";
                quer = quer + " a.recordId = b.recordId)a, (select * from hrdEmployees where branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b where a.empno = b.recordId)a,";
                quer = quer + " (select * from invDepartments where branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b where a.department = b.recordId)a,";
                quer = quer + " (select * from invum where customerCode = " + usr.cCode + ")b where a.UM = b.recordId )a left outer join";
                quer = quer + " (select* from purpurchaseorderdet where purRequest is not null and branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b";
                quer = quer + " on a.recordId = b.purRequest and a.itemId = b.itemId";

                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();
                PurRepRequestsExpired tot = new PurRepRequestsExpired();
                tot.details = new List<PurRepExpiredRequestsInformation>();
                while (dr.Read())
                {
                    tot.details.Add(new PurRepExpiredRequestsInformation
                    {
                        seq = dr[0].ToString(),
                        dat = dr[1].ToString(),
                        item = dr[2].ToString(),
                        usr = dr[4].ToString(),
                        reqdby = dr[5].ToString(),
                        employee = dr[6].ToString(),
                        qty = dr[3].ToString(),
                        um = dr[8].ToString()
                    });
                }
                dr.Close();
                g.db.Close();

                if (tot.details.Count > 0)
                {

                    DataTable dt = new DataTable();
                    dt.Columns.Add("sno", typeof(string));
                    dt.Columns.Add("reqid", typeof(string));
                    dt.Columns.Add("dat", typeof(string));
                    dt.Columns.Add("item", typeof(string));
                    dt.Columns.Add("usr", typeof(string));
                    dt.Columns.Add("reqdby", typeof(string));
                    dt.Columns.Add("employee", typeof(string));
                    dt.Columns.Add("qty", typeof(string));
                    dt.Columns.Add("um", typeof(string));
                    int i = 1;

                    foreach (var det in tot.details)
                    {
                        dt.Rows.Add(i.ToString(), det.seq, det.dat, det.item, det.usr, det.reqdby, det.employee, det.qty, det.um);
                        i++;
                    }

                    List<string> titles = new List<string>();
                    titles.Add("#");
                    titles.Add("Req.#");
                    titles.Add("Date");
                    titles.Add("Item");
                    titles.Add("User");
                    titles.Add("Reqd By");
                    titles.Add("Employee");
                    titles.Add("Qty");

                    titles.Add("UOM");

                    float[] widths = { 40f, 60f, 70f, 150f, 70f, 70f, 140f, 60f, 60f };
                    int[] aligns = { 3, 1, 1, 1, 1, 1, 1, 2, 3 };
                    PDFExcelMake ep = new PDFExcelMake();

                    AdminControl ac = new AdminControl();
                    DateTime dats = DateTime.Now;
                    string datss = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                    string fname = usr.uCode + "RequestsRep" + datss + usr.cCode + usr.bCode + ".pdf";
                    string filename = ho.WebRootPath + "\\Reps\\" + fname;
                    string msg = "";

                    msg = ep.pdfLandscapeConversion(filename, " Expired Requests as on " + dat, usr, dt, titles, widths, aligns, false);
                    if (msg == "OK")
                    {
                        tot.pdfFile = fname;
                    }
                    string fname1 = usr.uCode + " Expired Requests as on " + dat + usr.cCode + usr.bCode + ".xlsx";
                    string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                    msg = "";
                    msg = ep.makeExcelConversion(filename1, " Material wise requests to be ordered as on " + gg.strDateTime(ac.getPresentDateTime()), usr, dt, titles, widths, aligns, false);
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
        [Route("api/PurRepOrderManagement/PurRepRequestsTobeApproved")]
        public PurRepRequestsExpired PurRepRequestsTobeApproved([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 2, 11, 2, 0))
            {
                DataBaseContext g = new DataBaseContext();
                General gg = new General();
                var dat = ac.strDate(ac.getPresentDateTime());
                string quer = "";
                quer = quer + " select a.recordID, a.sno, a.seq, dbo.strDate(a.dat) dat, a.itemId, a.itemDescription, a.qty, a.usr, dbo.strDate(a.reqdby) reqdBy, a.empname,   b.um from ";
                 quer = quer + " (select a.recordID, a.sno, a.seq, a.dat, a.itemId, a.itemDescription, a.qty, a.usr, a.reqdby, b.empname, a.department, a.um from ";
                quer = quer + " (select a.recordId, a.sno, b.seq, a.dat, a.itemDescription, a.itemId, a.qty, a.usr, a.reqdBy, b.empno, b.department, a.UM from ";
                quer = quer + " (select * From purpurchaserequestdet where branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + " and pos = 1)a, ";
                quer = quer + " (select * from purPurchaseRequestUni where branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b where a.recordId = b.recordId)a,  ";
                quer = quer + " (select * from hrdEmployees where branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b where a.empno = b.recordId)a,  ";
                quer = quer + " (select * from invum where customerCode = " + usr.cCode + ")b where a.um = b.recordId ";

                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();
                PurRepRequestsExpired tot = new PurRepRequestsExpired();
                tot.details = new List<PurRepExpiredRequestsInformation>();
                while (dr.Read())
                {
                    tot.details.Add(new PurRepExpiredRequestsInformation
                    {
                        seq = dr[2].ToString(),
                        dat = dr[3].ToString(),
                        item = dr[5].ToString(),
                        usr = dr[7].ToString(),
                        reqdby = dr[8].ToString(),
                        employee = dr[9].ToString(),
                        qty = dr[6].ToString(),
                        um = dr[10].ToString()
                    });
                }
                dr.Close();
                g.db.Close();

                if (tot.details.Count > 0)
                {

                    DataTable dt = new DataTable();
                    dt.Columns.Add("sno", typeof(string));
                    dt.Columns.Add("reqid", typeof(string));
                    dt.Columns.Add("dat", typeof(string));
                    dt.Columns.Add("item", typeof(string));
                    dt.Columns.Add("usr", typeof(string));
                    dt.Columns.Add("reqdby", typeof(string));
                    dt.Columns.Add("employee", typeof(string));
                    dt.Columns.Add("qty", typeof(string));
                    dt.Columns.Add("um", typeof(string));
                    int i = 1;

                    foreach (var det in tot.details)
                    {
                        dt.Rows.Add(i.ToString(), det.seq, det.dat, det.item, det.usr, det.reqdby, det.employee, det.qty, det.um);
                        i++;
                    }

                    List<string> titles = new List<string>();
                    titles.Add("#");
                    titles.Add("Req.#");
                    titles.Add("Date");
                    titles.Add("Item");
                    titles.Add("User");
                    titles.Add("Reqd By");
                    titles.Add("Employee");
                    titles.Add("Qty");

                    titles.Add("UOM");

                    float[] widths = { 40f, 60f, 70f, 150f, 70f, 70f, 140f, 60f, 60f };
                    int[] aligns = { 3, 1, 1, 1, 1, 1, 1, 2, 3 };
                    PDFExcelMake ep = new PDFExcelMake();

                    AdminControl ac = new AdminControl();
                    DateTime dats = DateTime.Now;
                    string datss = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                    string fname = usr.uCode + "RequestsRep" + datss + usr.cCode + usr.bCode + ".pdf";
                    string filename = ho.WebRootPath + "\\Reps\\" + fname;
                    string msg = "";

                    msg = ep.pdfLandscapeConversion(filename, " Expired Requests as on " + dat, usr, dt, titles, widths, aligns, false);
                    if (msg == "OK")
                    {
                        tot.pdfFile = fname;
                    }
                    string fname1 = usr.uCode + " Expired Requests as on " + dat + usr.cCode + usr.bCode + ".xlsx";
                    string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                    msg = "";
                    msg = ep.makeExcelConversion(filename1, " Material wise requests to be ordered as on " + gg.strDateTime(ac.getPresentDateTime()), usr, dt, titles, widths, aligns, false);
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
        [Route("api/PurRepOrderManagement/PurRepListOfEnquiries")]
        public PurRepEnquiriesTotal PurRepListOfEnquiries([FromBody] GeneralInformation inf)
        {
            if(ac.screenCheck(inf.usr,2,11,2,0))
            {

          
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
            var details = (from a in db.PurPurchaseEnquiryUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                           join b in db.PurQuotationUni.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.RecordId equals b.RefQuotationId into gj
                           from subdet in gj.DefaultIfEmpty()
                           select new PurPurchaseEnquiryUni
                           {
                               RecordId = a.RecordId,
                               Seq = a.Seq,
                               Reference = subdet == null ? " " : subdet.Seq,
                               Dat = a.Dat,
                               Usr = a.Usr,
                               Validity = a.Validity,
                               Supplier = a.Supplier,
                               Mobile = a.Mobile,
                               Pos = a.Pos,
                               BranchId = a.Pos == 1 ? "Pending" : (subdet == null ? "No Quotation" : "Cleared")
                           }).ToList();
            PurRepEnquiriesTotal tot = new PurRepEnquiriesTotal();
            string title = "";
            switch (inf.recordId)
            {
                case 1:
                    tot.details = details.OrderBy(a => a.Dat).ThenBy(b => b.Seq).ToList();
                    title = "List of Enquiries from " + inf.frmDate + " to " + inf.toDate;
                    break;
                case 2:
                    tot.details = details.Where(a => a.BranchId == "No Quotation" || a.BranchId=="Pending").OrderBy(b => b.Dat).ThenBy(c => c.Seq).ToList();
                    title = "List of Pending Enquiries from " + inf.frmDate + " to " + inf.toDate;
                    break;
                case 3:
                    tot.details = details.Where(a => a.Reference.Trim() =="" && a.Validity < ac.getPresentDateTime()).OrderBy(b => b.Dat).ThenBy(c => c.Seq).ToList();
                    title = "List of Expired Enquiries from " + inf.frmDate + " to " + inf.toDate;
                    break;
            }
             
                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("seq", typeof(string));
                dt.Columns.Add("refe", typeof(string));
                dt.Columns.Add("dat", typeof(string));
                dt.Columns.Add("usr", typeof(string));
                dt.Columns.Add("validity", typeof(string));
                dt.Columns.Add("supplier", typeof(string));
                dt.Columns.Add("mobile", typeof(string));
                dt.Columns.Add("statu", typeof(string));
                int i = 1;
                General gg = new General();
                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i.ToString(), det.Seq, det.Reference,gg.strDate(det.Dat.Value),det.Usr,gg.strDate(det.Validity.Value),det.Supplier,det.Mobile,det.BranchId);
                    i++;
                }

                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Seq");
                titles.Add("Reference");
                titles.Add("Date");
                titles.Add("User");
                titles.Add("Validity");
                titles.Add("Supplier");
                titles.Add("Mobile");
                titles.Add("Status");

                float[] widths = { 40f,60f,60f,70f,90f,70f, 150f, 90f,90f};
                int[] aligns = { 3, 1, 1, 1,1,1,1,1,1 };
                PDFExcelMake ep = new PDFExcelMake();
                
               
                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "enquiriesRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";

                msg = ep.pdfLandscapeConversion(filename, title, inf.usr, dt, titles, widths, aligns, false);
                if (msg == "OK")
                {
                    tot.pdfFile = fname;
                }
                string fname1 = inf.usr.uCode + "enquiries" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
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



        [HttpPost]
        [Authorize]
        [Route("api/PurRepOrderManagement/purRepOrderListOfOrders")]
        public PurRepOrdersTotal purRepOrderListOfOrders([FromBody] GeneralInformation inf)
        {
            if(ac.screenCheck(inf.usr,2,11,2,0))
                    {
                try
                {
                    DateTime dat1 = DateTime.Parse(inf.frmDate);
                    DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                    PurRepOrdersTotal tot = new PurRepOrdersTotal();
                    tot.details = db.PurPurchaseOrderUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.Dat).ThenBy(c => c.RecordId).ToList();
                    General g = new General();

                    if (tot.details.Count > 0)
                    {

                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));
                        dt.Columns.Add("seq", typeof(string));
                        dt.Columns.Add("dat", typeof(string));
                        dt.Columns.Add("supplier", typeof(string));
                        dt.Columns.Add("baseamt", typeof(string));
                        dt.Columns.Add("totalamt", typeof(string));
                        dt.Columns.Add("pos", typeof(string));
                        int i = 1;

                        foreach (var det in tot.details)
                        {
                            dt.Rows.Add(i.ToString(), det.Seq, g.strDate(det.Dat.Value), det.Vendorname, g.makeCur((double)det.Baseamt, 2), g.makeCur((double)det.TotalAmt, 2), det.Pos == 1 ? "Pending" : "Closed");
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Seq");
                        titles.Add("Date");
                        titles.Add("Supplier");

                        titles.Add("BaseAmt");

                        titles.Add("TotalAmt");
                        titles.Add("Position");
                        float[] widths = { 40f, 80f, 90f, 210f, 100f, 100f, 100f };
                        int[] aligns = { 3, 1, 1, 1, 2, 2, 1 };
                        PDFExcelMake ep = new PDFExcelMake();

                        AdminControl ac = new AdminControl();
                        DateTime dats = DateTime.Now;
                        string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname = inf.usr.uCode + "OrdersRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                        string filename = ho.WebRootPath + "\\Reps\\" + fname;
                        string msg = "";

                        msg = ep.pdfLandscapeConversion(filename, "Purchase Orders from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname;
                        }
                        string fname1 = inf.usr.uCode + "OrdersRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, "Purchase Orders from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
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
                catch (Exception ex)
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
        [Route("api/PurRepOrderManagement/purRepOrderListOfOrdersSupplierWise")]
        public PurRepOrdersTotal purRepOrderListOfOrdersSupplierWise([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 2, 0))
            {
                try
                {
                    DateTime dat1 = DateTime.Parse(inf.frmDate);
                    DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                    PurRepOrdersTotal tot = new PurRepOrdersTotal();
                    tot.details = db.PurPurchaseOrderUni.Where(a => a.Vendorid==inf.recordId && a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.Dat).ThenBy(c => c.RecordId).ToList();
                    General g = new General();

                    if (tot.details.Count > 0)
                    {

                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));
                        dt.Columns.Add("seq", typeof(string));
                        dt.Columns.Add("dat", typeof(string));
                        dt.Columns.Add("supplier", typeof(string));
                        dt.Columns.Add("baseamt", typeof(string));
                        dt.Columns.Add("totalamt", typeof(string));
                        dt.Columns.Add("pos", typeof(string));
                        int i = 1;

                        foreach (var det in tot.details)
                        {
                            dt.Rows.Add(i.ToString(), det.Seq, g.strDate(det.Dat.Value), det.Vendorname, g.makeCur((double)det.Baseamt, 2), g.makeCur((double)det.TotalAmt, 2), det.Pos == 1 ? "Pending" : "Closed");
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Seq");
                        titles.Add("Date");
                        titles.Add("Supplier");

                        titles.Add("BaseAmt");

                        titles.Add("TotalAmt");
                        titles.Add("Position");
                        float[] widths = { 40f, 80f, 90f, 210f, 100f, 100f, 100f };
                        int[] aligns = { 3, 1, 1, 1, 2, 2, 1 };
                        PDFExcelMake ep = new PDFExcelMake();

                        AdminControl ac = new AdminControl();
                        DateTime dats = DateTime.Now;
                        string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname = inf.usr.uCode + "OrdersRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                        string filename = ho.WebRootPath + "\\Reps\\" + fname;
                        string msg = "";

                        msg = ep.pdfLandscapeConversion(filename, "Purchase Orders from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname;
                        }
                        string fname1 = inf.usr.uCode + "OrdersRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, "Purchase Orders from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
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
                catch (Exception ex)
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
        [Route("api/PurRepOrderManagement/purRepOrderListOfOrdersPending")]
        public PurRepOrdersTotal purRepOrderListOfOrdersPending([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr, 2, 11, 2, 0))
            {
                try
                {
                    DateTime dat1 = DateTime.Parse(inf.frmDate);
                    DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                    PurRepOrdersTotal tot = new PurRepOrdersTotal();
                    tot.details = db.PurPurchaseOrderUni.Where(a => a.Pos == 1 && a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.Dat).ThenBy(c => c.RecordId).ToList();
                    General g = new General();

                    if (tot.details.Count > 0)
                    {

                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));
                        dt.Columns.Add("seq", typeof(string));
                        dt.Columns.Add("dat", typeof(string));
                        dt.Columns.Add("supplier", typeof(string));
                        dt.Columns.Add("baseamt", typeof(string));
                        dt.Columns.Add("totalamt", typeof(string));
                        dt.Columns.Add("pos", typeof(string));
                        int i = 1;

                        foreach (var det in tot.details)
                        {
                            dt.Rows.Add(i.ToString(), det.Seq, g.strDate(det.Dat.Value), det.Vendorname, g.makeCur((double)det.Baseamt, 2), g.makeCur((double)det.TotalAmt, 2), det.Pos == 1 ? "Pending" : "Closed");
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Seq");
                        titles.Add("Date");
                        titles.Add("Supplier");

                        titles.Add("BaseAmt");

                        titles.Add("TotalAmt");
                        titles.Add("Position");
                        float[] widths = { 40f, 80f, 90f, 210f, 100f, 100f, 100f };
                        int[] aligns = { 3, 1, 1, 1, 2, 2, 1 };
                        PDFExcelMake ep = new PDFExcelMake();

                        AdminControl ac = new AdminControl();
                        DateTime dats = DateTime.Now;
                        string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname = inf.usr.uCode + "OrdersRep" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                        string filename = ho.WebRootPath + "\\Reps\\" + fname;
                        string msg = "";

                        msg = ep.pdfLandscapeConversion(filename, "Pending Purchase Orders from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname;
                        }
                        string fname1 = inf.usr.uCode + "OrdersRep" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, "Pending Purchase Orders from " + dat1 + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
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
                catch (Exception ex)
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
        [Route("api/PurRepOrderManagement/purRepOrderExpectedMaterials")]
        public PurRepExpectedMaterialTotal purRepOrderExpectedMaterials([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 2, 11, 2, 0))
            {
                try
                {
                    General gg = new General();
                    PurRepExpectedMaterialTotal tot = new PurRepExpectedMaterialTotal();
                   
                    string quer = "";
                   /* quer = quer + " select * from ";
                    quer = quer + " (select itemname, convert(varchar(10), sum(qty)) qty, max(um) um, dbo.makcur(sum(valu)) valu, 1 sno from";
                    quer = quer + " (select a.itemid, a.itemname, qty, b.um, rat, qty * rat valu from";
                    quer = quer + " (select a.itemId, itemname, qty * conversionFactor qty, rat / conversionFactor rat, a.um umid, b.stdUm, conversionFactor from";
                    quer = quer + " (select itemId, itemname, qty, rat, um from purpurchaseorderdet where recordId in";
                    quer = quer + " (select recordId from purPurchaseOrderUni where pos=1 and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + "))a,";
                    quer = quer + " (select * from invMaterialUnits where customerCode = " + usr.cCode + " )b where a.um = b.um and a.itemId = b.recordId)a,";
                    quer = quer + " (select * from invum where customerCode = " + usr.cCode + ")b where a.stdUm = b.recordID)x group by itemname";
                    quer = quer + " union all select ' ' itemname,' ' qty,' ' um,' '  valu,2 sno  union all";
                    quer = quer + " select 'Total' itemname,' ' qty,' ' um, dbo.makcur(sum(valu)) valu, 3 sno from";
                    quer = quer + " (select a.itemid, a.itemname, qty, b.um, rat, qty* rat valu from";
                    quer = quer + " (select a.itemId, itemname, qty* conversionFactor qty, rat/ conversionFactor rat, a.um umid, b.stdUm,conversionFactor from";
                    quer = quer + " (select itemId, itemname, qty, rat, um from purpurchaseorderdet where recordId in";
                    quer = quer + " (select recordId from purPurchaseOrderUni where pos=1 and branchId= '" + usr.bCode + "' and customerCode = " + usr.cCode + ")  )a,";
                    quer = quer + " (select * from invMaterialUnits where customerCode = " + usr.cCode + ")b where a.um = b.um and a.itemId = b.recordId)a,";
                    quer = quer + " (select * from invum where customerCode = " + usr.cCode + ")b where a.stdUm = b.recordID)x)x order by sno, itemname";

                    quer = "";*/
                    quer = quer + " select itemname,sum(qty) qty,max(um) um,sum(valu) valu from";
                    quer = quer + " (select b.itemname, qty, b.um, rat, qty * rat valu from";
                    quer = quer + " (select a.itemid, a.qty * b.conversionFactor qty, b.stdum umid, a.rat / b.conversionFactor rat from";
                    quer = quer + " (select itemid, qty, um, rat from purPurchaseOrderDet where recordId in";
                    quer = quer + " (select recordID from purpurchaseorderuni where recordId not in";
                    quer = quer + " (select refPOId from purpurchasesuni where refPoid is not null and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")";
                    quer = quer + " and branchId = '" + usr.bCode + "' and customercode = " + usr.cCode + " and validity >= SYSDATETIME()) and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a,";
                    quer = quer + " (select * from invMaterialUnits where customercode = " + usr.cCode + " )b where a.itemId = b.recordId and a.um = b.um)a,";
                    quer = quer + " (select * from invMaterialCompleteDetails_view where branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b where a.itemId = b.recordId)x group by itemname";


                    var details = new List<PurRepExpectedMaterialInfo>();
                    tot.details = new List<PurRepExpectedMaterialInfo>();
                    DataBaseContext g = new DataBaseContext();
                    SqlCommand dc = new SqlCommand();
                    dc.Connection = g.db;
                    g.db.Open();
                    dc.CommandText = quer;
                    SqlDataReader dr = dc.ExecuteReader();
                    while (dr.Read())
                    {
                         details.Add(new PurRepExpectedMaterialInfo
                         {
                            itemname=dr[0].ToString(),
                            qty=gg.valNum( dr[1].ToString()),
                            um=dr[2].ToString(),
                            valu= gg.valNum(dr[3].ToString()),

                        });
                    }
                    dr.Close();
                    g.db.Close();

                    tot.details.AddRange(details);
                    tot.details.Add(new PurRepExpectedMaterialInfo
                    {
                        itemname = " ",
                        um = " "
                    });
                    tot.details.Add(new PurRepExpectedMaterialInfo
                    {
                        itemname = "Total",
                        um = " ",
                        valu=details.Sum(a => a.valu)
                    });


                    if (tot.details.Count > 0)
                    {

                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));
                       
                        dt.Columns.Add("item", typeof(string));
                        dt.Columns.Add("qty", typeof(string));
                        dt.Columns.Add("uom", typeof(string));
                        dt.Columns.Add("valu", typeof(string));
                        
                        int i = 1;

                        foreach (var det in tot.details)
                        {
                            dt.Rows.Add(i <tot.details.Count()-1 ?i.ToString():"", det.itemname, det.qty, det.um, det.valu==null?null:gg.fixCur((double)det.valu,2));
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Material");
                        titles.Add("Qty");
                        titles.Add("UOM");
                        titles.Add("Value");
                        
                        float[] widths = { 40f, 200f, 100f, 100f,110f };
                        int[] aligns = { 3, 1, 2, 1, 2 };
                        PDFExcelMake ep = new PDFExcelMake();

                        AdminControl ac = new AdminControl();
                        DateTime dats = DateTime.Now;
                        string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname =  usr.uCode + "ExpectedMaterial" + dat + usr.cCode + usr.bCode + ".pdf";
                        string filename = ho.WebRootPath + "\\Reps\\" + fname;
                        string msg = "";
                        string datss = ac.strDate( ac.getPresentDateTime());
                        msg = ep.pdfConversion(filename, "Expected Material as on " + datss, usr, dt, titles, widths, aligns, true);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname;
                        }
                        string fname1 = usr.uCode + "ExpectedMaterial" + dat + usr.cCode + usr.bCode + ".xlsx";
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, "Expected Material as on " + datss, usr, dt, titles, widths, aligns, true);
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


        [HttpPost]
        [Authorize]
        [Route("api/PurRepOrderManagement/purRepOrderPendingAdvances")]
        public PurRepOrderManagementTotal purRepOrderPendingAdvances([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 2, 11, 2, 0))
            {
                try
                {
                    General gg = new General();
                    PurRepOrderManagementTotal tot = new PurRepOrderManagementTotal();

                    string quer = "";
                    quer = quer + " select seq,dats,vendorname,mobile,dbo.makcur(amt) amt,paymentmode,sno from  ";
                    quer = quer + " (select right(a.seq,4) seq,dbo.strDAte(a.dat) dats,b.vendorname,b.mobile,amt,a.paymentmode,1 sno from";
                    quer = quer + " (select * from totAdvanceDetails where branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a,";
                    quer = quer + " (select * from purPurchaseOrderUni where pos = 1 and branchId = '" + usr.bCode + "' and customerCode =  " + usr.cCode + ")b where a.transactionid = b.recordId";

                    quer = quer + " union all";
                    quer = quer + " select ' ' seq,' ' dats,' ' vendorname,' ' mobile,0 amt,' ' paymentmode,2 sno";
                    quer = quer + " union all  select ' '  seq,' '  dats,'Total',' ' mobile, sum(a.amt) amt,' ' paymentmode,3 sno from";
                    quer = quer + " (select * from totAdvanceDetails where branchId= '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a,";
                    quer = quer + " (select * from purPurchaseOrderUni where pos = 1 and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b where a.transactionid = b.recordId)x order by sno, seq";




                    tot.details = new List<PurchaseRepRequestCompleteDetails>();
                    DataBaseContext g = new DataBaseContext();
                    SqlCommand dc = new SqlCommand();
                    dc.Connection = g.db;
                    g.db.Open();
                    dc.CommandText = quer;
                    SqlDataReader dr = dc.ExecuteReader();
                    while (dr.Read())
                    {
                        tot.details.Add(new PurchaseRepRequestCompleteDetails
                        {
                            seq=dr[0].ToString(),
                            dat=dr[1].ToString(),
                            itemdescription = dr[2].ToString(),
                            purpose=dr[3].ToString(),
                            qty = dr[4].ToString(),
                            uom = dr[5].ToString(),
                          

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
                            dt.Rows.Add(i < tot.details.Count() - 1 ? i.ToString() : "",det.seq,det.dat,det.itemdescription,det.purpose,det.qty,det.uom);
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Voucher");
                        titles.Add("Date");
                        titles.Add("Supplier");
                        titles.Add("Mobile");
                        titles.Add("Amt");
                        titles.Add("Mode");

                        float[] widths = { 40f,50f,60f, 160f, 100f, 80f, 70f };
                        int[] aligns = { 3, 1, 1, 1, 1,2,1};
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


        [HttpPost]
        [Authorize]
        [Route("api/PurRepOrderManagement/purRepOrderSupplierwisePendingAdvances")]
        public PurRepOrderManagementTotal purRepOrderSupplierwisePendingAdvances([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 2, 11, 2, 0))
            {
                try
                {
                    General gg = new General();
                    PurRepOrderManagementTotal tot = new PurRepOrderManagementTotal();

                    string quer = "";
                    quer = quer + " select * from";
                    quer = quer + " (select b.vendorname, dbo.makcur(sum(a.amt)) amt, 1 sno from";
                    quer = quer + " (select * from totAdvanceDetails where branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a,";
                    quer = quer + " (select * from purPurchaseOrderUni where pos = 1 and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b where a.transactionid = b.recordId";
                    quer = quer + " group by b.vendorname  union all select  ' ' vendorname,' ' amt,2 sno union all";
                    quer = quer + " select  'Total', dbo.makcur(sum(a.amt)) amt,3 sno from ";
                    quer = quer + " (select* from totAdvanceDetails where branchId= '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a,";
                    quer = quer + " (select * from purPurchaseOrderUni where pos = 1 and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b where a.transactionid = b.recordId)x order by sno,vendorname";



                    tot.details = new List<PurchaseRepRequestCompleteDetails>();
                    DataBaseContext g = new DataBaseContext();
                    SqlCommand dc = new SqlCommand();
                    dc.Connection = g.db;
                    g.db.Open();
                    dc.CommandText = quer;
                    SqlDataReader dr = dc.ExecuteReader();
                    while (dr.Read())
                    {
                        tot.details.Add(new PurchaseRepRequestCompleteDetails
                        {
                             
                            itemdescription = dr[0].ToString(),
                            
                            qty = dr[1].ToString(),
                   

                        });
                    }
                    dr.Close();
                    g.db.Close();

                    if (tot.details.Count > 0)
                    {

                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));

                        dt.Columns.Add("supplier", typeof(string));
                        dt.Columns.Add("amt", typeof(string));
                       
                        int i = 1;

                        foreach (var det in tot.details)
                        {
                            dt.Rows.Add(i < tot.details.Count() - 1 ? i.ToString() : "", det.itemdescription,  det.qty );
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Supplier");
                        titles.Add("Amount");
                       

                        float[] widths = { 40f, 300f,200f };
                        int[] aligns = { 3, 1, 2};
                        PDFExcelMake ep = new PDFExcelMake();

                        AdminControl ac = new AdminControl();
                        DateTime dats = DateTime.Now;
                        string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname = usr.uCode + "pendingadvances" + dat + usr.cCode + usr.bCode + ".pdf";
                        string filename = ho.WebRootPath + "\\Reps\\" + fname;
                        string msg = "";
                        string datss = ac.strDate(ac.getPresentDateTime());
                        msg = ep.pdfConversion(filename, "Supplier wise Pending Advances as on " + datss, usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname;
                        }
                        string fname1 = usr.uCode + "pendingadvances" + dat + usr.cCode + usr.bCode + ".xlsx";
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, "Supplier wise Pending Advances as on " + datss, usr, dt, titles, widths, aligns, false);
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
