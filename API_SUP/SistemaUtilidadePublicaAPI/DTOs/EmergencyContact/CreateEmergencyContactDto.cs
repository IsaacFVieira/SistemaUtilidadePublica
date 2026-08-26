using System.ComponentModel.DataAnnotations;

namespace SistemaUtilidadePublicaAPI.DTOs.EmergencyContact
{
    public class CreateEmergencyContactDto
    {
        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Phone]
        [StringLength(30, MinimumLength = 2)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Phone]
        [StringLength(30, MinimumLength = 2)]
        public string? PhoneNumber2 { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [Required]
        public int Id_EmergencyContactType { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }

        [Required]
        public string Bairro { get; set; } = string.Empty;

        [Required]
        public string Municipio { get; set; } = string.Empty;

        [Required]
        public string Provincia { get; set; } = string.Empty;


    }
}