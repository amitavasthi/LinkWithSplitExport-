using DataCore.Classes.StorageMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCore.Classes
{
    public abstract class EquationPlaceHolderFilterItem
    {
        #region Properties

        public EquationPlaceHolder PlaceHolder { get; set; }

        public string Source { get; set; }

        #endregion


        #region Constructor

        public EquationPlaceHolderFilterItem(EquationPlaceHolder placeHolder, string source)
        {
            this.PlaceHolder = placeHolder;
            this.Source = source;

            this.Parse();
        }

        #endregion


        #region Methods

        protected abstract void Parse();

        public abstract Data Evalute(Data baseFilter, Data filter);

        #endregion
    }

    public class EquationPlaceHolderFilter : EquationPlaceHolderFilterItem
    {
        #region Properties

        public List<EquationPlaceHolderFilterItem> Items { get; set; }

        #endregion


        #region Constructor

        public EquationPlaceHolderFilter(EquationPlaceHolder placeHolder, string source)
            : base(placeHolder, source)
        { }

        #endregion


        #region Methods

        protected override void Parse()
        {
            this.Items = new List<EquationPlaceHolderFilterItem>();

            string[] parts = this.Source.Split(' ');

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "AND" || parts[i] == "OR")
                    continue;

                EquationPlaceHolderFilterOperator op = EquationPlaceHolderFilterOperator.AND;

                if (i > 0 && (parts[i - 1] == "AND" || parts[i - 1] == "OR"))
                {
                    op = (EquationPlaceHolderFilterOperator)Enum.Parse(
                        typeof(EquationPlaceHolderFilterOperator),
                        parts[i - 1]
                    );
                }

                EquationPlaceHolderFilterCategory category = new EquationPlaceHolderFilterCategory(
                    base.PlaceHolder,
                    parts[i],
                    op
                );

                this.Items.Add(category);
            }
        }

        public override Data Evalute(Data baseFilter, Data filter)
        {
            foreach (EquationPlaceHolderFilterItem item in this.Items)
            {
                filter = item.Evalute(baseFilter, filter);
            }

            return filter;
        }

        #endregion
    }

    public class EquationPlaceHolderFilterCategory : EquationPlaceHolderFilterItem
    {
        #region Properties

        public EquationPlaceHolderFilterOperator Operator { get; set; }

        /// <summary>
        /// Gets or sets the id of the study in
        /// case the filter is a study category.
        /// </summary>
        public Guid? IdStudy { get; set; }

        /// <summary>
        /// Gets or sets the id of
        /// the category's variable.
        /// </summary>
        public Guid IdVariable { get; set; }

        /// <summary>
        /// Gets or sets the id of the category.
        /// </summary>
        public Guid IdCategory { get; set; }

        #endregion


        #region Constructor

        public EquationPlaceHolderFilterCategory(
            EquationPlaceHolder placeHolder,
            string source,
            EquationPlaceHolderFilterOperator op
        )
            : base(placeHolder, source)
        {
            this.Operator = op;
        }

        #endregion


        #region Methods

        protected override void Parse()
        {
            if (this.Source.StartsWith("/"))
            {
                this.IdStudy = Guid.Parse(this.Source.Split('/')[1].Split('\\')[0]);
                this.Source = this.Source.Replace("/" + this.IdStudy + "\\", "");
            }

            string variableName = this.Source.Split('.')[0];

            string categoryName = null;
            if (this.Source.Contains("."))
            {
                categoryName = this.Source.Split('.')[1].Split('$')[0];
            }

            object idVariable;
            if (this.IdStudy.HasValue)
            {
                idVariable = base.PlaceHolder.Equation.Core.Variables.GetValue(
                    "Id",
                    new string[] { "IdStudy", "Name" },
                    new object[] { this.IdStudy.Value, variableName }
                );
            }
            else
            {
                idVariable = base.PlaceHolder.Equation.Core.TaxonomyVariables.GetValue(
                    "Id",
                    new string[] { "Name" },
                    new object[] { variableName }
                );
            }

            if (idVariable != null)
                this.IdVariable = (Guid)idVariable;

            if (categoryName != null && idVariable != null)
            {
                object idCategory;
                if (this.IdStudy.HasValue)
                {
                    idCategory = base.PlaceHolder.Equation.Core.Categories.GetValue(
                        "Id",
                        new string[] { "IdVariable", "IdStudy", "Name" },
                        new object[] { this.IdVariable, this.IdStudy.Value, categoryName }
                    );
                }
                else
                {
                    idCategory = base.PlaceHolder.Equation.Core.TaxonomyCategories.GetValue(
                        "Id",
                        new string[] { "IdTaxonomyVariable", "Name" },
                        new object[] { this.IdVariable, categoryName }
                    );
                }

                if (this.IdCategory != null)
                    this.IdCategory = (Guid)idCategory;
            }
        }

        public override Data Evalute(Data baseFilter, Data filter)
        {
            Database storageMethod = new Database(
                this.PlaceHolder.Equation.Core,
                null,
                this.PlaceHolder.Equation.WeightMissingValue
            );

            if (this.Operator == EquationPlaceHolderFilterOperator.AND)
            {
                return storageMethod.GetRespondents(
                    this.IdCategory,
                    this.IdVariable,
                    !this.IdStudy.HasValue,
                    this.PlaceHolder.Equation.CaseDataLocation,
                    filter
                );
            }
            else
            {
                Data data = storageMethod.GetRespondents(
                    this.IdCategory,
                    this.IdVariable,
                    !this.IdStudy.HasValue,
                    this.PlaceHolder.Equation.CaseDataLocation,
                    baseFilter
                );

                foreach (Guid idRespondent in filter.Responses.Keys)
                {
                    if (!data.Responses.ContainsKey(idRespondent))
                        data.Responses.Add(idRespondent, new double[2]);
                }

                return data;
            }
        }

        #endregion
    }

    public enum EquationPlaceHolderFilterOperator
    {
        AND,
        OR
    }
}
