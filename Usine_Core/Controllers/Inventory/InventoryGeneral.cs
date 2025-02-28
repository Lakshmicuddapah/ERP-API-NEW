using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using System.Data;
using System.Data.SqlClient;

namespace Usine_Core.Controllers.Inventory
{
    public class InvMaterialOutwardRequirementsTotal
    {
        public List<InvMaterialCompleteDetailsView> totalmaterials { get; set; }
        public List<InvMaterialManagement> ginwiseinfo { get; set; }
    }
    public class InventoryGeneral
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        public InvMaterialOutwardRequirementsTotal GetMaterialOutwardRequirements(UserInfo usr)
        {
            InvMaterialOutwardRequirementsTotal tot = new InvMaterialOutwardRequirementsTotal();
            tot.totalmaterials = (from a in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode)
                            join b in db.InvMaterials.Where(a => a.CustomerCode == usr.cCode)
                            on a.RecordId equals b.RecordId
                            select new InvMaterialCompleteDetailsView
                            {
                                RecordId = a.RecordId,
                                Itemid = a.Itemid,
                                Itemname = a.Itemname,
                                Grpname = a.Grpname,
                                Um = a.Um,
                                Umid = b.CostingType == null ? 1 : b.CostingType
                            }).OrderBy(b => b.Itemname).ToList();

            DateTime dat1 = ac.getFinancialStart(ac.getPresentDateTime(), usr);
            DateTime dat2 = dat1.AddYears(1);
            var det = (from a in db.InvMaterialManagement.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).GroupBy(b => new { b.Gin, b.ItemName, b.Store })
                        select new
                        {
                            gin=a.Key.Gin,
                            itemname=a.Key.ItemName,
                            store=a.Key.Store,
                            available=a.Sum(b => (b.Qtyin-b.Qtyout))
                           
                        }).ToList();
            var det1 = det.Where(a => a.available > 0).ToList();
            var det2 = (from a in db.InvMaterialManagement.Where(a => a.TransactionType < 100 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                        select new
                        {
                            gin=a.Gin,
                            itemname=a.ItemName,
                            rat=a.Rat,
                            batchno=a.BatchNo.Trim()==""?a.Gin:a.BatchNo
                        }).ToList();

            var totaldet = (from a in det1
                            join b in det2 on a.gin equals b.gin
                            join c in tot.totalmaterials on a.itemname equals c.RecordId
                            select new InvMaterialManagement
                            {
                                Gin = a.gin,
                                ItemName = a.itemname,
                                Store = a.store,
                                Qtyin = a.available,
                                Rat = b.rat,
                                BatchNo = b.batchno,
                                Stdum=c.Umid
                            }).ToList();
            tot.ginwiseinfo = new List<InvMaterialManagement>();
            var list1 = totaldet.Where(a => a.Stdum == 3).OrderByDescending(b => b.Gin).ToList();
            var list2 = totaldet.Where(a => a.Stdum != 3).OrderBy(c => c.Gin).ToList();
            if(list1.Count() > 0)
            {
                tot.ginwiseinfo.AddRange(list1);
            }
            if(list2.Count() > 0)
            {
                tot.ginwiseinfo.AddRange(list2);
            }

            return tot;



        }

        public List<InvMaterialCompleteDetailsView> GetInvMaterialsConsumedBatchWise(GeneralInformation inf)
        {
            /*  var dat1 = DateTime.Parse(inf.frmDate);
              var dat2 = DateTime.Parse(inf.toDate).AddDays(1);
              var details=(from a in db.PpcBatchPlanningUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                           join b in db.InvMaterialManagement.Where(a => a.TransactionType == 103 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.RecordId equals b.Department
                           select new
                           {

                           })*/

            return null;
        }
    }
}
