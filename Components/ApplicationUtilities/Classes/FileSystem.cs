using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ApplicationUtilities
{
    public static class FileSystem
    {
        #region Properties

        public static Dictionary<string, byte[]> Cache { get; set; }

        public static Dictionary<string, XmlDocument> XmlCache { get; set; }

        #endregion


        #region Constructor

        #endregion


        #region Methods

        /*public static string ReadAllText(string path, bool cache = false)
        {
            if (Cache == null)
                Cache = new Dictionary<string, byte[]>();

            if(cache)
            {
                if (!Cache.ContainsKey(path))
                {
                    Cache.Add(path, System.IO.File.ReadAllBytes(path));
                }

                return System.Text.Encoding.UTF8.GetString(Cache[path]);
            }

            return System.IO.File.ReadAllText(path);
        }

        public static byte[] ReadAllBytes(string path, bool cache = false)
        {
            if (Cache == null)
                Cache = new Dictionary<string, byte[]>();

            if (cache)
            {
                if (!Cache.ContainsKey(path))
                {
                    Cache.Add(path, System.IO.File.ReadAllBytes(path));
                }

                return Cache[path];
            }

            return System.IO.File.ReadAllBytes(path);
        }*/

        public static XmlDocument LoadXml(string source, bool cache = false)
        {
            if (XmlCache == null)
                XmlCache = new Dictionary<string, XmlDocument>();

            XmlDocument document;

            if (cache)
            {
                if (!XmlCache.ContainsKey(source))
                {
                    document = new XmlDocument();
                    document.Load(source);

                    XmlCache.Add(source, document);
                }

                return XmlCache[source];
            }

            document = new XmlDocument();
            document.Load(source);
            return document;
        }

        #endregion
    }
}
