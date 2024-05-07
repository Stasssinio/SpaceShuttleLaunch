namespace SpaceShuttleLaunch
{
    public class WeatherData
    {
        public WeatherData()
        {
            Day = new List<int>();
            Temperature = new List<double>();
            Humidity = new List<double>();
            Wind = new List<double>();
            Precipitation = new List<double>();
            Lightning = new List<string>();
            Clouds = new List<string>();
        }

        public string? Spaceport { get; set; }
        public double Latitude { get; set; }
        public List<int>? Day { get; set; }
        public List<double> Temperature { get; set; }
        public List<double> Humidity { get; set; }
        public List<double> Wind { get; set; }
        public List<double> Precipitation { get; set; }
        public List<string> Lightning { get; set; }
        public List<string> Clouds { get; set; }
    }
}
