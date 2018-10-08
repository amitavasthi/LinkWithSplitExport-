using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages.Services
{
    public partial class TableauConnector : System.Web.UI.Page
    {
        #region Properties

        #endregion


        #region Constructor

        #endregion


        #region Methods

        private void BindHierarchySelector()
        {
            /*HierarchySelector.FileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "HierarchySelectors",
                Global.Core.ClientName + ".xml"
            );*/

            //HierarchySelector.Source = fileName;
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if(Request.Params["Method"] == "Load")
            {

                string script = File.ReadAllText(Path.Combine(
                    Request.PhysicalApplicationPath,
                    "App_Data",
                    "TableauConnector.html"
                ));

                script = script.Replace("###FIELDNAMES###", HttpUtility.UrlDecode(Request.Params["FieldNames"]));
                script = script.Replace("###FIELDTYPES###", HttpUtility.UrlDecode(Request.Params["FieldTypes"]));
                script = script.Replace("###RETURNDATA###", HttpUtility.UrlDecode(Request.Params["ReturnData"]));

                Guid idTempFile = Guid.NewGuid();

                string tempFile = Path.Combine(
                    Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "Temp",
                    "TableauCache",
                    idTempFile.ToString() + ".html"
                );

                if (!Directory.Exists(Path.GetDirectoryName(tempFile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(tempFile));

                File.WriteAllText(tempFile, script);

                //Response.Clear();

                //Response.Write(string.Format(
                //    "/Fileadmin/TableauCache/{0}.html"
                //));
                Response.Redirect(string.Format(
                    "/Fileadmin/Temp/TableauCache/{0}.html",
                    idTempFile
                ));

                //Response.Flush();
                //Response.End();
            }

            BindHierarchySelector();
        }

        #endregion
    }
}