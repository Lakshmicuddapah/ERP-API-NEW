using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace Usine_Core.Controllers.Inventory
{
    public class TransDet
    {
        public int? recordid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
    public class InvLossesTraRequirementInfo
    {
        public InvMaterialOutwardRequirementsTotal matinfo { get; set; }
        public List<TransDet> trans { get; set; }
        public List<InvStores> stores { get; set; }
    }
    public class SetInvTransactionTotal
    {
        public InvTransactionsUni header { get; set; }
        public List<InvTransactionsDet> lines { get; set; }
        
        public List<InvMaterialManagement> mgts { get; set; }
        public UserInfo usr { get; set; }

    }
    public class InvTransactionInformation
    {
        public InvTransactionsUni header { get; set; }
        public dynamic lines { get; set; }
    }
    public class invLossesTraController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/invLossesTra/GetInvLossesTraRequirements")]
        public InvLossesTraRequirementInfo GetInvLossesTraRequirements([FromBody] GeneralInformation inf)
        {
            InventoryGeneral ig = new InventoryGeneral();
            InvLossesTraRequirementInfo tot = new InvLossesTraRequirementInfo();
            tot.matinfo = ig.GetMaterialOutwardRequirements(inf.usr);
            tot.stores = db.InvStores.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();
            switch(inf.recordId)
            {
                case 102:
                    tot.trans = (from a in db.InvLosses.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                 select new TransDet
                                 {
                                     recordid=a.RecordId,
                                     name=a.LossName,
                                     description="Allowable " + a.Allowableper + "%"
                                 }).OrderBy(b => b.name).ToList();
                    break;
                case 103:
                    tot.trans = (from a in db.PpcBatchPlanningUni.Where(a => a.Pos == 1 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                 join b in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == inf.usr.cCode) on a.ItemId equals b.RecordId
                                 select new TransDet
                                 {
                                     recordid = a.RecordId,
                                     name = a.BatchNo,
                                     description = b.Itemname + " " + a.Qty.ToString() + " " + b.Um
                                 }).OrderBy(b => b.name).ToList();
                    break;
            }
            return tot;
        }



        [HttpPost]
        [Authorize]
        [Route("api/invLossesTra/GetInvTransactions")]
        public List<InvTransactionsUni> GetInvTransactions([FromBody] GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = dat1.AddDays(1);
            List<InvTransactionsUni> det = new List<InvTransactionsUni>();
            switch(inf.detail)
                {
                case "Issues":
                    det = (from a in db.InvTransactionsUni.Where(a => a.TraType == inf.detail && a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                               join b in db.InvMaterialManagement.Where(a => a.TransactionType == 103 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.RecordId equals b.TransactionId
                               join c in db.PpcBatchPlanningUni.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on b.Department equals c.RecordId
                               select new InvTransactionsUni
                               {
                                   RecordId = a.RecordId,
                                   Dat = a.Dat,
                                   Descriptio = "Issues of materials for batch " + c.BatchNo
                               }).OrderBy(d => d.RecordId).ToList();
                    break;
                case "Losses":
                    det = db.InvTransactionsUni.Where(a => a.TraType == inf.detail && a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();
                    break;
                case "":
                    break;
            }
            return det;
            //return db.InvTransactionsUni.Where(a => a.TraType == inf.detail && a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/invLossesTra/GetInvTransaction")]
        public InvTransactionInformation GetInvTransaction([FromBody] GeneralInformation inf)
        {
            InvTransactionInformation tot = new InvTransactionInformation();
            tot.header = db.InvTransactionsUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            tot.lines = (from a in db.InvTransactionsDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                         join b in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == inf.usr.cCode) on a.ItemId equals b.RecordId
                         select new
                         {
                             recordid = a.RecordId,
                             sno = a.Sno,
                             itemid = a.ItemId,
                             itemname = b.Itemname,
                             qtyin = a.Qtyin,
                             qtyout = a.Qtyout,
                             um = b.Um
                         }).OrderBy(c => c.sno).ToList();

            return tot;
        }

        [HttpPost]
        [Authorize]
        [Route("api/invLossesTra/SetInvTransaction")]
        public TransactionResult SetInvTransaction([FromBody] SetInvTransactionTotal tot)
        {
            string msg = "";
            try
            {
                Boolean b = false;
                switch(tot.header.TraType)
                {
                    case "Sameple In":
                        b = ac.screenCheck(tot.usr, 2, 2, 4, 1);
                        break;
                    case "Audit":
                        b = ac.screenCheck(tot.usr, 2, 2, 8, 1);
                        break;
                    case "Issues":
                        b = ac.screenCheck(tot.usr, 2, 2, 1, 1);
                        break;
                    case "Losses":
                        b = ac.screenCheck(tot.usr, 2, 2, 3, 1);
                        break;
                    case "Sample Out":
                        b = ac.screenCheck(tot.usr, 2, 2, 5, 1);
                        break;
                    

                }
                if(b)
                {
                    tot.header.BranchId = tot.usr.bCode;
                    tot.header.Dat = ac.getPresentDateTime();
                    tot.header.CustomerCode = tot.usr.cCode;
                    db.InvTransactionsUni.Add(tot.header);
                    db.SaveChanges();
                    foreach(var lin in tot.lines)
                    {
                        lin.RecordId = tot.header.RecordId;
                        lin.BranchId = tot.usr.bCode;
                        lin.CustomerCode = tot.usr.cCode;
                    }
                    db.InvTransactionsDet.AddRange(tot.lines);
                    foreach(var mgt in tot.mgts)
                    {
                        mgt.TransactionId = tot.header.RecordId;
                        mgt.Rat = findRate(mgt.Gin, mgt.ItemName, tot.usr);
                        mgt.Dat = ac.getPresentDateTime();
                        mgt.BranchId = tot.header.BranchId;
                        mgt.CustomerCode = tot.header.CustomerCode;
                    }
                    db.InvMaterialManagement.AddRange(tot.mgts);
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
        private double? findRate(string gin,int? itemid,UserInfo usr)
        {
            double? rat = 0;
            UsineContext db1 = new UsineContext();
            DateTime dat1 = ac.getFinancialStart(ac.getPresentDateTime(), usr);
            DateTime dat2 = dat1.AddYears(1);
            if(gin == null || gin.Trim()=="")
            {
                var valu = db1.InvMaterialManagement.Where(a => a.ItemName == itemid && a.TransactionType < 100 && a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Sum(b => (b.Qtyin * b.Rat));
                var qty = db1.InvMaterialManagement.Where(a => a.ItemName == itemid && a.TransactionType < 100 && a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Sum(b => (b.Qtyin));
                rat = valu / qty;
            }
            else
            {
                rat = db1.InvMaterialManagement.Where(a => a.Gin == gin && a.TransactionType < 100 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Select(b => b.Rat).FirstOrDefault();
                
            }
            if (rat == null)
            {
                rat = 0;
            }
            return rat;
        }

        [HttpPost]
        [Authorize]
        [Route("api/invLossesTra/DeleteInvTransaction")]
        public TransactionResult DeleteInvTransaction([FromBody] GeneralInformation inf)
        {
            string msg = "";
            int tcheck = 0;
            try
            {
                Boolean b = false;
                switch (inf.detail)
                {
                    case "Sameple In":
                        b = ac.screenCheck(inf.usr, 2, 2, 4, 3);
                        break;
                    case "Audit":
                        b = ac.screenCheck(inf.usr, 2, 2, 8, 3);
                        break;
                    case "Issues":
                        b = ac.screenCheck(inf.usr, 2, 2, 1, 3);
                        tcheck = 103;
                        break;
                    case "Losses":
                        b = ac.screenCheck(inf.usr, 2, 2, 3, 3);
                        tcheck = 102;
                        break;
                    case "Sample Out":
                        b = ac.screenCheck(inf.usr, 2, 2, 5, 3);
                        break;
                    

                }
                if(b)
                {
                    var mgts = db.InvMaterialManagement.Where(a => a.TransactionId == inf.recordId && a.TransactionType == tcheck && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                    if(mgts.Count() > 0)
                    {
                        db.InvMaterialManagement.RemoveRange(mgts);
                    }
                    var lines = db.InvTransactionsDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                    if(lines.Count() > 0)
                    {
                        db.InvTransactionsDet.RemoveRange(lines);
                    }
                    var header = db.InvTransactionsUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if(header != null)
                    {
                        db.InvTransactionsUni.Remove(header);
                    }
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
    }
}
