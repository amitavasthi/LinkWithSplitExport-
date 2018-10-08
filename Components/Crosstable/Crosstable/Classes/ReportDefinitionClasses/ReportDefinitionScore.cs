using System;
using System.Collections.Generic;
using System.Xml;

namespace Crosstables.Classes.ReportDefinitionClasses
{
    public abstract class ReportDefinitionScore
    {
        #region Properties

        public ReportDefinitionScoreProperties Properties { get; set; }

        //public List<object[]> Data { get; set; }

        /// <summary>
        /// Gets or sets the variable the category belongs to.
        /// </summary>
        public ReportDefinitionVariable Variable { get; set; }

        /// <summary>
        /// Gets or sets the xml node which contains the category definition.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets the significance-test letter of the category.
        /// </summary>
        public string SignificanceLetter
        {
            get
            {
                return this.Properties.GetValue<string>("SignificantLetter", null, "");
            }
            set
            {
                this.Properties.SetValue<string>("SignificantLetter", value);
            }
        }

        /// <summary>
        /// Checks if the score is hidden
        /// </summary>
        public bool Hidden
        {
            get
            {
                return !this.Properties.GetValue<bool>("Enabled", bool.Parse, true);
            }
            set
            {
                this.Properties.SetValue<bool>("Enabled", (!value));
            }
        }

        /// <summary>
        /// Checks if the score is persistent
        /// </summary>
        public bool Persistent
        {
            get
            {
                return this.Properties.GetValue<bool>("Persistent", bool.Parse, null);
            }
        }

        /// <summary>
        /// Checks if the score is persistent
        /// </summary>
        public bool ExceededCategoryLimit
        {
            get
            {
                return this.Properties.GetValue<bool>("ExceededCategoryLimit", bool.Parse, false);
            }
            set
            {
                this.Properties.SetValue<bool>("ExceededCategoryLimit", value);
            }
        }

        /// <summary>
        /// Gets or sets if the score has no values.
        /// </summary>
        public bool HasValues
        {
            get
            {
                return this.Properties.GetValue<bool>("HasValues", bool.Parse, true);
            }
            set
            {
                this.Properties.SetValue<bool>("HasValues", value);
            }
        }


        /// <summary>
        /// Gets or sets the identity of the score.
        /// </summary>
        public abstract Guid Identity { get; }

        /// <summary>
        /// Gets the label of the score.
        /// </summary>
        public abstract string _Label { get; }
        public abstract string _LabelValues { get; }

        public string Label
        {
            get
            {
                int idLanguage = this.Variable.Owner.Settings.IdLanguage;

                if (this.XmlNode.Attributes["Label" + idLanguage] == null)
                {
                    string result = this._Label;

                    this.XmlNode.AddAttribute("Label" + idLanguage, result);

                    return result;
                }
                else
                {
                    return this.XmlNode.Attributes["Label" + idLanguage].Value;
                }
            }
        }

        public string LabelValues
        {
            get
            {
                int idLanguage = this.Variable.Owner.Settings.IdLanguage;

                if (this.XmlNode.Attributes["Label" + idLanguage] == null)
                {
                    return null;
                }
                else
                {
                    return this.XmlNode.Attributes["Label" + idLanguage].Value;
                }
            }
        }

        /// <summary>
        /// Gets the name of the score.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the column span of the score.
        /// </summary>
        public abstract int ColumnSpan { get; }

        /// <summary>
        /// Gets the row span of the score.
        /// </summary>
        public abstract int RowSpan { get; }

        /// <summary>
        /// Gets the row span of the score.
        /// </summary>
        public abstract int RowSpanMean { get; }

        /// <summary>
        /// Gets the factor of the score.
        /// </summary>
        public abstract double Factor { get; }

        public abstract bool ScoreBase { get; }

        public abstract string Equation { get; set; }

        public List<string> CommandTexts { get; set; }

        #endregion


        #region Constructor

        public ReportDefinitionScore(ReportDefinitionVariable variable, XmlNode xmlNode)
        {
            this.Properties = new ReportDefinitionScoreProperties(this);
            this.Variable = variable;
            this.XmlNode = xmlNode;
        }

        #endregion


        #region Methods

        public abstract List<string> BuildCommandTexts();

        #endregion
    }

    public class ReportDefinitionScoreProperties
    {
        #region Properties

        public Dictionary<string, object> Properties { get; set; }

        public ReportDefinitionScore Score { get; set; }

        #endregion


        #region Constructor

        public ReportDefinitionScoreProperties(ReportDefinitionScore score)
        {
            this.Properties = new Dictionary<string, object>();
            this.Score = score;
        }

        #endregion


        #region Methods

        public T GetValue<T>(string key, ParseSettingType<T> Parse, object defaultValue = null)
        {
            lock (this.Properties)
            {
                if (!this.Properties.ContainsKey(key))
                {
                    if (this.Score.XmlNode.Attributes[key] != null)
                    {
                        if (Parse != null)
                            this.Properties.Add(key, Parse(this.Score.XmlNode.Attributes[key].Value));
                        else
                            this.Properties.Add(key, this.Score.XmlNode.Attributes[key].Value);
                    }
                    else
                    {
                        this.Properties.Add(key, defaultValue);

                        if (defaultValue != null)
                            this.Score.XmlNode.AddAttribute(key, defaultValue);
                    }
                }
            }

            if (this.Properties[key] == null)
                return default(T);

            return (T)this.Properties[key];
        }

        public void SetValue<T>(string key, T value)
        {
            if (this.Score.XmlNode.Attributes[key] == null)
            {
                this.Score.XmlNode.AddAttribute(key, value.ToString());
            }
            else
            {
                this.Score.XmlNode.Attributes[key].Value = value.ToString();
            }

            lock (this.Properties)
            {
                if (!this.Properties.ContainsKey(key))
                    this.Properties.Add(key, value);

                this.Properties[key] = value;
            }
        }

        #endregion
    }
}
