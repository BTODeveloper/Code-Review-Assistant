using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using CodeReviewAssistant.Models;

namespace CodeReviewAssistant.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<CodeFileViewModel> files = new();

        [ObservableProperty]
        private CodeFileViewModel currentFile;

        [ObservableProperty]
        private int filesCount;

        [ObservableProperty]
        private int totalLinesAdded;

        [ObservableProperty]
        private int totalLinesRemoved;

        [RelayCommand]
        private Task LoadDiff()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Diff files (*.diff)|*.diff|Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                return LoadDiffFromFile(dialog.FileName);
            }

            return Task.CompletedTask;
        }

        public async Task LoadDiffFromFile(string filePath)
        {
            var content = await File.ReadAllTextAsync(filePath);
            var parsedFiles = ParseDiffContent(content);

            Files.Clear();
            TotalLinesAdded = 0;
            TotalLinesRemoved = 0;

            foreach (var file in parsedFiles)
            {
                var fileVM = new CodeFileViewModel(file);
                Files.Add(fileVM);
                AnalyzeFile(fileVM);

                TotalLinesAdded += file.AddedLines;
                TotalLinesRemoved += file.RemovedLines;
            }

            FilesCount = Files.Count;
        }

        private List<CodeFile> ParseDiffContent(string content)
        {
            var files = new List<CodeFile>();
            var sections = content.Split("diff --git");

            foreach (var section in sections.Skip(1))
            {
                var lines = section.Split('\n');
                var filePath = lines[0].Trim().Split(' ').Last();
                
                var file = new CodeFile
                {
                    Name = Path.GetFileName(filePath),
                    FullPath = filePath,
                    DiffContent = string.Join('\n', lines.Skip(1)),
                    AddedLines = lines.Count(l => l.StartsWith('+')),
                    RemovedLines = lines.Count(l => l.StartsWith('-'))
                };

                files.Add(file);
            }

            return files;
        }

        private void AnalyzeFile(CodeFileViewModel file)
        {
            file.AnalysisResults.Clear();

            // Check for async/await usage
            if (file.DiffContent.Contains("async") && file.DiffContent.Contains("await"))
            {
                file.AnalysisResults.Add(new AnalysisResult
                {
                    Message = "Async/Await pattern introduced",
                    Severity = Severity.Info,
                    Location = "Async Programming"
                });
            }

            // Check for new method calls
            if (file.DiffContent.Contains("GetData()") || file.DiffContent.Contains("AnalyzeResults"))
            {
                file.AnalysisResults.Add(new AnalysisResult
                {
                    Message = "New method calls added",
                    Severity = Severity.Info,
                    Location = "Method Modification"
                });
            }

            // Detect potential code smells
            if (file.DiffContent.Contains("catch (Exception)"))
            {
                file.AnalysisResults.Add(new AnalysisResult
                {
                    Message = "Avoid catching generic exceptions",
                    Severity = Severity.Error,
                    Location = "Exception Handling"
                });
            }

            // Detect commented-out code
            if (Regex.IsMatch(file.DiffContent, @"//\s*[^\s]+"))
            {
                file.AnalysisResults.Add(new AnalysisResult
                {
                    Message = "Commented-out code detected",
                    Severity = Severity.Info,
                    Location = "Code Cleanliness"
                });
            }
        }

        private int CalculateCodeComplexity(string code)
        {
            // Simple complexity calculation
            int complexity = 0;
            complexity += Regex.Matches(code, @"\b(if|else|switch|case|for|while|catch)\b").Count;
            complexity += Regex.Matches(code, @"&&|\|\|").Count;
            return complexity;
        }

        [RelayCommand]
        private void ExportAnalysis()
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt",
                Title = "Export Code Review Analysis"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var analysisReport = GenerateAnalysisReport();
                File.WriteAllText(saveDialog.FileName, analysisReport);
                MessageBox.Show("Analysis exported successfully!", "Export Complete");
            }
        }

        private string GenerateAnalysisReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine("File,Location,Message,Severity");
            
            foreach (var file in Files)
            {
                foreach (var result in file.AnalysisResults)
                {
                    sb.AppendLine($"{file.Name},{result.Location},{result.Message},{result.Severity}");
                }
            }

            return sb.ToString();
        }

        public void SelectedFileChanged(CodeFileViewModel file)
        {
            if (file != null)
            {
                CurrentFile = file;
            }
        }
    }
}