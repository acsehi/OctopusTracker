namespace Octo
{
    public class Meter
    {
        public string Id { get; set; }

        public string MeterPointNumber { get; set; }
    }

    public enum EnergyType
    {
        Gas,
        Electicity
    }
}
