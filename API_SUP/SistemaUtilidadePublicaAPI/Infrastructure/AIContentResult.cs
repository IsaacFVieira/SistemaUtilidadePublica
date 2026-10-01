namespace SistemaUtilidadePublicaAPI.Infrastructure
{
    public class AIContentResult
    {
        public string Classification { get; set; } = string.Empty; 
        public string Seriousness { get; set; } = string.Empty; 
        public decimal Confidence { get; set; }
        public string Result { get; set; } = string.Empty;
        public int? SuggestedCategory { get; set; }
        public bool Approved { get; set; }

    }
}
