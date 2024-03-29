using System.Text.Json.Serialization;
namespace EmpowerID.Infrastructure.Configuration
{
    public class ConfigAppSettings
    {

        [JsonPropertyName("ConnString")]
        public string ConnString { get; set; }
        
        [JsonPropertyName("ConnStringSecondary")]
        public string ConnStringSecondary { get; set; }
        [JsonPropertyName("TenantId")]
        public string TenantId { get; set; }
        [JsonPropertyName("SubscriptionId")]
        public string SubscriptionId { get; set; }
        [JsonPropertyName("ApplicationId")]
        public string ApplicationId { get; set; }
        [JsonPropertyName("ClientSecret")]
        public string ClientSecret { get; set; }
        [JsonPropertyName("DataFactoryResourceGroupName")]
        public string DataFactoryResourceGroupName { get; set; }
        [JsonPropertyName("DataFactoryName")]
        public string DataFactoryName { get; set; }
        [JsonPropertyName("DataFactoryPipeLineName")]
        public string DataFactoryPipeLineName { get; set; }
        [JsonPropertyName("SearchServiceEndPoint")]
        public string SearchServiceEndPoint { get; set; }
        [JsonPropertyName("QueryApiKey")]
        public string QueryApiKey { get; set; }
        [JsonPropertyName("SearchIndexName")]
        public string SearchIndexName { get; set; }

        public class Logging
        {
            [JsonPropertyName("LogLevel")]
            public LogLevel LogLevel { get; set; }
        }
        public class LogLevel
        {
            [JsonPropertyName("Default")]
            public string Default { get; set; }

            [JsonPropertyName("Hangfire")]
            public string Hangfire { get; set; }
        }
    }
}
