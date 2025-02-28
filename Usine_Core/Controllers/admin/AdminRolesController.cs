using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
 using Usine_Core.Models;
using Usine_Core.others;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Controllers.Admin;
using Usine_Core.Controllers;

namespace Usine_Core.Controllers.Admin
{

    public class AdmRolesTotal
    {
        public List<Admroles> roles { get; set; }
        public String rolesName { get; set; }
        public int traCheck { get; set; }
        public UserInfo usr { get; set; }
        public String result { get; set; }
    }
    public class AdmUsersTotal
    {
        public UsrAut user { get; set; }
        public UserCompleteProfile profile { get; set; }
        public UserInfo usr { get; set; }
        public String result { get; set; }
    }
    public class SystemVerify
    {
        public string result { get; set; }
    }
    public class AdminRolesController : Controller
    {
        UsineContext db = new UsineContext();
        [HttpPost]
        [Authorize]
        [Route("api/AdminRoles/GetAdminRoles")]
        public List<string> GetRoles([FromBody] UserInfo usr)
        {
            return db.Admroles.Where(a => a.CustomerCode == usr.cCode).Select(b => b.RoleName).Distinct().ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/AdminRoles/GetAdminRole")]
        public List<Admroles> GetAdminRole([FromBody] GeneralInformation inf)
        {
            return db.Admroles.Where(a => a.CustomerCode == inf.usr.cCode && a.RoleName == inf.detail && a.Pos > 0).OrderBy(b => b.ModuleCode).ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/AdminRoles/setAdminRole")]
        public AdmRolesTotal setAdminRole([FromBody] AdmRolesTotal tot)
        {
            String msg = "";
            AdminControl ac = new AdminControl();
            try
            {
                if (ac.screenCheck(tot.usr, -1, -1, -1, -1))
                {
                    var role = db.Admroles.Where(a => a.RoleName == tot.rolesName && a.CustomerCode == tot.usr.cCode);
                    if(role.Count() > 0)
                    db.Admroles.RemoveRange(role);
                    List<Admroles> roles = new List<Admroles>();
                    for (int i = 0; i < tot.roles.Count; i++)
                    {
                        roles.Add
                            (new Admroles
                            {
                                RoleName = tot.rolesName,
                                ModuleCode = tot.roles[i].ModuleCode,
                                MenuCode = tot.roles[i].MenuCode,
                                ScreenCode = tot.roles[i].ScreenCode,
                                TransCode = tot.roles[i].TransCode,
                                Pos = 1,
                                CustomerCode = tot.usr.cCode
                            });
                    }

                    if (roles.Count > 0)
                    {
                        db.Admroles.AddRange(roles);
                    }
                    db.SaveChanges();
                    msg = "OK";
                }
                else
                {
                    msg = "You are not authorised to transact on roles";
                }
            }
            catch (Exception ee)
            {
                msg = ee.Message;
            }

            tot.result = msg;
            return tot;
        }

        

        [HttpPost]
        [Authorize]
        [Route("api/AdminRoles/addUser")]
        public AdmUsersTotal addUser([FromBody] AdmUsersTotal tot)
        {
            String msg = "";
            AdminControl ac = new AdminControl();
            if (ac.screenCheck(tot.usr, -1, -1, -1, -1))
            {
                StringConversions sc = new StringConversions();
                try
                {
                    var ur = sc.makeStringToAscii(tot.user.UsrName.ToLower().Trim());

                    var det = db.UsrAut.Where(a => a.UsrName == ur && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (det != null)
                    {
                        det.RoleName= sc.makeStringToAscii(tot.user.RoleName);
                        det.WebFreeEnable = tot.user.WebFreeEnable;
                        det.MobileFreeEnable = tot.user.MobileFreeEnable;

                        var profile = db.UserCompleteProfile.Where(a => a.UsrName == ur).FirstOrDefault();
                        if(profile==null)
                        {
                            tot.profile.UsrName = sc.makeStringToAscii(tot.user.UsrName.ToLower().Trim());
                            tot.profile.BranchId = tot.usr.bCode;
                            tot.profile.CustomerCode = tot.usr.cCode;
                            db.UserCompleteProfile.Add(tot.profile);
                        }
                        else
                        {
                            profile.EmployeeNo = tot.profile.EmployeeNo;
                        }
                    }
                    else
                    {
                        UsrAut usr = new UsrAut();
                        usr.UsrName = sc.makeStringToAscii(tot.user.UsrName.ToLower().Trim());
                        usr.RoleName = sc.makeStringToAscii(tot.user.RoleName);
                        usr.Email = " ";
                        usr.Pwd = sc.makeStringToAscii(tot.user.UsrName.ToLower().Trim());
                        usr.WebFreeEnable = tot.user.WebFreeEnable;
                        usr.MobileFreeEnable = tot.user.MobileFreeEnable;
                        usr.Pos = 1;
                        usr.CustomerCode = tot.usr.cCode;
                        db.UsrAut.Add(usr);

                        //UserCompleteProfile profile = new UserCompleteProfile();
                        tot.profile.UsrName = sc.makeStringToAscii(tot.user.UsrName.ToLower().Trim());
                          tot.profile.BranchId = tot.usr.bCode;
                        tot.profile.CustomerCode = tot.usr.cCode;
                        db.UserCompleteProfile.Add(tot.profile);

                    
                    }
                    db.SaveChanges();
                    msg = "OK";
                }
                catch (Exception ee)
                {
                    msg = ee.Message;
                }
            }
            else
            {
                msg = "You are not authorised";
            }


            tot.result = msg;
            return tot;
        }

        [HttpPost]
        [Authorize]
        [Route("api/AdminRoles/getUsers")]
        public List<UsrAut> getUsers([FromBody] UserInfo usr)
        {
            
            AdminControl ac = new AdminControl();
            if (ac.screenCheck(usr, -1, -1, -1, -1))
            {
                StringConversions sc = new StringConversions();
                var ur = sc.makeStringToAscii(usr.uCode.ToLower().Trim());
                var sy = sc.makeStringToAscii("system");
                DataBaseContext gg = new DataBaseContext();
                General g = new General();
                string quer = "";
                quer = quer + " select * from usraut where customercode=" + usr.cCode.ToString() + " and usrname not in ('";
                quer = quer + ur + "', '" + sy + "')";
                SqlCommand dc = new SqlCommand();
                dc.Connection = gg.db;
                gg.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();
                List<UsrAut> lst = new List<UsrAut>();
                while (dr.Read())
                {
                    lst.Add(new UsrAut
                    {
                        UsrName = sc.makeAsciitoString(dr[0].ToString()),
                        RoleName = sc.makeAsciitoString(dr[1].ToString()),
                        Pos = g.valInt(dr[4].ToString()),
                        WebFreeEnable = g.valInt(dr[6].ToString()),
                        MobileFreeEnable = g.valInt(dr[7].ToString())

                    });
                }
                return lst;
            }
            else
            {
                return null;
            }
        }


        [HttpPost]
        [Authorize]
        [Route("api/AdminRoles/getAllUsers")]
        public List<UsrAut> getAllUsers([FromBody] UserInfo usr)
        {
            AdminControl ac = new AdminControl();
            StringConversions sc = new StringConversions();
                  return (from a in db.UsrAut.Where(a => a.CustomerCode == usr.cCode)
                        select new UsrAut
                        {
                            UsrName = sc.makeAsciitoString(a.UsrName),
                            RoleName = sc.makeAsciitoString(a.RoleName),
                            Pwd = sc.makeAsciitoString(a.Pwd)
                        }).OrderBy(b => b.UsrName).ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/AdminRoles/admPasswordReset")]
        public TransactionResult admPasswordReset([FromBody] GeneralInformation inf)
        {
            String msg = "";
            try
            {
                StringConversions sc = new StringConversions();
                var ur = sc.makeStringToAscii(inf.detail.ToLower().Trim());
                var det = db.UsrAut.Where(a => a.UsrName == ur && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                if (det != null)
                {
                    det.Pwd = det.UsrName;
                    db.SaveChanges();
                    msg = "OK";
                }
                else
                {
                    msg = "No record found";
                }
            }
            catch (Exception ee)
            {
                msg = ee.Message;
            }
            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;
        }

        [HttpPost]
        [Authorize]
        [Route("api/AdminRoles/admUserBlock")]
        public TransactionResult admUserBlock([FromBody] GeneralInformation inf)
        {
            String msg = "";
            try
            {
                StringConversions sc = new StringConversions();
                var ur = sc.makeStringToAscii(inf.detail.ToLower().Trim());
                var det = db.UsrAut.Where(a => a.UsrName == ur && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                if (det != null)
                {
                    det.Pos = 0-det.Pos;
                    db.SaveChanges();
                    msg = "OK";
                }
                else
                {
                    msg = "No record found";
                }
            }
            catch (Exception ee)
            {
                msg = ee.Message;
            }
            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;
        }


        [HttpPost]
        [Authorize]
        [Route("api/AdminRoles/admChangeRoleName")]
        public String admChangeRoleName([FromBody] AdmUsersTotal tot)
        {
            String msg = "";
            try
            {
                StringConversions sc = new StringConversions();
                var ur = sc.makeStringToAscii(tot.user.UsrName.ToLower().Trim());
                var det = db.UsrAut.Where(a => a.UsrName == ur && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                if (det != null)
                {
                    det.RoleName = sc.makeStringToAscii(tot.user.RoleName);
                    db.SaveChanges();
                    msg = "OK";
                }
                else
                {
                    msg = "No record found";
                }
            }
            catch (Exception ee)
            {
                msg = ee.Message;
            }
            return msg;
        }

    }
}
