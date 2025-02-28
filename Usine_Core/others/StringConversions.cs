
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usine_Core.Controllers;

namespace Usine_Core.others
{
    class StringConversions
    {
        public String makeStringToAscii(String str)
        {
            General g = new General();
            String s = "";
            String s1 = str;
            String s2 = "";
            int x = 0;
            int i;
            for (i = 0; i < str.Length; i += 1)
            {
                s2 = s1.Substring(i, 1);
                char ch = Convert.ToChar(s2);
                x = (int)ch;
                s = s + g.zeroMake(x, 3) + "0";
            }
            return (s);
        }
        public String makeAsciitoString(String str)
        {
            String s = "";
            String s1 = str;
            General g = new General();
            int i;
            int x = 0;
            for (i = 0; i < str.Length; i += 4)
            {
                s1 = str.Substring(i, 3); // g.midString(str, i, 3);
                x = g.valInt(s1);
                char ch = (char)(x);
                s = s + ch.ToString();
            }
            return (s);
        }
         
        

    }
}
