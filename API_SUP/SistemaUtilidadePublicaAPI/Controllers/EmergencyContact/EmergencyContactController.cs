using Microsoft.AspNetCore.Mvc;
using SistemaUtilidadePublicaAPI.DTOs.EmergencyContact;
using SistemaUtilidadePublicaAPI.Services.EmergencyContact;

namespace SistemaUtilidadePublicaAPI.Controllers.EmergencyContact
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmergencyContactController : ControllerBase
    {
        private readonly EmergencyContactService _emergencyContactService;s

        public EmergencyContactController(EmergencyContactService emergencyContactService)
        {
            _emergencyContactService = emergencyContactService;
        }

        [HttpPost("Addemergencycontacts")]
        public async Task<IActionResult> AddEmergencyContact([FromBody] CreateEmergencyContactDto dto)
        {
            var emergencyContact = await _emergencyContactService.AddEmergencyContactAsync(dto);
            return Created($"/api/emergencycontacts/{emergencyContact.Id_EmergencyContact}", emergencyContact);
        }
    }
}
