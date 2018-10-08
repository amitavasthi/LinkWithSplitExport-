using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseDataCore1
{
    public class DataCommand
    {
        #region Properties

        public CaseDataType DataType { get; set; }

        public bool Valid { get; set; }

        public Guid IdVariable { get; set; }

        public Dictionary<Guid, object> CategoryFilter { get; set; }

        #endregion


        #region Constructor

        public DataCommand(string commandText)
        {
            this.DataType = CaseDataType.Categorical;
            if (commandText.Contains("NumericAnswer"))
            {
                this.DataType = CaseDataType.Numeric;
            }
            else if (commandText.Contains("TextAnswer"))
            {
                this.DataType = CaseDataType.Text;
            }

            try
            {
                this.IdVariable = Guid.Parse(commandText.Split(new string[]
                {
                "[resp].[Var_"
                }, StringSplitOptions.None)[1].Split(']')[0]);

                if (commandText.Contains(" WHERE [IdCategory] IN ("))
                {
                    this.CategoryFilter = new Dictionary<Guid, object>();

                    string[] categoryStrings = commandText.Split(new string[]
                    {
                    " WHERE [IdCategory] IN ("
                    }, StringSplitOptions.None)[1].Split(')')[0].Split(',');

                    foreach (string categoryString in categoryStrings)
                    {
                        if (categoryString.Length <= 2)
                            continue;

                        Guid idCategory;
                        if (Guid.TryParse(categoryString.Remove(categoryString.Length - 1, 1).Remove(0, 1), out idCategory))
                            this.CategoryFilter.Add(idCategory, null);
                    }
                }
                else if (commandText.Contains(" WHERE IdCategory IN ("))
                {
                    this.CategoryFilter = new Dictionary<Guid, object>();

                    string[] categoryStrings = commandText.Split(new string[]
                    {
                    " WHERE IdCategory IN ("
                    }, StringSplitOptions.None)[1].Split(')')[0].Split(',');

                    foreach (string categoryString in categoryStrings)
                    {
                        if (categoryString.Length <= 2)
                            continue;

                        Guid idCategory;
                        if (Guid.TryParse(categoryString.Remove(categoryString.Length - 1, 1).Remove(0, 1), out idCategory))
                            this.CategoryFilter.Add(idCategory, null);
                    }
                }
                else if (commandText.Contains(" WHERE [IdCategory]=") || commandText.Contains(" WHERE [IdCategory] ="))
                {
                    commandText = commandText.Replace(" WHERE [IdCategory]=", " WHERE [IdCategory] =");

                    this.CategoryFilter = new Dictionary<Guid, object>();

                    string categoryString = commandText.Split(new string[]
                    {
                    " WHERE [IdCategory] ="
                    }, StringSplitOptions.None)[1].Split('\'')[1].Split('\'')[0].Trim();

                    if (categoryString.Length > 2)
                    {
                        Guid idCategory;
                        if (Guid.TryParse(categoryString, out idCategory))
                            this.CategoryFilter.Add(idCategory, null);
                    }
                }
                else if (commandText.Contains(" WHERE IdCategory=") || commandText.Contains(" WHERE IdCategory ="))
                {
                    commandText = commandText.Replace(" WHERE IdCategory=", " WHERE IdCategory =");

                    this.CategoryFilter = new Dictionary<Guid, object>();

                    string categoryString = commandText.Split(new string[]
                    {
                    " WHERE IdCategory ="
                    }, StringSplitOptions.None)[1].Split('\'')[1].Split('\'')[0].Trim();

                    if (categoryString.Length > 2)
                    {
                        Guid idCategory;
                        if (Guid.TryParse(categoryString, out idCategory))
                            this.CategoryFilter.Add(idCategory, null);
                    }
                }

                this.Valid = true;
            }
            catch
            {
                this.Valid = false;
            }
        }

        #endregion
    }

    public enum CaseDataType
    {
        Categorical,
        Numeric,
        Text
    }
}
