using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInterface.Classes
{
    public class TextCoder
    {
        #region Properties

        /// <summary>
        /// Gets or sets the text variable to code.
        /// </summary>
        public Variable Variable { get; set; }

        /// <summary>
        /// Gets or sets the coding type for the text variable.
        /// </summary>
        public TextCodingType CodingType { get; set; }

        /// <summary>
        /// Gets or sets a dictionary of the import's respondents
        /// by the respondent variable value.
        /// </summary>
        private Dictionary<string, Guid> Respondents { get; set; }

        #endregion


        #region Constructor

        public TextCoder(Variable variable, Dictionary<string, Guid> respondents, TextCodingType codingType)
        {
            this.Variable = variable;
            this.Respondents = respondents;
            this.CodingType = codingType;
        }

        #endregion


        #region Methods

        Dictionary<string, Guid> idCategories = new Dictionary<string, Guid>();
        public void CodeResponses(string[] respondentIds, string[] responses)
        {
            // Run through all text responses of the variable.
            for (int i = 0; i < responses.Length; i++)
            {
                if (this.CodingType == TextCodingType.Single)
                {
                    if (!idCategories.ContainsKey(responses[i]))
                        CreateCategory(responses[i]);
                    
                    // Create a new response.
                    Response response = new Response(((DatabaseCore.Core)this.Variable.Owner.Owner).Responses[this.Variable.Id]);
                    response.IdStudy = this.Variable.IdStudy;
                    response.IdRespondent = this.Respondents[respondentIds[i]];
                    response.IdCategory = idCategories[responses[i]];

                    response.Insert();
                }
                else
                {
                    // Split the response by a space to get the multiple responses.
                    string[] parts = responses[i].Split(',');

                    // Run through all multiple responses.
                    foreach (string part in parts)
                    {
                        string categoryName = part.Trim();

                        if (!idCategories.ContainsKey(categoryName))
                            CreateCategory(categoryName);

                        // Create a new response.
                        Response response = new Response(((DatabaseCore.Core)this.Variable.Owner.Owner).Responses[this.Variable.Id]);
                        response.IdStudy = this.Variable.IdStudy;
                        response.IdRespondent = this.Respondents[respondentIds[i]];
                        response.IdCategory = idCategories[categoryName];
                        
                        response.Insert();
                    }
                }
            }

            if (this.CodingType == TextCodingType.Single)
                this.Variable.Type = VariableType.Single;
            else
                this.Variable.Type = VariableType.Multi;

            // Update the variable's changes in the database.
            this.Variable.Save();
        }

        int order = 0;
        private void CreateCategory(string name)
        {
            // Create a new category.
            Category category = new Category(((DatabaseCore.Core)this.Variable.Owner.Owner).Categories);

            // Set the category's name.
            category.Name = name;

            // Set the category's variable id.
            category.IdVariable = this.Variable.Id;

            // Set the order value of the category.
            category.Order = order++;

            // Insert the category into the database.
            category.Insert();

            idCategories.Add(name, category.Id);
        }

        #endregion
    }

    public enum TextCodingType
    {
        Single,
        Multi
    }
}
