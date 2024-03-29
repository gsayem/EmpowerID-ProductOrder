using EmpowerID.Common.Enums;
using EmpowerID.Interfaces.Repository;
using EmpowerID.Interfaces.Services;
using EmpowerID.Models;
using EmpowerID.Models.CDC;
using Microsoft.Extensions.Logging;

namespace EmpowerID.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepositoryAsync<Order> _orderRepositoryAsync;
        private readonly IRepositoryAsync<CDC_Order> _cdc_OrderRepositoryAsync;
        public OrderService(ILogger<OrderService> logger, IUnitOfWork unitOfWork, IRepositoryAsync<Order> orderRepositoryAsync, IRepositoryAsync<CDC_Order> cdc_OrderRepositoryAsync)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _orderRepositoryAsync = orderRepositoryAsync;
            _cdc_OrderRepositoryAsync = cdc_OrderRepositoryAsync;
        }
        public async Task SyncCDCOrdersAsync(CancellationToken cancellationToken)
        {
            //Get the CDC Product From Primary/Source Database
            var cdcOrders = await _cdc_OrderRepositoryAsync.GetAllFromRawSqlAsync("exec sp_get_CDC_Data_For_Orders", cancellationToken);

            _unitOfWork.ChangeDatabase(Common.Enums.DatabaseConnection.Secondary);
            var uowOrderRepository = _unitOfWork.GetRepositoryAsync<Order>();
            //Change the connection to Secondary/Destination Database
            uowOrderRepository.ChangeDatabase(Common.Enums.DatabaseConnection.Secondary);
            _logger.LogInformation($"Total CDC Orders found : {cdcOrders.Count}");

            if (cdcOrders.Any())
            {
                //Deleted Orders
                var deletedOrderIds = cdcOrders.Where(p => p.DataStatus == DataStatus.Deleted).Select(s => s.Id);
                var deletedOrders = await uowOrderRepository.FindAllAsync(s => deletedOrderIds.Contains(s.Id), cancellationToken);

                if (deletedOrders != null && deletedOrders.Any())
                {
                    _logger.LogInformation($"Total CDC deleted orders found : {deletedOrders.Count}");
                    await uowOrderRepository.RemoveRangeAsync(cancellationToken, [.. deletedOrders]);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                //Inserted Orders
                var insertedOrdersIds = cdcOrders.Where(p => p.DataStatus == DataStatus.Inserted).Select(s => s.Id);
                var insertedOrders = await uowOrderRepository.FindAllAsync(s => insertedOrdersIds.Contains(s.Id), cancellationToken);

                if (insertedOrders != null && insertedOrders.Any())
                {
                    _logger.LogInformation($"Total CDC inserted orders found : {insertedOrders.Count}");

                    await uowOrderRepository.AddRangeAsync(cancellationToken, [.. insertedOrders]);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                //Updated Orders
                var cdcUpdatedOrders = cdcOrders.Where(p => p.DataStatus == DataStatus.Updated);
                var updatedOrderIds = cdcUpdatedOrders.Select(s => s.Id);
                var updatedOrders = await uowOrderRepository.FindAllAsync(s => updatedOrderIds.Contains(s.Id), cancellationToken);
                if (updatedOrders != null && updatedOrders.Any())
                {
                    _logger.LogInformation($"Total CDC updated orders found : {updatedOrders.Count}");
                    updatedOrders.ToList().ForEach(o =>
                    {
                        var uo = cdcUpdatedOrders.First(s => s.Id == o.Id);
                        o.OrderDate = uo.OrderDate;
                        o.CustomerName = uo.CustomerName;
                    });
                    await uowOrderRepository.UpdateRangeAsync(cancellationToken, [.. updatedOrders]);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                //Cleanup CDC Table
                _logger.LogInformation($"Cleanup CDC Orders in source database");
                _unitOfWork.ChangeDatabase(Common.Enums.DatabaseConnection.Primary);
                _cdc_OrderRepositoryAsync.ChangeDatabase(Common.Enums.DatabaseConnection.Primary);
                await _cdc_OrderRepositoryAsync.GetAllFromRawSqlAsync("exec sp_CDC_Orders_Table_Cleanup", cancellationToken);
            }
        }

        public async Task<IList<Order>> GetOrdersAsync(CancellationToken cancellationToken)
        {
            var orders = await _orderRepositoryAsync.GetAllAsync(cancellationToken);
            return orders;
        }

        public async Task SaveOrdersAsync(IList<Order> orders, CancellationToken cancellationToken)
        {
            await _orderRepositoryAsync.AddRangeAsync(cancellationToken, [.. orders]);
        }
    }
}
