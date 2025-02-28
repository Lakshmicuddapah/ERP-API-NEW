using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Complete_Solutions_Core.Controllers.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Complete_Solutions_Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace Complete_Solutions_Core.Controllers.CRM
{
    public class CRMOrderRxTotal
    {
        public CrmOrdersRxuni header { get; set; }
        public List<CrmOrdersRxdet> lines { get; set; }
        public int traCheck { get; set; }
        public UserInfo usr { get; set; }
        public string result { get; set; }
    }
    public class CRMOrdersRxController : Controller
    {
        CompleteContext db = new CompleteContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/CRMRx/getCRMRxOrders1")]
        public List<CrmOrdersRxuni> getCRMRxOrders([FromBody] GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(  inf.frmDate);

            DateTime dat2= DateTime.Parse( inf.toDate ).AddDays(1);

            return db.CrmOrdersRxuni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();

        }
        [HttpPost]
        [Authorize]
        [Route("api/CRMRx/getCRMRxOrder")]
        public CRMOrderRxTotal getCRMRxOrder(GeneralInformation inf)
        {
            CRMOrderRxTotal tot = new CRMOrderRxTotal();
            tot.header = db.CrmOrdersRxuni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            tot.lines = db.CrmOrdersRxdet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.Sno).ToList();
            return tot;
        }
        [HttpPost]
        [Authorize]
        [Route("api/CRMRx/SetCRMRxOrder")]
        public CRMOrderRxTotal SetCRMRxOrder(CRMOrderRxTotal tot)
        {
            String msg = "";
            try
            {
                String res = ac.transactionCheck("CRM", tot.header.Dat, tot.usr);
                if (res == "OK")
                {
                    switch (tot.traCheck)
                    {
                        case 1:
                            if (ac.screenCheck(tot.usr, 7, 2, 4, 1))
                            {
                                using (var txn = db.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        CrmOrdersRxuni header = new CrmOrdersRxuni();
                                        header.Seq = findSeq(tot.usr);
                                        header.Dat = tot.header.Dat;
                                        header.Usr = tot.usr.uCode;
                                        header.ApprovedUsr = tot.usr.uCode;
                                        header.Approveddat = tot.header.Dat;
                                        header.CustomerId = tot.header.CustomerId;
                                        header.Customer = tot.header.Customer;
                                        header.Addr = tot.header.Addr;
                                        header.Country = tot.header.Country;
                                        header.Stat = tot.header.Stat;
                                        header.District = tot.header.District;
                                        header.City = tot.header.City;
                                        header.Zip = tot.header.Zip;
                                        header.Mobile = tot.header.Mobile;
                                        header.Tel = tot.header.Tel;
                                        header.Fax = tot.header.Fax;
                                        header.Email = tot.header.Email;
                                        header.Webid = tot.header.Webid;
                                        header.TypeofSale = tot.header.TypeofSale;
                                        header.BaseAmt = tot.header.BaseAmt;
                                        header.Discount = tot.header.Discount;
                                        header.Taxes = tot.header.Taxes;
                                        header.Roundoff = tot.header.Roundoff;
                                        header.TotalAmt = tot.header.TotalAmt;
                                        header.ValidityDate = tot.header.ValidityDate;
                                        header.Typ = tot.header.Typ;
                                        header.BranchId = tot.usr.bCode;
                                        header.CustomerCode = tot.usr.cCode;
                                        db.CrmOrdersRxuni.Add(header);
                                        db.SaveChanges();
                                        int sno = 1;
                                        foreach(CrmOrdersRxdet line in tot.lines)
                                        {
                                            line.RecordId = header.RecordId;
                                            line.Sno = sno;
                                            line.BranchId = tot.usr.bCode;
                                            line.CustomerCode = tot.usr.cCode;
                                            sno++;
                                        }
                                        db.CrmOrdersRxdet.AddRange(tot.lines);
                                        db.SaveChanges();
                                        msg = "OK";
                                    }
                                    catch (Exception ee)
                                    {
                                        txn.Rollback();
                                        msg = ee.Message;
                                    }
                                }
                            }
                            else
                            {
                                msg = "You are not authorised to create orders";
                            }
                            break;
                        case 2:
                            if (ac.screenCheck(tot.usr, 7, 2, 4, 1))
                            {
                                msg = "OK";
                            }
                            else
                            {
                                msg = "You are not authorised to create orders";
                            }
                                break;
                    }
                }
                else
                {
                    msg = res;
                }
            }
            catch(Exception ee)
            {
                msg = ee.Message;
            }

            tot.result = msg;
            return tot;
        }
        private String findSeq(UserInfo usr)
        {
            int x = 0;
            DateTime dat = ac.getPresentDateTime();
            General g = new General();
            String seq = db.CrmOrdersRxuni.Where(a => a.Dat.Value.Year == dat.Year && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.Seq);
            if (seq != null)
            {
                x = int.Parse(seq.Substring(5, 7));
            }
            x++;
            return "SO" + dat.Year.ToString().Substring(2, 2) + "-" + g.zeroMake(x, 7);
        }
    }
}
