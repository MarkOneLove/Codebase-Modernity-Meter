using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.CSharp;
using static ModernityAnalyzer.Program;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace ModernityAnalyzer
{
    public class SolutionLoader
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("STARTED THE ENGINE");

            var folderWithRepos = @"C:\Users\markt\Documents\CSharpRepos\";

            // Set up MSBuild workspace
            using var workspace = MSBuildWorkspace.Create();

            foreach (var repository in Directory.GetDirectories(folderWithRepos))
            {
                Console.WriteLine($"Working with repository: {repository}");

                try
                {
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

                        Console.WriteLine($" - {csFile}");

                        results = AnalysisData.concatenateData(results, temp);
                        Console.WriteLine("Language version : Feature count");
                        foreach (double key in results.dataStruct.Keys)
                        {
                            Console.WriteLine($"{key} : {results.dataStruct[key]}");
                        }

                    }

                    Console.WriteLine("Final output \n Language version : Feature count");
                    foreach (double key in results.dataStruct.Keys)
                    {
                        Console.WriteLine($"{key} : {results.dataStruct[key]}");
                    }

                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"Failed to find solution in {repository}. Error: {ex.Message}");
                }

            }

        }
    }
}
