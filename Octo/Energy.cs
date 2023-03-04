namespace Octo
{
    public class Energy
    {
        public Meter Meter { get; set; }

        public TrackerTariff Tariff { get; set; }

        public Dictionary<DateTime,double> Consumption { get; set; }
    }
}
