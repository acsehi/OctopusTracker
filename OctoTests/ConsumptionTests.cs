using Moq;
using Octo;

namespace OctoTests
{
    [TestClass]
    public class ConsumptionTests
    {
        [TestMethod]
        public async Task GetCompsion()
        {
            var sr = new StreamReader("econsumption.json");

            var client = new Mock<IHttpClient>();
            client.Setup(x => x.GetStringAsync(It.IsAny<string>())).Returns(Task.FromResult(sr.ReadToEnd()));

            const string tariffCode = "E-1R-SILVER-FLEX-22-11-25-H";
            var x = await Consumption.GetConsumption(client.Object, "meterPointNumber", "meterId", EnergyType.Electicity);            


            Assert.AreEqual("01/03/2023", x.Rates.First().Key.ToShortDateString());
            Assert.AreEqual("0.1", x.Rates.First().Value.ToString());
            client.Verify(x => x.GetStringAsync("https://octopus.energy/api/v1/tracker/G-1R-SILVER-FLEX-22-11-25-H/daily/current/1/1/"));
        }
    }
}
