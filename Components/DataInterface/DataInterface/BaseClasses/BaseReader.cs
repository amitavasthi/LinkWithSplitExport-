using DatabaseCore;
using DatabaseCore.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInterface.BaseClasses
{
    public abstract class BaseReader
    {
        #region Properties

        /// <summary>
        /// Gets or sets the core of the reader.
        /// </summary>
        public Core Core { get; set; }

        /// <summary>
        /// Gets or sets the name of the file to read.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the variable name
        /// of the respondent variable.
        /// </summary>
        public string RespondentVariable { get; set; }

        /// <summary>
        /// Gets or sets the file name of
        /// the create responses script file.
        /// </summary>
        public string CreateResponsesTemplateFile { get; set; }

        /// <summary>
        /// Gets or sets the study of the reader.
        /// </summary>
        public Study Study { get; set; }

        /// <summary>
        /// Gets or sets the id of the metadata's language.
        /// </summary>
        public int IdLanguage { get; set; }

        /// <summary>
        /// Gets the current progress.
        /// </summary>
        public double Progress
        {
            set
            {
                try
                {
                    File.WriteAllText(Path.Combine(
                        this.ApplicationPath,
                        "Fileadmin",
                        "RunningImports",
                        this.Study.Id.ToString()
                    ),
                    string.Format(
                        "{0}|{1}|{2}",
                        this.Status,
                        value.ToString(),
                        this.ResponseInsertStarted != null ? this.ResponseInsertStarted.ToString() : ""
                    ));
                }
                catch { }
            }
        }

        public DateTime ResponseInsertStarted { get; set; }

        public DataImportStatus Status { get; set; }

        /// <summary>
        /// Gets or sets if a mail should be sended to the
        /// study's creator when the import has finished.
        /// </summary>
        public bool SendMail { get; set; }

        public string ApplicationPath { get; set; }

        public string Language { get; set; }

        /// <summary>
        /// Gets the calculated eta based on the current progress.
        /// </summary>
        /*public DateTime? Eta
        {
            get
            {
                TimeSpan elapsed = DateTime.Now - this.StartTime;

                long etaSeconds = ((int)(100 / this.Progress)) * (long)elapsed.TotalSeconds;

                if (etaSeconds < 0)
                    return null;

                int minutes = (int)(etaSeconds / 60);
                int seconds = (int)(etaSeconds - (minutes * 60));

                DateTime result = DateTime.Now;

                result = result.AddMinutes(minutes);
                result = result.AddSeconds(seconds);

                return result;
            }
        }*/

        /// <summary>
        /// Gets or sets the start time which is used to calculate the eta.
        /// </summary>
        public DateTime StartTime { get; private set; }


        protected Stopwatch swDatabaseInserts;
        protected Stopwatch swFileAccess;

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the base reader.
        /// </summary>
        /// <param name="fileName">The name of the file to read.</param>
        public BaseReader(string fileName, Core core, Study project, string createResponsesFile, int idLanguage)
        {
            this.FileName = fileName;
            this.CreateResponsesTemplateFile = createResponsesFile;
            this.Study = project;
            this.Core = core;
            this.IdLanguage = idLanguage;

            this.StartTime = DateTime.Now;

            swDatabaseInserts = new Stopwatch();
            swFileAccess = new Stopwatch();
        }

        #endregion


        #region Methods

        /// <summary>
        /// Reads the variables from the file.
        /// </summary>
        public abstract Variable[] Read();

        public abstract List<string> Validate();

        #endregion
    }

    public enum DataImportStatus
    {
        Step3,
        Step4,
        Step5,
        Step6
    }

    public enum DataUploadProvider
    {
        SPSS,
        Excel,
        CSV,
        LDF
    }
}
