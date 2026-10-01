using Microsoft.AspNetCore.Mvc;
using SistemaUtilidadePublicaAPI.DTOs.EmergencyContact;
using SistemaUtilidadePublicaAPI.Services.EmergencyContact;

namespace SistemaUtilidadePublicaAPI.Controllers.EmergencyContact
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmergencyContactController : ControllerBase
    {
        private readonly EmergencyContactService _emergencyContactService;

        public EmergencyContactController(EmergencyContactService emergencyContactService)
        {
            _emergencyContactService = emergencyContactService;
        }

        //Adicionar um novo contacto de emergência
        [HttpPost]
        public async Task<IActionResult> AddEmergencyContact([FromBody] CreateEmergencyContactDto dto)
        {
            var emergencyContact = await _emergencyContactService.AddEmergencyContactAsync(dto);
            return Created($"/api/EmergencyContact/{emergencyContact.Id_EmergencyContact}", emergencyContact);
        }
        //Obter todos os contactos de emergência
        [HttpGet]
        public async Task<IActionResult> GetAllEmergencyContacts()
        {
            var emergencyContacts = await _emergencyContactService.GetAllEmergencyContactsAsync();
            return Ok(emergencyContacts);
        }

        //Procurar os 5 contactos de emergência mais próximos com base na localização fornecida
        [HttpPost("nearest")]
        public async Task<IActionResult> GetNearestEmergencyContacts([FromBody] NearestEmergencyContactRequestDto dto)
        {
            var emergencyContacts = await _emergencyContactService.GetNearestEmergencyContactsAsync(dto);
            return Ok(emergencyContacts);
        }
        //Pesquisar e organizar por proximidade
        [HttpPost("search")]
        public async Task<IActionResult> SearchEmergencyContacts([FromBody] SearchEmergencyContactRequestDto dtos)
        {
            var emergencyContacts = await _emergencyContactService.SearchEmergencyContactsAsync(dtos.Search, dtos.Location);
            return Ok(emergencyContacts);
        }

    }
}
