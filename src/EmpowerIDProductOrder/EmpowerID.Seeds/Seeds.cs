using EmpowerID.Models;
using EmpowerID.Common.Extentions;
using EmpowerID.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
namespace EmpowerID.Seeds
{
    public class Seeds
    {
        private const string _categoryFile = @"SampleData\categories.json";
        private const string _productFile = @"SampleData\products.json";
        private const string _customerNameFile = @"SampleData\CustomerName.json";
        public Seeds()
        {

        }

        public async Task<List<Category>?> GenerateCategoriesAndProductsData()
        {
            if (File.Exists(_categoryFile) && File.Exists(_productFile))
            {
                var categories = await _categoryFile.ReadJsonFileAsync<List<Category>>();
                var products = await _productFile.ReadJsonFileAsync<List<Product>>();
                categories.ForEach(category =>
                {
                    category.Id = Utils.SequentialGuid();
                    category.Name = $"{category.Name} - {category.Id.Substring(category.Id.Length - 6, 6)}";
                    var randomProducts = Random.Shared.GetItems(products.ToArray(), Random.Shared.Next(5, 15)).ToList();
                    randomProducts.ForEach(rp =>
                    {
                        rp.Id = Utils.SequentialGuid();
                        rp.Name = $"{rp.Name} - {rp.Id.Substring(rp.Id.Length - 6, 6)}";
                        rp.ImageURL = @"https://picsum.photos/200"; // Random square image
                        rp.Description = Utils.GenerateRandomString();
                        rp.DateAddded = Utils.GetRandomDateTime();
                        rp.CategoryId = category.Id;
                    });
                    category.Products = randomProducts;
                });

                return categories;
            }
            return null;
        }

        public async Task<List<Order>?> GenerateOrderData(List<Product> products)
        {
            //var productsWithCategory = categories.SelectMany(c => c.Products);

            if (File.Exists(_customerNameFile))
            {
                var customerOrders = await _customerNameFile.ReadJsonFileAsync<List<Order>>();

                customerOrders.ForEach(customerOrder =>
                {
                    customerOrder.Id = Utils.SequentialGuid();
                    customerOrder.OrderDate = Utils.GetRandomDateTime();

                    var randomProductsWithCategory = Random.Shared.GetItems(products.ToArray(), Random.Shared.Next(2, 15)).ToList();
                    var productOrder = new List<ProductOrder>();
                    randomProductsWithCategory.ForEach(pwc =>
                    {
                        productOrder.Add(new ProductOrder
                        {
                            Id = Utils.SequentialGuid(),
                            OrderId = customerOrder.Id,
                            ProductId = pwc.Id
                        });
                    });
                    customerOrder.ProductOrders = productOrder;
                });
                return customerOrders;
            }
            return null;
        }
    }
}
