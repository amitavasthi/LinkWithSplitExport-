using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace AlexaAPI.Classes
{
    public class Users
    {
        #region Properties

        public XmlDocument Document { get; set; }

        public Dictionary<string, User> Items { get; set; }

        #endregion


        #region Constructor

        public Users()
        {
            this.Document = new XmlDocument();
            this.Document.Load(Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "App_Data",
                "Users.xml"
            ));

            this.Load();
        }

        #endregion


        #region Methods

        private void Load()
        {
            this.Items = new Dictionary<string, Classes.User>();

            XmlNodeList xmlNodes = this.Document.DocumentElement.SelectNodes("User");

            User user;
            foreach (XmlNode xmlNode in xmlNodes)
            {
                user = new Classes.User();
                user.Id = Guid.Parse(xmlNode.Attributes["IdUser"].Value);
                user.Client = xmlNode.Attributes["Client"].Value;
                user.AmazonId = xmlNode.Attributes["AmazonId"].Value;

                this.Items.Add(user.AmazonId, user);
            }
        }

        #endregion


        #region Event Handlers

        #endregion
    }

    public struct User
    {
        public Guid Id;
        public string Client;
        public string AmazonId;
    }
}