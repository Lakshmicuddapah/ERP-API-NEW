 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Usine_Core.Controllers
{

    public class ServerDetails
    {
        public string servername { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string database { get; set; }
    }
    public class FileUploadAttribute
    {
        public string ContentType { get; set; }
        public string Contents { get; set; }
        public string Filename { get; set; }
        public string FileType { get; set; }
        public string MsgID { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> UploadDate { get; set; }
        public bool Private { get; set; }
        public bool Public { get; set; }
    }
    public class TransactionResult
    {
        public int? recordId { get; set; }
        public string result { get; set; }
        public string seq { get; set; }
    }
    public class General
    {
        
        public String fixString(String s, int i, char j)//To Fix the length of the String
        {

            String x = "";
            String ss;
            if (s.Length > i)
            {
                return (s.Substring(0, i));
            }


            else
            {
                ss = s;

                for (int k = s.Length; k < i; k++)
                {
                    x = x + j.ToString();
                }
            }
            return (s + x);
        }
        public String fixLeftString(String s, int i, char j)//To Fix the length of the String
        {

            String x = "";
            String ss;
            if (s.Length > i)
            {
                return (s.Substring(0, i));
            }


            else
            {
                ss = s;

                for (int k = s.Length; k < i; k++)
                {
                    x = j.ToString() + x;
                }
            }
            return (x + s);
        }


        public String right(String s, int n)
        {

            try
            {
                String k = "";
                String x = "";
                int i;

                for (i = s.Length - 1; i >= s.Length - n; i--)
                {

                    k = k + s[i];

                }
                for (i = k.Length - 1; i >= 0; i--)
                {
                    x = x + k[i];
                }
                return (x);
            }
            catch
            {
                return (s);
            }

        }
        public String makeLine(int wid, char ch)
        {
            String str = "";
            for (int i = 1; i <= wid; i++)
            {
                str = str + ch.ToString();
            }
            return str;
        }
        public string strDate(DateTime x) //To Get the Date Fomate like dd/month/year
        {
            String s;

            s = right(x.Year.ToString(), 2);
            s = zeroMake(x.Day, 2) + "-" + shortMonth(x.Month).ToString() + "-" + s;
            return (s.Trim());
        }

        public string strDateTime(DateTime x) //To Get the Date Fomate like dd/month/year
        {
            String s;

            s = strDate(x) + " " + strShortTime(x);
            return (s.Trim());
        }
        public String strTime(DateTime x)
        {
            String s;
            s = zeroMake(x.Hour, 2) + ":" + zeroMake(x.Minute, 2) + ":" + zeroMake(x.Second, 2);
            return s;
        }
        public String strShortTime(DateTime x)
        {
            String s;
            s = zeroMake(x.Hour, 2) + ":" + zeroMake(x.Minute, 2);
            return s;
        }
        public DateTime monthStart(DateTime dat)
        {
            return DateTime.Parse("1-" + right(strDate(dat), 6));
        }
        public DateTime monthEnd(DateTime dat)
        {
            return monthStart(dat).AddMonths(1).AddDays(-1);
        }
        public String zeroMake(int n, int pad)
        {
            int i;
            String s = "";
            for (i = Convert.ToString(n).Length; i < pad; i++)
            {
                s = s + "0";
            }
            s = s + n;
            return (s);
        }
        public String shortMonth(int m)
        {
            String s;
            s = "";
            switch (m)
            {
                case 1:
                    s = "Jan";
                    break;
                case 2:
                    s = "Feb";
                    break;
                case 3:
                    s = "Mar";
                    break;
                case 4:
                    s = "Apr";
                    break;
                case 5:
                    s = "May";
                    break;
                case 6:
                    s = "Jun";
                    break;
                case 7:
                    s = "Jul";
                    break;
                case 8:
                    s = "Aug";
                    break;
                case 9:
                    s = "Sep";
                    break;
                case 10:
                    s = "Oct";
                    break;
                case 11:
                    s = "Nov";
                    break;
                case 12:
                    s = "Dec";
                    break;
            }
            return (s);
        }

        public int valInt(String str)
        {
            int x = 0;
            try
            {
                x = int.Parse(str);
            }
            catch
            {
                x = 0;
            }
            return x;
        }
        public double valNum(String str)
        {
            double x = 0;
            try
            {
                x = double.Parse(str);
            }
            catch
            {
                x = 0;
            }
            return x;
        }
        public string removeKama(string str)
        {
            string s = "";
            for(var i=0;i<str.Length;i++)
            {
                if(str.Substring(i,1) != ",")
                {
                    s = s + str.Substring(i, 1);
                }
            }
            return s;
        }
        public String fixCur(double X, int n)
        {
            try
            {
                long x1;
                double x2;
                String s;
                x2 = Math.Round(X, n);
                int pox = 0;
                if (x2 < 0)
                {
                    pox = 1;
                }
                x2 = Math.Abs(x2);
                x1 = int.Parse(Math.Floor(x2).ToString().Trim());
                x2 = (x2 - x1) * Math.Pow(10, n);

                x2 = Math.Round(x2, n);


                s = x1.ToString().Trim();


                x1 = int.Parse(x2.ToString());



                s = s + "." + zeroMake(int.Parse(x1.ToString().Trim()), n);
                if (pox == 1)
                {
                    s = "-" + s;
                }
                return (s);
            }
            catch
            {
                return (X.ToString());
            }

        }
        public String makeCur(double d, int x)
        {
            int i = x + 4;
            String ss = "";

            String s = fixCur(d, x);

            if (s.Length <= i)
            {
                return (s);
            }

            else
            {

                ss = right(s, i);
                ss = "," + ss;
                s = s.Substring(0, s.Length - i);
                if (s.Length > 2)
                {
                    ss = "," + right(s, 2) + ss;
                    s = s.Substring(0, s.Length - 2);
                }

                if (s.Length > 2)
                {
                    ss = "," + right(s, 2) + ss;
                    s = s.Substring(0, s.Length - 2);
                }
                ss = s + ss;
                if (s.Substring(0, 1) == ",")
                {
                    ss = s.Substring(2, s.Length - 1);
                }
                return (ss);
            }
        }
      
        public String sendMailDetails(String fname, String header)
        {
            return "Details not set";
        }

        public String numWord(long p)
        {
            try
            {
                String s = "", ss = "";
                long x;
                int n, a, b;
                x = p;
                n = int.Parse(Math.Floor(x / 10000000.0).ToString());
                if (n >= 10)
                {
                    if (n < 20)
                    {
                        s = teenStr(n);
                    }
                    else
                    {
                        b = n % 10;
                        a = int.Parse(Math.Floor(n / 10.0).ToString());
                        s = s + tenStr(a) + unitStr(b);
                    }
                }
                else
                {
                    if (n < 10)
                    {
                        s = unitStr(n);
                    }
                }

                if (n > 0)
                {
                    if (n == 1)
                        ss = ss + s + " Crore ";
                    else
                        ss = ss + s + " Crores ";
                    x = x % 10000000;
                }
                s = "";
                n = int.Parse(Math.Floor(x / 100000.0).ToString());
                if (n >= 10)
                {
                    if (n < 20)
                    {
                        s = tenStr(n);
                    }
                    else
                    {
                        b = n % 10;
                        a = int.Parse(Math.Floor(n / 10.0).ToString());
                        s = s + tenStr(a) + unitStr(b);
                    }
                }
                else
                {
                    if (n < 10)
                    {
                        s = unitStr(n);
                    }

                }

                if (n > 0)
                {
                    if (n == 1)
                        ss = ss + s + " lakh ";
                    else
                        ss = ss + s + " lakhs ";
                    x = x % 100000;
                }
                s = "";
                n = int.Parse(Math.Floor(x / 1000.0).ToString());
                if (n >= 10)
                {
                    if (n < 20)
                    {
                        s = teenStr(n);
                    }
                    else
                    {
                        b = n % 10;
                        a = int.Parse(Math.Floor(n / 10.0).ToString());
                        s = s + tenStr(a) + unitStr(b);
                    }
                }
                else
                {
                    if (n < 10)
                    {
                        s = unitStr(n);
                    }
                }
                if (n > 0)
                {
                    if (n == 1)
                    {
                        ss = ss + s + " thousand ";
                    }
                    else
                    {
                        ss = ss + s + " thousands ";
                    }
                    x = x % 1000;
                }

                s = "";
                n = int.Parse(Math.Floor(x / 100.0).ToString());
                if (n >= 10)
                {
                    if (n < 20)
                    {
                        s = teenStr(n);
                    }
                    else
                    {
                        b = n % 10;
                        a = int.Parse(Math.Floor(n / 10.0).ToString());
                        s = s + tenStr(a) + unitStr(b);
                    }
                }
                else
                {
                    if (n < 10)
                    {
                        s = unitStr(n);
                    }
                }
                if (n > 0)
                {
                    x = x % 100;
                    if (x > 0)
                    {
                        ss = ss + s + " hundred and ";
                    }
                    else
                    {
                        ss = ss + s + " hundred ";
                    }
                }
                else
                {
                    x = x % 100;
                    if (x > 0)
                    {
                    }
                }
                s = "";
                n = int.Parse(Math.Floor(x % 100.0).ToString());
                if (n >= 10)
                {
                    if (n < 20)
                    {
                        s = teenStr(n);
                    }
                    else
                    {
                        b = n % 10;
                        a = int.Parse(Math.Floor(n / 10.0).ToString());
                        s = s + tenStr(a) + unitStr(b);
                    }
                }
                else
                {
                    if (n < 10)
                    {
                        s = unitStr(n);
                    }
                }
                if (n > 0)
                {
                    ss = ss + s;
                }
                s = ss.Substring( 0, 1);
                s = s.ToUpper();
                ss = s + ss.Substring( 1, ss.Length-1);
                return (ss);
            }
            catch(Exception ee)
            {
                return (" ");
            }
        }
        private String unitStr(int x)
        {
            String s = "";
            switch (x)
            {
                case 0:
                    s = "";
                    break;
                case 1:
                    s = "one";
                    break;
                case 2:
                    s = "two";
                    break;
                case 3:
                    s = "three";
                    break;
                case 4:
                    s = "four";
                    break;
                case 5:
                    s = "five";
                    break;
                case 6:
                    s = "six";
                    break;
                case 7:
                    s = "seven";
                    break;
                case 8:
                    s = "eight";
                    break;
                case 9:
                    s = "nine";
                    break;
            }
            return (s);
        }
        private String tenStr(int x)
        {
            String s = "";

            switch (x)
            {
                case 0:
                    s = "";
                    break;
                case 1:
                    s = "";
                    break;
                case 2:
                    s = "twenty ";
                    break;
                case 3:
                    s = "thirty ";
                    break;
                case 4:
                    s = "forty ";
                    break;
                case 5:
                    s = "fifty ";
                    break;
                case 6:
                    s = "sixty ";
                    break;
                case 7:
                    s = "seventy ";
                    break;
                case 8:
                    s = "eighty ";
                    break;
                case 9:
                    s = "ninety ";
                    break;
            }
            return (s);

        }

        private String teenStr(int x)
        {
            String s = "";
            switch (x)
            {
                case 10:
                    s = "ten ";
                    break;
                case 11:
                    s = "eleven ";
                    break;
                case 12:
                    s = "twelve ";
                    break;
                case 13:
                    s = "thirteen ";
                    break;
                case 14:
                    s = "fourteen ";
                    break;
                case 15:
                    s = "fifteen ";
                    break;
                case 16:
                    s = "sixteen ";
                    break;
                case 17:
                    s = "seventeen ";
                    break;
                case 18:
                    s = "eighteen ";
                    break;
                case 19:
                    s = "ninteen ";
                    break;
            }
            return (s);
        }

        public ServerDetails setServerDetails()
        {
           
            ServerDetails ser = new ServerDetails();

            //ser.servername = "DESKTOP-FND9GDG";
            //ser.username = "";
            //ser.password = "";
            //ser.database = "usine";

            ser.servername = "192.168.29.53,49792";
            ser.username = "sa";
            ser.password = "C0rtr@ck3r@2024@0124";
            ser.database = "DevDB";



            return ser;
        }
    }
}
