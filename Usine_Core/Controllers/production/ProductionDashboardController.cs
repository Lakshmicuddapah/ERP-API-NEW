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

namespace Usine_Core.Controllers.production
{
    public class DashBoardDetails
    {
        public int? sno { get; set; }
        public string descr { get; set; }
        public string det1 { get; set; }
        public string det2 { get; set; }
        public string det3 { get; set; }
        public string det4 { get; set; }
        public double? fir { get; set; }
        public double? sec { get; set; }
        public int? typ { get; set; }
    }
    public class ProductionDashboardController : ControllerBase
    {
        AdminControl ac = new AdminControl();
        UsineContext db = new UsineContext();

        [HttpPost]
        [Authorize]
        [Route("api/ProductionDashboard/getProductionDashBoardDetails")]
        public List<DashBoardDetails> getProductionDashBoardDetails([FromBody] UserInfo usr)
        {
            List<DashBoardDetails> lst = new List<DashBoardDetails>();
            ProductionGeneral pg = new ProductionGeneral();

            var det = db.PpcBatchPlanningUni.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Count();
            lst.Add(new DashBoardDetails
            {
                sno=1,
                descr="Running Batches",
                fir=det,
                typ=1
            });
            lst.Add(new DashBoardDetails
            {
                sno = 2,
                descr = "Production Material Cost",
                fir = pg.GetMaterialsUsedForPresentBatches(usr).Sum(b => b.valu),
                typ = 2
            });
            lst.Add(new DashBoardDetails
            {
                sno = 3,
                descr = "Pending Processes",
                fir = pg.pendingProcessList(usr).Count(),
                typ = 3
            });
            var materials = pg.GetMaterialsUsedForPresentBatches(usr);
            lst.Add(new DashBoardDetails
            {
                sno = 4,
                descr = "Material estimation",
                fir = materials.Sum(a => a.valu),
                typ = 4
            });

            var dets = pg.pendingProcessList(usr);
            int sno = 101;
            foreach (var de in dets)
            {
                lst.Add(new DashBoardDetails
                {
                    det1 = de.batchno,
                    det2 = de.itemname,
                    fir = de.qty,
                    det3 = de.um,
                    det4 = de.process,
                    typ=sno
                });
                sno++;
            }
            sno = 201;
            var detss = (from a in db.PpcBatchPlanningUni.Where(a => a.Pos == 1 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                         join b in db.HrdEmployees.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode) on a.ProductionIncharge equals b.RecordId
                         join c in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode) on a.ItemId equals c.RecordId
                         select new DashBoardDetails
                         {
                             det1 = a.BatchNo,
                             det2 = b.Empname,
                             det3 = c.Itemname,
                             fir = a.Qty,
                             det4 = c.Um,
                             typ=sno
                         }).ToList();
            
            if(detss.Count() > 0)
            {
                foreach(var de in detss)
                {
                    lst.Add(de);
                }
            }

            return lst;
        }
    }
}
