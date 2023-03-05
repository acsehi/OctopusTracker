using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace Octo
{
    public class Account
    {

        public Account(IHttpClient httpClient, string userId)
        {
            this.Id = userId;
            this.httpClient = httpClient;
        }

        public string Id { get; private set; }

        private readonly IHttpClient httpClient;

        public string Region
        {
            get
            {
                if (this.Gas.Tariff == null)
                {
                    return null;
                }
                else
                {
                    return this.Gas.Tariff.Code.Substring(this.Gas.Tariff.Code.Length - 1);
                }
            }
        }

        public Energy Gas { get; set; } = new Energy();
        public Energy Electricity { get; set; } = new Energy();

        public void ToCSV(string Filename)
        {
            using (StreamWriter sr = new StreamWriter(Filename))
            {
                sr.WriteLine("Energy,Date,UnitRate,Consumption");

                foreach (var e in this.Electricity.Consumption)
                {

                    if (this.Electricity.Tariff.Rates.ContainsKey(e.Key))
                    {
                        sr.WriteLine($"Electricity,{e.Key},{this.Electricity.Tariff.Rates[e.Key]},{e.Value}");
                    }
                }

                foreach (var g in this.Gas.Consumption)
                {

                    if (this.Gas.Tariff.Rates.ContainsKey(g.Key))
                    {
                        sr.WriteLine($"Gas,{g.Key},{this.Gas.Tariff.Rates[g.Key]},{g.Value}");
                    }
                }
            }
        }

        public static async Task<Account> GetAccount(IHttpClient httpClient, string userId, string apiKey)
        {
            var uri = $"https://api.octopus.energy/v1/accounts/{userId}/";
            var json = await httpClient.GetStringAsync(uri, apiKey);
            dynamic jsonDe = JsonConvert.DeserializeObject(json);
            var electicity = jsonDe.properties[0].electricity_meter_points[0];
            var gas = jsonDe.properties[0].gas_meter_points[0];

            Account userAccount = new Account(httpClient, userId);

            userAccount.Gas.Meter = new Meter
            {
                Id = gas.meters[0].serial_number,
                MeterPointNumber = gas.mprn,
            };

            userAccount.Electricity.Meter = new Meter
            {
                Id = electicity.meters[0].serial_number,
                MeterPointNumber = electicity.mpan,
            };


            foreach (var e in electicity.agreements)
            {
                if (e.valid_to == null)
                {
                    userAccount.Electricity.Tariff = new TrackerTariff(httpClient, (string)e.tariff_code);
                }
            }

            foreach (var g in gas.agreements)
            {
                if (g.valid_to == null)
                {
                    userAccount.Gas.Tariff = new TrackerTariff(httpClient, (string)g.tariff_code);
                }
            }

            await userAccount.LoadRates();
            await userAccount.LoadConsumption();

            return userAccount;
        }
        public async Task LoadConsumption()
        {
            this.Electricity.Consumption = await Consumption.GetConsumption(this.httpClient, this.Electricity.Meter.MeterPointNumber, this.Electricity.Meter.Id, EnergyType.Electicity);
            this.Gas.Consumption = await Consumption.GetConsumption(this.httpClient, this.Gas.Meter.MeterPointNumber, this.Gas.Meter.Id, EnergyType.Gas);
        }

        public async Task<bool> LoadRates()
        {
            await this.Gas.Tariff.InitializeRates();
            await this.Electricity.Tariff.InitializeRates();

            return true;
        }
    }
}
