using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data.SqlClient;
using Usine_Core.others;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.Admin
{
    public class AdmTargetList
    {
        public String frmdate { get; set; }
        public String todate { get; set; }
        public String module { get; set; }
        public double? target { get; set; }
    }
    public class AdmTargetsTotal
    {
        public List<AdmTargetList> targets { get; set; }
        public String result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class MisCoveringLetterDetailsTotal
    {
        public MisCoveringLetterDetails detail { get; set; }
        public UserInfo usr { get; set; }
    }


    public class AdminTargetsController : ControllerBase
    {
        UsineContext db = new UsineContext();

       

      
        [HttpPost]
        [Authorize]
        [Route("api/AdminTargets/getActivitiesListUserWise")]
        public List<ActvityInformation> getActivitiesListUserWise([FromBody] UserInfo usr)
        {
            List<ActvityInformation> list = new List<ActvityInformation>();
            AdminControl ac = new AdminControl();
            DateTime dat = DateTime.Parse(ac.strDate(ac.getPresentDateTime()));
            var accounts = db.TransactionsAudit.Where(a => a.Dat >= dat && a.Usr == usr.uCode && a.CustomerCode == usr.cCode).OrderBy(b => b.Dat).ToList();
            List<ActivityList> lst = new List<ActivityList>();

            foreach (TransactionsAudit acc in accounts)
            {
                String title = acc.Syscode;
              
                switch(acc.TraModule.ToUpper())
                {
                    case "FIN":
                        title = title + " (Accounts)";
                        break;
                    case "PUR":
                        title = title + " (Purchases)";
                        break;
                }
                lst.Add(new ActivityList
                {
                    title = title,
                    tim = (acc.Dat.Value.Hour < 10 ? "0" : "") + acc.Dat.Value.Hour.ToString() + ":" + (acc.Dat.Value.Minute < 10 ? "0" : "") + acc.Dat.Value.Minute.ToString(),
                    description = acc.Descr,
                    typ = acc.Tratype,
                    typdescr = acc.Tratype == 1 ? "Entry" : (acc.Tratype == 2 ? "Updation" : "Deletion")
                });

            }
            if (lst.Count() > 0)
            {
                list.Add(new ActvityInformation
                {
                    module = "Accounts",
                    activities = lst
                });
            }

            return list;
        }


        [HttpPost]
        [Authorize]
        [Route("api/AdminTargets/GetMisCoveringLetterDetails")]
        public MisCoveringLetterDetails GetMisCoveringLetterDetails([FromBody] GeneralInformation inf)
        {
            return db.MisCoveringLetterDetails.Where(a => a.Typ == inf.detail && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
        }

        [HttpPost]
        [Authorize]
        [Route("api/AdminTargets/SetMisCoveringLetterDetails")]
        public TransactionResult SetMisCoveringLetterDetails([FromBody] MisCoveringLetterDetailsTotal tot)
        {
            AdminControl ac = new AdminControl();
            Boolean b = false;
            string msg = "";
            switch (tot.detail.Typ)
            {
                case "PUR_ENQ":
                    b = ac.screenCheck(tot.usr, 2, 8, 6, 0);
                    break;
                case "PUR_ORD":
                    b = ac.screenCheck(tot.usr, 2, 8, 7, 0);
                    break;
                case "PUR_PRT":
                    b = ac.screenCheck(tot.usr, 2, 8, 8, 0);
                    break;
            }
            if (b)
            {
                try
                {
                    var det = db.MisCoveringLetterDetails.Where(a => a.Typ == tot.detail.Typ && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (det != null)
                    {
                        det.Subjec = tot.detail.Subjec;
                        det.Body = tot.detail.Body;
                        det.Salutation = tot.detail.Salutation;
                        det.Img = tot.detail.Img;

                    }
                    else
                    {
                        tot.detail.BranchId = tot.usr.bCode;
                        tot.detail.CustomerCode = tot.usr.cCode;
                        db.MisCoveringLetterDetails.Add(tot.detail);
                    }
                    db.SaveChanges();
                    msg = "OK";
                }
                catch(Exception ee)
                {
                    msg = ee.Message;
                }
            }
            else
            {
                msg = "You are not authorised for this transaction";
            }
            TransactionResult res = new TransactionResult();
            res.result = msg;
            return res;
        }

    }
}
