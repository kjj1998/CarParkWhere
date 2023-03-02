using System.Globalization;
using CZ3002_Backend.Models;

namespace CZ3002_Backend.Services;

public class MallCarparkDataSetUpService : IDataSetUpService<MallCarparkModel, LtaLiveCarparkValue>
{
    private readonly HttpClient _client;

    public MallCarparkDataSetUpService()
    {
        _client = new HttpClient();
    }
    
    public async Task<List<MallCarparkModel>> SetUp(List<LtaLiveCarparkValue>? carparks)
    {
        var mallCarParks = new List<MallCarparkModel>();

        if (carparks == null) return mallCarParks;

        foreach (var carpark in carparks)
        {
            Console.WriteLine(carpark.Development);
            if (!mallCarParks.Exists(x => x.CarparkCode == carpark.CarParkID))
            {
                var newMallCarPark = new MallCarparkModel();
                var staticRecords = await RetrieveStaticRecords(carpark);
                
                if (staticRecords != null && staticRecords.Count > 0)
                {
                    newMallCarPark.Coordinates = retrieveLatLong(carpark.Location);
                    newMallCarPark.Name = carpark.Development;
                    newMallCarPark.CarparkCode = carpark.CarParkID;
                    newMallCarPark.SunPhRate = staticRecords[0].sunday_publicholiday_rate;
                    newMallCarPark.SatRate = staticRecords[0].saturday_rate;
                    newMallCarPark.WeekDayRate1 = staticRecords[0].weekdays_rate_1;
                    newMallCarPark.WeekDayRate2 = staticRecords[0].weekdays_rate_2;
                    newMallCarPark.Lots = new Lots();
                    
                }
                else
                {
                    newMallCarPark.Coordinates = retrieveLatLong(carpark.Location);
                    newMallCarPark.Name = carpark.Development;
                    newMallCarPark.CarparkCode = carpark.CarParkID;
                    newMallCarPark.SunPhRate = "Not Available";
                    newMallCarPark.SatRate = "Not Available";
                    newMallCarPark.WeekDayRate1 = "Not Available";
                    newMallCarPark.WeekDayRate2 = "Not Available";
                    newMallCarPark.Lots = new Lots();
                }
                
                mallCarParks.Add(newMallCarPark);
            }

            var mallCarPark = mallCarParks.Find(x => x.CarparkCode == carpark.CarParkID);
            switch (carpark.LotType)
            {
                case "C":
                    mallCarPark!.Lots.Car.Available = carpark.AvailableLots;
                    break;
                case "Y":
                    mallCarPark!.Lots.HeavyVehicle.Available = carpark.AvailableLots;
                    break;
                case "M":
                    mallCarPark!.Lots.Motorcycle.Available = carpark.AvailableLots;
                    break;
            }
        }

        return mallCarParks;
    }

    private async Task<List<GovStaticMallRecord>?> RetrieveStaticRecords(LtaLiveCarparkValue carpark)
    {
        var staticRecords = new List<GovStaticMallRecord>();
        if (carpark.Development == "VivoCity P3" || carpark.Development == "VivoCity P2")
        {
            staticRecords = await GetStaticMallRecord("VivoCity");
        }
        else if (carpark.Development == "Sentosa")
        {
            var emptyList = new List<GovStaticMallRecord>();
            return emptyList;
        }
        else if (carpark.Development == "Mandarin")
        {
            staticRecords = await GetStaticMallRecord("Mandarin Gallery");
        }
        else if (carpark.Development == "313@Somerset")
        {
            staticRecords = await GetStaticMallRecord("313 Somerset");
        }
        else if (carpark.Development == "The Atrium@Orchard")
        {
            staticRecords = await GetStaticMallRecord("The Atrium Orchard");
        }
        else
        {
            staticRecords = await GetStaticMallRecord(carpark.Development);
        }

        return staticRecords;
    }

    private async Task<List<GovStaticMallRecord>?> GetStaticMallRecord(string name)
    {
        var requestUri =
            $"https://data.gov.sg/api/action/datastore_search?resource_id=85207289-6ae7-4a56-9066-e6090a3684a5&q={name}";
        var response = await _client.GetFromJsonAsync<GovStaticMallRoot>(requestUri);

        return response?.result.records;
    }

    private LatLong retrieveLatLong(string latLongString)
    {
        var latLong = latLongString.Split(" ");
        
        var coordinate = new LatLong
        {
            latitude = float.Parse(latLong[0], CultureInfo.InvariantCulture),
            longitude = float.Parse(latLong[1], CultureInfo.InvariantCulture)
        };

        return coordinate;
    }
    
    private bool IsApproximatelyEqual(string s1, string s2, int maxDistance)
    {
        if (s1 == s2)
        {
            return true;
        }

        int s1Len = s1.Length;
        int s2Len = s2.Length;
        if (s1Len == 0 || s2Len == 0)
        {
            return false;
        }

        int[,] d = new int[s1Len + 1, s2Len + 1];

        for (int i = 0; i <= s1Len; i++)
        {
            d[i, 0] = i;
        }

        for (int i = 0; i <= s2Len; i++)
        {
            d[0, i] = i;
        }

        for (int j = 1; j <= s2Len; j++)
        {
            for (int i = 1; i <= s1Len; i++)
            {
                if (s1[i - 1] == s2[j - 1])
                {
                    d[i, j] = d[i - 1, j - 1];
                }
                else
                {
                    d[i, j] = Math.Min(Math.Min(
                            d[i - 1, j] + 1, // Deletion
                            d[i, j - 1] + 1), // Insertion
                        d[i - 1, j - 1] + 1); // Substitution
                }
            }
        }

        var dist = d[s1Len, s2Len];
        
        return dist <= maxDistance;
    }
}