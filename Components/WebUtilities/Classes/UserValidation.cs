using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Xml;
using WebUtilities.Controls;

namespace WebUtilities.Classes
{
    public class UserValidation
    {
        #region Properties

        public bool Exists { get; set; }

        /// <summary>
        /// Gets or sets the full path to the
        /// user validation definition file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets all the validation fields
        /// of the client's user validation.
        /// </summary>
        public Dictionary<string, UserValidationField> Fields { get; set; }

        /// <summary>
        /// Gets or sets if new users are
        /// forced to change their password.
        /// </summary>
        public bool ForcePasswordChange { get; set; }

        #endregion


        #region Constructor

        public UserValidation(string fileName)
        {
            this.FileName = fileName;
            this.Fields = new Dictionary<string, UserValidationField>();

            if (!File.Exists(this.FileName))
                return;

            this.Exists = true;

            this.Parse();
        }

        #endregion


        #region Methods

        private void Parse()
        {
            XmlDocument document = new XmlDocument();
            document.Load(this.FileName);

            if (document.DocumentElement.Attributes["EnforcePasswordChange"] != null)
            {
                this.ForcePasswordChange = bool.Parse(document.
                    DocumentElement.Attributes["EnforcePasswordChange"].Value);
            }

            foreach (XmlNode node in document.DocumentElement.SelectNodes("Fields/Field"))
            {
                this.Fields.Add(
                    node.Attributes["Name"].Value,
                    new UserValidationField(node)
                );
            }
        }

        public Control Render()
        {
            LanguageManager languageManager = (LanguageManager)HttpContext.Current.Session["LanguageManager"];

            Panel result = new Panel();
            result.CssClass = "UserValidationContainer";

            result.Controls.Add(new LiteralControl("<div class=\"BoxBackground\"></div>"));

            Panel control = new Panel();
            control.CssClass = "UserValidation";
            control.ID = "pnlUserValidation";

            control.Controls.Add(new LiteralControl("<h1 class=\"Color1\">" +
                languageManager.GetText("UserValidationTitle") + "</h1>"));

            control.Controls.Add(new LiteralControl("<div id=\"lblUserValidationError\" style=\"visibility:hidden;font-weight:bold;color:#FF0000\">" +
                languageManager.GetText("UserValidationError") + "</div>"));

            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append("<table>");

            foreach (string field in this.Fields.Keys)
            {
                htmlBuilder.Append("<tr>");

                htmlBuilder.Append("<td>");
                htmlBuilder.Append(languageManager.GetText("UserValidation" + field));
                htmlBuilder.Append("</td>");

                htmlBuilder.Append("<td>");
                htmlBuilder.Append("<input name=\"ctlUserValidationValue\" Field=\"" +
                    field + "\" type=\"" + this.Fields[field].Type + "\" />");
                htmlBuilder.Append("</td>");

                htmlBuilder.Append("</tr>");
            }

            htmlBuilder.Append("<tr><td colspan=\"2\" style=\"text-align:right;\">" +
                "<input type=\"button\" onclick=\"UserValidation.ActivateUser();\" value=\"" +
                languageManager.GetText("UserValidationButtonActivate") + "\" /></td></tr>");

            htmlBuilder.Append("</table>");

            control.Controls.Add(new LiteralControl(htmlBuilder.ToString()));
            htmlBuilder.Clear();

            result.Controls.Add(control);

            return result;
        }

        #endregion


        #region Event Handlers

        #endregion
    }

    public class UserValidationField
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the user validation field.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the user validation field.
        /// </summary>
        public UserValidationFieldType Type { get; set; }

        #endregion


        #region Constructor

        public UserValidationField(XmlNode node)
        {
            this.Name = node.Attributes["Name"].Value;
            this.Type = (UserValidationFieldType)Enum.Parse(
                typeof(UserValidationFieldType),
                node.Attributes["Type"].Value
            );
        }

        public Control Render(string value = null)
        {
            Control result = null;

            switch (this.Type)
            {
                case UserValidationFieldType.Numeric:
                    result = new TextBox()
                    {
                        TextMode = System.Web.UI.WebControls.TextBoxMode.Number,
                        Text = value
                    };
                    break;
                case UserValidationFieldType.Text:
                    result = new TextBox()
                    {
                        Text = value
                    };
                    break;
            }

            if (result == null)
                result = new System.Web.UI.WebControls.Label();

            result.ID = "ctlUserValidationField" + this.Name;

            return result;
        }

        #endregion


        #region Methods

        #endregion
    }

    public enum UserValidationFieldType
    {
        Numeric, Text
    }
}
