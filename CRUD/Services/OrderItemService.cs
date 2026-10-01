using CRUD.DTO;
using CRUD.Models;

namespace CRUD.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IProductService _productService;

        public OrderItemService(IProductService productService)
        {
            _productService = productService;
        }

        public Task<IEnumerable<OrderItemDto>> GetAllAsync()
        {
            var items = InMemoryOrderStore.Orders
                .SelectMany(order => order.Items)
                .Select(ToDto);

            return Task.FromResult<IEnumerable<OrderItemDto>>(items);
        }

        public Task<OrderItemDto?> GetByIdAsync(Guid id)
        {
            var item = FindItem(id);
            return Task.FromResult(item == null ? null : ToDto(item.Value.Item));
        }

        public async Task<OrderItemDto> CreateAsync(CreateOrderItemDto dto)
        {
            ValidateQuantity(dto.Quantity);

            var order = InMemoryOrderStore.Orders.FirstOrDefault(o => o.Id == dto.OrderId);
            if (order == null)
            {
                throw new KeyNotFoundException($"Замовлення з ID {dto.OrderId} не знайдено.");
            }

            var product = await _productService.GetByIdAsync(dto.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Товар з ID {dto.ProductId} не знайдено.");
            }

            var model = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = dto.Quantity,
                UnitPrice = product.Price
            };

            var item = ToDto(model);
            order.Items.Add(item);
            RecalculateTotal(order);
            return item;
        }

        public Task<bool> UpdateAsync(Guid id, UpdateOrderItemDto dto)
        {
            ValidateQuantity(dto.Quantity);

            var result = FindItem(id);
            if (result == null)
            {
                return Task.FromResult(false);
            }

            result.Value.Item.Quantity = dto.Quantity;
            RecalculateTotal(result.Value.Order);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var result = FindItem(id);
            if (result == null)
            {
                return Task.FromResult(false);
            }

            result.Value.Order.Items.Remove(result.Value.Item);
            RecalculateTotal(result.Value.Order);
            return Task.FromResult(true);
        }

        private static (OrderDto Order, OrderItemDto Item)? FindItem(Guid id)
        {
            foreach (var order in InMemoryOrderStore.Orders)
            {
                var item = order.Items.FirstOrDefault(item => item.Id == id);
                if (item != null)
                {
                    return (order, item);
                }
            }

            return null;
        }

        private static OrderItemDto ToDto(OrderItem item)
        {
            return new OrderItemDto
            {
                Id = item.Id,
                OrderId = item.OrderId,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            };
        }

        private static OrderItemDto ToDto(OrderItemDto item)
        {
            return new OrderItemDto
            {
                Id = item.Id,
                OrderId = item.OrderId,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            };
        }

        private static void RecalculateTotal(OrderDto order)
        {
            order.TotalAmount = order.Items.Sum(item => item.TotalPrice);
        }

        private static void ValidateQuantity(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Количество должно быть больше нуля.");
            }
        }
    }
}
