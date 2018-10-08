using DatabaseCore.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUtilities.Classes.Controls.GridClasses;

namespace WebUtilities.Controls
{
    public class GridControl<T> : BaseControl
    {
        #region Properties

        public string ID
        {
            get
            {
                return base.ID;
            }
            set
            {
                base.ID = value;

                this.Grid.ID = this.ID + "_Grid";
            }
        }

        public List<BaseItem<T>> Items { get; set; }

        public BaseItem<T> SelectedItem
        {
            get
            {
                if (this.Grid.SelectedItem == null)
                    return null;

                return this.Items.Find(x => x.Id == Guid.Parse(this.Grid.SelectedItem.ToString()));
            }
            set
            {
                this.Grid.SelectedItem = value.Id;
            }
        }

        public BaseCollection<T> CoreCollection { get; set; }

        public bool EnableDeleteButton { get; set; }

        public bool EnableEditButton { get; set; }

        public bool EnableAddButton { get; set; }

        public ConditionDeleteItem<T> DeleteCondition { get; set; }


        public Dictionary<string, GridHeadlineItemWidth> GridFields { get; set; }

        public List<string> Fields { get; set; }

        public List<AdditionalField<T>> AdditionalFields { get; set; }

        public List<Button> AdditionalButtons { get; set; }

        public List<string> GridFieldsSorting { get; set; }

        public Dictionary<string, string> GridRowBackgrounds { get; set; }

        public Dictionary<string, int> GridFieldsStringMaxLength { get; set; }


        public Grid Grid { get; set; }

        public Panel EditControl { get; set; }

        protected Box dragBox;
        ConfirmBox confirmBox;

        public event EventHandler OnSave;

        public bool HasAdditionalFieldError { get; set; }

        public Dictionary<string, object> HiddenAddValues { get; set; }

        #endregion


        #region Constructor

        public GridControl()
            : base()
        {
            this.HiddenAddValues = new Dictionary<string, object>();
            this.GridFieldsSorting = new List<string>();
            this.GridFieldsStringMaxLength = new Dictionary<string, int>();
            this.Items = new List<BaseItem<T>>();
            this.GridRowBackgrounds = new Dictionary<string, string>();

            this.Grid = new Grid();
            this.Grid.ID = this.ID + "_Grid";

            this.Fields = new List<string>();
            this.GridFields = new Dictionary<string, GridHeadlineItemWidth>();
            this.AdditionalFields = new List<AdditionalField<T>>();
            this.AdditionalButtons = new List<Button>();

            this.EnableDeleteButton = true;
            this.EnableEditButton = true;
            this.EnableAddButton = true;

            this.HasAdditionalFieldError = false;

            this.Load += UserManagementControl_Load;
            this.PreRender += UserManagementControl_PreRender;
        }

        #endregion


        #region Methods

        protected bool Validate(BaseItem<T> element, GridControlEditMode mode)
        {
            bool result = true;

            // Run through all fields.
            foreach (string field in this.Fields)
            {
                // Check if a string max value for this field is defined.
                if (!this.GridFieldsStringMaxLength.ContainsKey(field))
                    continue;

                // Get the item's value of this field.
                string value = element.GetValue<string>(field);

                // Check if the value's length is shorter than the max string length.
                if (value.Length > this.GridFieldsStringMaxLength[field])
                {
                    if (this.dragBox == null)
                        this.Controls.Add(BuildEditControl(mode));

                    string errorText = base.LanguageManager.GetText("MaxLengthExeeded").
                        Replace("###FIELD###", base.LanguageManager.GetText(field)).
                        Replace("###MAXLENGTH###", this.GridFieldsStringMaxLength[field].ToString());

                    this.dragBox.ShowError2(errorText);

                    result = false;
                }
            }

            // Run through all additional fields.
            foreach (AdditionalField<T> additionalField in this.AdditionalFields)
            {
                // Check if a string max value for this field is defined.
                if (!this.GridFieldsStringMaxLength.ContainsKey(additionalField.Name))
                    continue;

                // Check if the additional field is an edit field.
                if (!additionalField.ShowInEdit)
                    continue;

                string value = additionalField.GetValue(element as BaseItem<T>).ToString();

                // Check if the value's length is shorter than the max string length.
                if (value.Length > this.GridFieldsStringMaxLength[additionalField.Name])
                {
                    if (this.dragBox == null)
                        this.Controls.Add(BuildEditControl(mode));

                    string errorText = base.LanguageManager.GetText("MaxLengthExeeded").
                        Replace("###FIELD###", base.LanguageManager.GetText(additionalField.Name)).
                        Replace("###MAXLENGTH###", this.GridFieldsStringMaxLength[additionalField.Name].ToString());

                    this.dragBox.ShowError2(errorText);

                    result = false;
                }
            }

            if (this.HasAdditionalFieldError)
                result = false;

            // Return the result.
            return result;
        }

        public void ShowBoxError(string name)
        {
            this.HasAdditionalFieldError = true;

            if (this.dragBox == null)
                this.Controls.Add(BuildEditControl(GridControlEditMode.Add));

            // Check if a drag box is defined.
            if (this.dragBox == null)
                return;

            // Forward call.
            this.dragBox.ShowError(name);

            this.dragBox.Visible = true;
        }

        public void BuildGrid()
        {
            this.Grid.ID = this.ID + "_Grid";

            // Create the grid headline by the given fields as language labels.
            GridHeadline gridHeadline = new GridHeadline(this.Grid);

            int i = 0;

            // Run through all grid field names in the sorting.
            foreach (string fieldName in this.GridFieldsSorting)
            {
                if (this.GridFields.ContainsKey(fieldName))
                {
                    GridHeadlineItemWidth fieldWidth = this.GridFields[fieldName];

                    // Create a new headline item.
                    GridHeadlineItem headlineItem = new GridHeadlineItem(
                        gridHeadline,
                        i++,
                        fieldName,
                        fieldWidth,
                        true
                    );

                    // Add the headline item to the grid headline.
                    gridHeadline.Items.Add(headlineItem);
                }
                else if (this.AdditionalFields.Find(x => x.Name == fieldName) != null)
                {
                    AdditionalField<T> field = this.AdditionalFields.Find(x => x.Name == fieldName);

                    if (!field.ShowInGrid)
                        continue;

                    gridHeadline.Items.Add(new GridHeadlineItem(
                        gridHeadline,
                        i++,
                        field.Name,
                        field.Width,
                        true
                    ));
                }
            }

            // Set the grids headline.
            this.Grid.GridHeadline = gridHeadline;

            // Run through all items.
            foreach (BaseItem<T> item in this.Items)
            {
                // Create a new grid row for the item.
                GridRow row = new GridRow(this.Grid, item.Id);

                // Run through all grid field names in the sorting.
                foreach (string fieldName in this.GridFieldsSorting)
                {
                    if (this.GridFields.ContainsKey(fieldName))
                    {
                        // Get the items value of the field.
                        object value = item.GetValue(fieldName);

                        // Add the formated value as grid row item to the row.
                        row.Items.Add(new GridRowItem(row, Format(value)));
                    }
                    else if (this.AdditionalFields.Find(x => x.Name == fieldName) != null)
                    {
                        AdditionalField<T> field = this.AdditionalFields.Find(x => x.Name == fieldName);

                        if (!field.ShowInGrid)
                            continue;

                        // Get the items value of the field.
                        object value = field.GetValue.Invoke(item);

                        // Create a new row item with the formated value.
                        GridRowItem rowItem = new GridRowItem(row, Format(value));

                        // Add the row item to the grid row.
                        row.Items.Add(rowItem);

                        // Check if a background is set for this row.
                        if (this.GridRowBackgrounds.ContainsKey(item.Id + field.Name))
                        {
                            // Set the row's background color.
                            //row.Style.Add("background", this.GridRowBackgrounds[item.Id]);
                            rowItem.Style.Add("background", this.GridRowBackgrounds[item.Id + field.Name]);
                            rowItem.Style.Add("color", "#000000");
                        }
                    }
                }

                // Add the row to the grid's rows.
                this.Grid.Rows.Add(row);
            }
        }

        private Control BuildControl(string field, bool insertValues)
        {
            Control result = null;

            BaseItem<T> a = new BaseItem<T>();

            object value = a.GetValue(field);

            if (value == null && this.SelectedItem != null)
                value = this.SelectedItem.GetValue(field);

            TextBox txtFieldValue = new TextBox();

            if (insertValues && value != null)
                txtFieldValue.Text = value.ToString();

            result = txtFieldValue;

            Type fieldType = a.GetPropertyType(field);

            if (fieldType == typeof(bool) ||
                fieldType == typeof(bool?))
            {
                CheckBox chk = new CheckBox();

                if (insertValues)
                    chk.Checked = (bool)value;

                result = chk;
            }
            else if (fieldType == typeof(DateTime) ||
                fieldType == typeof(DateTime?))
            {
                DatePicker dp = new DatePicker();

                if (insertValues && value != null)
                    dp.Value = (DateTime)value;

                result = dp;
            }
            else if (fieldType == typeof(int) ||
                fieldType == typeof(int?))
            {
                NumericBox np = new NumericBox();

                if (insertValues && value != null)
                    np.Value = (int)value;

                result = np;
            }

            result.ID = field;

            return result;
        }

        private Panel BuildEditControl(GridControlEditMode mode)
        {
            List<Control> controls = new List<Control>();

            if (this.SelectedItem == null &&
                mode == GridControlEditMode.Edit)
                return new Panel();

            WebUtilities.Controls.Table table = new WebUtilities.Controls.Table();

            foreach (string field in this.Fields)
            {
                WebUtilities.Controls.TableRow tableRow =
                    new WebUtilities.Controls.TableRow();

                WebUtilities.Controls.TableCell tableCellFieldName =
                    new WebUtilities.Controls.TableCell();
                WebUtilities.Controls.TableCell tableCellFieldValue =
                    new WebUtilities.Controls.TableCell();

                Label lblFieldName = new Label();
                lblFieldName.Name = field;

                //object value = this.SelectedItem.GetValue(field);

                Control control = null;

                control = BuildControl(field, mode == GridControlEditMode.Edit);

                control.ID = this.ID + "_" + field;

                controls.Add(control);

                tableCellFieldName.Controls.Add(lblFieldName);
                tableCellFieldValue.Controls.Add(control);

                tableRow.Cells.Add(tableCellFieldName);
                tableRow.Cells.Add(tableCellFieldValue);

                table.Rows.Add(tableRow);
            }

            // Run through all additional fields.
            foreach (AdditionalField<T> field in this.AdditionalFields)
            {
                if (!field.ShowInEdit)
                    continue;

                if (field.Condition != null && field.Condition(this.SelectedItem) == false)
                    continue;

                WebUtilities.Controls.TableRow tableRow = new WebUtilities.Controls.TableRow();

                WebUtilities.Controls.TableCell tableCellFieldName =
                    new WebUtilities.Controls.TableCell();
                WebUtilities.Controls.TableCell tableCellFieldValue =
                    new WebUtilities.Controls.TableCell();

                Label lblFieldName = new Label();
                lblFieldName.Name = field.Name;

                Control value = field.Render.Invoke(
                    mode == GridControlEditMode.Edit ?
                    this.SelectedItem :
                    null
                );

                if (value == null)
                    continue;

                controls.Add(value);

                tableCellFieldName.Controls.Add(lblFieldName);
                tableCellFieldValue.Controls.Add(value);

                tableRow.Cells.Add(tableCellFieldName);
                tableRow.Cells.Add(tableCellFieldValue);

                table.Rows.Add(tableRow);
            }

            WebUtilities.Controls.TableRow tableRowButtons =
                new WebUtilities.Controls.TableRow();

            WebUtilities.Controls.TableCell tableCellButtons =
                new WebUtilities.Controls.TableCell();
            tableCellButtons.ColumnSpan = 2;
            tableCellButtons.HorizontalAlign = HorizontalAlign.Right;

            Button defaultButton = null;

            if (mode == GridControlEditMode.Add)
            {
                Button btnAddConfirm = new Button();
                btnAddConfirm.ID = this.ID + "BoxAdd";
                btnAddConfirm.Name = "Add";
                btnAddConfirm.Click += btnAddConfirm_Click;

                defaultButton = btnAddConfirm;

                tableCellButtons.Controls.Add(btnAddConfirm);
            }
            else
            {
                Button btnSave = new Button();
                btnSave.ID = this.ID + "BoxSave";
                btnSave.Name = "Save";
                btnSave.Click += btnSave_Click;

                defaultButton = btnSave;

                tableCellButtons.Controls.Add(btnSave);
            }

            Button btnCancel = new Button();
            btnCancel.Name = "Cancel";

            tableCellButtons.Controls.Add(btnCancel);

            tableRowButtons.Cells.Add(tableCellButtons);

            table.Rows.Add(tableRowButtons);

            dragBox = new Box();
            dragBox.ID = this.ID + mode.ToString();
            dragBox.Title = mode.ToString() + (this.Items.Count > 0 ? this.Items[0].GetType().Name : "");
            dragBox.TitleLanguageLabel = true;
            dragBox.Dragable = true;
            dragBox.Visible = true;

            dragBox.Controls.Add(table);

            Panel panel = new Panel();
            panel.Controls.Add(dragBox);

            foreach (Control control in controls)
            {
                if (control.GetType() == typeof(TextBox))
                {
                    TextBox textBox = (TextBox)control;

                    textBox.Button = defaultButton.ID;
                }
            }

            return panel;
        }


        protected void FireOnSave()
        {
            if (this.OnSave != null)
                this.OnSave(this, new EventArgs());
        }

        private void Save()
        {
            // Run through all fields.
            foreach (string field in this.Fields)
            {
                // The request name of the field.
                string name = this.ClientName + "_" + field;

                // Check if there is a request value for the field.
                if (HttpContext.Current.Request.Params[name] != null)
                {
                    object value = HttpContext.Current.Request.Params[name];

                    Type fieldType = this.SelectedItem.GetPropertyType(field);

                    if (fieldType == typeof(bool))
                        value = value.ToString() == "on";

                    if (fieldType == typeof(DateTime?) && value != null && value.ToString() == "")
                        value = DBNull.Value;

                    if (fieldType == typeof(DateTime?) || fieldType == typeof(DateTime) && value.ToString() != "")
                    {
                        DateTime dateValue;

                        if (DateTime.TryParse(value.ToString(), out dateValue))
                            //value = base.SurveyCore.Timezone.ConvertToServerTime(base.SurveyCore.Timezone.ConvertFromLocalTime(dateValue));
                            value = dateValue;
                        else
                            value = DBNull.Value;
                    }

                    // Set the new value for the field.
                    this.SelectedItem.SetValue(
                        field,
                        value
                    );
                }
                else if (this.SelectedItem.GetPropertyType(field) == typeof(bool))
                {
                    this.SelectedItem.SetValue(
                        field,
                        false
                    );
                }
            }

            // Run through all additional fields.
            foreach (AdditionalField<T> field in this.AdditionalFields)
            {
                if (!field.ShowInEdit)
                    continue;

                // Check if the field have an condition.
                if (field.Condition != null)
                {
                    // Check if the condition returns true.
                    if (field.Condition(this.SelectedItem) == false)
                        continue;
                }

                // The request name of the field.
                string name = this.ClientName + "_" + field.Name;

                object value = null;

                // Check if there is a request value for the field.
                if (HttpContext.Current.Request.Params[name] != null)
                {
                    // Get the value from the response.
                    value = HttpContext.Current.Request.Params[name];
                }

                // Set the new value for the field.
                field.SetValue(this.SelectedItem, value);
            }

            // Check if the values are valid.
            if (this.Validate(this.SelectedItem, GridControlEditMode.Edit))
            {
                // Save the changed values of the selected item.
                this.SelectedItem.Save();

                if (this.OnSave != null)
                    this.OnSave(this, new EventArgs());

                // Self redirect to refresh grid values.
                HttpContext.Current.Response.Redirect(
                    HttpContext.Current.Request.Url.ToString()
                );
            }
            else
            {
                dragBox.Visible = true;
            }
        }

        protected void Add()
        {
            // Create a new company.
            BaseItem<T> element = new BaseItem<T>(this.CoreCollection);

            // Run through all fields.
            foreach (string field in this.Fields)
            {
                // The request name of the field.
                string name = this.ClientName + "_" + field;

                // Get the type of the property field.
                Type fieldType = element.GetPropertyType(field);

                // Check if there is a request value for the field.
                if (HttpContext.Current.Request.Params[name] != null)
                {
                    object value = HttpContext.Current.Request.Params[name];

                    if (fieldType == typeof(bool))
                        value = value.ToString() == "on";

                    if (fieldType == typeof(DateTime?) && value != null && value.ToString() == "")
                        value = DBNull.Value;

                    if (fieldType == typeof(DateTime?) || fieldType == typeof(DateTime) && value.ToString() != "")
                    {
                        DateTime dateValue;

                        if (DateTime.TryParse(value.ToString(), out dateValue))
                            //value = base.SurveyCore.Timezone.ConvertToServerTime(base.SurveyCore.Timezone.ConvertFromLocalTime(dateValue));
                            value = dateValue;
                        else
                            value = DBNull.Value;
                    }

                    // Set the new value for the field.
                    element.SetValue(
                        field,
                        value
                    );
                }
                else if(fieldType == typeof(bool))
                {
                    // Set the new value for the field.
                    element.SetValue(
                        field,
                        false
                    );
                }
            }

            // Run through all hidden add values.
            foreach (KeyValuePair<string, object> value in this.HiddenAddValues)
            {
                // Set the value for the field.
                element.SetValue(
                    value.Key,
                    value.Value
                );
            }

            // Run through all additional fields.
            foreach (AdditionalField<T> field in this.AdditionalFields)
            {
                if (!field.ShowInEdit)
                    continue;

                // The request name of the field.
                string name = this.ClientName + "_" + field.Name;

                // Check if there is a request value for the field.
                if (HttpContext.Current.Request.Params[name] != null)
                {
                    // Set the new value for the field.
                    field.SetValue(element, HttpContext.Current.Request.Params[name]);
                }
            }

            if (Validate(element, GridControlEditMode.Add))
            {
                // Save the changed values of the selected item.
                element.Insert();

                FireOnSave();
                
                // Self redirect to refresh grid values.
                HttpContext.Current.Response.Redirect(
                    HttpContext.Current.Request.Url.ToString()
                );
            }
            else
            {
                this.dragBox.Visible = true;
            }
        }

        private string Format(object obj)
        {
            if (obj == null)
                return "";

            if (obj.GetType() == typeof(DateTime))
            {
                obj = ((DateTime)obj).ToString(
                    base.LanguageManager.GetText("DateFormat") + " " +
                    base.LanguageManager.GetText("TimeFormat")
                );
            }
            else if (obj.GetType() == typeof(bool))
            {
                obj = base.LanguageManager.GetText(obj.ToString());
            }

            return obj.ToString();
        }

        #endregion


        #region Event Handlers

        protected void UserManagementControl_Load(object sender, EventArgs e)
        {
            BuildGrid();

            Panel pnlAdditionalButtons = new Panel();
            pnlAdditionalButtons.CssClass = "PanelAdditionalButtons";

            foreach (Button additionalButton in this.AdditionalButtons)
            {
                pnlAdditionalButtons.Controls.Add(additionalButton);
            }

            Panel pnlButtons = new Panel();
            pnlButtons.CssClass = "PanelButtons";

            Button btnDelete = new Button();
            Button btnEdit = new Button();
            Button btnAdd = new Button();

            btnDelete.Attributes.Add("Mode", "ActiveOnly");
            btnEdit.Attributes.Add("Mode", "ActiveOnly");
            btnAdd.Attributes.Add("Mode", "Always");

            btnDelete.CssClass = "DeleteButton";

            btnDelete.ID = this.ID + "_Delete";
            btnEdit.ID = this.ID + "_Edit";
            btnAdd.ID = this.ID + "_Add";

            btnDelete.Name = "Delete";
            btnEdit.Name = "Edit";
            btnAdd.Name = "Add";

            btnDelete.Style.Add("display", "none");
            btnEdit.Style.Add("display", "none");

            btnDelete.Click += btnDelete_Click;
            btnEdit.Click += btnEdit_Click;
            btnAdd.Click += btnAdd_Click;

            if (this.EnableDeleteButton)
                pnlButtons.Controls.Add(btnDelete);

            if (this.EnableEditButton)
            {
                pnlButtons.Controls.Add(btnEdit);

                //this.Grid.DoubleClick += btnEdit_Click;
            }

            if (this.EnableAddButton)
                pnlButtons.Controls.Add(btnAdd);

            this.Controls.Add(this.Grid);
            this.Controls.Add(pnlAdditionalButtons);
            this.Controls.Add(pnlButtons);

            if (HttpContext.Current.Request.Params[this.ClientName + "BoxSave"] != null)
                Save();

            if (HttpContext.Current.Request.Params[this.ClientName + "BoxAdd"] != null)
                Add();

            if (HttpContext.Current.Request.Params[this.ClientName + "ConfirmBox_btnWarningBoxConfirm"] != null)
                btnDelete_Click(sender, e);

            this.Controls.Add(new LiteralControl("<div style=\"clear:both\"></div>"));

            string scriptAdditionalButtons = "";

            foreach (Button btn in this.AdditionalButtons)
            {
                btn.Style.Add("display", "none");

                scriptAdditionalButtons += "obj.Buttons.push('" + btn.ClientID + "');";
            }

            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                this.ID + "SetGridButtons",
                "loadFunctions.push(function() {" +
                "var obj = new Object();" +
                "obj.IdGrid = '" + this.Grid.ClientID + "';" +
                "obj.Buttons = new Array();" +
                "obj.Buttons.push('" + btnDelete.ClientID + "');" +
                "obj.Buttons.push('" + btnEdit.ClientID + "');" +
                "obj.Buttons.push('" + btnAdd.ClientID + "');" +
                scriptAdditionalButtons +
                "gridButtons.push(obj);" +
                "});",
                true
            );
        }

        protected void UserManagementControl_PreRender(object sender, EventArgs e)
        {
            if (dragBox != null)
                dragBox.Visible = true;

            if (confirmBox != null)
                confirmBox.Visible = true;
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.Grid.SelectedItem == null)
                return;

            if (this.DeleteCondition != null && this.DeleteCondition(this.SelectedItem) == false)
                return;

            confirmBox = new ConfirmBox();
            confirmBox.ID = this.ID + "ConfirmBox";
            confirmBox.Title = "Delete" + (this.Items.Count > 0 ? this.Items[0].GetType().Name : "");

            Guid idCompany = Guid.Parse(this.Grid.SelectedItem.ToString());

            BaseItem<T> element = this.CoreCollection.GetSingle(idCompany) as BaseItem<T>;

            if (element == null)
                return;

            confirmBox.Text = string.Format(
                base.LanguageManager.GetText("AreYouSure2"),
                base.LanguageManager.GetText("SpellingArticle2"),
                base.LanguageManager.GetText(element.GetType().Name),
                element.GetValue("Name")
            );

            confirmBox.Confirm = delegate()
            {
                this.CoreCollection.Delete(element.Id);
            };

            this.Controls.Add(confirmBox);
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            //this.SelectedItem = this.Items[0];

            this.Controls.Add(BuildEditControl(GridControlEditMode.Edit));
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            this.Controls.Add(BuildEditControl(GridControlEditMode.Add));
        }


        protected void btnAddConfirm_Click(object sender, EventArgs e)
        {
        }

        #endregion
    }

    public delegate bool ConditionDeleteItem<T>(BaseItem<T> element);


    public delegate object GetAdditionalFieldValue<T>(BaseItem<T> element);

    public delegate bool ConditionAdditionalFieldValue<T>(BaseItem<T> element);

    public delegate Control RenderAdditionalField<T>(BaseItem<T> element);

    public delegate void SetAdditionalFieldValue<T>(BaseItem<T> element, object value);

    public class AdditionalField<T>
    {
        #region Properties

        public string Name { get; set; }

        public bool ShowInGrid { get; set; }

        public bool ShowInEdit { get; set; }

        public GridHeadlineItemWidth Width { get; set; }

        public ConditionAdditionalFieldValue<T> Condition { get; set; }

        public GetAdditionalFieldValue<T> GetValue { get; set; }

        public SetAdditionalFieldValue<T> SetValue { get; set; }

        public RenderAdditionalField<T> Render { get; set; }

        #endregion


        #region Constructor

        public AdditionalField(bool showInGrid = false, bool showInEdit = true)
        {
            this.ShowInGrid = showInGrid;
            this.ShowInEdit = showInEdit;
        }

        public AdditionalField(int stringMaxLength = 0, bool showInGrid = false, bool showInEdit = true)
            : this(showInGrid, showInEdit)
        {

        }

        #endregion
    }

    public enum GridControlEditMode
    {
        Add, Edit
    }
}
