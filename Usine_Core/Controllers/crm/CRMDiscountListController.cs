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
    public class CRMDiscountListTotal
    {
        public CrmDiscountListUni header { get; set; }
        public List<CrmDiscountListDet> lines { get; set; }
        public dynamic discountdetails { get; set; }
        public int? traCheck { get; set; }
        public UserInfo usr { get; set; }
    }
    public class CRMDiscountListController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/CRMDiscountList/GetDiscountListRequirements")]
        public dynamic GetDiscountListRequirements([FromBody] UserInfo usr)
        {
            return (from a in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode)
                    select new
                    {
                        productid = a.RecordId,
                        product = a.Itemname,
                        grpname = a.Grpname,
                        uom = a.Um,
                        discount = 0
                    }).OrderBy(b => b.grpname).ThenBy(c => c.product).ToList();
        }
        [HttpPost]
        [Authorize]
        [Route("api/CRMDiscountList/GetCRMDiscountLists")]
        public List<CrmDiscountListUni> GetCRMDiscountLists([FromBody] UserInfo usr)
        {
            return db.CrmDiscountListUni.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();
        }
        [HttpPost]
        [Authorize]
        [Route("api/CRMDiscountList/GetCRMDiscountListDetails")]
        public CRMDiscountListTotal GetCRMDiscountListDetails([FromBody] GeneralInformation inf)
        {
            CRMDiscountListTotal tot = new CRMDiscountListTotal();
            tot.header = db.CrmDiscountListUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            tot.discountdetails = (from a in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == inf.usr.cCode)
                                join b in db.CrmDiscountListDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                on a.RecordId equals b.ProductId into gj
                                from subdet in gj.DefaultIfEmpty()
                                select new
                                {
                                    productid = a.RecordId,
                                    product = a.Itemname,
                                    grpname = a.Grpname,
                                    uom = a.Um,
                                    discount = subdet == null ? 0 : subdet.Discount

                                });

            return tot;
        }


        [HttpPost]
        [Authorize]
        [Route("api/CRMDiscountList/setCRMDiscountList")]
        public TransactionResult setCRMDiscountList([FromBody] CRMDiscountListTotal tot)
        {
            string msg = "";
            try
            {
                if (ac.screenCheck(tot.usr, 7, 1, 2, 0))
                {
                    int sno = 1;
                    if (duplicateDiscountListcheck(tot))
                    {
                        switch (tot.traCheck)
                        {
                            case 1:
                                using (var txn = db.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        tot.header.BranchId = tot.usr.bCode;
                                        tot.header.EffectiveDate = ac.DateAdjustFromFrontEnd(tot.header.EffectiveDate.Value);
                                        tot.header.CustomerCode = tot.usr.cCode;
                                        db.CrmDiscountListUni.Add(tot.header);
                                        db.SaveChanges();

                                        foreach (CrmDiscountListDet line in tot.lines)
                                        {
                                            line.RecordId = tot.header.RecordId;
                                            line.Sno = sno;
                                            line.BranchId = tot.usr.bCode;
                                            line.CustomerCode = tot.usr.cCode;
                                            sno++;
                                        }
                                        db.CrmDiscountListDet.AddRange(tot.lines);
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
                                var headupd = db.CrmDiscountListUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (headupd != null)
                                {
                                    headupd.DiscountListName = tot.header.DiscountListName;
                                    headupd.EffectiveDate = ac.DateAdjustFromFrontEnd(tot.header.EffectiveDate.Value);
                                    headupd.Pos = tot.header.Pos;
                                }
                                var linesupd = db.CrmDiscountListDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                if (linesupd != null)
                                {
                                    if (linesupd.Count() > 0)
                                    {
                                        db.CrmDiscountListDet.RemoveRange(linesupd);
                                    }
                                }

                                foreach (CrmDiscountListDet line in tot.lines)
                                {
                                    line.RecordId = tot.header.RecordId;
                                    line.Sno = sno;
                                    line.BranchId = tot.usr.bCode;
                                    line.CustomerCode = tot.usr.cCode;
                                    sno++;
                                }
                                db.CrmDiscountListDet.AddRange(tot.lines);
                                db.SaveChanges();
                                msg = "OK";
                                break;
                            case 3:
                                var linesdel = db.CrmDiscountListDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                if (linesdel != null)
                                {
                                    if (linesdel.Count() > 0)
                                    {
                                        db.CrmDiscountListDet.RemoveRange(linesdel);
                                    }
                                }
                                var headdel = db.CrmDiscountListUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (headdel != null)
                                {
                                    db.CrmDiscountListUni.Remove(headdel);
                                }
                                db.SaveChanges();
                                msg = "OK";
                                break;
                        }

                    }
                    else
                    {
                        msg = tot.traCheck == 3 ? "This Discount list is in use deletion is not possible" : "This name is already existed";
                    }
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
        private Boolean duplicateDiscountListcheck(CRMDiscountListTotal tot)
        {
            Boolean b = false;
            switch (tot.traCheck)
            {
                case 1:
                    var cre = db.CrmDiscountListUni.Where(a => a.DiscountListName == tot.header.DiscountListName && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (cre == null)
                        b = true;
                    break;
                case 2:
                    var upd = db.CrmDiscountListUni.Where(a => a.RecordId != tot.header.RecordId && a.DiscountListName == tot.header.DiscountListName && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (upd == null)
                        b = true;
                    break;
                case 3:
                     var del = db.PartyDetails.Where(a => a.Discountlist == tot.header.DiscountListName && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                     if (del == null)
                         b = true;

                    break;
            }
            return b;
        }



    }
}
