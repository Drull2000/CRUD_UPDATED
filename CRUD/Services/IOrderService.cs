using CRUD.DTO;

namespace CRUD.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<OrderDto?> GetByIdAsync(Guid id);
        Task<OrderDto> CreateAsync(CreateOrderDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}