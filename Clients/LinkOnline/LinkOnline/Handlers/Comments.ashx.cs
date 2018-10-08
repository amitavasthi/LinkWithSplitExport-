using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Summary description for Comments
    /// </summary>
    public class Comments : WebUtilities.BaseHandler
    {
        #region Properties

        #endregion


        #region Constructor

        public Comments()
        {
            base.Methods.Add("GetComments", GetComments);
            base.Methods.Add("AddComment", AddComment);
            base.Methods.Add("SaveComment", SaveComment);
            base.Methods.Add("DeleteComment", DeleteComment);
        }

        #endregion


        #region Methods

        #endregion


        #region Web Methods

        private void GetComments(HttpContext context)
        {
            // Get the section of which to get the comments
            // from the http request's parameters.
            string section = context.Request.Params["Section"];

            // Check if there is a permission
            // restriction for the comment section.
            if (Global.PermissionCore.Permissions["CommentSection_View_" + section] != null)
            {
                // Check if the user has access
                // granted to the comment section.
                if (Global.Core.Users.HasPermission(
                    Global.IdUser.Value,
                    Global.PermissionCore.Permissions["CommentSection_View_" + section].Id
                ) == false)
                {
                    throw new Exception("Access denied.");
                }
            }

            // Build the full path to the xml
            // file that stores the comments.
            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Comments",
                Global.Core.ClientName,
                section + ".xml"
            );

            // Check if there are any comments for that section.
            if (!File.Exists(fileName))
            {
                // Write an empty array to the http response.
                context.Response.Write("[]");
                return;
            }

            // Create a new string builder that
            // holds the result JSON string.
            StringBuilder result = new StringBuilder();

            // Open the JSON array that holds the comments.
            result.Append("[");

            // Create a new xml document that
            // holds the comments definition.
            XmlDocument document = new XmlDocument();

            // Load the contents of the comments
            // xml file into the xml document.
            document.Load(fileName);

            // Run through all comment xml nodes.
            foreach (XmlNode xmlNode in document.
                DocumentElement.SelectNodes("Comment"))
            {
                // Check if the comment was deleted.
                if (xmlNode.Attributes["Deleted"] != null)
                    continue;

                // Parse the id of the user that created the comment.
                Guid idUser = Guid.Parse(xmlNode.Attributes["IdUser"].Value);

                // Get the display name of the user that created the comment.
                string displayUsername = Global.GetNiceUsername(idUser);

                // Check if the a user with the id still exists.
                if (displayUsername == "")
                {
                    // Get the name of the user by the
                    // time the comment was created.
                    displayUsername = xmlNode.Attributes["DisplayUsername"].Value;
                }

                // Render the comment details to the result JSON string.
                result.Append(base.ToJson(new string[]
                {
                    "Id",
                    "Username",
                    "CreationDate",
                    "TimeDifference",
                    "Comment",
                    "Attributes",
                    "CanModify"
                }, new object[] {
                    xmlNode.Attributes["Id"].Value,
                    displayUsername,
                    DateTime.Parse(xmlNode.Attributes["CreationDate"].Value).ToFormattedString(),
                    DateTime.Parse(xmlNode.Attributes["CreationDate"].Value).ToTimeDifference(),
                    HttpUtility.HtmlEncode(xmlNode.InnerText.Trim()),
                    xmlNode.Attributes["Attributes"].Value,
                    xmlNode.Attributes["IdUser"].Value == Global.IdUser.Value.ToString()
                }));

                result.Append(",");
            }

            result.RemoveLastComma();

            // Close the JSON array that holds the comments.
            result.Append("]");

            // Write the contents of the result
            // JSON string to the http response.
            context.Response.Write(result.ToString());
        }

        private void AddComment(HttpContext context)
        {
            // Get the section of which to get the comments
            // from the http request's parameters.
            string section = context.Request.Params["Section"];

            // Check if there is a permission
            // restriction for the comment section.
            if (Global.PermissionCore.Permissions["CommentSection_Set_" + section] != null)
            {
                // Check if the user has access
                // granted to the comment section.
                if (Global.Core.Users.HasPermission(
                    Global.IdUser.Value,
                    Global.PermissionCore.Permissions["CommentSection_Set_" + section].Id
                ) == false)
                {
                    throw new Exception("Access denied.");
                }
            }

            // Build the full path to the xml
            // file that stores the comments.
            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Comments",
                Global.Core.ClientName,
                section + ".xml"
            );

            // Check if there are any comments for that section.
            if (!File.Exists(fileName))
            {
                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                // Create a new, blank comments file.
                File.WriteAllText(fileName, "<Comments></Comments>");
            }

            // Create a new xml document that
            // holds the comments definition.
            XmlDocument document = new XmlDocument();

            // Load the contents of the comments
            // xml file into the xml document.
            document.Load(fileName);

            // Add the comment to the comments xml file.
            document.DocumentElement.InnerXml += string.Format(
                "<Comment Id=\"{0}\" IdUser=\"{1}\" " +
                "DisplayUsername=\"{2}\" CreationDate=\"{3}\" Attributes=\"{5}\"><![CDATA[{4}]]></Comment>",
                Guid.NewGuid(),
                Global.IdUser.Value,
                Global.GetNiceUsername(Global.IdUser.Value),
                DateTime.Now.ToString(),
                context.Request.Params["Comment"],
                HttpUtility.HtmlEncode(context.Request.Params["Attributes"])
            );

            // Save the comments xml file.
            document.Save(fileName);
        }

        private void SaveComment(HttpContext context)
        {
            // Get the section of which to get the comments
            // from the http request's parameters.
            string section = context.Request.Params["Section"];

            // Check if there is a permission
            // restriction for the comment section.
            if (Global.PermissionCore.Permissions["CommentSection_Set_" + section] != null)
            {
                // Check if the user has access
                // granted to the comment section.
                if (Global.Core.Users.HasPermission(
                    Global.IdUser.Value,
                    Global.PermissionCore.Permissions["CommentSection_Set_" + section].Id
                ) == false)
                {
                    throw new Exception("Access denied.");
                }
            }

            // Build the full path to the xml
            // file that stores the comments.
            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Comments",
                Global.Core.ClientName,
                section + ".xml"
            );

            // Check if there are any comments for that section.
            if (!File.Exists(fileName))
            {
                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                // Create a new, blank comments file.
                File.WriteAllText(fileName, "<Comments></Comments>");
            }

            // Create a new xml document that
            // holds the comments definition.
            XmlDocument document = new XmlDocument();

            // Load the contents of the comments
            // xml file into the xml document.
            document.Load(fileName);

            XmlNode xmlNode = document.DocumentElement.SelectSingleNode(string.Format(
                "Comment[@Id=\"{0}\"]",
                context.Request.Params["IdComment"]
            ));

            // Check if the comment exists.
            if (xmlNode == null)
                throw new Exception("Comment does not exist.");

            // Check if the user is allowed the modify the comment.
            if (document.DocumentElement.Attributes["SaveUserRestricted"] != null &&
                bool.Parse(document.DocumentElement.Attributes["SaveUserRestricted"].Value) == true &&
                xmlNode.Attributes["IdUser"].Value != Global.IdUser.Value.ToString())
                throw new Exception("Access denied.");

            // Set the new comment text.
            xmlNode.InnerText = context.Request.Params["Comment"];

            // Save the comments xml file.
            document.Save(fileName);
        }

        private void DeleteComment(HttpContext context)
        {
            // Get the section of which to get the comments
            // from the http request's parameters.
            string section = context.Request.Params["Section"];

            // Check if there is a permission
            // restriction for the comment section.
            if (Global.PermissionCore.Permissions["CommentSection_View_" + section] != null)
            {
                // Check if the user has access
                // granted to the comment section.
                if (Global.Core.Users.HasPermission(
                    Global.IdUser.Value,
                    Global.PermissionCore.Permissions["CommentSection_View_" + section].Id
                ) == false)
                {
                    throw new Exception("Access denied.");
                }
            }

            // Build the full path to the xml
            // file that stores the comments.
            string fileName = Path.Combine(
                context.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Comments",
                Global.Core.ClientName,
                section + ".xml"
            );

            // Check if there are any comments for that section.
            if (!File.Exists(fileName))
            {
                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                // Create a new, blank comments file.
                File.WriteAllText(fileName, "<Comments></Comments>");
            }

            // Create a new xml document that
            // holds the comments definition.
            XmlDocument document = new XmlDocument();

            // Load the contents of the comments
            // xml file into the xml document.
            document.Load(fileName);

            XmlNode xmlNode = document.DocumentElement.SelectSingleNode(string.Format(
                "Comment[@Id=\"{0}\"]",
                context.Request.Params["IdComment"]
            ));

            // Check if the comment exists.
            if (xmlNode == null)
                throw new Exception("Comment does not exist.");

            // Check if the user is allowed the delete the comment.
            if (xmlNode.Attributes["IdUser"].Value != Global.IdUser.Value.ToString())
                throw new Exception("Access denied.");

            // Remove the xml node that defines the comment.
            //xmlNode.ParentNode.RemoveChild(xmlNode);
            xmlNode.AddAttribute("Deleted", "True");

            // Save the comments xml file.
            document.Save(fileName);
        }

        #endregion
    }
}