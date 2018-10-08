using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationUtilities.Classes
{
    public class ExcelReader
    {
        #region Properties

        /// <summary>
        /// Gets or sets the full path of the source file to read.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the current position in the file.
        /// </summary>
        public int Position { get; set; }

        public IWorkbook Workbook { get; set; }

        /// <summary>
        /// Gets or sets the active sheet to read from.
        /// </summary>
        public IWorksheet ActiveSheet { get; set; }

        #endregion


        #region Constructor

        public ExcelReader(string source)
        {
            this.Source = source;
            this.Position = 0;
            
            OpenFile();
        }

        #endregion


        #region Methods

        private void OpenFile()
        {
            // Open the source workbook file.
            this.Workbook = Factory.GetWorkbook(this.Source);

            // Set the active sheet to the first sheet in the workbook.
            this.ActiveSheet = this.Workbook.Worksheets[0];
        }

        public bool Read()
        {
            // Check if the reader is at the end of the document.
            if (this.ActiveSheet.Cells[this.Position, 0].Formula == "")
                return false;

            this.Position++;

            return true;
        }

        #endregion


        #region Operators

        public string this[int position, int column]
        {
            get
            {
                return this.ActiveSheet.Cells[position, column].Formula.Trim();
            }
        }

        public string this[int column]
        {
            get
            {
                return this.ActiveSheet.Cells[this.Position, column].Formula.Trim();
            }
        }

        #endregion
    }
}