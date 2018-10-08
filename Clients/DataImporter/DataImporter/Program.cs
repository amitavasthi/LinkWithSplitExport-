using ApplicationUtilities.Classes;
using DatabaseCore.Items;
using DataInterface.BaseClasses;
using DataInterface.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DataImporter
{
    class Program
    {
        static Guid idStudy;
        static string applicationPath;

        static void Main(string[] args)
        {
            System.Threading.Thread.Sleep(10000);

            idStudy = new Guid();
            DataUploadProvider provider = DataUploadProvider.SPSS;
            CaseDataLocation caseDataLocation = CaseDataLocation.Sql;
            string databaseProvider = "";
            string connectionString = "";
            string fileName = "";
            string respondentVariable = "";
            applicationPath = "";
            string language = "";
            int idLanguage = 2057;
            bool debug = false;
            string clientName = "";
            

            // Run through all command line arguments.
            foreach (string arg in args)
            {
                // Get the name of the argument.
                string name = arg.Split('=')[0];

                // Get the value of the argument.
                string value = HttpUtility.UrlDecode(arg.Split('=')[1]);

                switch (name)
                {
                    case "IdStudy":
                        // Parse the id of the study to import from the argument value.
                        idStudy = Guid.Parse(value);
                        break;
                    case "Provider":
                        // Parse the import provider to use from the argument value.
                        provider = (DataUploadProvider)Enum.Parse(
                            typeof(DataUploadProvider),
                            value
                        );
                        break;
                    case "DatabaseProvider":
                        databaseProvider = HttpUtility.UrlDecode(value);
                        break;
                    case "ConnectionString":
                        connectionString = HttpUtility.UrlDecode(value);
                        break;
                    case "FileName":
                        fileName = value;
                        break;
                    case "RespondentVariable":
                        respondentVariable = value;
                        break;
                    case "ApplicationPath":
                        applicationPath = HttpUtility.UrlDecode(value);
                        break;
                    case "Language":
                        language = value;
                        break;
                    case "ClientName":
                        clientName = value;
                        break;
                    case "CaseDataLocation":
                        caseDataLocation = (CaseDataLocation)Enum.Parse(
                            typeof(CaseDataLocation),
                            value
                        );
                        break;
                    case "IdLanguage":
                        idLanguage = int.Parse(value);
                        break;
                    case "Debug":
                        debug = bool.Parse(value);
                        break;
                }
            }

            string createResponsesFile = Path.Combine(
                applicationPath,
                "App_Data",
                "DataStorage",
                "CreateResponses.sql"
            );

            if (debug)
            {
                //Console.ReadLine();
            }

            DatabaseCore.Core core;
            Study study;
            try
            {
                // Create a connection to the database.
                core = new DatabaseCore.Core(
                    databaseProvider,
                    connectionString,
                    databaseProvider,
                    connectionString,
                    new string[0]
                );

                core.ClientName = clientName;
                core.CaseDataVersion = 0;
                core.CaseDataLocation = caseDataLocation;

                // Get the study from the database.
                study = core.Studies.GetSingle(idStudy);
            }
            catch(Exception ex)
            {
                LogError(applicationPath, idStudy, ex.Message);
                return;
            }

            // Check if the study exists.
            if (study == null)
            {
                LogError(applicationPath, idStudy, "The study was not found in the database.");
                return;
            }

            BaseReader reader = null;

            switch (provider)
            {
                case DataUploadProvider.SPSS:

                    // Create a new spss reader.
                    reader = new SpssReader(
                        fileName,
                        core,
                        study,
                        respondentVariable,
                        createResponsesFile,
                        idLanguage
                    );

                    break;
                case DataUploadProvider.Excel:
                case DataUploadProvider.CSV:

                    // Create a new excel reader.
                    reader = new ExcelDataReader(
                        fileName,
                        core,
                        study,
                        respondentVariable,
                        createResponsesFile,
                        idLanguage
                    );

                    break;

                case DataUploadProvider.LDF:

                    reader = new LdfReader(
                        fileName,
                        core,
                        study,
                        createResponsesFile,
                        idLanguage
                    );

                    break;
            }

            reader.ApplicationPath = applicationPath;
            reader.Language = language;

            // Validate the file.
            List<string> validationErrors = reader.Validate();

            // Check if there are validation errors.
            if (validationErrors.Count > 0)
            {
                LogError(applicationPath, idStudy, string.Join(
                    Environment.NewLine,
                    validationErrors
                ));
            }
            else
            {
                //try
                {
                    // Start the import.
                    reader.Read();

                    Log("The import was successful.", LogType.Success);

                    study.Status = StudyStatus.None;
                    study.Save();
                }
                /*catch (Exception ex)
                {
                    Log("An error occurred during the import.", LogType.Error);

                    LogError(
                        applicationPath, 
                        idStudy, 
                        ex.ToString()
                    );
                }*/
            }

            string fileNameRunningImport = Path.Combine(
                applicationPath,
                "Fileadmin",
                "RunningImports",
                idStudy.ToString()
            );

            if (File.Exists(fileNameRunningImport))
                File.Delete(fileNameRunningImport);

            File.Delete(fileName);
        }

        static void LogError(string applicationPath, Guid idStudy, string message)
        {
            string fileName = Path.Combine(
                applicationPath,
                "Fileadmin",
                "StudyUploadErrors",
                idStudy.ToString() + ".log"
            );

            FileInfo fInfo = new FileInfo(fileName);

            if (!Directory.Exists(fInfo.Directory.FullName))
                Directory.CreateDirectory(fInfo.Directory.FullName);

            File.WriteAllText(
                fileName,
                message
            );
        }

        static void Log(string message, LogType type)
        {
            switch (type)
            {
                case LogType.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.WriteLine(string.Format(
                "[{0}]: {1}",
                type.ToString(),
                message
            ));

            Console.ResetColor();
        }

        ~Program()
        {
            string fileName = Path.Combine(
                applicationPath,
                "Fileadmin",
                "RunningImports",
                idStudy.ToString()
            );

            if (File.Exists(fileName))
                File.Delete(fileName);
        }
    }

    public enum LogType
    {
        Success,
        Warning,
        Error
    }
}
