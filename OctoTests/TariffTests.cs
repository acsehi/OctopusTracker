using Moq;
using Octo;

namespace OctoTests
{
    [TestClass]
    public class TariffTests
    {
        [TestMethod]
        public async Task GetRates()
        {
            var sr = new StreamReader("unitrates.json");

            var client = new Mock<IHttpClient>();
            client.Setup(x => x.GetStringAsync(It.IsAny<string>())).Returns(Task.FromResult(sr.ReadToEnd()));

            const string tariffCode = "G-1R-SILVER-FLEX-22-11-25-H";
            var x = new TrackerTariff(client.Object, tariffCode);
            
            await x.InitializeRates();

            Assert.AreEqual(tariffCode, x.Code);
            Assert.AreEqual("01/02/2023", x.Rates.First().Key.ToShortDateString());
            Assert.AreEqual("6.972", x.Rates.First().Value.ToString());
            Assert.AreEqual("7.098", x.Rates.Skip(1).First().Value.ToString());            
            client.Verify(x => x.GetStringAsync("https://octopus.energy/api/v1/tracker/G-1R-SILVER-FLEX-22-11-25-H/daily/current/1/1/"));
        }
    }
}
