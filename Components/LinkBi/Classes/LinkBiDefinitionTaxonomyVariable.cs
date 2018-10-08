﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LinkBi1.Classes
{
    public class LinkBiDefinitionTaxonomyVariable : LinkBiDefinitionDimension
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
                return (string)this.Owner.Core.TaxonomyVariableLabels.GetValue(
                    "Label",
                    new string[] { "IdTaxonomyVariable", "IdLanguage" },
                    new object[] { this.Identity, this.Owner.Settings.IdLanguage }
                );
            }
        }

        public override string Name
        {
            get
            {
                return (string)this.Owner.Core.TaxonomyVariables.GetValue(
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
                return true;
            }
        }

        public override DatabaseCore.Items.VariableType VariableType
        {
            get
            {
                if(this.XmlNode.Attributes["VariableType"] == null)
                {
                    object type = this.Owner.Core.TaxonomyVariables.GetValue(
                        "Type",
                        new string[] { "Id" },
                        new object[] { this.Identity }
                    );

                    DatabaseCore.Items.VariableType variableType = DatabaseCore.Items.VariableType.Single;

                    if (type != null)
                        variableType = (DatabaseCore.Items.VariableType)type;

                    this.XmlNode.AddAttribute("VariableType", variableType);
                }

                return (DatabaseCore.Items.VariableType)Enum.Parse(
                    typeof(DatabaseCore.Items.VariableType),
                    this.XmlNode.Attributes["VariableType"].Value
                );
            }
        }

        #endregion


        #region Constructor

        public LinkBiDefinitionTaxonomyVariable(LinkBiDefinition owner, XmlNode xmlNode)
            : base(owner, xmlNode)
        { }

        #endregion


        #region Methods

        #endregion
    }
}
