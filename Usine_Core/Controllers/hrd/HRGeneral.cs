using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Controllers.HRD;
using System.Data;
using Usine_Core.Models;

namespace Usine_Core.Controllers.hrd
{
    public class HRGeneral
    {
        UsineContext db = new UsineContext();
        public List<Employees> getEmployeesByDepartment(GeneralInformation inf)
        {
            return (from a in db.HrdEmployees.Where(a => a.Department == inf.recordId && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                       join b in db.HrdDepartments.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.Department equals b.RecordId
                       select new Employees
                       {
                           empid=(int)a.RecordId,
                           empname=a.Empname,
                           department=b.SGrp
                       }).OrderBy(b => b.empname).ToList();
        }
    }
}
