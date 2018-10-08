using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace LinkAdminService.Classes
{
    public class InstanceCollection
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path to
        /// the instance root directory.
        /// </summary>
        public string Source { get; set; }

        public Dictionary<string, Instance> Instances { get; set; }

        #endregion


        #region Constructor

        public InstanceCollection(string source)
        {
            this.Instances = new Dictionary<string, Instance>();
            this.Source = source;

            this.Parse();
        }

        #endregion


        #region Methods

        private void Parse()
        {
            foreach (string directory in Directory.GetDirectories(this.Source))
            {
                if (!File.Exists(Path.Combine(
                    directory,
                    "App_Data",
                    "Clients.xml"
                )))
                    continue;

                Instance instance = new Instance(directory);

                if (this.Instances.ContainsKey(instance.Name))
                    continue;

                this.Instances.Add(instance.Name, instance);
            }
        }

        public string ToXml()
        {
            StringBuilder result = new StringBuilder();

            result.Append("<Instances>");

            foreach (Instance instance in this.Instances.Values)
            {
                result.Append(instance.ToXml());
            }

            result.Append("</Instances>");

            return result.ToString();
        }

        #endregion
    }
}