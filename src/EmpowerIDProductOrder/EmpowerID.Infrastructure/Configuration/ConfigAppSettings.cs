using System.Text.Json.Serialization;
namespace EmpowerID.Infrastructure.Configuration
{
    public class ConfigAppSettings
    {

        [JsonPropertyName("ConnString")]
        public string ConnString { get; set; }
        
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
