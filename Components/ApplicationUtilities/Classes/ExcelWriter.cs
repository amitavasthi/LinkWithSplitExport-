using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ApplicationUtilities.Classes
{
    public class ExcelWriter
    {
        #region Properties

        /// <summary>
        /// Gets or sets the current position in the file.
        /// </summary>
        public int Position { get; set; }

        public IWorkbook Workbook { get; set; }

        /// <summary>
        /// Gets or sets the active sheet to read from.
        /// </summary>
        public IWorksheet ActiveSheet { get; set; }

        /// <summary>
        /// Gets or sets a list of all merge ranges in the sheet.
        /// </summary>
        public List<ExcelMergeRange> Merges { get; set; }

        public string FileName { get; set; }

        #endregion


        #region Constructor

        public ExcelWriter(string fileName = null)
        {
            this.FileName = fileName;
            this.Merges = new List<ExcelMergeRange>();
            this.Position = 0;

            OpenFile();
        }

        #endregion


        #region Methods

        private void OpenFile()
        {
            // Check if an existing file should be opened.
            if (string.IsNullOrEmpty(this.FileName) == false && File.Exists(this.FileName))
            {
                // Open the source workbook file.
                this.Workbook = Factory.GetWorkbook(this.FileName);
            }
            else
            {
                // Open the source workbook file.
                this.Workbook = Factory.GetWorkbook();
            }

            // Set the active sheet to the first sheet in the workbook.
            this.ActiveSheet = this.Workbook.Worksheets[0];
        }

        public void NewLine()
        {
            this.Position++;
        }

        public void NewSheet(string name = null)
        {
            // Add a new worksheet to the end.
            this.Workbook.Worksheets.Add();

            // Set the new worksheet as active worksheet.
            this.ActiveSheet = this.Workbook.Worksheets[
                this.Workbook.Worksheets.Count - 1
            ];

            if (name != null)
            {
                if (name.Length > 31)
                    name = name.Substring(0, 31);

                this.ActiveSheet.Name = name;
                this.Position = 0;
            }
        }


        public void Merge(int rowOffset1, int columnOffset1, int rowOffset2, int columnOffset2)
        {
            int newColumnOffset1 = GetColumnOffset(rowOffset1, columnOffset1);
            columnOffset2 = GetColumnOffset(rowOffset2, columnOffset2) + (newColumnOffset1 - columnOffset1);
            columnOffset1 = newColumnOffset1;

            this.Merges.Add(new ExcelMergeRange(
                rowOffset1,
                columnOffset1,
                rowOffset2,
                columnOffset2
            ));

            try
            {
                this.ActiveSheet.Cells[rowOffset1, columnOffset1, rowOffset2, columnOffset2].Merge();
            }
            catch { }
        }

        public int Write(int columnOffset, string value)
        {
            try
            {
                columnOffset = GetColumnOffset(this.Position, columnOffset);

                this.ActiveSheet.Cells[this.Position, columnOffset].Formula = value;
            }
            catch (Exception ex)
            {
                throw;
            }
            return columnOffset;
        }

        public void Write(int columnOffset, int rowOffset, string value)
        {
            columnOffset = GetColumnOffset(rowOffset, columnOffset);

            if (value != null)
                this.ActiveSheet.Cells[rowOffset, columnOffset].Formula = value;
        }

        public void AutoFit()
        {
            int i = 0;

            while (true)
            {
                if (this.ActiveSheet.Cells[0, i].Formula == "")
                    break;

                try
                {
                    this.ActiveSheet.Cells[0, i].EntireColumn.AutoFit();
                }
                catch { }

                i++;
            }
        }

        public void Save(string fileName)
        {
            this.Workbook.SaveAs(fileName, FileFormat.OpenXMLWorkbook);
        }

        public byte[] Save()
        {
            // Get a temp file name to store the workbook.
            string fileName = Path.GetTempFileName() + ".xlsx";

            // Save the workbook to the temp file name.
            Save(fileName);

            // Get the data of the workbook.
            byte[] buffer = File.ReadAllBytes(fileName);

            // Delete the temp file.
            File.Delete(fileName);

            // Return the buffer containing the workbook.
            return buffer;
        }


        private int GetColumnOffset(int rowOffset, int columnOffset)
        {
            // Run through all merge ranges.
            foreach (ExcelMergeRange range in this.Merges)
            {
                // Check if the merge affects the column.
                columnOffset = range.GetColumnOffset(rowOffset, columnOffset);
            }

            return columnOffset;
        }

        #endregion


        #region Operators

        public string this[int column]
        {
            get
            {
                return this.ActiveSheet.Cells[this.Position, column].Formula.Trim();
            }
        }

        #endregion
    }

    public class ExcelMergeRange
    {
        #region Properties

        public int RowOffset1 { get; set; }
        public int RowOffset2 { get; set; }

        public int ColumnOffset1 { get; set; }
        public int ColumnOffset2 { get; set; }

        #endregion


        #region Constructor

        public ExcelMergeRange(int rowOffset1, int columnOffset1, int rowOffset2, int columnOffset2)
        {
            this.RowOffset1 = rowOffset1;
            this.RowOffset2 = rowOffset2;
            this.ColumnOffset1 = columnOffset1;
            this.ColumnOffset2 = columnOffset2;
        }

        #endregion


        #region Methods

        public int GetColumnOffset(int rowOffset, int columnOffset)
        {
            // Check if the row is affected.
            if (rowOffset >= this.RowOffset1 && rowOffset <= this.RowOffset2)
            {
                // Check if the column is affected.
                if (columnOffset >= this.ColumnOffset1 && columnOffset <= this.ColumnOffset2)
                {
                    //return (this.ColumnOffset2 - columnOffset) + 1;
                    return this.ColumnOffset2 + 1;
                }
            }

            return columnOffset;
        }

        #endregion


        #region Event Handlers

        #endregion
    }
}