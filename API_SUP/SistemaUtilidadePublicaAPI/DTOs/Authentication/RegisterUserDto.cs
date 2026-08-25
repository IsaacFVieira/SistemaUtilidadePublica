using System.ComponentModel.DataAnnotations;

namespace SistemaUtilidadePublicaAPI.DTOs.Authentication
{
    public class RegisterUserDto
    {
        [Required]
        [StringLength(100, MinimumLength =2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string? telephone { get; set; }


        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength (100, MinimumLength =8)]
        public string Password { get; set; } = string.Empty;
    }
}
