using DatabaseCore;
using DatabaseCore.BaseClasses;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace VariableSelector1.Classes
{
    public class DefinitionObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets the database core that
        /// is used to access metadata.
        /// </summary>
        public DatabaseCore.Core Core { get; set; }

        /// <summary>
        /// Gets or sets the source where
        /// the variable is stored.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the path to
        /// the variable in the source.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the path to the parent xml node.
        /// </summary>
        public string ParentPath { get; set; }

        /// <summary>
        /// Gets or sets the type of the source.
        /// </summary>
        public StorageMethodType StorageType { get; set; }

        public string TypeName { get; set; }

        public Dictionary<string, object> Values { get; set; }


        public XmlNode XmlNode { get; set; }

        #endregion


        #region Constructor

        public DefinitionObject(DatabaseCore.Core core, string source)
        {
            this.Core = core;
            this.Values = new Dictionary<string, object>();
            this.Source = source;

            if (this.Source.Contains("dbo."))
                this.Source = this.Source.Replace("dbo.", "");

            if (File.Exists(source))
                this.StorageType = StorageMethodType.Xml;
        }

        public DefinitionObject(DatabaseCore.Core core, string source, string path)
            : this(core, source)
        {
            this.Path = path;

            if (this.Path.StartsWith("Id="))
            {
                Guid id = Guid.Parse(this.Path.Remove(0, 3).Split('&')[0]);
                this.Values.Add("Id", id);
            }

            if (this.StorageType == StorageMethodType.Xml)
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(this.Source);
                /*XmlDocument xmlDocument = ApplicationUtilities.FileSystem.LoadXml(this.Source, true);*/
                
                XmlNode xmlNode = xmlDocument.SelectSingleNode(this.Path);

                if (xmlNode != null)
                {
                    this.TypeName = xmlNode.Name;

                    if (this.TypeName == "Variable" && xmlNode.Attributes["IsTaxonomy"] != null && bool.Parse(xmlNode.Attributes["IsTaxonomy"].Value) == true)
                        this.TypeName = "TaxonomyVariable";

                    if (xmlNode.ParentNode.NodeType != XmlNodeType.Document)
                        this.ParentPath = xmlNode.ParentNode.GetXPath(true);

                    this.XmlNode = xmlNode;
                }
            }
        }

        public DefinitionObject(DatabaseCore.Core core, string fileName, XmlNode xmlNode)
            : this(core, fileName)
        {
            this.XmlNode = xmlNode;
            this.Path = xmlNode.GetXPath();

            this.TypeName = xmlNode.Name;
            this.ParentPath = xmlNode.ParentNode.GetXPath(true);

            if (this.TypeName.StartsWith("IsTaxonomy") == false &&
                xmlNode.Attributes["IsTaxonomy"] != null &&
                bool.Parse(xmlNode.Attributes["IsTaxonomy"].Value) == true)
            {
                this.TypeName = "Taxonomy" + this.TypeName;
            }
        }

        public DefinitionObject(DatabaseCore.Core core, BaseItem item)
        {
            this.Core = core;
            this.Values = new Dictionary<string, object>();
            this.Source = item.Owner.TableName.Replace("[", "").Replace("]", "");
            this.Path = "Id=" + item.Id;
            this.Values.Add("Id", item.Id);

            if (this.Source.Contains("dbo."))
                this.Source = this.Source.Replace("dbo.", "");

            this.TypeName = item.GetType().Name;
        }

        #endregion


        #region Methods

        public DefinitionObject[] GetChilds()
        {
            List<DefinitionObject> result = new List<DefinitionObject>();

            switch (this.StorageType)
            {
                case StorageMethodType.Database:

                    string source = "";
                    string identifier = "";

                    switch (this.Source)
                    {
                        case "TaxonomyVariables":
                            source = "TaxonomyCategories";
                            identifier = "IdTaxonomyVariable";
                            break;
                        case "TaxonomyCategories":
                            source = "TaxonomyCategoryLinks";
                            identifier = "IdScoreGroup";
                            break;
                        case "Variables":
                            source = "Categories";
                            identifier = "IdVariable";
                            break;
                    }

                    if (source == "")
                        return result.ToArray();

                    BaseItem[] items = this.Core.Tables[source].GetItems(
                        new string[] { identifier },
                        new object[] { this.GetValue("Id") },
                        "Order"
                    );

                    foreach (BaseItem item in items)
                    {
                        if (source == "TaxonomyCategoryLinks")
                        {
                            result.Add(new DefinitionObject(this.Core, "TaxonomyCategories", "Id=" + ((BaseItem<TaxonomyCategoryLink>)item).GetValue("IdTaxonomyCategory")));
                        }
                        else
                        {
                            result.Add(new DefinitionObject(this.Core, item));
                        }
                    }

                    break;
                case StorageMethodType.Xml:

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(this.Source);

                    XmlNode xmlNode = xmlDocument.SelectSingleNode(this.Path);

                    List<XmlNode> xmlNodesItems = xmlNode.ChildNodes.OrderByNumeric("Order");

                    foreach (XmlNode xmlNodeItem in xmlNodesItems)
                    {
                        result.Add(new DefinitionObject(this.Core, this.Source, xmlNodeItem));
                    }

                    break;
            }

            return result.ToArray();
        }

        public DefinitionObject GetParent()
        {
            DefinitionObject result = null;

            switch (this.StorageType)
            {
                case StorageMethodType.Database:

                    string source = "";
                    string identifier = "";

                    switch (this.Source)
                    {
                        case "TaxonomyCategories":
                            source = "TaxonomyVariables";
                            identifier = "IdTaxonomyVariable";
                            break;
                        case "Variables":
                            source = "Categories";
                            identifier = "IdVariable";
                            break;
                    }

                    Guid id = (Guid)this.Core.Tables[this.Source].GetValue(
                        identifier,
                        this.Path
                    );

                    BaseItem[] items = this.Core.Tables[source].GetItems(
                        new string[] { "Id" },
                        new object[] { id }
                    );

                    if (items.Length > 0)
                        result = new DefinitionObject(this.Core, items[0]);

                    break;
                case StorageMethodType.Xml:

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(this.Source);

                    XmlNode xmlNode = xmlDocument.SelectSingleNode(this.Path);

                    result = new DefinitionObject(
                        this.Core,
                        this.Source,
                        xmlNode.ParentNode
                    );

                    break;
            }

            return result;
        }


        public object GetValue(string name)
        {
            return GetValue(name, true);
        }

        public object GetValue(string name, bool create = true, bool getFromDatabase = false)
        {
            if (!this.Values.ContainsKey(name))
            {
                if (create)
                    this.Values.Add(name, GetValue(this.StorageType, name));
                else
                    return GetValue(this.StorageType, name, create);
            }

            return this.Values[name];
        }

        public object GetValue(StorageMethodType storageType, string name, bool create = true)
        {
            object result = null;
            string path;

            switch (storageType)
            {
                case StorageMethodType.Database:

                    try {
                        //result = this.Core.Tables[this.Source].GetValue(name, "Id", Guid.Parse(this.GetValue("Id").ToString()));
                        result = this.Core.Tables[this.Source].GetValue(name, this.Path);
                    }
                    catch
                    {
                        result = null;
                    }

                    break;
                case StorageMethodType.Xml:

                    if (this.XmlNode != null && this.XmlNode.Attributes[name] != null)
                        result = this.XmlNode.Attributes[name].Value;
                    else if (this.XmlNode != null)
                    {
                        string source = "";
                        path = "";

                        switch (this.TypeName)
                        {
                            case "TaxonomyCategory":
                            case "ScoreGroup":
                                source = "TaxonomyCategories";
                                break;
                            case "TaxonomyVariable":
                                source = "TaxonomyVariables";
                                break;
                            case "Category":
                                source = "Categories";
                                break;
                            case "Variable":
                                source = "Variables";
                                break;
                        }

                        if (source == "")
                            return result;

                        path = "Id=" + this.GetValue("Id");

                        string _name = name;

                        if (name.StartsWith("Label"))
                        {
                            _name = "Label";

                            switch (source)
                            {
                                case "TaxonomyVariables":
                                    source = "TaxonomyVariableLabels";
                                    path = "IdTaxonomyVariable=" + this.GetValue("Id");
                                    break;
                                case "TaxonomyCategories":
                                    source = "TaxonomyCategoryLabels";
                                    path = "IdTaxonomyCategory=" + this.GetValue("Id");
                                    break;
                                case "Variables":
                                    source = "VariableLabels";
                                    path = "IdVariable=" + this.GetValue("Id");
                                    break;
                                case "Categories":
                                    source = "CategoryLabels";
                                    path = "IdCategory=" + this.GetValue("Id");
                                    break;
                            }

                            int idLanguage;

                            if (int.TryParse(name.Replace("Label", ""), out idLanguage))
                            {
                                path += "&IdLanguage=" + idLanguage;
                            }
                        }

                        DefinitionObject score = new DefinitionObject(this.Core, source, path);

                        try
                        {
                            result = score.GetValue(_name, create);
                        }
                        catch {
                            result = null;
                        }

                        SetValue(name, result);
                    }

                    break;
            }

            return result;
        }

        public void SetValue(string name, object value)
        {
            if (value == null)
                return;

            if (this.Values.ContainsKey(name))
                this.Values[name] = value;

            switch (this.StorageType)
            {
                case StorageMethodType.Database:

                    this.Core.Tables[this.Source].SetValue(this.Path, name, value);

                    break;
                case StorageMethodType.Xml:

                    /*XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(this.Source);

                    XmlNode xmlNode = xmlDocument.SelectSingleNode(this.Path);*/

                    if (this.XmlNode != null)
                    {

                        if (this.XmlNode.Attributes[name] != null)
                            this.XmlNode.Attributes[name].Value = HttpUtility.HtmlEncode(value.ToString());
                        else
                            this.XmlNode.AddAttribute(name, HttpUtility.HtmlEncode(value));
                    }

                    //SetLatestChange(xmlDocument);

                    //this.XmlNode.OwnerDocument.Save(this.Source);

                    break;
            }
        }


        public string GetLabel(int idLanguage)
        {
            if (this.Values.ContainsKey("Label" + idLanguage))
                return (string)this.Values["Label" + idLanguage];

            string result = "";

            string source = "";
            string identifier = "";

            switch (this.StorageType)
            {
                case StorageMethodType.Database:

                    switch (this.Source)
                    {
                        case "TaxonomyVariables":
                            source = "TaxonomyVariableLabels";
                            identifier = "IdTaxonomyVariable";
                            break;
                        case "TaxonomyCategories":
                            source = "TaxonomyCategoryLabels";
                            identifier = "IdTaxonomyCategory";
                            break;
                        case "Variables":
                            source = "VariableLabels";
                            identifier = "IdVariable";
                            break;
                        case "Categories":
                            source = "CategoryLabels";
                            identifier = "IdCategory";
                            break;
                    }

                    /*result = (string)this.Core.Tables[source].GetValue(
                        "Label",
                        identifier + "=" + Guid.Parse(this.GetValue("Id").ToString()) + "&IdLanguage=" + idLanguage
                    );*/
                    result = (string)this.Core.Tables[source].GetValue(
                        "Label",
                        new string[] { identifier, "IdLanguage" },
                        new object[] { Guid.Parse(this.GetValue("Id").ToString()), idLanguage }
                    );

                    break;
                case StorageMethodType.Xml:

                    string attibuteName = "Label" + idLanguage;
                    object label = this.GetValue(attibuteName, false);

                    if (label == null)
                    {
                        if (this.TypeName == "ScoreGroup")
                        {
                            this.TypeName = "TaxonomyCategory";

                            result = this.GetLabel(idLanguage);

                            this.TypeName = "ScoreGroup";

                            if (string.IsNullOrEmpty(result))
                                result = (string)this.GetValue("Name");
                        }
                        else
                        {
                            switch (this.TypeName)
                            {
                                case "TaxonomyVariable":
                                    source = "TaxonomyVariableLabels";
                                    identifier = "IdTaxonomyVariable";
                                    break;
                                case "TaxonomyCategory":
                                    source = "TaxonomyCategoryLabels";
                                    identifier = "IdTaxonomyCategory";
                                    break;
                                case "Variable":
                                    source = "VariableLabels";
                                    identifier = "IdVariable";
                                    break;
                                case "Category":
                                    source = "CategoryLabels";
                                    identifier = "IdCategory";
                                    break;
                            }

                            if (string.IsNullOrEmpty(source))
                            {
                                result = "";
                            }
                            else
                            {
                                result = (string)this.Core.Tables[source].GetValue(
                                    "Label",
                                    identifier + "=" + this.GetValue("Id") + "&IdLanguage=" + idLanguage
                                );
                            }
                        }

                        this.SetValue(attibuteName, result);
                    }
                    else
                    {
                        result = label.ToString();
                    }

                    result = HttpUtility.HtmlDecode(result);

                    break;
            }

            return result;
        }

        public void SetLabel(int idLanguage, string label)
        {
            switch (this.StorageType)
            {
                case StorageMethodType.Database:

                    string source = "";
                    string identifier = "";

                    switch (this.Source)
                    {
                        case "TaxonomyVariables":
                            source = "TaxonomyVariableLabels";
                            identifier = "IdTaxonomyVariable";
                            break;
                        case "TaxonomyCategories":
                            source = "TaxonomyCategoryLabels";
                            identifier = "IdTaxonomyCategory";
                            break;
                        case "Variables":
                            source = "VariableLabels";
                            identifier = "IdVariable";
                            break;
                        case "Categories":
                            source = "CategoryLabels";
                            identifier = "IdCategory";
                            break;
                    }

                    string path = identifier + "=" + this.GetValue("Id") + "&IdLanguage=" + idLanguage;

                    object idLabel = this.Core.Tables[source].GetValue(
                        "Id",
                        path
                    );

                    if (idLabel != null)
                    {
                        this.Core.Tables[source].SetValue(
                            path,
                            "Label",
                            label
                        );
                    }
                    else
                    {
                        switch (source)
                        {
                            case "TaxonomyVariableLabels":
                                TaxonomyVariableLabel taxonomyVariableLabel = new TaxonomyVariableLabel(this.Core.TaxonomyVariableLabels);
                                taxonomyVariableLabel.IdTaxonomyVariable = (Guid)this.GetValue("Id");
                                taxonomyVariableLabel.IdLanguage = idLanguage;
                                taxonomyVariableLabel.Label = label;

                                taxonomyVariableLabel.Insert();
                                break;
                            case "TaxonomyCategoryLabels":
                                TaxonomyCategoryLabel taxonomyCategoryLabel = new TaxonomyCategoryLabel(this.Core.TaxonomyCategoryLabels);
                                taxonomyCategoryLabel.IdTaxonomyCategory = (Guid)this.GetValue("Id");
                                taxonomyCategoryLabel.IdLanguage = idLanguage;
                                taxonomyCategoryLabel.Label = label;

                                taxonomyCategoryLabel.Insert();
                                break;
                            case "VariableLabels":
                                VariableLabel variableLabel = new VariableLabel(this.Core.VariableLabels);
                                variableLabel.IdVariable = (Guid)this.GetValue("Id");
                                variableLabel.IdLanguage = idLanguage;
                                variableLabel.Label = label;

                                variableLabel.Insert();
                                break;
                            case "CategoryLabels":
                                CategoryLabel categoryLabel = new CategoryLabel(this.Core.CategoryLabels);
                                categoryLabel.IdCategory = (Guid)this.GetValue("Id");
                                categoryLabel.IdLanguage = idLanguage;
                                categoryLabel.Label = label;

                                categoryLabel.Insert();
                                break;
                        }
                    }

                    break;
                case StorageMethodType.Xml:

                    this.SetValue("Label" + idLanguage, label);

                    break;
            }
        }


        public string Combine(DefinitionObject score2)
        {
            string result = "";

            switch (this.StorageType)
            {
                case StorageMethodType.Database:

                    if (this.IsScoreGroup())
                    {
                        TaxonomyCategoryLink link = new TaxonomyCategoryLink(this.Core.TaxonomyCategoryLinks);

                        link.IdTaxonomyCategory = (Guid)score2.GetValue("Id");

                        link.IdScoreGroup = (Guid)this.GetValue("Id");

                        link.Insert();

                        //score2.SetValue("IdTaxonomyCategory", this.GetValue("Id"));
                    }
                    else if (score2 != null && score2.IsScoreGroup())
                    {
                        TaxonomyCategoryLink link = new TaxonomyCategoryLink(this.Core.TaxonomyCategoryLinks);

                        link.IdTaxonomyCategory = (Guid)this.GetValue("Id");

                        link.IdScoreGroup = (Guid)score2.GetValue("Id");

                        link.Insert();

                        //this.SetValue("IdTaxonomyCategory", score2.GetValue("Id"));
                    }
                    else
                    {
                        TaxonomyCategory group = new TaxonomyCategory(this.Core.TaxonomyCategories);
                        group.Name = "";
                        group.Order = (int)this.GetValue("Order");
                        group.SetValue("CreationDate", DateTime.Now);
                        group.IdTaxonomyVariable = (Guid)this.GetValue("IdTaxonomyVariable");
                        group.Color = ApplicationUtilities.Classes.ColorCalculator.GenerateRandomColor().ToString().Remove(0, 1);
                        group.IsScoreGroup = true;

                        result = "Id=" + group.Id;

                        group.Insert();

                        TaxonomyCategoryLink link1 = new TaxonomyCategoryLink(this.Core.TaxonomyCategoryLinks);

                        link1.IdTaxonomyCategory = (Guid)this.GetValue("Id");

                        link1.IdScoreGroup = group.Id;

                        link1.Insert();

                        if (score2 != null)
                        {
                            TaxonomyCategoryLink link2 = new TaxonomyCategoryLink(this.Core.TaxonomyCategoryLinks);
                            link2.IdTaxonomyCategory = (Guid)score2.GetValue("Id");
                            link2.IdScoreGroup = group.Id;
                            link2.Insert();
                        }
                    }

                    break;
                case StorageMethodType.Xml:

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(this.Source);

                    XmlNode xmlNode = xmlDocument.SelectSingleNode(this.Path);

                    if (this.TypeName == "ScoreGroup")
                    {
                        xmlNode.InnerXml += string.Format(
                            "<{2} Id=\"{0}\" Order=\"{1}\"></{2}>",
                            score2.GetValue("Id"),
                            xmlNode.ChildNodes.Count,
                            score2.TypeName
                        );

                        result = "ScoreGroup[@Id=\"" + xmlNode.Attributes["Id"].Value + "\"]";

                        SetLatestChange(xmlDocument);

                        xmlDocument.Save(this.Source);

                        //score2.SetValue("Enabled", false);
                    }
                    else
                    {
                        Guid idScoreGroup = Guid.NewGuid();

                        if (score2 != null)
                        {
                            xmlNode.ParentNode.InnerXml += string.Format(
                                "<ScoreGroup Id=\"{0}\" Name=\"{1}\" Order=\"{2}\" Value=\"{5}\" Color=\"{6}\">" +
                                "<{7} Id=\"{3}\" Order=\"0\"></{7}>" +
                                "<{8} Id=\"{4}\" Order=\"1\"></{8}>" +
                                "</ScoreGroup>",
                                idScoreGroup,
                                "",
                                xmlNode.ParentNode.ChildNodes.Count,
                                this.GetValue("Id"),
                                score2.GetValue("Id"),
                                xmlNode.ParentNode.ChildNodes.Count + 1,
                                ApplicationUtilities.Classes.ColorCalculator.GenerateRandomColor().ToString().Remove(0, 1),
                                this.TypeName,
                                score2.TypeName
                            );
                        }
                        else
                        {
                            xmlNode.ParentNode.InnerXml += string.Format(
                                "<ScoreGroup Id=\"{0}\" Name=\"{1}\" Order=\"{2}\" Value=\"{5}\" Color=\"{6}\">" +
                                "<{7} Id=\"{3}\" Order=\"0\"></{7}>" +
                                "</ScoreGroup>",
                                idScoreGroup,
                                "",
                                xmlNode.ParentNode.ChildNodes.Count,
                                this.GetValue("Id"),
                                "",
                                xmlNode.ParentNode.ChildNodes.Count + 1,
                                ApplicationUtilities.Classes.ColorCalculator.GenerateRandomColor().ToString().Remove(0, 1),
                                this.TypeName
                            );
                        }

                        result = "ScoreGroup[@Id=\"" + idScoreGroup + "\"]";

                        SetLatestChange(xmlDocument);

                        xmlDocument.Save(this.Source);

                        //this.SetValue("Enabled", false);
                        //score2.SetValue("Enabled", false);
                    }

                    break;
            }

            return result;
        }

        public bool IsScoreGroup()
        {
            switch (this.StorageType)
            {
                case StorageMethodType.Database:

                    return this.GetChilds().Length > 0;

                    break;
                case StorageMethodType.Xml:

                    return this.TypeName == "ScoreGroup";

                    break;
            }

            return false;
        }


        public void Delete()
        {
            switch (this.StorageType)
            {
                case StorageMethodType.Database:

                    this.Core.Tables[this.Source].Delete(this.Path);

                    break;
                case StorageMethodType.Xml:

                    DefinitionObject[] scores = this.GetChilds();

                    foreach (DefinitionObject score in scores)
                    {
                        score.Delete();
                    }

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(this.Source);

                    XmlNode xmlNode = xmlDocument.SelectSingleNode(this.Path);

                    if (xmlNode != null)
                    {
                        xmlNode.ParentNode.RemoveChild(xmlNode);

                        SetLatestChange(xmlDocument);

                        xmlDocument.Save(this.Source);
                    }

                    break;
            }
        }


        public void SetLatestChange(XmlDocument xmlDocument)
        {
            XmlNode xmlNodeProperties = xmlDocument.DocumentElement.SelectSingleNode("Properties");

            if (xmlNodeProperties == null)
                return;

            XmlNode xmlNodeLatestUpdate = xmlNodeProperties.SelectSingleNode("LatestUpdate");

            if (xmlNodeLatestUpdate == null)
            {
                xmlNodeProperties.InnerXml += string.Format(
                    "<{0}>{1}</{0}>",
                    "LatestUpdate",
                    DateTime.Now.ToString()
                );
            }
            else
            {
                xmlNodeLatestUpdate.InnerText = DateTime.Now.ToString();
            }
        }


        public void Save()
        {
            if(this.StorageType == StorageMethodType.Xml)
            {
                this.XmlNode.OwnerDocument.Save(this.Source);
            }
        }

        #endregion
    }
}
