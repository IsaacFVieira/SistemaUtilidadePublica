namespace SistemaUtilidadePublicaAPI.DTOs.Content
{
    public class UpdateContentCommentDto
    {
        public long Id_Comentario { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}