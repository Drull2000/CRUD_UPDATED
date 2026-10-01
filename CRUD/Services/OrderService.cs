using CRUD.DTO;

namespace CRUD.Services
{
    public class OrderService : IOrderService
    {
        // Тимчасове збереження в пам'яті (замість бази даних)
        private readonly IProductService _productService;

        public OrderService(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            return await Task.FromResult(InMemoryOrderStore.Orders);
        }

        public async Task<OrderDto?> GetByIdAsync(Guid id)
        {
            var order = InMemoryOrderStore.Orders.FirstOrDefault(o => o.Id == id);
            return await Task.FromResult(order);
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
        {
            var orderItems = new List<OrderItemDto>();
            decimal totalAmount = 0;
            var orderId = Guid.NewGuid();

            foreach (var itemDto in dto.Items)
            {
                var product = await _productService.GetByIdAsync(itemDto.ProductId);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Товар з ID {itemDto.ProductId} не знайдено.");
                }

                var item = new OrderItemDto
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price
                };

                orderItems.Add(item);
                totalAmount += item.TotalPrice;
            }

            var order = new OrderDto
            {
                Id = orderId,
                CreatedAt = DateTime.UtcNow,
                Items = orderItems,
                TotalAmount = totalAmount
            };

            InMemoryOrderStore.Orders.Add(order);
            return order;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var order = InMemoryOrderStore.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return false;

            InMemoryOrderStore.Orders.Remove(order);
            return await Task.FromResult(true);
        }
    }

    
}