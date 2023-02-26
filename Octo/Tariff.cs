
using Newtonsoft.Json;

namespace Octo
{
    public class Tariff
    {
        public Tariff()
        {
        }

        public Tariff(string code)
        {
            this.Code = code;
        }

        public string Code { set;  get; }

        public Dictionary<DateTime, double> Rates { get; set; }

        public static async Task<Tariff> GetRates(IHttpClient client, string code)
        {
            var json = await client.GetStringAsync($"https://octopus.energy/api/v1/tracker/{code}/daily/current/1/1/");

            dynamic jsonDe = JsonConvert.DeserializeObject(json);
            var rates = new Dictionary<DateTime, double>();

            foreach (var period in jsonDe.periods)
            {
                DateTime d = period.date;
                double r = period.unit_rate;
                rates.Add(d, r);
            }

            return new Tariff()
            {
                Code = code,
                Rates = rates
            };
        }
    }
}
