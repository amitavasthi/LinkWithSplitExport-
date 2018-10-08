using Crosstables.Classes.ReportDefinitionClasses;
using LinkBi1.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Homescreen1.Classes.Modules
{
    public class HomescreenModuleRecentUsed : HomescreenModule
    {
        #region Properties

        /// <summary>
        /// Gets or sets the count of items.
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// Gets or sets a list of the available recent use items.
        /// </summary>
        public List<RecentUsedItem> Items { get; set; }

        #endregion


        #region Constructor

        public HomescreenModuleRecentUsed(HomescreenNode node)
            : base(node)
        { }

        #endregion


        #region Methods

        private void LoadSavedReports(Guid idUser)
        {
            string directoryNameSavedReports = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "SavedReports",
                this.Node.Owner.Core.ClientName,
                idUser.ToString()
            );

            if (!Directory.Exists(directoryNameSavedReports))
                return;

            // Get the user's saved reports.
            string[] savedReports = Directory.GetDirectories(directoryNameSavedReports);

            // Run through all saved reports of the user.
            foreach (string savedReport in savedReports)
            {
                Guid guidOutput = Guid.Empty;
                //check is report directory is guid or no.
                if (!Guid.TryParse(Path.GetFileName(savedReport), out guidOutput))
                    continue;

                ReportDefinitionInfo info = new ReportDefinitionInfo(Path.Combine(
                savedReport,
                "Info.xml"
            ));

                if (info.GetReports(this.Node.Owner.Core, this.Node.Owner.IdUser.Value).Count == 0)
                    continue;

                // Create a new recent use item by the saved report.
                RecentUsedItem item = new RecentUsedItem();

                item.LatestUse = info.LatestAccess;
                item.Name = info.Name;
                item.Type = RecentUsedItemType.Report;
                item.OnClick = string.Format(
                    "window.location='/Pages/LinkReporter/Crosstabs.aspx?SavedReport={0}{1}';",
                    idUser,
                    new FileInfo(savedReport).Name
                );
                item.Identity = new DirectoryInfo(directoryNameSavedReports).Name +
                    new DirectoryInfo(savedReport).Name;

                this.Items.Add(item);
            }
        }

        private void LoadLinkBiReports(Guid idUser)
        {
            string directoryName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "SavedLinkBiDefinitions",
                ((DatabaseCore.Core)HttpContext.Current.Session["Core"]).ClientName
            );

            if (!Directory.Exists(directoryName))
                return;

            // Get the client's saved linkBi definitions.
            string[] savedLinkBIDefinitions = Directory.GetFiles(directoryName);

            // Run through all saved linkBI reports of the client.
            foreach (string savedLinkBIDefinition in savedLinkBIDefinitions)
            {
                LinkBiDefinitionInfo info = new LinkBiDefinitionInfo(savedLinkBIDefinition);

                // Create a new recent use item by the saved report.
                RecentUsedItem item = new RecentUsedItem();

                DateTime latestUse = new DateTime();

                if (info.LatestUses.ContainsKey(idUser))
                    latestUse = info.LatestUses[idUser];

                item.LatestUse = latestUse;
                item.Name = info.ReportName;
                item.Type = RecentUsedItemType.LinkBi;
                item.OnClick = "_AjaxRequest('/Handlers/LinkBi.ashx', 'SetActiveLinkBiDefinition', 'FileName=" +
                    savedLinkBIDefinition.Replace("\\", "/") + "', function (response) { window.location = '/Pages/LinkBi/LinkBi.aspx'; });";

                this.Items.Add(item);
            }
        }

        private void LoadDashboards(Guid idUser)
        {
            string directoryName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "DashboardItems",
                ((DatabaseCore.Core)HttpContext.Current.Session["Core"]).ClientName
            );

            if (!Directory.Exists(directoryName))
                return;

            // Get the client's saved linkBi definitions.
            string[] dashboardItems = Directory.GetFiles(directoryName);

            // Run through all saved linkBI reports of the client.
            foreach (string dashboardItem in dashboardItems)
            {
                DashboardItem info = new DashboardItem(dashboardItem);

                // Create a new recent use item by the saved report.
                RecentUsedItem item = new RecentUsedItem();

                DateTime latestUse = new DateTime();

                if (info.LatestUses.ContainsKey(idUser))
                    latestUse = info.LatestUses[idUser];

                item.LatestUse = latestUse;
                item.Name = info.Name;
                item.Type = RecentUsedItemType.Dashboard;
                item.OnClick = string.Format(
                    "window.location = '/Pages/Dashboards.aspx?IdDashboardItem={0}';",
                    info.Id
                );

                this.Items.Add(item);
            }
        }


        private void GetRecentUsedItems()
        {
            Guid idUser = (Guid)HttpContext.Current.Session["User"];

            this.Items = new List<RecentUsedItem>();

            LoadSavedReports(idUser);
            LoadLinkBiReports(idUser);
            LoadDashboards(idUser);

            this.Items = this.Items.OrderByDescending(x => x.LatestUse).ToList();
        }

        public override void Render(StringBuilder writer)
        {
            if (this.Width == null || this.Width == "")
                this.Width = "ContentWidth";

            //this.ItemCount = this.Width / 120;
            this.ItemCount = this.Node.Owner.BaseContentWidth / 120;

            this.GetRecentUsedItems();

            for (int i = 0; i < this.ItemCount; i++)
            {
                if (i >= this.Items.Count)
                    break;

                this.Items[i].Render(writer);
            }
        }

        #endregion
    }

    public class RecentUsedItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets the type of the recent used item.
        /// </summary>
        public RecentUsedItemType Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the recent used item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the click action of
        /// the recent used item as javascript.
        /// </summary>
        public string OnClick { get; set; }

        /// <summary>
        /// Gets or sets when the latest use
        /// of the recent use item was.
        /// </summary>
        public DateTime LatestUse { get; set; }

        public string Identity { get; set; }

        #endregion


        #region Methods

        public void Render(StringBuilder writer)
        {
            // Open the recent used item's div tag.
            writer.Append("<div onmouseover=\"this.title=this.innerText\" style=\"max-height:140px;overflow: hidden;\"; class=\"RecentUsedItem\">");

            // Render the recent used item's icon.
            writer.Append(string.Format(
                "<img class=\"RecentUsedItemImage\" style=\"height:100px;\" src=\"/Images/Icons/RecentUsedItems/{0}.png\" " +
                "onmouseover=\"this.src = '/Images/Icons/Cloud/Run.png'\" " +
                "onmouseout=\"this.src = '/Images/Icons/RecentUsedItems/{0}.png'\" " +
                "onclick=\"{1}\"",
                this.Type,
                this.OnClick
            ));

            if (this.Type == RecentUsedItemType.Report)
            {
                writer.Append(string.Format(
                    " oncontextmenu=\"RenderSavedReportOptions(this, '{0}', '{1}'); return false;\"",
                    this.Name,
                    this.Identity
                ));
            }

            writer.Append(" /><br />");

            // Render the recent used item's name.
            writer.Append(this.Name);

            // Close the recent used item's div tag.
            writer.Append("</div>");
        }

        #endregion
    }

    public enum RecentUsedItemType
    {
        Report,
        LinkBi,
        Dashboard
    }
}
