using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class Study : BaseClasses.BaseItem<Study>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the description of the project.
        /// </summary>
        public string Description
        {
            get
            {
                return base.GetValue<string>("Description");
            }
            set
            {
                base.SetValue("Description", value);
            }
        }

        /// <summary>
        /// Gets or sets the creation date of the project.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the id of the user who created the study.
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
        /// Gets or sets the id of the study's hierarchy.
        /// </summary>
        public Guid IdHierarchy
        {
            get
            {
                return base.GetValue<Guid>("IdHierarchy");
            }
            set
            {
                base.SetValue("IdHierarchy", value);
            }
        }

        /// <summary>
        /// Gets or sets the id of the user who created the study.
        /// </summary>
        public StudyStatus Status
        {
            get
            {
                return base.GetValue<StudyStatus>("Status");
            }
            set
            {
                base.SetValue("Status", value);
            }
        }


        public Variable[] Variables
        {
            get
            {
                return ((DatabaseCore.Core)this.Owner.Owner).Variables.Get("IdStudy", this.Id);
            }
        }

        #endregion


        #region Constructor

        public Study(BaseClasses.BaseCollection<Study> collection, DbDataReader reader = null)
            : base(collection, reader)
        { }

        #endregion
    }

    public enum StudyStatus
    {
        None = 0,
        ImportFailed = 1,
        Deleting = 2,
        DeletionFailed = 3
    }
}
