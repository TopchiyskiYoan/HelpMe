using HelpMe.Application.DTOs;

namespace HelpMe.Application.Interfaces;

public interface IRegionService
{
    Task<List<RegionDto>> GetAllAsync();
    Task<RegionDto?> GetByIdAsync(int id);
}
