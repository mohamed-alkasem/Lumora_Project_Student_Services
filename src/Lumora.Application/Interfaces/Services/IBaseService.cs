namespace Lumora.Application.Interfaces.Services;

public interface IBaseService<TDto, TCreateDto, TUpdateDto>
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    Task<TDto?> GetByIdAsync(int id);
    Task<IEnumerable<TDto>> GetAllAsync();
    Task<TDto> CreateAsync(TCreateDto createDto);
    Task<TDto?> UpdateAsync(int id, TUpdateDto updateDto);
    Task<bool> DeleteAsync(int id);
}
