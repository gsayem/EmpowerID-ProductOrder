using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.DataFactory;
using Azure.ResourceManager.DataFactory.Models;
using Azure.ResourceManager.Resources;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using EmpowerID.DomainModel;
using EmpowerID.Infrastructure.Configuration;
using EmpowerID.Interfaces.Services.Products;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ProductOrderApp
{
    public class Application
    {
        private readonly ILogger<Application> _logger;
        private readonly IProductService _productService;

        private readonly ConfigAppSettings _configuration;
        public Application(ILogger<Application> logger, IProductService productService, IOptions<ConfigAppSettings> configuration)
        {
            _logger = logger;
            _productService = productService;
            _configuration = configuration.Value;
        }

        public async Task StartApplication(CancellationToken cancellationToken = default)
        {
            await OpenApplication(cancellationToken);
        }

        private async Task OpenApplication(CancellationToken cancellationToken)
        {
            Console.WriteLine("ETL Console Application");
            Console.WriteLine("1. Initiate and monitor ETL pipelines");
            Console.WriteLine("2. Search the data using Azure Cognitive Search");
            Console.WriteLine("3. CDC");
            Console.WriteLine("Select an option:");
            switch (Console.ReadLine())
            {
                case "1":
                    await MonitorDataFactoryPipeLine();
                    break;
                case "2":
                    await CreateSearchClientForQueries();
                    break;
                case "3":
                    await _productService.GetCDCProductList(cancellationToken);
                    break;
                default:
                    return;
            }

            await RestartApplication(cancellationToken);
        }

        private async Task RestartApplication(CancellationToken cancellationToken)
        {
            Console.WriteLine("Would you like to restart? Press r to restart.");
            if (Console.ReadKey().KeyChar == 'r')
            {
                await OpenApplication(cancellationToken);
            }
        }

        private async Task MonitorDataFactoryPipeLine()
        {
            var csc = new ClientSecretCredential(_configuration.TenantId, _configuration.ApplicationId, _configuration.ClientSecret, new TokenCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud });

            var armClient = new ArmClient(csc, _configuration.SubscriptionId, new ArmClientOptions { Environment = ArmEnvironment.AzurePublicCloud });

            ResourceIdentifier resourceIdentifier = SubscriptionResource.CreateResourceIdentifier(_configuration.SubscriptionId);
            SubscriptionResource subscriptionResource = armClient.GetSubscriptionResource(resourceIdentifier);

            Console.WriteLine("Get an existing resource group " + _configuration.DataFactoryResourceGroupName + "...");
            var resourceGroupOperation = subscriptionResource.GetResourceGroups().Get(_configuration.DataFactoryResourceGroupName);
            ResourceGroupResource resourceGroupResource = resourceGroupOperation.Value;

            Console.WriteLine("Data factory : " + _configuration.DataFactoryName + "...");
            var dataFactoryOperation = resourceGroupResource.GetDataFactories().Get(_configuration.DataFactoryName);
            DataFactoryResource dataFactoryResource = dataFactoryOperation.Value;
            var pipilene = await dataFactoryResource.GetDataFactoryPipelineAsync(pipelineName: _configuration.DataFactoryPipeLineName);
            var runResponse = await pipilene.Value.CreateRunAsync();
            Console.WriteLine("Pipeline run ID: " + runResponse.Value.RunId);

            // Monitor the pipeline run
            Console.WriteLine("Checking pipeline run status...");
            DataFactoryPipelineRunInfo pipelineRun;
            while (true)
            {
                pipelineRun = dataFactoryResource.GetPipelineRun(runResponse.Value.RunId.ToString());
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Status: " + pipelineRun.Status);
                if (pipelineRun.Status == "InProgress" || pipelineRun.Status == "Queued")
                {
                    Thread.Sleep(15000);
                }
                else
                {
                    break;
                }
            }

            // Check the copy activity run details
            Console.WriteLine("Checking copy activity run details...");

            var queryResponse = dataFactoryResource.GetActivityRun(pipelineRun.RunId.ToString(), new RunFilterContent(DateTime.UtcNow.AddMinutes(-10), DateTime.UtcNow.AddMinutes(10)));

            var enumerator = queryResponse.GetEnumerator();
            enumerator.MoveNext();

            if (pipelineRun.Status == "Succeeded")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(enumerator.Current.Output);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(enumerator.Current.Error);
                Console.ResetColor();
            }
        }


        private async Task CreateSearchClientForQueries()
        {
            SearchClient searchClient = new SearchClient(new Uri(_configuration.SearchServiceEndPoint), _configuration.SearchIndexName, new AzureKeyCredential(_configuration.QueryApiKey));
            await RunQueries(searchClient);
        }
        private static async Task RunQueries(SearchClient searchClient)
        {
            var searchOptions = new SearchOptions();
            SearchResults<ProductDomainModel> results;
            string searchString = "*";
            Console.WriteLine("Search Options:\n");
            Console.WriteLine("1. Search by Product Name :\n");
            Console.WriteLine("2. Search by Category Name :\n");
            Console.WriteLine("3. Search by Product Price :\n");
            Console.WriteLine("4. Search by Description :\n");
            Console.WriteLine("5. Search by Product Date Added :\n");
            Console.WriteLine("6. Search in all parameters(columns):\n");

            switch (Console.ReadLine())
            {
                case "1":
                    searchOptions.SearchFields.Add("product_name");
                    Console.WriteLine("Enter Product Name:\n");
                    break;
                case "2":
                    searchOptions.SearchFields.Add("category_name");
                    Console.WriteLine("Enter Category Name:\n");
                    break;
                case "3":
                    searchOptions.SearchFields.Add("price");
                    Console.WriteLine("Enter Product Price:\n");
                    break;
                case "4":
                    searchOptions.SearchFields.Add("description");
                    Console.WriteLine("Enter Product Description:\n");
                    break;
                case "5":
                    Console.WriteLine("Enter Product Added Date:\n");
                    searchOptions.SearchFields.Add("date_added");
                    break;
                case "6":
                    Console.WriteLine("Enter Text:\n");
                    break;
                default:
                    return;
            }
            searchString = Console.ReadLine();

            searchOptions.Select.Add("product_name");
            searchOptions.Select.Add("product_id");
            searchOptions.Select.Add("category_name");
            searchOptions.Select.Add("price");
            searchOptions.Select.Add("description");
            searchOptions.Select.Add("date_added");

            searchOptions.IncludeTotalCount = true;
            results = await searchClient.SearchAsync<ProductDomainModel>(searchString, searchOptions);

            Console.WriteLine($"Search Results: Total Count: {results.TotalCount}\n");

            int i = 0;
            foreach (SearchResult<ProductDomainModel> result in results.GetResults())
            {
                Console.WriteLine($"{++i} : {JsonSerializer.Serialize(result.Document)}");
                Console.WriteLine(Environment.NewLine);
            }

            Console.WriteLine();
        }
    }
}
