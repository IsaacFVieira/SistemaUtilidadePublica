namespace SistemaUtilidadePublicaAPI.DTOs.Content
{
    public class ContentResponseDto
    {
        public int Id_Content { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ContentText { get; set; }

        public int Id_ContentType { get; set; }
        public string ContentType { get; set; } = string.Empty;

        public int? Id_Category { get; set; }
        public string? Category { get; set; }

        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? AudioUrl { get; set; }

        public int? Id_Creator { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool Active { get; set; }
    }
}
