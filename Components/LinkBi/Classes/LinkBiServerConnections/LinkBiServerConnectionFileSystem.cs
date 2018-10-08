using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WebUtilities.Controls;

namespace LinkBi1.Classes.LinkBiServerConnections
{
    public class LinkBiServerConnectionFileSystem : LinkBiServerConnection
    {
        #region Properties

        /// <summary>
        /// Gets or sets the file path where the
        /// LinkBi results should be stored at.
        /// </summary>
        public string FilePath
        {
            get
            {
                return base.GetValue("FilePath", "");
            }
            set
            {
                base.SetValue("FilePath", value);
            }
        }

        #endregion


        #region Constructor

        public LinkBiServerConnectionFileSystem(LinkBiDefinition owner, XmlNode xmlNode)
            : base(owner, xmlNode)
        { }

        #endregion


        #region Methods

        public override TableRow[] Render()
        {
            TableRow[] result = new TableRow[1];

            TableCell tableCellFilePathTitle = new TableCell();
            TableCell tableCellFilePathValue = new TableCell();

            tableCellFilePathTitle.Text = base.LanguageManager.GetText("LinkBiServerConnectionFileSystemFilePath");

            TextBox txtFilePath = new TextBox();
            txtFilePath.Text = this.FilePath;

            txtFilePath.Attributes.Add("onchange", string.Format(
                "UpdateServerConnectionProperty('{0}' ,'{1}', '{2}', '{3}', this.value);",
                this.XmlNode.Attributes["Id"].Value,
                this.Owner.FileName.Replace("\\", "/"),
                this.XmlNode.GetXPath(),
                "FilePath"
            ));

            tableCellFilePathValue.Controls.Add(txtFilePath);

            result[0] = new TableRow();
            result[0].Cells.Add(tableCellFilePathTitle);
            result[0].Cells.Add(tableCellFilePathValue);

            return result;
        }

        public override bool Deploy(string fileName)
        {
            try
            {
                File.Copy(
                    fileName,
                    this.FilePath,
                    true
                );

                base.Outdated = false;
                base.LatestDeploy = DateTime.Now;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool IsValid()
        {
            try
            {
                FileInfo fInfo = new FileInfo(this.FilePath);

                return fInfo.Directory.Exists;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
