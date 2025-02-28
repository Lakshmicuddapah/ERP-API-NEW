
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Usine_Core.Controllers.Admin
{
    public class DashboardPostDetails
    {
       public List<FileUploadAttribute> imgs { get; set; }
        public string postinfo { get; set; }   
        public UserInfo usr { get; set; }
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
    public class DashboardGeneralController : ControllerBase
    {
        private readonly IHostingEnvironment ho;
        public DashboardGeneralController(IHostingEnvironment hostingEnvironment)
        {

            ho = hostingEnvironment;
        }
        [HttpPost]
        [Route("api/DashboardGeneral/PostGeneralPosts")]
        public string PostGeneralPosts([FromBody] DashboardPostDetails postdet)
        {
            string msg = "";
            try
            {
                if (postdet.imgs != null && postdet.imgs.Count > 0)
                {
                    postdet.imgs.ForEach(x =>
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
                                string filename = ho.WebRootPath + "\\Attachments\\" + "POST" + x.Filename;
                                byte[] arbytes = System.Convert.FromBase64String(x.Contents);
                                MemoryStream ms = new MemoryStream(arbytes);
                                Image returnImage = Image.FromStream(ms);
                                returnImage.Save(filename);
                                msg = "OK";
                            }
                          
                            catch (Exception ex)
                            {
                                msg=ex.Message;
                            }
                        }
                    });
         
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }
    }
}
