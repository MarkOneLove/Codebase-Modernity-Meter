using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.CSharp;
using static ModernityAnalyzer.Program;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;
using System.Text.Json;
using Newtonsoft.Json;
using LibGit2Sharp;
using Microsoft.CodeAnalysis;

namespace ModernityAnalyzer
{
    public class SolutionLoader
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("ENGINE STARTED");

            // PATH TO REPOS FOR ANALYSIS
            string folderWithRepos = @"C:\Users\Mark\Desktop\Code Modernity Analyzer\CSharpRepos";

            // PATH TO FOLDER TO WRITE RESULTS
            string pathToResults = @"C:\Users\Mark\Desktop\Code Modernity Analyzer\Results5";
            ClearDirectory(pathToResults);

            // Set up MSBuild workspace
            using var workspace = MSBuildWorkspace.Create();

            // Get list of repos for analysis
            Repositories repos = new Repositories();

            foreach (var repository in repos.listOfRepos.Keys)
            {
                Console.WriteLine($"Working with repository: {repository}");

                string repoPath = Path.Combine(folderWithRepos, repository);

                // Create directory
                /*if (!Directory.Exists(repoPath))
                {
                    Directory.CreateDirectory(repoPath);
                } else
                {
                    Directory.Delete(repoPath);
                    Directory.CreateDirectory(repoPath);
                }*/

                Repository.Clone(repos.listOfRepos[repository], repoPath);

                // Open the repository
                using (var repo = new Repository(repoPath))
                {
                    // Get the commit log
                    var commits = repo.Commits;

                    // Get all tags
                    //var tags = repo.Tags;

                    // Filter tags that are considered release tags (e.g., tags starting with "v" or "release")
                    //var releaseTags = tags.Where(tag => tag.FriendlyName.StartsWith("v") || tag.FriendlyName.StartsWith("release"));

                    //await Console.Out.WriteLineAsync(repo.Commits.Count());
                    int i = 0;

                    /*foreach (var tag in releaseTags)
                    {
                        i++;
                        var temp = tag as Tag;
                        try
                        {
                            if (temp != null)
                            {
                                // Annotated tag: print tagger date
                                Console.WriteLine($"{temp.}");
                            }
                        } catch { }
                    }*/

                    Console.WriteLine($"CNT: {commits.Count()}");

                    foreach (var commit in commits)
                    {
                        if (i == 20)
                        {
                            /*var tags = repo.Tags.Where(tag => tag.Target.Id == commit.Id && 
                                (tag.FriendlyName.StartsWith("v") || tag.FriendlyName.StartsWith("release")));
                            var com = commit as Commit;
                            if (tags.Count() > 0)
                            {
                                Console.WriteLine(com.Committer.When);
                                i++;

                                // Update repo to this state
                                Commands.Checkout(repo, commit);

                                // Find the solution
                                var solutionFiles = Directory.GetFiles(repoPath, "*.sln", SearchOption.TopDirectoryOnly);

                                // Open the solution
                                var solution = await workspace.OpenSolutionAsync(solutionFiles[0]);

                                AnalyzeRepository(solution, repoPath, pathToResults, com.Committer.When.DateTime.ToString("dd-MM-yyyy"));
                            }*/

                            var com = commit as Commit;

                            Console.WriteLine(com.Committer.When);

                            // Update repo to this state
                            Commands.Checkout(repo, commit);

                            // Find the solution
                            var solutionFiles = Directory.GetFiles(repoPath, "*.sln", SearchOption.TopDirectoryOnly);

                            // Open the solution
                            var solution = await workspace.OpenSolutionAsync(solutionFiles[0]);

                            AnalyzeRepository(solution, repoPath, pathToResults, com.Committer.When.DateTime.ToString("dd-MM-yyyy"));

                            i = 0;
                        } else
                        {
                            i++;
                        }

                        // Checkout the specific commit
                        //Commands.Checkout(repo, commit);

                        // Path to the working directory (same as localRepoPath)
                        //string workingDirectory = repo.Info.WorkingDirectory;

                        // Call your analysis tool here, passing the working directory
                        //AnalyzeRepository(workingDirectory);

                        //Console.WriteLine($"Analyzed commit: {commit.Id}");
                    }
                    await Console.Out.WriteLineAsync(i.ToString());
                }

            }

        }

        static void AnalyzeRepository(Solution solution, string repository, string pathToResults, string date)
        {

            //Console.WriteLine($"Working with repository: {repository}");

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

            //Console.WriteLine("C# Files:");
            foreach (var csFile in csFiles)
            {
                try
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
                }
                catch { }

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
            var jsonFilePath = Path.Combine(pathToResults, $"{repoName}_{date}.json");
            string json = JsonConvert.SerializeObject(results.dataStruct, Formatting.Indented);
            File.WriteAllText(jsonFilePath, json);

            Console.WriteLine($"Results written to {jsonFilePath}");

            //return null;
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
    }
}
