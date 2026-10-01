using CRUD.DTO;

namespace CRUD.Services
{
    internal static class InMemoryOrderStore
    {
        public static List<OrderDto> Orders { get; } = new();
    }
}
