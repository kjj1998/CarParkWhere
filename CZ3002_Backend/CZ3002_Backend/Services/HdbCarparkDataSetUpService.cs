using System.Diagnostics;
using System.Globalization;
using CZ3002_Backend.Models;

namespace CZ3002_Backend.Services;

public class HdbCarparkDataSetUpService : IDataSetUpService<HdbCarParkModel, GovLiveCarparkDatum>
{
    private readonly HttpClient _client;
    private ILogger<HdbCarparkDataSetUpService> _logger;
    private IConfiguration _configuration;
    private const string NotAvailable = "Not Available";

    public HdbCarparkDataSetUpService(ILogger<HdbCarparkDataSetUpService> logger, IConfiguration configuration)
    {
        _client = new HttpClient();
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<List<HdbCarParkModel>> SetUp(List<GovLiveCarparkDatum>? carparks)
    {
        var hdbCarParks = new List<HdbCarParkModel>();

        if (carparks == null) return hdbCarParks;
        // var count = 1;
        
        foreach (var carpark in carparks)
        {
            // Console.WriteLine($"{count++}. Carpark Id = {carpark.carpark_number}");
            
            if (!hdbCarParks.Exists(x => x.CarparkCode == carpark.carpark_number))
            {
                var newHdbCarPark = new HdbCarParkModel();
                var staticRecords = await GetStaticHdbCarParkRecord(carpark.carpark_number);

                if (staticRecords.Count > 0 && staticRecords[0]?.car_park_no == carpark.carpark_number)
                {
                    var staticRecord = staticRecords[0];

                    if (staticRecord != null)
                    {
                        var latLong = await ConvertSvy21ToLatLong(
                            double.Parse(staticRecord.x_coord, CultureInfo.InvariantCulture),
                            double.Parse(staticRecord.y_coord, CultureInfo.InvariantCulture));

                        newHdbCarPark.CarparkCode = staticRecord.car_park_no;
                        newHdbCarPark.Name = staticRecord.address;
                        newHdbCarPark.System = staticRecord.type_of_parking_system;
                        newHdbCarPark.Coordinates = latLong;
                        newHdbCarPark.ShortTermParking = staticRecord.short_term_parking;
                        newHdbCarPark.FreeParking = staticRecord.free_parking;
                        newHdbCarPark.NightParking = staticRecord.night_parking;
                        newHdbCarPark.CarParkDecks = int.Parse(staticRecord.car_park_decks, CultureInfo.InvariantCulture);
                        newHdbCarPark.GantryHeight = float.Parse(staticRecord.gantry_height, CultureInfo.InvariantCulture);
                        newHdbCarPark.CarParkBasement = staticRecord.car_park_basement;
                        newHdbCarPark.Lots = new Lots();
                    }
                }
                else
                {
                    InitializeStaticDataNotAvailableCarparks(ref newHdbCarPark, carpark.carpark_number);
                }

                hdbCarParks.Add(newHdbCarPark);
            }

            var hdbCarPark = hdbCarParks.Find(x => x.CarparkCode == carpark.carpark_number);
            foreach (var info in carpark.carpark_info)
            {
                switch (info.lot_type)
                {
                    case "C":
                        hdbCarPark!.Lots.Car.Available = int.Parse(info.lots_available);
                        hdbCarPark.Lots.Car.Total = int.Parse(info.total_lots);
                        break;
                    case "Y":
                        hdbCarPark!.Lots.HeavyVehicle.Available = int.Parse(info.lots_available);
                        hdbCarPark.Lots.HeavyVehicle.Total = int.Parse(info.total_lots);
                        break;
                    case "M":
                        hdbCarPark!.Lots.Motorcycle.Available = int.Parse(info.lots_available);
                        hdbCarPark.Lots.Motorcycle.Total = int.Parse(info.total_lots);
                        break;
                }
            }
        }

        return hdbCarParks;
    }

    private static void InitializeStaticDataNotAvailableCarparks(ref HdbCarParkModel newHdbCarPark, string carparkCode)
    {
        newHdbCarPark.CarparkCode = carparkCode;
        newHdbCarPark.Name = NotAvailable;
        newHdbCarPark.System = NotAvailable;
        newHdbCarPark.Coordinates = new LatLong();
        newHdbCarPark.ShortTermParking = NotAvailable;
        newHdbCarPark.FreeParking = NotAvailable;
        newHdbCarPark.NightParking = NotAvailable;
        newHdbCarPark.CarParkDecks = 0;
        newHdbCarPark.GantryHeight = 0.0f;
        newHdbCarPark.CarParkBasement = NotAvailable;
        newHdbCarPark.Lots = new Lots();
    }

    private async Task<LatLong?> ConvertSvy21ToLatLong(double x, double y)
    {
        var requestUri = $"https://developers.onemap.sg/commonapi/convert/3414to4326?X={x}&Y={y}";
        var latLong = await _client.GetFromJsonAsync<LatLong>(requestUri);

        return latLong;
    }

    private async Task<List<GovStaticRecord?>> GetStaticHdbCarParkRecord(string Id)
    {
        var requestUri = _configuration["GOV_HDB_CARPARK_RATES_API"] + Id;
        var result = await _client.GetFromJsonAsync<GovStaticRoot>(requestUri);

        return result?.result.records;
    }
}