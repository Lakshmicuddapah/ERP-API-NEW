using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace Usine_Core.Controllers.Inventory
{
    public class InvOpeningStockDetails
    {
        public DateTime dat { get; set; }
        public List<InvStores> stores { get; set; }
        public List<invStockInformation> ostocks { get; set; }
    }
    public class invStockInformation
    {
        public int? recordId { get; set; }
        public string itemCode { get; set; }
        public string itemname { get; set; }
        public string gin { get; set; }
        public string batchNo { get; set; }
        public DateTime? manudate { get; set; }
        public DateTime? expdate { get; set; }
        public double? qty { get; set; }
        public double? rat { get; set; }
        public int? storeCode { get; set; }
        public int? stdUmId { get; set; }
        public string stdUm { get; set; }
    }
    public class invOpeningStockTotal
    {
        public InvMaterialManagement stock { get; set; }
        public UserInfo usr { get; set; }
        public String result { get; set; }
    }

    public class invOpeningStocksController : Controller
    {
        UsineContext db = new UsineContext();

        [HttpPost]
        [Authorize]
        [Route("api/invOpening/getInvOpeningStocks")]
        public InvOpeningStockDetails getInvOpeningStocks([FromBody]UserInfo usr)
        {
            AdminControl ac = new AdminControl();
            DateTime dat1 = ac.getFinancialStart(ac.getPresentDateTime(), usr);
            DateTime dat2 = dat1.AddDays(1);
            var stocks = (from a in db.InvMaterialManagement.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode && a.TransactionType == 0)
                          join b in db.InvMaterials.Where(a => a.CustomerCode == usr.cCode)
                          on a.ItemName equals b.RecordId
                          select new
                          {
                              gin = a.Gin,
                              itemid = a.ItemName,
                              itemcode = b.Itemid,
                              itemname = b.ItemName,
                              batchno = a.BatchNo,
                              expdate = a.Expdate,
                              manudate = a.Manudate,
                              store = a.Store,
                              qty = a.Qtyin,
                              stdum = a.Stdum,
                              rat = a.Rat,
                          }).ToList();

            var ums = db.InvUm.Where(a => a.CustomerCode == usr.cCode).ToList();
            var details = (from a in stocks
                           join b in ums
     on a.stdum equals b.RecordId
                           select new invStockInformation
                           {
                               recordId = a.itemid,
                               gin = a.gin,
                               batchNo = a.batchno,
                               manudate = a.manudate,
                               expdate = a.expdate,
                               itemCode = a.itemcode,
                               itemname = a.itemname,
                               qty = a.qty,
                               rat = a.rat,
                               storeCode = a.store,
                               stdUmId = a.stdum,
                               stdUm = b.Um

                           }
                    ).ToList();
            InvOpeningStockDetails info = new InvOpeningStockDetails();
            info.dat = dat1;
            info.ostocks = details;
            info.stores = db.InvStores.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();
            return info;

        }

      


        [HttpPost]
       [Authorize]
        [Route("api/invOpening/setInvOpeningStocks")]
        public invOpeningStockTotal setInvOpeningStocks([FromBody]invOpeningStockTotal tot)
        {

            string msg = "";
            AdminControl ac = new AdminControl();
            DateTime dat1 = ac.getPresentDateTime();
            DateTime dat = ac.getFinancialStart(dat1, tot.usr);
            try
            {
                if(ac.screenCheck(tot.usr,3,1,5,0))
                {
                    tot.stock.TransactionId = 0;
                    tot.stock.Sno = findMaxSno();
                    tot.stock.Gin = findMaxGin(dat,tot.usr);
                    tot.stock.Dat = dat;
                    tot.stock.BranchId = tot.usr.bCode;
                    tot.stock.CustomerCode = tot.usr.cCode;
                    db.InvMaterialManagement.Add(tot.stock);
                    db.SaveChanges();
                    msg = "OK";
                }
                else
                {
                    msg = "You are not authorised to add opening stocks";
                }
            }
            catch(Exception ee)
            {
                msg = ee.Message;
            }

            tot.result = msg;
            return tot;
        }
        [HttpPost]
        [Authorize]
        [Route("api/invOpening/DeleteOpeningStock")]
        public GeneralInformation DeleteOpeningStock([FromBody] GeneralInformation inf)
        {
            String msg = "";
            try
            {
                if (duplicateCheck(inf))
                {
                    var det = db.InvMaterialManagement.Where(a => a.Gin == inf.detail && a.TransactionType == 0 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if (det != null)
                    {
                        db.InvMaterialManagement.Remove(det);
                        db.SaveChanges();
                        msg = "OK";
                    }
                    else
                    {
                        msg = "No record found";
                    }
                }
                else
                {
                    msg = "This LOT is already in use deletion is not possible";
                }
            }
            catch(Exception ee)
            {
                msg = ee.Message;
            }
            inf.detail = msg;
            return inf;
        }
        private Boolean duplicateCheck(GeneralInformation inf)
        {
            var xx = db.InvMaterialManagement.Where(a => a.Gin == inf.detail && a.TransactionType != 0 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            if(xx==null)
            {
                return true;
            }
            else
            {
                return true;
            }
        }
        private int? findMaxSno()
        {
            int? x = 0;
            var xx = db.InvMaterialManagement.Where(a => a.TransactionId == 0 && a.TransactionType == 0).FirstOrDefault();
            if(xx != null)
            {
                x = db.InvMaterialManagement.Where(a => a.TransactionId == 0 && a.TransactionType == 0).Max(s => s.Sno);
            }
            x++;
            return x;
        }
        private String findMaxGin(DateTime dat,UserInfo usr)
        {
            String str = dat.Year.ToString().Substring(2, 2);
            General g = new General();
            str = str + "0000";

            String x = "";
            var xx = db.InvMaterialManagement.Where(a => a.Gin.Contains(str) && a.CustomerCode == usr.cCode).FirstOrDefault();
            if (xx != null)
            {
                x = db.InvMaterialManagement.Where(a => a.Gin.Contains(str) && a.CustomerCode == usr.cCode).Max(b => b.Gin);
            }
            if(x=="")
            {
                str = str + "00001";
            }
            else
            {
                int a =int.Parse( x.Substring(6, 5));
                a++;
                str = str + g.zeroMake(a, 5);
            }
            return str;
            
             
        }



      
    }
}
