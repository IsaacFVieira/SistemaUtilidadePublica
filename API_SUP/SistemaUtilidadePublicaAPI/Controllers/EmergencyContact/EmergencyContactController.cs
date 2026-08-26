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

        [HttpPost]
        public async Task<IActionResult> AddEmergencyContact([FromBody] CreateEmergencyContactDto dto)
        {
            var emergencyContact = await _emergencyContactService.AddEmergencyContactAsync(dto);
            return Created($"/api/EmergencyContact/{emergencyContact.Id_EmergencyContact}", emergencyContact);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmergencyContacts()
        {
            var emergencyContacts = await _emergencyContactService.GetAllEmergencyContactsAsync();
            return Ok(emergencyContacts);
        }

        [HttpPost("nearest")]
        public async Task<IActionResult> GetNearestEmergencyContacts([FromBody] NearestEmergencyContactRequestDto dto)
        {
            var emergencyContacts = await _emergencyContactService.GetNearestEmergencyContactsAsync(dto);
            return Ok(emergencyContacts);
        }
    }
}
