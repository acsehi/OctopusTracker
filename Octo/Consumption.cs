using Newtonsoft.Json;

namespace Octo
{
    public class Consumption
    {
        public static async Task<Dictionary<DateTime, double>> GetConsumption(IHttpClient httpClient,
            string meterPointNumber,
            string meterId,
            EnergyType energySource)
        {
            string eUrl = string.Empty;
            switch (energySource)
            {
                case EnergyType.Gas:
                    eUrl = "gas-meter-points";
                    break;
                case EnergyType.Electicity:
                    eUrl = "electricity-meter-points";
                    break;
            }

            var uri = $"https://api.octopus.energy/v1/{eUrl}/{meterPointNumber}/meters/{meterId}/consumption/?group_by=day";
            var json = await httpClient.GetStringAsync(uri);
            dynamic jsonDe = JsonConvert.DeserializeObject(json);

            var c = new Dictionary<DateTime, double>();

            foreach (var item in jsonDe.results)
            {
                DateTime date = item.interval_start;
                double value = item.consumption;

                c.Add(date, value);
            }

            return c;
        }

    }
}
