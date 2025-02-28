using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Usine_Core.Controllers.Others;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Usine_Core.Controllers.Purchases
{
    public class PurchaseRequestListInfo
    {
        public string seq { get; set; }
        public string sno { get; set; }
        public string dat { get; set; }
        public string matname { get; set; }
        public string purpose { get; set; }
        public string qty { get; set; }
        public string approvedqty { get; set; }
        public string uom { get; set; }
        public string reqdby { get; set; }
        public int? pos { get; set; }
        public string department { get; set; }
        public string employee { get; set; }
    }
    public class PurchaseRequestListInfoTotal
    {
        public List<PurchaseRequestListInfo> details { get; set; }
        public String pdfFile { get; set; }
        public String excelFile { get; set; }
    }
    public class PurchaseOrderManagementController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        private readonly IHostingEnvironment ho;
        public PurchaseOrderManagementController(IHostingEnvironment hostingEnvironment)
        {
            ho = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        [Route("api/posDayReps/getSettlements")]
        public PurchaseRequestListInfoTotal ListOfPurchaseRequests([FromBody] GeneralInformation inf)
        {
            string dat1, dat2;
            AdminControl ac = new AdminControl();
            dat1 =  inf.frmDate;
            dat2 = ac.strDate(DateTime.Parse(inf.toDate).AddDays(1));
            General g = new General();
            DataBaseContext gg = new DataBaseContext();
            PurchaseRequestListInfoTotal tot = new PurchaseRequestListInfoTotal();
            string quer = "";
            quer = quer + " select b.seq,a.sno,dats,itemdescription,purpose,qty,approvedQty,um,dbo.strdate(a.reqdby) reqds,a.pos,department,empname from";
            quer = quer + " (select a.recordId, a.sno, dats, itemdescription, purpose, qty, approvedQty, um, a.reqdby, a.pos, b.department from";
            quer = quer + " (select a.recordId, a.sno, dbo.strdate(a.dat) dats, itemdescription, purpose, qty, approvedQty, b.um, a.reqdby, a.pos, a.department from";
            quer = quer + " (select recordId, sno, dat, itemdescription, purpose, qty, approvedQty, um, reqdby, pos, department";
            quer = quer + " from purpurchaserequestdet where dat >= '" + dat1 + "' and dat < '" + dat2 + "' and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
            quer = quer + " (select * from invum where customerCode = " + inf.usr.cCode + ")b where a.um = b.recordId)a,";
            quer = quer + " (select * from invDepartments where branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b where a.department = b.recordId)a,";
            quer = quer + " (select a.recordId,seq,b.empname from";
            quer = quer + " (select * from purpurchaserequestuni where branchid= '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
            quer = quer + " (select * from hrdEmployees where branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b where a.empno = b.recordId)b where a.recordId = b.recordId order by a.recordId";

            SqlCommand dc = new SqlCommand();
            dc.Connection = gg.db;
            gg.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
           tot.details = new List<PurchaseRequestListInfo>();
            while (dr.Read())
            {
                tot.details.Add(new PurchaseRequestListInfo
                {
                    seq = dr[0].ToString(),
                    sno = dr[1].ToString(),
                    dat = dr[2].ToString(),
                    matname = dr[3].ToString(),
                    purpose = dr[4].ToString(),
                    qty = dr[5].ToString(),
                    approvedqty = dr[6].ToString(),
                    uom = dr[7].ToString(),
                    reqdby = dr[8].ToString(),
                    pos= g.valInt(dr[9].ToString()),
                    department = dr[10].ToString(),
                    employee = dr[11].ToString()
                });
            }
            dr.Close();
            gg.db.Close();
            if(inf.recordId >= 1 && inf.recordId <= 2)
            {
                tot.details = tot.details.Where(a => a.pos == (int?)inf.recordId).ToList() ;
            }
             
            if (tot.details.Count > 0)
            {

                DataTable dt = new DataTable();
                dt.Columns.Add("id", typeof(string));
                dt.Columns.Add("dat", typeof(string));
                dt.Columns.Add("item", typeof(string));
                dt.Columns.Add("purpose", typeof(string));
                dt.Columns.Add("qty", typeof(string));
                dt.Columns.Add("approved", typeof(string));
                dt.Columns.Add("UOM", typeof(string));
                dt.Columns.Add("ReqdBy", typeof(string));
                dt.Columns.Add("Employee", typeof(string));
                dt.Columns.Add("Department", typeof(string));
                int i = 1;

                foreach (var det in tot.details)
                {
                    dt.Rows.Add(det.seq.Substring(7,5), det.dat, det.matname, det.purpose, det.qty, det.approvedqty, det.uom, det.reqdby, det.employee, det.department);
                    i++;
                }
                List<string> titles = new List<string>();
                titles.Add("Id");
                titles.Add("Date");
                titles.Add("Item");
                titles.Add("Purpose");
                titles.Add("Qty");
                titles.Add("Approved");
                titles.Add("UOM");
                titles.Add("Reqd By");
                titles.Add("Employee");
              //  titles.Add("Department");
                float[] widths = { 50f, 60f, 150f, 150f, 50f, 50f, 50f, 60f, 100f};
                int[] aligns = { 3, 1, 1, 1, 2, 2,2, 3, 1  };
                PDFExcelMake ep = new PDFExcelMake();
                DateTime da = DateTime.Now;
                String filename = inf.usr.uCode + "PUROMREPS" + da.Hour.ToString() + da.Minute.ToString() + da.Second.ToString() + inf.usr.cCode + inf.usr.bCode + ".pdf";

                String msg = "";

                msg = ep.pdfLandscapeConversion(ho.WebRootPath + "\\Reps\\" + filename, "List of Purchase Requests from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
                if (msg == "OK")
                {
                    tot.pdfFile = filename;
                }
                else
                {
                    tot.pdfFile = "";
                }

                filename = inf.usr.uCode + "PUROMREPS" + da.Hour.ToString() + da.Minute.ToString() + da.Second.ToString() + inf.usr.cCode + inf.usr.bCode + ".xlsx";

                msg = ep.makeExcelConversion(ho.WebRootPath + "\\Reps\\" + filename, "List of Purchase Requests from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, false);
                if (msg == "OK")
                {
                    tot.excelFile = filename;
                }
                else
                {
                    tot.excelFile = "";
                }



                return tot;
            }
            else
            {
                return null;
            }
        }

        public ServerDetails getServerDetails()
        {
            return null;
        }

    }
}
