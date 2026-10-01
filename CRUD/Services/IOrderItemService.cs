using CRUD.DTO;

namespace CRUD.Services
{
    public interface IOrderItemService
    {
        Task<IEnumerable<OrderItemDto>> GetAllAsync();
        Task<OrderItemDto?> GetByIdAsync(Guid id);
        Task<OrderItemDto> CreateAsync(CreateOrderItemDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateOrderItemDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
