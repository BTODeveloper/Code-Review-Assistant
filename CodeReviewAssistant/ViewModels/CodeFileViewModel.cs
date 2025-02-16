using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CodeReviewAssistant.Models;

namespace CodeReviewAssistant.ViewModels
{
    public partial class CodeFileViewModel : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string fullPath;

        [ObservableProperty]
        private string diffContent;

        [ObservableProperty]
        private ObservableCollection<AnalysisResult> analysisResults = new();

        [ObservableProperty]
        private int addedLines;

        [ObservableProperty]
        private int removedLines;

        public string ChangeStats => $"+{AddedLines}/-{RemovedLines}";

        public CodeFileViewModel(CodeFile file)
        {
            Name = file.Name;
            FullPath = file.FullPath;
            DiffContent = file.DiffContent;
            AddedLines = file.AddedLines;
            RemovedLines = file.RemovedLines;
        }
    }
}