using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ProductManagement.Data;
using ProductManagement.Models;
using ProductManagement.Repositories;

namespace ProductManagement.UnitTests.UnitTestV2
{
    public class ProductRepositoryTests
    {
        private static readonly Product product1 = new Product { Id = 1, Name = "Laptop", Price = 100 };
        private static readonly Product product2 = new Product { Id = 2, Name = "Television", Price = 200 };
        private IProductRepository _mockProductRepository;

        public ProductRepositoryTests()
        {
            // _mockProductRepository = Substitute.For<IProductRepository>();

        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnProducts()
        {
            // Arrange
            var products = new List<Product> { product1, product2 };
            var mockSet = products.CreateMockDbSet();
            var mockContext = Substitute.For<ProductContext>();
            mockContext.Products.Returns(mockSet);
            _mockProductRepository = new ProductRepository(mockContext);


            // Act
            var result = await _mockProductRepository.GetAllProductsAsync();


            // Assert
            result.Should().BeEquivalentTo(products);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldGetProductByIdAsync()
        {
            // Arrange
            var products = new List<Product> { product1, product2 };
            var mockSet = products.CreateMockDbSet();
            mockSet.FindAsync(Arg.Any<object[]>()).Returns(call =>
            {
                var id = (int)call.Arg<object[]>()[0];
                return new ValueTask<Product?>(products.FirstOrDefault(p => p.Id == id) ?? null);
            });
            var mockContext = Substitute.For<ProductContext>();
            mockContext.Products.Returns(mockSet);
            _mockProductRepository = new ProductRepository(mockContext);


            // Act
            var result = await _mockProductRepository.GetProductByIdAsync(1);


            //Assert
            result.Should().BeEquivalentTo(products.FirstOrDefault(x => x.Id == 1));
            await mockContext.Products.Received().FindAsync(Arg.Any<int>());
            await mockContext.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task CreateProductAsync_ShouldAddProduct()
        {
            // Arrange

            var products = new List<Product> { product1, product2 };
            var product = new Product { Id = 3, Name = "Laptop", Price = 100 };
            var mockSet = products.CreateMockDbSet();
            var mockContext = Substitute.For<ProductContext>();
            mockContext.Products.Returns(mockSet);
            _mockProductRepository = new ProductRepository(mockContext);


            // Act
            await _mockProductRepository.CreateProductAsync(product);


            //Assert
            mockContext.Products.Received().Add(Arg.Any<Product>());
            await mockContext.Received().SaveChangesAsync();
        }

        [Fact]
        public async Task CreateProductAsync_ShouldThrowException_WhenSaveChangesFails()
        {
            // Arrange
            var products = new List<Product> { product1, product2 };
            var product = new Product { Id = -3, Name = "Laptop", Price = 100 };
            var mockSet = products.CreateMockDbSet();
            var mockContext = Substitute.For<ProductContext>();
            mockContext.Products.Returns(mockSet);
            mockContext.SaveChangesAsync().ThrowsAsync(new InvalidOperationException("Database error"));
            _mockProductRepository = new ProductRepository(mockContext);


            // Act
            Func<Task> result = () => _mockProductRepository.CreateProductAsync(product);


            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(result);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldUpdateProduct()
        {
            // Arrange
            var products = new List<Product> { product1, product2 };

            var mockSet = products.CreateMockDbSet();
            var mockContext = Substitute.For<ProductContext>();
            mockContext.Products.Returns(mockSet);
            _mockProductRepository = new ProductRepository(mockContext);


            // Act
            await _mockProductRepository.UpdateProductAsync(products.First());


            //Assert
            mockContext.Products.Received().Update(Arg.Any<Product>());
            await mockContext.Received().SaveChangesAsync();
        }

        [Fact]
        public async Task CreateProductAsync_ShouldDeleteProduct()
        {
            // Arrange
            var products = new List<Product> { product1, product2 };
            var mockSet = products.CreateMockDbSet();
            mockSet.FindAsync(Arg.Any<object[]>()).Returns(call =>
            {
                var id = (int)call.Arg<object[]>()[0];
                return new ValueTask<Product?>(products.FirstOrDefault(p => p.Id == id) ?? null);
            });
            var mockContext = Substitute.For<ProductContext>();
            mockContext.Products.Returns(mockSet);
            _mockProductRepository = new ProductRepository(mockContext);


            // Act
            await _mockProductRepository.DeleteProductAsync(1);


            //Assert
            mockContext.Products.Received().Remove(Arg.Any<Product>());
            await mockContext.Received().SaveChangesAsync();
        }

        [Fact]
        public async Task CreateProductAsync_ShouldSearchAsync()
        {
            // Arrange
            Product product3 = new Product { Id = 3, Name = "Laptop", Price = 200 };
            Product product4 = new Product { Id = 3, Name = "Laptop", Price = 150 };
            var products = new List<Product> { product1, product2, product3, product4 };

            var mockSet = products.CreateMockDbSet();
            var mockContext = Substitute.For<ProductContext>();
            mockContext.Products.Returns(mockSet);
            _mockProductRepository = new ProductRepository(mockContext);


            // Act
            var result = await _mockProductRepository.SearchAsync("Laptop", 100, 150, 2, 1);

            
            //Assert
            result.Should().NotBeNull();
            await mockContext.Products.Received().FindAsync(Arg.Any<int>());
            await mockContext.DidNotReceive().SaveChangesAsync();
        }
    }
}
