using System;
using System.Collections.Generic;
using Usine_Core.Controllers.Purchases;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;

namespace Usine_Core.Controllers.CRM
{
    public class TaxRequirements
    {
        public List<string> taxes { get; set; }
        public List<AdmTaxes> taxdetails { get; set; }
    }
    public class CRMTaxAssigningsTotal
    {
        public CrmTaxAssigningUni header { get; set; }
        public List<CrmTaxAssigningDet> lines { get; set; }
        public UserInfo usr { get; set; }
        public int? traCheck { get; set; }
    }
    public class TransactionResult
    {
        public int? recordId { get; set; }
        public string result { get; set; }
    }
   
    public class CRMTaxAssigningController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/CRMTaxAssigning/CRMTaxAssignRequirements")]
        public TaxRequirements CRMTaxAssignRequirements([FromBody] UserInfo usr)
        {
             try
            {
                if (ac.screenCheck(usr, 7, 10, 1, 0))
                {
                    TaxRequirements tot = new TaxRequirements();
                    tot.taxes = db.AdmTaxes.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Select(b => b.TaxCode).Distinct().ToList();
                    tot.taxdetails= db.AdmTaxes.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.TaxCode).ThenBy(c => c.TaxPer).ToList();
                    return tot;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }


        [HttpPost]
        [Authorize]
        [Route("api/CRMTaxAssigning/GetCRMTaxAssignings")]
        public List<CrmTaxAssigningUni> GetCRMTaxAssignings([FromBody] UserInfo usr)
        {
             try
            {
                return db.CrmTaxAssigningUni.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMTaxAssigning/GetCRMTaxAssigning")]
        public CRMTaxAssigningsTotal GetCRMTaxAssigning([FromBody] GeneralInformation inf)
        {
             try
            {
                CRMTaxAssigningsTotal tot = new CRMTaxAssigningsTotal();
                tot.header = db.CrmTaxAssigningUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                tot.lines = db.CrmTaxAssigningDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.Sno).ToList();
                return tot;
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMTaxAssigning/SetCRMTaxAssigning")]
        public TransactionResult SetCRMTaxAssigning([FromBody] CRMTaxAssigningsTotal tot)
        {
            string msg = "";
            int sno = 1;
            if (dupTaxCheck(tot))
            {
                if (ac.screenCheck(tot.usr, 7, 10, 1, 0))
                {
                    switch(tot.traCheck)
                    {
                        case 1:
                            tot.header.BranchId = tot.usr.bCode;
                            tot.header.CustomerCode = tot.usr.cCode;
                            db.CrmTaxAssigningUni.Add(tot.header);
                            db.SaveChanges();
                           
                            foreach(CrmTaxAssigningDet line in tot.lines)
                            {
                                line.Sno = sno;
                                line.RecordId = tot.header.RecordId;
                                line.BranchId = tot.header.BranchId;
                                line.CustomerCode = tot.header.CustomerCode;
                                sno++;
                            }
                            db.CrmTaxAssigningDet.AddRange(tot.lines);
                            db.SaveChanges();
                            msg = "OK";
                            break;
                        case 2:
                            var header = db.CrmTaxAssigningUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            header.TaxName= tot.header.TaxName;

                            var lines = db.CrmTaxAssigningDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                            if(lines.Count() > 0)
                            {
                                db.CrmTaxAssigningDet.RemoveRange(lines);
                            }
                            foreach (CrmTaxAssigningDet line in tot.lines)
                            {
                                line.Sno = sno;
                                line.RecordId = tot.header.RecordId;
                                line.BranchId =  header.BranchId;
                                line.CustomerCode = header.CustomerCode;
                                sno++;
                            }
                            db.CrmTaxAssigningDet.AddRange(tot.lines);
                            db.SaveChanges();
                            msg = "OK";
                            break;
                        case 3:
                            try
                            {
                                var headerdel = db.CrmTaxAssigningUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();

                                var linesdel = db.CrmTaxAssigningDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                if (linesdel.Count() > 0)
                                {
                                    db.CrmTaxAssigningDet.RemoveRange(linesdel);
                                    db.SaveChanges();
                                }

                                if (headerdel != null)
                                {
                                    db.CrmTaxAssigningUni.Remove(headerdel);
                                }
                                db.SaveChanges();
                                msg = "OK";
                            }
                            catch (Exception ex)
                            {
                                msg = "This Tax is assigned some where else so deletion is not possible";
                            }
                            break;
                    }
                }
                else
                {
                    msg = "You are not authorised fot this transactions";
                }
            }
            else
            {
                msg = "This name already existed";
            }


            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;
        }
        private Boolean dupTaxCheck(CRMTaxAssigningsTotal tot)
        {
            Boolean b = false;
            switch(tot.traCheck)
            {
                case 1:
                    var cre = db.CrmTaxAssigningUni.Where(a => a.TaxName.ToUpper() == tot.header.TaxName.ToUpper() && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(cre == null)
                    {
                        b = true;
                    }
                    break;
                case 2:
                    var upd = db.CrmTaxAssigningUni.Where(a => a.RecordId != tot.header.RecordId && a.TaxName.ToUpper() == tot.header.TaxName.ToUpper() && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
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
    }
}
