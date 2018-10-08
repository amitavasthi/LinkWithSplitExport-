using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Crosstables.Classes.ReportDefinitionClasses
{
    public class ReportDefinitionInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path to the report info file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the xml document that contains
        /// the information for the user's report.
        /// </summary>
        public XmlDocument XmlDocument { get; set; }


        /// <summary>
        /// Gets or sets the id of the currently selected report.
        /// </summary>
        public Guid? ActiveReport
        {
            get
            {
                string resultStr = this.GetValue("ActiveReport");

                if (resultStr == null)
                    return null;

                if (this.Reports != null)
                {
                    if (!this.Reports.Contains(Path.Combine(
                        Path.GetDirectoryName(this.FileName),
                        resultStr + ".xml"
                    )))
                    {
                        return null;
                    }
                }

                return Guid.Parse(resultStr);
            }
            set
            {
                this.SetValue("ActiveReport", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets if the report
        /// can be overwritten by others.
        /// </summary>
        public bool AllowOverwrite
        {
            get
            {
                string resultStr = this.GetValue("AllowOverwrite");

                if (string.IsNullOrEmpty(resultStr))
                    return true;

                return bool.Parse(resultStr);
            }
            set
            {
                this.SetValue("AllowOverwrite", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the name of the saved report.
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValue("Name");
            }
            set
            {
                this.SetValue("Name", value.ToString());
            }
        }
        public string IsPublic
        {
            get
            {
                return this.GetValue("Private");
            }
            set
            {
                this.SetValue("Private", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the when the report was accessed.
        /// </summary>
        public DateTime LatestAccess
        {
            get
            {
                string resultStr = this.GetValue("LatestAccess");

                if (resultStr == null)
                    return new DateTime();

                DateTime result;

                if (DateTime.TryParse(resultStr, out result))
                    return result;

                return new DateTime();
            }
            set
            {
                this.SetValue("LatestAccess", value.ToString());
            }
        }

        public List<string> Reports { get; set; }

        #endregion


        #region Constructor

        public ReportDefinitionInfo(string fileName)
        {
            this.FileName = fileName;

            if (!File.Exists(fileName))
            {
                File.WriteAllText(
                    fileName,
                    "<Info></Info>"
                );
            }

            this.XmlDocument = new XmlDocument();
            this.XmlDocument.Load(fileName);

            this.Reports = new List<string>();
        }

        #endregion


        #region Methods

        public List<string> GetReports(DatabaseCore.Core core, Guid idUser)
        {
            List<string> reports = new List<string>();

            string[] files = Directory.GetFiles(Path.GetDirectoryName(this.FileName)).
                OrderBy(x => (new FileInfo(x)).CreationTime).ToArray();

            foreach (string report in files)
            {
                if (report.EndsWith("Info.xml"))
                    continue;

                try
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(report);

                    XmlNodeList xmlNodesHierarchies = xmlDocument.DocumentElement.SelectNodes("HierarchyFilter/Hierarchy");
                    bool hasHierarchyPermission = true;

                    foreach (XmlNode xmlNodeHierarchy in xmlNodesHierarchies)
                    {
                        Guid idHierarchy = Guid.Parse(xmlNodeHierarchy.Attributes["Id"].Value);

                        if (!this.HasHierarchyPermission(core, idHierarchy, idUser))
                        {
                            hasHierarchyPermission = false;
                            break;
                        }
                    }

                    if (hasHierarchyPermission)
                        reports.Add(report);
                }
                catch
                {
                    reports.Add(report);
                }
            }

            this.Reports = reports;

            return reports;
        }


        public void Save()
        {
            this.XmlDocument.Save(this.FileName);
        }


        private string GetValue(string name)
        {
            XmlNode xmlNode = this.XmlDocument.DocumentElement.SelectSingleNode(name);

            if (xmlNode == null)
                return null;

            return xmlNode.InnerXml;
        }

        private void SetValue(string name, object value)
        {
            XmlNode xmlNode = this.XmlDocument.DocumentElement.SelectSingleNode(name);

            if (xmlNode == null)
            {
                this.XmlDocument.DocumentElement.InnerXml += string.Format(
                    "<{0}>{1}</{0}>",
                    name,
                    value
                );
            }
            else
            {
                xmlNode.InnerXml = value.ToString();
            }
        }


        public bool HasHierarchyPermission(DatabaseCore.Core core, Guid idHierarchy, Guid idUser)
        {
            return (int)core.WorkgroupHierarchies.ExecuteReader(
                "SELECT Count(*) FROM WorkgroupHierarchies WHERE IdHierarchy={0} AND IdWorkgroup IN (SELECT IdWorkgroup FROM UserWorkgroups WHERE IdUser={1})",
                new object[] { idHierarchy, idUser }
            )[0][0] != 0;
        }

        #endregion
    }
}
