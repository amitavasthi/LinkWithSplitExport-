using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace MasterPage.Classes
{
    public class VirtualFileMasterPage : VirtualFile
    {
        #region Properties

        /// <summary>
        /// Gets or sets the virtual path of the virtual file.
        /// </summary>
        public string VirtualPath { get; set; }

        #endregion


        #region Constructor

        public VirtualFileMasterPage(string virtualPath)
            : base(virtualPath)
        {
            this.VirtualPath = virtualPath;
        }

        #endregion


        #region Methods

        public override Stream Open()
        {
            if (!(HttpContext.Current == null))
            {
                if (HttpContext.Current.Cache[this.VirtualPath] == null)
                {
                    HttpContext.Current.Cache.Insert(
                        this.VirtualPath, 
                        ReadResource(this.VirtualPath)
                    );
                }
                return (Stream)HttpContext.Current.Cache[this.VirtualPath];
            }
            else
            {
                return ReadResource(this.VirtualPath);
            }
        }

        private static Stream ReadResource(string embeddedFileName)
        {
            string resourceFileName = VirtualPathUtility.GetFileName(embeddedFileName);
            Assembly assembly = Assembly.GetExecutingAssembly();

            return assembly.GetManifestResourceStream(MasterPageVirtualPathProvider.VirtualPathProviderResourceLocation + "." + resourceFileName);
        }

        #endregion
    }
}
