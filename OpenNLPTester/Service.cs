using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace OpenNLPTester
{
    class Service : IService
    {
        private readonly string _baseUrl;
        private readonly string _token;
        public readonly string _solrUrl;
        private readonly ILogger<Service> _logger;

        public Service(ILoggerFactory loggerFactory, IConfigurationRoot config)
        {
            var baseUrl = config["SomeConfigItem:BaseUrl"];
            var token = config["SomeConfigItem:Token"];
            var solrUrl = config["solrConfig:url"];

            _baseUrl = baseUrl;
            _token = token;
            _solrUrl = solrUrl; 
            _logger = loggerFactory.CreateLogger<Service>();
        }

        public async Task AddLogging()
        {
            _logger.LogDebug(_baseUrl);
            _logger.LogDebug(_token);
        }
    }
}
