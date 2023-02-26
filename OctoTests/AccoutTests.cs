using Moq;
using Octo;

namespace OctoTests
{
    [TestClass]
    public class AccoutTests
    {
        [TestMethod]
        public async Task GetAccountTest()
        {
            var response = "{\"number\":\"A-tet\",\"properties\":[{\"id\":4517221,\"moved_in_at\":\"2001-02-12T00:00:00Z\",\"moved_out_at\":null,\"address_line_1\":\"Test CLOSE\",\"address_line_2\":\"\",\"address_line_3\":\"London\",\"town\":\"London\",\"county\":\"\",\"postcode\":\"POST CODE\",\"electricity_meter_points\":[{\"mpan\":\"20000\",\"profile_class\":1,\"consumption_standard\":2210,\"meters\":[{\"serial_number\":\"22\",\"registers\":[{\"identifier\":\"1\",\"rate\":\"STANDARD\",\"is_settlement_register\":true}]}],\"agreements\":[{\"tariff_code\":\"E-1R-VAR-22-10-01-H\",\"valid_from\":\"2001-02-15T00:00:00Z\",\"valid_to\":\"2001-02-17T00:00:00Z\"},{\"tariff_code\":\"E-1R-SILVER-FLEX-22-11-25-H\",\"valid_from\":\"2001-02-17T00:00:00Z\",\"valid_to\":null}],\"is_export\":false}],\"gas_meter_points\":[{\"mprn\":\"39\",\"consumption_standard\":12874,\"meters\":[{\"serial_number\":\"G4\"}],\"agreements\":[{\"tariff_code\":\"G-1R-VAR-22-10-01-H\",\"valid_from\":\"2023-02-15T00:00:00Z\",\"valid_to\":\"2023-02-17T00:00:00Z\"},{\"tariff_code\":\"G-1R-SILVER-FLEX-22-11-25-H\",\"valid_from\":\"2001-02-17T00:00:00Z\",\"valid_to\":null}]}]}]}";

            var client = new Mock<IHttpClient>();
            client.Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(response));

            var result = await Account.GetAccount(client.Object, "accountName","auth");

            Assert.AreEqual("accountName", result.Id);

            Assert.AreEqual("E-1R-SILVER-FLEX-22-11-25-H", result.Electricity.Tariff.Code);
            Assert.AreEqual("G-1R-SILVER-FLEX-22-11-25-H", result.Gas.Tariff.Code);
            Assert.AreEqual("H", result.Region);

            Assert.AreEqual("G4", result.Gas.Meter.Id);
            Assert.AreEqual("22", result.Electricity.Meter.Id);

            Assert.AreEqual("39", result.Gas.Meter.MeterPointNumber);
            Assert.AreEqual("20000", result.Electricity.Meter.MeterPointNumber);
        }
    }
}