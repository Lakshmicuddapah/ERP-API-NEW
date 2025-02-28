using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Controllers.CRM;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace Usine_Core.Controllers.Maintenance
{
    public class MaiEquipmentTotal
    {
        public MaiEquipment equip { get; set; }
        public List<MaiEquipmentSpecifications> specifications { get; set; }
        public List<MaiEquipmentPreventiveMaintenances> prevmaintenances { get; set; }
        public int? traCheck { get; set; }
        public UserInfo usr { get; set; }
    }
    public class MaiEqupmentController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/MaiEqupment/GetMaiEquipmentDetails")]
        public dynamic GetMaiEquipmentDetails([FromBody] UserInfo usr)
        {
           return (from a in db.MaiEquipment.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                       join b in db.MaiEquipGroups.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                       on a.EquipmentGroup equals b.RecordId
                       select new
                       {
                           a.RecordId,
                           a.EquipmentCode,
                           a.EquipmentName,
                           b.SGrp
                       }).OrderBy(b => b.EquipmentName).ToList();
        }
        [HttpPost]
        [Authorize]
        [Route("api/MaiEqupment/GetMaiEquipmentDetail")]
        public string GetMaiEquipmentDetail([FromBody] GeneralInformation inf)
        {
            MaiEquipmentTotal tot = new MaiEquipmentTotal();
            tot.equip = (from a in db.MaiEquipment.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                         join b in db.MaiEquipGroups.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.EquipmentGroup equals b.RecordId
                         select new MaiEquipment
                         {
                             EquipmentCode = a.EquipmentCode,
                             EquipmentName = a.EquipmentName,
                             EquipmentGroup=a.EquipmentGroup,
                             PreferableServiceSupplier = a.PreferableServiceSupplier,
                             PreferableSparesSupplier=a.PreferableSparesSupplier,
                             MobileCheck= a.MobileCheck,
                             Roomno=a.Roomno,
                             AmcDate=a.AmcDate,
                             LastPmdate=a.LastPmdate,
                             MaxHrs=a.MaxHrs,
                             BranchId=b.SGrp
                         }).FirstOrDefault();
             tot.specifications = (from a in db.MaiEquipmentSpecifications.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                   select new MaiEquipmentSpecifications
                                   {
                                       Sno = a.Sno,
                                       Specification = a.Specification,
                                       Valu = a.Valu
                                   }).OrderBy(b => b.Sno).ToList();
             tot.prevmaintenances =(from a in  db.MaiEquipmentPreventiveMaintenances.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                    select new MaiEquipmentPreventiveMaintenances
                                    {
                                        Sno=a.Sno,
                                        Prevmaintenance=a.Prevmaintenance,
                                        FrequencyInDays=a.FrequencyInDays
                                    }).OrderBy(b => b.Sno).ToList();

            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(tot);
            return JSONString;

        }
        [HttpPost]
        [Authorize]
        [Route("api/MaiEqupment/SetMaiEquipmentDetail")]
        public TransactionResult SetMaiEquipmentDetail([FromBody] MaiEquipmentTotal tot)
        {
            string msg = "";
            int? sno = 1;
            try
            {
                if(ac.screenCheck(tot.usr,9,1,2,(int)tot.traCheck))
                {
                    if(dupCheck(tot))
                    {
                        switch(tot.traCheck)
                        {
                            case 1:
                                using (var txn = db.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        tot.equip.BranchId = tot.usr.bCode;
                                        tot.equip.CustomerCode = tot.usr.cCode;
                                        db.MaiEquipment.Add(tot.equip);
                                        db.SaveChanges();

                                        
                                        if (tot.specifications.Count() > 0)
                                        {
                                            foreach (var spec in tot.specifications)
                                            {
                                                spec.RecordId = tot.equip.RecordId;
                                                spec.Sno = sno;
                                                spec.BranchId = tot.usr.bCode;
                                                spec.CustomerCode = tot.usr.cCode;
                                                sno++;
                                            }
                                            db.MaiEquipmentSpecifications.AddRange(tot.specifications);
                                        }
                                        sno = 1;
                                        if (tot.prevmaintenances.Count() > 0)
                                        {
                                            foreach (var main in tot.prevmaintenances)
                                            {
                                                main.RecordId = tot.equip.RecordId;
                                                main.Sno = sno;
                                                main.BranchId = tot.usr.bCode;
                                                main.CustomerCode = tot.usr.cCode;
                                                sno++;
                                            }
                                            db.MaiEquipmentPreventiveMaintenances.AddRange(tot.prevmaintenances);
                                        }


                                        db.SaveChanges();

                                        txn.Commit();
                                        msg = "OK";
                                    }
                                    catch (Exception ex)
                                    {
                                        txn.Rollback();
                                        msg = ex.Message;
                                    }
                                }


                                break;
                            case 2:
                                var upd = db.MaiEquipment.Where(a => a.RecordId == tot.equip.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                upd.EquipmentCode = tot.equip.EquipmentCode;
                                upd.EquipmentName = tot.equip.EquipmentName;
                                upd.EquipmentGroup = tot.equip.EquipmentGroup;
                                upd.PreferableSparesSupplier = tot.equip.PreferableSparesSupplier;
                                upd.PreferableServiceSupplier = tot.equip.PreferableServiceSupplier;
                                upd.MobileCheck = tot.equip.MobileCheck;
                                upd.Roomno = tot.equip.Roomno;
                                upd.AmcDate=tot.equip.AmcDate;
                                upd.LastPmdate = tot.equip.LastPmdate;
                                upd.MaxHrs = tot.equip.MaxHrs;
                                 sno = db.MaiEquipmentSpecifications.Where(a => a.RecordId == tot.equip.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).Max(b => b.Sno);
                                if(sno==null)
                                {
                                    sno = 1;
                                }
                                else
                                {
                                    sno++;
                                }
                                var specsupd = db.MaiEquipmentSpecifications.Where(a => a.RecordId == tot.equip.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                if (tot.specifications.Count() > 0)
                                {
                                    if(specsupd.Count() > 0)
                                    {
                                        db.MaiEquipmentSpecifications.RemoveRange(specsupd);
                                    }
                                    foreach (var spec in tot.specifications)
                                    {
                                        spec.RecordId = tot.equip.RecordId;
                                        spec.Sno = sno;
                                        spec.BranchId = tot.usr.bCode;
                                        spec.CustomerCode = tot.usr.cCode;
                                        sno++;
                                    }
                                    db.MaiEquipmentSpecifications.AddRange(tot.specifications);
                                }

                                sno = db.MaiEquipmentSpecifications.Where(a => a.RecordId == tot.equip.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).Max(b => b.Sno);
                                if (sno == null)
                                {
                                    sno = 1;
                                }
                                else
                                {
                                    sno++;
                                }
                                var pmupd = db.MaiEquipmentPreventiveMaintenances.Where(a => a.RecordId == tot.equip.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                if (tot.prevmaintenances.Count() > 0)
                                {
                                    if (pmupd.Count() > 0)
                                    {
                                        db.MaiEquipmentPreventiveMaintenances.RemoveRange(pmupd);
                                    }
                                    foreach (var pm in tot.prevmaintenances)
                                    {
                                        pm.RecordId = tot.equip.RecordId;
                                        pm.Sno = sno;
                                        pm.BranchId = tot.usr.bCode;
                                        pm.CustomerCode = tot.usr.cCode;
                                        sno++;
                                    }
                                    db.MaiEquipmentPreventiveMaintenances.AddRange(tot.prevmaintenances);
                                }
                                db.SaveChanges();
                                msg = "OK";

                                break;
                            case 3:
                                var del = db.MaiEquipment.Where(a => a.RecordId == tot.equip.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                var specsdel = db.MaiEquipmentSpecifications.Where(a => a.RecordId == tot.equip.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                var pmdel = db.MaiEquipmentPreventiveMaintenances.Where(a => a.RecordId == tot.equip.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                if (pmdel.Count() > 0)
                                {
                                    db.MaiEquipmentPreventiveMaintenances.RemoveRange(pmdel);
                                }
                                if (specsdel.Count() > 0)
                                {
                                    db.MaiEquipmentSpecifications.RemoveRange(specsdel);
                                }
                                if(del !=null)
                                {
                                    db.MaiEquipment.Remove(del);
                                }
                                db.SaveChanges();
                                msg = "OK";
                                break;
                        }
                    }
                    else
                    {
                        msg = "This equipment code or name already existed";
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
        private Boolean dupCheck(MaiEquipmentTotal tot)
        {
            Boolean b = true;
            UsineContext db = new UsineContext();
            switch(tot.traCheck)
            {
                case 1:
                    var cre = db.MaiEquipment.Where(a => (a.EquipmentCode == tot.equip.EquipmentCode || a.EquipmentName == tot.equip.EquipmentName) && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (cre != null)
                        b = false;
                    break;
                    case 2:
                    var upd1 = db.MaiEquipment.Where(a => a.RecordId != tot.equip.RecordId &&  a.EquipmentCode == tot.equip.EquipmentCode  && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    var upd2 = db.MaiEquipment.Where(a => a.RecordId != tot.equip.RecordId && a.EquipmentName == tot.equip.EquipmentName  && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();

                 
                    if (upd1 != null || upd2 != null)
                        b = false;
                    break;

            }

            return b;
        }

    }
}
