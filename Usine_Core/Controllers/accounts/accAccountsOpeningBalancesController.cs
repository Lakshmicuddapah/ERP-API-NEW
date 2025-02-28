using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.Accounts
{
    public class TotalOpeningBalances
    {
        public int? recordId { get; set; }
        public string accname { get; set; }
        public double? deb { get; set; }
        public double? cre { get; set; }
    }
    public class AccOpeningBalancesInfo
    {
        public DateTime dat { get; set; }
        public IQueryable<TotalOpeningBalances> accdet { get; set; }
    }
    public class AccOpeningBalancesTotal
    {
        public List<TotalOpeningBalances> accdet { get; set; }
        public UserInfo usr { get; set; }
        public String result { get; set; }
    }
    public class accAccountsOpeningBalancesController : ControllerBase
    {
        AdminControl ac = new AdminControl();
        UsineContext db = new UsineContext();

        [HttpPost]
        [Authorize]
        [Route("api/accOpenings/GetOpeningBalances")]
        public AccOpeningBalancesInfo GetOpeningBalances([FromBody] UserInfo usr)
        {

            DateTime dat = ac.getFinancialStart(ac.getPresentDateTime(), usr);



            var details = (from a in db.FinAccounts.Where(a => a.CustomerCode == usr.cCode)
                           join b in
                           (from a in db.FinexecUni.Where(a => a.Dat == dat && a.Tratype == "OPB" && a.CustomerCode == usr.cCode)
                            join b in db.FinexecDet.Where(a => a.CustomerCode == usr.cCode) on a.RecordId equals b.RecordId
                            select new
                            {
                                recordID = a.RecordId,
                                sno = b.Sno,
                                accname = b.Accname,
                                cre = b.Cre,
                                deb = b.Deb

                            }) on a.Recordid equals b.accname
                           select new TotalOpeningBalances
                           {
                               recordId = a.Recordid,
                               accname = a.Accname,
                               deb = b.deb,
                               cre = b.cre
                           });


            return new AccOpeningBalancesInfo { dat = dat, accdet = details };
        }

        [HttpPost]
        [Authorize]
        [Route("api/accOpenings/setOpeningBalances")]
        public AccOpeningBalancesTotal setOpeningBalances([FromBody] AccOpeningBalancesTotal tot)
        {
            String msg = "";
            if (ac.screenCheck(tot.usr, 1, 1, 3, 0))
            {
                using (var txn = db.Database.BeginTransaction())
                {
                    try
                    {

                        DateTime dat = ac.getFinancialStart(ac.getPresentDateTime(), tot.usr);
                        var details = db.FinexecDet.Where(a => a.CustomerCode == tot.usr.cCode && a.RecordId == db.FinexecUni.Where(b => b.Dat == dat && b.Tratype == "OPB" && b.CustomerCode == tot.usr.cCode).Select(c => c.RecordId).FirstOrDefault());
                        if (details != null)
                        {
                            if (details.Count() > 0)
                            {
                                db.FinexecDet.RemoveRange(details);
                            }
                        }


                        var detail = db.FinexecUni.Where(a => a.CustomerCode == tot.usr.cCode && a.Tratype == "OPB" && a.Dat == dat).FirstOrDefault();
                        if (detail != null)
                            db.FinexecUni.Remove(detail);

                        FinexecUni uni = new FinexecUni();

                        //  uni.RecordId = idno;
                        uni.Seq = 0;
                        uni.Dat = dat;
                        uni.Narr = "Opening Balances";
                        uni.Tratype = "OPB";
                        uni.Traref = " ";
                        uni.Vouchertype = "OPB";
                        uni.BankDet = " ";
                        uni.Branchid = tot.usr.bCode;
                        uni.CustomerCode = tot.usr.cCode;
                        db.FinexecUni.Add(uni);
                        db.SaveChanges();
                        int i = 1;
                        foreach (TotalOpeningBalances det in tot.accdet)
                        {
                            FinexecDet dets = new FinexecDet();
                            dets.RecordId = uni.RecordId;
                            dets.Sno = i;
                            dets.Accname = det.recordId;
                            dets.Cre = det.cre;
                            dets.Deb = det.deb;
                            dets.Branchid = tot.usr.bCode;
                            dets.CustomerCode = tot.usr.cCode;
                            dets.Dat = dat;
                            db.FinexecDet.Add(dets);
                            i++;
                        }
                        db.SaveChanges();
                        txn.Commit();
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
                msg = "You are not authorised to add opening Balnces";
            }
            tot.result = msg;
            return tot;
        }
        private int? findOpeningId()
        {
            UsineContext db = new UsineContext();
            int? x = 0;
            var xx = db.FinexecUni.FirstOrDefault();
            if (xx != null)
            {
                x = db.FinexecUni.Max(a => a.RecordId);
            }
            x++;
            return x;
        }

    }
}

