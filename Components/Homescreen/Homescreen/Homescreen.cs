using Homescreen1.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Xml;

namespace Homescreen1
{
    public class Homescreen : WebUtilities.BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path to
        /// the homescreen definition file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets a list of homescreen nodes on root level.
        /// </summary>
        public List<HomescreenNode> Nodes { get; set; }

        /// <summary>
        /// Gets or sets the available height for the homescreen.
        /// </summary>
        public string ContentHeight { get; set; }

        /// <summary>
        /// Gets or sets the available width for the homescreen.
        /// </summary>
        public string ContentWidth { get; set; }

        /// <summary>
        /// Gets or sets the available height for the homescreen.
        /// </summary>
        public int BaseContentHeight { get; set; }

        /// <summary>
        /// Gets or sets the available width for the homescreen.
        /// </summary>
        public int BaseContentWidth { get; set; }

        #endregion


        #region Constructor

        public Homescreen(string fileName)
        {
            this.FileName = fileName;

            this.Load += Homescreen_Load;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Parses the homescreen definitions from
        /// the provided homescreen definition file.
        /// </summary>
        public void Parse()
        {
            // DEPLOY FIX;
            this.ContentWidth = "ContentWidth - 50";
            this.ContentHeight = "ContentHeight - 55";

            // Create a new list that stores the
            // homescreen nodes on root level.
            this.Nodes = new List<HomescreenNode>();

            // Create a new xml document that
            // contains the homescreen's definitions.
            XmlDocument document = new XmlDocument();

            // Load the contents of the homescreen
            // definition file into the xml document.
            document.Load(this.FileName);

            // Run through all child nodes of the
            // homescreen definition's document node.
            foreach (XmlNode xmlNode in document.DocumentElement.ChildNodes)
            {
                // Create a new homescreen node by the xml node.
                HomescreenNode node = new HomescreenNode(xmlNode, this);

                // Set the dimensions for the homescreen node.
                //node.Height = this.ContentHeight;
                node.Height = this.ContentHeight;
                node.Width = this.ContentWidth;

                // Add the homescreen node to the homescreens root nodes.
                this.Nodes.Add(node);
            }
        }

        public void Render()
        {
            // DEPLOY FIX;
            Parse();

            // Create a new string builder as html writer.
            StringBuilder writer = new StringBuilder();

            // Run through all homescreen nodes on root level.
            foreach (HomescreenNode node in this.Nodes)
            {
                // Render the homescreen node.
                node.Render(writer);
            }

            this.Controls.Add(new LiteralControl(writer.ToString()));
        }

        #endregion


        #region Event Handlers

        protected void Homescreen_Load(object sender, EventArgs e)
        {
            this.Render();
        }

        #endregion
    }
}
