using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages
{
    public partial class SamplePage : WebUtilities.BasePage 
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string fileName;
            string chartDirectory;
            string directoryName;
            chartDirectory = Path.Combine(
                  Request.PhysicalApplicationPath,
                  "Fileadmin",
                  "HomeScreen",
                  Global.Core.ClientName,
                  "SavedChart",
                  Global.User.Id.ToString()
                );

            if (Directory.Exists(chartDirectory))
            {
                directoryName = Directory.GetDirectories(chartDirectory).OrderByDescending(file => new FileInfo(file).LastWriteTime).FirstOrDefault();
                if (Directory.Exists(directoryName))
                {
                    fileName = Directory.GetFiles(directoryName).OrderByDescending(file => new FileInfo(file).LastWriteTime).FirstOrDefault();


                    Crosstables.Classes.Crosstable crosstable = new Crosstables.Classes.Crosstable(
                                                                                   Global.Core,
                                                                                   fileName
                                                                               );
                    crosstable.FilterClickAction = "ctl00$cphContent$btnDisplayFilters";
                    pnl.Controls.Add(crosstable);
                }
            }
        }
    }
}