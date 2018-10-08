using Crosstables.Classes.ReportDefinitionClasses;
using LinkBi1.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using WebUtilities.Controls;

namespace Homescreen1.Classes.Modules
{
    public class HomescreenModuleNews : HomescreenModule
    {
        #region Properties

        #endregion


        #region Constructor

        public HomescreenModuleNews(HomescreenNode node)
            : base(node)
        { }

        #endregion


        #region Methods

        private string RenderCalendar(DateTime creationDate)
        {
            StringBuilder result = new StringBuilder();

            result.Append("<table class=\"NewsItemCalendar BorderColor1\" cellspacing=\"0\" cellpadding=\"0\">");

            result.Append(string.Format(
                "<tr><td class=\"NewsItemCalendarMonth BackgroundColor1\">{0}</td></tr>",
                this.Node.Owner.LanguageManager.GetText("CalendarMonthShort" + creationDate.Month)
            ));
            result.Append(string.Format(
                "<tr><td class=\"NewsItemCalendarDay\">{0}</td></tr>",
                creationDate.Day
            ));

            result.Append("</table>");

            return result.ToString();
        }

        private void RenderNoChartMessage(StringBuilder writer)
        {
            writer.Append(string.Format(
                "<div class=\"HomescreenNoChartDefinedSmiley\">:(</div><div class=\"HomescreenNoChartDefinedText\">{0}</div>",
                this.Node.Owner.LanguageManager.GetText("HomescreenNoChartDefined")
            ));
        }

        public override void Render(StringBuilder writer)
        {
            // Build the full path to the client's news definition file.
            string fileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "News",
                ((DatabaseCore.Core)HttpContext.Current.Session["Core"]).ClientName + ".xml"
            );

            // Check if there are any news defined for the client.
            if (!File.Exists(fileName))
            {
                RenderNoChartMessage(writer);
                return;
            }

            // Create a new xml document that
            // contains the client's news entries.
            XmlDocument document = new XmlDocument();

            document.Load(fileName);

            XmlNodeList xmlNodesNews = document.SelectNodes("ClientNews/NewsList/News");

            if(xmlNodesNews.Count == 0)
            {
                RenderNoChartMessage(writer);
                return;
            }

            /*writer.Append(string.Format(
                "<div class=\"NewsContainer\" style=\"height:{0}px\">",
                this.Node.Height
            ));*/
            writer.Append(string.Format(
                "<div class=\"NewsContainer\" HeightScript=\"{0}\">",
                this.Node.Height
            ));

            int itemCount = this.Node.Owner.BaseContentHeight / 65;
            //int itemCount = 1;

            if (itemCount <= 0)
                itemCount = 1;

            int i = 0;
            foreach (XmlNode xmlNode in xmlNodesNews.OrderByDate("CreatedDate"))
            {
                if (i++ == itemCount)
                    break;

                DateTime creationDate = DateTime.Parse(
                    xmlNode.Attributes["CreatedDate"].Value
                );

                writer.Append(string.Format(
                    "<table class=\"NewsItem\" cellspacing=\"0\" cellpadding=\"0\"><tr>"+
                    "<td rowspan=\"2\">{0}</td>"+
                    "<td class=\"NewsItemTitle\">{1}</td>"+
                    "<tr><td class=\"NewsItemText\"><div style=\"max-height:35px;overflow:hidden;\">{2}</div></td></tr></table>",
                    RenderCalendar(creationDate),
                    xmlNode.Attributes["Heading"].Value,
                    xmlNode.Attributes["Description"].Value
                ));
            }

            writer.Append("</div>");
        }

        #endregion
    }
}
