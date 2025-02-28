using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using System.Data;
using Usine_Core.Controllers.Others;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace Usine_Core.Controllers.Accounts
{
    public class accKeyGroupsList
    {
        public List<accAccountGroupDetails> grps { get; set; }
        public string pdffile { get; set; }
        public string excelfile { get; set; }
    }
    public class accKeyAccountsList
    {
        public List<AccountDetails> accs { get; set; }
        public string pdffile { get; set; }
        public string excelfile { get; set; }
    }
    public class accKeyAssetsList
    {
        public List<FinassetsView> assets { get; set; }
        public string pdffile { get; set; }
        public string excelfile { get; set; }
    }
     

    public class accKeyReportsController : Controller
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        private readonly IHostingEnvironment ho;
        public accKeyReportsController(IHostingEnvironment hostingEnvironment)
        {

            ho = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        [Route("api/accKeyReports/getAccKeyRepAccountGroups")]
        public accKeyGroupsList getAccKeyRepAccountGroups([FromBody]UserInfo usr)
        {
            if (ac.screenCheck(usr, 1, 8, 1, 0))
            {
                accKeyGroupsList lst = new accKeyGroupsList();
                lst.grps = (from a in db.FinAccGroups.Where(a => a.CustomerCode == usr.cCode)
                            select new accAccountGroupDetails
                            {
                                mainGroup = a.MGrp,
                                subGroup = a.SGrp,
                                statu = a.Statu == 1 ? "Active" : "Inactive"
                            }).OrderBy(b => b.mainGroup).ThenBy(c => c.subGroup).ToList();

                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("subgrp", typeof(string));
                dt.Columns.Add("maingrp", typeof(string));
                dt.Columns.Add("statu", typeof(string));

                int i = 1;
                foreach (accAccountGroupDetails grp in lst.grps)
                {
                    dt.Rows.Add(i.ToString(), grp.subGroup, grp.mainGroup, grp.statu);
                    i++;
                }
                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Sub Group");
                titles.Add("Main Group");
                titles.Add("Status");
                float[] widths = { 50f, 200f, 200f, 100f };
                int[] aligns = { 3, 1, 1, 1 };

                lst.pdffile = usr.uCode + "ACCKEYREPS" + usr.cCode + usr.bCode + ".pdf";
                lst.excelfile = usr.uCode + "ACCKEYREPS" + usr.cCode + usr.bCode + ".xlsx";

                PDFExcelMake ep = new PDFExcelMake();

                ep.pdfConversion(ho.WebRootPath + "\\Reps\\" + lst.pdffile, "List of Account Groups", usr, dt, titles, widths, aligns, false);
                ep.makeExcelConversion(ho.WebRootPath + "\\Reps\\" + lst.excelfile, "List of Account Groups", usr, dt, titles, widths, aligns, false);
                return lst;
            }
            else

            {
                return null;
            }
        }


        [HttpPost]
        [Authorize]
        [Route("api/accKeyReports/getAccKeyRepAccountDetails")]
        public accKeyAccountsList getAccKeyRepAccountDetails([FromBody]UserInfo usr)
        {
            if (ac.screenCheck(usr, 1, 8, 1, 0))
            {
                accKeyAccountsList lst = new accKeyAccountsList();
                try
                {
                    lst.accs = (from a in db.FinAccounts.Where(a => a.CustomerCode == usr.cCode)
                                join b in db.FinAccGroups.Where(a => a.CustomerCode == usr.cCode) on a.Accgroup equals b.RecordId
                                select new AccountDetails
                                {
                                    accountId = a.Recordid,
                                    accountname = a.Accname,
                                    grp = b.SGrp,
                                    grpid = b.RecordId,
                                    address = a.Address,
                                    country = a.Country,
                                    stat = a.State,
                                    district = a.District,
                                    city = a.City,
                                    zip = a.Pin,
                                    mobile = a.Mobile,
                                    tel = a.Tel,
                                    fax = a.Fax,
                                    email = a.Email,
                                    webid = a.WebId,
                                    actype = a.AcType,
                                    acchk = a.AcChk
                                }).OrderBy(b => b.accountname).ToList();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("sno", typeof(string));
                    dt.Columns.Add("account", typeof(string));
                    dt.Columns.Add("grp", typeof(string));
                    dt.Columns.Add("mobile", typeof(string));
                    dt.Columns.Add("email", typeof(string));
                    dt.Columns.Add("statu", typeof(string));

                    int i = 1;
                    foreach (AccountDetails acc in lst.accs)
                    {
                        dt.Rows.Add(i.ToString(), acc.accountname, acc.grp, acc.mobile, acc.email, acc.acchk == 1 ? "Active" : "Inactive");
                        i++;
                    }
                    List<string> titles = new List<string>();
                    titles.Add("#");
                    titles.Add("Account");
                    titles.Add("Group");
                    titles.Add("Mobile");
                    titles.Add("E-Mail");
                    titles.Add("Status");
                    float[] widths = { 30f, 200f, 120f, 80f, 70f, 50f };
                    int[] aligns = { 3, 1, 1, 1, 1, 1 };

                    lst.pdffile = usr.uCode + "ACCKEYREPS" + usr.cCode + usr.bCode + ".pdf";
                    lst.excelfile = usr.uCode + "ACCKEYREPS" + usr.cCode + usr.bCode + ".xlsx";

                    PDFExcelMake ep = new PDFExcelMake();

                    ep.pdfConversion(ho.WebRootPath + "\\Reps\\" + lst.pdffile, "List of Accounts", usr, dt, titles, widths, aligns, false);
                    ep.makeExcelConversion(ho.WebRootPath + "\\Reps\\" + lst.excelfile, "List of Accounts", usr, dt, titles, widths, aligns, false);
                    return lst;
                }
                catch (Exception ee)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
       
        [HttpPost]
        [Authorize]
        [Route("api/accKeyReports/getAccKeyRepAssetDetails")]
        public accKeyAssetsList getAccKeyRepAssetDetails([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 1, 8, 1, 0))
            {
                accKeyAssetsList result = new accKeyAssetsList();
                AdminControl ac = new AdminControl();
                result.assets = db.FinassetsView.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.AssetName).ToList();
                General g = new General();
                DataTable dt = new DataTable();
                dt.Columns.Add("sno", typeof(string));
                dt.Columns.Add("asset", typeof(string));
                dt.Columns.Add("opdate", typeof(string));
                dt.Columns.Add("opvalue", typeof(string));
                dt.Columns.Add("currvalue", typeof(string));


                int i = 1;
                foreach (var ass in result.assets)
                {
                    dt.Rows.Add(i.ToString(), ass.AssetName, ac.strDate((DateTime)ass.Opedate), ass.Opvalue, ass.Presetnvalue);
                    i++;
                }
                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Asset");
                titles.Add("Op Date");
                titles.Add("Op Value");
                // titles.Add("E-Mail");
                titles.Add("Curr Value");
                float[] widths = { 30f, 220f, 120f, 90f, 90f };
                int[] aligns = { 3, 1, 1, 2, 2 };

                result.pdffile = usr.uCode + "ACCKEYREPS" + usr.cCode + usr.bCode + ".pdf";
                result.excelfile = usr.uCode + "ACCKEYREPS" + usr.cCode + usr.bCode + ".xlsx";

                PDFExcelMake ep = new PDFExcelMake();

                ep.pdfConversion(ho.WebRootPath + "\\Reps\\" + result.pdffile, "List of Assets", usr, dt, titles, widths, aligns, false);
                ep.makeExcelConversion(ho.WebRootPath + "\\Reps\\" + result.excelfile, "List of Assets", usr, dt, titles, widths, aligns, false);
                return result;
            }
            else
            {
                return null;
            }

        }

    }
}
