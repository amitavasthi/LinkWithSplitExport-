using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Net.Mail;
using System.IO;
using System.Threading;
using DatabaseCore.BaseClasses;
using ApplicationUtilities;
using DatabaseCore.Items;

namespace LinkOnline.Classes
{
    public class SupportClass
    {
        #region Properties
        /// <summary>
        /// Gets or sets the idlast inserted support Id
        /// </summary>
        public string supportId { get; set; }

        #endregion
        #region Methods
        public string ReturnSupportId()
        {
            if (Global.Core.SupportModule.ExecuteReader("SELECT SupportId FROM SupportModule where CreatedDate = ( select  max(CreatedDate) from SupportModule)").Count > 0)
            {
                supportId = Global.Core.SupportModule.ExecuteReader(string.Format(
                   "SELECT SupportId FROM SupportModule where CreatedDate = ( select  max(CreatedDate) from SupportModule)")).FirstOrDefault()[0].ToString();
                supportId = "LiNK_" + (Convert.ToInt64(supportId.Split('_')[1]) + 1).ToString();
            }
            else
            {
                supportId = supportId = "LiNK_" + ConfigurationManager.AppSettings["SupportId"].ToString();
            }

            return supportId;
        }
        public void InsertSupportDetails(string module, string attachImage, string snapShotPath, string userAgent, string browserDetails, string description, string iSupportId)
        {

            if (attachImage == "NO")
            {
                snapShotPath = "No snapshot";
            }

            DatabaseCore.Items.SupportModule supportDetails = new DatabaseCore.Items.SupportModule(Global.Core.SupportModule)
            {

                IdUser = Global.User.Id,
                SupportId = iSupportId,
                Description = description,
                OSDetails = userAgent,
                BrowserDetails = browserDetails,
                Module = module,
                SnapshotPath = snapShotPath,
                Status = "Open"
            };

            supportDetails.Insert();

        }

        #endregion
    }
}