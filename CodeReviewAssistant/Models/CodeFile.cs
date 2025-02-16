namespace CodeReviewAssistant.Models
{
    public class CodeFile
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public string DiffContent { get; set; }
        public int AddedLines { get; set; }
        public int RemovedLines { get; set; }
    }
}