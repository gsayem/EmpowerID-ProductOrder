using EmpowerID.Infrastructure.Configuration;
using EmpowerID.Interfaces.Services.Products;
using EmpowerID.Seeds;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ProductOrderApp
{
    public class Application
    {
        private readonly ILogger<Application> _logger;
        private readonly IProductService _productService;

        private readonly IOptions<ConfigAppSettings> _configuration;
        public Application(ILogger<Application> logger, IProductService productService, IOptions<ConfigAppSettings> configuration)
        {
            _logger = logger;
            _productService = productService;
            _configuration = configuration;
        }

        public async Task MyLogic(CancellationToken cancellationToken = default)
        {
            var seeds = new Seeds();
            var categoriesAndProducts = await seeds.GenerateCategoriesAndProductsData();
            await _productService.SaveCategoriesProductList(categoriesAndProducts, cancellationToken);

            var products = categoriesAndProducts.SelectMany(d => d.Products).ToList();
            var orders = await seeds.GenerateOrderData(products);
            await _productService.SaveOrderList(orders, cancellationToken);

            _logger.LogInformation("Hello, World!");
            var p = await _productService.GetProductList(cancellationToken);
            _logger.LogInformation(products[0].Name);
            _logger.LogInformation(_configuration.Value.ConnString);
        }
    }
}
