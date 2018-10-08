using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUtilities
{
    public class BaseControl : WebControl
    {
        #region Properties

        /// <summary>
        /// Gets the parent table cell
        /// returns null if there isn't one.
        /// </summary>
        public TableCell ParentTableCell
        {
            get
            {
                Control control = this;
                TableCell tableCell = null;

                while (tableCell == null)
                {
                    if (control.GetType().Name == "TableCell")
                    {
                        tableCell = (TableCell)control;

                        break;
                    }

                    if (this.Parent == null)
                        return null;

                    control = this.Parent;
                }

                return tableCell;
            }
        }

        /// <summary>
        /// Gets the parent table row
        /// returns null if there isn't one.
        /// </summary>
        public TableRow ParentTableRow
        {
            get
            {
                Control control = this;
                TableRow tableRow = null;

                while (tableRow == null)
                {
                    if (control.GetType().Name == "TableRow")
                    {
                        tableRow = (TableRow)control;

                        break;
                    }

                    if (this.Parent == null)
                        return null;

                    control = this.Parent;
                }

                return tableRow;
            }
        }

        /// <summary>
        /// Gets or sets the tooltip
        /// is shown when mouse over.
        /// </summary>
        public string ToolTip { get; set; }

        public bool ToolTipMustOverflow { get; set; }

        /// <summary>
        /// Gets the current user of the session.
        /// </summary>
        /*public User User
        {
            get
            {
                if (HttpContext.Current.Session["User"] == null)
                    return null;

                return this.Core.Users.GetSingle((Guid)HttpContext.Current.Session["User"]);
            }
        }*/

        /// <summary>
        /// Gets the database core of the web application's session.
        /// </summary>
        public DatabaseCore.Core Core
        {
            get
            {
                return (DatabaseCore.Core)HttpContext.Current.Session["Core"];
            }
        }

        public LanguageManager LanguageManager
        {
            get
            {
                return (LanguageManager)HttpContext.Current.Session["LanguageManager"];
            }
        }

        public PermissionCore.PermissionCore PermissionCore
        {
            get
            {
                return (PermissionCore.PermissionCore)HttpContext.Current.Session["PermissionCore"];
            }
        }

        public string ClientName
        {
            get
            {
                string result = "";

                Control namingContainer = this.NamingContainer;

                while (namingContainer != null)
                {
                    if (namingContainer.ClientID != "__Page")
                        result = namingContainer.ID + "$" + result;

                    if (namingContainer.GetType() == typeof(System.Web.UI.MasterPage))
                        break;

                    namingContainer = namingContainer.NamingContainer;

                }

                result += this.ID;

                return result;
            }
        }

        /// <summary>
        /// Gets the id of the authenticated of the current session.
        /// </summary>
        public Guid? IdUser
        {
            get
            {
                if (HttpContext.Current.Session["User"] == null)
                    return null;

                return (Guid)HttpContext.Current.Session["User"];
            }
        }

        #endregion


        #region Constructor

        public BaseControl()
            : base("div")
        { }

        public BaseControl(string tagName)
            : base(tagName)
        { }

        #endregion


        #region Methods

        public bool HasPagePermission(string url)
        {
            if (this.PermissionCore != null && this.PermissionCore.PagePermissions != null)
            {
                if (this.PermissionCore.PagePermissions[url] != null)
                {
                    //if (!this.User.HasPermission(this.PermissionCore.PagePermissions[url].Permission.Id))
                    if (!UserHasPermission(this.PermissionCore.PagePermissions[url].Permission.Id))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool UserHasPermission(int permission)
        {
            if (!this.IdUser.HasValue)
                return false;

            if ((int)this.Core.RolePermissions.ExecuteReader(string.Format(
                        "SELECT Count(*) FROM RolePermissions WHERE Permission='{0}' AND IdRole IN (SELECT IdRole FROM UserRoles WHERE IdUser='{1}')",
                        permission,
                        this.IdUser.Value
                    ), typeof(int))[0][0] == 0)
            {
                return false;
            }

            return true;
        }

        #endregion


        #region Event Handlers

        #endregion
    }
}
