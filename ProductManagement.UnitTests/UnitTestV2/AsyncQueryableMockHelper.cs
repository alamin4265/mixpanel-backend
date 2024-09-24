using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ProductManagement.UnitTests.UnitTestV2
{
    internal static class AsyncQueryableMockHelper
    {
        public static DbSet<T> CreateMockDbSet<T>(this List<T> data) where T : class
        {
            // Convert the input list to IQueryable
            var queryableData = data.AsQueryable();

            // Create a mock DbSet implementing both IQueryable<T> and IAsyncEnumerable<T>
            var mockSet = Substitute.For<DbSet<T>, IQueryable<T>, IAsyncEnumerable<T>>();

            // Setup for IQueryable<T>
            ((IQueryable<T>)mockSet).Provider.Returns(queryableData.Provider);
            ((IQueryable<T>)mockSet).Expression.Returns(queryableData.Expression);
            ((IQueryable<T>)mockSet).ElementType.Returns(queryableData.ElementType);
            ((IQueryable<T>)mockSet).GetEnumerator().Returns(queryableData.GetEnumerator());

            // Setup for IAsyncEnumerable<T>
            ((IAsyncEnumerable<T>)mockSet).GetAsyncEnumerator(Arg.Any<CancellationToken>())
                .Returns(new TestAsyncEnumerator<T>(queryableData.GetEnumerator()));
            ((IQueryable<T>)mockSet).Provider.Returns(new TestAsyncQueryProvider<T>(queryableData.Provider));

            return mockSet;
        }
    }
}
