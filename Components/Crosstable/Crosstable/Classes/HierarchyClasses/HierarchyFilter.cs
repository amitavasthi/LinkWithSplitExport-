using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Crosstables.Classes.HierarchyClasses
{
    public class HierarchyFilter
    {
        #region Properties

        /// <summary>
        /// Gets or sets if the variable ids has been loaded.
        /// </summary>
        public bool IsLoaded { get; set; }

        /// <summary>
        /// Gets or sets a list of taxonomy variable ids which are
        /// available for the current hierarchy context.
        /// </summary>
        public Dictionary<Guid, object> TaxonomyVariables { get; set; }

        /// <summary>
        /// Gets or sets a list of taxonomy category ids which are
        /// available for the current hierarchy context.
        /// </summary>
        public Dictionary<Guid, object> TaxonomyCategories { get; set; }

        /// <summary>
        /// Gets or sets a list of variable ids which are
        /// available for the current hierarchy context.
        /// </summary>
        public Dictionary<Guid, object> Variables { get; set; }

        /// <summary>
        /// Gets or sets a list of hierarchy ids
        /// on which should be filtered on.
        /// </summary>
        public List<Guid> Hierarchies { get; set; }

        public DatabaseCore.Core Core
        {
            get
            {
                return (DatabaseCore.Core)HttpContext.Current.Session["Core"];
            }
        }
        public Guid IdUser
        {
            get
            {
                return (Guid)HttpContext.Current.Session["User"];
            }
        }

        public string FileName { get; set; }

        #endregion


        #region Constructor

        public HierarchyFilter(string fileName)
        {
            this.FileName = fileName;
            this.TaxonomyVariables = new Dictionary<Guid, object>();
            this.TaxonomyCategories = new Dictionary<Guid, object>();
            this.Variables = new Dictionary<Guid, object>();
            this.Hierarchies = new List<Guid>();

            this.IsLoaded = false;
        }

        #endregion


        #region Methods

        private void Parse(string fileName)
        {
            // Create a new xml document that
            // contains the report definition.
            XmlDocument document = new XmlDocument();

            // Load the contents of the report
            // definition file into the xml document.
            document.Load(fileName);

            XmlNodeList hierarchies = document.SelectNodes("//Workflow/Selection/HierarchyFilter/*");

            foreach (XmlNode hierarchy in hierarchies)
            {
                this.Hierarchies.Add(Guid.Parse(hierarchy.Attributes["Id"].Value));
            }

            hierarchies = document.DocumentElement.SelectNodes("HierarchyFilter/Hierarchy");

            foreach (XmlNode hierarchy in hierarchies)
            {
                this.Hierarchies.Add(Guid.Parse(hierarchy.Attributes["Id"].Value));
            }
        }


        public void Load()
        {
            if (this.FileName != null)
                Parse(this.FileName);

            // Load the taxonomy variable ids.
            LoadTaxonomyVariables();

            // Load the taxonomy category ids.
            LoadTaxonomyCategories();

            // Load the variable ids.
            LoadVariables();

            this.IsLoaded = true;
        }

        private void LoadTaxonomyVariables()
        {
            this.TaxonomyVariables = new Dictionary<Guid, object>();

            List<object[]> items;

            if (this.Hierarchies.Count == 0)
            {
                items = this.Core.TaxonomyVariables.ExecuteReader(string.Format(
                    "SELECT Id FROM TaxonomyVariables WHERE Id IN(SELECT IdTaxonomyVariable FROM TaxonomyVariableHierarchies WHERE IdHierarchy IN (SELECT IdHierarchy FROM WorkgroupHierarchies " +
                    "WHERE IdWorkgroup IN (SELECT IdWorkgroup FROM UserWorkgroups WHERE IdUser='{0}')) OR IdHierarchy IN (SELECT Id FROM Hierarchies WHERE IdHierarchy IS NULL))",
                    this.IdUser
                ));
            }
            else
            {
                items = this.Core.TaxonomyVariables.ExecuteReader(string.Format(
                    "SELECT Id FROM TaxonomyVariables WHERE Id IN (SELECT IdTaxonomyVariable FROM TaxonomyVariableHierarchies WHERE IdHierarchy IN ({0}) OR IdHierarchy IN (SELECT Id FROM Hierarchies WHERE IdHierarchy IS NULL))",
                    string.Join(",", this.Hierarchies.Select(x => "'" + x + "'"))
                ));
            }

            foreach (object[] item in items)
            {
                if (!this.TaxonomyVariables.ContainsKey((Guid)item[0]))
                    this.TaxonomyVariables.Add((Guid)item[0], null);
            }
        }

        private void LoadTaxonomyCategories()
        {
            this.TaxonomyCategories = new Dictionary<Guid, object>();

            List<object[]> items;

            if (this.Hierarchies.Count == 0)
            {
                items = this.Core.TaxonomyCategories.ExecuteReader(string.Format(
                    "SELECT IdTaxonomyCategory FROM TaxonomyCategoryHierarchies WHERE IdHierarchy IN (SELECT IdHierarchy FROM WorkgroupHierarchies " +
                    "WHERE IdWorkgroup IN (SELECT IdWorkgroup FROM UserWorkgroups WHERE IdUser='{0}')) OR IdHierarchy IN (SELECT Id FROM Hierarchies WHERE IdHierarchy IS NULL)",
                    this.IdUser
                ));
            }
            else
            {
                items = this.Core.TaxonomyCategories.ExecuteReader(string.Format(
                    "SELECT IdTaxonomyCategory FROM TaxonomyCategoryHierarchies WHERE IdHierarchy IN ({0}) OR IdHierarchy IN (SELECT Id FROM Hierarchies WHERE IdHierarchy IS NULL)",
                    string.Join(",", this.Hierarchies.Select(x => "'" + x + "'"))
                ));
            }

            foreach (object[] item in items)
            {
                if (!this.TaxonomyCategories.ContainsKey((Guid)item[0]))
                    this.TaxonomyCategories.Add((Guid)item[0], null);
            }
        }

        private void LoadVariables()
        {
            this.Variables = new Dictionary<Guid, object>();

            List<object[]> items;

            if (this.Hierarchies.Count == 0)
            {
                items = this.Core.Variables.ExecuteReader(string.Format(
                    "SELECT Id FROM Variables WHERE IdStudy IN (SELECT Id FROM Studies WHERE IdHierarchy IN (SELECT IdHierarchy FROM WorkgroupHierarchies " +
                    "WHERE IdWorkgroup IN (SELECT IdWorkgroup FROM UserWorkgroups WHERE IdUser='{0}')) OR IdHierarchy IN (SELECT Id FROM Hierarchies WHERE IdHierarchy IS NULL))",
                    this.IdUser
                ));
            }
            else
            {
                items = this.Core.Variables.ExecuteReader(string.Format(
                    "SELECT Id FROM Variables WHERE IdStudy IN (SELECT Id FROM Studies WHERE IdHierarchy IN ({0}) OR IdHierarchy IN (SELECT Id FROM Hierarchies WHERE IdHierarchy IS NULL))",
                    string.Join(",", this.Hierarchies.Select(x => "'" + x + "'"))
                ));
            }

            foreach (object[] item in items)
            {
                if (!this.Variables.ContainsKey((Guid)item[0]))
                    this.Variables.Add((Guid)item[0], null);
            }
        }

        public void Clear()
        {
            this.Hierarchies = new List<Guid>();
            this.TaxonomyCategories = new Dictionary<Guid, object>();
            this.TaxonomyVariables = new Dictionary<Guid, object>();
            this.Variables = new Dictionary<Guid, object>();
            this.IsLoaded = false;
        }

        #endregion
    }
}