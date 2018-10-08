using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace StorageMethodSpeedTest
{
    class Program
    {
        static Dictionary<int, Guid> sqlRespondents;

        static void Main(string[] args)
        {
            Guid idCategory = Guid.Parse("0b57c466-c75e-4a71-a0e5-181680dba0a5");
            Guid idCategory2 = Guid.Parse("0a2ff3d9-8057-459b-ba80-4884e4b38c03");

            Guid _idCategory = Guid.Parse("DB76FAFA-6D93-4CE2-92EA-AB55D81330D0");
            Guid _idCategory2 = Guid.Parse("EBB057F9-439B-4713-A2F8-B0E5836C1BEE");

            Guid _idVariable = Guid.Parse("DC73B10B-50F1-40F4-8F08-811CF5346664");
            Guid _idVariable2 = Guid.Parse("4953F030-2649-4F9F-83C6-C286AB35145C");

            //ConvertToPlainText(idCategory4);

            Stopwatch stopwatchSql = new Stopwatch();
            Stopwatch stopwatchXml = new Stopwatch();
            Stopwatch stopwatchPlainText = new Stopwatch();

            List<Guid> filter = GetRespondentsXml(idCategory2);
            filter = GetRespondentsXml(idCategory3);

            List<int> _filter = GetRespondentsSql(_idCategory2, idVariable2);
            _filter = GetRespondentsSql(_idCategory3, idVariable3);
            //filter = GetRespondentsXml(idCategory4);

            stopwatchSql.Start();

            List<int> sqlResult = GetRespondentsSql(_idCategory, idVariable, _filter);

            stopwatchSql.Stop();

            stopwatchXml.Start();

            List<Guid> xmlResult = GetRespondentsXml(idCategory, filter);

            stopwatchXml.Stop();

            stopwatchPlainText.Start();

            List<Guid> plainResult = GetRespondentsPlainText(idCategory, filter);

            stopwatchPlainText.Stop();

            Console.WriteLine("SQL: " + stopwatchSql.Elapsed + "(" + sqlResult.Count + ")");
            Console.WriteLine("XML: " + stopwatchXml.Elapsed + "(" + xmlResult.Count + ")");
            Console.WriteLine("PLAIN: " + stopwatchPlainText.Elapsed + "(" + plainResult.Count + ")");

            Console.ReadLine();
        }

        static void ConvertToPlainText(Guid idCategory)
        {
            List<Guid> respondents = GetRespondentsXml(idCategory);

            string fileName = Path.Combine(
                @"C:\Projects\Cognicient\LinkManager SVN\Clients\LinkOnline\LinkOnline\Fileadmin\Responses\microsoft_PLAIN_TEXT",
                idCategory + ".txt"
            );

            foreach (Guid idRespondent in respondents)
            {
                File.AppendAllText(
                    fileName,
                    idRespondent.ToString()
                );
            }
        }

        static List<int> GetRespondentsSql(int idCategory, int idVariable, List<int> filter = null)
        {
            SqlConnection sqlConnection = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=Microsoft1;Integrated Security=True");
            SqlCommand sqlCommand = new SqlCommand(
                "SELECT IdRespondent FROM resp.Var_" + idVariable + " WHERE IdCategory=" + idCategory,
                sqlConnection
            );

            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();

            List<int> result = new List<int>();

            while(reader.Read())
            {
                int idRespondent = (int)reader.GetValue(0);

                if (filter != null && filter.Contains(idRespondent) == false)
                    continue;

                result.Add(idRespondent);
            }

            sqlConnection.Close();

            return result;
        }

        static List<Guid> GetRespondentsXml(Guid idCategory, List<Guid> filter = null)
        {
            string fileName = Path.Combine(
                @"C:\Projects\Cognicient\LinkManager SVN\Clients\LinkOnline\LinkOnline\Fileadmin\Responses\microsoft",
                "[resp].[Var_" + idCategory + "].xml"
            );

            StringBuilder xmlString = new StringBuilder();
            xmlString.Append("<Results>");
            xmlString.Append(File.ReadAllText(fileName));
            xmlString.Append("</Results>");

            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.LoadXml(xmlString.ToString());

            xmlString.Clear();

            List<Guid> result = new List<Guid>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Guid idRespondent = Guid.Parse(
                    xmlNode.Attributes["IdRespondent"].Value
                );

                if (filter != null && filter.Contains(idRespondent) == false)
                    continue;

                result.Add(idRespondent);
            }

            return result;
        }

        static List<Guid> GetRespondentsPlainText(Guid idCategory, List<Guid> filter = null)
        {
            string fileName = Path.Combine(
                @"C:\Projects\Cognicient\LinkManager SVN\Clients\LinkOnline\LinkOnline\Fileadmin\Responses\microsoft_PLAIN_TEXT",
                idCategory + ".txt"
            );

            StreamReader reader = new StreamReader(fileName);

            List<Guid> result = new List<Guid>();

            while(reader.EndOfStream == false)
            {
                char[] respondentIdBuffer = new char[36];

                reader.ReadBlock(respondentIdBuffer, 0, 36);

                Guid idRespondent = Guid.Parse(
                    new String(respondentIdBuffer)
                );

                if (filter != null && filter.Contains(idRespondent) == false)
                    continue;

                result.Add(idRespondent);
            }

            return result;
        }
    }
}
