using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Controllers.Admin;
 
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
 using Microsoft.AspNetCore.Hosting;
using Usine_Core.Controllers.Others;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.Inventory
{
    public class InvDepartmentsTotal
    {
        public InvDepartments dept { get; set; }
        public int? tracheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
   
    public class ClosingInfo
    {
        public string itemname { get; set; }
        public string grpname { get; set; }
        public string ostock { get; set; }
        public string issue { get; set; }
        public string total { get; set; }
        public string sale { get; set; }
        public string others { get; set; }
        public string closing { get; set; }
    }
    public class ClosingInfoReport
    {
        public List<ClosingInfo> details { get; set; }
        public string pdfFile { get; set; }
        public string excelFile { get; set; }

    }
    public class invDepartmentsController : Controller 
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        private readonly IHostingEnvironment ho;
        public invDepartmentsController(IHostingEnvironment hostingEnvironment)
        {

            ho = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        [Route("api/invDepartments/GetInvDepartments")]
        public List<InvDepartments> GetInvDepartments([FromBody]UserInfo usr)
        {
            return db.InvDepartments.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.Department).ToList();
        }
        [HttpPost]
        [Authorize]
        [Route("api/invDepartments/SetInvDepartment")]
        public InvDepartmentsTotal SetInvDepartment([FromBody] InvDepartmentsTotal tot)
        {
            string msg = "";
            try
            {
                if(ac.screenCheck(tot.usr,3,1,6,(int)tot.tracheck))
                {


                    if (dupDepartmentCheck(tot))
                    {
                        switch (tot.tracheck)
                        {
                            case 1:
                                InvDepartments deptcre = new InvDepartments();
                                deptcre.Department = tot.dept.Department;
                                deptcre.Area = tot.dept.Area;
                                deptcre.BranchId = tot.usr.bCode;
                                deptcre.CustomerCode = tot.usr.cCode;
                                db.InvDepartments.Add(deptcre);
                                db.SaveChanges();
                                msg = "OK";
                                break;
                            case 2:
                                var deptupd = db.InvDepartments.Where(a => a.RecordId == tot.dept.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (deptupd != null)
                                {
                                    deptupd.Department = tot.dept.Department;
                                    deptupd.Area = tot.dept.Area;
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "Record not found";
                                }
                                break;
                            case 3:
                                var deptdel = db.InvDepartments.Where(a => a.RecordId == tot.dept.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (deptdel != null)
                                {
                                    db.InvDepartments.Remove(deptdel);
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "Record not found";
                                }
                                break;

                        }
                    }
                    else
                    {
                        msg = "You are not authorised for this transaction";
                    }
                }
                else
                {
                    if(tot.tracheck==3)
                    {
                        msg = "This department is in use deletion is not possible";
                    }
                    else
                    {
                        msg = "This department is already existed";
                    }
                }
            }
            catch(Exception ee)
            {
                msg = ee.Message;
            }
            tot.result = msg;
            return tot;
          }

        public Boolean dupDepartmentCheck(InvDepartmentsTotal tot)
        {
            Boolean b = false;
            switch(tot.tracheck)
            {
                case 1:
                    var cre = db.InvDepartments.Where(a => a.Department == tot.dept.Department && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(cre == null)
                    {
                        b = true;
                    }
                    break;
                case 2:
                    var upd = db.InvDepartments.Where(a => a.Department == tot.dept.Department && a.RecordId != tot.dept.RecordId  && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (upd == null)
                    {
                        b = true;
                    }
                    break;
                case 3:
                    var del = db.InvMaterialManagement.Where(a => a.Department == tot.dept.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (del == null)
                    {
                        b = true;
                    }
                    break;
            }
            return b;
        }

        /*
        [HttpPost]
        [Authorize]
        [Route("api/invDepartments/GetInvLossesTransactions")]
        public List<InvMaterialManagement> GetInvLossesTransactions([FromBody] UserInfo usr)
        {
            DateTime dat1, dat2;
            dat1 = DateTime.Parse(ac.strDate(DateTime.Now));
            dat2 = dat1.AddDays(1);
            return (from l in (from p in (from a in db.InvMaterialManagement.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.TransactionType == 14 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                                              join b in db.InvMaterials.Where(a => a.CustomerCode == usr.cCode) on a.ItemName equals b.RecordId
                                              select new
                                              {
                                                  a.TransactionId,
                                                  a.Sno,
                                                  a.Gin,
                                                  b.RecordId,
                                                  b.ItemName,
                                                  a.Qtyout,
                                                  a.Stdum,
                                                  a.Department
                                              }
                        )
                                   join
                                 q in db.InvLosses.Where(a => a.CustomerCode == usr.cCode) on p.Department equals q.RecordId
                                   select new
                                   {
                                       p.TransactionId,
                                       p.Sno,
                                       p.Gin,
                                       p.RecordId,
                                       p.ItemName,
                                       p.Qtyout,
                                       p.Stdum,
                                       q.LossName
                                   })
                        join m in db.InvUm.Where(a => a.CustomerCode == usr.cCode)
                     on l.Stdum equals m.RecordId
                        select new InvMaterialManagement
                        {
                          TransactionId=  l.TransactionId,
                           Sno= l.Sno,
                           Gin= l.Gin,
                           ItemName= l.RecordId,
                           Descr= l.ItemName,
                           Qtyout= l.Qtyout,
                           BatchNo= m.Um,
                           BranchId= l.LossName
                        }).OrderBy(b => b.TransactionId).ToList();

         }
        [HttpPost]
        [Authorize]
        [Route("api/invDepartments/GetInvIssuesDepartment")]
        public dynamic GetInvIssuesDepartment([FromBody] UserInfo usr)
        {
            DateTime dat1, dat2;
            dat1 = DateTime.Parse(ac.strDate(DateTime.Now));
            dat2 = dat1.AddDays(1);
            return   (from p in (from a in db.InvMaterialManagement.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.TransactionType == 101 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                                          join b in db.InvMaterials.Where(a => a.CustomerCode == usr.cCode) on a.ItemName equals b.RecordId
                                          select new
                                          {
                                              a.TransactionId,
                                              a.Sno,
                                              a.Gin,
                                              b.RecordId,
                                              b.ItemName,
                                              a.Qtyout,
                                              a.Stdum,
                                              a.Department,
                                              a.Descr,
                                              a.BatchNo
                                             
                                          }
                        )
                               join
                             q in db.InvDepartments.Where(a => a.CustomerCode == usr.cCode) on p.Department equals q.RecordId
                               select new
                               {
                                   p.TransactionId,
                                   p.Sno,
                                   p.Gin,
                                   p.RecordId,
                                   p.ItemName,
                                   p.Qtyout,
                                   p.Descr,
                                   FnbCode=q.Department,
                                   p.BatchNo
                               }).OrderBy(b => b.TransactionId).ToList();

        }

        public TransactionResult SetInvIssue()
        {
            string msg = "";




            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;
        }


        [HttpPost]
        [Authorize]
        [Route("api/invDepartments/GetMaterialCompleteDetails")]
        public string GetMaterialCompleteDetails([FromBody]UserInfo usr)
        {
             DateTime dat1 = DateTime.Parse("1-Apr-23");
                DateTime dat2 = DateTime.Parse("31-Mar-24");


            string quer = "";
            quer = quer + " select a.recordId,a.itemname,a.grpname,a.umid,a.um,case when b.qty is null then 0 else b.qty end qty from";
            quer = quer + " (select * from InvMaterialCompleteDetails_View where customerCode = " + usr.cCode + ")a left outer join";
            quer = quer + " (select itemname, sum(qtyin-qtyout) qty from invmaterialmanagement where customerCode = " + usr.cCode + " group by itemname)b on a.recordId = b.itemname";
            DataBaseContext g = new DataBaseContext();
            General gg = new General();
            SqlCommand dc = new SqlCommand();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            List<InvMaterials> list = new List<InvMaterials>();
            while(dr.Read())
            {
                list.Add(new InvMaterials
                {
                    RecordId=gg.valInt(dr[0].ToString()),
                    ItemName=dr[1].ToString(),
                    Itemid=dr[2].ToString(),
                    InventoryReqd=gg.valInt(dr[3].ToString()),
                    Pic=dr[4].ToString(),
                    ReOrderQty=gg.valNum(dr[5].ToString()),

                });
            }
            dr.Close();
            g.db.Close();
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(list);
            return JSONString;


        }


        [HttpGet]
        [Route("api/invDepartments/GetClosingStocks")]
        public DataTable GetClosingStocks()
        {
            string quer = "";

            quer = quer + " select b.recordId,b.ItemName,b.sGrp,qty,round(valu, 2) valu from";
            quer = quer + " (select itemname, sum(qty) qty, sum(qty* rat) valu from";
            quer = quer + " (select a.gin, itemname, qty, rat from";
            quer = quer + " (select gin, itemname, sum(qtyin-qtyout) qty from invMaterialManagement where dat >= '1-Apr-22' and dat< '12-Jul-22' and branchId = 'E001' and customerCode = 11115 group by gin,itemname)a,";
            quer = quer + " (select gin, rat from invMaterialManagement where dat >= '1-Apr-22' and transactionType< 10 and branchId = 'E001' and customerCode = 11115  )b where a.gin = b.gin)x group by itemname)a,";
            quer = quer + " (select a.recordId, a.itemname,b.sGrp from invMaterials a, invGroups b where a.grp = b.recordId)b where a.itemname = b.recordId ";

            DateTime dat1, dat2;


          //  var list1=(from a in db.InvBlanksManagement.Where(a => a.))

            DataBaseContext g = new DataBaseContext();
            SqlCommand dc = new SqlCommand();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            da.SelectCommand = dc;
            da.Fill(ds);
            if(ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
            
        }

        private int? findId()
        {
            CompleteContext db = new CompleteContext();
            var x = db.InvMaterialManagement.Where(a => a.TransactionType == 101).Max(b => b.TransactionId);
            if(x==null)
            {
                x = 0;
            }
            x++;
            return x;

        }

        [HttpPost]
        [Route("api/invDepartments/SetIssue")]
        public TransactionResult SetIssue([FromBody] ItemIssuesTotal tot)
        {
            string msg = "";
            try
            {
                switch (tot.traCheck)
                {
                    case 1:
                        InvMaterialManagement mgt = new InvMaterialManagement();

                        mgt.TransactionId = findId();
                            mgt.Sno = 1;
                        // Gin = gin + gg.zeroMake((int)x, 5),
                        mgt.ItemName = tot.line.ItemId;
                        mgt.Dat = ac.getPresentDateTime();
                        //   BatchNo = line.Batchno,
                        //   Manudate = line.Manudate,
                        //   Expdate = line.Expdate,
                        //    Store = line.Store,
                       mgt.Qtyin = 0;
                        mgt.BatchNo = tot.line.Mrp.ToString();
                        mgt.Qtyout = tot.line.Qty * tot.line.Mrp;
                        mgt.Rat = 0;
                        mgt.Descr = tot.line.BranchId;
                            mgt.TransactionType = 101;
                        mgt.Department = tot.line.CustomerCode;
                        mgt.BranchId = tot.usr.bCode;
                             mgt.CustomerCode = tot.usr.cCode;
                        db.InvMaterialManagement.Add(mgt);
                        db.SaveChanges();
 
                        break;
                    case 3:
                        var det = db.InvMaterialManagement.Where(a => a.TransactionId == tot.line.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                        if(det != null)
                        {
                            db.InvMaterialManagement.Remove(det);
                            db.SaveChanges();
                        }    
                        break;
                }
                msg = "OK";
            }
            catch(Exception ee)
            {
                msg = ee.Message;
            }
            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;
        }


        [HttpPost]
        [Route("api/invDepartments/GetFnBWiseClosingStock")]
        public ClosingInfoReport GetFnBWiseClosingStock([FromBody] GeneralInformation inf)
        {
            var dat1 = inf.frmDate;
            var dat2 = ac.strDate(DateTime.Parse(dat1).AddDays(1));
            string quer = "";
            quer = quer + " select itemname, grpname,";
            quer = quer + " convert(varchar(10), floor(ostock / conversionfactor)) +' - ' + convert(varchar(10), ostock % conversionfactor) + '' ostock,";
            quer = quer + " convert(varchar(10), floor(issue / conversionfactor)) + ' - ' + convert(varchar(10), issue % conversionfactor) + '' issue,";
            quer = quer + " convert(varchar(10), floor(total / conversionfactor)) + ' - ' + convert(varchar(10), total % conversionfactor) + '' total,";
            quer = quer + " convert(varchar(10), floor(sale / conversionfactor)) + ' - ' + convert(varchar(10), sale % conversionfactor) + '' sale,";
            quer = quer + " convert(varchar(10), floor(closing / conversionfactor)) + ' - ' + convert(varchar(10), closing % conversionfactor) + '' closing";
            quer = quer + " from(select itemname, grpname,";
            quer = quer + " convert(int, ostock) ostock, convert(int, issue) issue, convert(int, ostock + issue) total, convert(int, sale) sale, convert(int, ostock + issue - sale) closing, convert(int, conversionfactor) conversionfactor from";
            quer = quer + " (select b.recordId, b.itemname, b.grpname,case when a.ostock is null then 0 else ostock end ostock,case when issue is null then 0 else issue end issue";
            quer = quer + " ,case when sale is null then 0 else sale end sale from";
            quer = quer + " (select case when a.itemname is null then b.itemname else a.itemname end itemname,case when ostock is null then 0 else ostock end ostock,";
            quer = quer + " case when issue is null then 0 else issue end issue,case when sale is null then 0 else sale end sale from";
            quer = quer + " (select case when a.itemname is null then b.itemname else a.itemname end itemname,case when ostock is null then 0 else ostock end ostock,";
            quer = quer + " case when issue is null then 0 else issue end issue from";
            quer = quer + " (select a.itemname, a.qtyout-b.sale ostock,a.department from";
            quer = quer + " (select itemname, sum(qtyout) qtyout, department from invMaterialManagement where transactionType = 101 and department = " + inf.recordId + " and dat < '" + dat1 + "' and branchid = 'E001' and customerCode = 12011 group by itemname,department)a,";
            quer = quer + " (select b.ingredient,a.itemname,a.qty* b.qty sale from";
            quer = quer + " (select itemname, sum(qty) qty from possalesdet where restacode in";
            quer = quer + " (select restaCode from resFNBServiceWiseResta where branchid = 'E001' and customerCode = 12011 and fnbcode = " + inf.recordId + ") and dat<  '" + dat1 + "' and branchid = 'E001' and customerCode = 12011";
            quer = quer + " group by itemname)a,";
            quer = quer + " (select * from proItemWiseIngredients where branchid = 'E001' and customerCode = 12011)b where a.itemname = b.itemid)b where a.itemname = b.ingredient)a full outer join";
            quer = quer + " (select itemname, sum(qtyout) issue from invMaterialManagement where transactionType = 101 and department = " + inf.recordId + " and dat >= '" + dat1 + "' and dat< '" + dat2 + "' and branchid = 'E001' and customerCode = 12011 group by itemname,department)b on a.itemname = b.itemname)a full outer join";
            quer = quer + " (select b.ingredient itemname, a.qty sale  from";
            quer = quer + " (select itemname, sum(qty) qty from possalesdet where restacode in";
            quer = quer + " (select restaCode from resFNBServiceWiseResta where branchid= 'E001' and customerCode = 12011 and fnbcode = " + inf.recordId + ") and dat >= '" + dat1 + "' and dat< '" + dat2 + "' and branchid = 'E001' and customerCode = 12011";
            quer = quer + " group by itemname)a,";
            quer = quer + " (select * from proItemWiseIngredients where branchid = 'E001' and customerCode = 12011)b where a.itemname = b.ingredient)b on a.itemname = b.itemname)a right outer join";
            quer = quer + " (select* from invMaterialCompleteDetails_view where customerCode= 12011)b on a.itemname = b.recordId)a,";
            quer = quer + " (select * from invMaterialUnits where customerCode = 12011 and sno = 1)b where a.recordId = b.recordId)x";
            DataBaseContext g = new DataBaseContext();
            SqlCommand dc = new SqlCommand();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            ClosingInfoReport tot = new ClosingInfoReport();
            tot.details = new List<ClosingInfo>();
            while(dr.Read())
            {
                tot.details.Add(new ClosingInfo
                {
                    itemname=dr[0].ToString(),
                    grpname=dr[1].ToString(),
                    ostock=dr[2].ToString(),
                    issue=dr[3].ToString(),
                    total=dr[4].ToString(),
                    sale=dr[5].ToString(),
                    others="0 - 0",
                    closing=dr[6].ToString()
                });
            }
            dr.Close();
            g.db.Close();


            DataTable dt = new DataTable();
            dt.Columns.Add("sno", typeof(string));
            dt.Columns.Add("item", typeof(string));
            dt.Columns.Add("group", typeof(string));
            dt.Columns.Add("ostock", typeof(string));
            dt.Columns.Add("issue", typeof(string));
            dt.Columns.Add("total", typeof(string));
            dt.Columns.Add("sale", typeof(string));
            dt.Columns.Add("others", typeof(string));
            dt.Columns.Add("closing", typeof(string));
            int i = 1;

            foreach (var det in tot.details)
            {
                dt.Rows.Add(i.ToString(), det.itemname,det.grpname,det.ostock,det.issue,det.total,det.sale,det.others,det.closing );
                i++;
            }

            List<string> titles = new List<string>();
            titles.Add("#");
            titles.Add("Item");
            titles.Add("Group");
            titles.Add("Opening");
            titles.Add("Receipt");
            titles.Add("Total");
            titles.Add("Sale");
            titles.Add("Others");


            float[] widths = { 40f,190f, 140f, 70f, 70f, 70f, 70f, 70f};
            int[] aligns = { 3, 1, 1, 1 ,1,1,1,1};
            PDFExcelMake ep = new PDFExcelMake();

            DateTime dats = DateTime.Now;
            string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
            string fname = inf.usr.uCode + "Closings" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
            string filename = ho.WebRootPath + "\\Reps\\" + fname;
            string msg = "";

            msg = ep.pdfConversion(filename, "Stock information as on " + dat1, inf.usr, dt, titles, widths, aligns, false);
            if (msg == "OK")
            {
                tot.pdfFile = fname;
            }
            string fname1 = inf.usr.uCode + "Closings" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
            string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
            msg = "";
            msg = ep.makeExcelConversion(filename, "Stock information as on " + dat1, inf.usr, dt, titles, widths, aligns, false);
            if (msg == "OK")
            {
                tot.excelFile = fname1;
            }
            return tot;


        }*/

    }
}
