﻿using Moq;
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


            Assert.AreEqual("03/04/2023", x.First().Key.ToString("MM/dd/yyyy"));
            Assert.AreEqual("0.1", x.First().Value.ToString());
            client.Verify(x => x.GetStringAsync("https://api.octopus.energy/v1/electricity-meter-points/meterPointNumber/meters/meterId/consumption/?group_by=day"));
        }
    }
}
