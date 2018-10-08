using Crosstables.Classes;
using Crosstables.Classes.ReportDefinitionClasses;
using LinkBi1.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using WebUtilities.Controls;

namespace Homescreen1.Classes.Modules
{
    public class HomescreenModuleChart : HomescreenModule
    {
        #region Properties

        /// <summary>
        /// Gets or sets the count of items.
        /// </summary>
        public int ItemCount { get; set; }

        #endregion


        #region Constructor

        public HomescreenModuleChart(HomescreenNode node)
            : base(node)
        { }

        #endregion


        #region Methods

        private void RenderNoChartMessage(StringBuilder writer)
        {
            writer.Append(string.Format(
                "<div class=\"HomescreenNoChartDefinedSmiley\">:(</div><div class=\"HomescreenNoChartDefinedText\">{0}</div>",
                this.Node.Owner.LanguageManager.GetText("HomescreenNoChartDefined")
            ));
        }

        public override void Render(StringBuilder writer)
        {
            if (this.Node.XmlNode.Attributes["Source"] == null || File.Exists(this.Node.XmlNode.Attributes["Source"].Value) == false)
            {
                RenderNoChartMessage(writer);
                return;
            }

            try
            {
                Crosstables.Classes.Crosstable crosstable = new Crosstables.Classes.Crosstable(
                    this.Node.Owner.Core,
                    this.Node.XmlNode.Attributes["Source"].Value
                );

                DisplayType displayType = crosstable.ReportDefinition.Settings.DisplayType;

                if (displayType == DisplayType.Crosstable)
                    crosstable.ReportDefinition.Settings.DisplayType = DisplayType.Pie;

                if (this.Node.XmlNode.Attributes["DisplayType"] != null)
                {
                    crosstable.ReportDefinition.Settings.DisplayType = (DisplayType)Enum.Parse(
                        typeof(DisplayType),
                        this.Node.XmlNode.Attributes["DisplayType"].Value
                    );
                }

                crosstable.Render();
                crosstable.Style.Add("HeightScript", this.Node.Height);

                writer.Append(crosstable.ToHtml());

                crosstable.ReportDefinition.Settings.DisplayType = displayType;
                crosstable.ReportDefinition.Save();
            }
            catch (Exception ex)
            {
                RenderNoChartMessage(writer);
            }
        }

        #endregion
    }
}
