namespace SistemaUtilidadePublicaAPI.Models
{
    public class Content
    {
        public int Id_Content { get; set; }
        //public int ID_Creator { get; set; }
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? ContentText { get; set; }

        public int Id_ContentType { get; set; }

        public int? Id_Category { get; set; }

        public string? ImageUrl { get; set; }

        public string? VideoUrl { get; set; }

        public string? AudioUrl { get; set; }

        public int? Id_Creator { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public bool IsActive { get; set; }
    }
}
