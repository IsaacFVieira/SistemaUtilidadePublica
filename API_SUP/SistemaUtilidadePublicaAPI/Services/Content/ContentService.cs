using SistemaUtilidadePublicaAPI.Common.Exceptions;
using SistemaUtilidadePublicaAPI.Data.Repositories;
using SistemaUtilidadePublicaAPI.DTOs;
using SistemaUtilidadePublicaAPI.DTOs.Content;
using SistemaUtilidadePublicaAPI.Models;
using SistemaUtilidadePublicaAPI.Services.Content;
using static SistemaUtilidadePublicaAPI.Data.Repositories.ContentRepository;

namespace SistemaUtilidadePublicaAPI.Services.Content
{
    public class ContentService
    {
        private readonly ContentRepository _contentRepository;
        //private readonly AIService _aiService;

        public ContentService(
            ContentRepository contentRepository
            /*AIService aiService*/)
        {
            _contentRepository = contentRepository;
            //_aiService = aiService;
        }
        public async Task<ContentResponseDto> AddContentAsync(CreateContentDto dto)
        {
            var content = await _contentRepository.CreateAsync(dto);
            return content;

        }

        public async Task<List<ContentResponseDto>> GetAllContentAsync()
        {
            var content = await _contentRepository.GetAllContentAsync();
            return content;

        }

        public async Task<ContentResponseDto> GetContentIdAsync(GetIdDto dto)
        {
            var content = await _contentRepository.GetContentAsync(dto);
            return content;

        }

        public async Task<List<ContentResponseDto>> SearchGetContentAsync(searchDto dto)
        {
            var content = await _contentRepository.SearchGetContentAsync(dto);
            return content;

        }
        public async Task<bool> DeleteAsync(GetIdDto dto)
        {
            return await _contentRepository.DeleteAsync(dto);

        }

        public async Task<bool> ActivateContentAsync(GetIdDto dto)
        {
            return await _contentRepository.ActivateAsync(dto);
        }
        public async Task<bool> DeactivateContentAsync(GetIdDto dto)
        {
            return await _contentRepository.DeactivateAsync(dto);
        }
        public async Task<ContentResponseDto> UpdateContentAsync(CreateContentDto dto, GetIdDto idDto)
        {
            return await _contentRepository.UpdateAsync(dto, idDto);
        }

        public async Task<List<ContentResponseDto>> GetContentByQuantityAsync(GetIdDto dto)
        {
            return await _contentRepository.GetByQuantityAsync(dto);
        }

        public async Task<bool> RegisterContentViewAsync(CreateContentViewDto dto)
        {
            return await _contentRepository.RegisterViewAsync(dto);
        }

        public async Task<bool> CreateCommentAsync(
            CreateContentCommentDto dto)
        {
            return await _contentRepository.CreateCommentAsync(dto);
        }

        public async Task<bool> UpdateCommentAsync(UpdateContentCommentDto dto)
        {
            return await _contentRepository.UpdateCommentAsync(dto);
        }

        public async Task<bool> DeleteCommentAsync(GetIdDto dto)
        {
            return await _contentRepository.DeleteCommentAsync(dto);
        }

        public async Task<bool> CreateReportAsync(CreateContentReportDto dto)
        {
            return await _contentRepository.CreateReportAsync(dto);
        }
    }
}