using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DashboardCore
{
    public class DashboardSettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets the owning dashboard.
        /// </summary>
        public Dashboard Dashboard { get; set; }

        /// <summary>
        /// Gets or sets a list to all the
        /// includes used in the dashboard.
        /// </summary>
        public List<string> Includes { get; set; }

        public XmlNode XmlNode { get; set; }

        public Crosstables.Classes.CrosstableSettings ReportSettings { get; set; }

        public DashboardExportFormat ExportFormat { get; set; }

        #endregion


        #region Settings

        public bool DivTables { get; set; }

        #endregion


        #region Constructor

        public DashboardSettings(Dashboard dashboard)
        {
            this.Dashboard = dashboard;
            this.XmlNode = this.Dashboard.Document.DocumentElement.SelectSingleNode("Settings");

            this.Parse();
        }

        #endregion


        #region Methods

        private void Parse()
        {
            this.Includes = new List<string>();

            XmlNode xmlNodeExportFormat = this.XmlNode.SelectSingleNode("ReportSettings/ExportFormat");

            if (xmlNodeExportFormat != null)
            {
                this.ExportFormat = (DashboardExportFormat)Enum.Parse(
                    typeof(DashboardExportFormat),
                    xmlNodeExportFormat.InnerXml
                );
            }

            // Run through all xml nodes that define a stylesheet.
            foreach (XmlNode xmlNode in this.XmlNode.SelectNodes("Includes/*"))
            {
                if (xmlNode.Name == "Component")
                {
                    string directoryName = Path.Combine(
                        ConfigurationManager.AppSettings["DashboardIncludeRoot"],
                        xmlNode.Attributes["Name"].Value
                    );

                    if (!Directory.Exists(directoryName))
                        continue;

                    foreach (string includeFile in Directory.GetFiles(directoryName))
                    {
                        StringBuilder html = new StringBuilder();
                        string tagName = "";

                        if (includeFile.EndsWith(".css"))
                        {
                            tagName = "style";
                            html.Append(string.Format(
                                "<{0} type=\"text/css\"",
                                tagName
                            ));
                        }
                        else if (includeFile.EndsWith(".js"))
                        {
                            tagName = "script";
                            html.Append(string.Format(
                                "<{0} type=\"text/javascript\"",
                                tagName
                            ));
                        }

                        html.Append(string.Format(
                            " Component=\"{0}\">",
                            xmlNode.Attributes["Name"].Value
                        ));

                        html.Append(File.ReadAllText(includeFile));

                        html.Append(string.Format("</{0}>", tagName));

                        this.Includes.Add(html.ToString());
                    }
                }
                else
                {
                    if (xmlNode.Attributes["Source"] != null)
                    {
                        string fileName = Path.Combine(
                            Path.GetDirectoryName(this.Dashboard.FileName),
                            xmlNode.Attributes["Source"].Value
                        );

                        xmlNode.InnerXml = "###REPLACE###";
                        this.Includes.Add(xmlNode.OuterXml.Replace("###REPLACE###", File.ReadAllText(fileName)));
                    }
                    else
                    {
                        this.Includes.Add(xmlNode.OuterXml);
                    }
                }
            }

            this.ParseSettings();
            this.ParseReportSettings();
        }

        private void ParseSettings()
        {
            this.DivTables = GetSetting<bool>("DivTables", bool.Parse, false);
        }

        private void ParseReportSettings()
        {
            this.ReportSettings = new Crosstables.Classes.
                CrosstableSettings(null, this.XmlNode.SelectSingleNode("ReportSettings"));
        }

        private T GetSetting<T>(string name, Crosstables.Classes.ParseSettingType<T> Parse, T defaultValue)
        {
            XmlNode xmlNode = this.XmlNode.SelectSingleNode(name);

            if (xmlNode == null)
                return defaultValue;

            return Parse(xmlNode.InnerXml);
        }

        #endregion
    }

    public enum DashboardExportFormat
    {
        Excel, Pdf
    }
}
