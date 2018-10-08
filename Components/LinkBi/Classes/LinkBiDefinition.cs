using Crosstables.Classes;
using Crosstables.Classes.HierarchyClasses;
using Crosstables.Classes.ReportDefinitionClasses;
using Crosstables.Classes.ReportDefinitionClasses.Collections;
using Crosstables.Classes.WorkflowClasses;
using DatabaseCore.Items;
using LinkBi1.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace LinkBi1.Classes
{
    public class LinkBiDefinition : BaseReportDefinition
    {
        #region Properties

        /// <summary>
        /// Gets or sets a list of dimensions.
        /// </summary>
        public List<LinkBiDefinitionDimension> Dimensions { get; set; }

        /// <summary>
        /// Gets or sets a list of measures.
        /// </summary>
        public List<LinkBiDefinitionDimension> Measures { get; set; }

        public LinkBiDefinitionProperties Properties { get; set; }

        public LinkBiSettings Settings
        {
            get
            {
                return (LinkBiSettings)base.Settings;
            }
            set
            {
                base.Settings = value;
            }
        }

        #endregion


        #region Constructor

        public LinkBiDefinition(DatabaseCore.Core core, string fileName, HierarchyFilter hierarchyFilter)
            : base(core, fileName, hierarchyFilter)
        { }

        public LinkBiDefinition(DatabaseCore.Core core, XmlDocument xmlDocument, HierarchyFilter hierarchyFilter)
            : base(core, xmlDocument, hierarchyFilter)
        { }

        #endregion


        #region Methods

        public override void Parse()
        {
            this.Dimensions = new List<LinkBiDefinitionDimension>();
            this.Measures = new List<LinkBiDefinitionDimension>();

            // Get all filter xml nodes.
            XmlNodeList xmlNodesDimensions = this.XmlDocument.DocumentElement.SelectNodes("Dimensions/*");

            // Run through all filter xml nodes.
            foreach (XmlNode xmlNodeDimension in xmlNodesDimensions)
            {
                LinkBiDefinitionDimension filter = null;

                switch (xmlNodeDimension.Name)
                {
                    case "Variable":
                        if (xmlNodeDimension.Attributes["IsTaxonomy"] != null && bool.Parse(xmlNodeDimension.Attributes["IsTaxonomy"].Value) == false)
                        {
                            // Create a new variable as filter by the xml node.
                            filter = new LinkBiDefinitionVariable(this, xmlNodeDimension);
                        }
                        else
                        {
                            // Create a new taxonomy variable as filter by the xml node.
                            filter = new LinkBiDefinitionTaxonomyVariable(this, xmlNodeDimension);
                        }

                        break;
                }

                // Check if it was possible to parse the filter.
                if (filter == null)
                    continue;

                // Add the filter to the LinkBi definition's filters.
                this.Dimensions.Add(filter);
            }

            // Get all measure xml nodes.
            XmlNodeList xmlNodesMeasures = this.XmlDocument.DocumentElement.SelectNodes("Measures/*");

            // Run through all measure xml nodes.
            foreach (XmlNode xmlNodeMeasure in xmlNodesMeasures)
            {
                LinkBiDefinitionDimension measures = null;

                switch (xmlNodeMeasure.Name)
                {
                    case "Variable":
                        if (xmlNodeMeasure.Attributes["IsTaxonomy"] != null && bool.Parse(xmlNodeMeasure.Attributes["IsTaxonomy"].Value) == false)
                        {
                            // Create a new taxonomy variable as filter by the xml node.
                            measures = new LinkBiDefinitionVariable(this, xmlNodeMeasure);
                        }
                        else
                        {
                            // Create a new taxonomy variable as filter by the xml node.
                            measures = new LinkBiDefinitionTaxonomyVariable(this, xmlNodeMeasure);
                        }
                        break;
                }

                // Check if it was possible to parse the filter.
                if (measures == null)
                    continue;

                // Add the filter to the LinkBi definition's filters.
                this.Measures.Add(measures);
            }

            XmlNode xmlNodeProperties = this.XmlDocument.DocumentElement.SelectSingleNode("Properties");

            this.Properties = new LinkBiDefinitionProperties(this, xmlNodeProperties);

            this.Settings = new LinkBiSettings(this, this.XmlDocument.DocumentElement.SelectSingleNode("Settings"));
        }

        public override void Save()
        {
            XmlNode xmlNodeSettings = this.XmlDocument.DocumentElement.SelectSingleNode("Settings");

            foreach (KeyValuePair<string, object> setting in this.Settings.Values)
            {
                XmlNode xmlNodeSetting = xmlNodeSettings.SelectSingleNode(string.Format(
                    "Setting[@Name=\"{0}\"]",
                    setting.Key
                ));

                if (xmlNodeSetting == null)
                {
                    xmlNodeSettings.InnerXml += string.Format(
                        "<Setting Name=\"{0}\">{1}</Setting>",
                        setting.Key,
                        setting.Value
                    );
                }
                else
                {
                    xmlNodeSetting.InnerText = setting.Value.ToString();
                }
            }

            if (this.FileName != null)
                this.XmlDocument.Save(this.FileName);
        }

        public override void ClearData()
        { }


        public void RemoveDimension(Guid idVariable)
        {
            XmlNode xmlNode = this.XmlDocument.DocumentElement.SelectSingleNode(string.Format(
                "Dimensions/*[@Id=\"{0}\"]",
                idVariable
            ));

            if (xmlNode == null)
                return;

            xmlNode.ParentNode.RemoveChild(xmlNode);

            this.Save();
        }


        public void RemoveMeasure(Guid idVariable)
        {
            XmlNode xmlNode = this.XmlDocument.DocumentElement.SelectSingleNode(string.Format(
                "Measures/*[@Id=\"{0}\"]",
                idVariable
            ));

            if (xmlNode == null)
                return;

            xmlNode.ParentNode.RemoveChild(xmlNode);

            this.Save();
        }


        public bool Deploy(Guid idServerConnection)
        {
            bool result = true;

            string export = null;

            if (!this.Properties.ServerConnections.ContainsKey(idServerConnection))
                return false;

            LinkBiServerConnection serverConnection = this.Properties.ServerConnections[idServerConnection];

            LinkBiInterface exportInterface = null;

            switch (serverConnection.InterfaceType)
            {
                case LinkBiInterfaceType.PowerBI:
                    exportInterface = new Excel(this.Core, this);
                    break;

                case LinkBiInterfaceType.CustomCharts:
                    exportInterface = new CustomCharts(this.Core, this);
                    break;
            }

            if (exportInterface != null)
            {
                // Create the export for the server connection's provider.
                export = exportInterface.Render();

                // Deploy the file to the server.
                if (serverConnection.Deploy(
                    export
                ) == false)
                    result = false;
            }
            else
            {
                return false;
            }

            return result;
        }

        public bool Deploy()
        {
            bool result = true;

            Dictionary<LinkBiInterfaceType, string> exports = new Dictionary<LinkBiInterfaceType, string>();

            // Run through all server connections.
            foreach (LinkBiServerConnection serverConnection in this.Properties.ServerConnections.Values)
            {
                // Check if an export with the server connection's interface type was already created.
                if (exports.ContainsKey(serverConnection.InterfaceType))
                    continue;

                LinkBiInterface exportInterface = null;

                switch (serverConnection.InterfaceType)
                {
                    case LinkBiInterfaceType.PowerBI:
                        exportInterface = new Excel(this.Core, this);
                        break;

                    case LinkBiInterfaceType.CustomCharts:
                        exportInterface = new CustomCharts(this.Core, this);
                        break;
                }

                if (exportInterface == null)
                    continue;

                // Create the export for the server connection's provider.
                string fileName = exportInterface.Render();

                exports.Add(serverConnection.InterfaceType, fileName);
            }

            // Run through all server connections.
            foreach (LinkBiServerConnection serverConnection in this.Properties.ServerConnections.Values)
            {
                // Deploy the file to the server.
                if (serverConnection.Deploy(
                    exports[serverConnection.InterfaceType]
                ) == false)
                    result = false;
            }

            return result;
        }


        public bool IsUpToDate()
        {
            DateTime latestUpdate = this.Properties.LatestUpdate;

            // Run through all measures of the LinkBi definition.
            foreach (LinkBiDefinitionDimension measure in this.Measures)
            {
                DateTime measureUpdated = GetLatestUpdate(measure);

                if (measureUpdated > latestUpdate)
                    latestUpdate = measureUpdated;
            }

            // Run through all filters of the LinkBi definition.
            foreach (LinkBiDefinitionDimension filter in this.Dimensions)
            {
                DateTime filterUpdated = GetLatestUpdate(filter);

                if (filterUpdated > latestUpdate)
                    latestUpdate = filterUpdated;
            }

            bool result = true;

            foreach (LinkBiServerConnection serverConnection in this.Properties.ServerConnections.Values)
            {
                DateTime latestDeploy = serverConnection.LatestDeploy;

                if (latestDeploy < latestUpdate)
                {
                    serverConnection.Outdated = true;

                    result = false;
                }
                else
                {
                    serverConnection.Outdated = false;
                }
            }

            this.Save();

            return result;
        }

        private DateTime GetLatestUpdate(LinkBiDefinitionDimension variable)
        {
            DateTime result = new DateTime();

            // Run through all scores of the filter.
            foreach (LinkBiDefinitionDimensionScore score in variable.Scores)
            {
                // Get all category linkings of the score.
                List<object[]> links = this.Core.CategoryLinks.GetValues(
                    new string[] { "CreationDate" },
                    new string[] { "IdTaxonomyCategory" },
                    new object[] { score.Identity }
                );

                // Run through all category links.
                foreach (object[] link in links)
                {
                    if (((DateTime)link[0]) > result)
                        result = (DateTime)link[0];
                }
            }

            return result;
        }

        #endregion
    }
}
