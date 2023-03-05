
using Newtonsoft.Json;

namespace Octo
{
    public class TrackerTariff
    {
        public TrackerTariff(IHttpClient client, string code)
        {
            this.Code = code;
            this.httpClient = client;
        }

        private readonly IHttpClient httpClient;

        public string Code { private set; get; }

        public Dictionary<DateTime, double> Rates { set; get; }

        public async Task InitializeRates()
        {
            var js = JsonSerializer.Create();

            if (File.Exists(this.Code + ".json"))
            {
                using (TextReader tr = new StreamReader(this.Code + ".json"))
                {
                    JsonReader sr = new JsonTextReader(tr);
                    var x = js.Deserialize<TrackerTariff>(sr);
                    this.Rates = x.Rates;
                }
            }
            else
            {
                this.Rates = new Dictionary<DateTime, double>();
            }

            var json = await this.httpClient.GetStringAsync($"https://octopus.energy/api/v1/tracker/{this.Code}/daily/current/1/1/");

            dynamic jsonDe = JsonConvert.DeserializeObject(json);

            foreach (var period in jsonDe.periods)
            {
                DateTime d = period.date;
                double r = period.unit_rate;

                // Rates in the future are not real rates
                if (d <= DateTime.Now)
                {
                    // Overwrite if conflict with local source
                    this.Rates[d] = r;
                }
            }

            using (StreamWriter sr = new StreamWriter(this.Code + ".json"))
            {
                js.Serialize(sr, this);
            }
        }
    }
}
