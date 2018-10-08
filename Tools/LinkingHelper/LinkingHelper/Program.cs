using ApplicationUtilities.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinkingHelper
{
    static class Program
    {
        static string PhysicalApplicationPath;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            PhysicalApplicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            AllocConsole();

            Console.BufferWidth = 120;
            Console.WindowWidth = 120;

            ReadFiles();

            Console.ReadLine();
        }

        static int count = 0;
        static int total = 0;
        static List<WordMatch> matches = new List<LinkingHelper.WordMatch>();

        static void ReadFiles()
        {
            Log("Reading files...", LogType.Information);

            List<string> files = new List<string>();

            foreach (string file in Directory.GetFiles(PhysicalApplicationPath))
            {
                if (file.EndsWith(".sav"))
                    files.Add(file);
            }

            if (files.Count == 0)
            {
                Log("no sav files found.", LogType.Error);
                return;
            }

            Log(files.Count + " files found.", LogType.Success);

            Dictionary<string, Metadata> metadata = new Dictionary<string, Metadata>();

            foreach (string file in files)
            {
                Console.WriteLine("");

                Log(string.Format(
                    "Reading metadata of file '{0}'...",
                    new FileInfo(file).Name
                ));

                metadata.Add(file, new Metadata(file));

                Log(string.Format(
                    "{0} variables found in metadata of file '{1}'...",
                    metadata[file].Variables.Count,
                    new FileInfo(file).Name
                ), LogType.Success);
            }

            Console.WriteLine("");
            Log("Searching matches...");
            Console.WriteLine("");
            Console.Write("0%");

            int variablesCount = 0;

            foreach (string file in metadata.Keys)
            {
                variablesCount += metadata[file].Variables.Count;

                foreach (string file2 in metadata.Keys)
                {
                    if (file == file2)
                        continue;

                    total += metadata[file].Variables.Count;
                }
            }

            Metadata taxonomy = new Metadata(null);

            foreach (string file in metadata.Keys)
            {
                foreach (string file2 in metadata.Keys)
                {
                    if (file == file2)
                        continue;

                    Compare(metadata[file], metadata[file2], taxonomy);
                }
            }

            ResetConsoleLine();
            Log(taxonomy.Variables.Count + " common variables out of " + 
                (variablesCount - taxonomy.Variables.Count) + " variables found.", 
                LogType.Success
            );
            Log(matches.Count + " word matches found.", LogType.Success);

            Metadata uncommonVariables = new Metadata(null);

            foreach (string file in metadata.Keys)
            {
                foreach (string variable in metadata[file].Variables.Keys)
                {
                    if (metadata[file].Variables[variable].Matched)
                        continue;
                    if (taxonomy.Variables.ContainsKey(variable))
                        continue;

                    uncommonVariables.Variables.Add(variable, metadata[file].Variables[variable]);

                    if (!metadata[file].Categories.ContainsKey(variable))
                        continue;

                    uncommonVariables.Categories[variable] = metadata[file].Categories[variable];
                    uncommonVariables.CategoryFactors[variable] = metadata[file].CategoryFactors[variable];
                }
            }

            Log("Writing taxonomy file...", LogType.Information);
            Log("Done.", LogType.Information);

            WriteTaxonomyExport(taxonomy, "Taxonomy");
            WriteTaxonomyExport(uncommonVariables, "UncommonVariables");

            if (matches.Count != 0)
            {
                WordMatchesOverview matchesOverview = new WordMatchesOverview();
                matchesOverview.LoadMatches(matches);
                matchesOverview.ShowDialog();
            }

            Form1 form = new Form1();
            form.LoadMetadata(taxonomy);
            //Application.Run(form);
            form.ShowDialog();
        }

        private static void WriteTaxonomyExport(Metadata taxonomy, string name)
        {
            ExcelWriter writer = new ExcelWriter();
            writer.Write(0, "Chapter");
            writer.Write(1, "Type");
            writer.Write(2, "Name");
            writer.Write(3, "Label");
            writer.Write(4, "Additional (SCALE, ...)");

            writer.NewLine();

            foreach (string variable in taxonomy.Variables.Keys)
            {
                writer.Write(0, "Default");
                writer.Write(1, "SINGLE");
                writer.Write(2, variable);
                writer.Write(3, taxonomy.Variables[variable].Label);

                writer.NewLine();
            }

            writer.NewSheet("Categories");

            writer.Write(0, "Variable");
            writer.Write(1, "Value");
            writer.Write(2, "Name");
            writer.Write(3, "Label");

            writer.NewLine();

            foreach (string variable in taxonomy.Categories.Keys)
            {
                foreach (string category in taxonomy.Categories[variable].Keys)
                {
                    writer.Write(0, variable);
                    writer.Write(1, taxonomy.CategoryFactors[variable][category].ToString());
                    writer.Write(2, category);
                    writer.Write(3, taxonomy.Categories[variable][category]);

                    writer.NewLine();
                }
            }

            writer.Save(Path.Combine(
                PhysicalApplicationPath,
                name + ".xlsx"
            ));
        }

        static void Compare(
            Metadata metadata1,
            Metadata metadata2,
            Metadata taxonomy
        )
        {
            foreach (string variable in metadata1.Variables.Keys)
            {
                if (taxonomy.Variables.ContainsKey(variable))
                {
                    count++;
                    LogProgress(count * 100 / total);
                    continue;
                }

                bool match = false;
                int highestWordMatchPerc = 0;
                string matchVariable = null;

                int wordMatchPerc = 0;
                foreach (string variable2 in metadata2.Variables.Keys)
                {
                    if (taxonomy.Variables.ContainsKey(variable2))
                        continue;

                    if (variable == variable2)
                    {
                        match = true;
                        matchVariable = variable2;
                        break;
                    }

                    if (metadata1.Variables[variable].Label == metadata2.Variables[variable2].Label)
                    {
                        match = true;
                        matchVariable = variable2;
                        break;
                    }

                    wordMatchPerc = WordMatch(
                        metadata1.Variables[variable].Label, 
                        metadata2.Variables[variable2].Label
                    );

                    if (wordMatchPerc > highestWordMatchPerc)
                    {
                        highestWordMatchPerc = wordMatchPerc;
                        matchVariable = variable2;
                    }
                }

                count++;
                LogProgress(count * 100 / total);

                if (match)
                {
                    metadata1.Variables[variable].Matched = true;
                    metadata2.Variables[matchVariable].Matched = true;

                    taxonomy.Variables.Add(variable, metadata1.Variables[variable]);

                    if (!metadata1.Categories.ContainsKey(variable))
                        return;

                    if (!metadata2.Categories.ContainsKey(matchVariable))
                        return;

                    taxonomy.Categories.Add(variable, new Dictionary<string, string>());
                    taxonomy.CategoryFactors.Add(variable, new Dictionary<string, double>());

                    match = false;
                    string matchCategory = null;

                    foreach (string category in metadata1.Categories[variable].Keys)
                    {
                        if (taxonomy.Categories[variable].ContainsKey(category))
                            continue;

                        foreach (string category2 in metadata2.Categories[matchVariable].Keys)
                        {
                            if (taxonomy.Categories[variable].ContainsKey(category2))
                                continue;

                            if (category == category2)
                            {
                                match = true;
                                matchCategory = category2;
                                break;
                            }

                            if (metadata1.Categories[variable][category] == 
                                metadata2.Categories[matchVariable][category2])
                            {
                                match = true;
                                matchCategory = category2;
                                break;
                            }
                        }

                        if (match)
                        {
                            taxonomy.Categories[variable].Add(category, metadata1.Categories[variable][category]);
                            taxonomy.CategoryFactors[variable].Add(category, metadata1.CategoryFactors[variable][category]);
                        }
                    }
                }
                else if (matchVariable != null && highestWordMatchPerc != 0)
                {
                    matches.Add(new LinkingHelper.WordMatch(
                        metadata1.FileName,
                        metadata2.FileName,
                        variable,
                        matchVariable,
                        metadata1.Variables[variable].Label,
                        metadata2.Variables[matchVariable].Label,
                        highestWordMatchPerc
                    ));
                }
            }
        }

        static void LogProgress(double perc)
        {
            ResetConsoleLine();
            Console.Write(perc);
            Console.Write("%");
        }

        static void ResetConsoleLine()
        {
            int oldCL = Console.CursorLeft;

            Console.CursorLeft = 0;

            for (int i = 0; i < oldCL; i++)
            {
                Console.Write(" ");
            }

            Console.CursorLeft = 0;
        }

        static int WordMatch(string sentence1, string sentence2)
        {
            string[] ignoreWords = new string[]
            {
                ":",
                ".",
                ","
            };

            int count = 0;

            string[] words1 = sentence1.Split(' ');
            string[] words2 = sentence2.Split(' ');

            foreach (string word1 in words1)
            {
                if (ignoreWords.Contains(word1))
                    continue;

                if (words2.Contains(word1))
                    count++;
            }
            foreach (string word2 in words2)
            {
                if (ignoreWords.Contains(word2))
                    continue;

                if (words1.Contains(word2))
                    count++;
            }

            return count * 100 / (words1.Length + words2.Length);
        }

        static void Log(string message, LogType type = LogType.Information)
        {
            switch (type)
            {
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogType.Information:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }

            Console.WriteLine(string.Format(
                "[{0}]\t{1}",
                DateTime.Now.ToString("HH:mm"),
                message
            ));

            Console.ResetColor();
        }
    }
    public enum LogType
    {
        Error,
        Information,
        Success
    }
}
