namespace SistemaUtilidadePublicaAPI.DTOs.EmergencyContact
{
    public class LocationResponseDto
    {
        public int Id_Localizacao { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public string? Endereco { get; set; }
        public string? Bairro { get; set; }
        public string? Municipio { get; set; }
        public string? Provincia { get; set; }
    }
}
