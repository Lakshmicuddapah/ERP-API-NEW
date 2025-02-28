using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Usine_Core.Controllers.Inventory
{
    public class InvMaterialDetails
    {
        public int? recordId { get; set; }
        public String itemcode { get; set; }
        public String itemname { get; set; }
        public String grp { get; set; }
        public int? shelflife { get; set; }
        public double? stdrat { get; set; }
        public double? reorderqty { get; set; }
        public String statu { get; set; }
        public String stdum { get; set; }
        public int? stdumId { get; set; }
        public int? costingType { get; set; }
    }
    public class invMaterialTotal
    {
        public InvMaterials mat { get; set; }
        public List<InvMaterialUnits> matum { get; set; }
        public int? tracheck { get; set; }
        public String result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class InvMaterialsUnitsDetails
    {
        public int? recordId { get; set; }
        public int? umid { get; set; }
        public string um { get; set; }
        public double? cofactor { get; set; }
        public int? stdumid { get; set; }
        public string stdum { get; set; }
    }
    public class InvMaterialsController : Controller
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/invmaterial/GetInvMaterials")]
        public List<InvMaterialDetails> GetInvMaterials([FromBody] UserInfo usr)
        {
              var det1 = (from a in db.InvMaterials.Where(a => a.CustomerCode == usr.cCode)
                          join b in db.InvGroups.Where(a => a.CustomerCode == usr.cCode) on a.Grp equals b.RecordId
                          select new
                          {
                              a.RecordId,
                              a.Itemid,
                              a.ItemName,
                              a.StdRate,
                              a.ShelfLifeReqd,
                              a.ReOrderQty,
                              a.Statu,
                              b.SGrp,
                              a.CostingType

                          }).ToList();
               var det2 = (from x in (from a in db.InvMaterialUnits.Where(a => a.CustomerCode == usr.cCode).GroupBy(b => b.RecordId)
                                      select new
                                      {
                                          recordID = a.Key,
                                          um = a.Max(b => b.StdUm)
                                      })
                           join y in db.InvUm.Where(a => a.CustomerCode == usr.cCode) on x.um equals y.RecordId
                           select new
                           {
                               x.recordID,
                               y.Um,
                             umid=  y.RecordId
                           }).ToList();

               return (from a in det1
                       join b in det2 on a.RecordId equals b.recordID
                       select new InvMaterialDetails
                       {
                           recordId =a.RecordId,
                           itemcode=a.Itemid,
                           itemname=a.ItemName,
                           grp=a.SGrp,
                           stdrat=a.StdRate,
                           shelflife=a.ShelfLifeReqd,
                           reorderqty=a.ReOrderQty,
                           statu=a.Statu==1?"Active":"Inactive",
                           stdum=b.Um,
                           stdumId=b.umid,
                           costingType=a.CostingType
                       }
                       ).OrderBy(b => b.itemname).ToList();




        }

        [HttpPost]
        [Authorize]
        [Route("api/invmaterial/GetInvProductsOnly")]
        public List<InvMaterialDetails> GetInvProductsOnly([FromBody] UserInfo usr)
        {
            var det1 = (from a in db.InvMaterials.Where(a => a.CustomerCode == usr.cCode)
                        join b in db.InvGroups.Where(a => a.GroupCode=="PRO" && a.CustomerCode == usr.cCode) on a.Grp equals b.RecordId
                        select new
                        {
                            a.RecordId,
                            a.Itemid,
                            a.ItemName,
                            a.StdRate,
                            a.ShelfLifeReqd,
                            a.ReOrderQty,
                            a.Statu,
                            b.SGrp,
                            a.CostingType

                        }).ToList();
            var det2 = (from x in (from a in db.InvMaterialUnits.Where(a => a.CustomerCode == usr.cCode).GroupBy(b => b.RecordId)
                                   select new
                                   {
                                       recordID = a.Key,
                                       um = a.Max(b => b.StdUm)
                                   })
                        join y in db.InvUm.Where(a => a.CustomerCode == usr.cCode) on x.um equals y.RecordId
                        select new
                        {
                            x.recordID,
                            y.Um,
                            umid = y.RecordId
                        }).ToList();

            return (from a in det1
                    join b in det2 on a.RecordId equals b.recordID
                    select new InvMaterialDetails
                    {
                        recordId = a.RecordId,
                        itemcode = a.Itemid,
                        itemname = a.ItemName,
                        grp = a.SGrp,
                        stdrat = a.StdRate,
                        shelflife = a.ShelfLifeReqd,
                        reorderqty = a.ReOrderQty,
                        statu = a.Statu == 1 ? "Active" : "Inactive",
                        stdum = b.Um,
                        stdumId = b.umid,
                        costingType = a.CostingType
                    }
                    ).OrderBy(b => b.itemname).ToList();



        }
        [HttpGet]
        [Authorize]
        [Route("api/invmaterial/GetInvMaterial")]
        public invMaterialTotal GetInvMaterial(int recordId,int cCode)
        {
           var usage = db.InvMaterialManagement.Where(a => a.ItemName == recordId && a.CustomerCode ==  cCode).FirstOrDefault();
            invMaterialTotal tot = new invMaterialTotal();
            tot.mat = (from a in db.InvMaterials.Where(a => a.RecordId == recordId && a.CustomerCode == cCode)
                       select new InvMaterials
                       {
                           RecordId = a.RecordId,
                           Itemid = a.Itemid,
                           ItemName = a.ItemName,
                           Grp = a.Grp,
                           StdRate = a.StdRate,
                           ReOrderQty = a.ReOrderQty,
                           ShelfLifeReqd = a.ShelfLifeReqd,
                           InventoryReqd = a.InventoryReqd,
                           Statu = a.Statu,
                           Pic = a.Pic,
                           VendorId = a.VendorId,
                           CostingType = a.CostingType
                       }).FirstOrDefault();

           tot.matum = (from a in  db.InvMaterialUnits.Where(a => a.RecordId == recordId && a.CustomerCode == cCode)
                        select new InvMaterialUnits
                        {
                            Record=a.Record,
                            Sno=a.Sno,
                            Um=a.Um,
                            StdUm=a.StdUm,
                            ConversionFactor=a.ConversionFactor
                        }).OrderBy(b => b.Sno).ToList();
            tot.tracheck = usage == null ? 1 : 0;

            return tot;
        }

        


        [HttpPost]
        [Authorize]
        [Route("api/invmaterial/GetInvMaterialUnits")]
        public List<InvMaterialsUnitsDetails> GetInvMaterialUnits(UserInfo usr)
        {
            try
            {
                return (from x in (from a in db.InvMaterialUnits.Where(a => a.CustomerCode == usr.cCode)
                                   join b in db.InvUm.Where(a => a.CustomerCode == usr.cCode) on a.Um equals b.RecordId
                                   select new
                                   {
                                       recordid = a.RecordId,
                                       umid = b.RecordId,
                                       um = b.Um,
                                       cfactor = a.ConversionFactor,
                                       stdumid = a.StdUm
                                   })
                        join y in db.InvUm.Where(a => a.CustomerCode == usr.cCode) on x.stdumid equals y.RecordId
                        select new InvMaterialsUnitsDetails
                        {
                            recordId=x.recordid,
                            umid=x.umid,
                            um=x.um,
                            cofactor=x.cfactor,
                            stdumid=y.RecordId,
                            stdum=y.Um
                        }).ToList();
            }
            catch
            {
                return null;
            }
            
        }


        [HttpPost]
        [Authorize]
        [Route("api/invmaterial/setInvMaterial")]
        public TransactionResult setInvMaterial([FromBody] invMaterialTotal tot)
        {
            String msg = "";
            AdminControl ac = new AdminControl();
            
            try
            {
                if (dupCheck(tot))
                {
                    switch (tot.tracheck)
                    {
                        case 1:
                            if (ac.screenCheck(tot.usr, 3, 1, 4, 1))
                            {
                                
                                tot.mat.BranchId = tot.usr.bCode;
                                tot.mat.CustomerCode = tot.usr.cCode;
                                db.InvMaterials.Add(tot.mat);
                                db.SaveChanges();
                                int x = 1;
                                foreach( InvMaterialUnits det in tot.matum)
                                {
                                    det.RecordId = tot.mat.RecordId;
                                    det.Sno = x;
                                    det.BranchId = tot.usr.bCode;
                                    det.CustomerCode = tot.usr.cCode;
                                    x++;

                                }
                                if(tot.matum.Count > 0)
                                db.InvMaterialUnits.AddRange(tot.matum);
                                db.SaveChanges();

                                msg = "OK";
                            }
                            else
                            {
                                msg = "You are not authorised to create materials";
                            }
                            break;
                        case 2:
                            if (ac.screenCheck(tot.usr, 2, 1, 4, 2))
                            {
                                var material = db.InvMaterials.Where(a => a.RecordId == tot.mat.RecordId && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (material != null)
                                {
                                    var id = material.RecordId;
                                    material.Itemid = tot.mat.Itemid;
                                    material.ItemName = tot.mat.ItemName;
                                    material.Grp = tot.mat.Grp;
                                    material.StdRate = tot.mat.StdRate;
                                    material.ReOrderQty = tot.mat.ReOrderQty;
                                    material.ShelfLifeReqd = tot.mat.ShelfLifeReqd;
                                    material.InventoryReqd = tot.mat.InventoryReqd;
                                    material.Statu = tot.mat.Statu;
                                    material.CostingType = tot.mat.CostingType;

                                    var matdets = db.InvMaterialUnits.Where(a => a.RecordId == id && a.CustomerCode == tot.usr.cCode);
                                    db.InvMaterialUnits.RemoveRange(matdets);
                                    db.SaveChanges();
                                    int x = 1;
                                    foreach (InvMaterialUnits det in tot.matum)
                                    {
                                        det.RecordId = id;
                                        det.Sno = x;
                                        det.BranchId = tot.usr.bCode;
                                        det.CustomerCode = tot.usr.cCode;
                                        x++;

                                    }
                                    db.InvMaterialUnits.AddRange(tot.matum);
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "Record not found";
                                }

                              
                            }
                            else
                            {
                                msg = "You are not authorised to modify materials";
                            }
                            break;
                        case 3:
                            if (ac.screenCheck(tot.usr, 2, 1, 4, 3))
                            {
                                var material = db.InvMaterials.Where(a => a.RecordId == tot.mat.RecordId && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (material != null)
                                {
                                   


                                    var matdets = db.InvMaterialUnits.Where(a => a.RecordId == tot.mat.RecordId && a.CustomerCode == tot.usr.cCode);
                                    db.InvMaterialUnits.RemoveRange(matdets);
                                    db.InvMaterials.Remove(material);

                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "Record not found";
                                }


                            }
                            else
                            {
                                msg = "You are not authorised to create materials";
                            }
                            break;
                    }
                }
                else
                {
                    msg = tot.tracheck == 3 ? "This material is in use deletion not possible" : "This material name is already existed";
                }
            }
            catch(Exception ee)
            {
                msg = tot.tracheck == 3 ? "This material is in use deletion not possible" : ee.Message;
            }

            TransactionResult res = new TransactionResult();
            res.recordId = tot.mat.RecordId;
            res.result = msg;
            return res;
        }
        private Boolean dupCheck(invMaterialTotal tot)
        {
            Boolean b = false;
            switch(tot.tracheck)
            {
                case 1:
                    var x = db.InvMaterials.Where(a => a.ItemName == tot.mat.ItemName && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(x==null)
                    {
                        b = true;
                    }
                    break;
                case 2:
                    var y = db.InvMaterials.Where(a => a.ItemName == tot.mat.ItemName && a.RecordId != tot.mat.RecordId && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (y == null)
                    {
                        b = true;
                    }
                    break;
                case 3:
                    return true;
                    break;
            }

            return b;
        }
         

    }
}
