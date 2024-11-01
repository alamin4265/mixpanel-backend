using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProductManagement.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<SampleProduct> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetAllpageProductAsync(int limit, int skip);
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<Product>> SearchAsync(string? name, decimal? minPrice, decimal? maxPrice, int pageNumber, int pageSize);
        
    }

    public class ProductRepository : IProductRepository
    {
        private readonly ProductContext _context;
        List<SampleProduct> sampleProduct = new List<SampleProduct>
        {
            new SampleProduct
            {
               Id = 1,
               Brand = "Essence",
               Title = "Essence Mascara Lash Princess",
               Category = "beauty",
               Description = "The Essence Mascara Lash Princess is a popular mascara known for its volumizing and lengthening effects. Achieve dramatic lashes with this long-lasting and cruelty-free formula.",
               Price = 10,
               Images = new string[]{"https://cdn.dummyjson.com/products/images/beauty/Essence%20Mascara%20Lash%20Princess/1.png"},
               Stock = 5
            },
            new SampleProduct
            {
               Id = 2,
               Brand = "Glamour Beauty",
               Title = "Eyeshadow Palette with Mirror",
               Category = "beauty",
               Description = "The Essence Mascara Lash Princess is a popular mascara known for its volumizing and lengthening effects. Achieve dramatic lashes with this long-lasting and cruelty-free formula.",
               Price = 10,
               Images = new string[]{"https://cdn.dummyjson.com/products/images/beauty/Eyeshadow%20Palette%20with%20Mirror/1.png"},
               Stock = 5
            }
        };

        public ProductRepository(ProductContext context)
        {
            _context = context;
        }

        public IEnumerable<SampleProduct> GetAllProductsAsync()
        {
            return sampleProduct;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Product>> SearchAsync(string? name, decimal? minPrice, decimal? maxPrice, int pageNumber, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }

        public  async Task<IEnumerable<Product>> GetAllpageProductAsync(int limit, int skip)
        {
            var query = _context.Products.AsQueryable();
            if (limit > 0)
            {
                query = query.Skip(skip).Take(limit);
            }

            return await query.ToListAsync();
        }

     
    }
}
