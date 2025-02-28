using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Usine_Core.others;
using Usine_Core.ModelsAdmin;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.Admin
{
    public class ProductInfo
    {
        public string productcode { get; set; }
        public string customercode { get; set; }
    }
    public class ChangePassword
    {
        public string oldpassword { get; set; }
        public string newpassword { get; set; }
        public UserInfo usr { get; set; }
    }
    public class LoginControlController : ControllerBase
    {
        UsineContext db = new UsineContext();
        StringConversions sc = new StringConversions();
 


        [HttpPost]
        [Route("api/LoginControl/LoginVerification")]
        public UserCompleteInfo LoginVerification([FromBody] LoginControls usr)
        {
            UserCompleteInfo uc = new UserCompleteInfo();
            AdminControl ac = new AdminControl();

            var newUser = sc.makeStringToAscii(usr.userName.ToLower());
            var newpwd = sc.makeStringToAscii(usr.password);

            var log = db.UsrAut.Where(a => a.CustomerCode == usr.customerCode && a.UsrName == newUser && a.Pwd == newpwd && a.Pos > 0).FirstOrDefault();

            if (log == null)
            {
                 return null;
            }
            else
            {
                String[] roles = { "Ravi", "Raja", "Gopal", "Ananth", "Ratnam", "Nagendra" };

                UserInfo uinf = new UserInfo();
                uinf.uCode = usr.userName.ToUpper();
                uinf.rCode = sc.makeAsciitoString(log.RoleName);
                uinf.cCode = usr.customerCode;
                uinf.kCode = GenerateAccessToken(log.Email, Guid.NewGuid().ToString(), roles);
                uinf.vCode = AdminControl.versionNumber;
                uinf.pCode = findProductCode(usr.customerCode);//"D-VEN";
                uinf.webCheck = log.WebFreeEnable;
                uinf.mobileCheck = log.MobileFreeEnable;
                uc.roles = findRoles(uinf.rCode, usr.customerCode);
                uc.assg = findAssignings(uinf.uCode, uinf.rCode, usr.customerCode);

                var br = uc.assg.Where(b => b.typ == "BRA").FirstOrDefault();
                if (br != null)
                {
                    uinf.bCode = br.code;
                }
                else
                {
                    uinf.bCode = "E001";
                }


                uc.usr = uinf;

                uc.addr = makeBranchAddress( uinf.bCode, uinf.cCode);
                var pursets = (from a in db.PurSetup.Where(a => a.BranchId == uinf.bCode && a.CustomerCode == uinf.cCode)
                            select new PurSetup
                            {
                                SetupCode = a.SetupCode,
                                SetupDescription = a.SetupDescription,
                                SetupValue = a.SetupValue,
                                BranchId = "PUR"
                            }).ToList();
                var crmsets = (from a in db.CrmSetup.Where(a => a.BranchId == uinf.bCode && a.CustomerCode == uinf.cCode)
                               select new PurSetup
                               {
                                   SetupCode = a.SetupCode,
                                   SetupDescription = a.PosDescription,
                                   SetupValue = a.Pos,
                                   BranchId = "CRM"
                               }).ToList();

                var sets = pursets.Union(crmsets).ToList();
                uc.sets = sets;
                return uc;
            }
        }
       
        private List<UserRoles> findRoles(String rCode, int cCode)
        {
            UsineContext db = new UsineContext();
            List<UserRoles> uroles = new List<UserRoles>();
            var roles = db.Admroles.Where(a => a.RoleName == rCode && a.CustomerCode == cCode && a.Pos == 1);
            foreach (Admroles role in roles)
            {
                uroles.Add(new UserRoles { moduleCode = role.ModuleCode, menuCode = role.MenuCode, screenCode = role.ScreenCode, transCode = role.TransCode });
            }
            return uroles;
        }

        [HttpGet]
        [Route("api/LoginControl/makeBranchAddress/{bCode}/{cCode}")]
        public UserAddress makeBranchAddress( string bCode, int cCode)
        {
        

             
            PrismProductsAdminContext dc = new PrismProductsAdminContext();
            AdminControl ac = new AdminControl();
            var det = dc.CustomerRegistrations.Where(a => a.CustomerId == cCode).FirstOrDefault();
            UserAddress addr = new UserAddress();
            DateTime dat =   ac.getPresentDateTime();
            if (det != null)
            {
                if(det.ExpDate.Value >= dat)
                {
                    addr.company = det.Customer;
                    addr.branchName = det.Customer;
                    addr.address = det.Addr;
                    addr.country= det.Country;
                    addr.stat = det.Stat;
                    addr.district = det.District;
                    addr.city = det.City;
                    addr.zip = det.Zip;
                    addr.mobile = det.Mobile;
                    addr.tel = det.Tel;
                    addr.fax= det.Fax;
                    addr.email = det.Email;
                    addr.web= det.Web;
                }
                else
                {
                    addr.country = "REGPROB";
                }
            }   
            else
            {
                addr = null;
            } 

            return addr; 
        }
        [HttpPost]
        [Route("api/LoginControl/logChangePassword")]
        public TransactionResult logChangePassword([FromBody] ChangePassword det)
        {
            string result = "";
            TransactionResult res = new TransactionResult();
            AdminControl ac = new AdminControl();
            try
            {
                string oldpass=sc.makeStringToAscii(det.oldpassword);
                string newpass = sc.makeStringToAscii(det.newpassword);
                string user = sc.makeStringToAscii(det.usr.uCode.ToLower());
                var userdet = db.UsrAut.Where(a => a.UsrName == user && a.CustomerCode == det.usr.cCode).FirstOrDefault();
                if(userdet != null)
                {
                    if(userdet.Pwd==oldpass)
                    {
                        userdet.Pwd = newpass;
                        db.SaveChanges();
                        result = "OK";
                    }
                    else
                    {
                        result = "Incorrect current password";
                    }
                }
                else
                {
                    result = "No user is existed";
                }
               
            }
            catch(Exception ee)
            {
                result = ee.Message;
            }
            res.result = result;
            return res;
        }
        private List<UserAssignings> findAssignings(string uCode,String rCode, int cCode)
        {
              List<UserAssignings> assgs = new List<UserAssignings>();

            UsineContext db = new UsineContext();
            PrismProductsAdminContext dc = new PrismProductsAdminContext();
             if (rCode.ToUpper() == "ADMINISTRATOR")
             {
            var accs = db.FinAccounts.Where(a => (a.AcType == "CAS" || a.AcType == "BAN" || a.AcType == "MOB") && a.CustomerCode == cCode);
            foreach (FinAccounts acc in accs)
            {
                assgs.Add(new UserAssignings { code = acc.Recordid.ToString(), detail = acc.Accname, typ = acc.AcType });
            }
            var outlets = db.ResOutletMaster.Where(a => a.CustomerCode == cCode);
            foreach (ResOutletMaster outlet in outlets)
            {
                assgs.Add(new UserAssignings { code = outlet.RestaCode, detail = outlet.RestaName, typ = "RES" });
            }
                var branches = dc.CustomerBranches.Where(a => a.CustomerId == cCode).OrderBy(b => b.BranchId).ToList(); //db.Sertaxes4.Where(a => a.SerTax4 == sc.makeStringToAsciiCustomer(cCode.ToString()) && a.SerTax1 == sc.makeStringToAsciiCustomer("bra_nam")).OrderBy(b => b.SerTax3);
            foreach (var branch in branches)
            {
                assgs.Add(new UserAssignings { code = branch.BranchId, detail = branch.BranchName, typ = "BRA" });
            }

              }
             else
            {
                var furs = db.AdmUserwiseAssigns.Where(a => a.UserName.ToUpper() == uCode && a.CustomerCode == cCode).ToList();
                foreach(var fur in furs)
                {
                    assgs.Add(new UserAssignings { code = fur.AssignedId, typ = fur.AssignedTyp });
                }
            }
            return assgs; 
        }
        private String findProductCode(int cCode)
        {
            PrismProductsAdminContext dc = new PrismProductsAdminContext();
                  var det = dc.CustomerRegistrations.Where(a => a.CustomerId == cCode).FirstOrDefault();
            if(det != null)
            {
                return det.ProductId;
            }
            else
            {
                return " ";
            }
        }
        [HttpGet]
        [Authorize]
        [Route("api/LoginControl/GetCustomerRegistration")]
        public CustomerRegistrations GetCustomerRegistration(int cCode)
        {
            PrismProductsAdminContext dc = new PrismProductsAdminContext();
            return dc.CustomerRegistrations.Where(a => a.CustomerId == cCode).FirstOrDefault();
        }

            private string GenerateAccessToken(string email, string userId, string[] roles)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("secretsecretsecret"));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, email)
            };

            claims = claims.Concat(roles.Select(role => new Claim(ClaimTypes.Role, role))).ToList();


            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                "issuer",
                "audience",
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public Task<string> Authenticate(string email, string password)
        {
            throw new NotImplementedException();
        }

    }
}
