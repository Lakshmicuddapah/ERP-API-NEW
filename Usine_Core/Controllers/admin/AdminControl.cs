using Usine_Core.Models;
using Usine_Core.others;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.SqlClient;
using Usine_Core.ModelsAdmin;

namespace Usine_Core.Controllers.Admin
{
    public class ActivityList
    {
        public string title { get; set; }
        public string tim { get; set; }
        public string description { get; set; }
        public int? typ { get; set; }
        public string typdescr { get; set; }
    }
    public class ActvityInformation
    {
        public string module { get; set; }
        public List<ActivityList> activities { get; set; }
    }
    public class AdminControl
    {
        public UsineContext db = new UsineContext();
        public static string versionNumber = "4.5.001";
        public String transactionCheck(string module, DateTime? dat, UserInfo usr)
        {
            PrismProductsAdminContext dc = new PrismProductsAdminContext();
            var dats= dc.CustomerRegistrations.Where(a => a.CustomerId == usr.cCode).Select(b => b.ExpDate).FirstOrDefault();
            if(getPresentDateTime() > dats)
            {
                return "Your License is expired please contact vendor";
            }
            else
            {
                return "OK";
            }
         
        }


        public Boolean screenCheck(UserInfo usr, int modCode, int mnuCode, int scrCode, int traCode)
        {
            Boolean b;

            if (sessionCheck(usr))
            {
                if (usr.rCode.ToUpper() == "ADMINISTRATOR")
                {
                    b = true;
                }
                else
                {
                    var x = db.Admroles.Where(a => a.RoleName == usr.rCode && a.CustomerCode == usr.cCode && a.ModuleCode == modCode && a.MenuCode == mnuCode && a.ScreenCode == scrCode && a.TransCode == traCode && a.Pos == 1).FirstOrDefault();
                    if (x == null)
                    {
                        b = false;
                    }
                    else
                    {
                        b = true;
                    }
                }
            }
            else
            {
                b = false;
            }
            return b;
        }
        public Boolean sessionCheck(UserInfo usr)
        {
            var x = db.MakeSessions.Where(a => a.UserName == usr.uCode && a.CustomerCode == usr.cCode && a.KCode == usr.kCode && a.Pos == 1 && a.SessionClose >= DateTime.Now);
            if (x == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public DateTime getFinancialStart(DateTime dat, UserInfo usr)
        {
            PrismProductsAdminContext dc = new PrismProductsAdminContext();
            var x = dc.CustomerRegistrations.Where(a => a.CustomerId == usr.cCode).Select(b => b.Fiscal).FirstOrDefault();
            String str = "1-Apr-";
            if (x != null)
            {
                str = x.Substring(0,6);
            }
            str = str + dat.Year.ToString();
            DateTime finstart = DateTime.Parse(str);
            if (dat < finstart)
            {
                finstart = finstart.AddYears(-1);
            }
            return finstart;
        }
        public DateTime getPresentDateTime()
        {
            // DateTime dat =  DateTime.Now.AddHours(5).AddMinutes(30);
            DateTime dat = DateTime.Now;
            return dat;

        }
         
        public DateTime getPresentDateTimeFromDb(UserInfo usr)
        {
            // return DateTime.Now.AddHours(5).AddMinutes(30);
            DateTime dat =DateTime.Now;
            try
            {
                DataBaseContext g = new DataBaseContext();
                string quer = "";
                quer = quer + " select dateAdd(mi,convert(float,hrs)*60,systemtime) currentdate from ";
                quer = quer + " (select sysdatetime()  systemtime)a, ";
                quer = quer + " (select case when cnt = 0 then '0' else (select pos from admSetup where customerCode = " + usr.cCode.ToString() + " and setupCode = 'adm_tim') end hrs from";
                quer = quer + " (select count(*) cnt from admSetup where customerCode = " + usr.cCode.ToString() + " and setupCode = 'adm_tim')x)b";
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();
                if (dr.Read())
                {
                    dat = DateTime.Parse(dr[0].ToString());
                }
                dr.Close();
                g.db.Close();
            }
            catch
            {

            }
            return dat;

        }
        public DateTime DateAdjustFromFrontEnd(DateTime dat)
        {
            return dat.AddHours(5.5);
        }
        public String strDate(DateTime dat)
        {
            String monthname = new DateTime(dat.Year, dat.Month, dat.Day).ToString("MMM", CultureInfo.InvariantCulture);
            String str = (dat.Day < 10 ? "0" : "") + dat.Day.ToString() + "-" + monthname.Substring(0, 3) + "-" + dat.Year.ToString().Substring(2, 2);
            return str;
        }
        public String strTime(DateTime tim)
        {
            String str = (tim.Hour < 10 ? "0" : "") + tim.Hour.ToString();
            str = str + ":";
            str = str + (tim.Minute < 10 ? "0" : "") + tim.Minute.ToString() + ":" + (tim.Second < 10 ? "0" : "") + tim.Second.ToString();
            return str;
        }
        
        public String removeEnter(String str)
        {
            if (str == null)
            {
                return "";
            }
            else
            {
                String ss = str.Replace((char)(13), ',');
                ss = ss.Replace((char)(10), ',');
                return ss;
            }
        }
        public void makeHeader(Document document, String hea, int totPages, int pageNo, UserAddress addr)
        {

            PdfPTable ptot = new PdfPTable(2);
            float[] widths = new float[] { 450f, 100f };

            ptot.SetWidths(widths);
            //ptot.HorizontalAlignment = PdfTable.ALIGN_LEFT;
            ptot.TotalWidth = 550f;
            ptot.LockedWidth = true;

            iTextSharp.text.Font fn = new iTextSharp.text.Font(1, 9, iTextSharp.text.Font.BOLD);
            PdfPCell plC = new PdfPCell(new Phrase(addr.branchName, fn));

            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            plC = new PdfPCell(new Phrase(" ", fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);

            fn = new iTextSharp.text.Font(1, 8, iTextSharp.text.Font.BOLD);
            plC = new PdfPCell(new Phrase(removeEnter(addr.address) + ", " + addr.city, fn));

            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            plC = new PdfPCell(new Phrase("Page " + pageNo.ToString() + " of " + totPages.ToString(), fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            /*
                        fn = new iTextSharp.text.Font(1, 8, iTextSharp.text.Font.BOLD);
                        plC = new PdfPCell(new Phrase(addr.city, fn));
                        plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                        plC.BorderWidth = 0f;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        ptot.AddCell(plC);
                        fn = new iTextSharp.text.Font(1, 7, iTextSharp.text.Font.NORMAL);
                        plC = new PdfPCell(new Phrase("Page " + pageNo.ToString() + " of " + totPages.ToString(), fn));
                        plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                        plC.BorderWidth = 0f;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        ptot.AddCell(plC);*/

            fn = new iTextSharp.text.Font(1, 12, iTextSharp.text.Font.BOLD);
            plC = new PdfPCell(new Phrase(" ", fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            plC = new PdfPCell(new Phrase(" ", fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);

            fn = new iTextSharp.text.Font(1, 8, iTextSharp.text.Font.ITALIC);
            plC = new PdfPCell(new Phrase(hea, fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            plC = new PdfPCell(new Phrase(" ", fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);


            /* plC = new PdfPCell(new Phrase(" ", fn));
             plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
             plC.BorderWidth = 0f;
             plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
             ptot.AddCell(plC);
             plC = new PdfPCell(new Phrase(" ", fn));
             plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
             plC.BorderWidth = 0f;
             plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
             ptot.AddCell(plC);*/



            document.Add(ptot);
        }

        public void makeFooter(Document document, UserInfo usr)
        {

            PdfPTable ptot = new PdfPTable(2);
            float[] widths = new float[] { 275f, 275f };
            AdminControl ac = new AdminControl();

            ptot.SetWidths(widths);
            //ptot.HorizontalAlignment = PdfTable.ALIGN_LEFT;
            ptot.TotalWidth = 550f;
            ptot.LockedWidth = true;
            iTextSharp.text.Font fn = new iTextSharp.text.Font(1, 8, iTextSharp.text.Font.NORMAL);

            PdfPCell plC = new PdfPCell(new Phrase("  ", fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;

            ptot.AddCell(plC);

            plC = new PdfPCell(new Phrase("  ", fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;

            ptot.AddCell(plC);

            plC = new PdfPCell(new Phrase("Printing Date Time : " + strDate(ac.getPresentDateTime()) + " " + strTime(getPresentDateTime()), fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            plC.BorderWidthTop = 1f;
            ptot.AddCell(plC);

            plC = new PdfPCell(new Phrase("Powered by Prism Cloud Solutions   User : " + usr.uCode, fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            plC.BorderWidthTop = 1f;
            ptot.AddCell(plC);






            document.Add(ptot);
        }




        public void makeLandscapeHeader(Document document, String hea, int totPages, int pageNo, UserAddress addr)
        {

            PdfPTable ptot = new PdfPTable(2);
            float[] widths = new float[] { 620f, 100f };

            ptot.SetWidths(widths);
            //ptot.HorizontalAlignment = PdfTable.ALIGN_LEFT;
            ptot.TotalWidth = 720f;
            ptot.LockedWidth = true;

            iTextSharp.text.Font fn = new iTextSharp.text.Font(1, 9, iTextSharp.text.Font.BOLD);
            PdfPCell plC = new PdfPCell(new Phrase(addr.branchName, fn));

            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            plC = new PdfPCell(new Phrase(" ", fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);

            fn = new iTextSharp.text.Font(1, 8, iTextSharp.text.Font.BOLD);
            plC = new PdfPCell(new Phrase(removeEnter(addr.address) + ", " + addr.city, fn));

            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            plC = new PdfPCell(new Phrase("Page " + pageNo.ToString() + " of " + totPages.ToString(), fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            /*
                        fn = new iTextSharp.text.Font(1, 8, iTextSharp.text.Font.BOLD);
                        plC = new PdfPCell(new Phrase(addr.city, fn));
                        plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                        plC.BorderWidth = 0f;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        ptot.AddCell(plC);
                        fn = new iTextSharp.text.Font(1, 7, iTextSharp.text.Font.NORMAL);
                        plC = new PdfPCell(new Phrase("Page " + pageNo.ToString() + " of " + totPages.ToString(), fn));
                        plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                        plC.BorderWidth = 0f;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        ptot.AddCell(plC);*/

            fn = new iTextSharp.text.Font(1, 12, iTextSharp.text.Font.BOLD);
            plC = new PdfPCell(new Phrase(" ", fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            plC = new PdfPCell(new Phrase(" ", fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);

            fn = new iTextSharp.text.Font(1, 8, iTextSharp.text.Font.ITALIC);
            plC = new PdfPCell(new Phrase(hea, fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            plC = new PdfPCell(new Phrase(" ", fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);


            /* plC = new PdfPCell(new Phrase(" ", fn));
             plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
             plC.BorderWidth = 0f;
             plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
             ptot.AddCell(plC);
             plC = new PdfPCell(new Phrase(" ", fn));
             plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
             plC.BorderWidth = 0f;
             plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
             ptot.AddCell(plC);*/



            document.Add(ptot);
        }

        public void makeLandscapeFooter(Document document, UserInfo usr)
        {

            PdfPTable ptot = new PdfPTable(2);
            float[] widths = new float[] { 360f, 360f };
            AdminControl ac = new AdminControl();
            ptot.SetWidths(widths);
            //ptot.HorizontalAlignment = PdfTable.ALIGN_LEFT;
            ptot.TotalWidth = 720f;
            ptot.LockedWidth = true;
            iTextSharp.text.Font fn = new iTextSharp.text.Font(1, 8, iTextSharp.text.Font.NORMAL);

            PdfPCell plC = new PdfPCell(new Phrase("  ", fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;

            ptot.AddCell(plC);

            plC = new PdfPCell(new Phrase("  ", fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;

            ptot.AddCell(plC);

            plC = new PdfPCell(new Phrase("Printing Date Time : " + strDate(ac.getPresentDateTime()) + " " + strTime(getPresentDateTime()), fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            plC.BorderWidthTop = 1f;
            ptot.AddCell(plC);

            plC = new PdfPCell(new Phrase("Powered by Prism Cloud Solutions   User : " + usr.uCode, fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.BorderWidth = 0f;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            plC.BorderWidthTop = 1f;
            ptot.AddCell(plC);






            document.Add(ptot);
        }
    }
}