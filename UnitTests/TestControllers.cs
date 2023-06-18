
using AcmeCorp.Controllers;
using AcmeCorp.Core.Data;
using AcmeCorp.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;

namespace UnitTests
{
    [TestFixture]
    public class TestControllers
    {
        private Mock<AcmeCorpDbContext> _mockDbContext;
        private CustomersController _customersController;
        private OrdersController _ordersController;

        [SetUp]
        public void Init()
        { // Create a mock instance of YourDbContext
            _mockDbContext = new Mock<AcmeCorpDbContext>();

            // Initialize the controllers with the mock DbContext
            _customersController = new CustomersController(_mockDbContext.Object);
            _ordersController = new OrdersController(_mockDbContext.Object);

        }
        [Test]
        public async Task GetCustomers_ReturnsOkResult()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Customer 1" },
                new Customer { Id = 2, Name = "Customer 2" }
            };

            _mockDbContext.ReturnsDbSet(customers);

            // Act
            var result = await _customersController.GetCustomers();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public async Task GetOrder_ReturnsNotFound_WhenOrderNotFound()
        {
            // Arrange
            int orderId = 1;
            _mockDbContext.Setup(db => db.Orders.FindAsync(orderId)).ReturnsAsync((Order)null);

            // Act
            var result = await _ordersController.GetOrder(orderId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task CreateOrder_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var order = new Order { Id = 1, TotalAmount = 100 };

            // Act
            var result = await _ordersController.CreateOrder(order);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
        }


    }
    public static class MockExtension
    {
        public static void ReturnsDbSet<TEntity>(this Mock<AcmeCorpDbContext> mockDbContext, List<TEntity> data) where TEntity : class
        {
            var queryable = data.AsQueryable();

            var dbSetMock = new Mock<DbSet<TEntity>>();
            dbSetMock.As<IAsyncEnumerable<TEntity>>().Setup(m => m.GetAsyncEnumerator(default)).Returns(new TestAsyncEnumerator<TEntity>(data.GetEnumerator()));
            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<TEntity>(queryable.Provider));
            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            mockDbContext.Setup(db => db.Set<TEntity>()).Returns(dbSetMock.Object);
        }

    }
    public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return new TestAsyncEnumerable<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return _inner.Execute<TResult>(expression);
        }
    }


    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public TestAsyncEnumerator(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public T Current => _enumerator.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(Task.FromResult(_enumerator.MoveNext()));
        }

        public ValueTask DisposeAsync()
        {
            _enumerator.Dispose();
            return default;
        }
    }

    public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        {
        }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        {
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

}