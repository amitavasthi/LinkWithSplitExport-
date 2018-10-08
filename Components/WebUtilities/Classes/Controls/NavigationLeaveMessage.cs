using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WebUtilities.Controls
{
    public class NavigationLeaveMessage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the xml node that contains the
        /// navigation leave message's definitions.
        /// </summary>
        public XmlNode XmlNode { get; set; }

        /// <summary>
        /// Gets or sets the name of the leave message.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets which buttons the leave message should have.
        /// </summary>
        public LeaveMessageButtons Buttons { get; set; }

        #endregion


        #region Constructor

        public NavigationLeaveMessage(XmlNode xmlNode)
        {
            this.XmlNode = xmlNode;

            this.Parse();
        }

        #endregion


        #region Methods

        private void Parse()
        {
            this.Name = this.XmlNode.Attributes["LeaveMessageName"].Value;

            this.Buttons = (LeaveMessageButtons)Enum.Parse(
                typeof(LeaveMessageButtons),
                this.XmlNode.Attributes["LeaveMessageButtons"].Value
            );
        }

        public string RenderScript()
        {
            StringBuilder result = new StringBuilder();

            result.Append(string.Format(
                "ShowLeaveMessage(\"{0}\", \"{1}\", \"{2}\" ,\"{3}\");",
                this.Name,
                this.Buttons,
                this.XmlNode.Attributes["LeaveMessageActionPositive"].Value,
                this.XmlNode.Attributes["LeaveMessageActionNegative"].Value
            ));

            return result.ToString();
        }

        #endregion
    }

    public enum LeaveMessageButtons
    {
        YesNo,
        ConfirmCancel
    }
}
