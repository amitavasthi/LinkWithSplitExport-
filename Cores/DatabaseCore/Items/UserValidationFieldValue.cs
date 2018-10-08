using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class UserValidationFieldValue : BaseClasses.BaseItem<UserValidationFieldValue>
    {
        #region Properties

        /// <summary>
        /// Gets or sets id of the user.
        /// </summary>
        public Guid IdUser
        {
            get
            {
                return base.GetValue<Guid>("IdUser");
            }
            set
            {
                base.SetValue("IdUser", value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        public string Field
        {
            get
            {
                return base.GetValue<string>("Field");
            }
            set
            {
                base.SetValue("Field", value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the field.
        /// </summary>
        public string Value
        {
            get
            {
                return base.GetValue<string>("Value");
            }
            set
            {
                base.SetValue("Value", value);
            }
        }

        #endregion


        #region Constructor

        public UserValidationFieldValue(BaseClasses.BaseCollection<UserValidationFieldValue> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }
}
