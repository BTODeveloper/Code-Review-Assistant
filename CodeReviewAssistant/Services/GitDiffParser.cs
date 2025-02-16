using System.IO;
using CodeReviewAssistant.Models;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace CodeReviewAssistant.Services
{
    public class GitDiffParser
    {
        public CodeFile ParseDiff(string diffContent, string filePath)
        {
            var file = new CodeFile
            {
                Name = Path.GetFileName(filePath),
                FullPath = filePath,
                DiffContent = diffContent
            };

            // Count added/removed lines
            var lines = diffContent.Split('\n');
            foreach (var line in lines)
            {
                if (line.StartsWith('+')) file.AddedLines++;
                if (line.StartsWith('-')) file.RemovedLines++;
            }

            return file;
        }

        public async Task<List<CodeFile>> LoadFromFile(string path)
        {
            var content = await File.ReadAllTextAsync(path);
            var files = new List<CodeFile>();
            
            // Simple diff parsing (you can enhance this)
            var sections = content.Split("diff --git");
            foreach (var section in sections.Skip(1))
            {
                var lines = section.Split('\n');
                var filePath = lines[0].Trim().Split(' ').Last();
                files.Add(ParseDiff(string.Join('\n', lines.Skip(1)), filePath));
            }

            return files;
        }
    }
}