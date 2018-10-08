using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace WebUtilities
{
    public enum Language
    {
        English
    }

    public class LanguageManager
    {
        #region Properties

        /// <summary>
        /// Gets or sets the default language.
        /// </summary>
        public Language DefaultLanguage { get; set; }

        public Dictionary<Language, Dictionary<string, string>> Labels { get; set; }

        public Dictionary<Language, Dictionary<string, Dictionary<string, string>>> PageLabels { get; set; }

        #endregion


        #region Constructor

        public LanguageManager(string clientName, string applicationPath)
        {
            this.Labels = new Dictionary<Language, Dictionary<string, string>>();
            this.PageLabels = new Dictionary<Language, Dictionary<string, Dictionary<string, string>>>();

            foreach (Language language in Enum.GetValues(typeof(Language)))
            {
                this.Labels.Add(language, new Dictionary<string, string>());
                this.PageLabels.Add(language, new Dictionary<string, Dictionary<string, string>>());

                string fileName = Path.Combine(
                    applicationPath,
                    "App_Data",
                    "Languages",
                    language.ToString() + ".xml"
                );

                if (!File.Exists(fileName)) continue;

                Parse(language, fileName);

                if (string.IsNullOrEmpty(clientName))
                    continue;

                fileName = Path.Combine(
                    applicationPath,
                    "App_Data",
                    "Languages",
                    clientName
                );

                if (!Directory.Exists(fileName))
                    continue;

                fileName = Path.Combine(
                    applicationPath,
                    "App_Data",
                    "Languages",
                    clientName,
                    language.ToString() + ".xml"
                );

                Parse(language, fileName);
            }
        }

        #endregion


        #region Methods

        private void Parse(Language language, string fileName)
        {

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);

            XmlNodeList xmlNodes = xmlDocument.DocumentElement.SelectNodes("Label");

            foreach (XmlNode xmlNode in xmlNodes)
            {
                if (!this.Labels[language].ContainsKey(xmlNode.Attributes["Name"].Value))
                    this.Labels[language].Add(xmlNode.Attributes["Name"].Value, xmlNode.InnerXml);
                else
                    this.Labels[language][xmlNode.Attributes["Name"].Value] = xmlNode.InnerXml;
            }

            XmlNodeList xmlNodesPages = xmlDocument.DocumentElement.SelectNodes("Page");

            foreach (XmlNode xmlNode in xmlNodesPages)
            {
                string path = xmlNode.Attributes["Path"].Value;

                if (this.PageLabels[language].ContainsKey(path))
                    continue;

                xmlNodes = xmlNode.SelectNodes("Label");

                foreach (XmlNode xmlNodeLabel in xmlNodes)
                {
                    if (!this.PageLabels[language][path].ContainsKey(xmlNodeLabel.Attributes["Name"].Value))
                        this.PageLabels[language][path].Add(xmlNodeLabel.Attributes["Name"].Value, xmlNodeLabel.InnerXml);
                    else
                        this.PageLabels[language][path][xmlNodeLabel.Attributes["Name"].Value] = xmlNodeLabel.InnerXml;
                }
            }
        }


        public string GetText(Language language, string name)
        {
            if (name == null)
                return "";

            if (this.Labels.ContainsKey(language) && this.Labels[language].ContainsKey(name))
                return this.Labels[language][name];
            else
                return name;
        }

        public string GetText(string name)
        {
            return GetText((Language)HttpContext.Current.Session["Language"], name);
        }

        #endregion
    }
}