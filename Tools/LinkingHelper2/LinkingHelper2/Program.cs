using ApplicationUtilities.Classes;
using LinkingHelper2.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LinkingHelper2
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WindowWidth = Console.LargestWindowWidth - 20;
            Console.BufferWidth = Console.LargestWindowWidth - 20;

            string physicalApplicationPath = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location
            );

            Console.WriteLine("Press enter to start.");
            Console.ReadLine();

            string fileName = Path.Combine(physicalApplicationPath, "Taxonomy.xlsx");

            if (!File.Exists(fileName))
            {
                Log("Taxonomy file was not found. Please store taxonomy file " +
                    "called 'Taxonomy.xlsx' in the application directory.", LogType.Error);
                Console.WriteLine("Press any key to quit.");
                Console.ReadLine();
                return;
            }

            Log("Parsing taxonomy file...", LogType.Information);

            Dictionary<string, TaxonomyVariable> taxonomy = ParseTaxonomyFile(fileName);

            Log(string.Format(
                "Parsing taxonomy file finished successfully. {0} variables found." + Environment.NewLine,
                taxonomy.Count
            ), LogType.Success);

            List<Study> linkedStudies = new List<Study>();
            List<Study> unlinkedStudies = new List<Study>();

            // Run through all sub directories of the application's
            // directory which contain the already linked studies.
            foreach (string directoryName in Directory.GetDirectories(physicalApplicationPath))
            {
                fileName = null;
                foreach (string file in Directory.GetFiles(directoryName))
                {
                    if (file.EndsWith(".xlsx"))
                    {
                        fileName = file;
                        break;
                    }
                }

                if (fileName == null)
                {
                    Log(string.Format(
                        "Augment file was not found in directory '{0}'. Ignoring study...",
                        new DirectoryInfo(directoryName).Name
                    ), LogType.Error);
                    continue;
                }

                string augmentFileName = fileName;

                fileName = null;
                foreach (string file in Directory.GetFiles(directoryName))
                {
                    if (file.ToLower().EndsWith(".sav") ||
                        file.ToLower().EndsWith(".pkd") ||
                        file.ToLower().EndsWith(".mdd"))
                    {
                        fileName = file;
                        break;
                    }
                }

                if (fileName == null)
                {
                    Log(string.Format(
                        "Study file was not found in directory '{0}'. Ignoring study...",
                        new DirectoryInfo(directoryName).Name
                    ), LogType.Error);
                    continue;
                }

                Log(string.Format(
                    "Parsing study '{0}'...",
                    new FileInfo(fileName).Name
                ), LogType.Information);

                Study study = new Study();
                study.Parse(fileName, augmentFileName);

                linkedStudies.Add(study);

                Log(string.Format(
                    "Parsing study '{0}' finished successfully." + Environment.NewLine,
                    new FileInfo(fileName).Name
                ), LogType.Success);
            }

            fileName = null;
            foreach (string file in Directory.GetFiles(physicalApplicationPath))
            {
                if (file.EndsWith(".sav") ||
                    file.EndsWith(".pkd") ||
                    file.EndsWith(".mdd"))
                {
                    Log(string.Format(
                        "Parsing unlinked study '{0}'...",
                        new FileInfo(file).Name
                    ), LogType.Information);

                    Study study = new Study();
                    study.ParseMetadata(file);

                    unlinkedStudies.Add(study);

                    Log(string.Format(
                        "Parsing unlinked study '{0}' finished successfully." + Environment.NewLine,
                        new FileInfo(file).Name
                    ), LogType.Success);
                }
            }

            List<string> newTaxonomyVariables = new List<string>();

            ExcelWriter writer;
            foreach (Study unlinkedStudy in unlinkedStudies)
            {
                writer = new ExcelWriter();
                writer.Write(0, "Taxonomy variable name");
                writer.Write(1, "variable name");
                writer.Write(2, "Taxonomy category name");
                writer.Write(3, "category name");

                writer.NewLine();

                Log(string.Format(
                    "Finding matches for study '{0}'...",
                    unlinkedStudy.Name
                ), LogType.Information);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                List<WordMatch> matches = new List<Classes.WordMatch>();

                ulong total = 0;
                foreach (string variable in unlinkedStudy.Variables.Keys)
                {
                    foreach (Study linkedStudy in linkedStudies)
                    {
                        total += (ulong)linkedStudy.Variables.Count;
                    }
                }
                ulong steps = total / 100;
                ulong count = 0;

                // Run through all the variables of the unlinked study.
                foreach (string variable in unlinkedStudy.Variables.Keys)
                {

                    #region Step 1

                    // Step 1: Match 100% agains other studies labels.
                    Dictionary<string, Dictionary<string, List<string>>> matchedTaxonomyVariables = new Dictionary<string, Dictionary<string, List<string>>>();

                    foreach (Study linkedStudy in linkedStudies)
                    {
                        if (linkedStudy.VariableLinks.ContainsKey(unlinkedStudy.Variables[variable].Label))
                        {
                            foreach (string matchedTaxonomyVariable in linkedStudy.VariableLinks[unlinkedStudy.Variables[variable].Label].First().Value)
                            {
                                if (matchedTaxonomyVariables.ContainsKey(matchedTaxonomyVariable))
                                    continue;

                                matchedTaxonomyVariables.Add(matchedTaxonomyVariable, new Dictionary<string, List<string>>());

                                foreach (string category in unlinkedStudy.Variables[variable].Categories.Keys)
                                {
                                    if (!linkedStudy.CategoryLinks.ContainsKey(variable))
                                        break;

                                    if (!linkedStudy.CategoryLinks[variable].ContainsKey(category))
                                        continue;

                                    if (!matchedTaxonomyVariables[matchedTaxonomyVariable].ContainsKey(category))
                                        matchedTaxonomyVariables[matchedTaxonomyVariable].Add(category, linkedStudy.CategoryLinks[variable][category]);
                                }
                            }
                            break;
                        }
                    }

                    #endregion

                    #region Step 2

                    // Step 2: Match 100% agains taxonomy variable labels.
                    if (matchedTaxonomyVariables.Count == 0)
                    {
                        Dictionary<string, string> matchedCategories = new Dictionary<string, string>();
                        foreach (string taxonomyVariable in taxonomy.Keys)
                        {
                            if (unlinkedStudy.Variables[variable].Label == taxonomy[taxonomyVariable.ToUpper()].Label)
                            {
                                foreach (string category in unlinkedStudy.Variables[variable].Categories.Keys)
                                {
                                    foreach (string taxonomyCategory in taxonomy[taxonomyVariable.ToUpper()].Categories.Keys)
                                    {
                                        if (category == taxonomyCategory)
                                        {
                                            matchedCategories.Add(category, taxonomyCategory);
                                            break;
                                        }

                                        if (unlinkedStudy.Variables[variable].Categories[category].Label ==
                                            taxonomy[taxonomyVariable.ToUpper()].Categories[taxonomyCategory.ToUpper()].Label)
                                        {
                                            matchedCategories.Add(category, taxonomyCategory);
                                            break;
                                        }
                                    }
                                }

                                matchedTaxonomyVariables.Add(taxonomyVariable, new Dictionary<string, List<string>>());

                                foreach (string category in matchedCategories.Keys)
                                {
                                    matchedTaxonomyVariables[taxonomyVariable].Add(category, new List<string>());
                                    matchedTaxonomyVariables[taxonomyVariable][category].Add(matchedCategories[category]);
                                }
                            }
                        }
                    }

                    #endregion

                    #region Step 3

                    // Step 3: Word match agains other studies labels.
                    if (matchedTaxonomyVariables.Count == 0)
                    {
                        /*Dictionary<TaxonomyVariable, double> mm = new Dictionary<TaxonomyVariable, double>();
                        List<Task> tasks = new List<Task>();*/

                        double highestMatch = 0;
                        TaxonomyVariable highestMatchVariable = null;

                        double match;

                        // Run through all linked studies.
                        foreach (Study linkedStudy in linkedStudies)
                        {
                            // Run through all variables of the linked study.
                            foreach (string linkedVariable in linkedStudy.Variables.Keys)
                            {
                                count++;

                                if (count % steps == 0)
                                {
                                    Console.CursorLeft = 0;
                                    Console.Write("     ");
                                    Console.CursorLeft = 0;
                                    Console.Write(count * 100 / total);
                                    Console.Write(" %");
                                }

                                /*Task task = new Task(() =>
                                {*/
                                if (!linkedStudy.Variables[linkedVariable].Linked)
                                        continue;

                                    // Word match the labels.
                                    match = WordMatch(
                                        unlinkedStudy.Variables[variable].Label,
                                        linkedStudy.Variables[linkedVariable].Label
                                    );

                                    if (match < 40)
                                        continue;

                                    // TEST:
                                    /*WordMatch(
                                        unlinkedStudy.Variables[variable].Label,
                                        linkedStudy.Variables[linkedVariable].Label,
                                        true
                                    );*/

                                    if (match > highestMatch)
                                    {
                                        highestMatch = match;
                                        highestMatchVariable = linkedStudy.Variables[linkedVariable];
                                    }

                                    /*lock (mm)
                                    {
                                        mm.Add(linkedStudy.Variables[linkedVariable], match);
                                    }*/
                                //});

                                /*tasks.Add(task);

                                task.Start();*/
                            }
                        }

                        /*Task.WaitAll(tasks.ToArray());

                        foreach (TaxonomyVariable item in mm.Keys)
                        {
                            if (mm[item] > highestMatch)
                            {
                                highestMatch = mm[item];
                                highestMatchVariable = item;
                            }
                        }
                        
                        Console.CursorLeft = 0;
                        Console.Write("     ");
                        Console.CursorLeft = 0;
                        Console.Write(count * 100 / total);
                        Console.Write(" %");*/

                        if (highestMatchVariable != null)
                        {
                            WordMatch wordMatch = new Classes.WordMatch();
                            wordMatch.TaxonomyVariable = highestMatchVariable.LinkedTaxonomyVariable;
                            wordMatch.LinkedVariable = highestMatchVariable;
                            wordMatch.UnlinkedVariable = unlinkedStudy.Variables[variable];
                            wordMatch.Match = highestMatch;

                            matches.Add(wordMatch);
                            /*Console.WriteLine(string.Format(
                                "Highest match for {0}: {1}",
                                variable,
                                test[0].Value
                            ));*/
                        }
                    }

                    #endregion

                    foreach (string matchedTaxonomyVariable in matchedTaxonomyVariables.Keys)
                    {
                        foreach (string categoryName in matchedTaxonomyVariables[matchedTaxonomyVariable].Keys)
                        {
                            foreach (string taxonomyCategoryName in matchedTaxonomyVariables[matchedTaxonomyVariable][categoryName])
                            {
                                writer.Write(0, matchedTaxonomyVariable);
                                writer.Write(1, variable);
                                writer.Write(2, taxonomyCategoryName);
                                writer.Write(3, categoryName);

                                writer.NewLine();
                            }
                        }
                    }
                }

                Console.Write(Environment.NewLine);

                stopwatch.Stop();

                Console.WriteLine(string.Format(
                    "Elapsed: {0}",
                    stopwatch.ElapsedMilliseconds
                ));

                string directoryName = Path.Combine(
                    physicalApplicationPath,
                    unlinkedStudy.Name
                );

                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                writer.Save(Path.Combine(
                    directoryName,
                    unlinkedStudy.Name + ".xlsx"
                ));

                File.Move(unlinkedStudy.Source, Path.Combine(
                    directoryName,
                    new FileInfo(unlinkedStudy.Source).Name
                ));


                test1 = new StringBuilder();
                test2 = new StringBuilder();

                foreach (string variable in unlinkedStudies[0].Variables.Keys)
                {
                    test1.Append(unlinkedStudies[0].Variables[variable].Label);
                    test1.Append(Environment.NewLine);
                }
                foreach (string variable in linkedStudies[0].Variables.Keys)
                {
                    test2.Append(linkedStudies[0].Variables[variable].Label);
                    test2.Append(Environment.NewLine);
                }

                File.WriteAllText(Path.Combine(physicalApplicationPath, "Text1.txt"), test1.ToString());
                File.WriteAllText(Path.Combine(physicalApplicationPath, "Text2.txt"), test2.ToString());

                ExcelWriter writerMatches = new ExcelWriter();

                writerMatches.Write(0, "Match");
                writerMatches.Write(1, "Taxonomy variable name");
                writerMatches.Write(2, "Taxonomy variable label");
                writerMatches.Write(3, "Study variable name");
                writerMatches.Write(4, "Study variable label");

                writerMatches.NewLine();

                foreach (WordMatch match in matches.OrderByDescending(x => x.Match))
                {
                    if (!taxonomy.ContainsKey(match.LinkedVariable.LinkedTaxonomyVariable.ToUpper()))
                    {
                        Log(string.Format(
                            "Taxonomy variable '{0}' doesn't exist in the taxonomy. Linked in study: '{1}'",
                            match.LinkedVariable.LinkedTaxonomyVariable,
                            match.LinkedVariable.Study.Name
                        ), LogType.Error);
                        continue;
                    }

                    writerMatches.Write(0, Math.Round(match.Match, 2) + " %");
                    writerMatches.Write(1, match.LinkedVariable.LinkedTaxonomyVariable);
                    writerMatches.Write(2, taxonomy[match.LinkedVariable.LinkedTaxonomyVariable.ToUpper()].Label);
                    writerMatches.Write(3, match.UnlinkedVariable.Name);
                    writerMatches.Write(4, match.UnlinkedVariable.Label);

                    writerMatches.NewLine();
                }

                writerMatches.Save(Path.Combine(
                    directoryName,
                    "Matches.xlsx"
                ));
 
                foreach (WordMatch match in matches.OrderByDescending(x => x.Match))
                {
                    /*Forms.WordMatchView view = new Forms.WordMatchView(match);
                    view.Show();*/

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(Math.Round(match.Match, 2) + " % match:");
                    WordMatch(
                        match.UnlinkedVariable.Label,
                        match.LinkedVariable.Label,
                        true
                    );

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("Link? (y=yes/n=no/b=finished/c=create): " + Environment.NewLine);

                    string answer = Console.ReadLine();

                    Console.WriteLine(Environment.NewLine);

                    if (answer.Trim() == "b")
                        break;

                    if (answer.Trim() == "c")
                    {
                        string name = match.UnlinkedVariable.Label
                                .Replace(" ", "")
                                .Replace("'", "")
                                .Replace("?", "")
                                .Replace("!", "")
                                .Replace(".", "")
                                .Replace(",", "")
                                .Replace(";", "");

                        while (taxonomy.ContainsKey(name.ToUpper()))
                        {
                            name += "_";
                        }

                        TaxonomyVariable taxonomyVariable = new TaxonomyVariable(name, match.UnlinkedVariable.Label);

                        foreach (string category in match.UnlinkedVariable.Categories.Keys)
                        {
                            taxonomyVariable.Categories.Add(category, match.UnlinkedVariable.Categories[category]);
                            taxonomyVariable.Categories[category].Value = match.UnlinkedVariable.Categories[category].Value;
                        }

                        taxonomy.Add(name, taxonomyVariable);

                        match.TaxonomyVariable = name;

                        newTaxonomyVariables.Add(name);
                    }

                    if (answer.Trim() != "y")
                        continue;

                    if (!taxonomy.ContainsKey(match.TaxonomyVariable.ToUpper()))
                        continue;

                    foreach (string category in match.UnlinkedVariable.Categories.Keys)
                    {
                        foreach (string taxonomyCategory in taxonomy[match.TaxonomyVariable.ToUpper()].Categories.Keys)
                        {
                            if (category == taxonomyCategory)
                            {

                                writer.Write(0, match.LinkedVariable.LinkedTaxonomyVariable);
                                writer.Write(1, match.UnlinkedVariable.Name);
                                writer.Write(2, taxonomyCategory);
                                writer.Write(3, category);

                                writer.NewLine();
                                break;
                            }

                            if (match.UnlinkedVariable.Categories[category].Label ==
                                taxonomy[match.TaxonomyVariable.ToUpper()].Categories[taxonomyCategory.ToUpper()].Label)
                            {

                                writer.Write(0, match.LinkedVariable.LinkedTaxonomyVariable);
                                writer.Write(1, match.UnlinkedVariable.Name);
                                writer.Write(2, taxonomyCategory);
                                writer.Write(3, category);

                                writer.NewLine();
                                break;
                            }
                        }
                    }

                    writer.Save(Path.Combine(
                        directoryName,
                        unlinkedStudy.Name + ".xlsx"
                    ));
                }

                //Log(string.Format(
                //    "Parsing study '{0}' finished successfully.",
                //    unlinkedStudy.Name
                //), LogType.Success);
            }

            writer = new ExcelWriter();
            writer.Write(0, "Chapter");
            writer.Write(1, "Variable type");
            writer.Write(2, "Variable name");
            writer.Write(3, "Variable label");
            writer.Write(4, "Additional (SCALE, ...)");
            writer.Write(5, "Hierarchy");

            writer.NewLine();

            foreach (string taxonomyVariable in newTaxonomyVariables)
            {
                writer.Write(0, "Default");
                writer.Write(1, "Single");
                writer.Write(2, taxonomyVariable);
                writer.Write(3, taxonomy[taxonomyVariable.ToUpper()].Label);
                writer.Write(4, "");
                writer.Write(5, "Root");

                writer.NewLine();
            }

            writer.NewSheet("Categories");
            writer.Write(0, "Variable");
            writer.Write(1, "Value");
            writer.Write(2, "Category name");
            writer.Write(3, "Category label");
            writer.Write(4, "Additional (HIDDEN, ...)");
            writer.Write(5, "Hierarchy");

            writer.NewLine();

            foreach (string taxonomyVariable in newTaxonomyVariables)
            {
                foreach (string category in taxonomy[taxonomyVariable.ToUpper()].Categories.Keys)
                {
                    writer.Write(0, taxonomyVariable);
                    writer.Write(1, taxonomy[taxonomyVariable.ToUpper()].Categories[category.ToUpper()].Value.ToString());
                    writer.Write(2, category);
                    writer.Write(3, taxonomy[taxonomyVariable.ToUpper()].Categories[category.ToUpper()].Label);
                    writer.Write(4, "");
                    writer.Write(5, "Root");

                    writer.NewLine();
                }
            }

            writer.Save(Path.Combine(
                physicalApplicationPath,
                "NewTaxonomyVariables.xlsx"
            ));

            Console.WriteLine("");
            Console.WriteLine("Finished. Press any key to exit.");
            Console.ReadLine();
        }

        private static double WordMatch(string label1, string label2, bool print)
        {
            Console.ResetColor();

            int count = 0;

            string[] words1 = label1.Split(separators);
            string[] words2 = label2.Split(separators);

            int index = 0;
            foreach (string word1 in words1)
            {
                index += word1.Length + 1;

                if (words2.Contains(word1))
                    count++;
                else if (print)
                    Console.ForegroundColor = ConsoleColor.Yellow;

                if (print)
                {
                    Console.Write(word1);

                    if (label1.Length > (index - 1))
                        Console.Write(label1[index - 1]);

                    Console.ResetColor();
                }
            }

            if (print)
                Console.WriteLine("");

            index = 0;
            foreach (string word2 in words2)
            {
                index += word2.Length + 1;

                if (words1.Contains(word2))
                    count++;
                else if (print)
                    Console.ForegroundColor = ConsoleColor.Yellow;

                if (print)
                {
                    Console.Write(word2);

                    if (label2.Length > (index - 1))
                        Console.Write(label2[index - 1]);

                    Console.ResetColor();
                }
            }
            if (print)
                Console.WriteLine(Environment.NewLine);

            return count * 100.0 / (words1.Length + words2.Length);
        }

        static char[] separators = new char[]
        {
                ' ', '=', '<', '>', '.', ',', '!', '?', '-', ':'
        };
        static StringBuilder test1 = new StringBuilder();
        static StringBuilder test2 = new StringBuilder();
        private static double WordMatch(string label1, string label2)
        {

            int count = 0;

            string[] words1 = label1.Split(separators);
            string[] words2 = label2.Split(separators);

            foreach (string word1 in words1)
            {
                if (words2.Contains(word1))
                    count++;
            }

            foreach (string word2 in words2)
            {
                if (words1.Contains(word2))
                    count++;
            }

            return count * 100.0 / (words1.Length + words2.Length);
        }

        static Dictionary<string, TaxonomyVariable> ParseTaxonomyFile(string fileName)
        {
            Dictionary<string, TaxonomyVariable> result = new Dictionary<string, TaxonomyVariable>();

            ExcelReader reader = new ExcelReader(fileName);

            reader.ActiveSheet = reader.Workbook.Worksheets[0];

            string name;
            string label;
            while (reader.Read())
            {
                name = reader[2].Trim();
                label = reader[3].Trim().ToLower();

                if (result.ContainsKey(name.ToUpper()))
                    continue;

                result.Add(name.ToUpper(), new TaxonomyVariable(name, label));
            }

            reader.ActiveSheet = reader.Workbook.Worksheets[1];
            reader.Position = 0;

            string variable;
            while (reader.Read())
            {
                variable = reader[0].Trim().ToUpper();
                name = reader[2].Trim();
                label = reader[3].Trim().ToLower();

                if (!result.ContainsKey(variable))
                    continue;

                if (result[variable].Categories.ContainsKey(name.ToUpper()))
                    continue;

                result[variable].Categories.Add(name.ToUpper(), new TaxonomyCategory(result[variable], name, label));
            }

            return result;
        }

        static void Log(string message, LogType type)
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

            Console.WriteLine(message);

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
