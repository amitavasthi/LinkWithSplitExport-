using LinkingHelper2.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinkingHelper2.Forms
{
    public partial class WordMatchView : Form
    {
        #region Properties

        public WordMatch WordMatch { get; set; }

        #endregion


        #region Constructor

        public WordMatchView(WordMatch wordMatch)
        {
            this.WordMatch = wordMatch;
            InitializeComponent();
        }

        #endregion


        #region Methods

        private string Match(string label1, string label2)
        {
            string[] ignoreWords = new string[]
            {
                ":",
                ".",
                ",",
                "-"
            };

            StringBuilder result = new StringBuilder();

            string[] words1 = label1.Split(' ');
            string[] words2 = label2.Split(' ');

            foreach (string word1 in words1)
            {
                if (ignoreWords.Contains(word1))
                    continue;

                if (!words2.Contains(word1))
                {
                    result.Append("<span style=\"color:#FF0000\">");
                }

                result.Append(word1 + " ");

                if (!words2.Contains(word1))
                {
                    result.Append("</span>");
                }
            }

            return result.ToString();
        }

        #endregion


        #region Event Handlers

        private void WordMatchView_Load(object sender, EventArgs e)
        {
            if (this.WordMatch == null)
                return;

            string physicalApplicationPath = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location
            );

            File.WriteAllText(Path.Combine(
                physicalApplicationPath,
                "Html1.html"
            ), Match(
                this.WordMatch.LinkedVariable.Label,
                this.WordMatch.UnlinkedVariable.Label
            ));

            File.WriteAllText(Path.Combine(
                physicalApplicationPath,
                "Html2.html"
            ), Match(
                this.WordMatch.UnlinkedVariable.Label,
                this.WordMatch.LinkedVariable.Label
            ));

            webBrowser1.Navigate("file:///" + physicalApplicationPath.Replace("\\", "/") + "/" + "Html1.html");
            webBrowser2.Navigate("file:///" + physicalApplicationPath.Replace("\\", "/") + "/" + "Html2.html");
        }

        #endregion
    }
}
