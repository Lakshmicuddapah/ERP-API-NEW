using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;

using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;
using Usine_Core.others;
using Usine_Core.Controllers.Accounts;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.Purchases
{

    public class purchaseRequestDetail
    {
        public int? recordId { get; set; }
        public int? sno { get; set; }
        public DateTime? dat { get; set; }
        public int? itemId { get; set; }
        public string itemName { get; set; }
        public string itemDescription { get; set; }
        public string purpose { get; set; }
        public int? suggestSupplier { get; set; }
        public string suggetsupplierName { get; set; }
        public string departmentName { get; set; }
        public double? qty { get; set; }
        public double? approved { get; set; }
        public int? umid { get; set; }
        public string um { get; set; }
        public DateTime? requiredBy { get; set; }

    }
    public class PurchaseRequestDetails
    {
        public int? recordid { get; set; }
        public string seq { get; set; }
        public string usr { get; set; }
        public string departmentname { get; set; }
        public int? empno { get; set; }
        public string employee { get; set; }
        public int? statu { get; set; }
        public int? printCount { get; set; }
        
    }
    public class PurchaseRequestApprovalInformation
    {
        public int? recordid { get; set; }
        public int? sno { get; set; }
        public string seq { get; set; }
        public string dat { get; set; }
        public int? itemid { get; set; }
        public string itemdescription { get; set; }
        public string purpose { get; set; }
        public string empname { get; set; }
        public string supplier { get; set; }
        public double? qty { get; set; }
        public string um { get; set; }
        public string reqdby { get; set; }
        public string usr { get; set; }
    }

    public class PurchaseRequestRequirements
    {
        public string seq { get; set; }
        public string dat { get; set; }
        public dynamic departments { get; set; }
        public dynamic suppliers { get; set; }
        public dynamic employees { get; set; }
        public dynamic materials { get; set; }
        public dynamic materialunits { get; set; }
        public dynamic materialstocks { get; set; }
        public dynamic purchasenotes { get; set; }
        public dynamic addresses { get; set; }
    }
    public class PurchaseRequestTotal
    {
        public PurPurchaseRequestUni header { get; set; }
        public List<purchaseRequestDetail> lines { get; set; }
        public List<PurPurchaseRequestDet> details { get; set; }
        public int? tracheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class PurchaseRequestApprovalInfo
    {
        public List<PurPurchaseRequestDet> lines { get; set; }
        public UserInfo usr { get; set; }
    }
    public class PurchaseTransactionResult
    {
        public long? id { get; set; }
        public string result { get; set; }
    }
    public class AuditHeader
    {
        public long? auditId { get; set; }
        public DateTime? auditdate { get; set; }

        public long? recordId { get; set; }
        public string seq { get; set; }
        public DateTime? dat { get; set; }

        public string usr { get; set; }
        public string department { get; set; }
        public string employee { get; set; }
        public int? auditType { get; set; }
    }

    public class CompleteAudutById
    {
        public AuditHeader header { get; set; }
        public dynamic lines { get; set; }
    }
    public class PurchaseRequestAuditComplete
    {
        public List<PurPurchaseRequestUni> headers { get; set; }
        public List<purchaseRequestDetail> lines { get; set; }

    }
   
    public class PurchaseRequestsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        private readonly IHostingEnvironment ho;
        public PurchaseRequestsController(IHostingEnvironment hostingEnvironment)
        {

            ho = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurchaseRequests/GetPurchaseRequestRequirements")]
        public PurchaseRequestRequirements GetPurchaseRequestRequirements([FromBody] UserInfo usr)
        {
            AdminControl ac = new AdminControl();
            DateTime dat1 = ac.getFinancialStart(ac.getPresentDateTime(), usr);
            DateTime dat2 =DateTime.Parse(ac.strDate( ac.getPresentDateTime())).AddDays(1);
            PurchaseRequestRequirements tot = new PurchaseRequestRequirements();
            tot.seq = findRequestSeq(usr);
            tot.dat =ac.strDate( ac.getPresentDateTime());
            tot.departments = (from a in db.InvDepartments.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                               select new
                               {
                                   deptid = a.RecordId,
                                   deptname = a.Department,
                                   area = a.Area
                               }
                             ).OrderBy(b => b.area).ThenBy(c => c.deptname).ToList();
            tot.employees = (from p in (from a in db.HrdEmployees.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                                        join b in db.HrdDepartments.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                                        on a.Department equals b.RecordId
                                        select new
                                        {
                                            empid = a.RecordId,
                                            empno = a.Empno,
                                            empname = a.Empname,
                                            surname = a.Surname,
                                            fathername = a.Fathername,
                                            gender = a.Gender == 1 ? "Male" : (a.Gender == 2 ? "Female" : "Others"),
                                            mobile = a.Mobile,
                                            deptid = b.RecordId,
                                            department = b.SGrp,
                                            desigid = a.Designation,
                                            branchid = a.Branchid,
                                            customercode = a.CustomerCode
                                        })
                             join q in db.HrdDesignations.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on p.desigid equals q.RecordId
                             select new
                             {
                                 p.empid,
                                 p.empno,
                                 p.empname,
                                 p.surname,
                                 p.fathername,
                                 p.gender,
                                 p.mobile,
                                 p.deptid,
                                 p.department,
                                 p.desigid,
                                 q.Designation,
                                 p.branchid,
                                 p.customercode
                             }).OrderBy(b => b.empname).ToList();
            tot.suppliers = (from a in db.PartyDetails.Where(a => a.Statu==1 && a.PartyType == "SUP" && a.CustomerCode == usr.cCode)
                             join b in db.PurSupplierGroups.Where(a => a.CustomerCode == usr.cCode) on a.PartyGroup equals b.RecordId
                             select new
                             {
                                 recordid = a.RecordId,
                                 supplier = a.PartyName,
                                 partygroup = b.SGrp,

                             }).OrderBy(b => b.supplier).ToList();

            tot.materials = (from a in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode)
                             join b in (from p in db.InvMaterialManagement.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).GroupBy(b => b.ItemName)
                                        select new
                                        {
                                            materail = p.Key,
                                            qty = p.Sum(b => b.Qtyin - b.Qtyout)
                                        }) on a.RecordId equals b.materail into gj
                             from subdet in gj.DefaultIfEmpty()

                             select new
                             {
                                 itemid = a.RecordId,
                                 itemname = a.Itemname,
                                 grpname = a.Grpname,
                                 stock = subdet == null ? 0 : (subdet.qty==null?0:subdet.qty),
                                 um = a.Um,
                             }).OrderBy(b => b.itemname).ToList();


     


            tot.materialunits = (from a in db.InvMaterialUnits.Where(a => a.CustomerCode == usr.cCode)
                                 join b in db.InvUm.Where(a => a.CustomerCode == usr.cCode) on a.Um equals b.RecordId
                                 select new
                                 {
                                     recordid = a.RecordId,
                                     sno = a.Sno,
                                     umid=b.RecordId,
                                     um = b.Um
                                 }).OrderBy(a => a.recordid).ThenBy(b => b.sno).ToList();
    

            return tot;
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurchaseRequests/GetPurchaseRequests")]
        public List<PurchaseRequestDetails> GetPurchaseRequests([FromBody] GeneralInformation inf)
        {
            if (ac.screenCheck(inf.usr,2, 2, 1, 0))
            {
                DataBaseContext g = new DataBaseContext();
                General gg = new General();
                string dat1 = inf.frmDate;
                string dat2 = ac.strDate(DateTime.Parse(inf.toDate).AddDays(1));
                string quer = "";
                quer = quer + " select a.recordId,a.seq,a.usr,a.department,a.empno,a.empname,a.printcount,a.statu + (case when b.purrequest is null then 0 else 2 end) statu from";
                quer = quer + " (select a.recordId, a.seq, a.usr, a.empno, a.empname, a.printcount, b.department, a.statu from";
                quer = quer + " (select a.recordId, a.seq, a.usr, a.department, a.empno, a.statu, a.printcount, b.empname from";
                quer = quer + " (select * from purpurchaserequestuni where dat >= '" + dat1 + "' and dat < '" + dat2 + "'";
                quer = quer + " and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
                quer = quer + " (select * from hrdEmployees where branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b where a.empno = b.recordId)a,";
                quer = quer + " (select * from invDepartments where customerCode = " + inf.usr.cCode + ")b where a.department = b.recordId)a left outer join";
                quer = quer + " (select* from purpurchaseorderdet where purrequest is not null and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b on a.recordID = b.purrequest";
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();
                List<PurchaseRequestDetails> details = new List<PurchaseRequestDetails>();
                while (dr.Read())
                {
                    details.Add(new PurchaseRequestDetails
                    {
                        recordid=gg.valInt(dr[0].ToString()),
                        seq=dr[1].ToString(),
                        usr=dr[2].ToString(),
                        departmentname=dr[3].ToString(),
                        empno=gg.valInt(dr[4].ToString()),
                        employee=dr[5].ToString(),
                        printCount=gg.valInt(dr[6].ToString()),
                        statu=gg.valInt(dr[7].ToString())
                    });
                     
                }
                dr.Close();
                g.db.Close();

                return details;



                /*
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);

              return (from p in (from a in db.PurPurchaseRequestUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                    join b in db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.Empno equals b.RecordId
                                    select new
                                    {
                                        recordid = a.RecordId,
                                        seq = a.Seq,
                                        usr = a.Usr,
                                        department = a.Department,
                                        empno = a.Empno,
                                        employee = b.Empname,
                                        statu=a.Statu,
                                        printCount=a.PrintCount
                                    }
                         )
                         join q in db.InvDepartments.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                       on p.department equals q.RecordId
                         select new
                         {
                             p.recordid,
                             p.seq,
                             p.usr,
                             deptid = p.department,
                             departmentname = q.Department,
                             p.empno,
                             p.employee,
                             statu=p.statu,
                             p.printCount

                         }).OrderBy(b => b.seq).ToList();*/
                       

            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurchaseRequests/GetPurchaseRequest")]
        public PurchaseRequestTotal GetPurchaseRequest([FromBody] GeneralInformation inf)
        {
            PurchaseRequestTotal tot = new PurchaseRequestTotal();
            tot.header =db.PurPurchaseRequestUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            tot.lines = (from a in db.PurPurchaseRequestDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                         join b in db.InvMaterials.Where(a => a.CustomerCode == inf.usr.cCode)
                         on a.ItemId equals b.RecordId
                         select new purchaseRequestDetail
                         {
                             recordId = a.RecordId,
                             sno = a.Sno,
                             dat = a.Dat,
                             itemId = a.ItemId,
                             itemName = b.ItemName,
                             itemDescription = a.ItemDescription,
                             purpose = a.Purpose,
                             suggestSupplier = a.SuggestedSupplier,
                             qty = a.Qty,
                             umid = a.Um,
                             um = db.InvUm.Where(b => b.RecordId == a.Um && b.CustomerCode == inf.usr.cCode).Select(c => c.Um).FirstOrDefault(),
                             requiredBy = a.ReqdBy
                         }).OrderBy(c => c.sno).ToList();

            return tot;
        }


        [HttpPost]
        [Authorize]
        [Route("api/PurchaseRequests/setPurchaseRequest")]
        public TransactionResult setPurchaseRequest([FromBody] PurchaseRequestTotal tot)
        {
            string msg = "";
            UsineContext db1 = new UsineContext();

            try
            {
                if (ac.screenCheck(tot.usr, 2, 2, 1, (int)tot.tracheck))
                {
                    int? statu = 2;
                    General gg = new General();
                    var statucheck = db1.PurSetup.Where(a => a.SetupCode == "pur_req" && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (statucheck != null)
                    {
                        statu = gg.valInt(statucheck.SetupValue) == 1 ? 2 : 1;

                    }

                    switch (tot.tracheck)
                    {
                        case 1:
                            
                                using (var txn = db.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        string seq = findRequestSeq(tot.usr);
                                    tot.header.Seq = seq;
                                    tot.header.Dat = ac.DateAdjustFromFrontEnd(tot.header.Dat.Value);
                                    tot.header.BranchId = tot.usr.bCode;
                                    tot.header.CustomerCode = tot.usr.cCode;
                                    tot.header.Usr = tot.usr.uCode;
                                    tot.header.PrintCount = 0;
                                    tot.header.Statu = statu;
                                    db.PurPurchaseRequestUni.Add(tot.header);
                                    db.SaveChanges();

                                    int sno = 1;
                                 
                                    foreach(PurPurchaseRequestDet det in tot.details)
                                    {
                                        det.RecordId = tot.header.RecordId;
                                        det.Sno = sno;
                                        det.Dat = tot.header.Dat;
                                        det.Usr = tot.usr.uCode;
                                        det.ReqdBy = ac.DateAdjustFromFrontEnd(det.ReqdBy.Value);
                                        det.BranchId = tot.usr.bCode;
                                        det.ApprovedQty = statu == 1 ? 0 : det.Qty;
                                        det.Pos = statu;
                                        det.CustomerCode = tot.usr.cCode;
                                        sno++;
                                    }
                                    db.PurPurchaseRequestDet.AddRange(tot.details);
                                    db.SaveChanges();
                                    txn.Commit();
                                    msg = "OK";
                                  
                                        makeTotalAudit(tot, 1);
                                    }
                                    catch(Exception ee)
                                    {
                                    msg = ee.Message;
                                    txn.Rollback();
                                    }

                                }
                                    
                           
                            break;
                        case 2:
                            try
                            {
                            var requpduni=db.PurPurchaseRequestUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            requpduni.Dat = ac.DateAdjustFromFrontEnd(tot.header.Dat.Value);
                            requpduni.Descriptio = tot.header.Descriptio;
                            requpduni.Usr=tot.usr.uCode;
                            requpduni.Department = tot.header.Department;
                            requpduni.Empno = tot.header.Empno;
                            var requpddet = db.PurPurchaseRequestDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                            db.PurPurchaseRequestDet.RemoveRange(requpddet);
                            int sno1 = 1;
                            foreach (PurPurchaseRequestDet det in tot.details)
                            {
                                det.RecordId = tot.header.RecordId;
                                det.Dat = requpduni.Dat.Value;
                                det.Sno = sno1;
                                det.Usr = tot.usr.uCode;
                                    det.Pos = statu;
                                    det.ApprovedQty = statu == 1 ? 0 : det.Qty;
                                    det.BranchId = tot.usr.bCode;
                                det.CustomerCode = tot.usr.cCode;
                                sno1++;
                            }
                            db.PurPurchaseRequestDet.AddRange(tot.details);
                            db.SaveChanges();
                            msg = "OK";
                           
                                makeTotalAudit(tot, 2);
                            }
                            catch(Exception ee)
                            {
                                msg = ee.Message;
                            }


                            break;
                        case 3:
                            try
                            {
                                var reqdeluni = db.PurPurchaseRequestUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            reqdeluni.BranchId = reqdeluni.BranchId + "DELETE";
                            var reqdeldet = db.PurPurchaseRequestDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                            foreach(PurPurchaseRequestDet lin in reqdeldet)
                            {
                                lin.BranchId=lin.BranchId + "DELETE";
                            }
                            db.SaveChanges();
                            msg = "OK";
                           
                                makeTotalAudit(tot, 3);
                            }
                            catch(Exception ee)
                            {
                                msg = ee.Message;
                            }
                            break;
                    }
                }
                else
                {
                    msg = "You are not authorised for this transaction";
                }
            }
            catch(Exception ex)
            {
                msg = ex.Message;
            }

            TransactionResult res = new TransactionResult();
            res.result = msg;
            res.seq = findRequestSeq(tot.usr);
            return res;
        }
        private string findRequestSeq(UserInfo usr)
        {
            UsineContext db1 = new UsineContext();
            General g = new General();
            DateTime dat = DateTime.Now;
            String seq = "PR" + dat.Year.ToString() + "-";
            int x = 0;
            var detail = db1.PurPurchaseRequestUni.Where(a => a.Seq.Contains(seq) && a.BranchId.Contains(usr.bCode) && a.CustomerCode == usr.cCode).Max(b => b.Seq);
            if(detail != null)
            {
                x = int.Parse(g.right(detail,5));

            }
            x++;
            seq = seq + g.zeroMake(x, 5);
            return seq;
        }


        private void makeTotalAudit(PurchaseRequestTotal tot, int x)
        {
            UsineContext db1 = new UsineContext();
            using (var txn = db1.Database.BeginTransaction())
            {
                try
                {


                    PurPurchaseRequestUniHistory header = new PurPurchaseRequestUniHistory();
                    header.AuditDat = ac.getPresentDateTime();
                    header.RecordId = (int)tot.header.RecordId;
                    header.Seq = findRequestSeq(tot.usr);
                    header.Empno = (int)tot.header.Empno;
                    header.Dat = tot.header.Dat;
                    header.Descriptio = tot.header.Descriptio;
                    header.Usr = tot.usr.uCode;
                    header.Department = tot.header.Department;
                    header.AuditType = x;
                    header.BranchId = tot.usr.bCode;
                    header.CustomerCode = tot.usr.cCode;

                    db.PurPurchaseRequestUniHistory.Add(header);
                    db.SaveChanges();

                    int sno = 1;
                    List<PurPurchaseRequestDetHistory> lines = new List<PurPurchaseRequestDetHistory>();
                    foreach (PurPurchaseRequestDet det in tot.details)
                    {
                        lines.Add(new PurPurchaseRequestDetHistory
                        {
                            AuditId = header.AuditId,
                            Audsno = sno,
                            AuditDat = ac.getPresentDateTime(),
                            RecordId = (int)tot.header.RecordId,
                            Sno =sno,
                            Dat = det.Dat,
                            ItemId = det.ItemId,
                            ItemDescription = det.ItemDescription,
                            Purpose = det.Purpose,
                            SuggestedSupplier = det.SuggestedSupplier,
                            Qty = det.Qty,
                            ApprovedQty = det.ApprovedQty,
                            Um = det.Um,
                            ReqdBy = det.ReqdBy,
                            Usr = tot.usr.uCode,
                            ApprovedUsr = det.ApprovedUsr,
                            Pos = det.Pos,
                            BranchId = tot.usr.bCode,
                            CustomerCode = tot.usr.cCode,
                            AuditType = x
                        });


                        sno++;
                    }
                    db.PurPurchaseRequestDetHistory.AddRange(lines);

                    TransactionsAudit audit = new TransactionsAudit();
                    audit.TraId= (int)tot.header.RecordId;
                    switch(tot.tracheck)
                    {
                        case 1:
                            audit.Descr = " One Purchase request with " + tot.header.Seq + " has been created";
                            break;
                        case 2:
                            audit.Descr = " Purchase request " + tot.header.Seq + " is modified";
                            break;
                        case 3:
                            audit.Descr = " Purchase request " + tot.header.Seq + " is deleted";
                            break;
                    }
                    audit.Usr = tot.usr.uCode;
                    audit.Tratype = tot.tracheck;
                    audit.Transact = "PUR_REQ";
                    audit.TraModule = "PUR";
                    audit.Syscode = "Purchase Request";
                    audit.BranchId = tot.usr.bCode;
                    audit.CustomerCode = tot.usr.cCode;
                    audit.Dat = ac.getPresentDateTime();
                    db.TransactionsAudit.Add(audit);
                    

                    db.SaveChanges();
                    txn.Commit();
                }
                catch(Exception ee)
                {
                    txn.Rollback();
                }
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurchaseRequests/GetPurchaseRequestsForApproval")]
        public List<PurchaseRequestApprovalInformation> GetPurchaseRequestsForApproval([FromBody] GeneralInformation inf)
        {

            string quer = "";
            quer = quer + " select a.recordId,a.sno,a.seq,dbo.strDate(a.dat) dat,dbo.strDate(a.reqdby) reqdby ,a.itemid,a.itemdescription,a.qty,a.purpose,a.supplier,a.empname,b.um from";
            quer = quer + " (select a.recordId, a.sno, a.seq, a.dat, a.reqdby,a.itemid, a.itemdescription, a.qty, a.purpose, a.supplier,case when b.empname is null then ' '  else b.empname end empname,a.empno,a.um from";
            quer = quer + " (select a.recordId, a.sno, a.seq, a.dat,a.reqdby, a.itemid, a.itemdescription, a.qty, a.purpose,case when b.partyname is null then ' '  else b.partyname end supplier,a.empno,a.um from";
            quer = quer + " (select a.recordId, a.sno, b.seq, a.dat, a.reqdby, a.itemid, a.itemDescription, a.qty, a.purpose, a.suggestedsupplier, b.empno, a.um from";
            quer = quer + " (select* from purpurchaserequestdet where (pos is null or pos = 1) and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
            quer = quer + " (select * from purpurchaserequestuni where branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b where a.recordId = b.recordId)a left outer join";
            quer = quer + " (select* from partydetails where customerCode= " + inf.usr.cCode + ")b on a.suggestedSupplier = b.recordId)a left outer join";
            quer = quer + " (select* from hrdEmployees where branchid= '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b on a.empno = b.recordId )a,";
            quer = quer + " (select * from invum where customerCode = " + inf.usr.cCode + ")b where a.um = b.recordId order by recordid";

            DataBaseContext g = new DataBaseContext();
            SqlCommand dc = new SqlCommand();
            General gg = new General();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            List<PurchaseRequestApprovalInformation> lst = new List<PurchaseRequestApprovalInformation>();
            while (dr.Read())
            {
                lst.Add(new PurchaseRequestApprovalInformation
                {
                    recordid = gg.valInt(dr[0].ToString()),
                    sno = gg.valInt(dr[1].ToString()),
                    seq = dr[2].ToString(),
                    dat = dr[3].ToString(),
                    reqdby = dr[4].ToString(),
                    itemid = gg.valInt(dr[5].ToString()),
                    itemdescription = dr[6].ToString(),
                    qty = gg.valNum(dr[7].ToString()),
                    purpose = dr[8].ToString(),
                    supplier = dr[9].ToString(),
                    empname = dr[10].ToString(),
                    um = dr[11].ToString()
                });
            }

          
            dr.Close();
            g.db.Close();

            return lst;

        }

        [HttpPost]
        [Authorize]
        [Route("api/PurchaseRequests/GetPendingRequests")]
        public List<PurchaseRequestApprovalInformation> GetPendingRequests([FromBody] UserInfo usr)
        {

            string quer = "";
            quer = quer + " select itemdescription,sum(qty) qty,um from ";
            quer = quer + " (select a.itemid, a.itemdescription, a.qty * conversionfactor qty, b.um, stdum from";
            quer = quer + " (select a.itemid, a.itemdescription, a.qty, a.um, conversionfactor, stdum from";
            quer = quer + " (select itemid, itemdescription, qty, a.um, b.conversionfactor, b.stdum from";
            quer = quer + " (select * from purpurchaserequestdet where pos >= 2 and branchid = '" +  usr.bCode + "' and customerCode = " + usr.cCode + ")a,";
            quer = quer + " (select * from invMaterialUnits where customerCode = " + usr.cCode + ")b where a.itemId = b.recordId and a.UM = b.um)a left outer join";
            quer = quer + " (select * from purPurchaseOrderDet where branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + " and purRequest is not null)b on a.itemId = b.purRequest)a,";
            quer = quer + " (select * from invum where customerCode = " + usr.cCode + ")b where a.stdum = b.recordId)x";
            quer = quer + " group by itemDescription, um order by itemDescription";

            DataBaseContext g = new DataBaseContext();
            SqlCommand dc = new SqlCommand();
            General gg = new General();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            List<PurchaseRequestApprovalInformation> lst = new List<PurchaseRequestApprovalInformation>();
            while (dr.Read())
            {
                lst.Add(new PurchaseRequestApprovalInformation
                { 
                    itemdescription = dr[0].ToString(),
                    qty = gg.valNum(dr[1].ToString()),
                    um = dr[2].ToString()
                });
            }


            dr.Close();
            g.db.Close();

            return lst;

        }



        [HttpPost]
        [Authorize]
        [Route("api/PurchaseRequests/setPurchaseRequestApproval")]
        public PurchaseTransactionResult setPurchaseRequestApproval([FromBody] PurchaseRequestApprovalInfo tot )
        {
            string msg = "";
            try
            {
                if(ac.screenCheck(tot.usr,2,2,1,98))
                {
                    var headers = tot.lines.Select(a => a.RecordId).Distinct();
                    foreach(var header in headers)
                    {
                        var headerdet = db.PurPurchaseRequestUni.Where(a => a.RecordId == header).FirstOrDefault();
                        if(headerdet != null)
                        {
                            headerdet.Statu = 3;
                        }
                    }
                    foreach(var line in tot.lines)
                    {
                        var linedet = db.PurPurchaseRequestDet.Where(a => a.RecordId == line.RecordId && a.Sno == line.Sno && a.ItemId == line.ItemId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                        if(linedet != null)
                        {
                            linedet.ApprovedQty = line.ApprovedQty;
                            linedet.ApprovedUsr = tot.usr.uCode;
                            linedet.Pos = 3;
                        }
                    }

                    db.SaveChanges();

                    msg = "OK";
                }
                else
                {
                    msg = "You are not authorised for this transaction";
                }
            }
            catch(Exception ee)
            {
                msg = ee.Message;
            }


            PurchaseTransactionResult result = new PurchaseTransactionResult();
            result.result = msg;
            return result;
        }


        [HttpPost]
        [Authorize]
        [Route("api/PurchaseRequests/GetPurchaseRequestAudits")]
        public dynamic GetPurchaseRequestAudits([FromBody] GeneralInformation inf)
        {
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
                return (from p in (from a in db.PurPurchaseRequestUniHistory.Where(a => a.AuditDat >= dat1 && a.AuditDat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                        join b in db.InvDepartments.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode ==inf.usr.cCode) on a.Department equals b.RecordId
                        select new
                        {
                            auditid=a.AuditId,
                            auditdat=a.AuditDat,
                            recordid=a.RecordId,
                            seq=a.Seq,
                            dat=a.Dat,
                            usr=a.Usr,
                            departmentid=a.Department,
                            department=b.Department,
                            empno=a.Empno,
                            auditType=a.AuditType

                        }) join q in (db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)) on p.empno equals q.RecordId
                        select new
                        {
                            p.auditid,
                            p.auditdat,
                            p.recordid,
                            p.seq,
                            p.dat,
                            p.usr,
                            p.departmentid,
                            p.department,
                            q.Empname,
                            p.auditType
                        }).OrderBy(b => b.recordid).ThenBy(c => c.auditid).ToList();
        }


        [HttpPost]
        [Authorize]
        [Route("api/PurchaseRequests/GetPurchaseRequestAuditCompleteDetails")]
        public List<CompleteAudutById> GetPurchaseRequestAuditCompleteDetails([FromBody] GeneralInformation inf)
        {
                var headers= (from p in (from a in db.PurPurchaseRequestUniHistory.Where(a => a.RecordId ==inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                         join b in db.InvDepartments.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.Department equals b.RecordId
                                         select new
                                         {
                                             auditid = a.AuditId,
                                             auditdat = a.AuditDat,
                                             recordid = a.RecordId,
                                             seq = a.Seq,
                                             dat = a.Dat,
                                             usr = a.Usr,
                                             departmentid = a.Department,
                                             department = b.Department,
                                             empno = a.Empno,
                                             auditType = a.AuditType
                                         })
                              join q in (db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)) on p.empno equals q.RecordId
                              select new AuditHeader
                              {
                                auditId=  p.auditid,
                                 auditdate= p.auditdat,
                                recordId=  p.recordid,
                                 seq= p.seq,
                                 dat= p.dat,
                                  usr = p.usr,
                                department=  p.department,
                                employee=  q.Empname,
                                 auditType= p.auditType
                              }).OrderBy(b => b.recordId).ThenBy(c => c.auditId).ToList();
            List<CompleteAudutById> details = new List<CompleteAudutById>();

            foreach(var header in headers)
            {
                
                var lines= (from a in db.PurPurchaseRequestDetHistory.Where(a => a.AuditId == header.auditId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                            join b in db.InvUm.Where(a => a.CustomerCode == inf.usr.cCode)
                            on a.Um equals b.RecordId
                            select new purchaseRequestDetail
                            {
                                recordId = a.RecordId,
                                sno = a.Sno,
                                dat = a.Dat,
                                itemId = a.ItemId,
                                itemDescription = a.ItemDescription,
                                purpose = a.Purpose,
                                suggestSupplier = a.SuggestedSupplier,
                                qty = a.Qty,
                                approved=a.ApprovedQty,
                                 um = b.Um,
                                requiredBy = a.ReqdBy
                            }).OrderBy(c => c.sno).ToList();
                details.Add(new CompleteAudutById
                {
                    header = header,
                    lines = lines
                });
            }


            return details;

                    
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurchaseRequests/printPurchaseRequest")]
        public VoucherResult printPurchaseRequest([FromBody] GeneralInformation inf)
        {
            VoucherResult result = new VoucherResult();
            string msg = "";
             

            var header = (from p in (from a in db.PurPurchaseRequestUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                     join b in db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.Empno equals b.RecordId
                                     select new
                                     {
                                         recordid = a.RecordId,
                                         seq = a.Seq,
                                         dat=a.Dat,
                                         usr = a.Usr,
                                         department = a.Department,
                                         empno = a.Empno,
                                         employee = b.Empname,
                                         statu = a.Statu
                                     }
                           )
                          join q in db.InvDepartments.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                        on p.department equals q.RecordId
                          select new
                          {
                              p.recordid,
                              p.seq,
                              p.dat,
                              p.usr,
                              deptid = p.department,
                              departmentname = q.Department,
                              p.empno,
                              p.employee,
                              statu = p.statu

                          }).FirstOrDefault();

            var lines =(from a in db.PurPurchaseRequestDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                        join b in db.InvUm.Where(a => a.CustomerCode == inf.usr.cCode) on a.Um equals b.RecordId
                        select new
                        {
                            a.Sno,
                            a.ItemDescription,
                            a.Qty,
                            a.ApprovedQty,
                            b.Um,
                            reqdby=ac.strDate(a.ReqdBy.Value),
                            a.Purpose
                        } ).OrderBy(c => c.Sno).ToList();



            String str = ho.WebRootPath + "     " + ho.ContentRootPath;
            String filename =  inf.usr.uCode + "PURREQUEST" + inf.usr.cCode + inf.usr.bCode + ".pdf";
            LoginControlController ll = new LoginControlController();
            UserAddress addr = ll.makeBranchAddress(inf.usr.bCode, inf.usr.cCode);
            using (FileStream ms = new FileStream(ho.WebRootPath + "\\Reps\\" + filename, FileMode.Append, FileAccess.Write))

            {
                Document document = new Document(PageSize.A4, 75f, 40f, 20f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();
                PdfPTable ptot = new PdfPTable(1);
                float[] widths = new float[] { 550f };
                ptot.SetWidths(widths);
                ptot.TotalWidth = 550f;
                ptot.LockedWidth = true;
                iTextSharp.text.Font fn;
                fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 14, Font.BOLD));
                PdfPCell plC = new PdfPCell(new Phrase(addr.branchName, fn));
                plC.BorderWidth = 0f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                ptot.AddCell(plC);

                fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.BOLD));
                plC = new PdfPCell(new Phrase(addr.address + ", " + addr.city, fn));
                plC.BorderWidth = 0f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                ptot.AddCell(plC);
                plC = new PdfPCell(new Phrase("Ph: " + addr.tel, fn));
                plC.BorderWidth = 0f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                ptot.AddCell(plC);

                fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.BOLD));


                plC = new PdfPCell(new Phrase("PURCHASE REQUEST", fn));
                plC.BorderWidth = 1f;
                plC.BackgroundColor = iTextSharp.text.BaseColor.LightGray;
                plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                ptot.AddCell(plC);


                plC = new PdfPCell(new Phrase("\n", fn));
                plC.BorderWidth = 0f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                ptot.AddCell(plC);


                fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.BOLD));
                PdfPTable pdftab = new PdfPTable(2);
                pdftab.SetWidths(new float[] { 250f, 250f });

                PdfPCell pl1 = new PdfPCell(new Phrase("Request # : " + header.seq, fn));
                pl1.BorderWidth = 0f;
                pl1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                pl1.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                pdftab.AddCell(pl1);
                pl1 = new PdfPCell(new Phrase("Date : " + ac.strDate(header.dat.Value), fn));
                pl1.BorderWidth = 0f;
                pl1.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                pl1.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                pdftab.AddCell(pl1);
                pl1 = new PdfPCell(new Phrase("Department : " + header.departmentname, fn));
                pl1.BorderWidth = 0f;
                pl1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                pl1.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                pdftab.AddCell(pl1);
                pl1 = new PdfPCell(new Phrase("Employee : " + header.employee, fn));
                pl1.BorderWidth = 0f;
                pl1.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                pl1.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                pdftab.AddCell(pl1);
                plC = new PdfPCell(pdftab);

                pl1 = new PdfPCell(new Phrase("User : " + header.usr, fn));
                pl1.BorderWidth = 0f;
                pl1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                pl1.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                pdftab.AddCell(pl1);
                pl1 = new PdfPCell(new Phrase("Status : " + (header.statu == 1 ? "Pending" : "Approved"), fn));
                pl1.BorderWidth = 0f;
                pl1.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                pl1.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                pdftab.AddCell(pl1);
                plC = new PdfPCell(pdftab);

                plC.BorderWidth = 0f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                ptot.AddCell(pdftab);



                fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.BOLD,iTextSharp.text.BaseColor.White));
                PdfPTable plines = new PdfPTable(6);
                plines.SetWidths(new float[] { 30f, 200f,80f,80f,60f,100f });


                plC = new PdfPCell(new Phrase("#", fn));
                plC.BorderWidth = 0f;
                plC.BackgroundColor = iTextSharp.text.BaseColor.Black;
                plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                plines.AddCell(plC);

                plC = new PdfPCell(new Phrase("Description", fn));
                plC.BorderWidth = 0f;
                plC.BackgroundColor = iTextSharp.text.BaseColor.Black;
                plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                plines.AddCell(plC);

                plC = new PdfPCell(new Phrase("Qty", fn));
                plC.BorderWidth = 0f;
                plC.BackgroundColor = iTextSharp.text.BaseColor.Black;
                plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                plines.AddCell(plC);
                plC = new PdfPCell(new Phrase("Approved", fn));
                plC.BorderWidth = 0f;
                plC.BackgroundColor = iTextSharp.text.BaseColor.Black;
                plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                plines.AddCell(plC);
                plC = new PdfPCell(new Phrase("UOM", fn));
                plC.BorderWidth = 0f;
                plC.BackgroundColor = iTextSharp.text.BaseColor.Black;
                plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                plines.AddCell(plC);
                plC = new PdfPCell(new Phrase("Reqd By", fn));
                plC.BorderWidth = 0f;
                plC.BackgroundColor = iTextSharp.text.BaseColor.Black;
                plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                plines.AddCell(plC);

                ptot.AddCell(plines);


                fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.NORMAL));
                iTextSharp.text.Font fx = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.ITALIC));

                plines = new PdfPTable(6);
                plines.SetWidths(new float[] { 30f, 200f, 80f, 80f, 60f, 100f });
                int? sno = 1;
                foreach (var lin in lines)
                {
                    plC = new PdfPCell(new Phrase(sno.ToString(), fn));
                    plC.BorderWidth = 0f;
                     plC.BorderWidthRight = 1;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);


                    plC = new PdfPCell(new Phrase(lin.ItemDescription, fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                    plC.BorderWidthRight = 1;
                     plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);


                    plC = new PdfPCell(new Phrase(lin.Qty.ToString(), fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                    plC.BorderWidthRight = 1;
                     plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);


                    plC = new PdfPCell(new Phrase(lin.ApprovedQty.ToString(), fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                    plC.BorderWidthRight = 1;
                      plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);


                    plC = new PdfPCell(new Phrase(lin.Um, fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                    plC.BorderWidthRight = 1;
                     plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);


                    plC = new PdfPCell(new Phrase(lin.reqdby, fn));
                    plC.BorderWidth = 0f;
                    plC.BorderWidthLeft = 1;
                       plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);



                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;

                     plC.BorderWidthRight = 1;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);
                    plC = new PdfPCell(new Phrase(lin.Purpose, fx));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                    plC.BorderWidthRight = 1;
                     plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);
                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                    plC.BorderWidthRight = 1;
                     plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);
                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                    plC.BorderWidthRight = 1;
                     plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);
                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                    plC.BorderWidthRight = 1;
                     plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);
                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;
                    plC.BorderWidthLeft = 1;
                      plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);


                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;

                      plC.BorderWidthRight = 1;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);
                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                    plC.BorderWidthRight = 1;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);
                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                    plC.BorderWidthRight = 1;
                     plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);
                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                    plC.BorderWidthRight = 1;
                     plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);
                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                    plC.BorderWidthRight = 1;
                     plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);
                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                     plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);



                    sno++;
                }

                while(sno<=20)
                {

                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthRight = 1;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);
                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                    plC.BorderWidthRight = 1;
                     plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);
                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                    plC.BorderWidthRight = 1;
                     plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);
                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                    plC.BorderWidthRight = 1;
                     plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);
                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                    plC.BorderWidthRight = 1;
                     plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);
                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;

                    plC.BorderWidthLeft = 1;
                      plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    plines.AddCell(plC);

                    sno++;
                }


           
                ptot.AddCell(plines);

                pdftab = new PdfPTable(2);
                pdftab.SetWidths(new float[] { 250f, 250f });

                for(var i=1;i<=20;i++)
                {
                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    pdftab.AddCell(plC);
                }
                plC = new PdfPCell(new Phrase("", fn));
                plC.BorderWidth = 0f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                pdftab.AddCell(plC);
                plC = new PdfPCell(new Phrase("Purchase Manager", fn));
                plC.BorderWidth = 0f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                pdftab.AddCell(plC);

                ptot.AddCell(pdftab);


                PdfPTable ptotal = new PdfPTable(1);
                widths = new float[] { 550f };
                ptotal.SetWidths(widths);
                ptotal.TotalWidth = 550f;
                ptotal.LockedWidth = true;

                plC = new PdfPCell(ptot);
                plC.BorderWidth = 1f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                ptotal.AddCell(plC);

                document.Add(ptotal);
                document.Close();
            }


            result.result = msg;
            result.filename = filename;
            return result;
        }
    }
}
