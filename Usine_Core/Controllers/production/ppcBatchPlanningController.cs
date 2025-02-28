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
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;

namespace Usine_Core.Controllers.production
{
    public class PPCBatchPlanningRequirementsInfo
    {
        public List<ProductionPendingSosInfo> orders { get; set; }
        public List<InvMaterialCompleteDetailsView> products { get; set; }
        public dynamic employees { get; set; }
        public List<PpcProcessesMaster> processes { get; set; }
    }
    public class PPCBatchPlanningCompleteInfo
    {
        public PpcBatchPlanningUni header { get; set; }
        public dynamic productinfo { get; set; }
        public dynamic orderinfo { get; set; }
        public dynamic employeeinfo { get; set; }
        public dynamic processinfo { get; set; }
        public string result { get; set; }
    }
    public class PPCBatchPlanningTotal
    {
        public PpcBatchPlanningUni header { get; set; }
        public List<PpcBatchPlanningProcesses> processes { get; set; }
        public List<PpcBatchPlanningEmployeeAssignings> employees { get; set; }
        public List<PpcBatchPlanningSaleOrders> orders { get; set; }
        public int? traCheck { get; set; }
        public UserInfo usr { get; set; }
    }
    public class ppcBatchPlanningController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();


        [HttpPost]
        [Authorize]
        [Route("api/ppcBatchPlanning/GetPPCBatchPlans")]
        public dynamic GetPPCBatchPlans([FromBody] GeneralInformation inf)
        {
            var dat1 = DateTime.Parse(inf.frmDate);
            var dat2 = DateTime.Parse(inf.toDate).AddDays(1);

            return (from a in db.PpcBatchPlanningUni.Where(a => a.Pos >=0 && a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                           join b in db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                           on a.ProductionIncharge equals b.RecordId
                           join c in db.InvMaterials.Where(a => a.CustomerCode ==inf.usr.cCode) on a.ItemId equals c.RecordId
                           select new
                           {
                               recordid = a.RecordId,
                               batchno = a.BatchNo,
                               dat = a.Dat,
                               fromdate = a.FromDate,
                               todate = a.ToDate,
                               productionincharge = b.Empname,
                               product=c.ItemName
                           }).OrderBy(a => a.batchno).ToList();
        }



        private Boolean usageCheck(GeneralInformation inf)
        {
            UsineContext db = new UsineContext();
            var det = db.PpcBatchProcessWiseDetailsDet.Where(a => a.Batchno == inf.recordId && a.Pos != 1 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            if(det==null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        [HttpPost]
        [Authorize]
        [Route("api/ppcBatchPlanning/GetPPCBatchPlan")]
        public PPCBatchPlanningCompleteInfo GetPPCBatchPlan([FromBody] GeneralInformation inf)
        {
            PPCBatchPlanningCompleteInfo tot = new PPCBatchPlanningCompleteInfo();

            if (usageCheck(inf))
            {
                 tot.header = db.PpcBatchPlanningUni.Where(a => a.RecordId == inf.recordId && a.Pos >=0 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                var lst1 = (from a in db.PpcBatchPlanningSaleOrders.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                            join b in db.CrmSaleOrderDet.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on new { soid = a.Soid, sno = a.LineId } equals new { soid = b.RecordId, sno = b.Sno }
                            select new
                            {
                                a.RecordId,
                                a.Sno,
                                a.Soid,
                                lineId = b.Sno,
                                b.ItemId,
                                b.ItemName,
                                b.Qty,

                            }).ToList();
                var lst2 = db.CrmSaleOrderUni.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                tot.orderinfo = (from a in lst1
                                 join b in lst2 on a.Soid equals b.RecordId
                                 select new
                                 {
                                     a.RecordId,
                                     a.Sno,
                                     a.Soid,
                                     a.lineId,
                                     b.PartyName,
                                     b.Seq,
                                     a.ItemId,
                                     a.ItemName,
                                     a.Qty
                                 }).OrderBy(b => b.Sno).ToList();
                tot.employeeinfo = (from a in db.PpcBatchPlanningEmployeeAssignings.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                    join b in db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.Employee equals b.RecordId
                                    join c in db.HrdDesignations.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on b.Designation equals c.RecordId
                                    select new
                                    {
                                        recordid = a.RecordId,
                                        sno = a.Sno,
                                        empid = b.RecordId,
                                        empname = b.Empname,
                                        designation = c.Designation
                                    }).OrderBy(d => d.sno).ToList();
                tot.processinfo = (from a in db.PpcBatchPlanningProcesses.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                   join b in db.PpcProcessesMaster.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.ProcessId equals b.RecordId
                                 //  join c in db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.ProcessIncharge equals c.RecordId
                                   select new
                                   {
                                       recordid = a.RecordId,
                                       sno = a.Sno,
                                       processid = a.ProcessId,
                                       processname = b.ProcessName,
                                       fromdate = a.FromDate,
                                       todate = a.ToDate,
                                       qcrequired = a.QcRequired,
                                       processincharge = a.ProcessIncharge,
                                       //processinchargename = c.Empname
                                   }).OrderBy(d => d.sno).ToList();


                tot.result = "OK";
                
            }
            else
            {
                tot.result = "This batch is in use";
            }

            return tot;

        }

        [HttpPost]
        [Authorize]
        [Route("api/ppcBatchPlanning/GetPPCBatchRequirements")]
        public PPCBatchPlanningRequirementsInfo GetPPCBatchRequirements([FromBody] UserInfo usr)
        {
            PPCBatchPlanningRequirementsInfo tot = new PPCBatchPlanningRequirementsInfo();
            ppcTransactionsController ppct = new ppcTransactionsController();
            tot.products = ppct.GetProductionMassPlanningRequirements(usr);
            tot.orders = ppct.GetPendingSOToBePlanned(usr);
            tot.processes = db.PpcProcessesMaster.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();
            tot.employees = (from a in db.HrdEmployees.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                             join b in db.HrdDesignations.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                             on a.Designation equals b.RecordId
                             select new
                             {
                                 recordid = a.RecordId,
                                 empno = a.Empno,
                                 empname = a.Empname,
                                 mobile = a.Mobile,
                                 designation = b.Designation
                             }).OrderBy(b => b.empname).ToList();
            return tot;
        }
        [HttpPost]
        [Authorize]
        [Route("api/ppcBatchPlanning/SetPPCBatchPlanning")]
        public TransactionResult SetPPCBatchPlanning([FromBody] PPCBatchPlanningTotal tot)
        {
            string msg = "";
            try
            {
                if(ac.screenCheck(tot.usr,10,2,2,(int)tot.traCheck))
                {
                    switch(tot.traCheck)
                    {
                        case 1:
                            
                            using (var txn = db.Database.BeginTransaction())
                            {
                                try
                                {
                                    tot.header.Dat = ac.getPresentDateTime();
                                    tot.header.BranchId = tot.usr.bCode;
                                    tot.header.CustomerCode = tot.usr.cCode;
                                    tot.header.Pos = 1;
                                    db.PpcBatchPlanningUni.Add(tot.header);
                                    db.SaveChanges();
                                    int sno = 1;
                                    foreach(var order in tot.orders)
                                    {
                                        order.RecordId = tot.header.RecordId;
                                        order.Sno = sno;
                                        order.BranchId = tot.usr.bCode;
                                        order.CustomerCode = tot.usr.cCode;
                                        sno++;
                                    }
                                    if(tot.orders.Count() > 0)
                                    {
                                        db.PpcBatchPlanningSaleOrders.AddRange(tot.orders);
                                    }

                                    sno = 1;
                                    foreach(var emp in tot.employees)
                                    {
                                        emp.RecordId = tot.header.RecordId;
                                        emp.Sno = sno;
                                        emp.BranchId = tot.usr.bCode;
                                        emp.CustomerCode = tot.usr.cCode;
                                        sno++;
                                    }
                                    if(tot.employees.Count() > 0)
                                    {
                                        db.PpcBatchPlanningEmployeeAssignings.AddRange(tot.employees);
                                    }
                                    sno = 1;
                                    foreach(var process in tot.processes)
                                    {
                                        process.RecordId = tot.header.RecordId;
                                        process.Sno = sno;
                                        process.BranchId = tot.usr.bCode;
                                        process.CustomerCode = tot.usr.cCode;
                                        sno++;
                                    }
                                    if(tot.processes.Count() > 0)
                                    {
                                        db.PpcBatchPlanningProcesses.AddRange(tot.processes);

                                    }
                                    var proc = tot.processes.FirstOrDefault();
                                    UsineContext db1 = new UsineContext();
                                    if(proc != null)
                                    {
                                        PpcBatchProcessWiseDetailsDet det = new PpcBatchProcessWiseDetailsDet();
                                        det.Batchno = tot.header.RecordId;
                                        det.LineId = 1;
                                        det.Qty = tot.header.Qty;
                                        det.Pos = 1;
                                        det.BranchId = tot.usr.bCode;
                                        det.CustomerCode = tot.usr.cCode;
                                        db.PpcBatchProcessWiseDetailsDet.Add(det);
                                     }
                                    db.SaveChanges();

                                    txn.Commit();
                                    msg = "OK";
                                }
                                catch (Exception ee)
                                {
                                    txn.Rollback();
                                    msg = ee.Message;
                                }
                            }
                                
                            
                            break;
                        case 2:
                            var headerupd = db.PpcBatchPlanningUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            headerupd.BatchNo = tot.header.BatchNo;
                            headerupd.FromDate = tot.header.FromDate;
                            headerupd.ToDate = tot.header.ToDate;
                            headerupd.ProductionIncharge = tot.header.ProductionIncharge;
                            headerupd.ItemId = tot.header.ItemId;
                            headerupd.Qty = tot.header.Qty;

                            var snoupd = db.PpcBatchPlanningSaleOrders.Max(b => b.Sno);
                            snoupd=snoupd==null?0:snoupd;
                            snoupd++;
                            var lineorders = db.PpcBatchPlanningSaleOrders.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                            if(lineorders.Count() > 0)
                            {
                                db.PpcBatchPlanningSaleOrders.RemoveRange(lineorders);
                            }
                            foreach (var order in tot.orders)
                            {
                                order.RecordId = tot.header.RecordId;
                                order.Sno = snoupd;
                                order.BranchId = tot.usr.bCode;
                                order.CustomerCode = tot.usr.cCode;
                                snoupd++;
                            }
                            if (tot.orders.Count() > 0)
                            {
                                db.PpcBatchPlanningSaleOrders.AddRange(tot.orders);
                            }

                            snoupd = db.PpcBatchPlanningEmployeeAssignings.Max(b => b.Sno);
                            snoupd = snoupd == null ? 0 : snoupd;
                            snoupd++;
                            var lineemployees = db.PpcBatchPlanningEmployeeAssignings.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                            if (lineemployees.Count() > 0)
                            {
                                db.PpcBatchPlanningEmployeeAssignings.RemoveRange(lineemployees);
                            }
                            foreach (var emp in tot.employees)
                            {
                                emp.RecordId = tot.header.RecordId;
                                emp.Sno = snoupd;
                                emp.BranchId = tot.usr.bCode;
                                emp.CustomerCode = tot.usr.cCode;
                                snoupd++;
                            }
                            if (tot.employees.Count() > 0)
                            {
                                db.PpcBatchPlanningEmployeeAssignings.AddRange(tot.employees);
                            }

                            snoupd = db.PpcBatchPlanningProcesses.Max(b => b.Sno);
                            snoupd = snoupd == null ? 0 : snoupd;
                            snoupd++;
                            var lineprocesses = db.PpcBatchPlanningProcesses.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                            if (lineprocesses.Count() > 0)
                            {
                                db.PpcBatchPlanningProcesses.RemoveRange(lineprocesses);
                            }
                            foreach (var process in tot.processes)
                            {
                                process.RecordId = tot.header.RecordId;
                                process.Sno = snoupd;
                                process.BranchId = tot.usr.bCode;
                                process.CustomerCode = tot.usr.cCode;
                                snoupd++;
                            }
                            if (tot.processes.Count() > 0)
                            {
                                db.PpcBatchPlanningProcesses.AddRange(tot.processes);

                            }
                            db.SaveChanges();
                            msg = "OK";
                            break;
                        case 3:
                            var headerdel = db.PpcBatchPlanningUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            headerdel.Pos = -1;
                            headerdel.BranchId = headerdel.BranchId + "CANCEL";
                            var lineordersdel = db.PpcBatchPlanningSaleOrders.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                            foreach(var ord in lineordersdel)
                            {
                                ord.BranchId = ord.BranchId + "CANCEL";
                            }
                            var lineemployeesdel = db.PpcBatchPlanningEmployeeAssignings.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                            foreach(var emp in lineemployeesdel )
                            {
                                emp.BranchId = emp.BranchId + "CANCEL";
                            }
                            var lineprocessesdel = db.PpcBatchPlanningProcesses.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                            foreach(var process in lineprocessesdel)
                            {
                                process.BranchId = process.BranchId + "CANCEL";
                            }
                          
                            db.SaveChanges();
                            msg = "OK";
                            break;
                    }
                }
                else
                {
                    msg = "You are not authorised for this transaction";
                }
            }
            catch(Exception ee)
            {
                msg = ee.Message;
            }

            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;
        }
    }
}
