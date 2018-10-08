using Crosstables.Classes.ReportDefinitionClasses;
using LinkBi1.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebUtilities.Controls;

namespace Homescreen1.Classes.Modules
{
    public class HomescreenModuleChat : HomescreenModule
    {
        #region Properties

        /// <summary>
        /// Gets or sets the count of items.
        /// </summary>
        public int ItemCount { get; set; }

        #endregion


        #region Constructor

        public HomescreenModuleChat(HomescreenNode node)
            : base(node)
        { }

        #endregion


        #region Methods

        public override void Render(StringBuilder writer)
        {
            string fileName = Path.Combine(
                HttpContext.Current.Request.PhysicalApplicationPath,
                "Fileadmin",
                "Chat",
                ((DatabaseCore.Core)HttpContext.Current.Session["Core"]).ClientName,
                "Global.xml"
            );

            Chat chat = new Chat(fileName);
            chat.ID = "chatHomescreenSocial";
            chat.ControlHeight = this.Height;
            //chat.Style.Add("height", this.Node.Height + "px");
            chat.Style.Add("HeightScript", this.Node.Height);

            chat.Render();

            writer.Append(chat.ToHtml());
        }

        #endregion
    }
}
