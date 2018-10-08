using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace LinkOnline.Pages
{
    public partial class DataCheck : WebUtilities.BasePage
    {
        #region Properties

        #endregion


        #region Constructor

        public DataCheck()
        {

        }

        #endregion


        #region Methods

        private void BindStudies()
        {
            // Run through all studies.
            foreach (Study study in Global.Core.Studies.Get())
            {
                ListItem lItem = new ListItem();
                lItem.Text = study.Name;
                lItem.Value = study.Id.ToString();

                ddlStudy.Items.Add(lItem);
            }
        }


        public int GetV3Value(Guid idCategory)
        {
            string fileName = Path.Combine(
                string.Format(ConfigurationManager.AppSettings["FileStorageRoot"], "linkmanager"),
                string.Format("[resp].[Var_{0}].xml", idCategory)
            );

            if (!File.Exists(fileName))
                return 0;

            StringBuilder xmlString = new StringBuilder();
            xmlString.Append("<Responses>");
            xmlString.Append(File.ReadAllText(fileName));
            xmlString.Append("</Responses>");

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlString.ToString());

            xmlString.Clear();

            return xmlDocument.DocumentElement.ChildNodes.Count;
        }

        public int GetV2Value(string variableName, string categoryName)
        {
            SqlConnection sqlConnection = new SqlConnection(string.Format(
                "Data Source={0};Initial Catalog={1};Integrated Security=True",
                txtDatabaseServer.Text,
                txtDatabaseName.Text
            ));

            int[] ids = GetV2CategoryId(variableName, categoryName);

            if (ids == null)
                return -1;

            int[] filter = GetV2CategoryId("DETId", ddlStudy.SelectedItem.Text);

            SqlCommand sqlCommand = new SqlCommand(string.Format(
                "SELECT Count(*) FROM resp.Var_{0} WHERE IdCategory='{1}' AND IdRespondent IN (SELECT IdRespondent FROM resp.Var_{2} WHERE IdCategory='{3}')",
                ids[1],
                ids[0],
                filter[1],
                filter[0]
            ), sqlConnection);

            sqlConnection.Open();

            int result = (int)sqlCommand.ExecuteScalar();

            sqlConnection.Close();

            return result;
        }


        private int[] GetV2CategoryId(string variableName, string categoryName)
        {
            SqlConnection sqlConnection = new SqlConnection(string.Format(
                "Data Source={0};Initial Catalog={1};Integrated Security=True",
                txtDatabaseServer.Text,
                txtDatabaseName.Text
            ));

            SqlDataAdapter sqlCommand = new SqlDataAdapter(string.Format(
                "SELECT IdCategory, (SELECT IdVariable FROM Categories WHERE Id=IdCategory) FROM VariableLinks WHERE IdOriginalCategory = (SELECT Id FROM OriginalCategories WHERE IdOriginalVariable=(SELECT Id FROM OriginalVariables WHERE Name='{0}') AND Name='{1}')",
                variableName,
                categoryName
            ), sqlConnection);

            sqlConnection.Open();

            DataTable dtResult = new DataTable();
            sqlCommand.Fill(dtResult);

            sqlConnection.Close();

            int[] result = null;

            try
            {
                result = new int[]{
                    (int)dtResult.Rows[0][0],
                    (int)dtResult.Rows[0][1]
                };
            }
            catch { }

            dtResult.Dispose();

            return result;
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindStudies();
        }

        protected void btnCheck_Click(object sender, EventArgs e)
        {
            Dictionary<Category, int[]> categoryErrors = new Dictionary<Category, int[]>();

            // Get the selected study.
            Study study = Global.Core.Studies.GetSingle(
                Guid.Parse(ddlStudy.SelectedValue)
            );

            // Run through all variables of the study.
            foreach (Variable variable in study.Variables)
            {
                // Run through all categories of the variable.
                foreach (Category category in variable.Categories)
                {
                    int v3Value = GetV3Value(category.Id);
                    int v2Value = GetV2Value(variable.Name, category.Name);

                    if (v3Value == 0)
                        continue;

                    if (v2Value == -1)
                        continue;

                    if (v3Value > v2Value)
                        categoryErrors.Add(category, new int[] { v2Value, v3Value });
                }
            }

            StringBuilder logFile = new StringBuilder();

            Table table = new Table();

            // Run through all category errors.
            foreach (KeyValuePair<Category, int[]> category in categoryErrors)
            {
                TableRow tableRow = new TableRow();

                TableCell tableCellVariable = new TableCell();
                TableCell tableCellCategory = new TableCell();

                tableCellVariable.Text = category.Key.Variable.Name;
                tableCellCategory.Text = category.Key.Name;

                tableRow.Cells.Add(tableCellVariable);
                tableRow.Cells.Add(tableCellCategory);

                table.Rows.Add(tableRow);

                logFile.Append(string.Format(
                    "Variable '{0}' has an error with category '{1}': V3 value: {2}   V2 value: {3}" + Environment.NewLine,
                    tableCellVariable.Text,
                    tableCellCategory.Text,
                    category.Value[1],
                    category.Value[0]
                ));
            }

            File.WriteAllText(
                "C:\\Temp\\V3V2Compare.log",
                logFile.ToString()
            );

            logFile.Clear();

            pnlResult.Controls.Add(table);
        }

        #endregion
    }
}