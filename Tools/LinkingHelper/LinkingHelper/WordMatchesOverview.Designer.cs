namespace LinkingHelper
{
    partial class WordMatchesOverview
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gridView = new System.Windows.Forms.DataGridView();
            this.Name1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Name2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Label1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Label2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Match = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.SuspendLayout();
            // 
            // gridView
            // 
            this.gridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Name1,
            this.Name2,
            this.Label1,
            this.Label2,
            this.Match});
            this.gridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridView.Location = new System.Drawing.Point(0, 0);
            this.gridView.Name = "gridView";
            this.gridView.Size = new System.Drawing.Size(895, 474);
            this.gridView.TabIndex = 0;
            // 
            // Name1
            // 
            this.Name1.HeaderText = "Name1";
            this.Name1.Name = "Name1";
            this.Name1.ReadOnly = true;
            // 
            // Name2
            // 
            this.Name2.HeaderText = "Name2";
            this.Name2.Name = "Name2";
            this.Name2.ReadOnly = true;
            // 
            // Label1
            // 
            this.Label1.HeaderText = "Label1";
            this.Label1.Name = "Label1";
            this.Label1.ReadOnly = true;
            // 
            // Label2
            // 
            this.Label2.HeaderText = "Label2";
            this.Label2.Name = "Label2";
            this.Label2.ReadOnly = true;
            // 
            // Match
            // 
            this.Match.HeaderText = "Match";
            this.Match.Name = "Match";
            this.Match.ReadOnly = true;
            // 
            // WordMatchesOverview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(895, 474);
            this.Controls.Add(this.gridView);
            this.Name = "WordMatchesOverview";
            this.Text = "WordMatchesOverview";
            this.Load += new System.EventHandler(this.WordMatchesOverview_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Name1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Name2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Match;
    }
}