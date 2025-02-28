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

 namespace Usine_Core.Controllers.production
{
    public class MaterialConsumptionsForProduction
    {
        public int? materialid { get; set; }
        public string materialname { get; set; }
        public string grpname { get; set; }
        public double? qty { get; set; }
        public string um { get; set; }
        public double? valu { get; set; }
    }
    public class ProcessDetailInfo
    {
        public int? batchid { get; set; }
        public int? lineid { get; set; }
        public string batchno { get; set; }
        public DateTime? dat { get; set; }
        public string itemname { get; set; }
        public double? qty { get; set; }
        public string um { get; set; }
        public string process { get; set; }
      
    }
    public class ProductionGeneral
    {
        UsineContext db = new UsineContext();
        public List<MaterialConsumptionsForProduction> GetMaterialsUsedForPresentBatches(UserInfo usr)
        {
            AdminControl ac = new AdminControl();
            DataBaseContext g = new DataBaseContext();
            DateTime dat = ac.getFinancialStart(ac.getPresentDateTime(),usr);
            string dat1 = ac.strDate(dat);
            string dat2 = ac.strDate(dat.AddYears(1));
            string quer = "";
            quer = quer + " select b.recordId,b.itemname,b.grpname,a.qtyout,b.um,a.valu from";
            quer = quer + " (select itemname, sum(qtyout) qtyout, sum(qtyout * rat) valu from";
            quer = quer + " (select a.itemname, qtyout, rat from";
            quer = quer + " (select gin, itemname, qtyout, 1 costingType from";
            quer = quer + " (select * from invMaterialManagement where transactionType = 103 and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + " and itemname in";
            quer = quer + " (select recordId from invMaterials where costingType = 1 and customerCode = " + usr.cCode + "))a,";
            quer = quer + " (select * from ppcBatchPlanningUni where pos = 1 and branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b where a.productBatchNo = b.recordId)a,";
            quer = quer + " (select itemname, sum(qtyin * rat) / sum(qtyin) rat from invMaterialManagement where transactionType < 100 and";
            quer = quer + " dat >= '" + dat1 + "' and dat< '" + dat2 + "' and branchid = '" + usr.bCode + "'";
            quer = quer + " and customerCode = " + usr.cCode + " group by itemname)b where a.itemName = b.itemName)x group by itemname";
            quer = quer + " union all";
            quer = quer + " select itemname, sum(qtyout) qtyout,sum(qtyout * rat) valu from";
            quer = quer + " (select a.gin, a.itemname, a.qtyout, b.rat from";
            quer = quer + " (select gin, itemname, qtyout,1 costingType from";
            quer = quer + " (select * from invMaterialManagement where transactionType = 103 and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + " and itemname in";
            quer = quer + " (select recordId from invMaterials where costingType > 1 and customerCode = " + usr.cCode + ") )a,";
            quer = quer + " (select * from ppcBatchPlanningUni where pos = 1 and branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b where a.productBatchNo = b.recordId)a,";
            quer = quer + " (select gin, itemname, rat from invMaterialManagement where transactionType < 100";
            quer = quer + " and dat >= '" + dat1 + "' and dat< '" + dat2 + "' and branchid = '" + usr.bCode + "' and customercode = " + usr.cCode + ")b where a.gin = b.gin and a.itemName = b.itemName)x group by itemname)a,";
            quer = quer + " (select * from invMaterialCompleteDetails_view where customerCode = " + usr.cCode + ")b where a.itemName = b.recordId";
            SqlCommand dc = new SqlCommand();
            dc.Connection = g.db;
            General gg = new General();
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            List<MaterialConsumptionsForProduction> lst = new List<MaterialConsumptionsForProduction>();
            while (dr.Read())
            {
                lst.Add(new MaterialConsumptionsForProduction
                {
                    materialid = gg.valInt(dr[0].ToString()),
                    materialname = dr[1].ToString(),
                    grpname=dr[2].ToString(),
                    qty=gg.valNum(dr[3].ToString()),
                    um=dr[4].ToString(),
                    valu=gg.valNum(dr[5].ToString())

                });
                
            }
            dr.Close();
            g.db.Close();
            return lst;

        }
        public List<ProcessDetailInfo> pendingProcessList(UserInfo usr)
        {
            var lst = (from a in db.PpcBatchProcessWiseDetailsDet.Where(a => a.Pos != 0 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                       join b in db.PpcBatchPlanningProcesses.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on new { batch = a.Batchno, sno = a.LineId } equals new { batch = b.RecordId, sno = b.Sno }
                       join c in db.PpcProcessesMaster.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on b.ProcessId equals c.RecordId
                       join d in db.PpcBatchPlanningUni.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on a.Batchno equals d.RecordId
                       join e in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode) on d.ItemId equals e.RecordId
                       select new ProcessDetailInfo
                       {
                           batchid = a.Batchno,
                           lineid = a.LineId,
                           batchno = d.BatchNo,
                           dat = d.Dat,
                           itemname = e.Itemname,
                           qty = a.Qty,
                           um = e.Um,
                           process=c.ProcessName + (a.Pos == 1 ? "" : " Quality")

                       }).OrderBy(b => b.dat).ThenBy(c => c.batchid).ToList();
            return lst;

        }
    }
}
