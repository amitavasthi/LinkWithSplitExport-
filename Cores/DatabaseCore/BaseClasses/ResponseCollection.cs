using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.BaseClasses
{
    public class ResponseCollection
    {
        #region Properties

        /// <summary>
        /// Gets or sets the owning core of the response collection.
        /// </summary>
        public Core Owner { get; set; }

        #endregion


        #region Constructor

        public ResponseCollection(Core owner)
        {
            this.Owner = owner;
        }

        #endregion


        #region Opertators

        /// <summary>
        /// Gets a variable response collection of a variable.
        /// </summary>
        /// <param name="idVariable">The id of the variable for the response collection.</param>
        public BaseCollection<Response> this[Guid idVariable, StorageMethodType xmlStorage = StorageMethodType.Database]
        {
            get
            {
                // Create a new base collection for the variable's responses.
                BaseCollection<Response> result = new BaseCollection<Response>(
                    this.Owner,
                    "resp.Var_" + idVariable,
                    false
                );

                result.StorageMethod = xmlStorage;
                //result.StorageMethod = StorageMethodType.Xml;

                return result;
            }
        }

        #endregion
    }
}
