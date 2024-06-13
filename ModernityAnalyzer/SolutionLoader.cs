using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.CSharp;
using static ModernityAnalyzer.Program;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;
using System.Text.Json;
using Newtonsoft.Json;

namespace ModernityAnalyzer
{
    public class SolutionLoader
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("ENGINE STARTED");

            // PATH TO REPOS FOR ANALYSIS
            string folderWithRepos = @"C:\Users\markt\Documents\CSharpRepos\";

            // PATH TO FOLDER TO WRITE RESULTS
            string pathToResults = @"C:\Users\markt\source\repos\Code Modernity Analyzer\Results";
            ClearDirectory(pathToResults);

            // Set up MSBuild workspace
            using var workspace = MSBuildWorkspace.Create();

            foreach (var repository in Directory.GetDirectories(folderWithRepos))
            {
                Console.WriteLine($"Working with repository: {repository}");

                // Find the solution
                var solutionFiles = Directory.GetFiles(repository, "*.sln", SearchOption.TopDirectoryOnly);

                // Open the solution
                var solution = await workspace.OpenSolutionAsync(solutionFiles[0]);

                // Get all C# files from the solution
                var csFiles = solution.Projects
                    .SelectMany(project => project.Documents)
                    .Where(document => document.FilePath.EndsWith(".cs"))
                    .Select(document => document.FilePath);

                if (!csFiles.Any())
                {
                    Console.WriteLine("No C# files found in the solution.");
                    return;
                }

                AnalysisData results = new AnalysisData();

                Console.WriteLine("C# Files:");
                foreach (var csFile in csFiles)
                {
                    // backup the previous output handler connected to Console
                    TextWriter backupOut = Console.Out;

                    // activate a null handle
                    Console.SetOut(TextWriter.Null);

                    string code = File.ReadAllText(csFile);
                    AnalysisData temp = analyzeFile(code);

                    // restore the previous handle
                    Console.SetOut(backupOut);

                    //Console.WriteLine($" - {csFile}");

                    results = AnalysisData.concatenateData(results, temp);
                    /*Console.WriteLine("Language version : Feature count");
                    foreach (double key in results.dataStruct.Keys)
                    {
                        Console.WriteLine($"{key} : {results.dataStruct[key]}");
                    }*/

                }

                Console.WriteLine("Output:\nC# Language Version : Feature Count");
                foreach (double key in results.dataStruct.Keys)
                {
                    Console.WriteLine($"{key} : {results.dataStruct[key]}");
                }

                // Write results to JSON file with the name of the repository
                var repoName = Path.GetFileName(repository);
                var jsonFilePath = Path.Combine(pathToResults, $"{repoName}_results.json");
                string json = JsonConvert.SerializeObject(results.dataStruct, Formatting.Indented);
                File.WriteAllText(jsonFilePath, json);

                Console.WriteLine($"Results written to {jsonFilePath}");

            }

            static void ClearDirectory(string path)
            {
                if (Directory.Exists(path))
                {
                    // Delete all files in the directory
                    foreach (string file in Directory.GetFiles(path))
                    {
                        File.Delete(file);
                    }

                    // Delete all subdirectories in the directory
                    foreach (string directory in Directory.GetDirectories(path))
                    {
                        Directory.Delete(directory, true);
                    }
                }
                else
                {
                    Console.WriteLine($"Directory does not exist: {path}");
                }
            }

            //Console.WriteLine("Ss");

        }
    }
}
