using LinkBi1.Classes;
using LinkBi1.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages.LinkBi
{
    public partial class SelectInterface : WebUtilities.BasePage
    {
        #region Properties

        public LinkBiDefinition LinkBiDefinition { get; set; }

        #endregion


        #region Constructor

        #endregion


        #region Methods

        private void Start(LinkBiInterface linkBiInterface)
        {
            if (chkMailMe.Checked)
            {
                linkBiInterface.SendMail = true;
                linkBiInterface.Language = Global.Language.ToString();
                linkBiInterface.Request = Request;
                linkBiInterface.User = Global.User;
            }

            HttpContext.Current.Session["CurrentLinkBiCreation"] = linkBiInterface;

            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(StartAsynch);

            Thread thread = new Thread(threadStart);
            thread.Start(linkBiInterface);

            Response.Redirect("Status.aspx");
        }

        private void StartAsynch(object _linkBiInterface)
        {
            LinkBiInterface linkBiInterface = (LinkBiInterface)_linkBiInterface;

            string fileName = linkBiInterface.Render();
            linkBiInterface.FileName = fileName;
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(HttpContext.Current.Session["LinkBiDefinition"].ToString()))
            {
                Response.Redirect("LinkBi.aspx");
                return;
            }

            string fileName = HttpContext.Current.Session["LinkBiDefinition"].ToString();

            // Check if the LinkBi definition exists.
            if (!File.Exists(fileName))
            {
                Response.Redirect("LinkBi.aspx");
            }

            this.LinkBiDefinition = new LinkBiDefinition(
                Global.Core, 
                fileName,
                Global.HierarchyFilters[fileName]
            );
        }

        protected void btnInterfacePowerBi_Click(object sender, EventArgs e)
        {
            Excel powerBI = new Excel(
                Global.Core,
                this.LinkBiDefinition
            );

            Start(powerBI);
        }

        protected void btnInterfaceXML_Click(object sender, EventArgs e)
        {
            XML xml = new XML(
                Global.Core,
                this.LinkBiDefinition
            );

            Start(xml);
        }

        protected void btnInterfaceCSV_Click(object sender, EventArgs e)
        {
            LinkBi1.Classes.Interfaces.CSV csv = new LinkBi1.Classes.Interfaces.CSV(
                Global.Core,
                this.LinkBiDefinition
            );

            Start(csv);
        }

        protected void btnInterfaceCustomCharts_Click(object sender, EventArgs e)
        {
            LinkBi1.Classes.Interfaces.CustomCharts csv = new LinkBi1.Classes.Interfaces.CustomCharts(
                Global.Core,
                this.LinkBiDefinition
            );

            Start(csv);
        }

        protected void btnInterfaceTempTableTest_Click(object sender, EventArgs e)
        {
            LinkBi1.Classes.Interfaces.TempTableTest test = new LinkBi1.Classes.Interfaces.TempTableTest(
                Global.Core,
                this.LinkBiDefinition
            );

            Start(test);
        }

        #endregion
    }
}