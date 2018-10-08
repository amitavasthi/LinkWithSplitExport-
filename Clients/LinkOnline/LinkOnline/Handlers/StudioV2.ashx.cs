using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Summary description for StudioV2
    /// </summary>
    public class StudioV2 : WebUtilities.BaseHandler
    {
        #region Properties

        public int LeftOffset { get; set; }
        public int TopOffset { get; set; }

        #endregion


        #region Constructor

        public StudioV2()
        {
            base.Methods.Add("RenderHeadlineTop", RenderHeadlineTop);
            base.Methods.Add("RenderHeadlineLeft", RenderHeadlineLeft);
            base.Methods.Add("LoadChunkDefinition", LoadChunkDefinition);
            base.Methods.Add("LoadChunk", LoadChunk);
            base.Methods.Add("LoadChunks", LoadChunks);
        }

        #endregion


        #region Methods

        private XmlDocument LoadReport(string source)
        {
            if (HttpContext.Current.Session["CachedReport" + source] == null)
            {
                if (!File.Exists(source))
                    throw new FileNotFoundException();

                XmlDocument document = new XmlDocument();
                document.Load(source);

                HttpContext.Current.Session["CachedReport" + source] = document;
            }

            return (XmlDocument)HttpContext.Current.Session["CachedReport" + source];
        }


        private int LoadChunkDefinition(
            StringBuilder result,
            XmlNode xmlNodeLeft,
            XmlNode xmlNodeTop
        )
        {
            XmlNode xmlNodeNestedLeft = xmlNodeLeft.SelectSingleNode("Variable/Variable");
            XmlNode xmlNodeNestedTop = xmlNodeTop.SelectSingleNode("Variable/Variable");

            XmlNodeList xmlNodesLeft = xmlNodeLeft.SelectNodes("Variable/*");
            XmlNodeList xmlNodesTop = xmlNodeTop.SelectNodes("Variable/*");

            if (xmlNodeNestedLeft != null || xmlNodeNestedTop != null)
            {
                if (xmlNodeNestedLeft == null)
                {
                    for (int i = 0; i < (xmlNodesLeft.Count - 1); i++)
                    {
                        this.TopOffset = 0;
                        LoadChunkDefinition(
                            result,
                            xmlNodeLeft,
                            xmlNodeNestedTop.ParentNode
                        );
                    }
                }
                else if (xmlNodeNestedTop == null)
                {
                    for (int i = 0; i < (xmlNodesTop.Count - 1); i++)
                    {
                        LoadChunkDefinition(
                            result,
                            xmlNodeNestedLeft.ParentNode,
                            xmlNodeTop
                        );
                    }
                }
                else
                {
                    int offsetLeft = 0;
                    int _offsetLeft = 0;
                    for (int i = 0; i < (xmlNodesLeft.Count - 1); i++)
                    {
                        this.TopOffset = 0;
                        for (int t = 0; t < (xmlNodesTop.Count - 1); t++)
                        {
                            this.LeftOffset = _offsetLeft;
                            offsetLeft = LoadChunkDefinition(
                                result,
                                xmlNodeNestedLeft.ParentNode,
                                xmlNodeNestedTop.ParentNode
                            );
                        }

                        _offsetLeft += offsetLeft;
                    }
                }

                return 0;
            }

            for (int l = 0; l < xmlNodesLeft.Count; l += 10)
            {
                /*bool leftEnabled = true;

                if (xmlNodesLeft[l].Attributes["Enabled"] != null)
                {
                    bool.TryParse(xmlNodesLeft[l].Attributes["Enabled"].Value, out leftEnabled);
                }

                if (!leftEnabled)
                    continue;*/
                int topCount;
                for (int t = 0; t < xmlNodesTop.Count; t += 10)
                {
                    /*bool topEnabled = true;

                    if (xmlNodesTop[t].Attributes["Enabled"] != null)
                    {
                        bool.TryParse(xmlNodesTop[t].Attributes["Enabled"].Value, out topEnabled);
                    }

                    if (!topEnabled)
                        continue;*/

                    topCount = 10;

                    if ((t + 9) == xmlNodesTop.Count)
                        topCount = (xmlNodesTop.Count - 1) - t;

                    XmlNode parent = xmlNodesLeft[l].ParentNode;
                    string pathLeft = "Results";
                    string pathTop = "";

                    while (parent.Name == "Variable")
                    {
                        pathLeft += "/Variable/TaxonomyCategory";

                        parent = parent.ParentNode;
                    }
                    parent = xmlNodesTop[t].ParentNode;
                    while (parent.Name == "Variable")
                    {
                        pathTop += "/Variable/TaxonomyCategory";

                        parent = parent.ParentNode;
                    }

                    result.Append("{");
                    result.Append(string.Format(
                        "\"PathLeft\": \"{2}\", \"PathTop\": \"{3}\", \"LeftStart\": {0}, " +
                        "\"TopStart\": {1}, \"TopCount\": {4}",
                        (l + this.LeftOffset),
                        (t + this.TopOffset),
                        pathLeft,
                        pathTop.Remove(0, 1),
                        topCount
                    ));
                    result.Append("},");
                }
            }

            this.LeftOffset += xmlNodesLeft.Count - 1;
            //this.TopOffset += xmlNodesTop.Count - 1;
            this.TopOffset += xmlNodesTop.Count - 1;

            return xmlNodesLeft.Count;
        }

        #endregion


        #region Web Methods

        private void RenderHeadlineTop(HttpContext context)
        {
            // Get the full path to the report definition
            // file from the http request's parameters.
            string source = context.Request.Params["Source"];

            XmlDocument document = LoadReport(source);

            XmlNodeList xmlNodes = document.DocumentElement.SelectNodes(
                "Variables[@Position=\"Top\"]/Variable"
            );

            StringBuilder sbVariableLabels = new StringBuilder();
            StringBuilder sbCategoryLabels = new StringBuilder();
            StringBuilder sbSigDiffLabels = new StringBuilder();

            string label;
            string sigDiffLetter;
            foreach (XmlNode xmlNode in xmlNodes)
            {
                XmlNodeList xmlNodesCategories = xmlNode.ChildNodes;

                sbCategoryLabels.Append(string.Format(
                    "<td rowspan=\"2\">{0}</td>",
                    "Base"
                ));

                int count = 0;

                foreach (XmlNode xmlNodeCategory in xmlNodesCategories)
                {
                    /*bool enabled = true;

                    if(xmlNodeCategory.Attributes["Enabled"] != null)
                    {
                        bool.TryParse(xmlNodeCategory.Attributes["Enabled"].Value, out enabled);
                    }

                    if (!enabled)
                        continue;*/

                    label = "unknown";
                    sigDiffLetter = "";

                    if (xmlNodeCategory.Attributes["Label2057"] != null)
                        label = xmlNodeCategory.Attributes["Label2057"].Value;
                    else if (xmlNodeCategory.Attributes["Name"] != null)
                        label = xmlNodeCategory.Attributes["Name"].Value;

                    if (xmlNodeCategory.Attributes["SignificantLetter"] != null)
                        sigDiffLetter = xmlNodeCategory.Attributes["SignificantLetter"].Value;

                    sbCategoryLabels.Append(string.Format(
                        "<td>{0}</td>",
                        label
                    ));

                    sbSigDiffLabels.Append(string.Format(
                        "<td>{0}</td>",
                        sigDiffLetter
                    ));

                    count++;
                }

                sbVariableLabels.Append(string.Format(
                    "<td colspan=\"{0}\">{1}</td>",
                    count + 1,
                    xmlNode.Attributes["Label2057"].Value
                ));
            }

            context.Response.Write(string.Format(
                "<table class=\"Table\" cellpadding=\"0\" cellspacing=\"0\">" +
                "<tr>{0}</tr><tr class=\"HeadlineCategories\">{1}</tr>" +
                "<tr class=\"TableRowSignificanceLetters\">{2}</tr></table>",
                sbVariableLabels.ToString(),
                sbCategoryLabels.ToString(),
                sbSigDiffLabels.ToString()
            ));

            sbVariableLabels.Clear();
            sbCategoryLabels.Clear();
        }

        private void RenderHeadlineLeft(HttpContext context)
        {
            // Get the full path to the report definition
            // file from the http request's parameters.
            string source = context.Request.Params["Source"];

            XmlDocument document = LoadReport(source);

            XmlNodeList xmlNodes = document.DocumentElement.SelectNodes(
                "Variables[@Position=\"Left\"]/Variable"
            );

            context.Response.Write("<table class=\"Table\" cellpadding=\"0\" cellspacing=\"0\">");

            foreach (XmlNode xmlNode in xmlNodes)
            {
                XmlNodeList xmlNodesCategories = xmlNode.ChildNodes;

                context.Response.Write(string.Format(
                    "<tr><td rowspan=\"{0}\">{1}</td><td class=\"HeadlineCategories\">Base</td></tr>",
                    xmlNodesCategories.Count + 1,
                    xmlNode.Attributes["Label2057"].Value
                ));

                foreach (XmlNode xmlNodeCategory in xmlNodesCategories)
                {
                    context.Response.Write(string.Format(
                        "<tr><td class=\"HeadlineCategories HeadlineCategory\"><div>{0}</div></td></tr>",
                        xmlNodeCategory.Attributes["Label2057"].Value
                    ));
                }
            }

            context.Response.Write("</table>");
        }

        private void LoadChunkDefinition(HttpContext context)
        {
            // Get the full path to the report definition
            // file from the http request's parameters.
            string source = context.Request.Params["Source"];

            XmlDocument document = LoadReport(source);

            XmlNodeList xmlNodesTop = document.DocumentElement.SelectNodes(
                "Variables[@Position=\"Top\"]"
            );
            XmlNodeList xmlNodesLeft = document.DocumentElement.SelectNodes(
                "Variables[@Position=\"Left\"]"
            );

            StringBuilder result = new StringBuilder();
            result.Append("[");

            this.LeftOffset = 0;
            this.TopOffset = 0;

            foreach (XmlNode xmlNodeLeft in xmlNodesLeft)
            {
                foreach (XmlNode xmlNodeTop in xmlNodesTop)
                {
                    LoadChunkDefinition(result, xmlNodeLeft, xmlNodeTop);
                }
            }

            if (result.Length > 1)
                result = result.Remove(result.Length - 1, 1);

            result.Append("]");

            context.Response.Write(result.ToString());
            result.Clear();
        }

        private void LoadChunk(HttpContext context)
        {
            // Get the full path to the report definition
            // file from the http request's parameters.
            string source = context.Request.Params["Source"];

            // Get the xPath to the chunk
            // from the http request's parameters.
            string pathLeft = context.Request.Params["PathLeft"];
            // Get the xPath to the chunk
            // from the http request's parameters.
            string pathTop = context.Request.Params["PathTop"];

            int leftStart = int.Parse(context.Request.Params["LeftStart"]);
            int topStart = int.Parse(context.Request.Params["TopStart"]);

            XmlDocument document = LoadReport(source);

            XmlNodeList xmlNodesLeftValues = document.DocumentElement.SelectNodes(pathLeft);

            StringBuilder result = new StringBuilder();
            result.Append("[");

            for (int l = leftStart; l < (leftStart + 10); l++)
            {
                if (xmlNodesLeftValues.Count <= l)
                    break;

                XmlNodeList xmlNodesTopValues = xmlNodesLeftValues[l].SelectNodes(pathTop);

                if (xmlNodesTopValues.Count == 0)
                    break;

                if (xmlNodesLeftValues[l].ParentNode.ChildNodes[0] == xmlNodesLeftValues[l])
                {
                    result.Append("{ \"Index\": \"Base" + l + "\", \"Html\": \"");

                    for (int t = topStart; t < (topStart + 10); t++)
                    {
                        if (xmlNodesTopValues.Count <= t)
                            break;

                        if (xmlNodesTopValues[t].ParentNode.ChildNodes[0] == xmlNodesTopValues[t])
                        {
                            int value = 0;

                            if (xmlNodesTopValues[t].ParentNode.Attributes["VariableBase"] != null)
                                value = (int)Math.Round(double.Parse(xmlNodesTopValues[t].ParentNode.Attributes["VariableBase"].Value), 0);

                            result.Append(string.Format(
                                "<td class='TdBase'>{0}</td>",
                                value
                            ));
                        }

                        result.Append(string.Format(
                            "<td class='TdBase'>{0}</td>",
                            (int)double.Parse(xmlNodesTopValues[t].Attributes["Base"].Value)
                        ));
                    }

                    result.Append("\"},");
                }

                result.Append("{ \"Index\": \"" + l + "\", \"Html\": \"");

                StringBuilder resultPerc = new StringBuilder();
                StringBuilder resultSigDif = new StringBuilder();

                for (int t = topStart; t < (topStart + 10); t++)
                {
                    if (xmlNodesTopValues.Count <= t)
                        break;

                    if (xmlNodesTopValues[t].ParentNode.ChildNodes[0] == xmlNodesTopValues[t])
                    {
                        result.Append(string.Format(
                            "<td class='TdBase' TopChunk='{1}'>{0}</td>",
                            (int)double.Parse(xmlNodesTopValues[t].ParentNode.Attributes["Base"].Value),
                            topStart
                        ));
                    }

                    double value = double.Parse(xmlNodesTopValues[t].Attributes["Value"].Value);
                    double percentage = value * 100 / double.Parse(xmlNodesTopValues[t].Attributes["Base"].Value);
                    string sigDiff = "";

                    if (xmlNodesTopValues[t].Attributes["SigDiff"] != null)
                        sigDiff = xmlNodesTopValues[t].Attributes["SigDiff"].Value;

                    result.Append(string.Format(
                        "<td>{0}</td>",
                        value != 0 ? value.ToFormattedString() : "-"
                    ));
                    resultPerc.Append(string.Format(
                        "<td>{0}</td>",
                        percentage != 0 ? percentage.ToString() : "-"
                    ));
                    resultSigDif.Append(string.Format(
                        "<td>{0}</td>",
                        sigDiff
                    ));
                }

                result.Append("\", \"HtmlPerc\": \"");
                result.Append(resultPerc.ToString());
                result.Append("\", \"HtmlSigDiff\": \"");
                result.Append(resultSigDif.ToString());

                result.Append("\"},");
            }

            result = result.Remove(result.Length - 1, 1);
            result.Append("]");

            context.Response.Write(result.ToString());
            result.Clear();
            /*
            foreach (XmlNode xmlNodeLeftValue in xmlNodesLeftValues)
            {
                context.Response.Write(string.Format(
                    "<td>{0}</td>",
                    "100" // <-- Base
                ));

                XmlNodeList xmlNodesTopValues = xmlNodeLeftValue.SelectNodes(pathTop);

                foreach (XmlNode xmlNodeTopValue in xmlNodesTopValues)
                {
                    context.Response.Write(string.Format(
                        "<td>{0}</td>",
                        (int)double.Parse(xmlNodeTopValue.Attributes["Value"].Value)
                    ));
                }

                context.Response.Write("");
            }*/
        }
        private void LoadChunks(HttpContext context)
        {
            // Get the full path to the report definition
            // file from the http request's parameters.
            string source = context.Request.Params["Source"];

            List<Chunk> chunks = new JavaScriptSerializer().
                Deserialize<List<Chunk>>(context.Request.Params["Chunks"]);

            StringBuilder result = new StringBuilder();
            result.Append("[");

            XmlDocument document = LoadReport(source);

            Crosstables.Classes.CrosstableSettings settings = new Crosstables.Classes.CrosstableSettings(
                null,
                document.DocumentElement.SelectSingleNode("Settings")
            );

            Dictionary<string, StringBuilder> rows = new Dictionary<string, StringBuilder>();

            StringBuilder resultPerc;
            StringBuilder resultSigDiff;

            foreach (Chunk chunk in chunks)
            {
                // Get the xPath to the chunk
                // from the http request's parameters.
                string pathLeft = chunk.PathLeft;
                // Get the xPath to the chunk
                // from the http request's parameters.
                string pathTop = chunk.PathTop;

                int leftStart = chunk.LeftStart;
                int topStart = chunk.TopStart;

                XmlNodeList xmlNodesTopValues;
                double value;
                double percentage;
                string sigDiff;

                XmlNodeList xmlNodesLeftValues = document.DocumentElement.SelectNodes(pathLeft);

                for (int l = leftStart; l < (leftStart + 10); l++)
                {
                    if (xmlNodesLeftValues.Count <= l)
                        break;

                    xmlNodesTopValues = xmlNodesLeftValues[l].SelectNodes(pathTop);

                    if (xmlNodesTopValues.Count == 0)
                        break;

                    if (xmlNodesLeftValues[l].ParentNode.ChildNodes[0] == xmlNodesLeftValues[l])
                    {
                        if (!rows.ContainsKey("Base" + l))
                        {
                            rows.Add("Base" + l, new StringBuilder());

                            rows["Base" + l].Append("{ \"Index\": \"Base" + l + "\", \"Html\": \"");
                        }

                        for (int t = topStart; t < (topStart + chunk.TopCount); t++)
                        {
                            if (xmlNodesTopValues.Count <= t)
                                break;

                            if (xmlNodesTopValues[t].ParentNode.ChildNodes[0] == xmlNodesTopValues[t])
                            {
                                value = 0;

                                if (xmlNodesTopValues[t].ParentNode.Attributes["VariableBase"] != null)
                                    value = double.Parse(xmlNodesTopValues[t].ParentNode.Attributes["VariableBase"].Value);

                                rows["Base" + l].Append(string.Format(
                                    "<div class='TdBase'><div>{0}</div></div>",
                                    value != 0 ? value.ToFormattedString(settings.DecimalPlaces) : "-"
                                ));
                            }

                            value = 0;

                            if (xmlNodesTopValues[t].Attributes["Base"] != null)
                                value = double.Parse(xmlNodesTopValues[t].Attributes["Base"].Value);

                            rows["Base" + l].Append(string.Format(
                                "<div class='TdBase'><div>{0}</div></div>",
                                value != 0 ? value.ToFormattedString(settings.DecimalPlaces) : "-"
                            ));
                        }

                        //rows["Base" + l].Append("\"},");
                    }

                    if (!rows.ContainsKey(l.ToString()))
                    {
                        rows.Add(l.ToString(), new StringBuilder());
                        //rows.Add(l+ "Perc", new StringBuilder());
                        //rows.Add(l + "SigDiff", new StringBuilder());

                        rows[l.ToString()].Append("{ \"Index\": \"" + l + "\", \"Class\": \"\", \"Html\": \"");
                        //rows[l + "Perc"].Append("{ \"Index\": \"" + l + "Perc" + "\", \"Class\": \"TableRowPercentage\", \"Html\": \"");
                        //rows[l + "SigDiff"].Append("{ \"Index\": \"" + l + "SigDiff" + "\", \"Class\": \"TableRowSigDiff\", \"Html\": \"");
                    }

                    for (int t = topStart; t < (topStart + chunk.TopCount); t++)
                    {
                        if (xmlNodesTopValues.Count <= t)
                            break;

                        value = 0;

                        if (xmlNodesTopValues[t].ParentNode.ChildNodes[0] == xmlNodesTopValues[t])
                        {
                            if (xmlNodesTopValues[t].ParentNode.Attributes["Base"] != null)
                                value = double.Parse(xmlNodesTopValues[t].ParentNode.Attributes["Base"].Value);

                            rows[l.ToString()].Append(string.Format(
                                "<div class='TdBase TdBaseLeft V' TopChunk='{1}'><div>{0}</div></div>",
                                value != 0 ? value.ToFormattedString(settings.DecimalPlaces) : "-",
                                topStart
                            ));
                        }

                        value = 0;
                        percentage = 0;

                        if (xmlNodesTopValues[t].Attributes["Value"] != null)
                            value = double.Parse(xmlNodesTopValues[t].Attributes["Value"].Value);

                        if (xmlNodesTopValues[t].Attributes["Base"] != null)
                            percentage = value * 100 / double.Parse(xmlNodesTopValues[t].Attributes["Base"].Value);

                        sigDiff = "";

                        if (xmlNodesTopValues[t].Attributes["SigDiff"] != null)
                            sigDiff = xmlNodesTopValues[t].Attributes["SigDiff"].Value;

                        if (double.IsNaN(percentage))
                            percentage = 0;

                        rows[l.ToString()].Append("<div class='V'>");
                        rows[l.ToString()].Append(string.Format(
                            "<div class='N'>{0}</div>",
                            value != 0 ? value.ToFormattedString(settings.DecimalPlaces) : "-"
                        ));

                        rows[l.ToString()].Append(string.Format(
                            "<div class='P'>{0}</div>",
                            percentage != 0 ? (percentage.ToFormattedString(settings.DecimalPlaces) + " %") : " - "
                        ));
                        rows[l.ToString()].Append(string.Format(
                            "<div class='S'>{0}</div>",
                            sigDiff
                        ));
                        rows[l.ToString()].Append("</div>");
                        /*rows[l + "Perc"].Append(string.Format(
                            "<div>{0}</div>",
                            percentage != 0 ? percentage.ToString() : "-"
                        ));
                        rows[l + "SigDiff"].Append(string.Format(
                            "<div>{0}</div>",
                            sigDiff
                        ));*/
                    }

                    //rows[l.ToString()].Append("\"},");
                }
            }

            foreach (StringBuilder row in rows.Values)
            {
                result.Append(row.ToString());
                result.Append("\"},");
                row.Clear();
            }

            rows.Clear();

            result = result.Remove(result.Length - 1, 1);
            result.Append("]");

            context.Response.Write(result.ToString());
            result.Clear();
            /*
            foreach (XmlNode xmlNodeLeftValue in xmlNodesLeftValues)
            {
                context.Response.Write(string.Format(
                    "<td>{0}</td>",
                    "100" // <-- Base
                ));

                XmlNodeList xmlNodesTopValues = xmlNodeLeftValue.SelectNodes(pathTop);

                foreach (XmlNode xmlNodeTopValue in xmlNodesTopValues)
                {
                    context.Response.Write(string.Format(
                        "<td>{0}</td>",
                        (int)double.Parse(xmlNodeTopValue.Attributes["Value"].Value)
                    ));
                }

                context.Response.Write("");
            }*/
        }

        #endregion
    }

    public class Chunk
    {
        public string PathLeft { get; set; }
        public string PathTop { get; set; }
        public int LeftStart { get; set; }
        public int TopStart { get; set; }
        public int TopCount { get; set; }
    }
}