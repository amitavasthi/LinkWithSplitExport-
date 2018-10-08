using DatabaseCore.BaseClasses;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore
{
    public class Core : BaseCore
    {
        #region Properties

        public ApplicationUtilities.Classes.CaseDataLocation CaseDataLocation { get; set; }

        /// <summary>
        /// Gets or sets the client name.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets the case data version of the client.
        /// </summary>
        public int CaseDataVersion { get; set; }

        #endregion


        #region Collections

        /// <summary>
        /// Gets or sets the responses collection.
        /// </summary>
        public ResponseCollection Responses { get; set; }

        /// <summary>
        /// Gets or sets the projects collection.
        /// </summary>
        public BaseCollection<Study> Studies { get; set; }

        /// <summary>
        /// Gets or sets the variables collection.
        /// </summary>
        public BaseCollection<Variable> Variables { get; set; }

        /// <summary>
        /// Gets or sets the category collection.
        /// </summary>
        public BaseCollection<Category> Categories { get; set; }

        /// <summary>
        /// Gets or sets the variable labels collection.
        /// </summary>
        public BaseCollection<VariableLabel> VariableLabels { get; set; }

        /// <summary>
        /// Gets or sets the category labels collection.
        /// </summary>
        public BaseCollection<CategoryLabel> CategoryLabels { get; set; }

        /// <summary>
        /// Gets or sets the respondent collection.
        /// </summary>
        public BaseCollection<Respondent> Respondents { get; set; }

        /// <summary>
        /// Gets or sets the hierachies collection.
        /// </summary>
        public BaseCollection<Hierarchy> Hierarchies { get; set; }

        /// <summary>
        /// Gets or sets the taxonomy chapters collection.
        /// </summary>
        public BaseCollection<TaxonomyChapter> TaxonomyChapters { get; set; }

        /// <summary>
        /// Gets or sets the taxonomy chapter labels collection.
        /// </summary>
        public BaseCollection<TaxonomyChapterLabel> TaxonomyChapterLabels { get; set; }

        /// <summary>
        /// Gets or sets the taxonomy variables collection.
        /// </summary>
        public BaseCollection<TaxonomyVariable> TaxonomyVariables { get; set; }

        /// <summary>
        /// Gets or sets the taxonomy variable labels collection.
        /// </summary>
        public BaseCollection<TaxonomyVariableLabel> TaxonomyVariableLabels { get; set; }

        /// <summary>
        /// Gets or sets the taxonomy variable hierarchies collection.
        /// </summary>
        public BaseCollection<TaxonomyVariableHierarchy> TaxonomyVariableHierarchies { get; set; }

        /// <summary>
        /// Gets or sets the taxonomy category link collection.
        /// </summary>
        public BaseCollection<TaxonomyCategoryLink> TaxonomyCategoryLinks { get; set; }

        /// <summary>
        /// Gets or sets the taxonomy category collection.
        /// </summary>
        public BaseCollection<TaxonomyCategory> TaxonomyCategories { get; set; }

        /// <summary>
        /// Gets or sets the taxonomy category labels collection.
        /// </summary>
        public BaseCollection<TaxonomyCategoryLabel> TaxonomyCategoryLabels { get; set; }

        /// <summary>
        /// Gets or sets the taxonomy category labels collection.
        /// </summary>
        public BaseCollection<TaxonomyCategoryHierarchy> TaxonomyCategoryHierarchies { get; set; }

        /// <summary>
        /// Gets or sets the client removal details collection.
        /// </summary>
        public BaseCollection<ClientRemovalDetail> ClientRemovalDetails { get; set; }

        /// <summary>
        /// Gets or sets the variable link collection.
        /// </summary>
        public BaseCollection<VariableLink> VariableLinks { get; set; }

        /// <summary>
        /// Gets or sets the category link collection.
        /// </summary>
        public BaseCollection<CategoryLink> CategoryLinks { get; set; }

        /// <summary>
        /// Gets or sets the qa log collection.
        /// </summary>
        public BaseCollection<QALog> QALogs { get; set; }

        /// <summary>
        /// Gets or sets the support module collection.
        /// </summary>
        public BaseCollection<SupportModule> SupportModule { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of core.
        /// </summary>
        /// <param name="databaseProvider">The provider string of the database.</param>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <param name="databaseProviderUserManagement">The provider string of the user management database.</param>
        /// <param name="connectionStringUserManagement">The connection string to the user management database.</param>
        public Core(string databaseProvider, string connectionString,
            string databaseProviderUserManagement, string connectionStringUserManagement, string[] synchServers)
            : base(synchServers)
        {
            this.DatabaseProvider = databaseProvider;
            this.ConnectionString = connectionString;

            this.Tables = new Dictionary<string, BaseCollection>();

            this.Responses = new ResponseCollection(this);

            this.Studies = new BaseCollection<Study>(this, "Studies", true);
            this.Variables = new BaseCollection<Variable>(this, "Variables", true);
            this.Categories = new BaseCollection<Category>(this, "Categories", true);
            this.VariableLabels = new BaseCollection<VariableLabel>(this, "VariableLabels", true);
            this.CategoryLabels = new BaseCollection<CategoryLabel>(this, "CategoryLabels", true);
            this.Respondents = new BaseCollection<Respondent>(this, "Respondents", false);
            this.Hierarchies = new BaseCollection<Hierarchy>(this, "Hierarchies", true);
            this.TaxonomyChapters = new BaseCollection<TaxonomyChapter>(this, "TaxonomyChapters", true);
            this.TaxonomyChapterLabels = new BaseCollection<TaxonomyChapterLabel>(this, "TaxonomyChapterLabels", true);
            this.TaxonomyVariables = new BaseCollection<TaxonomyVariable>(this, "TaxonomyVariables", true);
            this.TaxonomyCategoryLinks = new BaseCollection<TaxonomyCategoryLink>(this, "TaxonomyCategoryLinks", true);
            this.TaxonomyCategories = new BaseCollection<TaxonomyCategory>(this, "TaxonomyCategories", true);
            this.TaxonomyVariableLabels = new BaseCollection<TaxonomyVariableLabel>(this, "TaxonomyVariableLabels", true);
            this.TaxonomyVariableHierarchies = new BaseCollection<TaxonomyVariableHierarchy>(this, "TaxonomyVariableHierarchies", true);
            this.TaxonomyCategoryLabels = new BaseCollection<TaxonomyCategoryLabel>(this, "TaxonomyCategoryLabels", true);
            this.TaxonomyCategoryHierarchies = new BaseCollection<TaxonomyCategoryHierarchy>(this, "TaxonomyCategoryHierarchies", true);
            this.ClientRemovalDetails = new BaseCollection<ClientRemovalDetail>(this, "ClientRemovalDetails", false);
            this.VariableLinks = new BaseCollection<VariableLink>(this, "VariableLinks", false);
            this.CategoryLinks = new BaseCollection<CategoryLink>(this, "CategoryLinks", false);
            this.QALogs = new BaseCollection<QALog>(this, "QALogs", false);
            this.SupportModule = new BaseCollection<SupportModule>(this, "SupportModule", false);
            // Create a new collection for the users.
            this.Users = new UserCollection(
                this,
                "Users",
                databaseProviderUserManagement,
                connectionStringUserManagement
            );

            // Create a new collection for the roles.
            this.UserValidationFieldValues = new BaseCollection<UserValidationFieldValue>(
                this,
                "UserValidationFieldValues",
                databaseProviderUserManagement,
                connectionStringUserManagement,
                true
            );

            // Create a new collection for the roles.
            this.Roles = new BaseCollection<Role>(
                this,
                "Roles",
                databaseProviderUserManagement,
                connectionStringUserManagement,
                true
            );

            // Create a new collection for the user roles.
            this.UserRoles = new BaseCollection<UserRole>(
                this,
                "UserRoles",
                databaseProviderUserManagement,
                connectionStringUserManagement,
                true
            );

            // Create a new collection for the role permissions.
            this.RolePermissions = new BaseCollection<RolePermission>(
                this,
                "RolePermissions",
                databaseProviderUserManagement,
                connectionStringUserManagement,
                true
            );

            // Create a new collection for the workgroups.
            this.Workgroups = new BaseCollection<Workgroup>(
                this,
                "Workgroups",
                databaseProviderUserManagement,
                connectionStringUserManagement,
                true
            );

            // Create a new collection for the user workgroups.
            this.UserWorkgroups = new BaseCollection<UserWorkgroup>(
                this,
                "UserWorkgroups",
                databaseProviderUserManagement,
                connectionStringUserManagement,
                true
            );

            // Create a new collection for the workgroups.
            this.WorkgroupHierarchies = new BaseCollection<WorkgroupHierarchy>(
                this,
                "WorkgroupHierarchies",
                databaseProviderUserManagement,
                connectionStringUserManagement,
                true
            );
        }

        #endregion


        #region Methods

        public void ClearCache()
        {
            lock (BaseCollection.Cache)
            {
                BaseCollection.Cache.Clear();
            }
        }

        #endregion
    }

    public enum StorageMethodType
    {
        Database,
        Xml
    }
}
