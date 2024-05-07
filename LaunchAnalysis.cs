using CsvHelper.Configuration;
using System.Globalization;

namespace SpaceShuttleLaunch
{
    internal class LaunchAnalysis
    {
        internal bool IsValid { get; set; }
        protected string FolderPath { get; }

        internal LaunchAnalysis(string folderPath)
        {
            IsValid = false;
            FolderPath = folderPath;
        }

        internal void AnalyzeAndReport()
        {
            List<WeatherData> weatherDataList = new List<WeatherData>();
            List<string> reportLines = new List<string> { "Spaceport, Best Launch Day" };

            string[] csvFiles = Directory.GetFiles(FolderPath, "*.csv")
                             .Where(filePath => Path.GetFileName(filePath) != "LaunchAnalysisReport.csv")
                             .ToArray();

            if (csvFiles.Length == 0)
            {
                throw new Exception($"No files were found in: {FolderPath}");
            }

            CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = false,
                HeaderValidated = null,
                MissingFieldFound = null
            };

            foreach (string filePath in csvFiles)
            {
                string spaceportName = Path.GetFileNameWithoutExtension(filePath);
                if (!Spaceports.SpaceportLatitudes.TryGetValue(spaceportName, out double latitude))
                {
                    throw new Exception($"Latitude for {spaceportName} is not defined.");
                }

                WeatherData weatherData = new WeatherData()
                {
                    Spaceport = spaceportName,
                    Latitude = latitude,
                };

                string[]? values = [];
                string fieldName = string.Empty;
                List<string> fieldValues = new List<string>();

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line = string.Empty;
                    while ((line = reader.ReadLine()!) != null)
                    {
                        values = line.Split(',');
                        fieldName = values[0].Trim();
                        fieldValues = values.Skip(1).Select(v => v.Trim()).ToList();

                        switch (fieldName)
                        {
                            case "Day":
                                weatherData.Day?.AddRange(fieldValues.Select(s =>
                                {
                                    if (int.TryParse(s, out int result))
                                        return result;
                                    return -1;
                                }).Where(x => x != -1));
                                break;
                            case "Temperature":
                                weatherData.Temperature.AddRange(fieldValues.Select(double.Parse));
                                break;
                            case "Wind":
                                weatherData.Wind.AddRange(fieldValues.Select(double.Parse));
                                break;
                            case "Humidity":
                                weatherData.Humidity.AddRange(fieldValues.Select(double.Parse));
                                break;
                            case "Precipitation":
                                weatherData.Precipitation.AddRange(fieldValues.Select(double.Parse));
                                break;
                            case "Lightning":
                                weatherData.Lightning.AddRange(fieldValues);
                                break;
                            case "Clouds":
                                weatherData.Clouds.AddRange(fieldValues);
                                break;
                        }
                    }
                }
                weatherDataList.Add(weatherData);
            }

            var bestDaysForSpaceports = weatherDataList.Select(data => new
            {
                data.Spaceport,
                BestDay = data?.Day?
                .Select((day, index) => new
                {
                    Day = day,
                    Index = index,
                    Score = WeatherCriteria.CalculateDayScore(data, index) // Calculate score for each day
                })
                .Where(x => WeatherCriteria.IsValidLaunchDay(data, x.Index)) // Check if the day is valid
                .OrderBy(x => x.Score) // Order by the calculated score, lowest is better
                .FirstOrDefault() // Select the best day based on the score
            })
            .Where(x => x.BestDay != null)
            .Select(x => new { x.Spaceport, BestDay = x?.BestDay?.Day })
            .ToList();

            var sortedSpaceports = bestDaysForSpaceports
                .OrderBy(x => Math.Abs(Spaceports.SpaceportLatitudes[x.Spaceport!])) // Sort by proximity to the Equator
                .ThenBy(x => x.BestDay)
                .ToList();

            if (sortedSpaceports == null || sortedSpaceports.Count == 0)
            {
                Console.WriteLine("No valid launch day and location found based on the criteria.");
            }
            else
            {
                IsValid = true;

                foreach (var entry in sortedSpaceports)
                {
                    reportLines.Add($"{entry?.Spaceport?.Replace(",", " ")}, July {entry?.BestDay}");.
                }

                File.WriteAllLines(Path.Combine(FolderPath, "LaunchAnalysisReport.csv"), reportLines);
            }
        }
    }
}

