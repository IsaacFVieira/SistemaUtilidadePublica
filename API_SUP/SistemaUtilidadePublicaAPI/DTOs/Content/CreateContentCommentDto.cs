namespace SistemaUtilidadePublicaAPI.DTOs.Content
{
    public class CreateContentCommentDto
    {
        public int Id_Content { get; set; }
        public int? Id_User { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}