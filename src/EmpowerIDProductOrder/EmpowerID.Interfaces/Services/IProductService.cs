using EmpowerID.Models;

namespace EmpowerID.Interfaces.Services
{
    public interface IProductService
    {
        Task<IList<Product>> GetProductList(CancellationToken cancellationToken);
        Task SyncCDCProductList(CancellationToken cancellationToken);
        Task SaveProductList(List<Product> products, CancellationToken cancellationToken);
    }
}
