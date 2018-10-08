using DataCore.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LinkBi1.Classes
{
    public abstract class LinkBiDefinitionDimensionScore
    {
        #region Properties

        public bool Hidden
        {
            get
            {
                string result = this.GetValue("Enabled");

                if (result == null)
                    return false;

                return !bool.Parse(result);
            }
            set
            {
                this.SetValue("Enabled", (!value).ToString());
            }
        }

        public bool Persistent
        {
            get
            {
                string result = this.GetValue("Persistent");

                if (result == null)
                    return false;

                return bool.Parse(result);
            }
            set
            {
                this.SetValue("Persistent", (value).ToString());
            }
        }

        public LinkBiDefinitionDimension Owner { get; set; }

        /// <summary>
        /// Gets or sets the xml node that contains
        /// the definition for the filter score.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets the identity of the filter score.
        /// </summary>
        public abstract Guid Identity { get; }

        public abstract string Label { get; }

        public abstract string Name { get; }

        public abstract string Equation { get; set; }

        #endregion


        #region Constructor

        public LinkBiDefinitionDimensionScore(LinkBiDefinitionDimension owner, XmlNode xmlNode)
        {
            this.Owner = owner;
            this.XmlNode = xmlNode;
        }

        #endregion


        #region Methods

        protected string GetValue(string field)
        {
            // Check if the field is present in the xml node's attributes.
            if (this.XmlNode.Attributes[field] == null)
                return null;

            // Return the value of the field stored
            // in the xml node's xml attributes.
            return this.XmlNode.Attributes[field].Value;
        }

        protected void SetValue(string field, string value)
        {
            // Check if the field is present in the xml node's attributes.
            if (this.XmlNode.Attributes[field] == null)
                this.XmlNode.AddAttribute(field, value);

            this.XmlNode.Attributes[field].Value = value;
        }

        public virtual Data GetRespondents(
            Data filter, 
            DataCore.Classes.StorageMethods.Database storageMethod
        )
        {
            return storageMethod.GetRespondents(
                this,
                filter,
                this.Owner.Owner.WeightingFilters
            );
        }

        #endregion
    }
}
