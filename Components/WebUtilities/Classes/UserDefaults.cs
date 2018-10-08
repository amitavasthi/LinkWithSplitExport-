using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WebUtilities
{
    public class UserDefaults
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path to the
        /// user defaults definition file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the xml document that
        /// contains the user defaults definitions.
        /// </summary>
        public XmlDocument Document { get; set; }

        /// <summary>
        /// Gets or sets the dictionary that holds the user's default settings.
        /// </summary>
        public Dictionary<string, string> Defaults { get; set; }

        #endregion


        #region Constructor

        public UserDefaults(string fileName)
        {
            this.FileName = fileName;

            // Check if the user defaults directory for the client exists.
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                // Create the user defaults directory for the client.
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }

            // Check if the user has already defaults defined.
            if (!File.Exists(fileName))
            {
                // Create a new, empty user defaults definition file.
                File.WriteAllText(fileName, "<Defaults></Defaults>");
            }

            // Create a new dictionary that
            // holds the user's default settings.
            this.Defaults = new Dictionary<string, string>();

            // Parse the user defaults
            // from the definition xml file.
            this.Parse();
        }

        #endregion


        #region Methods

        /// <summary>
        /// Parses the user defaults from the definition xml file.
        /// </summary>
        private void Parse()
        {
            // Create a new xml document that contains the user defaults.
            this.Document = new XmlDocument();

            // Load the contents of the user defaults
            // definition file into the xml document.
            this.Document.Load(this.FileName);

            // Run through all nodes that define a user default.
            foreach (XmlNode xmlNode in this.Document.SelectNodes("Defaults/Default"))
            {
                // Add the parsed user default to the dictionary.
                this.Defaults.Add(
                    xmlNode.Attributes["Name"].Value,
                    xmlNode.InnerXml
                );
            }
        }

        #endregion


        #region Operators

        /// <summary>
        /// Returns the value of a specified user default setting.
        /// </summary>
        /// <param name="key">
        /// The name of the user default setting.
        /// </param>
        /// <param name="defaultValue">
        /// The default value that is used if
        /// the default is not defined yet.
        /// </param>
        /// <returns></returns>
        public string this[string key, string defaultValue = null]
        {
            get
            {
                // Check if the default is defined.
                if (!this.Defaults.ContainsKey(key))
                {
                    if (defaultValue == null)
                        return null;

                    // Write the user default to the
                    // user defaults definition file.
                    this.Document.DocumentElement.InnerXml += string.Format(
                        "<Default Name=\"{0}\">{1}</Default>",
                        key,
                        defaultValue
                    );

                    // Saves the changes to the user
                    // default definition file.
                    this.Save();

                    // Add the new user default to the dictionary.
                    this.Defaults.Add(key, defaultValue);
                }

                // Return the default value of the user default.
                return this.Defaults[key];
            }
            set
            {
                // Check if the default is defined.
                if (!this.Defaults.ContainsKey(key))
                {
                    // Write the user default to the
                    // user defaults definition document.
                    this.Document.DocumentElement.InnerXml += string.Format(
                        "<Default Name=\"{0}\">{1}</Default>",
                        key,
                        value
                    );

                    // Add the new user default to the dictionary.
                    this.Defaults.Add(key, value);
                }
                else
                {
                    // Write the new user default to the
                    // user defaults definition document.
                    this.Document.DocumentElement.SelectSingleNode(string.Format(
                        "Default[@Name=\"{0}\"]",
                        key
                    )).InnerXml = value;

                    // Set the new user default in the dictionary.
                    this.Defaults[key] = value;
                }

                // Saves the changes to the user
                // default definition file.
                this.Save();
            }
        }

        /// <summary>
        /// Saves the changes to the user
        /// default definition file.
        /// </summary>
        public void Save()
        {
            // Write the contents of the xml document
            // to the user defaults definition file.
            this.Document.Save(this.FileName);
        }

        #endregion
    }
}
