namespace SistemaUtilidadePublicaAPI.DTOs.Content
{
    public class CreateContentReportDto
    {
        public int Id_Content { get; set; }
        public int Id_User { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}