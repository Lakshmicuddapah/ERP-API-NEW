using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Controllers.Others;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Usine_Core.Controllers.crm
{
    public class CRMKeyRepCustomerGroupsTotal
    {
        public string pdfFile { get; set; }
        public string excelFile { get; set; }
        public List<SalcustomerGroups> groups { get; set; }
    }
    public class PartyListOfDetails
    {
        public string partyname { get; set; }
        public string partgroup { get; set; }
        public string partycontact { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string statu { get; set; }
    }
    public class CRMKeyRepCustomersTotal
    {
        public string pdfFile { get; set; }
        public string excelFile { get; set; }
        public List<PartyListOfDetails> customers { get; set; }
    }
    public class CRMKeyReportsController : ControllerBase
    {
        private readonly IHostingEnvironment ho;
        public CRMKeyReportsController(IHostingEnvironment hostingEnvironment)
        {

            ho = hostingEnvironment;
        }

        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/CRMKeyReports/crmeyRepCustomerGroups")]
        public CRMKeyRepCustomerGroupsTotal crmeyRepCustomerGroups([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 7, 9, 1, 0))
            {
                try
                {
                    CRMKeyRepCustomerGroupsTotal tot = new CRMKeyRepCustomerGroupsTotal();
                    tot.groups = db.SalcustomerGroups.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();
                    if (tot.groups.Count > 0)
                    {

                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));
                        dt.Columns.Add("grp", typeof(string));
                        dt.Columns.Add("sgrp", typeof(string));
                        dt.Columns.Add("statu", typeof(string));

                        int i = 1;

                        foreach (SalcustomerGroups det in tot.groups)
                        {
                            dt.Rows.Add(i.ToString(), det.SGrp, det.MGrp, det.Statu == 1 ? "Active" : "Inactive");
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Group");
                        titles.Add("Main Group");
                        titles.Add("Status");

                        float[] widths = { 40f, 200f, 200f, 110f };
                        int[] aligns = { 3, 1, 1, 1 };
                        PDFExcelMake ep = new PDFExcelMake();

                        AdminControl ac = new AdminControl();
                        DateTime dats = DateTime.Now;
                        string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                        string fname = usr.uCode + "CustomerGroups" + dat + usr.cCode + usr.bCode + ".pdf";
                        string filename = ho.WebRootPath + "\\Reps\\" + fname;
                        string msg = "";

                        msg = ep.pdfConversion(filename, "List of Customer Groups", usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname;
                        }
                        string fname1 = usr.uCode + "CustomerGroups" + dat + usr.cCode + usr.bCode + ".xlsx";
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, "List of Customer Groups", usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.excelFile = fname1;
                        }
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
            else
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMKeyReports/CRMKeyRepCustomerDetails")]
        public CRMKeyRepCustomersTotal CRMKeyRepCustomerDetails([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 7, 9, 1, 0))
            {
                try
                {
                    AdminControl ac = new AdminControl();
                    DateTime dats = DateTime.Now;
                    string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                    string fname = usr.uCode + "Customers" + dat + usr.cCode + usr.bCode + ".pdf";
                    string filename = ho.WebRootPath + "\\Reps\\" + fname;
                    string msg = "";


                    CRMKeyRepCustomersTotal tot = new CRMKeyRepCustomersTotal();
                    tot.customers = (from a in db.PartyDetails.Where(a => a.PartyType == "CUS" && a.CustomerCode == usr.cCode)
                                     join b in db.SalcustomerGroups.Where(a => a.CustomerCode == usr.cCode)
                                     on a.PartyGroup equals b.RecordId
                                     select new PartyListOfDetails
                                     {
                                         partyname = a.PartyName,
                                         partgroup = b.SGrp,
                                         partycontact = a.ContactPerson,
                                         mobile = a.ContactMobile,
                                         email = a.ContactEmail,
                                         statu = a.Statu == 1 ? "Active" : "Inactive"
                                     }).OrderBy(a => a.partyname).ToList();
                    if (tot.customers.Count > 0)
                    {

                        DataTable dt = new DataTable();
                        dt.Columns.Add("sno", typeof(string));
                        dt.Columns.Add("party", typeof(string));
                        dt.Columns.Add("sgrp", typeof(string));
                        dt.Columns.Add("contact", typeof(string));
                        dt.Columns.Add("mobile", typeof(string));
                        dt.Columns.Add("email", typeof(string));
                        dt.Columns.Add("statu", typeof(string));

                        int i = 1;

                        foreach (PartyListOfDetails det in tot.customers)
                        {
                            dt.Rows.Add(i.ToString(), det.partyname, det.partgroup, det.partycontact, det.mobile, det.email, det.statu);
                            i++;
                        }

                        List<string> titles = new List<string>();
                        titles.Add("#");
                        titles.Add("Customer");
                        titles.Add("Group");
                        titles.Add("Contact");
                        titles.Add("Mobile");
                        titles.Add("Enail");
                        titles.Add("Status");

                        float[] widths = { 30f, 150f, 100f, 130f, 130f, 130f, 50f };
                        int[] aligns = { 3, 1, 1, 1, 1, 1, 1 };
                        PDFExcelMake ep = new PDFExcelMake();
                        msg = ep.pdfLandscapeConversion(filename, "List of Customers", usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.pdfFile = fname;
                        }
                        string fname1 = usr.uCode + "Customers" + dat + usr.cCode + usr.bCode + ".xlsx";
                        string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                        msg = "";
                        msg = ep.makeExcelConversion(filename1, "List of Customers", usr, dt, titles, widths, aligns, false);
                        if (msg == "OK")
                        {
                            tot.excelFile = fname1;
                        }

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
            else
            {
                return null;
            }
        }
    }
}