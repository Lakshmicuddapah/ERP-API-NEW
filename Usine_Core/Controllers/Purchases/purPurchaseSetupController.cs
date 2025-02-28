using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;

namespace Usine_Core.Controllers.Purchases
{
     public class PurPurchaseOrderTermsTotal
    {
        public int? recordId { get; set; }
        public string term { get; set; }
        public int tracheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class purPurchaseSetupController : Controller
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        [HttpPost]
        [Authorize]
        [Route("api/purPurchaseSetup/GetPurchaseTerms")]
        public List<PurTerms> GetPurchaseTerms([FromBody] UserInfo usr)
        {
            return db.PurTerms.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.Slno).ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/purPurchaseSetup/setPurchaseTerm")]
        public TransactionResult setPurchaseOrderTerm([FromBody] PurPurchaseOrderTermsTotal tot)
        {
            string msg = "";
            try
            {
                if (ac.screenCheck(tot.usr, 2, 8, 2, 0))
                {


                    switch (tot.tracheck)
                    {
                        case 1:
                            PurTerms term = new PurTerms();
                              term.Term = tot.term;
                            term.BranchId = tot.usr.bCode;
                            term.CustomerCode = tot.usr.cCode;
                            db.PurTerms.Add(term);
                            db.SaveChanges();
                            msg = "OK";
                            break;
                        case 3:
                            var ter = db.PurTerms.Where(a => a.Term == tot.term && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            if(ter != null)
                            {
                                db.PurTerms.Remove(ter);
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
                    msg = "You are not authorised for terms";
                }
            }
            catch(Exception ee)
            {
                msg = ee.Message;
            }
            TransactionResult res = new TransactionResult();
            res.result = msg;
            return res;
        }
     
        }
    }

