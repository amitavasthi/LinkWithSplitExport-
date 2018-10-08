using Homescreen1.Classes;
using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages.CustomCharts
{
    public partial class ChartsHome : WebUtilities.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RenderAvailableCharts();

        }

        protected void SavedReport_Click(object sender, ImageClickEventArgs e)
        {
            // Cast the sending object as image button.
            ImageButton imgDirectory = (ImageButton)sender;

            var id = imgDirectory.ID.Split('_');

            Response.Redirect(
                "/Pages/Dashboards.aspx?val=" + id[1]
            );
        }

        private string PrepareFileName(string name)
        {
            string result = name;

            int c = 0;
            for (int i = 0; i < name.Length; i++)
            {
                if (i != 0 && i % 15 == 0)
                {
                    result = result.Insert(i + c, "<br />");

                    c += 6;
                }
            }

            return result;
        }
        private void RenderAvailableCharts()
        {
            string directoryName = Path.Combine(
                   HttpContext.Current.Request.PhysicalApplicationPath,
                   "Fileadmin",
                   "DashboardItems",
                   ((DatabaseCore.Core)HttpContext.Current.Session["Core"]).ClientName
               );

            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            // Get the client's saved linkBi definitions.
            string[] dashboardItems = Directory.GetFiles(directoryName);

            // Run through all saved linkBI reports of the client.
            foreach (string dashboardItem in dashboardItems)
            {
                DashboardItem info = new DashboardItem(dashboardItem);

                Panel pnlFile = new Panel();
                pnlFile.CssClass = "CloudItem File";

                Image imgFile = new Image();
                imgFile.ID = "img_" + info.Id;
                imgFile.CssClass = "BackgroundColor1";
                imgFile.Attributes.Add("Source", info.Source);
                imgFile.Style.Add("cursor", "pointer");

                imgFile.Attributes.Add(
                     "onmousedown",
                     "showSubmitLoading = false;"
                );

                imgFile.ImageUrl = info.Icon;

                imgFile.Attributes.Add(
                  "onmouseover",
                  "this.src = '/Images/Icons/Charts/play.png'"
                 );
                imgFile.Attributes.Add(
                    "onmouseout",
                    "this.src = '" + info.Icon + "';document.forms[0].action = document.forms[0].action.split('?')[0];"
                 );
                imgFile.Attributes.Add("onclick", string.Format(
                    "window.location = '/Pages/Dashboards.aspx?IdDashboardItem={0}';",
                    info.Id
                ));

                Label lblFile = new Label();
                //lblFile.Text = "<br />" + info.Name;
                lblFile.Text = info.Name;


                pnlFile.Controls.Add(imgFile);
                //  pnlFile.Controls.Add(lblFile);               

                pnlFile.Controls.Add(new LiteralControl(string.Format(
                      "<div onmouseover=\"this.title=this.innerText\" style=\"max-height:70px;max-width:350px;overflow: hidden; text-overflow: ellipsis;\"; class=\"DirectoryName\">{0}</div>",
                    PrepareFileName(lblFile.Text)
                )));

                pnlFiles.Controls.Add(pnlFile);


            }
        }
    }
}