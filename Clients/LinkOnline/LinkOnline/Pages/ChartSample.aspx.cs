using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages
{
    public partial class ChartSample : WebUtilities.BasePage
    {
        public ChartSample()
            : base(false, false)
        { }
        protected void Page_Load(object sender, EventArgs e)
        {
            //var jSONUrl = "http://localhost:54580/Handlers/LinkBiExternal.ashx?Method=ProcessReport&Dimension1=Month&Dimension2=OrgSizeSegment&Measure1=CountofServerswithWindowsServer2000&Measure2=CountofServerswithWindowsServer2003&Measure3=CountofServerswithWindowsServer2008&Measure4=CountofServerswithWindowsServer2012&Measure5=CountofServerswithRedHatLinux&ResponseType=JSON";
            //string htmlBody = string.Empty;
            //HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(jSONUrl);
            //myRequest.Method = "GET";
            //WebResponse myResponse = myRequest.GetResponse();
            //using (StreamReader reader = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8))
            //{
            //    htmlBody = reader.ReadToEnd();
            //}


            //if (File.Exists(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "JSON", Global.Core.ClientName + "CustomChart.txt")))
            //{
            //    File.Delete(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "ChartDetails", Global.Core.ClientName + "CustomChart.html"));
            //}
            //using (StreamWriter outfile = new StreamWriter(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "ChartDetails", Global.Core.ClientName + "CustomChart.html")))
            //{
            //    outfile.Write(htmlBody.ToString());
            //}

        }
    }
}