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
   public class PPCProcessTotal
    {
       public PpcProcessesMaster process { get; set; }
        public int? traCheck { get; set; }
        public UserInfo usr { get; set; }
    }
    public class PPCProductWiseInfoRequirements
    {
        public List<InvMaterialCompleteDetailsView> products { get; set; }
        public List<InvMaterialCompleteDetailsView> materials { get; set; }
        public List<MaiEquipment> equipment { get; set; }
        public List<HrdDesignations> designations { get; set; }
        public List<ProItemWiseAttachmentsUni> attachments { get; set; }
        public dynamic materialdetails { get; set; }
        public dynamic equipmentdetails { get; set; }
        public dynamic designationdetails { get; set; }

    }
    public class PPCProductWiseInfoTotal
    {
        public int? productId { get; set; }
        public ProItemWiseAttachmentsUni attachment { get; set; }
        public List<ProItemWiseIngredients> ingredients { get; set; }
        public List<ProItemWiseEquipment> equipment { get; set; }
        public List<ProItemWiseDesignations> designations { get; set; }
        public UserInfo usr { get; set; }
    }
    public class ppcMastersController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/ppcMasters/GetPPCProcesses")]
        public List<PpcProcessesMaster> GetPPCProcesses([FromBody] UserInfo usr)
        {
            return db.PpcProcessesMaster.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/ppcMasters/SetPPCProcess")]
        public TransactionResult SetPPCProcess([FromBody] PPCProcessTotal tot)
        {
            string msg = "";
            try
            {
                if(ac.screenCheck(tot.usr,10,1,1,(int)tot.traCheck))
                {
                    if (dupProcessCheck(tot))
                    {
                        switch (tot.traCheck)
                        {
                            case 1:
                                tot.process.BranchId = tot.usr.bCode;
                                tot.process.CustomerCode = tot.usr.cCode;
                                db.PpcProcessesMaster.Add(tot.process);
                                db.SaveChanges();
                                msg = "OK";
                                break;
                            case 2:
                                var upd = db.PpcProcessesMaster.Where(a => a.RecordId == tot.process.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if(upd != null)
                                {
                                    upd.ProcessName = tot.process.ProcessName;
                                    upd.QcRequired = tot.process.QcRequired;
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "No record found";
                                }
                                break;
                            case 3:
                                var del = db.PpcProcessesMaster.Where(a => a.RecordId == tot.process.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (del != null)
                                {
                                    db.PpcProcessesMaster.Remove(del);
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "No record found";
                                }
                                break;
                        }
                    }
                    else
                    {
                        msg = tot.traCheck == 3 ? "This process is moved further deletion is not possible" : "This process name is already existed";
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
        private Boolean dupProcessCheck(PPCProcessTotal tot)
        {
            Boolean b = false;
            switch(tot.traCheck)
            {
                case 1:
                    var cre = db.PpcProcessesMaster.Where(a => a.ProcessName.ToUpper() == tot.process.ProcessName.ToUpper() && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(cre==null)
                    {
                        b=true;
                    }
                    break;
                case 2:
                    var upd = db.PpcProcessesMaster.Where(a => a.RecordId != tot.process.RecordId && a.ProcessName.ToUpper() == tot.process.ProcessName.ToUpper() && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (upd == null)
                    {
                        b = true;
                    }
                    break;
                case 3:
                    b = true;
                    break;
            }

            return b;
        }

        [HttpPost]
        [Authorize]
        [Route("api/ppcMasters/GetProductWiseAttachments")]
        public PPCProductWiseInfoRequirements GetProductWiseAttachments([FromBody] UserInfo usr)
        {
            try
            {
                PPCProductWiseInfoRequirements tot = new PPCProductWiseInfoRequirements();
                tot.products = (from a in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode)
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

                tot.materials = (from a in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode)
                                 select new InvMaterialCompleteDetailsView
                                 {
                                     RecordId = a.RecordId,
                                     Itemid = a.Itemid,
                                     Itemname = a.Itemname,
                                     Grpid = a.Grpid,
                                     Grpname = a.Grpname,
                                     Um = a.Um,
                                     Umid = a.Umid

                                 }).OrderBy(c => c.Itemname).ToList();
                tot.equipment = (from a in db.MaiEquipment.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                                 join b in db.MaiEquipGroups.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                                 on a.EquipmentGroup equals b.RecordId
                                 select new MaiEquipment
                                 {
                                     RecordId = a.RecordId,
                                     EquipmentCode = a.EquipmentCode,
                                     EquipmentName = a.EquipmentName,
                                     BranchId = b.SGrp
                                 }
                               ).OrderBy(b => b.EquipmentName).ToList();
                tot.designations = (from a in db.HrdDesignations.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                                    join b in db.HrdDepartments.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode) on a.Department equals b.RecordId
                                    select new HrdDesignations
                                    {
                                        RecordId = a.RecordId,
                                        BranchId = b.SGrp,
                                        Designation = a.Designation
                                    }).OrderBy(b => b.BranchId).ThenBy(c => c.Designation).ToList();

                tot.attachments = db.ProItemWiseAttachmentsUni.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
                tot.materialdetails = (from a in db.ProItemWiseIngredients.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                                       join b in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode) on a.Ingredient equals b.RecordId
                                       select new
                                       {
                                           slno = a.Slno,
                                           a.ItemId,
                                           a.Ingredient,
                                           ingredientname = b.Itemname,
                                           qty = a.Qty,
                                           umid = b.Umid,
                                           um = b.Um
                                       }).OrderBy(b => b.ItemId).ThenBy(c => c.slno).ToList();
                var lst1 = db.ProItemWiseEquipment.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
                tot.equipmentdetails = (from a in lst1
                                        join b in tot.equipment on a.EquipmentId equals b.RecordId
                                        select new
                                        {
                                            a.Itemid,
                                            a.Slno,
                                            a.EquipmentId,
                                            b.EquipmentName,
                                            equipgrp = b.BranchId,
                                            a.Manhrs
                                        }).OrderBy(b => b.Itemid).ThenBy(c => c.Slno).ToList();
                var lst2 = db.ProItemWiseDesignations.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
                tot.designationdetails = (from a in lst2
                                          join b in tot.designations on a.DesigId equals b.RecordId
                                          select new
                                          {
                                              a.Itemid,
                                              a.Slno,
                                              a.DesigId,
                                              b.Designation,
                                              a.Manhrs,
                                              department = b.BranchId
                                          }
                                        ).OrderBy(b => b.Itemid).ThenBy(c => c.Slno).ToList();
                return tot;
            }
            catch(Exception ee)
            {
                return null;
            }
        }


        [HttpPost]
        [Authorize]
        [Route("api/ppcMasters/SetProductWiseAttachments")]
        public TransactionResult SetProductWiseAttachments([FromBody] PPCProductWiseInfoTotal tot)
        {
            string msg = "";

            if (ac.screenCheck(tot.usr, 10, 1, 2, 0))
            {
                try
                {
                    var attach = db.ProItemWiseAttachmentsUni.Where(a => a.ItemId == tot.productId).FirstOrDefault();
                    if (attach == null)
                    {
                        tot.attachment.BranchId = tot.usr.bCode;
                        tot.attachment.CustomerCode = tot.usr.cCode;
                        db.ProItemWiseAttachmentsUni.Add(tot.attachment);
                    }
                    else
                    {
                        attach.MinBatchQty = tot.attachment.MinBatchQty;
                        attach.MaxBatchQty = tot.attachment.MaxBatchQty;
                    }
                    if (ac.screenCheck(tot.usr, 10, 1, 2, 0))
                    {
                        var ingrlst = db.ProItemWiseIngredients.Where(a => a.ItemId == tot.productId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                        if (ingrlst.Count() > 0)
                        {
                            db.ProItemWiseIngredients.RemoveRange(ingrlst);
                        }
                        foreach (var ingr in tot.ingredients)
                        {
                            ingr.BranchId = tot.usr.bCode;
                            ingr.CustomerCode = tot.usr.cCode;
                        }
                        if (tot.ingredients.Count() > 0)
                        {
                            db.ProItemWiseIngredients.AddRange(tot.ingredients);
                        }


                        var equiplst = db.ProItemWiseEquipment.Where(a => a.Itemid == tot.productId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                        if (equiplst.Count() > 0)
                        {
                            db.ProItemWiseEquipment.RemoveRange(equiplst);
                        }
                        foreach (var equ in tot.equipment)
                        {
                            equ.BranchId = tot.usr.bCode;
                            equ.CustomerCode = tot.usr.cCode;
                        }
                        if (tot.equipment.Count() > 0)
                        {
                            db.ProItemWiseEquipment.AddRange(tot.equipment);
                        }

                        var desiglist = db.ProItemWiseDesignations.Where(a => a.Itemid == tot.productId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                        if (desiglist.Count() > 0)
                        {
                            db.ProItemWiseDesignations.RemoveRange(desiglist);
                        }
                        foreach (var desi in tot.designations)
                        {
                            desi.BranchId = tot.usr.bCode;
                            desi.CustomerCode = tot.usr.cCode;
                        }
                        if (tot.designations.Count() > 0)
                        {
                            db.ProItemWiseDesignations.AddRange(tot.designations);
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

            }
            else
            {
                msg = "You are not authorised for this transaction";
            }

            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;
        }

    }
}
  