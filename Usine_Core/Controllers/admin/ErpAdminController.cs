using Microsoft.AspNetCore.Mvc;
using Usine_Core.ModelsAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.ModelsAdmin;
//using Complete_Solutions_Core.Models;
//using Complete_Solutions_Core.Others;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using Complete_Solutions_Core.ModelsAdmin;
using System.Net.Mail;
using Usine_Core.others;
using Usine_Core.Models;


namespace Usine_Core.Controllers.admin
{
    public class ErpAdminController : Controller
    {
        public class RegistrationResult
        {
            public int? customerCode { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string result { get; set; }
            public string url { get; set; }
        }

        UsineContext db;
        PrismProductsAdminContext dc;

        public ErpAdminController()
        {
            db = new UsineContext();
            dc = new PrismProductsAdminContext();
        }

        [HttpGet]
        [Route("api/PrismAdmin/getProductsList")]
        public List<ProductDetails> getProductsList()
        {
            return dc.ProductDetails.OrderBy(a => a.ProductCode).ToList();
        }

        [HttpPost]
        [Route("api/PrismAdmin/makeRegistration")]
        public RegistrationResult makeRegistration([FromBody] CustomerRegistrations reg)
          {
            string msg = "";
            RegistrationResult result = new RegistrationResult();
            if (dupRegistrationCheck(reg))
            {
                try
                {
                    StringConversions sc = new StringConversions();
                    // int vendorid = 101;
                    General g = new General();
                    int? idno = 11110;
                    string pwd = RandomString(8);
                    var det = dc.CustomerRegistrations.Max(a => a.CustomerId);
                    if (det != null)
                    {
                        idno = det;
                    }
                    idno++;
                    DateTime regdate = DateTime.Now;

                    string license = g.strDate(regdate) + g.strDate(regdate.AddDays(16));

                    //   reg.CustomerId = idno;
                    reg.Dat = regdate;
                    reg.RegDate = regdate;
                    reg.ExpDate = regdate.AddDays(15);
                    reg.MaxBranches = 1;
                    reg.MaxOutlets = 1;
                    //  reg.VendorId = vendorid;
                    dc.CustomerRegistrations.Add(reg);
                    dc.SaveChanges();
                    idno = reg.CustomerId;
                    CustomerBranches branch = new CustomerBranches();
                    branch.CustomerId = (int)idno;
                    branch.BranchId = "E001";
                    branch.Addr = reg.Addr;
                    branch.Country = reg.Country;
                    branch.Stat = reg.Stat;
                    branch.District = reg.District;
                    branch.City = reg.City;
                    branch.Zip = reg.Zip;
                    branch.Mobile = reg.Mobile;
                    branch.Tel = reg.Tel;
                    branch.Fax = reg.Fax;
                    branch.Email = reg.Email;
                    branch.Web = reg.Web;
                    branch.Outlets = 1;
                    dc.CustomerBranches.Add(branch);
                    List<CustomerModules> modules = new List<CustomerModules>();
                    var mods = dc.ProductModules.Where(a => a.ProductCode == reg.ProductId).OrderBy(b => b.Sno).ToList();
                    if (mods != null)
                    {
                        foreach (var mo in mods)
                        {
                            modules.Add(new CustomerModules
                            {
                                CustomerId = (int)idno,
                                Sno = mo.Sno,
                                ProductCode = mo.ProductCode,
                                Module = mo.ModuleName,
                                ExpDate = regdate.AddDays(15),
                                VendorId = reg.VendorId
                            });
                        }
                    }
                    dc.CustomerModules.AddRange(modules);
                    dc.SaveChanges();

                    db.FinAccGroups.Add(new FinAccGroups { MGrp = "MAIN", SGrp = "ASSETS", Sno = 1, Chk = 0, GroupCode = "FIN", GrpTag = "a", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    db.FinAccGroups.Add(new FinAccGroups { MGrp = "MAIN", SGrp = "LIABILITIES", Sno = 2, Chk = 0, GroupCode = "FIN", GrpTag = "b", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    db.FinAccGroups.Add(new FinAccGroups { MGrp = "MAIN", SGrp = "INCOMES", Sno = 3, Chk = 0, GroupCode = "FIN", GrpTag = "c", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    db.FinAccGroups.Add(new FinAccGroups { MGrp = "MAIN", SGrp = "EXPENDITURE", Sno = 4, Chk = 0, GroupCode = "FIN", GrpTag = "d", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    db.FinAccGroups.Add(new FinAccGroups { MGrp = "ASSETS", SGrp = "CASH IN HAND", Sno = 5, Chk = 0, GroupCode = "CAS", GrpTag = "a_01", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    db.FinAccGroups.Add(new FinAccGroups { MGrp = "ASSETS", SGrp = "CASH AT BANK", Sno = 6, Chk = 0, GroupCode = "BAN", GrpTag = "a_02", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    db.FinAccGroups.Add(new FinAccGroups { MGrp = "ASSETS", SGrp = "SUNDRY DEBITORS", Sno = 7, Chk = 0, GroupCode = "CUS", GrpTag = "a_03", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    db.FinAccGroups.Add(new FinAccGroups { MGrp = "LIABILITIES", SGrp = "SECURED LOANS", Sno = 8, Chk = 0, GroupCode = "BAN", GrpTag = "b_01", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    db.FinAccGroups.Add(new FinAccGroups { MGrp = "LIABILITIES", SGrp = "SUNDRY CREDITORS", Sno = 9, Chk = 0, GroupCode = "SUP", GrpTag = "b_02", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    db.FinAccGroups.Add(new FinAccGroups { MGrp = "ASSETS", SGrp = "MOBILE WALLET", Sno = 10, Chk = 0, GroupCode = "MOB", GrpTag = "a_04", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    db.FinAccGroups.Add(new FinAccGroups { MGrp = "LIABILITIES", SGrp = "CAPITAL", Sno = 11, Chk = 0, GroupCode = "FIN", GrpTag = "b_03", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });

                    db.FinAccGroups.Add(new FinAccGroups { MGrp = "EXPENDITURE", SGrp = "MANUFACTURING EXPENSES", Sno = 12, Chk = 0, GroupCode = "FIN", GrpTag = "d_01", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    db.FinAccGroups.Add(new FinAccGroups { MGrp = "EXPENDITURE", SGrp = "TRADING EXPENSES", Sno = 13, Chk = 0, GroupCode = "FIN", GrpTag = "d_02", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    db.FinAccGroups.Add(new FinAccGroups { MGrp = "EXPENDITURE", SGrp = "ADMINISTRATIVE EXPENSES", Sno = 14, Chk = 0, GroupCode = "FIN", GrpTag = "d_03", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    //db.FinAccGroups.Add(new FinAccGroups { MGrp = "EXPENDITURE", SGrp = "INTERESTS PAYING", Sno = 15, Chk = 0, GroupCode = "FIN", GrpTag = "d_04", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });

                    if (reg.ProductId == "O-MIN" || reg.ProductId == "D-MIN")
                    {
                        db.FinAccGroups.Add(new FinAccGroups { MGrp = "SUNDRY CREDITORS", SGrp = "BANQUET", Sno = 16, Chk = 0, GroupCode = "SUP", GrpTag = "b_02_01", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.FinAccGroups.Add(new FinAccGroups { MGrp = "SUNDRY CREDITORS", SGrp = "LAUNDRY", Sno = 17, Chk = 0, GroupCode = "SUP", GrpTag = "b_02_02", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.FinAccGroups.Add(new FinAccGroups { MGrp = "SUNDRY CREDITORS", SGrp = "INVENTORY", Sno = 18, Chk = 0, GroupCode = "SUP", GrpTag = "b_02_03", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    }

                    if (reg.ProductId == "O-MIN" || reg.ProductId == "D-MIN")
                    {
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "INGREDIENTS", Sno = 1, Chk = 0, GroupCode = "IGR", GrpTag = "a", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "DRINKS & LIQUOR", Sno = 2, Chk = 0, GroupCode = "DIR", GrpTag = "b", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "HOUSEKEEPING", Sno = 3, Chk = 0, GroupCode = "HOU", GrpTag = "c", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "SPARES", Sno = 4, Chk = 0, GroupCode = "SPA", GrpTag = "d", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "HOME", Sno = 5, Chk = 0, GroupCode = "HOM", GrpTag = "e", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "TOOLS", Sno = 6, Chk = 0, GroupCode = "TOO", GrpTag = "f", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "OTHERS", Sno = 7, Chk = 0, GroupCode = "OTH", GrpTag = "g", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });

                    }
                    else if (reg.ProductId == "D-USI")
                    {
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "RAW MATERIALS", Sno = 1, Chk = 0, GroupCode = "RAW", GrpTag = "a", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "PRODUCTS", Sno = 2, Chk = 0, GroupCode = "PRO", GrpTag = "b", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "MAINTENANCE", Sno = 3, Chk = 0, GroupCode = "MAI", GrpTag = "c", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "SPARES", Sno = 4, Chk = 0, GroupCode = "SPA", GrpTag = "d", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "HOME", Sno = 5, Chk = 0, GroupCode = "HOM", GrpTag = "e", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "TOOLS", Sno = 6, Chk = 0, GroupCode = "TOO", GrpTag = "f", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "OTHERS", Sno = 7, Chk = 0, GroupCode = "OTH", GrpTag = "g", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    }
                    else
                    {
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "MATERIALS", Sno = 2, Chk = 0, GroupCode = "PRO", GrpTag = "b", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "MAINTENANCE", Sno = 3, Chk = 0, GroupCode = "MAI", GrpTag = "c", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "SPARES", Sno = 4, Chk = 0, GroupCode = "SPA", GrpTag = "d", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "HOME", Sno = 5, Chk = 0, GroupCode = "HOM", GrpTag = "e", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "TOOLS", Sno = 6, Chk = 0, GroupCode = "TOO", GrpTag = "f", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.InvGroups.Add(new InvGroups { MGrp = "MATERIALS", SGrp = "OTHERS", Sno = 7, Chk = 0, GroupCode = "OTH", GrpTag = "g", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    }

                    if (reg.ProductId == "O-MIN" || reg.ProductId == "D-MIN")
                    {
                        db.HrdDepartments.Add(new HrdDepartments { MGrp = "Departments", SGrp = "Front Office", Sno = 1, Chk = 0, GroupCode = "DEP_01", GrpTag = "DEP", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.HrdDepartments.Add(new HrdDepartments { MGrp = "Departments", SGrp = "F & B Service", Sno = 2, Chk = 0, GroupCode = "DEP_02", GrpTag = "DEP", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.HrdDepartments.Add(new HrdDepartments { MGrp = "Departments", SGrp = "F & B Production", Sno = 3, Chk = 0, GroupCode = "DEP_03", GrpTag = "DEP", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.HrdDepartments.Add(new HrdDepartments { MGrp = "Departments", SGrp = "Housekeeping", Sno = 4, Chk = 0, GroupCode = "DEP_04", GrpTag = "DEP", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.HrdDepartments.Add(new HrdDepartments { MGrp = "Departments", SGrp = "Stores", Sno = 5, Chk = 0, GroupCode = "DEP_05", GrpTag = "DEP", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.HrdDepartments.Add(new HrdDepartments { MGrp = "Departments", SGrp = "Engineering", Sno = 6, Chk = 0, GroupCode = "DEP_06", GrpTag = "DEP", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.HrdDepartments.Add(new HrdDepartments { MGrp = "Departments", SGrp = "Administration", Sno = 7, Chk = 0, GroupCode = "DEP_07", GrpTag = "DEP", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.HrdDepartments.Add(new HrdDepartments { MGrp = "Departments", SGrp = "Others", Sno = 8, Chk = 0, GroupCode = "DEP_08", GrpTag = "DEP", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    }
                    else
                    {
                        db.HrdDepartments.Add(new HrdDepartments { MGrp = "Departments", SGrp = "Production", Sno = 1, Chk = 0, GroupCode = "DEP_01", GrpTag = "DEP", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.HrdDepartments.Add(new HrdDepartments { MGrp = "Departments", SGrp = "Sales", Sno = 2, Chk = 0, GroupCode = "DEP_02", GrpTag = "DEP", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.HrdDepartments.Add(new HrdDepartments { MGrp = "Departments", SGrp = "Front Office", Sno = 3, Chk = 0, GroupCode = "DEP_03", GrpTag = "DEP", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.HrdDepartments.Add(new HrdDepartments { MGrp = "Departments", SGrp = "Maintenance", Sno = 4, Chk = 0, GroupCode = "DEP_04", GrpTag = "DEP", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.HrdDepartments.Add(new HrdDepartments { MGrp = "Departments", SGrp = "Stores", Sno = 5, Chk = 0, GroupCode = "DEP_05", GrpTag = "DEP", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.HrdDepartments.Add(new HrdDepartments { MGrp = "Departments", SGrp = "Technical", Sno = 6, Chk = 0, GroupCode = "DEP_06", GrpTag = "DEP", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.HrdDepartments.Add(new HrdDepartments { MGrp = "Departments", SGrp = "Administration", Sno = 7, Chk = 0, GroupCode = "DEP_07", GrpTag = "DEP", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                        db.HrdDepartments.Add(new HrdDepartments { MGrp = "Departments", SGrp = "Others", Sno = 8, Chk = 0, GroupCode = "DEP_08", GrpTag = "DEP", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    }

                    db.SalcustomerGroups.Add(new SalcustomerGroups { MGrp = "Main", SGrp = "CUSTOMERS", Sno = 1,Chk = 0,GroupCode = "CUS", GrpTag = "a", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });
                    db.PurSupplierGroups.Add(new PurSupplierGroups { MGrp = "Main", SGrp = "SUPPLIERS", Sno = 1, Chk = 0, GroupCode = "SUP", GrpTag = "a", Statu = 1, Branchid = branch.BranchId, CustomerCode = idno });

                    //db.Cardtypes.Add(new Cardtypes { CardType = "MASTER CARD", CustomerCode = idno });
                    //db.Cardtypes.Add(new Cardtypes { CardType = "VISA", CustomerCode = idno });
                    //db.Cardtypes.Add(new Cardtypes { CardType = "AMERICAN EXPRESS", CustomerCode = idno });
                    //db.Cardtypes.Add(new Cardtypes { CardType = "RUPAY", CustomerCode = idno });
                    //db.Cardtypes.Add(new Cardtypes { CardType = "MAESTRO CARD", CustomerCode = idno });

                    db.UsrAut.Add(new UsrAut { UsrName = sc.makeStringToAscii(reg.DefaultUser.ToLower()), RoleName = sc.makeStringToAscii("administrator"), Email = sc.makeStringToAscii(reg.Email), Pwd = sc.makeStringToAscii(pwd.ToString()), Pos = 1, CustomerCode = (int)idno ,WebFreeEnable = 1, MobileFreeEnable = 1 });
                    //db.UsrAut.Add(new UsrAut { UsrName = sc.makeStringToAscii("system"), RoleName = sc.makeStringToAscii("administrator"), Email = sc.makeStringToAscii(" "), Pwd = sc.makeStringToAscii("upanishaT#9$"), Pos = 1, CustomerCode = (int)idno });

                    db.SaveChanges();

                    result.customerCode = idno;
                    result.username = reg.DefaultUser;
                    result.password = pwd;
                    if(reg.ProductId == "D-USI")
                    {
                        result.url = "https://sales.cortracker360.com/";
                    }
                    else if (reg.ProductId == "CORCRM")
                    {
                        result.url = "https://corcrm.cortracker360.com/";
                    }
                    else if (reg.ProductId == "CORHRM")
                    {
                        result.url = "https://corhrm.cortracker360.com/";
                    }
                    msg = "OK";

                    others.sendEmail sendEmail = new others.sendEmail();
                    sendEmail.EmailSend("Get Started With Cortracker", reg.Email, "Hi " + reg.Customer + ",\n\n" + "Welcome to Your Cortracker ERP Software Trial. \n\n" + "Thank you for registering for the trial version of our ERP software. We are pleased to have you on board and look forward to demonstrating how our solution can meet your needs.\r\n\r\nYour trial period will be active for the next 7 days. Should you have any questions or require assistance during this time, please do not hesitate to contact us. Our team is here to support you and ensure you fully explore the capabilities of our software..\n\n" + "Customer Code:" + result.customerCode + "\n\n" + "Username:" + result.username + "\n\n" + "Password:" + result.password + "\n\n" + "Url for Login: " + result.url + "\n \n For any additional information, please contact us accordingly.\n \n Thanks,\n \n Cortracker", null, "saikumar@cortracker360.com", "erp@cortracker360.com");
                    //this.sendMail(reg, pwd);

                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }
            else
            {
                msg = "Registration same product with given mobile/email is already existed";
            }
            result.result = msg;

            return result;
        }
        private Boolean dupRegistrationCheck(CustomerRegistrations reg)
        {
            Boolean b = false;
            var detail = dc.CustomerRegistrations.Where(a => (a.ProductId == reg.ProductId && a.Mobile == reg.Mobile) || (a.ProductId == reg.ProductId && a.Email == reg.Email)).FirstOrDefault();
            if (detail == null)
            {
                b = true;
            }
            return b;
        }
        private string RandomString(int length)
        {
            Random random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
