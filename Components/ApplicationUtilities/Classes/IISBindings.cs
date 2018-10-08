using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Administration;


namespace ApplicationUtilities.Classes
{
    public class IISBindings
    {
        #region Methods

        /// <summary>
        /// Remove the site Bindings from the IIS.
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="HostName"></param>
        public void RemoveBindings(string siteName, string HostName)
        {
            using (Microsoft.Web.Administration.ServerManager sManager = new ServerManager())
            {
                foreach (var site in sManager.Sites)
                {
                    if (site.Name == siteName.Trim())
                    {
                        foreach (Binding binding in site.Bindings.ToList())
                        {
                            if (binding.Host == HostName.Trim())
                                site.Bindings.Remove(binding);
                        }
                    }
                }
                sManager.CommitChanges();
            }

        }


        /// <summary>
        /// Add the site Bindings to the IIS.
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="HostName"></param>
        public void AddBindings(string siteName, string HostName)
        {
            using (Microsoft.Web.Administration.ServerManager sManager = new ServerManager())
            {
                foreach (var site in sManager.Sites)
                {
                    if (site.Name == siteName.Trim())
                    {
                        string ipAddress = "*";

                        try
                        {
                            Binding httpsBinding = site.Bindings.CreateElement("binding");
                            httpsBinding["protocol"] = "https";
                            httpsBinding["CertificateHash"] = getCertHash(siteName);
                            httpsBinding["CertificateStoreName"] = getCertStoreName(siteName);

                            httpsBinding["bindingInformation"] = string.Format(@"{0}:{1}:{2}", ipAddress, "443", HostName);
                            site.Bindings.Add(httpsBinding);
                        }
                        catch { }

                        Binding httpBinding = site.Bindings.CreateElement("binding");
                        int port = 80;
                        httpBinding["protocol"] = "http";
                        httpBinding.BindingInformation = string.Format("{0}:{1}:{2}", ipAddress, port, HostName);
                        site.Bindings.Add(httpBinding);
                    }
                }
                sManager.CommitChanges();
            }
        }

        /// <summary>
        /// To get the installed certificate hash code for the particular site.
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns></returns>
        private string getCertHash(string siteName)
        {
            string certHash = "";
            using (ServerManager serverManager = new ServerManager())
            {
                Site site = serverManager.Sites.Where(s => s.Name == siteName).Single();
                foreach (Binding binding in site.Bindings)
                {
                    if (binding.Protocol == "https")
                    {
                        certHash += BitConverter.ToString(binding.CertificateHash).Replace("-", string.Empty);
                    }
                }
            }
            return certHash;
        }
        /// <summary>
        /// To get the installed certificate store name for the particular site.
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns></returns>
        private string getCertStoreName(string siteName)
        {
            string certSName = "";
            using (ServerManager serverManager = new ServerManager())
            {
                Site site = serverManager.Sites.Where(s => s.Name == siteName).Single();
                foreach (Binding binding in site.Bindings)
                {
                    if (binding.Protocol == "https")
                    {
                        certSName += binding.CertificateStoreName;
                    }
                }
            }
            return certSName;
        }
        #endregion

    }
}
