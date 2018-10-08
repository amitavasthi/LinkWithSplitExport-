using System;
using System.Collections.Generic;
using System.Xml;

namespace Crosstables.Classes
{
    public class BaseReportSettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets the setting values;
        /// </summary>
        public Dictionary<string, object> Values { get; set; }

        /// <summary>
        /// Gets or sets the owning report definition.
        /// </summary>
        public BaseReportDefinition Owner { get; set; }

        /// <summary>
        /// Gets or sets the xml node that contains the setting definitions.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets the id of the language the labels should appear in.
        /// </summary>
        public int IdLanguage
        {
            get
            {
                return this.GetValue<int>("IdLanguage", int.Parse, "2057");
            }
            set
            {
                this.SetValue("IdLanguage", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets if the unweighted base should be displayed.
        /// </summary>
        public int WeightMissingValue
        {
            get
            {
                return this.GetValue<int>("WeightMissingValue", int.Parse, "1");
            }
            set
            {
                this.SetValue("WeightMissingValue", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets if the unweighted base should be displayed.
        /// </summary>
        public bool DisplayUnweightedBase
        {
            get
            {
                return this.GetValue<bool>("DisplayUnweightedBase", bool.Parse, "False");
            }
            set
            {
                this.SetValue("DisplayUnweightedBase", value.ToString());
            }
        }

        public bool DisplayEffectiveBase
        {
            get
            {
                return this.GetValue<bool>("DisplayEffectiveBase", bool.Parse, "False");
            }
            set
            {
                this.SetValue("DisplayEffectiveBase", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the display type of the report.
        /// </summary>
        public DisplayType DisplayType
        {
            get
            {
                return this.GetValue<DisplayType>("DisplayType", delegate (string value)
                {
                    return (DisplayType)Enum.Parse(
                        typeof(DisplayType),
                        value
                    );
                }, "Crosstable");
            }
            set
            {
                this.SetValue("DisplayType", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets if the data check should be enabled when searching for variables.
        /// </summary>
        public bool DataCheckEnabled
        {
            get
            {
                return this.GetValue("DataCheckEnabled", bool.Parse, "True");
            }
            set
            {
                this.SetValue("DataCheckEnabled", value.ToString());
            }
        }

        #endregion


        #region Constructor

        public BaseReportSettings(BaseReportDefinition owner, XmlNode xmlNode)
        {
            this.Values = new Dictionary<string, object>();
            this.Owner = owner;
            this.XmlNode = xmlNode;

            // Run through all child xml nodes of the settings xml node.
            /*foreach (XmlNode xmlNodeSetting in xmlNode.ChildNodes)
            {
                if (this.Values.ContainsKey(xmlNodeSetting.Attributes["Name"].Value))
                    continue;

                this.Values.Add(
                    xmlNodeSetting.Attributes["Name"].Value,
                    xmlNodeSetting.InnerXml
                );
            }*/
        }

        #endregion


        #region Methods

        protected T GetValue<T>(string name, ParseSettingType<T> Parse, string defaultValue = "")
        {
            if (!this.Values.ContainsKey(name))
            {
                XmlNode xmlNode = this.XmlNode.SelectSingleNode(string.Format(
                    "*[@Name=\"{0}\"]",
                    name
                ));

                if (xmlNode == null)
                {
                    this.XmlNode.InnerXml += string.Format(
                        "<Setting Name=\"{0}\">{1}</Setting>",
                        name,
                        defaultValue
                    );

                    if (Parse != null)
                        this.Values.Add(name, Parse(defaultValue));
                    else
                        this.Values.Add(name, defaultValue);

                    //this.Owner.Save();
                }
                else
                {                

                    if (Parse != null)
                        this.Values.Add(name, Parse(xmlNode.InnerXml));
                    else
                        this.Values.Add(name, xmlNode.InnerXml);
                }

                //this.Owner.Save();

                return GetValue<T>(name, Parse, defaultValue);
            }

            //  return (T)this.Values[name];
            return (T)System.ComponentModel.TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(this.Values[name].ToString());
        }

        public void SetValue(string name, string value)
        {
            XmlNode xmlNode = this.XmlNode.SelectSingleNode(string.Format(
                "Setting[@Name=\"{0}\"]",
                name
            ));

            if (xmlNode == null)
            {
                this.XmlNode.InnerXml += string.Format(
                    "<Setting Name=\"{0}\">{1}</Setting>",
                    name,
                    value
                );

                this.Values[name] = value;
                this.Owner.Save();

                return;
            }

            xmlNode.InnerXml = value;
            this.Values[name] = value;
        }

        #endregion
    }

    public delegate T ParseSettingType<T>(string value);
    public delegate object ParseSettingType(string value);
}
