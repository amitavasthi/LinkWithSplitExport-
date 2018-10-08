using ApplicationUtilities.Classes;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.HtmlControls;

namespace MasterPage.Classes
{
    public class ColorSchemeStylesheet : HtmlGenericControl
    {
        #region Properties

        /// <summary>
        /// Gets the current session's database core.
        /// </summary>
        public DatabaseCore.Core Core
        {
            get
            {
                return (DatabaseCore.Core)HttpContext.Current.Session["Core"];
            }
        }

        #endregion


        #region Constructor

        public ColorSchemeStylesheet()
            : base("style")
        {
            this.Attributes.Add("type", "text/css");

            this.Load += ColorSchemeStylesheet_Load;
        }

        #endregion


        #region Methods

        public void Render()
        {
            // Create a new client manager.
            ClientManager clientManager = new ClientManager();

            if (!File.Exists(clientManager.FileName))
            {
                clientManager.FileName = ConfigurationManager.AppSettings["ClientDetailsRootPath"];
            }

            // Get the user's client.
            Client client = clientManager.GetSingle(this.Core.ClientName);

            string mainColor;
            string secondaryColor;
            string color5;
            string color7;
            string color9;
            string color20;
            string color21;

            ColorCalculator colorCalculator = new ColorCalculator();

            // Check if the client is null.
            if (client != null)
            {
                // Get the client's main color.
                mainColor = client.Color1;

                // Get the client's secondary color.
                secondaryColor = client.Color2;
            }
            else
            {
                mainColor = "#6CAEE0";
                secondaryColor = "#FCB040";
            }

            color5 = colorCalculator.AdjustBrightness(new HexColor(mainColor), 1.4074074, 1.1379310, 1.04017857).ToString();
            color7 = colorCalculator.AdjustBrightness(new HexColor(mainColor), 2.09259259, 1.37356321, 1.1116071).ToString();
            color9 = colorCalculator.AdjustBrightness(new HexColor(mainColor), 1.69444444, 1.24137931, 1.0714285).ToString();

            color20 = colorCalculator.AdjustBrightness(new HexColor(secondaryColor), 1.15).ToString();
            color21 = colorCalculator.AdjustBrightness(new HexColor(secondaryColor), 1.2).ToString();

            // Get the contents of the color scheme template css file.
            string css = File.ReadAllText(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "ColorScheme.css"
            ));

            // Replace the main color.
            css = css.Replace("###MAINCOLOR###", mainColor);

            // Replace the secondary color.
            css = css.Replace("###SECONDARYCOLOR###", secondaryColor);

            // Replace the secondary color.
            css = css.Replace("###SECONDARYCOLOR2###", color20);

            // Replace the secondary color.
            css = css.Replace("###SECONDARYCOLOR3###", color21);

            // Replace the fifth color.
            css = css.Replace("###COLOR5###", color5);

            // Replace the seventh color.
            css = css.Replace("###COLOR7###", color7);

            // Replace the ninth color.
            css = css.Replace("###COLOR9###", color9);

            // Set the control's inner html code to the css.
            this.InnerHtml = css;
        }

        #endregion


        #region Event Handlers

        protected void ColorSchemeStylesheet_Load(object sender, EventArgs e)
        {
            Render();
        }

        #endregion
    }
}
