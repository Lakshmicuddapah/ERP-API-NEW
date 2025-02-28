using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.Accounts
{
    public class accAssetsTotal
    {
        public Finassets asset { get; set; }
        public UserInfo usr { get; set; }
        public int? traCheck { get; set; }
        public String result { get; set; }

    }
    public class accAssetsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        [HttpPost]
        [Authorize]
        [Route("api/accAssets/GetAssets")]
        public List<FinassetsView> GetAssets([FromBody] UserInfo usr)
        {
            return db.FinassetsView.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.AssetName).ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/accAssets/SetAsset")]
        public accAssetsTotal SetAsset([FromBody] accAssetsTotal tot)
        {
            String msg = "";
            AdminControl ac = new AdminControl();
            try
            {
                if (ac.screenCheck(tot.usr, 1, 1, 4, (int)tot.traCheck))
                {
                    if (duplicateCheck(tot))
                    {
                        switch (tot.traCheck)
                        {
                            case 1:
                                Finassets asset = new Finassets();
                                asset.AssetName = tot.asset.AssetName;
                                asset.Depreciation = tot.asset.Depreciation;
                                asset.OpeningValue = tot.asset.OpeningValue;
                                asset.Date = ac.DateAdjustFromFrontEnd(tot.asset.Date.Value);
                                asset.BranchId = tot.usr.bCode;
                                asset.CustomerCode = tot.usr.cCode;
                                db.Finassets.Add(asset);
                                db.SaveChanges();
                                msg = "OK";
                                break;
                            case 2:
                                var asst = db.Finassets.Where(a => a.RecordId == tot.asset.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (asst != null)
                                {
                                    asst.AssetName = tot.asset.AssetName;
                                    asst.Depreciation = tot.asset.Depreciation;
                                    asst.OpeningValue = tot.asset.OpeningValue;
                                    asst.Date = tot.asset.Date;
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "Record not found";
                                }
                                break;
                            case 3:
                                var ast = db.Finassets.Where(a => a.RecordId == tot.asset.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (ast != null)
                                {
                                    db.Finassets.Remove(ast);
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "Record not found";
                                }
                                break;
                        }
                    }
                    else
                    {
                        msg = "This asset name is already existed";
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

            tot.result = msg;
            return tot;
        }
        private Boolean duplicateCheck(accAssetsTotal tot)
        {
            Boolean b = false;
            try
            {
                switch (tot.traCheck)
                {
                    case 1:
                        var x = db.Finassets.Where(a => a.AssetName == tot.asset.AssetName && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                        if (x == null)
                            b = true;
                        break;
                    case 2:
                        var y = db.Finassets.Where(a => a.RecordId != tot.asset.RecordId && a.AssetName == tot.asset.AssetName && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                        if (y == null)
                            b = true;
                        break;
                    case 3:
                        b = true;
                        break;
                }
            }
            catch
            {

            }
            return b;
        }

    }
}
