// See https://aka.ms/new-console-template for more information
using Octo;
using OctoCmd;
using System.Configuration;

var appSettings = ConfigurationManager.AppSettings;


var apiKey = appSettings["ApiKey"];
var userId = appSettings["UserId"];

var httpClient = new OctoHttpClient(apiKey);

var account = Account.GetAccount(httpClient, userId, apiKey).ConfigureAwait(false).GetAwaiter().GetResult();
await account.LoadRates(httpClient);
await account.LoadUsage(httpClient);

double gasCost = 0;
double gasCost_flex = 0;
int days = 0;

foreach (var g in account.Gas.Consumption)
{

    if (account.Gas.Tariff.Rates.ContainsKey(g.Key))
    {
        gasCost += EnergyConverter.CubiCMeterTokwh(g.Value) * account.Gas.Tariff.Rates[g.Key];
        gasCost_flex += EnergyConverter.CubiCMeterTokwh(g.Value) * 10.5;
        days++;
    }
    else
    {
        Console.WriteLine(g.Key + " is dropped");
    }
}

gasCost = gasCost / 100.0; // Pence to £
gasCost_flex = gasCost_flex / 100.0;

Console.WriteLine($"Gas cost over {days} days: £{gasCost.ToString("0.00")}");
Console.WriteLine($"Same cost on Capped tariff: £{gasCost_flex.ToString("0.00")}. A reduction of £{(gasCost_flex - gasCost).ToString("0.00")}");
Console.WriteLine($"Which is a {(100 - (100.0 * gasCost / gasCost_flex)).ToString("0.00")}% reduction.");

Console.WriteLine();

double electricityCost = 0;
double electricityCost_flex = 0;
days = 0;


foreach (var e in account.Electricity.Consumption)
{

    if (account.Electricity.Tariff.Rates.ContainsKey(e.Key))
    {
        electricityCost += e.Value * account.Electricity.Tariff.Rates[e.Key];
        electricityCost_flex += e.Value * 34.0;
        days++;
    }
}

electricityCost = electricityCost / 100.0; // Pence to £
electricityCost_flex = electricityCost_flex / 100.0;

Console.WriteLine($"Electricity cost over {days} days: £{electricityCost.ToString("0.00")}");
Console.WriteLine($"Same cost on Capped tariff: £{electricityCost_flex.ToString("0.00")}. A reduction of £{(electricityCost_flex - electricityCost).ToString("0.00")}");
Console.WriteLine($"Which is a {(100 - (100.0 * electricityCost / electricityCost_flex)).ToString("0.00")}% reduction.");
Console.WriteLine();

int allSavings = (int)((electricityCost_flex - electricityCost) + (gasCost_flex - gasCost));

Console.WriteLine($"All saving:£{allSavings}");

account.ToCSV($"consumption-{DateTime.Now.Year}-{DateTime.Now.Month}.tsv");

Console.ReadLine();

