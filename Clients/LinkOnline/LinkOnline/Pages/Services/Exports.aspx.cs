using DatabaseCore.Items;
using DataCore.Classes;
using DataInterface.Classes;
using LinkOnline.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkOnline.Pages.Services
{
    public partial class Exports : WebUtilities.BasePage
    {
        #region Properties

        public SpssWriter Writer
        {
            get
            {
                if (HttpContext.Current.Session["Exports_SpssWriter"] == null)
                    return null;

                return (SpssWriter)HttpContext.Current.Session["Exports_SpssWriter"];
            }
            set
            {
                HttpContext.Current.Session["Exports_SpssWriter"] = value;
            }
        }

        public ExportDefinition Definition { get; set; }

        #endregion


        #region Constructor

        #endregion


        #region Methods

        private void BindVariables()
        {
            List<object[]> taxonomyVariables = Global.Core.TaxonomyVariables.GetValues(
                new string[] { "Id" },
                new string[] { },
                new object[] { }
            );

            foreach (object[] taxonomyVariable in taxonomyVariables)
            {
                VariableSelector1.Classes.VariableSelector variableSelector = new VariableSelector1.Classes.VariableSelector(
                    2057,
                    "TaxonomyVariables",
                    "Id=" + taxonomyVariable[0]
                );

                variableSelector.Settings.Dragable = true;

                if (this.Definition.SelectedVariables.Contains((Guid)taxonomyVariable[0]))
                    pnlSelectedVariables.Controls.Add(variableSelector);
                else
                    pnlAvailableVariables.Controls.Add(variableSelector);
            }
        }


        private void ExportAsynch(object p)
        {
            object[] parameters = (object[])p;

            HttpSessionState session = (HttpSessionState)parameters[1];

            session["Exports_FileName"] = null;

            SpssWriter writer = (SpssWriter)parameters[0];

            session["Exports_FileName"] = writer.Export((TaxonomyVariable[])parameters[2]);

            session["Exports_SpssWriter"] = null;
        }

        private void Export()
        {
            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(ExportAsynch);

            Thread thread = new Thread(threadStart);

            SpssWriter writer = new SpssWriter(
                new CultureInfo(2057),
                Global.Core
            );

            /*List<Guid> respondents = Global.Core.Respondents.GetValues(
                new string[] { "Id" },
                new string[] { },
                new object[] { }
            ).Select(x=>(Guid)x[0]).ToList();*/

            //Data filter = this.Definition.Workflow.GetWorkflowFilter(true);
            Data filter = null;

            if (filter != null)
            {
                writer.Respondents = filter.Responses.Keys.ToList();
            }
            else
            {
                writer.Respondents = Global.Core.Respondents.GetValues(
                    new string[] { "Id" },
                    new string[] { },
                    new object[] { }
                ).Select(x => (Guid)x[0]).ToList();
            }

            this.Writer = writer;

            List<TaxonomyVariable> variables = new List<TaxonomyVariable>();

            foreach (Guid idVariable in this.Definition.SelectedVariables)
            {
                TaxonomyVariable taxonomyVariable = Global.Core.TaxonomyVariables.GetSingle(idVariable);

                if (taxonomyVariable == null)
                    continue;

                variables.Add(taxonomyVariable);
            }

            if (variables.Count == 0)
            {
                this.Writer = null;
                return;
            }

            thread.Start(new object[]{
                writer,
                HttpContext.Current.Session,
                variables.ToArray()
            });
        }

        private void GetExportProgress()
        {
            this.Response.Clear();

            if (this.Writer == null)
            {
                this.Response.Write(100);
            }
            else
            {
                this.Response.Write(this.Writer.CaseDataProgress);
            }

            this.Response.End();
        }

        private string _GetExportFileName()
        {
            if (HttpContext.Current.Session["Exports_FileName"] != null)
            {
                string fileName = Path.Combine(
                    this.Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "Temp",
                    "DataExports",
                    Global.Core.ClientName + ".sav"
                );

                if ((string)HttpContext.Current.Session["Exports_FileName"] != fileName)
                {
                    if (File.Exists(fileName))
                        File.Delete(fileName);

                    File.Move((string)HttpContext.Current.Session["Exports_FileName"], fileName);
                    HttpContext.Current.Session["Exports_FileName"] = fileName;
                }

                return (string.Format(
                    "/Fileadmin/Temp/{0}.sav",
                    Global.Core.ClientName
                ));
            }

            return "";
        }

        private void GetExportFileName()
        {
            this.Response.Clear();

            this.Response.Write(this._GetExportFileName());

            this.Response.End();
        }

        private void DownloadExport()
        {
            if (HttpContext.Current.Session["Exports_FileName"] == null)
                return;

            if (!File.Exists((string)HttpContext.Current.Session["Exports_FileName"]))
                return;

            base.WriteFileToResponse(
                (string)HttpContext.Current.Session["Exports_FileName"],
                Global.Core.ClientName + ".sav",
                "application/sav",
                true
            );
        }


        private void SelectVariable()
        {
            Guid idVariable = Guid.Parse(this.Request.Params["Id"]);

            this.Definition.SelectVariable(idVariable);
            this.Definition.Save();
        }

        private void SelectWorkflowSelectorItem()
        {
            string workflowSelection = this.Request.Params["WorkflowSelection"];
            string workflowSelectionVariable = this.Request.Params["WorkflowSelectionVariable"];
            string action = this.Request.Params["Action"];

            Guid idItem = Guid.Parse(
                this.Request.Params["IdItem"]
            );

            bool isDefault = this.Definition.Workflow.Selections[workflowSelection].SelectionVariables[workflowSelectionVariable].IsDefaultSelection;

            if (action == "Select")
            {
                this.Definition.Workflow.Selections[workflowSelection].SelectionVariables[workflowSelectionVariable].Select(idItem);
            }
            else if (action == "DeSelect")
            {
                this.Definition.Workflow.Selections[workflowSelection].SelectionVariables[workflowSelectionVariable].DeSelect(idItem);
            }

            this.Definition.Save();
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            string fileName = Path.Combine(
                this.Request.PhysicalApplicationPath,
                "Fileadmin",
                "ExportDefinitions",
                Global.IdUser.Value + ".xml"
            );

            this.Definition = new ExportDefinition(fileName);

            pnlWorkflow.Controls.Add(this.Definition.Workflow);

            // Check if a method to execute is defined.
            if (this.Request.Params["Method"] != null)
            {
                switch (this.Request.Params["Method"])
                {
                    case "Export":
                        Export();
                        break;

                    case "GetExportProgress":
                        GetExportProgress();
                        break;

                    case "GetExportFileName":
                        GetExportFileName();
                        break;

                    case "DownloadExport":
                        DownloadExport();
                        break;

                    case "SelectVariable":
                        SelectVariable();
                        break;

                    case "SelectWorkflowSelectorItem":
                        SelectWorkflowSelectorItem();
                        break;

                    case "ClearExportDefinition":
                        File.Delete(fileName);
                        break;
                }

                return;
            }

            BindVariables();
        }

        #endregion
    }
}