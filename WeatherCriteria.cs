namespace SpaceShuttleLaunch
{
    internal static class WeatherCriteria
    {
        public static bool IsValidLaunchDay(WeatherData data, int index)
        {
            return data.Temperature[index] >= 1d
            && data.Temperature[index] <= 32d
            && data.Wind[index] <= 11d
            && data.Humidity[index] < 55d
            && data.Precipitation[index] == 0d
            && data.Lightning[index] == "No"
            && (data.Clouds[index] != "Nimbus"
            || data.Clouds[index] != "Cumulus");
        }

        public static double CalculateDayScore(WeatherData data, int index)
        {
            // Simulated data for best weather conditions
            double score = 0;

            // Wind factor: Lower wind is better, so score increases with higher wind
            score += data.Wind[index];
            // Humidity factor: Humidity contributes less so it's normalized
            score += data.Humidity[index] / 100;
            // Precipitation penalty: Any precipitation adds a significant penalty
            score += (data.Precipitation[index] > 0) ? 10 : 0;
            // Temperature factor: penalize deviation from the ideal range [15, 25]
            if (data.Temperature[index] < 15)
                score += (15 - data.Temperature[index]); // Penalty for temperatures below the ideal range
            else if (data.Temperature[index] > 25)
                score += (data.Temperature[index] - 25); // Penalty for temperatures above the ideal range

            return score;
        }
    }
}
