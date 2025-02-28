
using System;
using System.Collections.Generic;
using Usine_Core.Controllers.Purchases;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;

namespace Usine_Core.Controllers.CRM
{

   
    public class CRMPriceListTotal
    {
        public CrmPriceListUni header { get; set; }
        public List<CrmPriceListDet> lines { get; set; }
        public dynamic pricedetails { get; set; }
        public int? traCheck { get; set; }
        public UserInfo usr { get; set; }
    }
    public class CRMPriceListRequirements
    {
        public dynamic prices { get; set; }
        public dynamic taxes { get; set; }
    }
    public class CRMPriceListsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/CRMPriceLists/GetPriceListRequirements")]
        public CRMPriceListRequirements GetPriceListRequirements([FromBody] UserInfo usr)
        {
            CRMPriceListRequirements  tot = new CRMPriceListRequirements();
            tot.prices =(from a in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode)
                    select new
                    {
                        productid = a.RecordId,
                        product = a.Itemname,
                        grpname = a.Grpname,
                        uom = a.Um,
                        price = 0,
                        tax=-1
                    }).OrderBy(b => b.grpname).ThenBy(c => c.product).ToList();
            tot.taxes = db.CrmTaxAssigningUni.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();
            return tot;

        }

      

        [HttpPost]
        [Authorize]
        [Route("api/CRMPriceLists/GetCRMPricesLists")]
        public List<CrmPriceListUni> GetCRMPricesLists([FromBody] UserInfo usr)
        {
            return db.CrmPriceListUni.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMPriceLists/GetCRMPriceListDetails")]
        public CRMPriceListTotal GetCRMPriceListDetails([FromBody] GeneralInformation inf)
        {
            CRMPriceListTotal tot = new CRMPriceListTotal();
            tot.header = db.CrmPriceListUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            tot.pricedetails = (from a in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == inf.usr.cCode)
                         join b in db.CrmPriceListDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                         on a.RecordId equals b.ProductId into gj
                         from subdet in gj.DefaultIfEmpty()
                         select new
                         {
                             productid=a.RecordId,
                             product=a.Itemname,
                             grpname=a.Grpname,
                             uom=a.Um,
                             price=subdet==null?0:subdet.Price,
                             tax=subdet==null?-1:subdet.TaxId

                         });

            return tot;
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMPriceLists/setCRMPriceList")]
        public TransactionResult setCRMPriceList([FromBody] CRMPriceListTotal tot)
        {
            string msg = "";
            try
            {
                if(ac.screenCheck(tot.usr,7,1,1,0))
                {
                    int sno = 1;
                    if (duplicatePriceListcheck(tot))
                    {
                        switch(tot.traCheck)
                        {
                            case 1:
                                using (var txn = db.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        tot.header.BranchId = tot.usr.bCode;
                                        tot.header.EffectiveDate= ac.DateAdjustFromFrontEnd(tot.header.EffectiveDate.Value);
                                        tot.header.CustomerCode = tot.usr.cCode;
                                        db.CrmPriceListUni.Add(tot.header);
                                        db.SaveChanges();

                                        foreach(CrmPriceListDet line in tot.lines)
                                        {
                                            line.RecordId = tot.header.RecordId;
                                            line.Sno = sno;
                                            line.BranchId = tot.usr.bCode;
                                            line.CustomerCode= tot.usr.cCode;
                                            sno++;
                                        }
                                        db.CrmPriceListDet.AddRange(tot.lines);
                                        db.SaveChanges();
                                        txn.Commit();
                                        msg = "OK";
                                    }
                                    catch (Exception ee)
                                    {
                                        msg = ee.Message;
                                        txn.Rollback();
                                    }
                                }
                                break;
                            case 2:
                                var headupd = db.CrmPriceListUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if(headupd != null)
                                {
                                    headupd.PriceListName=tot.header.PriceListName;
                                    headupd.EffectiveDate=ac.DateAdjustFromFrontEnd(tot.header.EffectiveDate.Value);
                                    headupd.MrpCheck = tot.header.MrpCheck;
                                    headupd.Pos=tot.header.Pos;
                                }
                                var linesupd = db.CrmPriceListDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                if(linesupd != null)
                                {
                                    if(linesupd.Count() > 0)
                                    {
                                        db.CrmPriceListDet.RemoveRange(linesupd);
                                    }
                                }

                                foreach (CrmPriceListDet line in tot.lines)
                                {
                                    line.RecordId = tot.header.RecordId;
                                    line.Sno = sno;
                                    line.BranchId = tot.usr.bCode;
                                    line.CustomerCode = tot.usr.cCode;
                                    sno++;
                                }
                                db.CrmPriceListDet.AddRange(tot.lines);
                                db.SaveChanges();
                                msg = "OK";
                                break;
                            case 3:
                                var linesdel = db.CrmPriceListDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                if (linesdel != null)
                                {
                                    if (linesdel.Count() > 0)
                                    {
                                        db.CrmPriceListDet.RemoveRange(linesdel);
                                    }
                                }
                                var headdel = db.CrmPriceListUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (headdel != null)
                                {
                                    db.CrmPriceListUni.Remove(headdel);
                                }
                                db.SaveChanges();
                                msg = "OK";
                                break;
                        }
 
                    }
                    else
                    {
                        msg = tot.traCheck == 3 ? "This Price list is in use deletion is not possible" : "This name is already existed";
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
        private Boolean duplicatePriceListcheck(CRMPriceListTotal tot)
        {
            Boolean b=false;
            switch(tot.traCheck)
            {
                case 1:
                    var cre = db.CrmPriceListUni.Where(a => a.PriceListName == tot.header.PriceListName && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(cre == null)
                        b = true;
                    break;
                case 2:
                    var upd = db.CrmPriceListUni.Where(a => a.RecordId != tot.header.RecordId && a.PriceListName == tot.header.PriceListName && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(upd==null)
                        b= true;
                    break;
                case 3:
                     var del = db.PartyDetails.Where(a => a.Pricelist == tot.header.PriceListName && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (del == null)
                       b = true;
                    break;
            }
            return b;
        }


    }
}
