using System.Globalization;
using CZ3002_Backend.Models;

namespace CZ3002_Backend.Services;

public class HdbCarparkDataSetUpService : IDataSetUpService<HdbCarParkModel, GovLiveCarparkDatum>
{
    private readonly HttpClient _client;

    public HdbCarparkDataSetUpService()
    {
        _client = new HttpClient();
    }

    public async Task<List<HdbCarParkModel>> SetUp(List<GovLiveCarparkDatum>? carparks)
    {
        var hdbCarParks = new List<HdbCarParkModel>();

        if (carparks == null) return hdbCarParks;
        
        foreach (var carpark in carparks)
        {
            if (!hdbCarParks.Exists(x => x.CarparkCode == carpark.carpark_number))
            {
                var newHdbCarPark = new HdbCarParkModel();
                var staticRecord = await GetStaticHdbCarParkRecord(carpark.carpark_number);
                if (staticRecord != null && staticRecord.car_park_no == carpark.carpark_number)
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
    
    private async Task<LatLong?> ConvertSvy21ToLatLong(double x, double y)
    {
        var requestUri = $"https://developers.onemap.sg/commonapi/convert/3414to4326?X={x}&Y={y}";
        var latLong = await _client.GetFromJsonAsync<LatLong>(requestUri);

        return latLong;
    }

    private async Task<GovStaticRecord?> GetStaticHdbCarParkRecord(string Id)
    {
        var requestUri =
            $"https://data.gov.sg/api/action/datastore_search?resource_id=139a3035-e624-4f56-b63f-89ae28d4ae4c&q={Id}";
        var result = await _client.GetFromJsonAsync<GovStaticRoot>(requestUri);

        return result?.result.records[0];
    }
}