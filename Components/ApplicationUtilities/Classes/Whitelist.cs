using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace ApplicationUtilities
{
    public class Whitelist
    {
        #region Properties

        public Dictionary<string, object> Items { get; set; }

        #endregion


        #region Constructor

        public Whitelist()
        {
            this.Items = new Dictionary<string, object>();

            XmlDocument document = new XmlDocument();
            document.Load(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "Whitelist.xml"
            ));

            foreach (XmlNode xmlNode in document.DocumentElement.SelectNodes("Item"))
            {
                this.Items.Add(xmlNode.Attributes["IP"].Value, null);
            }
        }

        public bool Valid(string host)
        {
            if (this.Items.ContainsKey(host))
                return true;

            foreach (string h in this.Items.Keys)
            {
                if (host.StartsWith(h))
                    return true;
            }

            return false;
        }

        #endregion
    }
}