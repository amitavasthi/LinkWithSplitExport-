using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crosstables.Classes.HierarchyClasses
{
    public class HierarchyFilterCollection
    {
        #region Properties

        /// <summary>
        /// Gets or sets a dictionary of all loaded hierarchy filters by report id.
        /// </summary>
        private Dictionary<string, HierarchyFilter> Items { get; set; }

        public DatabaseCore.Core Core { get; set; }
        public Guid IdUser { get; set; }


        private HierarchyFilter defaultFilter;

        public HierarchyFilter Default
        {
            get
            {
                if (!defaultFilter.IsLoaded)
                    defaultFilter.Load();
                return defaultFilter;
            }
            set
            {
                defaultFilter = value;
            }
        }


        #endregion


        #region Constructor

        public HierarchyFilterCollection()
        {
            this.Items = new Dictionary<string, HierarchyFilter>();

            this.Default = new HierarchyFilter(null);
        }

        #endregion


        #region Methods

        #endregion


        #region Operators

        public HierarchyFilter this[string fileName, bool load = true]
        {
            get
            {
                fileName = fileName.Replace("\\", "/");

                if (!this.Items.ContainsKey(fileName))
                    this.Items.Add(fileName, new HierarchyFilter(fileName));

                if (this.Items[fileName].IsLoaded == false && load == true)
                    this.Items[fileName].Load();

                return this.Items[fileName];
            }
            set
            {
                fileName = fileName.Replace("\\", "/");

                if (!this.Items.ContainsKey(fileName))
                    this.Items.Add(fileName, value);
                else
                    this.Items[fileName] = value;
            }
        }

        #endregion
    }
}