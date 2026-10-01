namespace SistemaUtilidadePublicaAPI.Models
{
    public class AIAnalysisResult
    {
        public bool Approved { get; set; }

        public string Relevance { get; set; } = string.Empty;

        public string Safety { get; set; } = string.Empty;

        public string Truthfulness { get; set; } = string.Empty;

        public string PublicUtility { get; set; } = string.Empty;

        public string Risk { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;

        public List<string> Recommendations { get; set; } = new();
    }
}