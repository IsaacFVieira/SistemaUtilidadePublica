using Microsoft.AspNetCore.Mvc;
using SistemaUtilidadePublicaAPI.DTOs;
using SistemaUtilidadePublicaAPI.DTOs.Content;
using SistemaUtilidadePublicaAPI.DTOs.EmergencyContact;
using SistemaUtilidadePublicaAPI.Services.Content;
using SistemaUtilidadePublicaAPI.Services.EmergencyContact;

namespace SistemaUtilidadePublicaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentController: ControllerBase
    {
    private readonly ContentService _ContentService;

    public ContentController(ContentService ContentService)
    {
        _ContentService = ContentService;
    }
    //Adicionar content
    [HttpPost]
    public async Task<IActionResult> AddContent([FromBody] CreateContentDto dto)
    {
        var content = await _ContentService.AddContentAsync(dto);
        return Created($"/api/Content/{content.Id_Content}", content);
    }
    //Obter todos os content
    [HttpGet]
    public async Task<IActionResult> GetAllContent()
    {
        var Contents = await _ContentService.GetAllContentAsync();
        return Ok(Contents);
    }

    //Obter content por id
    [HttpGet("GetById")]
    public async Task<IActionResult> GetByIdContent(GetIdDto dto)
    {
        var Contents = await _ContentService.GetContentIdAsync(dto);
        return Ok(Contents);
    }

    //Pesquisar content
    [HttpPost("Search")]
    public async Task<IActionResult> SearchGetContentAsync([FromBody] searchDto dto)
    {
        var contents = await _ContentService.SearchGetContentAsync(dto);
        return Ok(contents);
    }

    //Deletar content
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteContent(GetIdDto dto)
    {
        var deleted = await _ContentService.DeleteAsync(dto);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Conteúdo não encontrado."
            });
        }

        return Ok(new
        {
            success = true,
            message = "Conteúdo excluído com sucesso."
        });
    }

    //Ativar content
    [HttpPatch("Activate")]
    public async Task<IActionResult> ActivateContent(
    [FromBody] GetIdDto dto)
    {
        var activated = await _ContentService.ActivateContentAsync(dto);

        if (!activated)
        {
            return NotFound(new
            {
                message = "Conteúdo não encontrado."
            });
        }

        return Ok(new
        {
            success = true,
            message = "Conteúdo ativado com sucesso."
        });
    }

    //Desativar content
    [HttpPatch("Deactivate")]
    public async Task<IActionResult> DeactivateContent(
    [FromBody] GetIdDto dto)
    {
        var deactivated = await _ContentService.DeactivateContentAsync(dto);

        if (!deactivated)
        {
            return NotFound(new
            {
                success = false,
                message = "Conteúdo não encontrado."
            });
        }

        return Ok(new
        {
            success = true,
            message = "Conteúdo desativado com sucesso."
        });
    }

    [HttpPut("Update")]
    public async Task<IActionResult> UpdateContent([FromBody] UpdateContentDto dto)
    {
        var content = await _ContentService.UpdateContentAsync(dto.Content, dto.Id);

        return Ok(new
        {
            success = true,
            message = "Conteúdo atualizado com sucesso.",
            data = content
        });
    }

    [HttpPost("Quantity")]
    public async Task<IActionResult> GetContentByQuantity(
    [FromBody] GetIdDto dto)
    {
        var contents = await _ContentService.GetContentByQuantityAsync(dto);

        return Ok(contents);
    }

    [HttpPost("View")]
    public async Task<IActionResult> RegisterContentView(
    [FromBody] CreateContentViewDto dto)
    {
        var registered =
            await _ContentService.RegisterContentViewAsync(dto);

        if (!registered)
        {
            return BadRequest(new
            {
                success = false,
                message = "Não foi possível registrar a visualização."
            });
        }

        return Ok(new
        {
            success = true,
            message = "Visualização registrada com sucesso."
        });
    }

    

        [HttpPost("ContentComment")]
        public async Task<IActionResult> CreateComment(
            [FromBody] CreateContentCommentDto dto)
        {
            var created =
                await _ContentService.CreateCommentAsync(dto);

            if (!created)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Não foi possível adicionar o comentário."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Comentário adicionado com sucesso."
            });
        }
    [HttpPut("EditContentComment")]
    public async Task<IActionResult> UpdateComment(
    [FromBody] UpdateContentCommentDto dto)
    {
        var updated =
            await _ContentService.UpdateCommentAsync(dto);

        if (!updated)
        {
            return NotFound(new
            {
                success = false,
                message = "Comentário não encontrado ou inativo."
            });
        }

        return Ok(new
        {
            success = true,
            message = "Comentário atualizado com sucesso."
        });
    }

    [HttpDelete("DeleteContentComment")]
    public async Task<IActionResult> DeleteComment([FromBody]GetIdDto dto)
    {
        var deleted =
            await _ContentService.DeleteCommentAsync(dto);

        if (!deleted)
        {
            return NotFound(new
            {
                success = false,
                message = "Comentário não encontrado."
            });
        }

        return Ok(new
        {
            success = true,
            message = "Comentário eliminado com sucesso."
        });
    }

    [HttpPost("ContentReport")]
    public async Task<IActionResult> CreateReport(
    [FromBody] CreateContentReportDto dto)
    {
        var created =
            await _ContentService.CreateReportAsync(dto);

        if (!created)
        {
            return BadRequest(new
            {
                success = false,
                message = "Não foi possível registrar a denúncia."
            });
        }

        return Ok(new
        {
            success = true,
            message = "Denúncia registrada com sucesso."
        });
    }

}


