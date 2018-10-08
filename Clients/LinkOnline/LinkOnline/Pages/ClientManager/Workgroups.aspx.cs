using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using WebUtilities.Controls;

namespace LinkOnline.Pages.ClientManager
{
    public partial class Workgroups : WebUtilities.BasePage
    {
        #region Properties

        #endregion


        #region Constructor

        #endregion


        #region Methods

        private void BindWorkgroups()
        {
            // Get all workgroups of the client.
            List<object[]> workgroups = Global.Core.Workgroups.GetValues(
                new string[] { "Id", "Name" },
                new string[] { },
                new object[] { }
            );

            // Run through all workgroups of the client.
            foreach (object[] workgroup in workgroups)
            {
                Panel pnlWorkgroupContainer = new Panel();
                pnlWorkgroupContainer.ID = "pnlWorkgroupContainer" + workgroup[0];
                pnlWorkgroupContainer.Attributes.Add("oncontextmenu", string.Format(
                    "EditWorkgroup('{0}');return false;",
                    workgroup[0]
                ));

                Panel pnlWorkgroupHeadline = new Panel();
                Panel pnlWorkgroupHierachies = new Panel();
                pnlWorkgroupHierachies.ID = "pnlWorkgroupHierachies" + workgroup[0];
                pnlWorkgroupHierachies.CssClass = "WorkgroupHierachies";

                pnlWorkgroupHierachies.Controls.Add(BindHierarchies(workgroup));
                pnlWorkgroupHierachies.Style.Add("display", "none");

                pnlWorkgroupContainer.Controls.Add(pnlWorkgroupHeadline);
                pnlWorkgroupContainer.Controls.Add(pnlWorkgroupHierachies);

                pnlGridContainer.Controls.Add(pnlWorkgroupContainer);

                pnlWorkgroupHeadline.CssClass = "WorkgroupItem BackgroundColor6";
                pnlWorkgroupHeadline.Controls.Add(new LiteralControl(string.Format(
                    "<table cellspacing=\"0\" cellpadding=\"0\"><tr><td style=\"width:60px;\">" +
                    "<img src=\"/Images/Icons/ClientManager/Workgroup.png\" height=\"60\" /></td>" +
                    "<td class=\"WorkgroupItemName\"><div id=\"lblWorkgroupName{1}\">{0}</div></td><td align=\"right\"></td>" +
                    "<td class=\"WorkgroupItemExpand\" onclick=\"ShowWorkgroupHierarchies('{1}');\">" +
                    "<img id=\"imgWorkgroupItemExpand{1}\" src=\"/Images/Icons/VariableSelector/Down.png\" /></td>" +
                    "</tr></table>",
                    (string)workgroup[1],
                    workgroup[0]
                )));
            }
        }


        private TreeView BindHierarchies(object[] workgroup)
        {
            // Get all hierarchies on root level.
            List<object[]> hierarchies = Global.Core.Hierarchies.GetValues(
                new string[] { "Id", "Name" },
                new string[] { "IdHierarchy" },
                new object[] { null }
            );

            TreeView treeView = new TreeView("tvWorkgroupHierarchies" + workgroup[0]);

            // Run through all hierarchies on root level.
            foreach (object[] hierarchy in hierarchies)
            {
                TreeViewNode node = RenderHierarchyTreeViewNode(
                    treeView,
                    workgroup,
                    hierarchy,
                    ""
                );
                node.Attributes.Add("id", "tnHierarchy" + workgroup[0] + hierarchy[0]);

                treeView.Nodes.Add(node);
            }

            return treeView;
        }

        private TreeViewNode RenderHierarchyTreeViewNode(TreeView treeView, object[] workgroup, object[] hierarchy, string path)
        {
            // Create a new tree view node for
            // the hierarchy on root level.
            TreeViewNode node = new TreeViewNode(treeView, hierarchy[0].ToString(), "");
            node.CssClass = "Color1";
            node.Label = string.Format(
                "<table><tr><td><input type=\"checkbox\" {1} onclick=\"ToggleWorkgroupHierarchyCheckbox('{2}', '{3}');\" /></td><td>{0}</td></tr></table>",
                (string)hierarchy[1],
                path == "" ? "checked=\"CHECKED\"" : (Global.Core.WorkgroupHierarchies.Count(
                    new string[] { "IdWorkgroup", "IdHierarchy" },
                    new object[] { workgroup[0], hierarchy[0] }
                ) == 0 ? "" : "checked=\"CHECKED\""),
                workgroup[0],
                hierarchy[0]
            );

            path += "/" + (string)hierarchy[1];

            node.Attributes.Add(
                "Path",
                path
            );
            /*node.OnClientClick = string.Format(
                "LoadStudies(this, '{0}');",
                hierarchy[0]
            );*/

            // Get all hierarchies of the workgroups where the user
            // is assigned to where the hierarchy is the parent.
            List<object[]> childHierarchies = Global.Core.Hierarchies.ExecuteReader(string.Format(
                "SELECT Id, Name FROM [Hierarchies] WHERE IdHierarchy='{1}'",
                Global.IdUser.Value,
                hierarchy[0]
            ));

            // Run through all available child hierarchies of the hierarchy.
            foreach (object[] childHierarchy in childHierarchies)
            {
                node.AddChild(RenderHierarchyTreeViewNode(
                    treeView,
                    workgroup,
                    childHierarchy,
                    path
                ));
            }

            return node;
        }


        private void ToggleWorkgroupHierarchyCheckbox()
        {
            // Parse the id of the workgroup from
            // the http request's parameters.
            Guid idWorkgroup = Guid.Parse(Request.Params["IdWorkgroup"]);

            // Parse the id of the hierarchy from
            // the http request's parameters.
            Guid idHierarchy = Guid.Parse(Request.Params["IdHierarchy"]);

            // Check if the hierarchy is asigned to the workgroup.
            if (Global.Core.WorkgroupHierarchies.GetCount(
                new string[] { "IdWorkgroup", "IdHierarchy" },
                new object[] { idWorkgroup, idHierarchy }
            ) == 0)
            {
                WorkgroupHierarchy workgroupHierarchy = new WorkgroupHierarchy(Global.Core.WorkgroupHierarchies);
                workgroupHierarchy.IdWorkgroup = idWorkgroup;
                workgroupHierarchy.IdHierarchy = idHierarchy;

                workgroupHierarchy.Insert();
            }
            else
            {
                Global.Core.WorkgroupHierarchies.Delete((Guid)Global.Core.WorkgroupHierarchies.GetValue(
                    "Id",
                    new string[] { "IdWorkgroup", "IdHierarchy" },
                    new object[] { idWorkgroup, idHierarchy }
                ));
            }
        }

        private void RenameWorkgroup()
        {
            // Parse the id of the workgroup to rename
            // from the http request's paramters.
            Guid idWorkgroup = Guid.Parse(Request.Params["IdWorkgroup"]);

            // Get the new name for the workgroup
            // from the http request's parameters.
            string name = Request.Params["Name"];

            // Set the new value for the name in the database.
            Global.Core.Workgroups.SetValue("Id=" + idWorkgroup, "Name", name);
        }

        private void DeleteWorkgroup()
        {
            // Parse the id of the workgroup to rename
            // from the http request's paramters.
            Guid idWorkgroup = Guid.Parse(Request.Params["IdWorkgroup"]);

            // Delete all user workgroups of the workgroup.
            Global.Core.UserWorkgroups.ExecuteQuery(string.Format(
                "DELETE FROM UserWorkgroups WHERE IdWorkgroup='{0}'",
                idWorkgroup
            ));

            // Delete all workgroup hierarchies of the workgroup.
            Global.Core.UserWorkgroups.ExecuteQuery(string.Format(
                "DELETE FROM WorkgroupHierarchies WHERE IdWorkgroup='{0}'",
                idWorkgroup
            ));

            // Delete the workgroup in the database.
            Global.Core.Workgroups.Delete(idWorkgroup);
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params["Method"] != null)
            {
                Response.Clear();

                switch (Request.Params["Method"])
                {
                    case "ToggleWorkgroupHierarchyCheckbox":
                        ToggleWorkgroupHierarchyCheckbox();
                        break;
                    case "RenameWorkgroup":
                        RenameWorkgroup();
                        break;
                    case "DeleteWorkgroup":
                        DeleteWorkgroup();
                        break;
                }

                Response.End();
            }

            boxAddWorkgroup.Visible = true;

            BindWorkgroups();
        }


        protected void btnAddWorkgroupConfirm_Click(object sender, EventArgs e)
        {
            Workgroup workgroup = new Workgroup(Global.Core.Workgroups);
            workgroup.Name = txtAddWorkgroupName.Text.Trim();
            workgroup.CreationDate = DateTime.Now;

            workgroup.Insert();

            UserWorkgroup userWorkgroup = new UserWorkgroup(Global.Core.UserWorkgroups);
            userWorkgroup.IdUser = Global.IdUser.Value;
            userWorkgroup.IdWorkgroup = workgroup.Id;

            userWorkgroup.Insert();

            Response.Redirect(Request.Url.ToString());
        }

        #endregion
    }
}