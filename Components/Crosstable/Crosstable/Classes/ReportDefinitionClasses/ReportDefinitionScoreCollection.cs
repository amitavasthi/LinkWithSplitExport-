using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstables.Classes.ReportDefinitionClasses
{
    public class ReportDefinitionScoreCollection
    {
        #region Properties

        /// <summary>
        /// Gets or sets the items of the collection.
        /// </summary>
        public List<ReportDefinitionScore> Items { get; set; }

        public Dictionary<Guid, int> Mapping { get; set; }

        #endregion


        #region Constructor

        public ReportDefinitionScoreCollection()
        {
            this.Items = new List<ReportDefinitionScore>();
            this.Mapping = new Dictionary<Guid, int>();
        }

        #endregion


        #region Methods

        public void Add(ReportDefinitionScore item)
        {
            this.Items.Add(item);

            if (!this.Mapping.ContainsKey(item.Identity))
                this.Mapping.Add(item.Identity, this.Items.Count - 1);
        }

        public IEnumerator<ReportDefinitionScore> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion


        #region Operators

        public ReportDefinitionScore this[int index]
        {
            get
            {
                return this.Items.FindAll(x => x.Hidden == false).ElementAt(index);
            }
        }

        #endregion
    }
}
