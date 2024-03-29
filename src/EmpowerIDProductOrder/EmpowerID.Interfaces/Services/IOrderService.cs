using EmpowerID.Models;

namespace EmpowerID.Interfaces.Services
{
    public interface IOrderService
    {
        Task<IList<Order>> GetOrdersAsync(CancellationToken cancellationToken);
        Task SaveOrdersAsync(IList<Order> orders, CancellationToken cancellationToken);
        Task SyncCDCOrdersAsync(CancellationToken cancellationToken);
    }
}
