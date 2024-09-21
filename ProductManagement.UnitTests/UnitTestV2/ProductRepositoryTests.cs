using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ProductManagement.Data;
using ProductManagement.Models;
using ProductManagement.Repositories;
using ProductManagement.Services;
using System.Collections;
using System.ComponentModel;

namespace ProductManagement.UnitTests.UnitTestV2
{
    public class ProductRepositoryTests
    {
        private static readonly Product product1 = new Product { Id = 1, Name = "Laptop", Price = 100 };
        private static readonly Product product2 = new Product { Id = 2, Name = "Television", Price = 200 };
        private IProductRepository _mockProductRepository;
        private readonly ProductService _productService;

        public ProductRepositoryTests()
        {
            // _mockProductRepository = Substitute.For<IProductRepository>();

        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnProducts()
        {
            // Arrange
            var products = new List<Product> { product1, product2 }.AsQueryable();
            var mockSet = Substitute.For<Microsoft.EntityFrameworkCore.DbSet<Product>, IQueryable<Product>, IAsyncEnumerable<Product>>();
            var mockedDbSet = (DbSet<Product>)Substitute.For(new[] {
                    typeof(DbSet<Product>),
                    typeof(IAsyncEnumerable<Product>),
                    typeof(IEnumerable),
                    typeof(IEnumerable<Product>),
                    typeof(IInfrastructure<IServiceProvider>),
                    typeof(IListSource),
                    typeof(IQueryable<Product>)
                },
                new object[] { });

            //((IDbAsyncEnumerable<Product>)mockSet).GetAsyncEnumerator()
            // .Returns(new TestDbAsyncEnumerator<Product>(products.GetEnumerator()));

            ((IAsyncEnumerable<Product>)mockSet).GetAsyncEnumerator()
             .Returns(new TestAsyncEnumerator<Product>(products.GetEnumerator()));

            ((IQueryable<Product>)mockSet).Provider.Returns(new TestDbAsyncQueryProvider<Product>(products.Provider));

            ((IQueryable<Product>)mockSet).Expression.Returns(products.Expression);
            ((IQueryable<Product>)mockSet).ElementType.Returns(products.ElementType);
            ((IQueryable<Product>)mockSet).GetEnumerator().Returns(products.GetEnumerator());


            var mockContext = Substitute.For<ProductContext>();
            mockContext.Products.Returns(mockSet);

            _mockProductRepository = new ProductRepository(mockContext);

            // Act
            var result = await _mockProductRepository.GetAllProductsAsync();

            // Assert
            result.Should().BeEquivalentTo(products);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldAddProduct()
        {
            // Arrange
            var products = new List<Product> { product1, product2 }.AsQueryable();
            var product = new Product { Id = -3, Name = "Laptop", Price = 100 };
            var mockSet = Substitute.For<Microsoft.EntityFrameworkCore.DbSet<Product>, IQueryable<Product>, IAsyncEnumerable<Product>>();

            //((IDbAsyncEnumerable<Product>)mockSet).GetAsyncEnumerator()
            // .Returns(new TestDbAsyncEnumerator<Product>(products.GetEnumerator()));

            ((IAsyncEnumerable<Product>)mockSet).GetAsyncEnumerator()
             .Returns(new TestAsyncEnumerator<Product>(products.GetEnumerator()));

            ((IQueryable<Product>)mockSet).Provider.Returns(new TestDbAsyncQueryProvider<Product>(products.Provider));

            ((IQueryable<Product>)mockSet).Expression.Returns(products.Expression);
            ((IQueryable<Product>)mockSet).ElementType.Returns(products.ElementType);
            ((IQueryable<Product>)mockSet).GetEnumerator().Returns(products.GetEnumerator());


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
            var products = new List<Product> { product1, product2 }.AsQueryable();
            var product = new Product { Id = -3, Name = "Laptop", Price = 100 };
            var mockSet = Substitute.For<Microsoft.EntityFrameworkCore.DbSet<Product>, IQueryable<Product>, IAsyncEnumerable<Product>>();

            //((IDbAsyncEnumerable<Product>)mockSet).GetAsyncEnumerator()
            // .Returns(new TestDbAsyncEnumerator<Product>(products.GetEnumerator()));

            ((IAsyncEnumerable<Product>)mockSet).GetAsyncEnumerator()
             .Returns(new TestAsyncEnumerator<Product>(products.GetEnumerator()));

            ((IQueryable<Product>)mockSet).Provider.Returns(new TestDbAsyncQueryProvider<Product>(products.Provider));

            ((IQueryable<Product>)mockSet).Expression.Returns(products.Expression);
            ((IQueryable<Product>)mockSet).ElementType.Returns(products.ElementType);
            ((IQueryable<Product>)mockSet).GetEnumerator().Returns(products.GetEnumerator());


            var mockContext = Substitute.For<ProductContext>();
            mockContext.Products.Returns(mockSet);
            mockContext.SaveChangesAsync().ThrowsAsync(new InvalidOperationException("Database error"));

            _mockProductRepository = new ProductRepository(mockContext);

            // Act
            Func<Task> result = () => _mockProductRepository.CreateProductAsync(product);

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(result);
        }

    }
}
