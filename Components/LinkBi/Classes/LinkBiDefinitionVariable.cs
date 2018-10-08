using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LinkBi1.Classes
{
    public class LinkBiDefinitionVariable : LinkBiDefinitionDimension
    {
        #region Properties

        /// <summary>
        /// Gets the id of the taxonomy variable.
        /// </summary>
        public override Guid Identity
        {
            get
            {
                return Guid.Parse(base.GetValue("Id"));
            }
        }

        public override string Label
        {
            get
            {
                return (string)this.Owner.Core.VariableLabels.GetValue(
                    "Label",
                    new string[] { "IdVariable", "IdLanguage" },
                    new object[] { this.Identity, this.Owner.Settings.IdLanguage }
                );
            }
        }

        public override string Name
        {
            get
            {
                return (string)this.Owner.Core.Variables.GetValue(
                    "Name",
                    new string[] { "Id" },
                    new object[] { this.Identity }
                );
            }
        }

        public override bool IsTaxonomy
        {
            get
            {
                return false;
            }
        }

        public override VariableType VariableType
        {
            get
            {
                if (this.XmlNode.Attributes["VariableType"] == null)
                {
                    this.XmlNode.AddAttribute("VariableType", this.Owner.Core.Variables.GetValue(
                        "Type",
                        new string[] { "Id" },
                        new object[] { this.Identity }
                    ));
                }

                return (DatabaseCore.Items.VariableType)Enum.Parse(
                    typeof(DatabaseCore.Items.VariableType),
                    this.XmlNode.Attributes["VariableType"].Value
                );
            }
        }

        #endregion


        #region Constructor

        public LinkBiDefinitionVariable(LinkBiDefinition owner, XmlNode xmlNode)
            : base(owner, xmlNode)
        { }

        #endregion


        #region Methods

        #endregion
    }
}
