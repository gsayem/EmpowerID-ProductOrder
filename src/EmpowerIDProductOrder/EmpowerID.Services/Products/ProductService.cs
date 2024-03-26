
using EmpowerID.Interfaces.Repository;
using EmpowerID.Interfaces.Services.Products;
using EmpowerID.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EmpowerID.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepositoryAsync<Category> _categoryRepositoryAsync;
        private readonly IRepositoryAsync<Order> _orderRepositoryAsync;
        public ProductService( IRepositoryAsync<Category> categoryRepositoryAsync, IUnitOfWork unitOfWork, IRepositoryAsync<Order> orderRepositoryAsync)
        {
            _categoryRepositoryAsync = categoryRepositoryAsync;
            _unitOfWork = unitOfWork;
            _orderRepositoryAsync = orderRepositoryAsync;

        }
        public async Task<IList<Product>> GetProductList(CancellationToken cancellationToken)
        {
            var products = new List<Product>();
            products.Add(new Product
            {
                CategoryId = 1.ToString(),
                DateAddded = DateTime.Now,
                Description = "Test",
                Id = 1.ToString(),
                ImageURL = null,
                Name = "Test",
                Price = 1,
            });
            return await Task.FromResult(products);
        }

        public async Task SaveCategoriesProductList(List<Category> categories, CancellationToken cancellationToken)
        {
            await _categoryRepositoryAsync.AddRangeAsync(cancellationToken, [.. categories]);
        }

        public async Task SaveOrderList(List<Order> orders, CancellationToken cancellationToken)
        {
            await _orderRepositoryAsync.AddRangeAsync(cancellationToken, [.. orders]);
        }
    }
}
