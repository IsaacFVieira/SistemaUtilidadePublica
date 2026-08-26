namespace SistemaUtilidadePublicaAPI.DTOs.EmergencyContact
{
    public class EmergencyContactResponseDto
    {
        public int Id_EmergencyContact { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string? PhoneNumber2 { get; set; }
        public string? Address { get; set; }

        public int Id_EmergencyContactType { get; set; }

        public LocationResponseDto? Location { get; set; }

        public bool IsActive { get; set; }

        public Double? DistanceKm { get; set; }
    }
}
