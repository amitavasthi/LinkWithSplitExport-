using DataCore.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Crosstables.Classes.ReportDefinitionClasses.Collections
{
    public class WeightingFilterCollection
    {
        #region Properties

        /// <summary>
        /// Gets or sets the owning report definition.
        /// </summary>
        public BaseReportDefinition Owner { get; set; }

        public DatabaseCore.Core Core { get; set; }

        private Dictionary<Guid, WeightingFilter> Items { get; set; }

        /// <summary>
        /// Gets or sets the xml node that contains
        /// the weighting variable definitions.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        public int Length
        {
            get
            {
                return this.Items.Count;
            }
        }

        private Guid? _DefaultWeighting;

        public Guid? DefaultWeighting
        {
            get { return _DefaultWeighting; }
            set
            {
                if (this.XmlNode != null)
                {
                    if (value == null)
                    {
                        if (this.XmlNode.Attributes["DefaultWeighting"] != null)
                            this.XmlNode.Attributes.RemoveNamedItem("DefaultWeighting");
                    }
                    else if (this.XmlNode.Attributes["DefaultWeighting"] == null)
                        this.XmlNode.AddAttribute("DefaultWeighting", value.Value.ToString());
                    else
                        this.XmlNode.Attributes["DefaultWeighting"].Value = value.Value.ToString();
                }

                _DefaultWeighting = value;
            }
        }


        public Dictionary<Guid, double> Respondents { get; set; }

        public bool IsTaxonomy { get; set; }

        #endregion


        #region Constructor

        public WeightingFilterCollection(BaseReportDefinition owner, DatabaseCore.Core core, XmlNode xmlNode)
        {
            this.Owner = owner;
            this.Core = core;
            this.XmlNode = xmlNode;
            this.IsTaxonomy = true;
            this.Items = new Dictionary<Guid, WeightingFilter>();

            if (this.XmlNode != null)
            {
                // Run through all weighting variable xml nodes.
                foreach (XmlNode xmlNodeWeightingVariable in this.XmlNode.SelectNodes("Operator"))
                {
                    // Create a new weighting filter by the xml node.
                    WeightingFilter weightingFilter = new WeightingFilter(this, xmlNodeWeightingVariable);

                    // Add the weighting filter to the collection's items
                    this.Items.Add(
                        weightingFilter.IdWeightingVariable,
                        weightingFilter
                    );

                    /*if (weightingFilter.IdCategory == new Guid())
                        this.DefaultWeighting = weightingFilter.WeightingVariable;*/
                }

                if (this.XmlNode.Attributes["DefaultWeighting"] != null &&
                        this.XmlNode.Attributes["DefaultWeighting"].Value != "")
                    this._DefaultWeighting = Guid.Parse(this.XmlNode.Attributes["DefaultWeighting"].Value);

                if (this.XmlNode.Attributes["IsTaxonomy"] != null)
                    this.IsTaxonomy = bool.Parse(this.XmlNode.Attributes["IsTaxonomy"].Value);
            }
        }

        #endregion


        #region Methods

        public void LoadRespondents(Data filter)
        {
            /*DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                this.Owner.Settings.AggregateNonQAData,
                this.Core,
                null
            );*/
            DataCore.Classes.StorageMethods.Database storageMethod = new DataCore.Classes.StorageMethods.Database(
                this.Core,
                null
            );

            this.Respondents = new Dictionary<Guid, double>();

            if (this.DefaultWeighting != null)
            {

                //Guid idDefaultWeighting = (Guid)this.Core.Variables.GetValue("Id", "Name", this.WeightingVariables.DefaultWeighting);
                Guid idDefaultWeighting = this.DefaultWeighting.Value;

                if (this.DefaultWeighting != null)
                {
                    Data respondents = storageMethod.GetRespondentsNumeric(
                        idDefaultWeighting,
                        this.IsTaxonomy,
                        this.Core.CaseDataLocation,
                        filter,
                        null
                    );

                    // Run through all respondents.
                    foreach (KeyValuePair<Guid, double[]> respondent in respondents.Responses)
                    {
                        this.Respondents.Add(respondent.Key, respondent.Value[0]);
                    }
                }
            }

            foreach (WeightingFilter weightingFilter in this.ToArray())
            {
                Data respondents = weightingFilter.FilterCategoryOperator.GetRespondents(storageMethod);

                respondents = storageMethod.GetRespondentsNumeric(
                    weightingFilter.IdWeightingVariable,
                    true,
                    this.Owner.Core.CaseDataLocation,
                    respondents
                );

                Dictionary<Guid, double> respondentsWeights = new Dictionary<Guid, double>();

                // Run through all respondent ids.
                foreach (Guid idRespondent in respondents.Responses.Keys)
                {
                    // Get the respondent's weight.
                    double weight = respondents.Responses[idRespondent][0];

                    respondentsWeights.Add(idRespondent, weight);
                }

                weightingFilter.Respondents = respondentsWeights;
            }
        }

        List<WeightingFilter>.Enumerator GetEnumerator()
        {
            return this.Items.Values.ToList().GetEnumerator();
        }

        public void Add(Guid idCategory, string name)
        {
            XmlNode xmlNode = this.XmlNode.OwnerDocument.CreateElement("WeightingFilter");

            xmlNode.AddAttribute("IdCategory", idCategory);
            xmlNode.AddAttribute("WeightingVariable", name);

            this.XmlNode.AppendChild(xmlNode);

            this.Items.Add(idCategory, new WeightingFilter(this, xmlNode));

            this.Owner.Save();
        }

        public void Delete(Guid idCategory)
        {
            XmlNode xmlNode = this.XmlNode.SelectSingleNode(string.Format(
                "WeightingFilter[@IdCategory=\"{0}\"]",
                idCategory
            ));

            this.XmlNode.RemoveChild(xmlNode);

            this.Items.Remove(idCategory);
        }

        public WeightingFilter[] ToArray()
        {
            return this.Items.Values.ToArray();
        }

        #endregion


        #region Operators

        public WeightingFilter this[int index]
        {
            get
            {
                return this.Items.ElementAt(index).Value;
            }
            set
            {
                this.Items[this.Items.ElementAt(index).Key] = value;
            }
        }

        public WeightingFilter this[Guid idCategory]
        {
            get
            {
                return this.Items[idCategory];
            }
            set
            {
                this.Items[idCategory] = value;
            }
        }

        #endregion
    }

    public class WeightingFilter
    {
        #region Properties

        public WeightingFilterCollection Owner { get; set; }

        /// <summary>
        /// Gets or sets the xml node of the weighting filter
        /// that contains the weighting filter definition.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        public FilterCategoryOperator FilterCategoryOperator { get; set; }

        /// <summary>
        /// Gets or sets the id of the variable which is used for the weighting.
        /// </summary>
        public Guid IdWeightingVariable
        {
            get
            {
                return Guid.Parse(this.GetValue("WeightingVariable"));
            }
            set
            {
                this.SetValue("WeightingVariable", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a array of respondents
        /// that match to the weighting filter.
        /// </summary>
        public Dictionary<Guid, double> Respondents { get; set; }

        #endregion


        #region Constructor

        public WeightingFilter(WeightingFilterCollection owner, XmlNode xmlNode)
        {
            this.Owner = owner;

            // Set the weighting filter's xml node.
            this.XmlNode = xmlNode;

            // Parse the weighting filter
            // definitions from the xml node.
            Parse();
        }

        #endregion


        #region Methods

        private void Parse()
        {
            this.FilterCategoryOperator = new FilterCategoryOperator(
                this.Owner,
                this.XmlNode,
                0,
                ""
            );
        }


        private string GetValue(string name)
        {
            return this.XmlNode.Attributes[name].Value;
        }

        private void SetValue(string name, string value)
        {
            if (this.XmlNode.Attributes[name] == null)
                this.XmlNode.AddAttribute(name, value);
            else
                this.XmlNode.Attributes[name].Value = value;
        }

        #endregion
    }
}
