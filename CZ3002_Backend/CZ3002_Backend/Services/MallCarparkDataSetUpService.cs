using System.Globalization;
using CZ3002_Backend.Enums;
using CZ3002_Backend.Models;
using Geohash;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Services;

public class MallCarparkDataSetUpService : IDataSetUpService<MallCarparkModel, LtaLiveCarparkValue>
{
    private readonly HttpClient _client;
    private const string NotAvailable = "Not Available";
    private IConfiguration _configuration;

    public MallCarparkDataSetUpService(IConfiguration configuration)
    {
        _client = new HttpClient();
        _configuration = configuration;
    }
    
    public async Task<List<MallCarparkModel>> SetUp(List<LtaLiveCarparkValue>? carparks)
    {
        var mallCarParks = new List<MallCarparkModel>();
        if (carparks == null) return mallCarParks;

        foreach (var carpark in carparks)
        {
            // Console.WriteLine(carpark.Development);
            if (!mallCarParks.Exists(x => x.CarparkCode == carpark.CarParkID))
            {
                var newMallCarPark = new MallCarparkModel();
                var staticRecords = await RetrieveStaticRecords(carpark);
                
                if (staticRecords != null && staticRecords.Count > 0)
                {
                    newMallCarPark.Coordinates = retrieveLatLong(carpark.Location);
                    var hasher = new Geohasher();
                    var hash = hasher.Encode(newMallCarPark.Coordinates.Value.Latitude,
                        newMallCarPark.Coordinates.Value.Longitude,(int)Precision.GeoHash);
                    newMallCarPark.GeoHash = hash;
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
                    InitializeMallCarparksWithoutStaticData(ref newMallCarPark, carpark);
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

    private void InitializeMallCarparksWithoutStaticData(ref MallCarparkModel newMallCarPark, LtaLiveCarparkValue carpark)
    {
        newMallCarPark.Coordinates = retrieveLatLong(carpark.Location);
        var hasher = new Geohasher();
        var hash = hasher.Encode(newMallCarPark.Coordinates.Value.Latitude,
            newMallCarPark.Coordinates.Value.Longitude,(int)Precision.GeoHash);
        newMallCarPark.GeoHash = hash;
        newMallCarPark.Name = carpark.Development;
        newMallCarPark.CarparkCode = carpark.CarParkID;
        newMallCarPark.SunPhRate = NotAvailable;
        newMallCarPark.SatRate = NotAvailable;
        newMallCarPark.WeekDayRate1 = NotAvailable;
        newMallCarPark.WeekDayRate2 = NotAvailable;
        newMallCarPark.Lots = new Lots();
    }

    private async Task<List<GovStaticMallRecord>?> RetrieveStaticRecords(LtaLiveCarparkValue carpark)
    {
        var staticRecords = new List<GovStaticMallRecord>();
        
        // Some terms need to be handed separately
        if (carpark.Development is "VivoCity P3" or "VivoCity P2")
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
        var requestUri = _configuration["GOV_MALL_CARPARK_RATES_API"] + name;
        var response = await _client.GetFromJsonAsync<GovStaticMallRoot>(requestUri);

        return response?.result.records;
    }

    private GeoPoint retrieveLatLong(string latLongString)
    {
        var latLong = latLongString.Split(" ");
        
        var coordinate = new GeoPoint(float.Parse(latLong[0], CultureInfo.InvariantCulture),float.Parse(latLong[1], CultureInfo.InvariantCulture));

        return coordinate;
    }
}