using EmpowerID.Common.Enums;
using EmpowerID.Interfaces.Repository;
using EmpowerID.Interfaces.Services;
using EmpowerID.Models;
using EmpowerID.Models.CDC;
using Microsoft.Extensions.Logging;

namespace EmpowerID.Services
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IRepositoryAsync<CDC_Product> _cdc_ProductRepositoryAsync;
        public ProductService(ILogger<ProductService> logger, IUnitOfWork unitOfWork, IRepositoryAsync<Product> productRepositoryAsync, IRepositoryAsync<CDC_Product> cdc_ProductRepositoryAsync)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _productRepositoryAsync = productRepositoryAsync;
            _cdc_ProductRepositoryAsync = cdc_ProductRepositoryAsync;

        }

        public async Task SyncCDCProductList(CancellationToken cancellationToken)
        {
            //Get the CDC Product From Primary/Source Database
            var cdcProducts = await _cdc_ProductRepositoryAsync.GetAllFromRawSqlAsync("exec sp_get_CDC_Data_For_Products", cancellationToken);

            _unitOfWork.ChangeDatabase(Common.Enums.DatabaseConnection.Secondary);
            var uowProductRepository = _unitOfWork.GetRepositoryAsync<Product>();
            //Change the connection to Secondary/Destination Database
            uowProductRepository.ChangeDatabase(Common.Enums.DatabaseConnection.Secondary);
            _logger.LogInformation($"Total CDC Product found : {cdcProducts.Count}");

            if (cdcProducts.Any())
            {
                //Deleted Products
                var deletedProductIds = cdcProducts.Where(p => p.DataStatus == DataStatus.Deleted).Select(s => s.Id);
                var deletedProducts = await uowProductRepository.FindAllAsync(s => deletedProductIds.Contains(s.Id), cancellationToken);

                if (deletedProducts != null && deletedProducts.Any())
                {
                    _logger.LogInformation($"Total CDC deleted product found : {deletedProducts.Count}");
                    await uowProductRepository.RemoveRangeAsync(cancellationToken, [.. deletedProducts]);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                //Inserted Products
                var insertedProductIds = cdcProducts.Where(p => p.DataStatus == DataStatus.Inserted).Select(s => s.Id);
                var insertedProducts = await uowProductRepository.FindAllAsync(s => insertedProductIds.Contains(s.Id), cancellationToken);

                if (insertedProducts != null && insertedProducts.Any())
                {
                    _logger.LogInformation($"Total CDC inserted product found : {insertedProducts.Count}");

                    await uowProductRepository.AddRangeAsync(cancellationToken, [.. insertedProducts]);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                }

                //Updated Products
                var cdcUpdatedProducts = cdcProducts.Where(p => p.DataStatus == DataStatus.Updated);
                var updatedProductIds = cdcUpdatedProducts.Select(s => s.Id);
                var updatedProducts = await uowProductRepository.FindAllAsync(s => updatedProductIds.Contains(s.Id), cancellationToken);

                if (updatedProducts != null && updatedProducts.Any())
                {
                    _logger.LogInformation($"Total CDC updated product found : {updatedProducts.Count}");
                    updatedProducts.ToList().ForEach(p =>
                    {
                        var up = cdcUpdatedProducts.First(u => u.Id == p.Id);
                        p.Price = up.Price;
                        p.CategoryId = up.CategoryId;
                        p.DateAddded = up.DateAddded;
                        p.Description = up.Description;
                        p.ImageURL = up.ImageURL;
                        p.Name = up.Name;
                    });
                    await uowProductRepository.UpdateRangeAsync(cancellationToken, [.. updatedProducts]);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                //Cleanup CDC Table
                _logger.LogInformation($"Cleanup CDC Product in source database");
                _unitOfWork.ChangeDatabase(Common.Enums.DatabaseConnection.Primary);
                _cdc_ProductRepositoryAsync.ChangeDatabase(Common.Enums.DatabaseConnection.Primary);
                await _cdc_ProductRepositoryAsync.GetAllFromRawSqlAsync("exec sp_CDC_Products_Table_Cleanup", cancellationToken);
            }

        }

        public async Task<IList<Product>> GetProductList(CancellationToken cancellationToken)
        {
            var products = await _productRepositoryAsync.GetAllAsync(cancellationToken);
            return products;
        }

        public async Task SaveProductList(List<Product> products, CancellationToken cancellationToken)
        {
            await _productRepositoryAsync.AddRangeAsync(cancellationToken, [.. products]);
        }

    }
}
