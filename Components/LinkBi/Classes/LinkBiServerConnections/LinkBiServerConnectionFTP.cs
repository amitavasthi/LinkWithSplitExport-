using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WebUtilities.Controls;

namespace LinkBi1.Classes.LinkBiServerConnections
{
    public class LinkBiServerConnectionFTP : LinkBiServerConnection
    {
        #region Properties

        /// <summary>
        /// Gets or sets the FTP server.
        /// </summary>
        public string Server
        {
            get
            {
                return base.GetValue("Server");
            }
            set
            {
                base.SetValue("Server", value);
            }
        }

        /// <summary>
        /// Gets or sets the FTP username.
        /// </summary>
        public string Username
        {
            get
            {
                return base.GetValue("Username");
            }
            set
            {
                base.SetValue("Username", value);
            }
        }

        /// <summary>
        /// Gets or sets the FTP password.
        /// </summary>
        public string Password
        {
            get
            {
                return base.GetValue("Password");
            }
            set
            {
                base.SetValue("Password", value);
            }
        }

        #endregion


        #region Constructor

        public LinkBiServerConnectionFTP(LinkBiDefinition owner, XmlNode xmlNode)
            : base(owner, xmlNode)
        { }

        #endregion


        #region Methods

        public override TableRow[] Render()
        {
            TableRow[] result = new TableRow[3];

            TableCell tableCellServerTitle = new TableCell();
            TableCell tableCellServerValue = new TableCell();

            tableCellServerTitle.Text = base.LanguageManager.GetText("LinkBiServerConnectionFTPServer");

            TextBox txtServer = new TextBox();
            txtServer.Text = this.Server;

            txtServer.Attributes.Add("onchange", string.Format(
                "UpdateServerConnectionProperty('{0}' ,'{1}', '{2}', '{3}', this.value);",
                this.XmlNode.Attributes["Id"].Value,
                this.Owner.FileName.Replace("\\", "/"),
                this.XmlNode.GetXPath(),
                "Server"
            ));

            tableCellServerValue.Controls.Add(txtServer);

            result[0] = new TableRow();
            result[0].Cells.Add(tableCellServerTitle);
            result[0].Cells.Add(tableCellServerValue);


            TableCell tableCellUsernameTitle = new TableCell();
            TableCell tableCellUsernameValue = new TableCell();

            tableCellUsernameTitle.Text = base.LanguageManager.GetText("LinkBiServerConnectionFTPUsername");

            TextBox txtUsername = new TextBox();
            txtUsername.Text = this.Username;

            txtUsername.Attributes.Add("onchange", string.Format(
                "UpdateServerConnectionProperty('{0}' ,'{1}', '{2}', '{3}', this.value);",
                this.XmlNode.Attributes["Id"].Value,
                this.Owner.FileName.Replace("\\", "/"),
                this.XmlNode.GetXPath(),
                "Username"
            ));

            tableCellUsernameValue.Controls.Add(txtUsername);

            result[1] = new TableRow();
            result[1].Cells.Add(tableCellUsernameTitle);
            result[1].Cells.Add(tableCellUsernameValue);

            TableCell tableCellPasswordTitle = new TableCell();
            TableCell tableCellPasswordValue = new TableCell();

            tableCellPasswordTitle.Text = base.LanguageManager.GetText("LinkBiServerConnectionFTPPassword");

            TextBox txtPassword = new TextBox();
            txtPassword.Text = this.Password;

            txtPassword.Attributes.Add("onchange", string.Format(
                "UpdateServerConnectionProperty('{0}' ,'{1}', '{2}', '{3}', this.value);",
                this.XmlNode.Attributes["Id"].Value,
                this.Owner.FileName.Replace("\\", "/"),
                this.XmlNode.GetXPath(),
                "Password"
            ));

            tableCellPasswordValue.Controls.Add(txtPassword);

            result[2] = new TableRow();
            result[2].Cells.Add(tableCellPasswordTitle);
            result[2].Cells.Add(tableCellPasswordValue);

            return result;
        }

        public override bool Deploy(string fileName)
        {
            try
            {
                string uri = this.Server;

                if (!uri.StartsWith("ftp://"))
                    uri = "ftp://" + uri;

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential(this.Username, this.Password);

                // Copy the contents of the file to the request stream.
                StreamReader sourceStream = new StreamReader(fileName);
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
                request.ContentLength = fileContents.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                base.Outdated = false;
                base.LatestDeploy = DateTime.Now;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool IsValid()
        {
            try
            {
                string uri = this.Server;

                if (!uri.StartsWith("ftp://"))
                    uri = "ftp://" + uri;

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential(this.Username, this.Password);

                Stream requestStream = request.GetRequestStream();
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
