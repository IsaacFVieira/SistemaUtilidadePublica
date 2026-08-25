namespace SistemaUtilidadePublicaAPI.DTOs.Authentication
{
    public class UserResponseDto
    {
        public int Id_User { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Telephone { get; set; }
        public string Email { get; set; }= string.Empty;
        public string? ImagemPerfil { get; set; }
        public DateTime Create_Data { get; set; }


    }
}
