using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Controllers.CRM;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Usine_Core.Controllers.Maintenance
{
    public class MaiEquipGroupDetails
    {
        public int? recordID { get; set; }
        public string subGroup { get; set; }
        public string mainGroup { get; set; }
        public string GroupCode { get; set; }
        public int? sno { get; set; }
        public int? chk { get; set; }
        public string statu { get; set; }
    }
    public class MaiEquipGroupTree
    {
        public int? recordId { get; set; }
        public string mGrp { get; set; }
        public string sGrp { get; set; }
        public Boolean openCheck { get; set; }
        public List<MaiEquipGroupTree> child { get; set; }
    }
    public class MaiEquipGroupTotal
    {
        public MaiEquipGroups grp { get; set; }
        public int? traCheck { get; set; }
        public UserInfo usr { get; set; }
    }
    public class MaiEquipmentGroupsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/MaiEquipmentGroups/GetMaiEquiGroups")]
        public List<MaiEquipGroupDetails> GetMaiEquiGroups([FromBody] UserInfo usr)
        {
             try
            {
                if (ac.screenCheck(usr, 9, 1, 1, 0))
                {

                    return (from a in db.MaiEquipGroups.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode && a.Chk == 1)
                            join b in db.MaiEquipGroups.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                            on a.MGrp equals b.RecordId
                            select new MaiEquipGroupDetails
                            {
                                recordID = a.RecordId,
                                subGroup = a.SGrp,
                                mainGroup = b.SGrp,
                                GroupCode = a.GroupCode,
                                sno = a.Sno,
                                chk = a.Chk,
                                statu = a.Statu == 1 ? "Active" : "Inactive"
                            }).OrderBy(b => b.mainGroup).ThenBy(b => b.subGroup).ToList();


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

        [HttpPost]
        [Authorize]
        [Route("api/MaiEquipmentGroups/GetEquipGroupTrees")]

        public List<MaiEquipGroupTree> GetEquipGroupTrees([FromBody] UserInfo usr)
        {
            List<MaiEquipGroupTree> mainGrps = new List<MaiEquipGroupTree>();

            var lists = db.MaiEquipGroups.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode && a.MGrp == (db.MaiEquipGroups.Where(b => b.Branchid == usr.bCode && b.CustomerCode == usr.cCode && b.SGrp == "MAIN").Select(c => c.RecordId).FirstOrDefault())).OrderBy(d => d.RecordId);

            foreach (var list in lists)
            {
                mainGrps.Add(new MaiEquipGroupTree { recordId = list.RecordId, mGrp = "MAIN", sGrp = list.SGrp, openCheck = false, child = findChildren(usr, list) });
            }
            return mainGrps;
        }

        private List<MaiEquipGroupTree> findChildren(UserInfo usr, MaiEquipGroups grp)
        {
            UsineContext db = new UsineContext();
            List<MaiEquipGroupTree> subGrps = new List<MaiEquipGroupTree>();
            var lists = db.MaiEquipGroups.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode && a.MGrp == grp.RecordId).OrderBy(c => c.SGrp);
            foreach (var list in lists)
            {
                subGrps.Add(new MaiEquipGroupTree { recordId = list.RecordId, mGrp = grp.SGrp, sGrp = list.SGrp, openCheck = false });
            }
            return subGrps;
        }


        private int? findGroupId()
        {
            UsineContext db = new UsineContext();
            int? x = db.MaiEquipGroups.Max(a => a.RecordId);
            if (x == null)
            {
                x = 100;
            }
            x++;
            return x;
        }

        private Boolean maiEquipGrouspDuplicateCheck(UsineContext db, GeneralInformation inf)
        {
            Boolean b = false;
            switch (inf.traCheck)
            {
                case 1:
                    var x = db.MaiEquipGroups.Where(a => a.SGrp == inf.detail && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode);
                    if (x == null || x.Count() == 0)
                    {
                        b = true;
                    }
                    else
                    {
                        b = false;
                    }
                    break;
                case 2:
                    var y = db.MaiEquipGroups.Where(a => a.SGrp == inf.detail && a.RecordId != inf.recordId && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode);
                    if (y == null || y.Count() == 0)
                    {
                        b = true;
                    }
                    else
                    {
                        b = false;
                    }
                    break;
                case 3:
                    b = true;
                    break;
            }
            return b;
        }



        [HttpPost]
        [Authorize]
        [Route("api/MaiEquipmentGroups/SetEquipmentGroup")]
        public TransactionResult SetEquipmentGroup([FromBody] MaiEquipGroupTotal tot)
        {
            String msg = "";
            int modulecode = 9, menucode = 1, screencode = 1;
            AdminControl am = new AdminControl();
            GeneralInformation inf = new GeneralInformation();
            inf.recordId = tot.grp.RecordId;
            inf.detail = tot.grp.SGrp;
            inf.usr = tot.usr;
            inf.traCheck = tot.traCheck;
            try
            {
                switch (tot.traCheck)
                {
                    case 1:
                        if (am.screenCheck(tot.usr, modulecode, menucode, screencode, 1))
                        {
                            if (maiEquipGrouspDuplicateCheck(db, inf))
                            {
                                tot.grp.Sno = tot.grp.RecordId;
                                tot.grp.Chk = 1;
                                UsineContext tem = new UsineContext();
                                tot.grp.Branchid = tot.usr.bCode;
                                tot.grp.CustomerCode = tot.usr.cCode;
                                db.MaiEquipGroups.Add(tot.grp);
                                db.SaveChanges();
                                msg = "OK";
                            }
                            else
                            {
                                msg = "This group is already existed";
                            }
                        }
                        else
                        {
                            msg = "You are not authorised to create Item Groups";
                        }
                        break;
                    case 2:
                        if (am.screenCheck(tot.usr, modulecode, menucode, screencode, 2))
                        {
                            if (maiEquipGrouspDuplicateCheck(db, inf))
                            {
                                var grp = db.MaiEquipGroups.Where(a => a.RecordId == tot.grp.RecordId && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (grp != null)
                                {
                                    grp.SGrp = tot.grp.SGrp;
                                    grp.GrpTag = tot.grp.GrpTag;
                                    grp.MGrp = tot.grp.MGrp;
                                    grp.Statu = tot.grp.Statu;
                                }
                                db.SaveChanges();
                                msg = "OK";
                            }
                            else
                            {
                                msg = "This group is already existed";
                            }
                        }
                        else
                        {
                            msg = "You are not authorised to Modify Item Groups";
                        }

                        break;
                    case 3:
                        if (am.screenCheck(tot.usr, modulecode, menucode, screencode, 3))
                        {
                            if (maiEquipGrouspDuplicateCheck(db, inf))
                            {
                                var grp = db.MaiEquipGroups.Where(a => a.RecordId == tot.grp.RecordId && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (grp != null)
                                {
                                    db.MaiEquipGroups.Remove(grp);
                                }
                                db.SaveChanges();
                                msg = "OK";
                            }
                            else
                            {
                                msg = "This group is already in use";
                            }
                        }
                        else
                        {
                            msg = "You are not authorised to Delete Item Groups";
                        }

                        break;
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


    }
}
