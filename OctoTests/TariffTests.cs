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

            var x = await Tariff.GetRates(client.Object, "testTariff");

            Assert.AreEqual("testTariff", x.Code);
            Assert.AreEqual("01/02/2023", x.Rates.First().Key.ToShortDateString());
            Assert.AreEqual("6.972", x.Rates.First().Value.ToString());
            Assert.AreEqual("7.098", x.Rates.Skip(1).First().Value.ToString());            
            client.Verify(x => x.GetStringAsync("https://octopus.energy/api/v1/tracker/testTariff/daily/current/1/1/"));
        }
    }
}
