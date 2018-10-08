using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace DashboardCore.Exporter
{
    public abstract class DashboardExporter
    {
        #region Properties

        public string Extension { get; set; }
        public string MimeType { get; set; }

        public Dashboard Dashboard { get; set; }

        #endregion


        #region Constructor

        public DashboardExporter(Dashboard dashboard)
        {
            this.Dashboard = dashboard;
        }

        #endregion


        #region Methods

        public abstract string Export(string html, string fileName);

        public Panel ConvertHtmlTableStringToTable(string html)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(html);

            Panel result = new Panel();

            Control control = ConvertHtmlToControl(document.DocumentElement);

            if (control != null)
                result.Controls.Add(control);

            return result;
        }

        public Control ConvertHtmlToControl(XmlNode control)
        {
            WebControl result;

            bool copyAttributes = true;

            switch (control.Name)
            {
                case "table":
                    result = new Table();
                    break;
                case "tr":
                    if (control.ChildNodes.Count == 0)
                        return null;
                    result = new TableRow();
                    break;
                case "td":
                    result = new TableCell();

                    if (control.Attributes["rowspan"] != null)
                    {
                        ((TableCell)result).RowSpan = int.Parse(control.Attributes["rowspan"].Value);
                    }
                    if (control.Attributes["colspan"] != null)
                    {
                        ((TableCell)result).ColumnSpan = int.Parse(control.Attributes["colspan"].Value);
                    }
                    break;
                default:
                    copyAttributes = false;
                    result = new Panel();
                    ((Panel)result).Controls.Add(new LiteralControl(control.InnerText));
                    break;
            }

            if (copyAttributes)
            {
                foreach (XmlAttribute attribute in control.Attributes)
                {
                    result.Attributes.Add(attribute.Name, attribute.Value);
                }

                foreach (XmlNode xmlNodeChild in control.ChildNodes)
                {
                    Control c = ConvertHtmlToControl(xmlNodeChild);

                    if (c != null)
                        result.Controls.Add(c);
                }
            }

            return result;
        }

        #endregion
    }
}
