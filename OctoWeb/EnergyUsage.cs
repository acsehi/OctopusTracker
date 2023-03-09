namespace OctoWeb
{
    public class EnergyUsage
    {
        public DateTime Date { get; set; }

        public double Consumption { get; set; }

        public double UnitCost { get; set; }

        public double Cost
        {
            get
            {
                // *10 to convert from m3 to kwh, 100 to convert it to £
                return this.Consumption * 10 * UnitCost/ 100.0;
            }
        }
    }
}