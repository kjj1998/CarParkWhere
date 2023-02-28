using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using CZ3002_Backend.Models;
using CZ3002_Backend.Repo;
using CZ3002_Backend.Services;

namespace CZ3002_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase
{
    private readonly ILogger<DataController> _logger;
    private readonly HttpClient _client;
    private readonly IHdbCarparkRepository _repository;
    private readonly IDataSetUpService<HdbCarParkModel, GovLiveCarparkDatum> _hdbDataSetUpService;

    public DataController(
        ILogger<DataController> logger, 
        IHdbCarparkRepository repository, 
        IDataSetUpService<HdbCarParkModel, GovLiveCarparkDatum> hdbDataSetUpService)
    {
        _logger = logger;
        _client = new HttpClient();
        _repository = repository;
        _hdbDataSetUpService = hdbDataSetUpService;
    }
    
    [HttpGet]
    [Route("SetUpHdbStaticData")]
    public async Task<ActionResult> SetUpHdbStaticData()
    {
        
        var liveHdbResults = await _client.GetFromJsonAsync<GovLiveRoot>("https://api.data.gov.sg/v1/transport/carpark-availability");
        var carparks = liveHdbResults?.items[0].carpark_data.GetRange(0, 10);
        var hdbCarParks = await _hdbDataSetUpService.SetUp(carparks);
        
        await _repository.AddMultipleAsync(hdbCarParks);
        
        return Ok();
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