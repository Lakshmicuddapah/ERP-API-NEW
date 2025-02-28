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
    public class ProductionProcessInformation
    {
        public dynamic header { get; set; }
        public dynamic lines { get; set; }
        public string result { get; set; }
    }
    public class ProductionProcessTotal
    {
        public PpcBatchProcessWiseDetailsUni header { get; set; }
        public List<PpcBatchProcessWiseDetailsDet> lines { get; set; }
        public int? traCheck { get; set; }
        public UserInfo usr { get; set; }
    }

    public class ppcProductionController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();


        [HttpPost]
        [Authorize]
        [Route("api/ppcProduction/ppcShowPendingProcesses")]
        public dynamic ppcShowPendingProcesses([FromBody] UserInfo usr)
        {
         
            var lst1 = db.PpcBatchProcessWiseDetailsDet.Where(a => a.Pos == 1 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
            var lst2 = (from a in db.PpcBatchPlanningProcesses.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                        join b in db.PpcProcessesMaster.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on a.ProcessId equals b.RecordId
                        select new
                        {
                            batchid = a.RecordId,
                            sno = a.Sno,
                            processid = a.ProcessId,
                            processname = b.ProcessName
                        }).ToList();

            var lst3 = (from a in db.PpcBatchPlanningUni.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode && a.Pos == 1)
                        join b in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode) on a.ItemId equals b.RecordId
                        join c in db.HrdEmployees.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode) on a.ProductionIncharge equals c.RecordId
                        select new
                        {
                            batchid = a.RecordId,
                            batchno = a.BatchNo,
                            dat = a.Dat,
                            productionincharge = c.Empname,
                            itemid = a.ItemId,
                            itemname = b.Itemname,
                            um = b.Um
                        }).ToList();


            var detss= (from a in lst1
                       join b in lst2 on new { batchno = a.Batchno, sno = a.LineId } equals new { batchno = b.batchid, sno = b.sno }
                       join c in lst3 on a.Batchno equals c.batchid
                       select new
                       {
                           recordid = a.RecordId,
                           batchid = a.Batchno,
                           batchno = c.batchno,
                           sno = a.LineId,
                           processid = b.processid,
                           processname = b.processname,
                           productionincharge = c.productionincharge,
                           itemid = c.itemid,
                           itemname = c.itemname,
                           qty=a.Qty,
                           um = c.um

                       }).OrderBy(a => a.recordid).ToList();

            return detss;
           
        }

        [HttpPost]
        [Authorize]
        [Route("api/ppcProduction/getTodaysProductionDetails")]
        public dynamic getTodaysProductionDetails([FromBody] GeneralInformation inf)
        {
            var dat1 = DateTime.Parse(inf.frmDate);
            var dat2 = DateTime.Parse(inf.toDate);
            return (from a in db.PpcBatchProcessWiseDetailsUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                    join b in db.PpcProcessesMaster.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.ProcessId equals b.RecordId
                    join c in db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.ProcessIncharge equals c.RecordId
                    select new
                    {
                        recordid=a.RecordId,
                        jobcard=a.JobCardNo,
                        processid=b.RecordId,
                        processname=b.ProcessName,
                        processincharge=c.Empname

                    }).OrderBy(b =>b.recordid ).ToList();
        }

        private Boolean nextUsage(GeneralInformation inf)
        {
            var det = db.PpcBatchProcessWiseDetailsDet.Where(a => a.JobCardNo == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode && a.Pos != 1).FirstOrDefault();
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
        [Route("api/ppcProduction/getTodaysProductionDetail")]
        public ProductionProcessInformation getTodaysProductionDetail([FromBody] GeneralInformation inf)
        {
            ProductionProcessInformation tot = new ProductionProcessInformation();
            string msg = "";
            if(nextUsage(inf))
            {
                tot.header= (from a in db.PpcBatchProcessWiseDetailsUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                           join b in db.PpcProcessesMaster.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.ProcessId equals b.RecordId
                           join c in db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.ProcessIncharge equals c.RecordId
                           select new
                           {
                               recordid=a.RecordId,
                               seq=a.JobCardNo,
                               dat=a.Dat,
                               processid=a.ProcessId,
                               processname=b.ProcessName,
                               processincharge=c.Empname
                               
                           }).FirstOrDefault();

                tot.lines = (from a in db.PpcBatchProcessWiseDetailsDet.Where(a => a.JobCardNo == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                             join b in db.PpcBatchPlanningUni.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.Batchno equals b.RecordId
                             join c in db.PpcBatchPlanningProcesses.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on new { batchno = a.Batchno, sno = a.LineId } equals new { batchno = c.RecordId, sno = c.Sno }
                             join d in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == inf.usr.cCode) on b.ItemId equals d.RecordId
                             select new
                             {
                                 recordid = a.JobCardNo,
                                 batchid = a.Batchno,
                                 sno = a.LineId,
                                 batchno = b.BatchNo,
                                 itemid = b.ItemId,
                                 itemname = d.Itemname,
                                 um = d.Um,
                                 qty = a.Qty
                             }).OrderBy(b => b.recordid).ThenBy(c => c.sno).ToList();

                msg = "OK";
            }
            else
            {
                msg = "This next processes are completed already";
            }
            tot.result = msg;
            return tot;
        }
        [HttpPost]
        [Authorize]
        [Route("api/ppcProduction/SetProductionProcess")]
    
        public TransactionResult SetProductionProcess([FromBody] ProductionProcessTotal tot)
        {
            string msg = "";
            try
            {
                if(ac.screenCheck(tot.usr,10,2,3,(int)tot.traCheck))
                    {
                    switch(tot.traCheck)
                    {
                        case 1:
                            tot.header.Dat = ac.getPresentDateTime();
                            tot.header.JobCardNo = findSeq(tot.usr);
                            tot.header.BranchId = tot.usr.bCode;
                            tot.header.CustomerCode = tot.usr.cCode;
                            db.SaveChanges();
                            foreach(var line in tot.lines)
                            {
                                var lin = db.PpcBatchProcessWiseDetailsDet.Where(a => a.Batchno == line.Batchno && a.LineId == line.LineId).FirstOrDefault();

                                lin.JobCardNo = tot.header.RecordId;
                                lin.Pos = findPos(line.Batchno, line.LineId,tot.header.Qty, tot.usr);
                                db.SaveChanges();
                            }
                            
                            msg = "OK";
                            break;
                        case 3:
                            var headerdel = db.PpcBatchProcessWiseDetailsUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            var linesdel = db.PpcBatchProcessWiseDetailsDet.Where(a => a.JobCardNo == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                            List<PpcBatchProcessWiseDetailsDet> nextlines = new List<PpcBatchProcessWiseDetailsDet>();
                            foreach(var lin in tot.lines)
                            {
                                var nextline = db.PpcBatchProcessWiseDetailsDet.Where(a => a.Batchno == lin.Batchno && a.LineId == lin.LineId + 1 && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if(nextline != null)
                                {
                                    nextlines.Add(nextline);
                                }
                                var line= db.PpcBatchProcessWiseDetailsDet.Where(a => a.Batchno == lin.Batchno && a.LineId == lin.LineId  && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if(line != null)
                                {
                                    line.Pos = 1;
                                    line.JobCardNo = null;
                                }
                            }
                            if (nextlines.Count() > 0)
                            {
                                db.PpcBatchProcessWiseDetailsDet.RemoveRange(nextlines);
                               
                                
                            }
                           
                            if(headerdel != null)
                            {
                                db.PpcBatchProcessWiseDetailsUni.Remove(headerdel);
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
        private int? findPos(int? batchno,int? sno,double? qty, UserInfo usr)
        {
            UsineContext db = new UsineContext();
            int? pos = 0;
            var det = db.PpcBatchPlanningProcesses.Where(a => a.RecordId == batchno && a.Sno == sno && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).FirstOrDefault();
            if(det != null)
            {
                pos = det.QcRequired == 1 ? -1 : 0;
            }
            int? sno1 = sno + 1;
            if (pos == 0)
            {
                var det1 = db.PpcBatchPlanningProcesses.Where(a => a.RecordId == batchno && a.Sno == sno1 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).FirstOrDefault();
                if(det1 != null)
                {
                    PpcBatchProcessWiseDetailsDet rec = new PpcBatchProcessWiseDetailsDet();
                    rec.Batchno = batchno;
                    rec.LineId = sno1;

                    rec.Qty= qty;
                    rec.Pos = 1;
                    rec.BranchId = usr.bCode;
                    rec.CustomerCode = usr.cCode;
                    db.PpcBatchProcessWiseDetailsDet.Add(rec);
                    db.SaveChanges();
                }
            }
            return pos;
        }
        private string findSeq(UserInfo usr)
        {
            UsineContext db = new UsineContext();
            General gg = new General();
            string str = "JOB" + ac.getPresentDateTime().Year.ToString() + "-";
            int x = 0;
            var det = db.PpcBatchProcessWiseDetailsUni.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.JobCardNo);
            if(det != null)
            {
                x = gg.valInt(gg.right(det, 6));
            }
            x++;
            return str + gg.zeroMake(x, 6);
        }

    }
}
