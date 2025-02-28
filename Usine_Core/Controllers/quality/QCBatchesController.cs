using Microsoft.AspNetCore.Mvc;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Usine_Core.Controllers.Others;
using System.Collections.Generic;
using System;
using Usine_Core.Controllers.Purchases;

namespace Usine_Core.Controllers.quality
{
    public class QCBatchRequirements
    {
        public List<PpcProcessesMaster> processes { get; set; }
        public List<QcTestings> tests { get; set; }
        public dynamic pendings { get; set; }
    }
    public class QCProcessTestTotal
    {
        public QcProcessWiseDetails testdet { get; set; }
        public UserInfo usr { get; set; }
    }

    public class ApprovalLineInfo
    {
        public int? batchid { get; set; }
        public int? sno { get; set; }
        public int? processid { get; set; }
        public string processname { get; set; }
        public string statu { get; set; }
        public double? recitfied { get; set; }
        public double? rejected { get; set; }
        public double? rectificationvalue { get; set; }
        public double? rejectedvalue { get; set; }
    }
    public class QCBatchFinalApprovalRequirements
    {
        public dynamic header { get; set; }
        public List<ApprovalLineInfo> lines { get; set; }
    }
    public class QCFinalTotal
    {
        public InvMaterialManagement mgt { get; set; }
        public UserInfo usr { get; set; }
    }
    
    public class QCBatchesController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/QCBatches/GetQCBatchRequirements")]
        public QCBatchRequirements GetQCBatchRequirements([FromBody] UserInfo usr)
        {
            QCBatchRequirements tot = new QCBatchRequirements();
            tot.processes = db.PpcProcessesMaster.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
            tot.tests = db.QcTestings.Where(a => a.TestArea == "PRO" && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
            var pendings = (from a in db.PpcBatchProcessWiseDetailsDet.Where(a => a.Pos == -1 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                       join b in db.PpcBatchPlanningUni.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on a.Batchno equals b.RecordId
                       join c in db.PpcBatchPlanningProcesses.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on new { batch = a.Batchno, sno = a.LineId } equals new { batch = c.RecordId, sno = c.Sno }
                       join d in db.PpcProcessesMaster.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on c.ProcessId equals d.RecordId
                       join e in db.HrdEmployees.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode) on b.ProductionIncharge equals e.RecordId
                       join f in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode) on b.ItemId equals f.RecordId
                       select new
                       {
                           recordid=a.RecordId,
                           batchid=a.Batchno,
                           batchno=b.BatchNo,
                           lineid=a.LineId,
                           qty=a.Qty,
                           processid=d.RecordId,
                           processname=d.ProcessName,
                           productionincharge=e.Empname,
                           itemname=f.Itemname,
                           uom=f.Um
                       }).ToList();
            var lst2 = (from a in db.InvMaterialManagement.Where(a => a.TransactionType == 103 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).GroupBy(b => b.Department)
                        select new
                        {
                            batchno = a.Key,
                            qty = a.Sum(b => b.Qtyout),
                            valu = a.Sum(b => b.Qtyout * b.Rat)
                        }).ToList();
            tot.pendings = (from a in pendings
                            join b in lst2 on a.batchid equals b.batchno into gj
                            from subdet in gj.DefaultIfEmpty()
                            select new
                            {
                                a.recordid,
                                a.batchid,
                                a.batchno,
                                a.lineid,
                                a.qty,
                                a.processid,
                                a.processname,
                                a.productionincharge,
                                a.itemname,
                                a.uom,
                                valu = subdet == null ? 0 : subdet.valu
                            }).OrderBy(b => b.batchid).ToList();

            return tot;
        }


        [HttpPost]
        [Authorize]
        [Route("api/QCBatches/SetQCProcessSet")]
        public TransactionResult SetQCProcessSet([FromBody] QCProcessTestTotal tot)
        {
            string msg = "";
            try
            {
                int? batchno = tot.testdet.BatchNo;
                tot.testdet.BranchId = tot.usr.bCode;
                tot.testdet.CustomerCode = tot.usr.cCode;
                db.QcProcessWiseDetails.Add(tot.testdet);
                UsineContext db1 = new UsineContext();
                var detail = db.PpcBatchProcessWiseDetailsDet.Where(a => a.Batchno == batchno && a.Pos == -1 && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                if(detail != null)
                {
                    detail.Pos = 0;
                }
                
                int? sno = db1.PpcBatchPlanningProcesses.Where(a => a.RecordId == batchno && a.ProcessId == tot.testdet.ProcessId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).Max(b => b.Sno);
                var sno1 = sno + 1;
                var det1 = db1.PpcBatchPlanningProcesses.Where(a => a.RecordId == batchno && a.Sno == sno1 && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                if (det1 != null)
                {
                    PpcBatchProcessWiseDetailsDet rec = new PpcBatchProcessWiseDetailsDet();
                    rec.Batchno = batchno;
                    rec.LineId = sno1;

                    rec.Qty = tot.testdet.SamplesCollected-tot.testdet.Rejected;
                    rec.Pos = 1;
                    rec.BranchId = tot.usr.bCode;
                    rec.CustomerCode = tot.usr.cCode;
                    db.PpcBatchProcessWiseDetailsDet.Add(rec);
                    
                }
                db.SaveChanges();
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
        [Authorize]
        [Route("api/QCBatches/GetFinalProductionQCRequirements")]
        public QCBatchFinalApprovalRequirements GetFinalProductionQCRequirements([FromBody] UserInfo usr)
        {
            QCBatchFinalApprovalRequirements tot = new QCBatchFinalApprovalRequirements();
            var lst1 = (from a in db.PpcBatchPlanningUni.Where(a => a.Pos == 1 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                          join b in db.HrdEmployees.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode) on a.ProductionIncharge equals b.RecordId
                          join c in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode) on a.ItemId equals c.RecordId
                          select new
                          {
                              recordid = a.RecordId,
                              batchno = a.BatchNo,
                              dat = a.Dat,
                              fromdate = a.FromDate,
                              todate = a.ToDate,
                              productionincharge = b.Empname,
                              itemname = c.Itemname,
                              itemid=c.RecordId,
                              qty = a.Qty,
                              um = c.Um
                          }).ToList();
            var lst2 = (from a in db.InvMaterialManagement.Where(a => a.TransactionType == 103 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).GroupBy(b => b.Department)
                        select new
                        {
                            department = a.Key,
                            valu = a.Sum(b => b.Qtyout * b.Rat)
                        }).ToList();
            tot.header = (from a in lst1
                          join b in lst2 on a.recordid equals b.department into gj
                          from subdet
in gj.DefaultIfEmpty()
                          select new
                          {
                              batchid = a.recordid,
                              batchno = a.batchno,
                              dat = a.dat,
                              fromdate = a.fromdate,
                              todate = a.todate,
                              productionincharge = a.productionincharge,
                              itemname = a.itemname,
                              itemid=a.itemid,
                              qty = a.qty,
                              um = a.um,
                              materialcost = subdet == null ? 0 : subdet.valu


                          }).OrderBy(b => b.batchid).ToList();
            string quer = "";
            quer = quer + " select batchid,sno,processid,processname,statu,rectified,rejected,rectificationvalue,rejectedvalue,materialcost from";
            quer = quer + " (select batchid, sno, processid, statu, rectified, rejected, rectificationvalue, rejectedvalue,case when valu is null then 0 else valu end materialcost from";
            quer = quer + " (select a.batchid, a.sno, a.processId, a.statu,case when b.rectified is null then 0 else b.rectified end rectified,";
            quer = quer + " case when b.rejected is null then 0 else b.rejected end rejected, case when b.rectificationValue is null then 0 else b.rectificationValue end rectificationvalue,";
            quer = quer + " case when b.rejectedValue is null then 0 else b.rejectedValue end rejectedvalue from";
            quer = quer + " (select a.recordId batchid, a.sno, a.processId,case when b.pos= 0 then 'Completed' else 'Pending' end statu from";
            quer = quer + " (select * from ppcBatchPlanningProcesses)a left outer join";
            quer = quer + " (select * from ppcBatchProcessWiseDetailsDet)b on a.recordId = b.batchno and a.sno = b.lineId)a left outer join";
            quer = quer + " (select* from qcProcessWiseDetails)b on a.batchid = b.batchno and a.processId = b.processId)a left outer join";
            quer = quer + " (select department, sum(qtyout* rat) valu from invMaterialManagement where transactionType = 103 group by department)b on a.batchid = b.department)a,";
            quer = quer + " (select * from ppcProcessesMaster)b where a.processId = b.recordId order by sno";
            DataBaseContext g = new DataBaseContext();

            SqlCommand dc = new SqlCommand();
            General gg = new General();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            tot.lines = new List<ApprovalLineInfo>();
            while (dr.Read())
            {
                tot.lines.Add(new ApprovalLineInfo
                {
                    batchid = gg.valInt(dr[0].ToString()),
                    sno = gg.valInt(dr[1].ToString()),
                    processid = gg.valInt(dr[2].ToString()),
                    processname = dr[3].ToString(),
                    statu = dr[4].ToString(),
                    recitfied = gg.valNum(dr[5].ToString()),
                    rejected = gg.valNum(dr[6].ToString()),
                    rectificationvalue = gg.valNum(dr[7].ToString()),
                    rejectedvalue = gg.valNum(dr[8].ToString())

                });
            }
            dr.Close();
            g.db.Close();
            //tot.lines()

            return tot;
        }

        [HttpPost]
        [Authorize]
        [Route("api/QCBatches/SetFinalProductionQC")]
        public TransactionResult SetFinalProductionQC([FromBody] QCFinalTotal tot)
        {
            string msg = "";
            try
            {
                var dat = ac.getPresentDateTime();
                UsineContext db1 = new UsineContext();
                General gg = new General();
                string gin = dat.Year.ToString().Substring(2, 2) + gg.zeroMake(dat.Month, 2) + gg.zeroMake(dat.Day, 2);
                int? x = findGin(tot.usr, gin);
                int? y = findId();
                tot.mgt.Store = db1.InvStores.Where(a => a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).Min(b => b.RecordId);
                tot.mgt.TransactionId = y;
                tot.mgt.Sno = y;
                tot.mgt.Gin = gin + gg.zeroMake((int)x, 5);
                tot.mgt.Dat = ac.getPresentDateTime();
                tot.mgt.Manudate = dat;
                tot.mgt.TransactionType = 3;
                tot.mgt.BranchId = tot.usr.bCode;
                tot.mgt.CustomerCode = tot.usr.cCode;
                db.InvMaterialManagement.Add(tot.mgt);
                var det = db.PpcBatchPlanningUni.Where(a => a.RecordId == tot.mgt.ProductBatchNo).FirstOrDefault();
                if(det != null)
                {
                    det.Pos = 0;
                }
                db.SaveChanges();
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
        public int? findGin(UserInfo usr, string gin)
        {
            UsineContext db1 = new UsineContext();
            General g = new General();
            var det = db1.InvMaterialManagement.Where(a => a.Gin.Contains(gin) && a.CustomerCode == usr.cCode).Max(b => b.Gin);
            int x = 0;
            if (det != null)
            {
                x = g.valInt(g.right(det, 5));
            }
            x++;
            return x;
        }
        private int findId()
        {
            int x = 0;
            var det = db.InvMaterialManagement.Where(a => a.TransactionType == 3).Max(b => b.TransactionId);
            if(det != null)
            {
                x = (int)det;
            }
            x++;
            return x;
        }
    }
}
