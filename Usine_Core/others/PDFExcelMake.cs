using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.factories;
using iTextSharp.text.xml;
using System.Data;
using ClosedXML.Excel;
using System.IO;
using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Hosting;

namespace Usine_Core.Controllers.Others
{
    public class PDFExcelMake
    {

      
        public string pdfConversion(string filename,String header, UserInfo usr, DataTable dt, List<string> titles, float[] widths, int[] aligns, Boolean footer)
        {
            try
            {
                LoginControlController ll = new LoginControlController();
                UserAddress addr = ll.makeBranchAddress(usr.bCode, usr.cCode);
                using (FileStream ms = new FileStream(filename, FileMode.Append, FileAccess.Write))

                {
                        Document document = new Document(PageSize.A4, 75f, 40f, 20f, 0f);
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);

                    document.Open();
                    int pagelines = 58;
                    int pageno = 1;

                    int totalpages = dt.Rows.Count / pagelines;
                    if (dt.Rows.Count % pagelines != 0)
                    {
                        totalpages++;
                    }
                    AdminControl am = new AdminControl();
                    am.makeHeader(document, header, totalpages, pageno, addr);
                    makeHeader(document, header, titles, widths, aligns,550);
                    PdfPTable ptot1 = new PdfPTable(1);
                    float[] widths1 = new float[] { 550f };
                    ptot1.SetWidths(widths1);
                    ptot1.TotalWidth = 550f;
                    ptot1.LockedWidth = true;
                    PdfPTable ptot = new PdfPTable(titles.Count);
                    ptot.SetWidths(widths);
                    ptot.TotalWidth = 550f;
                    ptot.LockedWidth = true;

                    iTextSharp.text.Font fn;

                    PdfPCell plC;

                    int i, j;
                    i = 1;
                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 8, Font.NORMAL));
                    for (i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int k = 0; k < titles.Count; k++)
                        {
                            plC = new PdfPCell(new Phrase(dt.Rows[i][k].ToString(), fn));
                            plC.BorderWidth = 0f;
                            if (footer && i == dt.Rows.Count - 1)
                            {
                                plC.BorderWidthTop = 1f;
                                plC.BorderWidthBottom = 1f;
                            }
                            plC.HorizontalAlignment = aligns[k] == 1 ? PdfPCell.ALIGN_LEFT : aligns[k] == 2 ? PdfPCell.ALIGN_RIGHT : PdfPCell.ALIGN_CENTER;
                            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                            ptot.AddCell(plC);
                        }
                        if (i % pagelines == 0 && i != 0 && i != dt.Rows.Count - 1)
                        {
                            document.Add(ptot);
                            am.makeFooter(document, usr);
                            document.NewPage();

                            ptot = new PdfPTable(widths.Length);
                            ptot.SetWidths(widths);
                            ptot.TotalWidth = 550f;
                            ptot.LockedWidth = true;
                            pageno++;
                            am.makeHeader(document, header, totalpages, pageno, addr);
                            makeHeader(document, header, titles, widths, aligns,550);
                        }
                    }

                    i = i % pagelines;
                    if (i > 0)
                    {
                        while (i <= pagelines)
                        {
                            for (j = 0; j < titles.Count; j++)
                            {
                                fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 8, Font.NORMAL));
                                plC = new PdfPCell(new Phrase("  ", fn));
                                plC.BorderWidth = 0f;
                                plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                ptot.AddCell(plC);
                            }
                            i++;
                        }
                        document.Add(ptot);
                        am.makeFooter(document, usr);
                    }
                    else
                    {
                        document.Add(ptot);
                    }

                    document.Close();
                    //totalpdf = ms.GetBuffer();
                    //ms.Flush();
                    //ms.Dispose();
                }

                return "OK";
                
            }
            catch
            {
                return "Error";
            }

        }


        public string pdfLandscapeConversion(string filename, String header, UserInfo usr, DataTable dt, List<string> titles, float[] widths, int[] aligns, Boolean footer)
        {
            try
            {
                

                
                LoginControlController ll = new LoginControlController();
                UserAddress addr = ll.makeBranchAddress(usr.bCode, usr.cCode);



                using (FileStream ms = new FileStream(filename, FileMode.Append, FileAccess.Write))

                {
                    Document document = new Document(PageSize.A4.Rotate(), 75f, 40f, 20f, 0f);
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);

                    document.Open();
                    int pagelines = 37;
                    int pageno = 1;

                    int totalpages = dt.Rows.Count / pagelines;
                    if (dt.Rows.Count % pagelines != 0)
                    {
                        totalpages++;
                    }
                    AdminControl am = new AdminControl();
                    am.makeLandscapeHeader(document, header, totalpages, pageno, addr);
                    makeHeader(document, header, titles, widths, aligns,720);
                    PdfPTable ptot1 = new PdfPTable(1);
                    float[] widths1 = new float[] { 720f };
                    ptot1.SetWidths(widths1);
                    ptot1.TotalWidth = 720f;
                    ptot1.LockedWidth = true;
                    PdfPTable ptot = new PdfPTable(titles.Count);
                    ptot.SetWidths(widths);
                    ptot.TotalWidth = 720f;
                    ptot.LockedWidth = true;

                    iTextSharp.text.Font fn;

                    PdfPCell plC;

                    int i, j;
                    i = 1;
                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 8, Font.NORMAL));
                    for (i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int k = 0; k < titles.Count; k++)
                        {
                            plC = new PdfPCell(new Phrase(dt.Rows[i][k].ToString(), fn));
                            plC.BorderWidth = 0f;
                            if (footer && i == dt.Rows.Count - 1)
                            {
                                plC.BorderWidthTop = 1f;
                                plC.BorderWidthBottom = 1f;
                            }
                            plC.HorizontalAlignment = aligns[k] == 1 ? PdfPCell.ALIGN_LEFT : aligns[k] == 2 ? PdfPCell.ALIGN_RIGHT : PdfPCell.ALIGN_CENTER;
                            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                            ptot.AddCell(plC);
                        }
                        if (i % pagelines == 0 && i != 0 && i != dt.Rows.Count - 1)
                        {
                            document.Add(ptot);
                            am.makeLandscapeFooter(document, usr);
                            document.NewPage();

                            ptot = new PdfPTable(widths.Length);
                            ptot.SetWidths(widths);
                            ptot.TotalWidth = 720f;
                            ptot.LockedWidth = true;
                            pageno++;
                            am.makeLandscapeHeader(document, header, totalpages, pageno, addr);
                            makeHeader(document, header, titles, widths, aligns,720);
                        }






                    }

                    i = i % pagelines;
                    if (i > 0)
                    {
                        while (i <= pagelines)
                        {
                            for (j = 0; j < titles.Count; j++)
                            {
                                fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 8, Font.NORMAL));
                                plC = new PdfPCell(new Phrase("  ", fn));
                                plC.BorderWidth = 0f;
                                plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                ptot.AddCell(plC);
                            }
                            i++;
                        }
                        document.Add(ptot);
                        am.makeLandscapeFooter(document, usr);
                    }
                    else
                    {
                        document.Add(ptot);
                    }

                    document.Close();
                    //totalpdf = ms.GetBuffer();
                    //ms.Flush();
                    //ms.Dispose();
                }

                 return "OK";
                
            }
            catch (Exception ee)
            {
                return "Error";
            }

        }


        public void makeHeader(Document document, String hea, List<string> titles, float[] widths, int[] aligns, float totalwid)
        {

            document.Add(localHeader(titles, widths, aligns,totalwid));
        }
        public PdfPTable localHeader(List<string> titles, float[] widths, int[] aligns,float totalwid)
        {
            PdfPTable ptot = new PdfPTable(titles.Count);

            ptot.TotalWidth = totalwid;
            ptot.SetWidths(widths);
            ptot.LockedWidth = true;

            iTextSharp.text.Font fn;
            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 9, Font.NORMAL));

            for (int i = 0; i < titles.Count; i++)
            {
                PdfPCell plC = new PdfPCell(new Phrase(titles[i], fn));

                plC.BorderWidth = 0f;
                plC.BorderWidthBottom = 1f;
                plC.BorderWidthTop = 1f;
                plC.HorizontalAlignment = aligns[i] == 1 ? PdfPCell.ALIGN_LEFT : aligns[i] == 2 ? PdfPCell.ALIGN_RIGHT: PdfPCell.ALIGN_CENTER;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                ptot.AddCell(plC);
            }



            return (ptot);
        }




        public string makeExcelConversion(string filename, String header, UserInfo usr, DataTable dt, List<string> titles, float[] widths, int[] aligns, Boolean footer)
        {

            try
            {
               

                MemoryStream totalexcel = new MemoryStream();
                LoginControlController ll = new LoginControlController();
                UserAddress addr = ll.makeBranchAddress(usr.bCode, usr.cCode);
                AdminControl ac = new AdminControl();
                using (var wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("Sheet1");
                    ws.Rows("1").Height = 20;
                    for (int i = 0; i < titles.Count; i++)
                    {
                        ws.Columns(((char)(65 + i)).ToString()).Width = widths[i] / 5;
                    }

                    ws.Cell(1, 1).Value = addr.branchName;
                    ws.Range(1, 1, 1, titles.Count).Merge().AddToNamed("Company name");
                    ws.Range(1, 1, 1, titles.Count).Merge().Style.Font.FontName = "Arial";
                    ws.Range(1, 1, 1, titles.Count).Merge().Style.Font.FontSize = 16;

                    ws.Cell(2, 1).Value = ac.removeEnter(addr.address) + ", " + addr.city;
                    ws.Range(2, 1, 2, titles.Count).Merge().AddToNamed("Company address");
                    ws.Range(2, 1, 2, titles.Count).Merge().Style.Font.FontName = "Arial";
                    ws.Range(2, 1, 2, titles.Count).Merge().Style.Font.FontSize = 10;

                  
                    ws.Cell(4, 1).Value = header;
                    ws.Range(4, 1, 4, titles.Count).Merge().Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                    ws.Range(4, 1, 4, titles.Count).Merge().AddToNamed("Header");
                    ws.Range(4, 1, 4, titles.Count).Merge().Style.Font.FontName = "Arial";
                    ws.Range(4, 1, 4, titles.Count).Merge().Style.Font.FontSize = 9;
                    ws.Range(4, 1, 4, titles.Count).Merge().Style.Font.Italic = true;

                    for (int i = 1; i <= titles.Count; i++)
                    {
                        ws.Cell(5, i).Style.Border.TopBorder = ClosedXML.Excel.XLBorderStyleValues.DashDot;
                        ws.Cell(5, i).Style.Border.BottomBorder = ClosedXML.Excel.XLBorderStyleValues.DashDot;
                        ws.Cell(5, i).Style.Font.FontSize = 9;
                        ws.Cell(5, i).Style.Font.Bold = true;
                        ws.Cell(5, i).Value = titles[i - 1];

                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            ws.Cell(i + 6, j + 1).Style.Font.FontSize = 9;
                            ws.Cell(i + 6, j + 1).Style.Font.Bold = false;
                            ws.Cell(i + 6, j + 1).Value = dt.Rows[i][j].ToString();
                        }
                    }



                    wb.SaveAs(filename);

                }
                return "OK";
            }
            catch(Exception ee)
            {
                return "Error";
            }
        }











        public String accountsVoucherMake()
        {
            return "OK";
        }







    }
}
