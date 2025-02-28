using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Usine_Core.Controllers.Inventory
{
   public class InvStoreTotal
    {
        public InvStores store { get; set; }
        public int? tracheck { get; set; }
        public String result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class InvStoresController : Controller
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        [HttpPost]
        [Authorize]
        [Route("api/invstore/GetInvStores")]
        public IQueryable<InvStores> GetInvStores([FromBody]UserInfo usr)
        {
            return db.InvStores.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId);
        }

        [HttpPost]
        [Authorize]
        [Route("api/invstore/GetInvStore")]
        public InvStores GetInvStore([FromBody]GeneralInformation inf)
        {
            return db.InvStores.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
        }

        [HttpPost]
        [Authorize]
        [Route("api/invstore/setInvStore")]
        public InvStoreTotal setInvStore([FromBody]InvStoreTotal tot)
        {
            String msg = "";
            try
            {
                if(dupCheck(tot))
                {
                    if (ac.screenCheck(tot.usr, 3, 1, 2, (int)tot.tracheck))
                    {
                        switch(tot.tracheck)
                        {
                            case 1:
                                var store = new InvStores();
                                store.StoreCode = tot.store.StoreCode;
                                store.StoreName = tot.store.StoreName;
                                store.StoreIncharge = tot.store.StoreIncharge;
                                store.BranchId = tot.usr.bCode;
                                store.CustomerCode = tot.usr.cCode;
                                store.Pos = 1;
                                db.InvStores.Add(store);
                                db.SaveChanges();
                                msg = "OK";
                                break;
                            case 2:
                                var store1 = db.InvStores.Where(a => a.RecordId == tot.store.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                store1.StoreCode = tot.store.StoreCode;
                                store1.StoreName = tot.store.StoreName;
                                store1.StoreIncharge = tot.store.StoreIncharge;
                                db.SaveChanges();
                                msg = "OK";
                                break;
                            case 3:
                                var store2 = db.InvStores.Where(a => a.RecordId == tot.store.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                db.InvStores.Remove(store2);
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
                else
                {
                    msg = tot.tracheck == 3 ? "This store is in use deletion is not possible" : "This store is already existed";
                }
            }
            catch(Exception ee)
            {
                msg = ee.Message;
            }


            tot.result = msg;
            return tot;
        }
        private Boolean dupCheck(InvStoreTotal tot)
        {
            Boolean b = false;
            switch(tot.tracheck)
            {
                case 1:
                    var x = db.InvStores.Where(a => a.StoreCode == tot.store.StoreCode && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(x==null)
                    {
                       b= true;
                    }
                    break;
                case 2:
                    var y = db.InvStores.Where(a => a.StoreCode == tot.store.StoreCode && a.RecordId != tot.store.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
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
