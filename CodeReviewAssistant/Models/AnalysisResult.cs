namespace CodeReviewAssistant.Models
{
    public class AnalysisResult
    {
        public string Message { get; set; }
        public string Location { get; set; }
        public Severity Severity { get; set; }
        public string SeverityColor => Severity switch
        {
            Severity.Error => "#F14C4C",
            Severity.Warning => "#CCA700",
            _ => "#4CAF50"
        };
    }

    public enum Severity
    {
        Info,
        Warning,
        Error
    }
}