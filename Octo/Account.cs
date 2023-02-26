using Newtonsoft.Json;
using System.IO;
using System.Security.Principal;

namespace Octo
{
    public class Account
    {
        public string Id { get; set; }

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


        public async Task<bool> LoadRates(IHttpClient client)
        {
            this.Gas.Tariff = await Tariff.GetRates(client, this.Gas.Tariff.Code);
            this.Electricity.Tariff = await Tariff.GetRates(client, this.Electricity.Tariff.Code);

            return true;
        }

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

            Account userAccount = new Account()
            {
                Id = userId,
            };

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
                    userAccount.Electricity.Tariff = new Tariff((string)e.tariff_code);
                }
            }

            foreach (var g in gas.agreements)
            {
                if (g.valid_to == null)
                {
                    userAccount.Gas.Tariff = new Tariff((string)g.tariff_code);
                }
            }



            return userAccount;
        }
        public async Task LoadUsage(IHttpClient httpClient)
        {
            await UpdateElectricity(httpClient);
            await UpdateGas(httpClient);
        }

        private async Task UpdateElectricity(IHttpClient httpClient)
        {
            var uri = $"https://api.octopus.energy/v1/electricity-meter-points/{this.Electricity.Meter.MeterPointNumber}/meters/{this.Electricity.Meter.Id}/consumption/?group_by=day";
            var json = await httpClient.GetStringAsync(uri);
            dynamic jsonDe = JsonConvert.DeserializeObject(json);


            this.Electricity.Consumption = new Dictionary<DateTime, double>();

            foreach (var item in jsonDe.results)
            {
                DateTime date = item.interval_start;
                double value = item.consumption;

                this.Electricity.Consumption.Add(date, value);
            }
        }

        private async Task UpdateGas(IHttpClient httpClient)
        {
            var uri = $"https://api.octopus.energy/v1/gas-meter-points/{this.Gas.Meter.MeterPointNumber}/meters/{this.Gas.Meter.Id}/consumption/?group_by=day";
            var json = await httpClient.GetStringAsync(uri);
            dynamic jsonDe = JsonConvert.DeserializeObject(json);


            this.Gas.Consumption = new Dictionary<DateTime, double>();

            foreach (var item in jsonDe.results)
            {
                DateTime date = item.interval_start;
                double value = item.consumption;

                this.Gas.Consumption.Add(date, value);
            }
        }
    }
}
