using ApplicationUtilities;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages.ClientManager
{
    public partial class UsersHome :WebUtilities.BasePage 
    {
        #region Properties
        public string password;
        #endregion
        #region Method
        private static DbConnection DbConnection()
        {
            //get the proper factory 
            DbProviderFactory factory = DbProviderFactories.GetFactory(Global.Core.DatabaseProvider);
            DbConnectionStringBuilder dbProviderBuilder = factory.CreateConnectionStringBuilder();
            var connString = Global.Core.ConnectionString;
            //make sure it got created
            if (dbProviderBuilder != null)
            {
                dbProviderBuilder.ConnectionString = connString;
                //if this provider supports this setting, set it.
                if (dbProviderBuilder.ContainsKey("MultipleActiveResultSets"))
                {
                    dbProviderBuilder["MultipleActiveResultSets"] = true;
                }
                //use the new modified connection string
                connString = dbProviderBuilder.ConnectionString;
            }
            //create a command of the proper type.
            DbConnection dbCon = factory.CreateConnection();
            //set the connection string
            if (dbCon != null)
            {
                dbCon.ConnectionString = connString;
            }
            return dbCon;
        }
        private static bool DatabaseExists(string dbName)
        {
            bool bRet = false;
            var con = DbConnection();
            con.ConnectionString = string.Format(ConfigurationManager.AppSettings["ConnectionStringUserManagement"], "master");
            using (con)
            {
                con.Open();
                using (var command = con.CreateCommand())
                {
                    // string cmdText = "select * from master.dbo.sysdatabases where name=\'" + dbName + "\'";
                    //string cmdText = string.Format("select * from master.dbo.sysdatabases where name=\'{0}'", dbName);
                    string cmdText = string.Format("select * from master.dbo.sysdatabases where name=@name");
                    command.CommandText = cmdText;
                    DbParameter dbParameter1 = command.CreateParameter();
                    dbParameter1.ParameterName = "name";
                    dbParameter1.Value = dbName;
                    command.Parameters.Add(dbParameter1);

                    var nRet = command.ExecuteScalar();

                    if (nRet != null)
                    {
                        bRet = true;
                    }
                }
            }
            return bRet;
        }


        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["msg"] != null)
                {
                    switch (Request.QueryString["msg"].Trim())
                    {
                        case "1":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("UserAddMsg"),
                            Global.LanguageManager.GetText("UserAddMsg")), WebUtilities.MessageType.Success);
                            break;
                        case "2":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText(HttpContext.Current.Session["Error"].ToString()),
                            Global.LanguageManager.GetText(HttpContext.Current.Session["Error"].ToString())), WebUtilities.MessageType.Error);
                            break;
                        case "3":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("DuplicateError"),
                            Global.LanguageManager.GetText("DuplicateError")), WebUtilities.MessageType.Error);
                            break;                        
                    }
                }
            }
        }

        protected void btnAdd_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("CreateUser.aspx", false);
        }
        protected void btnManage_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("ManageUsers.aspx", false);
        }
       
    }
}