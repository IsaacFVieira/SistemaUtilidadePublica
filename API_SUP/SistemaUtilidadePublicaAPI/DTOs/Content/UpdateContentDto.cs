namespace SistemaUtilidadePublicaAPI.DTOs.Content
{
    public class UpdateContentDto
    {
        public GetIdDto Id { get; set; } = new();

        public CreateContentDto Content { get; set; } = new();
    }
}
