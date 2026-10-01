using CRUD.DTO;

namespace CRUD.Services
{
    public class ProductService : IProductService
    {
        private static readonly List<ProductDto> _products = new();

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            return await Task.FromResult(_products);
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return await Task.FromResult(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = new ProductDto
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Price = dto.Price
            };

            _products.Add(product);
            return await Task.FromResult(product);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateProductDto dto)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return false;

            product.Name = dto.Name;
            product.Price = dto.Price;

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return false;

            _products.Remove(product);
            return await Task.FromResult(true);
        }
    }
}