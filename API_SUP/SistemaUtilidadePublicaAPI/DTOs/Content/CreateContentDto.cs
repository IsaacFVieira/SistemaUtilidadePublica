using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SistemaUtilidadePublicaAPI.DTOs.Content
{
    public class CreateContentDto
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? ContentText { get; set; }

        public string ContentType { get; set; } = string.Empty;

        public string? Category { get; set; }

        public string? ImageUrl { get; set; }

        public string? VideoUrl { get; set; }

        public string? AudioUrl { get; set; }

        public int? CreatorId { get; set; }
    }
}

