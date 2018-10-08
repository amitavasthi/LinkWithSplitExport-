using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace LinkAdminService.Classes
{
    public class PortalDirectory
    {
        #region Properties

        public string Path { get; set; }

        public List<PortalDirectoryFormatType> FormatTypes { get; set; }


        #endregion


        #region Constructor

        public PortalDirectory(XmlNode xmlNode)
        {
            this.Path = xmlNode.Attributes["Path"].Value;
            this.FormatTypes = new List<PortalDirectoryFormatType>();

            int i = 0;
            while (true)
            {
                if (xmlNode.Attributes["FormatValue" + i] == null)
                    break;

                this.FormatTypes.Add((PortalDirectoryFormatType)Enum.Parse(
                    typeof(PortalDirectoryFormatType),
                    xmlNode.Attributes["FormatValue" + i].Value
                ));
            }
        }

        #endregion


        #region Methods

        #endregion
    }

    public enum PortalDirectoryFormatType
    {
        Client,
        User
    }
}