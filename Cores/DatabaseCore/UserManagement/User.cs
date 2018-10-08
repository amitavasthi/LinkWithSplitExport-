using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DatabaseCore.Items
{
    public class User : BaseClasses.BaseItem<User>
    {
        #region Properties

        public DateTime CreationDate
        {
            get
            {
                return base.GetValue<DateTime>("CreationDate");
            }
            set
            {
                base.SetValue("CreationDate", value);
            }
        }

        public string Name
        {
            get
            {
                return base.GetValue<string>("Name");
            }
            set
            {
                base.SetValue("Name", value);
            }
        }

        public string FirstName
        {
            get
            {
                return base.GetValue<string>("FirstName");
            }
            set
            {
                base.SetValue("FirstName", value);
            }
        }

        public string LastName
        {
            get
            {
                return base.GetValue<string>("LastName");
            }
            set
            {
                base.SetValue("LastName", value);
            }
        }

        public string Mail
        {
            get
            {
                return base.GetValue<string>("Mail");
            }
            set
            {
                base.SetValue("Mail", value);
            }
        }

        public string Phone
        {
            get
            {
                return base.GetValue<string>("Phone");
            }
            set
            {
                base.SetValue("Phone", value);
            }
        }

        public string Password
        {
            get
            {
                return base.GetValue<string>("Password");
            }
            set
            {
                base.SetValue("Password", value);
            }
        }

        public string Language
        {
            get
            {
                return base.GetValue<string>("Language");
            }
            set
            {
                base.SetValue("Language", value);
            }
        }

        public DateTime LastLogon
        {
            get
            {
                return base.GetValue<DateTime>("LastLogon");
            }
            set
            {
                base.SetValue("LastLogon", value);
            }
        }

        public string Browser
        {
            get
            {
                return base.GetValue<string>("Browser");
            }
            set
            {
                base.SetValue("Browser", value);
            }
        }

        public bool Validated
        {
            get
            {
                return base.GetValue<bool>("Validated");
            }
            set
            {
                base.SetValue("Validated", value);
            }
        }


        public Role Role
        {
            get
            {
                // Get the user role linking element.
                UserRole userRole = this.Owner.Owner.UserRoles.GetSingle("IdUser", this.Id);

                if (userRole == null)
                    return null;

                // Get the user's role.
                Role role = this.Owner.Owner.Roles.GetSingle(userRole.IdRole);

                return role;
            }
        }

        #endregion


        #region Constructor

        public User(BaseClasses.BaseCollection<User> collection, DbDataReader reader = null)
            : base(collection, reader)
        {
            if (reader == null)
                this.CreationDate = DateTime.Now;
        }

        public User(BaseClasses.BaseCollection<User> collection, XmlNode node)
            : base(collection, node)
        {
        }

        #endregion


        #region Methods

        public bool HasPermission(int idPermission)
        {
            // Get the user's role.
            Role role = this.Role;

            if (role == null)
                return false;

            // Get the role permission by the role's id and permission id.
            RolePermission rolePermission = this.Owner.Owner.RolePermissions.GetSingle(
                new string[] { "IdRole", "Permission" },
                new object[] { role.Id, idPermission }
            );

            if (rolePermission == null)
                return false;

            return true;
        }

        #endregion
    }
}
