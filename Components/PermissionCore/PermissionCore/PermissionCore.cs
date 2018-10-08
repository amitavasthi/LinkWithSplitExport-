using PermissionCore.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace PermissionCore
{
    public class PermissionCore
    {
        #region Properties

        public XmlDocument XmlDocument { get; set; }

        public string ProductName { get; set; }

        public PermissionCollection Permissions { get; set; }

        public PagePermissionCollection PagePermissions { get; set; }

        public ButtonPermissionCollection ButtonPermissions { get; set; }

        public ControlPermissionCollection ControlPermissions { get; set; }

        public SectionCollection Sections { get; set; }

        #endregion


        #region Constructor

        public PermissionCore(string applicationPath, string productName, string clientName)
        {
            string fileName = Path.Combine(
                applicationPath,
                "App_Data",
                "Permissions",
                "Global.xml"
            );

            if (!File.Exists(fileName))
            {
                throw new Exception(
                    "Permission file not found."
                );
            }

            Read(fileName, productName);

            fileName = Path.Combine(
                applicationPath,
                "App_Data",
                "Permissions",
                clientName + ".xml"
            );

            if (File.Exists(fileName))
            {
                XmlDocument document = new XmlDocument();
                document.Load(fileName);

                this.Parse(document);
            }
        }

        #endregion


        #region Methods

        private void Read(string fileName, string productName)
        {
            this.ProductName = productName;

            this.PagePermissions = new PagePermissionCollection();
            this.ButtonPermissions = new ButtonPermissionCollection();
            this.ControlPermissions = new ControlPermissionCollection();
            this.Sections = new SectionCollection(this);

            this.XmlDocument = new XmlDocument();
            this.XmlDocument.Load(fileName);

            this.Permissions = new PermissionCollection(this);

            this.Parse(this.XmlDocument);
        }

        private void Parse(XmlDocument document)
        {
            this.Permissions.Parse(document.DocumentElement);

            XmlNode xmlNodeProduct = document.DocumentElement.
                SelectSingleNode("//Product[@Name='" + this.ProductName + "']");

            XmlNode xmlNodePages = document.DocumentElement.SelectSingleNode("Pages");

            if (xmlNodePages != null)
            {
                ParsePagePermissions(xmlNodePages);
            }

            if (xmlNodeProduct != null)
            {
                XmlNode xmlNodeProductPermissions = xmlNodeProduct.SelectSingleNode("Permissions");

                if (xmlNodeProductPermissions != null)
                {
                    this.Permissions.Parse(xmlNodeProductPermissions);
                }

                XmlNode xmlNodeProductPages = xmlNodeProduct.SelectSingleNode("Pages");

                if (xmlNodeProductPages != null)
                {
                    ParsePagePermissions(xmlNodeProductPages);
                }

                XmlNode xmlNodeProductButtons = xmlNodeProduct.SelectSingleNode("Buttons");

                if (xmlNodeProductButtons != null)
                {
                    List<ButtonPermission> buttonPermissions = ParseButtonPermissions(xmlNodeProductButtons);

                    foreach (ButtonPermission buttonPermission in buttonPermissions)
                    {
                        this.ButtonPermissions.Add(buttonPermission);
                    }
                }

                XmlNode xmlNodeProductControls = xmlNodeProduct.SelectSingleNode("Controls");

                if (xmlNodeProductControls != null)
                {
                    List<ControlPermission> ControlPermissions = ParseControlPermissions(xmlNodeProductControls);

                    foreach (ControlPermission controlPermission in ControlPermissions)
                    {
                        this.ControlPermissions.Add(controlPermission);
                    }
                }

                XmlNode xmlNodeSections = xmlNodeProduct.SelectSingleNode("Sections");

                if (xmlNodeSections != null)
                {
                    this.Sections.Parse(xmlNodeSections);
                }
            }
        }


        private void ParsePagePermissions(XmlNode xmlNode)
        {
            XmlNodeList xmlNodesPages = xmlNode.SelectNodes("Page");

            foreach (XmlNode xmlNodePage in xmlNodesPages)
            {
                string pageName = xmlNodePage.Attributes["Name"].Value;
                int idPermission;

                if (int.TryParse(xmlNodePage.Attributes["Permission"].Value, out idPermission))
                {
                    Permission permission = this.Permissions[idPermission];

                    if (permission != null)
                    {
                        PagePermission pagePermission = new PagePermission(this, pageName, permission);

                        XmlNode xmlNodeButtons = xmlNodePage.SelectSingleNode("Buttons");

                        if (xmlNodeButtons != null)
                        {
                            foreach (ButtonPermission buttonPermission in ParseButtonPermissions(xmlNodeButtons))
                            {
                                pagePermission.ButtonPermissions.Add(buttonPermission);
                            }
                        }

                        XmlNode xmlNodeGridColumns = xmlNodePage.SelectSingleNode("GridColumns");

                        if (xmlNodeGridColumns != null)
                        {
                            pagePermission.GridColumnPermissions = new GridColumnPermissionCollection(this, xmlNodeGridColumns);
                        }

                        XmlNode xmlNodeControls = xmlNodePage.SelectSingleNode("Controls");

                        if (xmlNodeControls != null)
                        {
                            foreach (ControlPermission buttonPermission in ParseControlPermissions(xmlNodeControls))
                            {
                                pagePermission.ControlPermissions.Add(buttonPermission);
                            }
                        }

                        this.PagePermissions.Add(pagePermission);
                    }
                }
            }
        }

        private List<ButtonPermission> ParseButtonPermissions(XmlNode xmlNode)
        {
            List<ButtonPermission> result = new List<ButtonPermission>();

            XmlNodeList xmlNodesButtons = xmlNode.SelectNodes("Button");

            foreach (XmlNode xmlNodePage in xmlNodesButtons)
            {
                string idButton = xmlNodePage.Attributes["Id"].Value;
                int idPermission;
                
                if(int.TryParse(xmlNodePage.Attributes["Permission"].Value, out idPermission))
                {
                    Permission permission = this.Permissions[idPermission];

                    if (permission != null)
                    {
                        ButtonPermission buttonPermission = new ButtonPermission(idButton, permission);

                        result.Add(buttonPermission);
                    }
                }
            }

            return result;
        }

        private List<ControlPermission> ParseControlPermissions(XmlNode xmlNode)
        {
            List<ControlPermission> result = new List<ControlPermission>();

            XmlNodeList xmlNodesControls = xmlNode.SelectNodes("Control");

            foreach (XmlNode xmlNodePage in xmlNodesControls)
            {
                string idControl = xmlNodePage.Attributes["Id"].Value;
                int idPermission;

                if (int.TryParse(xmlNodePage.Attributes["Permission"].Value, out idPermission))
                {
                    Permission permission = this.Permissions[idPermission];

                    if (permission != null)
                    {
                        ControlPermission controlPermission = new ControlPermission(idControl, permission);

                        result.Add(controlPermission);
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
