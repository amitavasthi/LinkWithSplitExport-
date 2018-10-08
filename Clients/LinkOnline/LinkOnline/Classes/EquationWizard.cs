using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace LinkOnline.Classes
{
    public class EquationWizard
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path to the
        /// equation wizard's definition file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the xml document that
        /// contains the equation wizard's definition.
        /// </summary>
        public XmlDocument Document { get; set; }

        /// <summary>
        /// Gets or sets the equation wizard's properties.
        /// </summary>
        public EquationWizardProperties Properties { get; set; }

        /// <summary>
        /// Gets or sets the equation wizard's
        /// place holders by their unique name.
        /// </summary>
        public Dictionary<string, EquationWizardPlaceHolder> PlaceHolders { get; set; }

        #endregion


        #region Constructor

        public EquationWizard(string fileName)
        {
            this.FileName = fileName;

            this.Document = new XmlDocument();
            this.Document.Load(this.FileName);

            this.Parse();
        }

        #endregion


        #region Methods

        /// <summary>
        /// Parses the equation wizard's definition file.
        /// </summary>
        private void Parse()
        {
            this.Properties = new EquationWizardProperties(this.Document);
        }

        public void ParseFull()
        {
            EquationWizardPlaceHolder placeHolder;

            // Create a new dictionary to store the equation
            // wizard's place holders by their unique name.
            this.PlaceHolders = new Dictionary<string, EquationWizardPlaceHolder>();

            // Select all xml nodes that define
            // an equation wizard place holder.
            XmlNodeList xmlNodes = this.Document.SelectNodes("//PlaceHolder");

            // Run through all xml nodes that 
            // define an equation wizard place holder.
            foreach (XmlNode xmlNode in xmlNodes)
            {
                placeHolder = new EquationWizardPlaceHolder(xmlNode);

                this.PlaceHolders.Add(placeHolder.Name, placeHolder);
            }
        }

        #endregion
    }

    public class EquationWizardPlaceHolder
    {
        #region Properties

        /// <summary>
        /// Gets or sets the xml node that defines
        /// the equation wizard place holder.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets the unique name of the place holder.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the equation
        /// wizard place holder's type.
        /// </summary>
        public EquationWizardPlaceHolderType Type { get; set; }

        /// <summary>
        /// Gets or sets the equation wizard
        /// place holder's repeat type.
        /// </summary>
        public EquationWizardPlaceHolderRepeat Repeat { get; set; }

        #endregion


        #region Constructor

        public EquationWizardPlaceHolder(XmlNode xmlNode)
        {
            this.XmlNode = xmlNode;

            this.Name = xmlNode.Attributes["Name"].Value;

            this.Type = (EquationWizardPlaceHolderType)Enum.Parse(
                typeof(EquationWizardPlaceHolderType),
                this.XmlNode.Attributes["Type"].Value
            );

            this.Repeat = (EquationWizardPlaceHolderRepeat)Enum.Parse(
                typeof(EquationWizardPlaceHolderRepeat),
                this.XmlNode.Attributes["Repeat"].Value
            );
        }

        #endregion


        #region Methods

        #endregion
    }

    public enum EquationWizardPlaceHolderType
    {
        Variable
    }
    public enum EquationWizardPlaceHolderRepeat
    {
        None,
        Category
    }

    public struct EquationWizardProperties
    {
        public string Section;
        public string Name;
        public string Description;

        public EquationWizardProperties(XmlDocument document)
        {
            this.Section = document.SelectSingleNode(
                "EquationWizard/Properties/Section").InnerXml;
            this.Name = document.SelectSingleNode(
                "EquationWizard/Properties/Name").InnerXml;
            this.Description = document.SelectSingleNode(
                "EquationWizard/Properties/Description").InnerXml;
        }
    }
}