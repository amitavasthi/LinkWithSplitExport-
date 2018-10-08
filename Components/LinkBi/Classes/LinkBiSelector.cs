using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WebUtilities.Controls;

namespace LinkBi1.Classes
{
    public class LinkBiSelector : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id of the selected variable.
        /// </summary>
        public LinkBiDefinitionDimension SelectedItem { get; set; }

        /// <summary>
        /// Gets or sets if the selected item is a taxonomy variable.
        /// </summary>
        public bool SelectedItemIsTaxonomy { get; set; }

        /// <summary>
        /// Gets or sets the name of the method that should
        /// be invoked when deleting the item.
        /// </summary>
        public string DeleteMethod { get; set; }

        public int IdLanguage { get; set; }

        public event EventHandler OnSelectedItemChanged;

        #endregion


        #region Constructor

        public LinkBiSelector(int idLanguage, LinkBiDefinitionDimension selectedItem, bool isTaxonomy)
        {
            this.IdLanguage = idLanguage;
            this.SelectedItem = selectedItem;
            this.SelectedItemIsTaxonomy = isTaxonomy;

            this.Load += LinkBiSelector_Load;
        }

        #endregion


        #region Methods

        public void Render()
        {
            Panel pnlContainer = new Panel();
            pnlContainer.ID = "pnlContainer" + this.ID;
            pnlContainer.CssClass = "LinkBiSelector BackgroundColor1";

            System.Web.UI.WebControls.Label lblSelectedItem = new System.Web.UI.WebControls.Label();
            lblSelectedItem.ID = "lblSelectedItem" + this.ID;

            string name;

            if (this.SelectedItemIsTaxonomy)
            {
                name = (string)base.Core.TaxonomyVariables.GetValue(
                    "Name",
                    "Id",
                    this.SelectedItem.Identity
                );
            }
            else
            {
                name = (string)base.Core.Variables.GetValue(
                    "Name",
                    "Id",
                    this.SelectedItem.Identity
                );
            }

            lblSelectedItem.Text = name;

            pnlContainer.Controls.Add(lblSelectedItem);

            /*pnlContainer.Attributes.Add(
                "onmouseover",
                "ShowToolTip(this, '" + GenerateToolTip() + "', true)"
            );*/

            base.Controls.Add(pnlContainer);

            pnlContainer.Attributes.Add(
                "oncontextmenu",
                "this.style.background='url(/Images/Icons/Cloud/Delete.png) no-repeat 50% 50% #FF0000';" +
                "this.style.color='transparent';this.style.cursor = 'pointer';" + this.DeleteMethod + "(this, '" +
                this.SelectedItem.Identity + "'); return false;"
            );

            pnlContainer.Attributes.Add(
                "onmouseout",
                "this.style.background='';this.style.cursor = '';this.style.color='';this.onclick=undefined;"
            );

            /*this.Attributes.Add("onclick", string.Format(
                "EditLinkBiScores(document.getElementById('{0}'), '{1}')",
                pnlContainer.ClientID,
                this.SelectedItem.XmlNode.GetXPath(false)
            ));*/
            this.Attributes.Add("onclick", string.Format(
                "ShowScores(this, '{0}', '{1}')",
                this.SelectedItem.Owner.FileName.Replace("\\","/"),
                this.SelectedItem.XmlNode.GetXPath(false)
            ));
        }

        private string GenerateToolTip()
        {
            StringBuilder result = new StringBuilder();
            result.Append("<div class=\"LinkBiSelectorToolTipImage\"><img src=\"/Images/Icons/LinkBiSelectorToolTip.png\" /></div>");
            result.Append("<div class=\"LinkBiSelectorToolTip BackgroundColor9\">");

            List<string> labels = new List<string>();

            if (this.SelectedItemIsTaxonomy)
            {
                List<object[]> categories = base.Core.TaxonomyCategories.GetValues(
                    new string[] { "Id" },
                    new string[] { "IdTaxonomyVariable" },
                    new object[] { this.SelectedItem.Identity },
                    "Order"
                );

                foreach (object[] category in categories)
                {
                    labels.Add(
                        (string)base.Core.TaxonomyCategoryLabels.GetValue(
                            "Label",
                            new string[] { "IdTaxonomyCategory", "IdLanguage" },
                            new object[] { (Guid)category[0], this.IdLanguage }
                        )
                    );
                }
            }
            else
            {
                List<object[]> categories = base.Core.Categories.GetValues(
                    new string[] { "Id" },
                    new string[] { "IdVariable" },
                    new object[] { this.SelectedItem.Identity },
                    "Order"
                );

                foreach (object[] category in categories)
                {
                    labels.Add(
                        (string)base.Core.CategoryLabels.GetValue(
                            "Label",
                            new string[] { "IdCategory", "IdLanguage" },
                            new object[] { (Guid)category[0], this.IdLanguage }
                        )
                    );
                }
            }

            foreach (string label in labels)
            {
                result.Append(string.Format(
                    "<div class=\"LinkBiSelectorToolTipItem Color1\">{0}</div>",
                    label
                ));
            }

            result.Append("</div>");

            return result.ToString();
        }

        #endregion


        #region Event Handlers

        protected void LinkBiSelector_Load(object sender, EventArgs e)
        {
            Render();
        }

        #endregion
    }
}
