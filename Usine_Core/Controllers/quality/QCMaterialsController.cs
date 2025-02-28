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
using DocumentFormat.OpenXml.Drawing.Charts;

namespace Usine_Core.Controllers.quality
{
     public class QCMaterialTestRequirementInfo
    {
        public List<PurPurchasesUni> headers { get; set; }
        public List<PurPurchasesDet> lines { get; set; }
        public List<QcTestings> tests { get; set; }
    }
    public class QCMaterialTestingTotal
    {
        public QcTraTestsUni header { get; set; }
        public List<QcTraTestsDet> lines { get; set; }
        public int? traCheck { get; set; }
        public UserInfo usr { get; set; }
    }
    public class QCMIRApprovalInfo
    {
        public dynamic materials { get; set; }
        public dynamic tests { get; set; }

    }
    public class QCMaterialsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/QCMaterials/GetQCMaterialRequirements")]
        public QCMaterialTestRequirementInfo GetQCMaterialRequirements([FromBody] UserInfo usr)
        {
            QCMaterialTestRequirementInfo tot = new QCMaterialTestRequirementInfo();
           tot.headers=(from a in db.PurPurchasesUni.Where(a => a.QcCheck==1 && a.BranchId == usr.bCode && a.CustomerCode ==usr.cCode)
                        select new PurPurchasesUni
                        {
                            RecordId=a.RecordId,
                            Seq=a.Seq,
                            Dat=a.Dat,
                            Invoiceno=a.Invoiceno,
                            Vendorname=a.Vendorname,
                            TotalAmt=a.TotalAmt
                        }).OrderBy(b => b.RecordId).ToList();
            var recs = tot.headers.Select(b => b.RecordId).ToList();
            tot.lines = (from a in db.PurPurchasesDet.Where(a => recs.Contains(a.RecordId) && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                       join b in db.InvUm.Where(a => a.CustomerCode == usr.cCode) on a.Um equals b.RecordId
                         select new PurPurchasesDet
                         {
                             RecordId = a.RecordId,
                             Sno = a.Sno,
                             ItemId = a.ItemId,
                             ItemName = a.ItemName,
                             Batchno = a.Batchno,
                             Qty = a.Qty,
                             Rat=a.Rat,
                             Um = a.Um,
                             BranchId = b.Um
                         }).OrderBy(c => c.RecordId).ThenBy(d => d.Sno).ToList();

            tot.tests = (from a in db.QcTestings.Where(a => a.TestArea == "MAT" && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                         select new  QcTestings
                         {
                             RecordId=a.RecordId,
                             Testname=a.Testname,
                             TestArea=a.TestArea,
                             MicroCheck=a.MicroCheck,
                             CheckingType=a.CheckingType
 
    }).OrderBy(b => b.RecordId).ToList();
            return tot;

        }

        [HttpPost]
        [Authorize]
        [Route("api/QCMaterials/SetQCMaterial")]
        public TransactionResult SetQCMaterial([FromBody] QCMaterialTestingTotal tot)
        {
            string msg = "";
            try
            {
                if(ac.screenCheck(tot.usr, 11,2,1,0))
                {
                    switch(tot.traCheck)
                    {
                        case 1:
                            tot.header.BranchId = tot.usr.bCode;
                            tot.header.CustomerCode = tot.usr.cCode;
                            tot.header.Dat = ac.getPresentDateTime();
                            tot.header.Usrname = tot.usr.uCode;
                            db.QcTraTestsUni.Add(tot.header);
                            db.SaveChanges();
                            int sno = 1;
                            foreach(var line in tot.lines)
                            {
                                line.RecordId = tot.header.RecordId;
                                line.Sno = sno;
                                sno++;
                                line.BranchId = tot.usr.bCode;
                                line.CustomerCode = tot.usr.cCode;
                            }
                            db.QcTraTestsDet.AddRange(tot.lines);
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
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;
        }
        [HttpPost]
        [Authorize]
        [Route("api/QCMaterials/GetCompleteMIRQCDetails")]
        public dynamic GetCompleteMIRQCDetails([FromBody] GeneralInformation inf)
        {
             
            var list1=(from a in db.PurPurchasesDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                     join b in db.QcTraTestsDet.Where(a => a.TransactionId==inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                     on new { rec = a.RecordId, sno = a.Sno } equals new { rec = b.TransactionId, sno = b.Lotno } into gj
                     from subdet in gj.DefaultIfEmpty()
                     select new
                     {
                         recordid = a.RecordId,
                         sno = a.Sno,
                         itemid=a.ItemId,
                         itemname=a.ItemName,
                         qty=a.Qty,
                         rat=a.Rat,
                         rectifiedqty=subdet==null?0:subdet.RectifiedQty,
                         rectifiedvalue=subdet==null?0:subdet.RectificationCost,
                         rejectedqty=subdet==null?0:subdet.RejectedQty,
                         rejectedvalue=(subdet == null ? 0 : subdet.RejectedQty)*a.Rat,
                         comments=subdet==null?" ":subdet.Comments,
                         checkedqty=subdet==null?0:subdet.CheckedQty,
                         qcid=subdet==null?0:subdet.RecordId
                     });

            var list2 = (from a in db.QcTraTestsUni.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                         join b in db.QcTestings.Where(a => a.CustomerCode == inf.usr.cCode)
                         on a.Testid equals b.RecordId
                         select new
                         {
                             recordId=a.RecordId,
                             dat=a.Dat,
                             testid=a.Testid,
                             testname=b.Testname,
                             usrname=a.Usrname
                         });
            return (from a in list1
                       join b in list2 on a.qcid equals b.recordId into gj
                       from subdet in gj.DefaultIfEmpty()
                       select new
                       {
                           a.recordid,
                           a.sno,
                           a.itemid,
                           a.itemname,
                           a.qty,
                           a.checkedqty,
                           a.rat,
                           testname=subdet==null?"":subdet.testname,
                           username=subdet==null?"":subdet.usrname,
                           a.comments,
                           a.rectifiedqty,
                           a.rectifiedvalue,
                           a.rejectedqty,
                           a.rejectedvalue
                       });

            
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

        [HttpPost]
        [Authorize]
        [Route("api/QCMaterials/SetQCMIRApproval")]
        public TransactionResult SetQCMIRApproval([FromBody] GeneralInformation inf)
        {
            string msg = "";
            try
            {
                if(ac.screenCheck(inf.usr,11,2,2,0))
                {
                    UsineContext db1 = new UsineContext();
                    var header = db.PurPurchasesUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if (header != null)
                    {
                        header.QcCheck = -1;

                        var lines = (from a in db1.QcTraTestsDet.Where(a => a.Lottype == "MAT" && a.TransactionId == inf.recordId && a.RejectedQty > 0 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                      join b in db1.PurPurchasesDet.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.TransactionId equals b.RecordId
                                      join c in db1.InvMaterialUnits.Where(a => a.CustomerCode == inf.usr.cCode) on new {itemid=b.ItemId,um=b.Um} equals new { itemid=c.RecordId,um=c.Um}
                                      select new
                                      {
                                          itemid=b.ItemId,
                                          itemname=b.ItemName,
                                          recordid=b.RecordId,
                                          sno=b.Sno,
                                          qty=b.Qty,
                                          um=b.Um,
                                          stdum=c.StdUm,
                                          rejected=a.RectifiedQty,
                                          rat=b.Rat,
                                          gin=b.Gin,
                                          conversion=c.ConversionFactor
                                      }).ToList();

                    /*    var lines2 = db1.InvMaterialUnits.Where(a => a.CustomerCode == inf.usr.cCode).ToList();
                        var lines = (from a in lines1
                                     join b in lines2 on a.um equals b.RecordId
                                     select new
                                     {
                                         a.itemid,
                                         recordid=a.recordid,
                                         a.sno,
                                         a.itemname,
                                         a.qty,
                                         a.rejected,
                                         a.rat,
                                         um=a.um,
                                         stdun=b.RecordId,
                                         conversion=b.ConversionFactor,
                                         gin=a.gin
                                     }).ToList();*/
                        General gg = new General();
                         
                        var purlines = (from a in db.PurPurchasesDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                        join b in db.InvMaterialUnits.Where(a => a.CustomerCode == inf.usr.cCode) on new { itemid = a.ItemId, um = a.Um } equals new { itemid = b.RecordId, um = b.Um }
                                        select new InvMaterialManagement
                                        {
                                            TransactionId = a.RecordId,
                                            Sno=a.Sno,
                                               Gin =a.Gin,
                                            ItemName = a.ItemId,
                                            Dat = header.Dat,
                                            BatchNo = a.Batchno,
                                            Manudate = a.Manudate,
                                            Expdate = a.Expdate,
                                            Store = a.Store,
                                            Qtyin = a.Qty * b.ConversionFactor,
                                            Qtyout = 0,
                                            Rat = a.Rat / b.ConversionFactor,
                                            Descr = "Added material after Quality verification",
                                            TransactionType = 1,
                                            BranchId = inf.usr.bCode,
                                            CustomerCode = inf.usr.cCode

                                        }).ToList();


                        db.InvMaterialManagement.AddRange(purlines);   // code comminted by ajay(11/02/2025)
                     
                        //foreach (var item in purlines)
                        //{
                        //    var chklst = db.InvMaterialManagement.Where(x => x.ItemName == item.ItemName).FirstOrDefault();
                        //    if (chklst == null)
                        //    {
                        //        item.TransactionId = 0;
                        //        item.Sno = 0;

                        //        //db.InvMaterialManagement.Add(item);
                        //        //db.SaveChanges();
                        //    }
                        //    db.InvMaterialManagement.Add(item);
                        //    db.SaveChanges();

                        //}

                        if (lines.Count() > 0)
                        {
                            PurPurchaseReturnsUni header1 = new PurPurchaseReturnsUni();
                            header1.RefMir = (int)inf.recordId;
                            header1.Vendorid = header.Vendorid;
                            header1.Vendorname = header.Vendorname;
                            header1.Baseamt = lines.Sum(a => a.rejected * a.rat);
                            header1.Taxes = 0;
                            header1.Discount = 0;
                            header1.Others = 0;
                            header1.TotalAmt= lines.Sum(a => a.rejected * a.rat);
                            List<PurPurchaseReturnsDet> preturns = new List<PurPurchaseReturnsDet>();
                            foreach (var lin in lines)
                            {


                           
                           
                                preturns.Add(new PurPurchaseReturnsDet
                                {
                                    Lotno = (int)lin.sno,
                                    ItemId = lin.itemid,
                                    ItemName = lin.itemname,
                                    Batchno=" ",
                                    Manudate=null,
                                    Qty=lin.rejected,
                                    Um=lin.um,
                                    Rat=lin.rat,
                                    Mrp=lin.conversion,
                                    BranchId=lin.gin
                                   
                                }) ;
                                
                            }
                            PurPurchaseReturnController pc = new PurPurchaseReturnController();
                            PurPurchaseReturnTotal prtot = new PurPurchaseReturnTotal();
                            prtot.header = header1;
                            prtot.lines = preturns;
                            prtot.usr = inf.usr;
                            prtot.traCheck = 1;
                            prtot.taxes = new List<PurPurchaseReturnTaxes>();
                             pc.SetPurchaseReturn(prtot);
                        }

                        var crnotes = db1.QcTraTestsDet.Where(a => a.Lottype == "MAT" && a.TransactionId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).Sum(b => b.RectificationCost);
                        if(crnotes != null)
                        {
                            if(crnotes > 0)
                            {
                                PurCreditNotesController cc = new PurCreditNotesController();
                                PartyCreditDebitNotes pcr = new PartyCreditDebitNotes();
                                pcr.TransactionType = "PCR";
                                pcr.TransactionId = (int)inf.recordId;
                                pcr.Amt = (double)crnotes;
                                pcr.Descriptio = "Rectification cost from Quality Department";

                                CreditNoteTotal crtot = new CreditNoteTotal();
                                crtot.note = pcr;
                                crtot.usr = inf.usr;

                                cc.SetCreditNote(crtot);

                            }
                        }



                        db.SaveChanges();


                        msg = "OK";
                    }
                    else
                    {
                        msg = "No Record found";
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
