using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Summary description for RSVGExport
    /// </summary>
    public class RSVGExport : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            string picPath = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data", "RSVGConverter", "chartExport");
            string fileName = Path.ChangeExtension(picPath, "svg");
            string downloadFileName = context.Request.Params["type"].ToString() + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + context.Request.Params["output_format"].ToString();
            string svg = (context.Request.Params["data"].ToString());
            try
            {
                string newSvg = Uri.UnescapeDataString(svg);
                Process p = new Process();
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "cmd.exe";
                info.RedirectStandardInput = true;
                info.UseShellExecute = false;
                p.StartInfo = info;
                p.Start();
                /*var stream = newSvg.ToString();

                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    sw.WriteLine(stream);
                }*/
                File.WriteAllText(fileName, newSvg);
                fileName = fileName.Substring(fileName.LastIndexOf(@"\") + 1);

                using (StreamWriter sw = p.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.WriteLine(context.Request.PhysicalApplicationPath.Split(':')[0] + ":");
                        sw.WriteLine("cd " + Path.Combine(context.Request.PhysicalApplicationPath, "App_Data", "RSVGConverter"));
                        if (context.Request.Params["output_format"].ToString() == "pdf")
                        {
                            sw.WriteLine("rsvg-convert -o " + downloadFileName + " -b white  -f pdf chartExport.svg");
                        }
                        else if (context.Request.Params["output_format"].ToString() == "jpg")
                        {
                            sw.WriteLine("rsvg-convert -o " + downloadFileName + " -b white   -f jpg chartExport.svg");

                        }

                        else
                        {
                            sw.WriteLine("rsvg-convert -o " + downloadFileName + " -b white -f png chartExport.svg");
                        }
                    }
                    sw.Close();
                    sw.Dispose();
                }

                Thread.Sleep(900);

                System.Web.HttpResponse response = context.Response;
                response.ContentType = "application/octet-stream";
                response.AddHeader("Content-Disposition", "attachment; filename=" + downloadFileName + ";");
                response.WriteFile(Path.Combine(context.Request.PhysicalApplicationPath, "App_Data", "RSVGConverter", downloadFileName));
                response.Flush();
                response.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                File.Delete(Path.Combine(context.Request.PhysicalApplicationPath, "App_Data", "RSVGConverter", downloadFileName));
            }
        }



        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}