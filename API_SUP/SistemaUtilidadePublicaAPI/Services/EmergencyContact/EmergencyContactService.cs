using SistemaUtilidadePublicaAPI.Data.Repositories;
using SistemaUtilidadePublicaAPI.DTOs.EmergencyContact;
using SistemaUtilidadePublicaAPI.Models;
using SistemaUtilidadePublicaAPI.Common.Exceptions;
using SistemaUtilidadePublicaAPI.DTOs;

namespace SistemaUtilidadePublicaAPI.Services.EmergencyContact
{
    public class EmergencyContactService
    {
        private readonly EmergencyContactRepository _emergencyContactRepository;
        private readonly LocationRepository _locationRepository;

        public EmergencyContactService(
            EmergencyContactRepository emergencyContactRepository,
            LocationRepository locationRepository)
        {
            _emergencyContactRepository = emergencyContactRepository;
            _locationRepository = locationRepository;
        }

        public async Task<Models.EmergencyContact> AddEmergencyContactAsync(
            CreateEmergencyContactDto dto)
        {
            // 1. Criar a localização
            var location = new Models.Location
            {
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Endereco = dto.Address?.Trim(),
                Bairro = dto.Bairro?.Trim(),
                Municipio = dto.Municipio?.Trim(),
                Provincia = dto.Provincia?.Trim()
            };
            if(await _locationRepository.ExistsAsync(location.Latitude, location.Longitude)) {
                throw new ExceptionCommon("A localização com as coordenadas fornecidas já existe.");
            }
            // 2. Guardar localização e obter o seu ID
            var locationId = await _locationRepository.CreateAsync(location);

            // 3. Criar o contacto usando o ID da localização
            var emergencyContact = new Models.EmergencyContact
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                PhoneNumber = dto.PhoneNumber.Trim(),
                PhoneNumber2 = dto.PhoneNumber2?.Trim(),
                Address = dto.Address?.Trim(),
                Id_EmergencyContactType = dto.Id_EmergencyContactType,
                Id_Location = locationId,
               
                IsActive = true
            };

            // 4. Guardar contacto e obter o seu ID
            var id = await _emergencyContactRepository.CreateAsync(
                emergencyContact);

            emergencyContact.Id_EmergencyContact = id;

            return emergencyContact;
        }

        public async Task<List<EmergencyContactResponseDto>> GetAllEmergencyContactsAsync()
        {
          return await _emergencyContactRepository.GetAllAsync();
        }

        public async Task<List<EmergencyContactResponseDto>> GetNearestEmergencyContactsAsync(NearestEmergencyContactRequestDto dto)
        {
            return await _emergencyContactRepository.GetNearestByCategoryAsync(dto);
        }

        public async Task<List<EmergencyContactResponseDto>> SearchEmergencyContactsAsync(searchDto dto, NearestEmergencyContactRequestDto locationDto)
        {
            return await _emergencyContactRepository.GetSearchEmergencyContactsAsync(dto, locationDto);
        }


    }
}