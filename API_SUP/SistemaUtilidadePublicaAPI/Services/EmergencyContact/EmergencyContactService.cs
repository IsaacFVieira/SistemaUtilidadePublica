using SistemaUtilidadePublicaAPI.Data.Repositories;
using SistemaUtilidadePublicaAPI.DTOs.EmergencyContact;
using SistemaUtilidadePublicaAPI.Models;

namespace SistemaUtilidadePublicaAPI.Services.EmergencyContact
{
    public class EmergencyContactService
    {
        private readonly EmergencyContactRepository _emergencyContactRepository;

        public EmergencyContactService(EmergencyContactRepository emergencyContactRepository)
        {
            _emergencyContactRepository = emergencyContactRepository;
        }

        public async Task<Models.EmergencyContact> AddEmergencyContactAsync(
            CreateEmergencyContactDto dto)
        {
            var emergencyContact = new Models.EmergencyContact
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                PhoneNumber = dto.PhoneNumber.Trim(),
                PhoneNumber2 = dto.PhoneNumber2?.Trim(),
                Address = dto.Address?.Trim(),
                Id_EmergencyContactType = dto.Id_EmergencyContactType,
                Id_Location = dto.Id_Location,
                IsActive = true
            };

            var id = await _emergencyContactRepository.CreateAsync(
                emergencyContact);

            emergencyContact.Id_EmergencyContact = id;

            return emergencyContact;
        }
    }
}