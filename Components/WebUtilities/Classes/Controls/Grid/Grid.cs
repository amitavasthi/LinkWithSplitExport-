using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUtilities.Classes.Controls.GridClasses;

namespace WebUtilities.Controls
{
    public class Grid : BaseControl
    {
        #region Properties

        public Grid Session
        {
            get
            {
                return (Grid)HttpContext.Current.Session["Grid" + this.ClientID];
            }
            set
            {
                HttpContext.Current.Session["Grid" + this.ClientID] = value;
            }
        }

        public GridHeadline GridHeadline { get; set; }

        public List<GridRow> Rows { get; protected set; }

        private string ProjectId
        {
            get
            {
                string result = "";

                if (HttpContext.Current.Session["IdProject"] != null)
                {
                    result = HttpContext.Current.Session["IdProject"].ToString();
                }

                return result;
            }
        }

        public object SelectedItem
        {
            get
            {
                if (HttpContext.Current.Session["Grid_" + this.ID + "_" + this.ProjectId + "_SelectedItem"] == null)
                    return null;

                return HttpContext.Current.Session["Grid_" + this.ID + "_" + this.ProjectId + "_SelectedItem"];
            }
            set
            {
                HttpContext.Current.Session["Grid_" + this.ID + "_" + this.ProjectId + "_SelectedItem"] = value;
            }
        }

        public int MaxHeight { get; set; }

        public bool AutoPostBack { get; set; }

        public event EventHandler SelectedIndex_Changed;

        public event EventHandler DoubleClick;

        public int ScrollOffset
        {
            get
            {
                if (HttpContext.Current.Session["Grid_" + this.ID + "_" + this.ProjectId + "ScrollOffset"] == null)
                    return 0;

                return (int)HttpContext.Current.Session["Grid_" + this.ID + "_" + this.ProjectId + "ScrollOffset"];
            }
            set
            {
                HttpContext.Current.Session["Grid_" + this.ID + "_" + this.ProjectId + "ScrollOffset"] = value;
            }
        }

        public Grid DependingGrid { get; set; }

        #endregion


        #region Constructor

        public Grid()
            : base("div")
        {
            this.Rows = new List<GridRow>();

            this.Load += Grid_Load;
            this.PreRender += Grid_PreRender;
        }

        #endregion


        #region Methods

        public void Select()
        {
            if (this.SelectedIndex_Changed != null)
                this.SelectedIndex_Changed(this, new EventArgs());
        }

        public void Build(bool limit = true)
        {
            this.CssClass = "Grid";
            this.Attributes.Add("ItemCount", this.Rows.Count.ToString());
            if (this.Rows.Count > 20)
            {
                this.GridHeadline.Style.Add("padding-right", "8px");
            }
            HiddenField hiddenFieldScroll = new HiddenField();
            hiddenFieldScroll.ID = this.ID + "ScrollOffset";

            this.Controls.Add(hiddenFieldScroll);

            if (HttpContext.Current.Request.Params[this.ClientName + "ScrollOffset"] != null)
            {
                int scrollOffset;

                if (int.TryParse(HttpContext.Current.Request.Params[this.ClientName + "ScrollOffset"], out scrollOffset))
                {
                    this.ScrollOffset = scrollOffset;
                }
            }

            this.Controls.Add(new LiteralControl("<gridsection>"));

            this.Controls.Add(this.GridHeadline);

            if (Page == null)
                this.GridHeadline.Build();

            this.Controls.Add(new LiteralControl("</gridsection>"));

            if (this.MaxHeight == 0)
                this.MaxHeight = 300;

            this.Controls.Add(new LiteralControl("<div class=\"GridOverflow\" style=\"max-height:" + this.MaxHeight + "px\" onscroll=\"GridScroll('" +
                hiddenFieldScroll.ClientID + "', this,'" + this.ClientID + "');\"><gridsection>"));

            int limitCount = 20;

            int index = this.Rows.FindIndex(x => x.Identity == this.SelectedItem);
            if (index > limitCount)
                limitCount = index + 5;

            int i = 0;
            foreach (GridRow row in this.Rows)
            {
                if (i % 2 == 1)
                    row.CssClass = "GridRow2";

                this.Controls.Add(row);

                //if (limit && i >= limitCount)
                //    break;

                i++;

                if (Page == null)
                    row.Build();
            }

            this.Controls.Add(new LiteralControl("</gridsection></div>"));

            if (HttpContext.Current.Request.Params[this.ID] != null)
            {
                Guid idObject;

                if (Guid.TryParse(HttpContext.Current.Request.Params[this.ID], out idObject))
                {
                    this.SelectedItem = idObject;
                }
                else
                {
                    this.SelectedItem = HttpContext.Current.Request.Params[this.ID];
                }

                Select();
            }

            hiddenFieldScroll.Value = this.ScrollOffset.ToString();

            if (Page != null)
            {
                Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    this.ID + "PushGridIds",
                    "gridIds.push('" + this.ClientID + "');",
                    true
                );

                Page.ClientScript.RegisterStartupScript(this.GetType(), this.ID + "SetScrollOffset",
                    "loadFunctions.push(function() { GetChildByAttribute(document.getElementById('" + this.ClientID +
                    "'), 'class', 'GridOverflow').scrollTop " +
                    "= parseInt(document.getElementById('" + hiddenFieldScroll.ClientID +
                    "').value); });", true);
            }

            if (this.SelectedItem == null && this.Rows.Count > 0)
            {
                this.SelectedItem = this.Rows[0].Identity;

                Select();
            }
        }

        /*
        public byte[] Export(System.Drawing.Color color, System.Drawing.Color color2)
        {
            // Create a new spreadsheet gear workbook.
            IWorkbook workbook = Factory.GetWorkbook();
            IWorksheet worksheet = workbook.Worksheets["Sheet1"];
            IRange cells = worksheet.Cells;

            // Run through all grid headline items.
            for (int i = 0; i < this.GridHeadline.Items.Count; i++)
            {
                // Get the grid headline item by the index.
                GridHeadlineItem item = this.GridHeadline.Items[i];

                // Set the cell's text to the grid headline item's text.
                cells[0, i].Formula = item.Text;

                cells[0, i].EntireColumn.ColumnWidth = 150;

                // Set the cell's background color.
                cells[0, i].Interior.Color = color;

                // Set the cell's font color.
                cells[0, i].Font.Color = color2;

                // Set the cell's font size to 14.
                cells[0, i].Font.Size = 14;

                cells[0, i].Borders.Color = color2;
            }

            // Run through all grid rows.
            for (int r = 0; r < this.Rows.Count; r++)
            {
                GridRow row = this.Rows[r];

                // Run through all grid row items.
                for (int i = 0; i < row.Items.Count; i++)
                {
                    GridRowItem item = row.Items[i];

                    // Get the item's text.
                    string text = item.Text;

                    // Remove all html tags from the text.
                    text = System.Text.RegularExpressions.Regex.
                        Replace(text, "<.*?>", string.Empty);

                    // Set the cell's text to the prepared text.
                    cells[r + 1, i].Formula = text;

                    // Enable the cell's text wrap.
                    cells[r + 1, i].WrapText = true;

                    cells[r + 1, i].Borders.Color = color;

                    if (item.Style["background"] != null)
                    {
                        System.Drawing.Color backgroundColor = System.Drawing.
                            ColorTranslator.FromHtml(item.Style["background"]);

                        cells[r + 1, i].Interior.Color = backgroundColor;
                    }
                }
            }

            // Run for all grid headline items length.
            for (int i = 0; i < this.GridHeadline.Items.Count; i++)
            {
                // Auto fit the entire column.
                cells[0, i].EntireColumn.AutoFit();

                // Increase the auto fitted column width by 5.
                cells[0, i].EntireColumn.ColumnWidth += 5;
            }

            // Create a new memory stream to save the workbook.
            MemoryStream memoryStream = new MemoryStream();

            // Save the workbook to the memory stream.
            workbook.SaveToStream(memoryStream, FileFormat.Excel8);

            // Get the byte array of the memory stream.
            byte[] result = memoryStream.GetBuffer();

            // Dispose the memory stream.
            memoryStream.Dispose();

            // Return the byte array.
            return result;
        }
        */
        #endregion


        #region Event Handlers

        protected void Grid_Load(object sender, EventArgs e)
        {
            Build();
        }

        protected void Grid_PreRender(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.Params["GridDoubleClick" + this.ID] != null)
            {
                if (this.DoubleClick != null)
                    this.DoubleClick(this, new EventArgs());
            }

            this.Session = this;
        }

        #endregion
    }

    public class GridHeadlineItemWidth
    {
        #region Properties

        public int Width { get; set; }

        #endregion


        #region Constructor

        public GridHeadlineItemWidth(int width)
        {
            this.Width = width;
        }

        #endregion
    }
}
