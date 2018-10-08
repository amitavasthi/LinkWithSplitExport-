using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.BaseClasses
{
    public abstract class BaseCore// : System.Runtime.Serialization.ISerializable
    {
        #region Properties

        /// <summary>
        /// Gets or sets a dictionary that contains all
        /// of the base collections by table name.
        /// </summary>
        public Dictionary<string, BaseCollection> Tables { get; set; }

        /// <summary>
        /// Gets or sets the provider string of the database.
        /// </summary>
        public string DatabaseProvider { get; protected set; }

        /// <summary>
        /// Gets or sets the connection string to the database.
        /// </summary>
        public string ConnectionString { get; protected set; }

        /// <summary>
        /// Gets or sets the path to the root folder of file storages.
        /// </summary>
        public string FileStorageRoot { get; set; }

        /// <summary>
        /// Gets or sets the path to the directory where the log files are stored.
        /// </summary>
        public string LogDirectory { get; set; }

        public string[] SynchServers { get; set; }

        #endregion


        #region User Management Collections

        /// <summary>
        /// Gets or sets the users collection.
        /// </summary>
        public UserCollection Users { get; set; }

        /// <summary>
        /// Gets or sets the user validation field value collection.
        /// </summary>
        public BaseCollection<UserValidationFieldValue> UserValidationFieldValues { get; set; }

        /// <summary>
        /// Gets or sets the roles collection.
        /// </summary>
        public BaseCollection<Role> Roles { get; set; }

        /// <summary>
        /// Gets or sets the user roles collection.
        /// </summary>
        public BaseCollection<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Gets or sets the role permissions collection.
        /// </summary>
        public BaseCollection<RolePermission> RolePermissions { get; set; }

        /// <summary>
        /// Gets or sets the workgroups collection.
        /// </summary>
        public BaseCollection<Workgroup> Workgroups { get; set; }

        /// <summary>
        /// Gets or sets the user workgroups collection.
        /// </summary>
        public BaseCollection<UserWorkgroup> UserWorkgroups { get; set; }

        /// <summary>
        /// Gets or sets the workgroups hierarchy collection.
        /// </summary>
        public BaseCollection<WorkgroupHierarchy> WorkgroupHierarchies { get; set; }

        #endregion


        #region Constructor

        public BaseCore(string[] synchServers)
        {
            this.Tables = new Dictionary<string, BaseCollection>();
            this.SynchServers = synchServers;
        }

        #endregion
    }
}
