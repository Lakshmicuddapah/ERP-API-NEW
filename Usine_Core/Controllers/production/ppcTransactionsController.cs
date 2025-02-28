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
using System.Runtime.ConstrainedExecution;

namespace Usine_Core.Controllers.production
{
    public class ProductionPendingSosInfo
    {
        public int? soid { get; set; }
        public int? sno { get; set; }
        public string seq { get; set; }
        public string partyname { get; set; }
        public string mobile { get; set; }
        public int itemid { get; set; }
        public string itemname { get; set; }
        public string qty { get; set; }
        public string um { get; set; }
        public string qtyavailable { get; set; }
        public double pending { get; set; }
        
    }
    public class ItemDetails
    {
        public int? itemId { get; set; }
        public double? qty { get; set; }
    }
    public class ItemDetailsTotal
    {
        public List<ItemDetails> lst { get; set; }
        public UserInfo usr { get; set; }
    }
    public class ItemsRequiredForMassPlanning
    {
        public int? itemId { get; set; }
        public string itemName { get; set; }
        public string grp { get; set; }
        public double? qtyrequired { get; set; }
        public double? qtyavailable { get; set; }
        public double? qtypending { get; set; }
        public string uom { get; set; }
    }
    public class PPCMassPlanningTotal
    {
        public PpcMassPlanningUni header { get; set; }
        public List<PpcMassPlanningDet> lines { get; set; }
        public UserInfo usr { get; set; }
    }

    public class ppcTransactionsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        

        [HttpPost]
        [Authorize]
        [Route("api/ppcTransactions/GetMassPlanningRequirements")]
        public List<InvMaterialCompleteDetailsView> GetProductionMassPlanningRequirements([FromBody] UserInfo usr)
        {
            return (from a in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode)
                            join b in db.InvGroups.Where(a => a.GroupCode == "PRO" && a.CustomerCode == usr.cCode)
                            on a.Grpid equals b.RecordId
                            select new InvMaterialCompleteDetailsView
                            {
                                RecordId = a.RecordId,
                                Itemid = a.Itemid,
                                Itemname = a.Itemname,
                                Grpid = b.RecordId,
                                Grpname = a.Grpname,
                                Um = a.Um,
                                Umid = a.Umid

                            }).OrderBy(c => c.Itemname).ToList();
        }
        [HttpPost]
        [Authorize]
        [Route("api/ppcTransactions/GetPendingSOToBePlanned")]
        public List<ProductionPendingSosInfo> GetPendingSOToBePlanned([FromBody] UserInfo usr)
        {
            try
            {
                DateTime d = ac.getPresentDateTime();
                string dat1 = ac.strDate(ac.getFinancialStart(d, usr));
                string dat2 = ac.strDate(d.AddDays(1));
                DataBaseContext g = new DataBaseContext();

                string quer = "";
                quer = quer + " select a.soid,a.sno,seq,partyname, mobile,a.itemid,a.itemname,a.qty,b.um,a.qtyavailable,a.pending from";
                quer = quer + " (select a.soid, a.sno, b.seq, b.partyname, b.mobile, a.itemid, a.itemname, a.qty, um, a.qtyavailable, a.pending from";
                quer = quer + " (select soid, sno, itemid, itemname, qty, um, qtyavailable, pending from";
                quer = quer + " (select a.soid, a.sno, a.itemid, a.itemname, a.qty, a.um, a.qtyavailable, a.pending, b.lineId  from";
                quer = quer + " (select a.recordId soid, a.sno, a.itemid, a.itemname, a.qty, a.um,";
                quer = quer + " case when b.qtyavailable is null then 0 else b.qtyavailable end qtyavailable,";
                quer = quer + " a.qty -case when b.qtyavailable is null then 0 else b.qtyavailable end pending from";
                quer = quer + " (select* from (select a.recordId, a.sno, a.itemid, a.itemname, a.qty, a.um, b.refsoid  from";
                quer = quer + " (select * from crmSaleOrderdet  where branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a left outer join";
                quer = quer + " (select a.recordId, b.refSoid, a.itemid, a.qty from";
                quer = quer + " (select* from salSalesDet  where branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a,";
                quer = quer + " (select * from salSalesUni where branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b where a.recordId = b.recordId";
                quer = quer + " )b on a.recordid = b.refSoid and a.itemId = b.itemid )x where refsoid is null) a left outer join";
                quer = quer + " (select itemname, sum(qtyin-qtyout) qtyavailable from invMaterialManagement";
                quer = quer + " where dat >= '" + dat1 + "' and dat < '" + dat2 + "' and branchId = '" + usr.bCode + "' and customercode = " + usr.cCode + " group by itemname";
                quer = quer + " having sum(qtyin - qtyout) > 0)b on a.itemId = b.itemName )a left outer join";
                quer = quer + " (select* from ppcBatchPlanningSaleOrders where branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b";
                quer = quer + " on a.soid = b.soid and a.sno = b.lineId )x where lineId is null)a,";
                quer = quer + " (select * from crmSaleOrderUni where branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b where a.soid = b.recordId)a,";
                quer = quer + " (select * from invUM where customerCode = " + usr.cCode + ")b where a.um = b.recordId";


                List<ProductionPendingSosInfo> details = new List<ProductionPendingSosInfo>();
                General gg = new General();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();
                while (dr.Read())
                {
                    details.Add(new ProductionPendingSosInfo
                    {
                        soid = gg.valInt(dr[0].ToString()),
                        sno = gg.valInt(dr[1].ToString()),
                        seq = dr[2].ToString(),
                        partyname = dr[3].ToString(),
                        mobile = dr[4].ToString(),
                        itemid = gg.valInt(dr[5].ToString()),
                        itemname = dr[6].ToString(),
                        qty = dr[7].ToString(),
                        um = dr[8].ToString(),
                        qtyavailable = dr[9].ToString(),
                        pending = gg.valNum(dr[10].ToString())
                    });
                }
                dr.Close();
                g.db.Close();

                return details;
            }
            catch
            {
                return null;
            }
            
        }

        [HttpPost]
        [Authorize]
        [Route("api/ppcTransactions/PPCItemsRequiredForMassPlanning")]
        public List<ItemsRequiredForMassPlanning> PPCItemsRequiredForMassPlanning([FromBody] ItemDetailsTotal tot)
        {
            DateTime dat1 = ac.getFinancialStart(ac.getPresentDateTime(), tot.usr);
            DateTime dat2 = DateTime.Parse(ac.strDate(ac.getPresentDateTime())).AddDays(1);
            var lst1 = tot.lst;
            var lst2 = db.ProItemWiseIngredients.Where(a => a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
            var firstlist=(from a in lst1 join b in lst2 on a.itemId equals b.ItemId into gj
                           from subdet in gj.DefaultIfEmpty()
                           select new
                           {
                               productid = a.itemId,
                               ingredient=subdet==null?0:subdet.Ingredient,
                               qty=subdet==null?0:subdet.Qty,
                               reqd=a.qty
                           }).ToList();

            firstlist = firstlist.Where(a => a.ingredient > 0).ToList();
            var secondlist = (from a in firstlist.GroupBy(a => a.ingredient)
                              select new
                              {
                                  itemid=a.Key,
                                  qty = a.Sum(b => b.qty * b.reqd)
                              }).ToList();

            var thirdlist = (from a in db.InvMaterialManagement.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).GroupBy(b => b.ItemName)
                             select new
                             {
                                 itemid=a.Key,
                                 qtyavailable=a.Sum(b => b.Qtyin-b.Qtyout)
                             }).ToList();


            var finallist = (from a in secondlist
                             join b in thirdlist on a.itemid equals b.itemid into gj
                             from subdet in gj.DefaultIfEmpty()
                             select new
                             {
                                 itemid=a.itemid,
                                 qty=a.qty,
                                 qtyavailable=subdet==null?0:subdet.qtyavailable,
                                 qtyrequired=a.qty-(subdet == null ? 0 : subdet.qtyavailable)
                             }).ToList();
            finallist = finallist.Where(a => a.qtyrequired > 0).ToList();
            var det = db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == tot.usr.cCode).ToList();
            return (from a in finallist
                    join b in det on a.itemid equals b.RecordId
                    select new ItemsRequiredForMassPlanning
                    {
                        itemId=b.RecordId,
                        itemName=b.Itemname,
                        grp=b.Grpname,
                        qtyrequired=a.qty,
                        qtyavailable=a.qtyavailable,
                        qtypending=a.qtyrequired,
                        uom=b.Um
                    }).OrderBy(b => b.itemName).ToList();

        }

        [HttpPost]
        [Authorize]
        [Route("api/ppcTransactions/SetMassPlanning")]
        public TransactionResult SetMassPlanning([FromBody] PPCMassPlanningTotal tot)
        {
            string msg;
            try
            {
                if (ac.screenCheck(tot.usr, 10, 2, 1, 0))
                {
                    tot.header.BranchId = tot.usr.bCode;
                    tot.header.Seq = findMassPlanSeq(tot.usr);
                    tot.header.Dat = ac.getPresentDateTime();
                    tot.header.CustomerCode = tot.usr.cCode;
                    db.PpcMassPlanningUni.Add(tot.header);
                    db.SaveChanges();
                    int? sno = 1;
                    foreach(var line in tot.lines)
                    {
                        line.RecordId = tot.header.RecordId;
                        line.Sno = sno;
                        line.BranchId = tot.usr.bCode;
                        line.CustomerCode = tot.usr.cCode;
                        sno++;
                    }
                    db.PpcMassPlanningDet.AddRange(tot.lines);
                    db.SaveChanges();
                    msg = "OK";
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


        [HttpPost]
        [Authorize]
        [Route("api/ppcTransactions/DeleteMassPlanning")]
        public TransactionResult DeleteMassPlanning([FromBody] GeneralInformation inf)
        {
            string msg;
            try
            {
                if (ac.screenCheck(inf.usr, 10, 2, 1, 0))
                {
                    var lines = db.PpcMassPlanningDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                    if(lines.Count() > 0)
                    {
                        db.PpcMassPlanningDet.RemoveRange(lines);
                    }
                    var header = db.PpcMassPlanningUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if(header != null)
                    {
                        db.PpcMassPlanningUni.Remove(header);
                    }
                    db.SaveChanges();
                    msg = "OK";
                }
                else
                {
                    msg = "You are not authorised for this transaction";
                }
            }
            catch (Exception ee)
            {
                msg = ee.Message;
            }
            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;
        }


        private string findMassPlanSeq(UserInfo usr)
        {
             General g = new General();
            int x = 0;
            var det = db.PpcMassPlanningUni.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.Seq);
            if(det !=null)
            {
                x = g.valInt(g.right(det, 6));
            }
            x++;
            return "MPLAN" + g.zeroMake(x, 6);
        }



    }
}
