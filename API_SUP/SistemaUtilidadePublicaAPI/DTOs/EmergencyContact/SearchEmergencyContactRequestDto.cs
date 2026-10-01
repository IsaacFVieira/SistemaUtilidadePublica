namespace SistemaUtilidadePublicaAPI.DTOs.EmergencyContact
{
    public class SearchEmergencyContactRequestDto
    {
        public searchDto Search { get; set; } = new();

        public NearestEmergencyContactRequestDto Location { get; set; } = new();
    }
}
