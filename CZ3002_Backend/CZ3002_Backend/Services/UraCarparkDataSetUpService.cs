using System.Globalization;
using CZ3002_Backend.Models;

namespace CZ3002_Backend.Services;

public class UraCarparkDataSetUpService : IDataSetUpService<UraCarparkModel, UraLiveResult>
{
    private IConfiguration _configuration;
    private readonly HttpClient _client;

    public UraCarparkDataSetUpService(IConfiguration configuration)
    {
        _configuration = configuration;
        _client = new HttpClient();
    }
    
    public async Task<List<UraCarparkModel>> SetUp(List<UraLiveResult>? carparks)
    {
        var uraCarParks = new List<UraCarparkModel>();

        if (carparks == null)
            return uraCarParks;
        
        // var count = 1;
        var token = await GetUraToken();
        var staticRecords = await GetStaticUraCarParkRecords(token);

        foreach (var carpark in carparks)
        {
            // Console.WriteLine($"{count++}. Carpark Id = {carpark.carparkNo}");

            if (staticRecords != null)
            {
                foreach (var staticRecord in staticRecords)
                {
                    if (staticRecord.ppCode == carpark.carparkNo)
                    {
                        if (!uraCarParks.Exists(x => x.CarparkCode == carpark.carparkNo))
                        {
                            var newUraCarPark = new UraCarparkModel
                            {
                                CarparkCode = carpark.carparkNo,
                                Name = staticRecord.ppName,
                                System = staticRecord.parkingSystem == "B" ? "ELECTRONIC" : "COUPON"
                            };

                            var coordinates = staticRecord.geometries[0].coordinates.Split(",");
                            var latLong = await ConvertSvy21ToLatLong(
                                double.Parse(coordinates[0], CultureInfo.InvariantCulture),
                                double.Parse(coordinates[1], CultureInfo.InvariantCulture));
                            
                            newUraCarPark.Coordinates = latLong;
                            newUraCarPark.Lots = new Lots();
                            newUraCarPark.Rates = new List<CarparkRate>();

                            uraCarParks.Add(newUraCarPark);
                        }
                        
                        var existingUraCarPark = uraCarParks.Find(x => x.CarparkCode == carpark.carparkNo);
                        var carparkRate = new CarparkRate
                        {
                            StartTime = staticRecord.startTime,
                            EndTime = staticRecord.endTime,
                            WeekDayRate = staticRecord.weekdayRate + " per " + staticRecord.weekdayMin,
                            SatRate = staticRecord.satdayRate + " per " + staticRecord.satdayMin,
                            SunPhRate = staticRecord.sunPHRate + " per " + staticRecord.sunPHMin,
                            Remarks = staticRecord.remarks,
                            VehicleCategory = staticRecord.vehCat,
                        };
                        existingUraCarPark?.Rates.Add(carparkRate);
                        switch (staticRecord.vehCat)
                        {
                            case "Car":
                                existingUraCarPark!.Lots.Car.Total = staticRecord.parkCapacity;
                                break;
                            case "Motorcycle":
                                existingUraCarPark!.Lots.Motorcycle.Total = staticRecord.parkCapacity;
                                break;
                            case "Heavy Vehicle":
                                existingUraCarPark!.Lots.HeavyVehicle.Total = staticRecord.parkCapacity;
                                break;
                        }
                        
                    }
                }
            }

            var uraCarPark = uraCarParks.Find(x => x.CarparkCode == carpark.carparkNo);
            switch (carpark.lotType)
            {
                case "C":
                    uraCarPark!.Lots.Car.Available = int.Parse(carpark.lotsAvailable);
                    break;
                case "M":
                    uraCarPark!.Lots.Motorcycle.Total = int.Parse(carpark.lotsAvailable);
                    break;
                case "H":
                    uraCarPark!.Lots.HeavyVehicle.Total = int.Parse(carpark.lotsAvailable);
                    break;
            }
        }
        return uraCarParks;
    }
    
    private async Task<string?> GetUraToken()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _configuration["URA_GET_DAILY_TOKEN"]);
        request.Headers.Add("AccessKey", _configuration["CarParkWhere:UraAccessKey"]);
        
        var response = await _client.SendAsync(request);
        var results = await response.Content.ReadFromJsonAsync<UraTokenRoot>();
        
        return results?.Result;
    }
    
    private async Task<List<UraStaticResult>?> GetStaticUraCarParkRecords(string? token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _configuration["URA_CARPARK_RATES_API"]);
        request.Headers.Add("AccessKey", _configuration["CarParkWhere:UraAccessKey"]);
        request.Headers.Add("Token", token);
        request.Headers.Add("User-Agent", "Mozilla/5.0");
        
        var response = await _client.SendAsync(request);
        var results = await response.Content.ReadFromJsonAsync<UraStaticRoot>();

        return results?.Result;
    }
    
    private async Task<LatLong?> ConvertSvy21ToLatLong(double x, double y)
    {
        var requestUri = $"https://developers.onemap.sg/commonapi/convert/3414to4326?X={x}&Y={y}";
        var latLong = await _client.GetFromJsonAsync<LatLong>(requestUri);

        return latLong;
    }
}