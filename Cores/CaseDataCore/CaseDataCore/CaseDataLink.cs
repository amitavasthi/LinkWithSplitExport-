using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseDataCore1
{
    public class CaseDataLink
    {
        #region Properties

        //public static Dictionary<string, List<object[]>> Cache { get; set; }

        #endregion


        #region Constructor

        public CaseDataLink()
        {
            /*if (Cache == null)
                Cache = new Dictionary<string, List<object[]>>();*/
        }

        #endregion


        #region Methods

        private List<DataCommand> ParseCommandText(string commandText)
        {
            string[] commandTexts = commandText.Split(
                new string[] { "UNION ALL" },
                StringSplitOptions.RemoveEmptyEntries
            );

            List<DataCommand> result = new List<DataCommand>();

            foreach (string command in commandTexts)
            {
                DataCommand c = new DataCommand(command);

                if (c.Valid)
                    result.Add(c);
            }

            return result;
        }

        public Dictionary<Guid, List<object>> Select(
            string commandText,
            string databaseName
        )
        {
            //commandText = "SELECT [IdRespondent], [IdCategory] FROM [resp].[Var_f65ecbe9-3c8a-4eba-bf40-8ab97afc99f3] WHERE [IdCategory] IN ('5750de81-f5ea-4f75-accf-a2c8029b31f4','edcf2ee9-ced3-4153-bbf9-b7e0e2d6642b','7969b8e2-42de-4eac-bc39-a41b663cddc0','9dd65b8b-64d6-4bfa-aa07-517c9980bb51','86142433-c75b-43fe-a001-5a23c7a4af97','313456a4-456d-4c9d-bba9-385c119256e9','1cd2ac8f-cab3-4cdd-9d03-47ad53c9f9c8','ab351e17-d75b-4d47-a506-c2476e64f05c','298a3bfb-53fc-4ea4-b57e-63be72cbe59a')"

            List<DataCommand> commands = ParseCommandText(commandText);

            Dictionary<Guid, List<object>> result = new Dictionary<Guid, List<object>>(); ;

            foreach (DataCommand command in commands)
            {
                GetValues(Path.Combine(
                    ConfigurationManager.AppSettings["CaseDataPath"],
                    databaseName,
                    command.IdVariable + ".kdf"
                ), command.DataType, result, command.CategoryFilter);
            }

            return result;
        }

        public List<object[]> Select2(
            string commandText,
            string databaseName
        )
        {
            //commandText = "SELECT [IdRespondent], [IdCategory] FROM [resp].[Var_f65ecbe9-3c8a-4eba-bf40-8ab97afc99f3] WHERE [IdCategory] IN ('5750de81-f5ea-4f75-accf-a2c8029b31f4','edcf2ee9-ced3-4153-bbf9-b7e0e2d6642b','7969b8e2-42de-4eac-bc39-a41b663cddc0','9dd65b8b-64d6-4bfa-aa07-517c9980bb51','86142433-c75b-43fe-a001-5a23c7a4af97','313456a4-456d-4c9d-bba9-385c119256e9','1cd2ac8f-cab3-4cdd-9d03-47ad53c9f9c8','ab351e17-d75b-4d47-a506-c2476e64f05c','298a3bfb-53fc-4ea4-b57e-63be72cbe59a')"

            /*if (Cache.ContainsKey(commandText))
                return Cache[commandText];*/

            List<DataCommand> commands = ParseCommandText(commandText);

            List<object[]> result = new List<object[]>();

            foreach (DataCommand command in commands)
            {
                result.AddRange(GetValues2(command.DataType, Path.Combine(
                    ConfigurationManager.AppSettings["CaseDataPath"],
                    databaseName,
                    command.IdVariable + ".kdf"
                ), command.CategoryFilter));
            }

            //Cache.Add(commandText, result);

            return result;
        }

        public int Count(Guid idVariable, CaseDataType dataType, string databaseName)
        {
            string fileName = Path.Combine(
                ConfigurationManager.AppSettings["CaseDataPath"],
                databaseName,
                idVariable + ".kdf"
            );

            if (!File.Exists(fileName))
                return 0;

            byte[] buffer = File.ReadAllBytes(fileName);

            if (dataType == CaseDataType.Categorical)
                return buffer.Length / 32;
            else if (dataType == CaseDataType.Numeric)
                return buffer.Length / 24;

            int fieldLength = 0;
            if (dataType == CaseDataType.Text)
            {
                fieldLength = BitConverter.ToInt32(new byte[]
                {
                    buffer[0],
                    buffer[1],
                    buffer[2],
                    buffer[3]
                }, 0);

                return buffer.Length / (16 + fieldLength);
            }

            return 0;
        }

        public Dictionary<Guid, List<object>> GetValues(
            string fileName,
            CaseDataType dataType,
            Dictionary<Guid, List<object>> result,
            Dictionary<Guid, object> categoryFilter = null
        )
        {
            if (!File.Exists(fileName))
                return new Dictionary<Guid, List<object>>();

            byte[] buffer = File.ReadAllBytes(fileName);

            int fieldLength = 0;
            int i = 0;
            if (dataType == CaseDataType.Text)
            {
                fieldLength = BitConverter.ToInt32(new byte[]
                {
                    buffer[i+0],
                    buffer[i+1],
                    buffer[i+2],
                    buffer[i+3]
                }, 0);

                i += 4;
            }

            while (true)
            {
                if (i >= buffer.Length)
                    break;

                Guid idRespondent = new Guid(new byte[]
                {
                    buffer[i+0],
                    buffer[i+1],
                    buffer[i+2],
                    buffer[i+3],
                    buffer[i+4],
                    buffer[i+5],
                    buffer[i+6],
                    buffer[i+7],
                    buffer[i+8],
                    buffer[i+9],
                    buffer[i+10],
                    buffer[i+11],
                    buffer[i+12],
                    buffer[i+13],
                    buffer[i+14],
                    buffer[i+15]
                });
                i += 16;

                object value;

                if (dataType == CaseDataType.Numeric)
                {

                    value = BitConverter.ToDouble(new byte[]
                    {
                        buffer[i+0],
                        buffer[i+1],
                        buffer[i+2],
                        buffer[i+3],
                        buffer[i+4],
                        buffer[i+5],
                        buffer[i+6],
                        buffer[i+7]
                    }, 0);

                    i += 8;
                }
                else if (dataType == CaseDataType.Text)
                {
                    byte[] stringBuffer = new byte[fieldLength];

                    for (int c = 0; c < fieldLength; c++)
                    {
                        stringBuffer[c] = buffer[i + c];
                    }

                    value = System.Text.Encoding.UTF8.GetString(stringBuffer).Replace("\0", "").Trim();
                    i += fieldLength;
                }
                else
                {
                    Guid idCategory = new Guid(new byte[]
                    {
                        buffer[i+0],
                        buffer[i+1],
                        buffer[i+2],
                        buffer[i+3],
                        buffer[i+4],
                        buffer[i+5],
                        buffer[i+6],
                        buffer[i+7],
                        buffer[i+8],
                        buffer[i+9],
                        buffer[i+10],
                        buffer[i+11],
                        buffer[i+12],
                        buffer[i+13],
                        buffer[i+14],
                        buffer[i+15]
                    });
                    i += 16;


                    if (categoryFilter != null && categoryFilter.ContainsKey(idCategory) == false)
                        continue;

                    value = idCategory;
                }

                if (!result.ContainsKey(idRespondent))
                    result.Add(idRespondent, new List<object>());

                result[idRespondent].Add(value);
            }

            return result;
        }

        public List<object[]> GetValues2(
            CaseDataType dataType,
            string fileName,
            Dictionary<Guid, object> categoryFilter = null
        )
        {
            if (!File.Exists(fileName))
                return new List<object[]>();

            byte[] buffer = File.ReadAllBytes(fileName);

            int i = 0;
            int fieldLength = 0;
            if (dataType == CaseDataType.Text)
            {
                fieldLength = BitConverter.ToInt32(new byte[]
                {
                    buffer[i+0],
                    buffer[i+1],
                    buffer[i+2],
                    buffer[i+3]
                }, 0);

                i += 4;
            }

            List<object[]> result = new List<object[]>();

            while (true)
            {
                if (i == buffer.Length)
                    break;
                /*
                long idRespondent = BitConverter.ToInt64(new byte[]
                    {
                        buffer[i+0],
                        buffer[i+1],
                        buffer[i+2],
                        buffer[i+3],
                        buffer[i+4],
                        buffer[i+5],
                        buffer[i+6],
                        buffer[i+7]
                    }, 0);

                i += 8;
                */
                Guid idRespondent = new Guid(
                    BitConverter.ToUInt32(buffer, i + 0),
                    BitConverter.ToUInt16(buffer, i + 4),
                    BitConverter.ToUInt16(buffer, i + 6),
                    buffer[i + 8],
                    buffer[i + 9],
                    buffer[i + 10],
                    buffer[i + 11],
                    buffer[i + 12],
                    buffer[i + 13],
                    buffer[i + 14],
                    buffer[i + 15]
                );
                i += 16;

                object value;

                if (dataType == CaseDataType.Numeric)
                {

                    value = BitConverter.ToDouble(new byte[]
                    {
                        buffer[i+0],
                        buffer[i+1],
                        buffer[i+2],
                        buffer[i+3],
                        buffer[i+4],
                        buffer[i+5],
                        buffer[i+6],
                        buffer[i+7]
                    }, 0);

                    i += 8;
                }
                else if (dataType == CaseDataType.Text)
                {
                    byte[] stringBuffer = new byte[fieldLength];

                    for (int c = 0; c < fieldLength; c++)
                    {
                        stringBuffer[c] = buffer[i + c];
                    }

                    value = System.Text.Encoding.UTF8.GetString(stringBuffer).Replace("\0", "").Trim();
                    i += fieldLength;
                }
                else
                {
                    Guid idCategory = new Guid(
                        BitConverter.ToUInt32(buffer, i + 0),
                        BitConverter.ToUInt16(buffer, i + 4),
                        BitConverter.ToUInt16(buffer, i + 6),
                        buffer[i + 8],
                        buffer[i + 9],
                        buffer[i + 10],
                        buffer[i + 11],
                        buffer[i + 12],
                        buffer[i + 13],
                        buffer[i + 14],
                        buffer[i + 15]
                    );
                    i += 16;


                    if (categoryFilter != null && categoryFilter.ContainsKey(idCategory) == false)
                        continue;

                    value = idCategory;
                }

                result.Add(new object[] { idRespondent, value });
            }

            return result;
        }



        public static byte[] Compress(byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);

                return compressedStream.ToArray();
            }
        }

        public byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }

        #endregion
    }
}
