using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Complete_Solutions_Core.Controllers.Admin;
using Complete_Solutions_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Complete_Solutions_Core.Controllers.CRM
{
    public class CRMQuotationRxTotal
    {
       public int? recordId { get; set; }
        public string pricelist { get; set; }
        public string discountlist { get; set; }
        public string result { get; set; }
        public int? tracheck { get; set; }
    }
    public class CRMCustomerDetails
    {
        public int? enquiryId { get; set; }
        public String customer { get; set; }
    }
    public class CRMQuotationRXRequirements
    {
        public String seq { get; set; }
        public List<CrmEnquiriesRx> Enquiries { get; set; }
        public List<String> PriceLists { get; set; }
        public List<String> DiscountLists { get; set; }

    }

    public class CRMQuotationsRXController : Controller
    {
        CompleteContext db = new CompleteContext();
        AdminControl ac = new AdminControl();


        [HttpPost]
        [Authorize]
        [Route("api/CRMRx/GetCRMRxQuotationsRequirements")]
        public CRMQuotationRXRequirements GetCRMRxQuotationsRequirements([FromBody] UserInfo usr)
        { 
       CRMQuotationRXRequirements tot = new CRMQuotationRXRequirements();
            tot.seq = findSeq(usr);
            tot.Enquiries = db.CrmEnquiriesRx.Where(a => a.PrevcallId == null & a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
            tot.PriceLists = db.SaleRxPriceList.Where(a => a.CustomerCode == usr.cCode).Select(b => b.PriceListName).Distinct().ToList();
            tot.DiscountLists = db.SaleRxDiscountList.Where(a => a.CustomerCode == usr.cCode).Select(b => b.PriceListName).Distinct().ToList();
            return tot;
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMRx/GetCRMRxQuotations")]
        public List<CrmQuotationsRxuni> GetCRMRxQuotations(GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.toDate);
            return db.CrmQuotationsRxuni.Where(a => a.Dat >= dat1 && a.Dat <= dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();
        }

      
        [HttpPost]
        [Authorize]
        [Route("api/CRMRx/SetCRMRxQuotation")]
        public CRMQuotationRxTotal SetCRMRxQuotation([FromBody]CRMQuotationRxTotal tot)
        {
           

            try
            {
                var req = db.CrmEnquiriesRx.Where(a => a.RecordId == tot.recordId).FirstOrDefault();
                req.Statu = tot.tracheck;
                req.PriceList = tot.pricelist;
                req.DiscountList = tot.discountlist;
                db.SaveChanges();
                tot.result = "OK";
            }
            catch(Exception ee)
            {
                tot.result = ee.Message;
            }
            return tot;
            
        }


        private String findSeq(UserInfo usr)
        {
            DateTime dat = ac.getPresentDateTime();
            General g = new General();
            String seq = db.CrmQuotationsRxuni.Where(a => a.Dat.Value.Year == dat.Year && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.Seq);
            int x = 0;
            if (seq != null)
            {
                x = int.Parse(seq.Substring(5, 7));
            }
            x++;
            return "SQ" + dat.Year.ToString().Substring(2, 2) + "-" + g.zeroMake(x, 7);
        }
    }
}
