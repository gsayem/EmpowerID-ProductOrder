using EmpowerID.Models;

namespace EmpowerID.Interfaces.Services.Products
{
    public interface IProductService
    {
        Task<IList<Product>> GetProductList(CancellationToken cancellationToken);
        Task<IList<Product>> GetCDCProductList(CancellationToken cancellationToken);
        Task SaveCategoriesProductList(List<Category> categories, CancellationToken cancellationToken);
        Task SaveOrderList(List<Order> orders, CancellationToken cancellationToken);
    }
}
