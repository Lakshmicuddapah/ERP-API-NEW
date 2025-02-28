using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Controllers.Admin;
 using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
 using Microsoft.AspNetCore.Hosting;
using Usine_Core.Controllers.Others;
namespace Usine_Core.Controllers.quality
{
    public class QCTestTotal
    {
        public QcTestings test { get; set; }
        public int? traCheck { get; set; }
        public UserInfo usr { get; set; }
    }
    
    public class QCMastersController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/QCMasters/GetQCTests")]
        public List<QcTestings> GetQCTests([FromBody] GeneralInformation inf)
        {
            return db.QcTestings.Where(a => a.TestArea == inf.detail && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();
        }

        private Boolean dupCheck(QCTestTotal tot)
        {
            Boolean b = true;
            UsineContext db = new UsineContext();
            switch(tot.traCheck)
            {
                case 1:
                    var cre = db.QcTestings.Where(a => a.Testname == tot.test.Testname && a.TestArea == tot.test.TestArea && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(cre != null)
                    {
                        b= false;
                    }
                    break;
                case 2:
                    var upd = db.QcTestings.Where(a => a.RecordId != tot.test.RecordId && a.Testname == tot.test.Testname && a.TestArea == tot.test.TestArea && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (upd != null)
                    {
                        b = false;
                    }
                    break;
            }


            return b;
        }
        [HttpPost]
        [Authorize]
        [Route("api/QCMasters/SetQcTest")]
        public TransactionResult SetQcTest([FromBody] QCTestTotal tot)
        {
            string msg = "";
            try
            {
                int scrcode=tot.test.TestArea=="MAT"?1:(tot.test.TestArea=="PRO"?2:3);
                if (ac.screenCheck(tot.usr, 11, 1, scrcode, (int)tot.traCheck))
                {
                    if (dupCheck(tot))
                    {
                        switch (tot.traCheck)
                        {
                            case 1:
                                tot.test.BranchId = tot.usr.bCode;
                                tot.test.CustomerCode = tot.usr.cCode;
                                db.QcTestings.Add(tot.test);
                                db.SaveChanges();
                                msg = "OK";
                                break;
                            case 2:
                                var upd = db.QcTestings.Where(a => a.RecordId == tot.test.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if(upd != null)
                                {
                                    upd.Testname=tot.test.Testname;
                                    upd.MicroCheck = tot.test.MicroCheck;
                                    upd.CheckingType=tot.test.CheckingType;
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "No record found to update";
                                }
                                break;
                            case 3:
                                var del = db.QcTestings.Where(a => a.RecordId == tot.test.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (del != null)
                                {
                                    db.QcTestings.Remove(del);
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "No record found to delete";
                                }
                                break;
                        }
                    }
                    else
                    {
                        msg = "This detail is already existed";
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
