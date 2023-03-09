using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Octo;
using System.Configuration;

namespace OctoWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnergyUsageController : ControllerBase
    {
        private readonly ILogger<EnergyUsageController> _logger;
        private readonly IConfiguration _config;

        public EnergyUsageController(ILogger<EnergyUsageController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

        }

        [HttpGet]
        public async Task<IEnumerable<EnergyUsage>> Get()
        {

            var userId = this._config.GetValue<string>("OctoConfiguration:UserId");
            var apiKey = this._config.GetValue<string>("OctoConfiguration:APIKey");

            var httpClient = new OctoHttpClient(apiKey);
            var account = await Account.GetAccount(httpClient, userId, apiKey);

            var energyUsage = new Dictionary<DateTime, EnergyUsage>();

            foreach (var gas in account.Gas.Consumption)
            {
                var e = new EnergyUsage()
                {
                    Date = gas.Key,
                    Consumption = gas.Value
                };

                energyUsage.Add(gas.Key, e);
            }

            foreach (var c in account.Gas.Tariff.Rates)
            {
                if (energyUsage.ContainsKey(c.Key))
                {
                    energyUsage[c.Key].UnitCost = c.Value;
                }
            }

            return energyUsage.Select(e => e.Value);
        }
    }
}