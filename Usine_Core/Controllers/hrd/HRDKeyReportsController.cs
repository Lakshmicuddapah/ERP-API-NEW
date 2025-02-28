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

namespace Usine_Core.Controllers.HRD
{
    public class HRDKeyReportDetails
    {
        public dynamic details { get; set; }
        public string pdffile { get; set; }
        public string excelfile { get; set; }
    }
    public class HRDKeyReportsController : ControllerBase
    {
        UsineContext db = new  UsineContext();
        AdminControl ac = new AdminControl();
        private readonly IHostingEnvironment ho;
        public HRDKeyReportsController(IHostingEnvironment hostingEnvironment)
        {

            ho = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        [Route("api/HRDKeyReports/GetKeyRepHRDDepartments")]
        public HRDKeyReportDetails GetKeyRepHRDDepartments([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 8, 9, 1, 0))
            {
                HRDKeyReportDetails tot = new HRDKeyReportDetails();
                tot.details = db.HrdDepartments.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.Sno).ToList();


                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("dept", typeof(string));
                dt.Columns.Add("maindept", typeof(string));

                int i = 1;
                General g = new General();
                foreach (var dept in tot.details)
                {
                    dt.Rows.Add(i.ToString(), dept.SGrp,dept.MGrp);
                    i++;
                }
                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Department");
                titles.Add("Main Department");
                float[] widths = { 50f, 300f,200f };
                int[] aligns = { 3, 1 ,1};

                tot.pdffile = usr.uCode + "HRDKEYREPS" + usr.cCode + usr.bCode + ".pdf";
                tot.excelfile = usr.uCode + "HRDKEYREPS" + usr.cCode + usr.bCode + ".xlsx";

                PDFExcelMake ep = new PDFExcelMake();

                ep.pdfConversion(ho.WebRootPath + "\\Reps\\" + tot.pdffile, "List of Departments", usr, dt, titles, widths, aligns, false);
                ep.makeExcelConversion(ho.WebRootPath + "\\Reps\\" + tot.excelfile, "List of Departments", usr, dt, titles, widths, aligns, false);


                return tot;
            }
            else
            {
                return null;
            }
        }



        [HttpPost]
        [Authorize]
        [Route("api/HRDKeyReports/GetKeyRepHRDDesignations")]
        public HRDKeyReportDetails GetKeyRepHRDDesignations([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 8, 9, 1, 0))
            {
                HRDKeyReportDetails tot = new HRDKeyReportDetails();
                tot.details = (from a in db.HrdDesignations.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                               join b in db.HrdDepartments.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode) on a.Department equals b.RecordId
                               select new
                               {
                                   a.RecordId,
                                   a.Designation,
                                   departmentid = b.RecordId,
                                   department = b.SGrp
                               }).OrderBy(c => c.departmentid).ThenBy(d => d.RecordId).ToList();

                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("desig", typeof(string));
                dt.Columns.Add("dept", typeof(string));

                int i = 1;
                General g = new General();
                foreach (var dept in tot.details)
                {
                    dt.Rows.Add(i.ToString(), dept.Designation, dept.department);
                    i++;
                }
                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Designation");
                titles.Add("Department");
                
                float[] widths = { 50f, 300f, 200f };
                int[] aligns = { 3, 1,1 };

                tot.pdffile = usr.uCode + "HRDKEYREPS" + usr.cCode + usr.bCode + ".pdf";
                tot.excelfile = usr.uCode + "HRDKEYREPS" + usr.cCode + usr.bCode + ".xlsx";

                PDFExcelMake ep = new PDFExcelMake();

                ep.pdfConversion(ho.WebRootPath + "\\Reps\\" + tot.pdffile, "List of Designations", usr, dt, titles, widths, aligns, false);
                ep.makeExcelConversion(ho.WebRootPath + "\\Reps\\" + tot.excelfile, "List of Designations", usr, dt, titles, widths, aligns, false);


                return tot;
            }
            else
            {
                return null;
            }
        }


        [HttpPost]
        [Authorize]
        [Route("api/HRDKeyReports/GetKeyRepHRDEmployees")]
        public HRDKeyReportDetails GetKeyRepHRDEmployees([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 8, 9, 1, 0))
            {
                HRDKeyReportDetails tot = new HRDKeyReportDetails();
                tot.details = (from empdepts in (from emps in db.HrdEmployees.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                                                 join depts in db.HrdDepartments.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                                                 on emps.Department equals depts.RecordId
                                                 select new
                                                 {
                                                     emps.RecordId,
                                                     emps.Empno,
                                                     emps.Empname,
                                                     emps.Mobile,
                                                     emps.Email,
                                                     emps.Pic,
                                                     emps.Designation,
                                                     department = depts.SGrp
                                                 }
                           )
                               join desigs in db.HrdDesignations.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                               on empdepts.Designation equals desigs.RecordId
                               select new
                               {
                                   empdepts.RecordId,
                                   empdepts.Empno,
                                   empdepts.Empname,
                                   empdepts.Mobile,
                                   empdepts.Email,
                                   empdepts.department,
                                   desigs.Designation,
                                   empdepts.Pic
                               }).OrderBy(a => a.Empname).ToList();
             

                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("empno", typeof(string));
                dt.Columns.Add("empname", typeof(string));
                dt.Columns.Add("mobile", typeof(string));
                dt.Columns.Add("email", typeof(string));
                dt.Columns.Add("department", typeof(string));
                dt.Columns.Add("designation", typeof(string));

                int i = 1;
                General g = new General();
                foreach (var emp in tot.details)
                {
                    dt.Rows.Add(i.ToString(), emp.Empno, emp.Empname,emp.Mobile,emp.Email,emp.department,emp.Designation);
                    i++;
                }
                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Emp No");
                titles.Add("Emp Name");
                titles.Add("Mobile");
                titles.Add("Email");
                titles.Add("Designation");
                titles.Add("Department");

                float[] widths = { 40f,80f,170f,80f,110f,120f,120f,};
                int[] aligns = { 3, 1,1,1,1,1,1 };

                tot.pdffile = usr.uCode + "HRDKEYREPS" + usr.cCode + usr.bCode + ".pdf";
                tot.excelfile = usr.uCode + "HRDKEYREPS" + usr.cCode + usr.bCode + ".xlsx";

                PDFExcelMake ep = new PDFExcelMake();

                ep.pdfLandscapeConversion(ho.WebRootPath + "\\Reps\\" + tot.pdffile, "List of Employees", usr, dt, titles, widths, aligns, false);
                ep.makeExcelConversion(ho.WebRootPath + "\\Reps\\" + tot.excelfile, "List of Employees", usr, dt, titles, widths, aligns, false);


                return tot;
            }
            else
            {
                return null;
            }
        }
    }
}
