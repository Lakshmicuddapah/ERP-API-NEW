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


namespace Usine_Core.Controllers.Maintenance
{
    public class MaiEquipKeyReportDetails
    {
        public dynamic details { get; set; }
        public string pdffile { get; set; }
        public string excelfile { get; set; }
    }
    public class MaiEquipKeyReportsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        private readonly IHostingEnvironment ho;
        public MaiEquipKeyReportsController(IHostingEnvironment hostingEnvironment)
        {
            ho = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        [Route("api/MaiEquipKeyReports/GetKeyRepMaiEquipmentGroups")]
        public MaiEquipKeyReportDetails GetKeyRepMaiEquipmentGroups([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 9, 9, 1, 0))
            {
                MaiEquipKeyReportDetails tot = new MaiEquipKeyReportDetails();
                 
                tot.details = (from a in db.MaiEquipGroups.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode && a.Chk == 1)
                            join b in db.MaiEquipGroups.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                            on a.MGrp equals b.RecordId
                            select new 
                            {
                                recordID = a.RecordId,
                                subGroup = a.SGrp,
                                mainGroup = b.SGrp,
                                sno = a.Sno,
                                chk = a.Chk,
                                statu = a.Statu == 1 ? "Active" : "Inactive"
                            });


                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("subgroup", typeof(string));
                dt.Columns.Add("maingroup", typeof(string));
                dt.Columns.Add("statu", typeof(string));

                int i = 1;
                General g = new General();
                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i.ToString(),det.subGroup,det.mainGroup,det.statu);
                    i++;
                }
                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Sub Group");
                titles.Add("Main Group");
                titles.Add("Status");
                float[] widths = { 50f, 200f,200,100 };
                int[] aligns = { 3, 1,1,1 };

                tot.pdffile = usr.uCode + "MAIKEYREPS" + usr.cCode + usr.bCode + ".pdf";
                tot.excelfile = usr.uCode + "MAIKEYREPS" + usr.cCode + usr.bCode + ".xlsx";

                PDFExcelMake ep = new PDFExcelMake();

                ep.pdfConversion(ho.WebRootPath + "\\Reps\\" + tot.pdffile, "List of Equipment Groups", usr, dt, titles, widths, aligns, false);
                ep.makeExcelConversion(ho.WebRootPath + "\\Reps\\" + tot.excelfile, "List of Equipment Groups", usr, dt, titles, widths, aligns, false);


                return tot;
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/MaiEquipKeyReports/GetKeyRepMaiEquipmentDetails")]
        public MaiEquipKeyReportDetails GetKeyRepMaiEquipmentDetails([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 9, 9, 1, 0))
            {
                MaiEquipKeyReportDetails tot = new MaiEquipKeyReportDetails();

                tot.details = (from a in db.MaiEquipment.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                               join b in db.MaiEquipGroups.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                               on a.EquipmentGroup equals b.RecordId
                               select new
                               {
                                   a.RecordId,
                                   a.EquipmentCode,
                                   a.EquipmentName,
                                   b.SGrp
                                  }).OrderBy(b => b.EquipmentName).ToList();


                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("equipcode", typeof(string));
                dt.Columns.Add("equipment", typeof(string));
                dt.Columns.Add("grp", typeof(string));

                int i = 1;
                General g = new General();
                foreach (var det in tot.details)
                {
                    dt.Rows.Add(i.ToString(), det.EquipmentCode, det.EquipmentName, det.SGrp);
                    i++;
                }
                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Equip Code");
                titles.Add("Equipment");
                titles.Add("Group");
                float[] widths = { 50f, 100f,200f, 200};
                int[] aligns = { 3, 1, 1, 1 };

                tot.pdffile = usr.uCode + "MAIKEYREPS" + usr.cCode + usr.bCode + ".pdf";
                tot.excelfile = usr.uCode + "MAIKEYREPS" + usr.cCode + usr.bCode + ".xlsx";

                PDFExcelMake ep = new PDFExcelMake();

                ep.pdfConversion(ho.WebRootPath + "\\Reps\\" + tot.pdffile, "List of Equipment", usr, dt, titles, widths, aligns, false);
                ep.makeExcelConversion(ho.WebRootPath + "\\Reps\\" + tot.excelfile, "List of Equipment", usr, dt, titles, widths, aligns, false);


                return tot;
            }
            else
            {
                return null;
            }
        }

    }
}
