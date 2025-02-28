using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;

namespace Usine_Core.Controllers
{
    public class DataBaseContext
    {

        public SqlConnection db;
        public SqlConnection dc;

        public int maxLevels;
        public string agentPrefix;
        public int maxAmountTimes;
        public int defaultLevelsPerOne;
        public double defaultBonus;
        public int maxDays;
        public DataBaseContext()
        {
            String s1, s2, s3, s4;

            General g = new General();
            var ser = g.setServerDetails();
            s1 = ser.servername;
            s2 = ser.username;
            s3 = ser.password;
            s4 = ser.database;
           

                if (s2.Trim() == "")
                {
                    db = new SqlConnection("data source=" + s1 + ";initial catalog=" + s4 + ";integrated security=true;MultipleActiveResultSets=True;");
                }

                else
                {
                    db = new SqlConnection("data source=" + s1 + ";initial catalog=" + s4 + ";user id=" + s2 + ";password=" + s3 + ";integrated security=false;MultipleActiveResultSets=True;");

                }
             
        }

        public string imageUploadGeneric(List<FileUploadAttribute> imgs,string fname)
        {
            int msg = -1;
            string result = "";
            try
            {
                if (imgs != null && imgs.Count > 0)
                {
                    imgs.ForEach(x =>
                    {
                        if (x.Filename != "" && x.Filename != null && x.Contents != null)
                        {
                            string fileName = x.Filename;
                            int fileExtPos = fileName.LastIndexOf(".");
                            if (fileExtPos >= 0)
                            {
                                fileName = fileName.Substring(0, fileExtPos);
                            }

                            try
                            {
                                
                                string filename = fname;
                                byte[] arbytes = System.Convert.FromBase64String(x.Contents);
                                MemoryStream ms = new MemoryStream(arbytes);
                                Image returnImage = Image.FromStream(ms);
                                returnImage.Save(filename);
                                msg = 1;
                                ms.Dispose();
                                result = "OK";
                            }

                            catch (Exception ex)
                            {
                                msg = 0;
                                result = ex.Message;
                            }
                        }
                        else
                        {
                            msg = 0;
                            result = "Wrong file format";
                        }
                    });

                }
                else
                {
                    msg = 0;
                    result = "No file found";
                }
            }
            catch (Exception ex)
            {
                msg = 0;
                result = ex.Message;
            }
             
            return result;

        }
    }
}
