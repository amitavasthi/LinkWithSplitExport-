using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homescreen1.Classes.Renderers
{
    public class HomescreenRendererSection : HomescreenRenderer
    {
        #region Properties

        /// <summary>
        /// Gets the amount of sections in the line.
        /// </summary>
        public int SectionCount
        {
            get
            {
                if (this.Node.XmlNode.Attributes["SectionCount"] == null)
                    return 1;

                return int.Parse(this.Node.XmlNode.Attributes["SectionCount"].Value);
            }
        }

        /// <summary>
        /// Gets where the section is docked.
        /// </summary>
        public HomescreenSectionDock Dock
        {
            get
            {
                if (this.Node.XmlNode.Attributes["Dock"] == null)
                    return HomescreenSectionDock.Left;

                return (HomescreenSectionDock)Enum.Parse(
                    typeof(HomescreenSectionDock),
                    this.Node.XmlNode.Attributes["Dock"].Value
                );
            }
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the section renderer.
        /// </summary>
        /// <param name="node">The node to render.</param>
        public HomescreenRendererSection(HomescreenNode node)
            : base(node)
        { }

        #endregion


        #region Methods

        public string CalculateHeight()
        {
            string script = this.Node.Height;

            if (this.Node.XmlNode.Attributes["Height"] != null)
            {
                script = (this.Node.XmlNode.Attributes["Height"].Value);
            }
            else if (this.Node.XmlNode.Attributes["SectionCountH"] != null)
            {
                script = "(100 / " + int.Parse(this.Node.XmlNode.Attributes["SectionCountH"].Value) + ") * (ContentHeight) / 100";
            }
            else if (this.Dock == HomescreenSectionDock.Top || this.Dock == HomescreenSectionDock.Bottom)
            {
                script = this.Node.Owner.ContentHeight.ToString();

                List<HomescreenNode> neighbors;

                if (this.Node.Parent != null)
                    neighbors = this.Node.Parent.ChildNodes;
                else
                    neighbors = this.Node.Owner.Nodes;

                foreach (HomescreenNode neighbor in neighbors)
                {
                    if (neighbor.XmlNode.Attributes["Height"] != null)
                        script += " - " + (neighbor.XmlNode.Attributes["Height"].Value);
                }
            }

            if (this.Node.XmlNode.Attributes["Title"] != null)
            {
                script += " - 45";
            }

            return script;

            /*this.Height = this.Node.Height;

            if (this.Node.XmlNode.Attributes["Height"] != null)
            {
                this.Height = int.Parse(this.Node.XmlNode.Attributes["Height"].Value);

            }
            else if (this.Node.XmlNode.Attributes["SectionCountH"] != null)
            {
                this.Height = (100 / int.Parse(this.Node.XmlNode.Attributes["SectionCountH"].Value)) * this.Node.Owner.ContentHeight / 100;
            }
            else if (this.Dock == HomescreenSectionDock.Top || this.Dock == HomescreenSectionDock.Bottom)
            {
                this.Height = this.Node.Owner.ContentHeight;

                List<HomescreenNode> neighbors;

                if (this.Node.Parent != null)
                    neighbors = this.Node.Parent.ChildNodes;
                else
                    neighbors = this.Node.Owner.Nodes;

                foreach (HomescreenNode neighbor in neighbors)
                {
                    if (neighbor.XmlNode.Attributes["Height"] != null)
                        this.Height -= int.Parse(neighbor.XmlNode.Attributes["Height"].Value);
                }
            }

            if (this.Node.XmlNode.Attributes["Title"] != null)
            {
                this.Height -= 45;
            }

            return this.Height;*/
        }

        public string CalculateWidth()
        {
            string script = this.Node.Width;

            if (this.Node.XmlNode.Attributes["Width"] != null)
            {
                script = this.Node.XmlNode.Attributes["Width"].Value;

            }
            else if (this.Node.XmlNode.Attributes["SectionCount"] != null)
            {
                script = "(100 / "+ int.Parse(this.Node.XmlNode.Attributes["SectionCount"].Value)+ ") * ("+ this.Node.Owner.ContentWidth +") / 100";
            }
            else
            {
                script = this.Node.Width;

                List<HomescreenNode> neighbors;

                if (this.Node.Parent != null)
                    neighbors = this.Node.Parent.ChildNodes;
                else
                    neighbors = this.Node.Owner.Nodes;

                foreach (HomescreenNode neighbor in neighbors)
                {
                    if (neighbor.XmlNode.Attributes["Width"] != null)
                        script += " - " + (neighbor.XmlNode.Attributes["Width"].Value);
                }
            }

            return script;

            /*
            this.Width = this.Node.Width;

            if (this.Node.XmlNode.Attributes["Width"] != null)
            {
                this.Width = int.Parse(this.Node.XmlNode.Attributes["Width"].Value);

            }
            else if (this.Node.XmlNode.Attributes["SectionCount"] != null)
            {
                this.Width = (100 / int.Parse(this.Node.XmlNode.Attributes["SectionCount"].Value)) * this.Node.Owner.ContentWidth / 100;
            }
            else
            {
                this.Width = this.Node.Owner.ContentWidth;

                List<HomescreenNode> neighbors;

                if (this.Node.Parent != null)
                    neighbors = this.Node.Parent.ChildNodes;
                else
                    neighbors = this.Node.Owner.Nodes;

                foreach (HomescreenNode neighbor in neighbors)
                {
                    if (neighbor.XmlNode.Attributes["Width"] != null)
                        this.Width -= int.Parse(neighbor.XmlNode.Attributes["Width"].Value);
                }
            }

            return this.Width;*/
        }

        public override void RenderBegin(StringBuilder writer)
        {
            string height = this.Node.Height;

            if (this.Node.XmlNode.Attributes["Title"] != null)
                height += " + 45";

            // Open the section's div tag.
            /*writer.Append(string.Format(
                "<div class=\"Section Section{0}\" style=\"width:{1}px;{2}\">",
                this.Node.Name,
                this.Width,
                "height:" + height + "px;"
            ));*/
            if (!this.Node.HasModule)
            {
                writer.Append(string.Format(
                    "<div class=\"Section Section{0}\" HeightScript=\"{1}\" WidthScript=\"{2}\">",
                    this.Node.Name,
                    height,
                    this.Node.Width
                ));
            }
            else
            {
                writer.Append(string.Format(
                    "<div class=\"Section Section{0}\">",
                    this.Node.Name
                ));
            }

            if (this.Node.XmlNode.Attributes["Title"] != null)
            {
                writer.Append(string.Format(
                    "<h1 class=\"Color1\">{0}</h1>",
                    this.Node.Owner.LanguageManager.GetText(this.Node.XmlNode.Attributes["Title"].Value)
                ));
            }
        }

        public override void RenderEnd(StringBuilder writer)
        {
            // Close the section's div tag.
            writer.Append("</div>");
        }

        #endregion
    }

    public enum HomescreenSectionDock
    {
        Left,
        Middle,
        Right,
        Top,
        Bottom
    }
}
