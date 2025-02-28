using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Usine_Core.Models;
namespace Usine_Core.Controllers.Admin
{
    public partial class UserInfo
    {
        public String uCode { get; set; }
        public String rCode { get; set; }
        public String bCode { get; set; }
        public int cCode { get; set; }
        public String kCode { get; set; }
        public String vCode { get; set; }
        public String pCode { get; set; }
        public int? webCheck { get; set; }
        public int? mobileCheck { get; set; }

    }
    public class LoginControls
    {
        public int customerCode { get; set; }
        public String userName { get; set; }
        public String password { get; set; }
    }
    public class ScreenInformation
    {
        public UserInfo usr { get; set; }
        public int moduleCode { get; set; }
        public int menuCode { get; set; }
        public int screenCode { get; set; }
        public int transCode { get; set; }

    }
    public class GeneralInformation
    {
        public long? recordId { get; set; }
        public string? detail { get; set; }
        public string frmDate { get; set; }
        public string toDate { get; set; }
        public UserInfo usr { get; set; }
        public int? traCheck { get; set; }
    }
    public class UserAddress
    {
        public String company { get; set; }
        public String branchName { get; set; }
        public String address { get; set; }
        public String city { get; set; }
        public String district { get; set; }
        public String stat { get; set; }
        public String country { get; set; }
        public String zip { get; set; }
        public String mobile { get; set; }
        public String tel { get; set; }
        public String fax { get; set; }
        public String email { get; set; }
        public String web { get; set; }
    }
    public class UserRoles
    {
        public int? moduleCode { get; set; }
        public int? menuCode { get; set; }
        public int? screenCode { get; set; }
        public int? transCode { get; set; }
    }
    public class UserCompleteInfo
    {
        public UserInfo usr { get; set; }
        public UserAddress addr { get; set; }
        public List<UserRoles> roles { get; set; }
        public List<UserAssignings> assg { get; set; }
        public List<PurSetup> sets { get; set; }
        public string localCheck { get; set; }
    }

    public class UserAssignings
    {
        public string code { get; set; }
        public string detail { get; set; }
        public string typ { get; set; }

    }
    class AdminGeneral
    { 
        
        public AdminGeneral()
        {

        
        }
    }
}
