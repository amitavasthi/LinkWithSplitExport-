using DatabaseCore.BaseClasses;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkAdmin.Classes
{
    public class LinkAdminCore : BaseCore
    {
        #region Constructor

        public LinkAdminCore(string fileStorageRoot)
            : base (new string[0])
        {
            base.FileStorageRoot = fileStorageRoot;

            // Create a new collection for the users.
            base.Users = new UserCollection(
                this,
                "Users",
                "",
                "",
                DatabaseCore.StorageMethodType.Xml
            );

            // Create a new collection for the roles.
            base.Roles = new BaseCollection<Role>(
                this,
                "Roles",
                "",
                "",
                true,
                DatabaseCore.StorageMethodType.Xml
            );

            // Create a new collection for the user roles.
            base.UserRoles = new BaseCollection<UserRole>(
                this,
                "UserRoles",
                "",
                "",
                true,
                DatabaseCore.StorageMethodType.Xml
            );

            // Create a new collection for the role permissions.
            base.RolePermissions = new BaseCollection<RolePermission>(
                this,
                "RolePermissions",
                "",
                "",
                true,
                DatabaseCore.StorageMethodType.Xml
            );

            // Create a new collection for the workgroups.
            base.Workgroups = new BaseCollection<Workgroup>(
                this,
                "Workgroups",
                "",
                "",
                true,
                DatabaseCore.StorageMethodType.Xml
            );

            // Create a new collection for the user workgroups.
            base.UserWorkgroups = new BaseCollection<UserWorkgroup>(
                this,
                "UserWorkgroups",
                "",
                "",
                true,
                DatabaseCore.StorageMethodType.Xml
            );

            // Create a new collection for the workgroups.
            base.WorkgroupHierarchies = new BaseCollection<WorkgroupHierarchy>(
                this,
                "WorkgroupHierarchies",
                "",
                "",
                true,
                DatabaseCore.StorageMethodType.Xml
            );
        }

        #endregion
    }
}