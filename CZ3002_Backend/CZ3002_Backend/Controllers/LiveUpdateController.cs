using CZ3002_Backend.Models;
using CZ3002_Backend.Repo;
using CZ3002_Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace CZ3002_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class LiveUpdateController : ControllerBase
{
    private readonly ILogger<DataController> _logger;
    private readonly HttpClient _client;

    private readonly IHdbCarparkRepository _hdbCarparkRepository;
    private readonly IMallCarparkRepository _mallCarparkRepository;
    private readonly IUraCarparkRepository _uraCarparkRepository;

    private readonly IConfiguration _configuration;

    private readonly IUpdateLiveCarparkDataService<MallCarparkModel, LtaLiveCarparkValue> _updateLiveMallCarparkDataService;
    private readonly IUpdateLiveCarparkDataService<UraCarparkModel, UraLiveResult> _updateLiveUraCarparkDataService;
    private readonly IUpdateLiveCarparkDataService<HdbCarParkModel, GovLiveCarparkDatum> _updateLiveHdbCarparkDataService;

    public LiveUpdateController(
        ILogger<DataController> logger,
        IConfiguration configuration,
        IHdbCarparkRepository hdbCarparkRepository,
        IMallCarparkRepository mallCarparkRepository,
        IUraCarparkRepository uraCarparkRepository,
        IUpdateLiveCarparkDataService<MallCarparkModel, LtaLiveCarparkValue> updateLiveMallCarparkDataService,
        IUpdateLiveCarparkDataService<UraCarparkModel, UraLiveResult> updateLiveUraCarparkDataService,
        IUpdateLiveCarparkDataService<HdbCarParkModel, GovLiveCarparkDatum> updateLiveHdbCarparkDataService)
    {
        _configuration = configuration;
        _logger = logger;
        _client = new HttpClient();

        _hdbCarparkRepository = hdbCarparkRepository;
        _mallCarparkRepository = mallCarparkRepository;
        _uraCarparkRepository = uraCarparkRepository;

        _updateLiveMallCarparkDataService = updateLiveMallCarparkDataService;
        _updateLiveUraCarparkDataService = updateLiveUraCarparkDataService;
        _updateLiveHdbCarparkDataService = updateLiveHdbCarparkDataService;
    }

    [HttpGet]
    [Route("LiveUpdateMallData")]
    public async Task<ActionResult> LiveUpdateMallData()
    {
        var mallCarParks = await _mallCarparkRepository.GetAllAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, _configuration["LTA_CARPARK_AVAILABILITY_API"]);
        request.Headers.Add("AccountKey", _configuration["CarParkWhere:LtaAccountKey"]);
        request.Headers.Add("accept", "application/json");

        var response = await _client.SendAsync(request);
        var results = await response.Content.ReadFromJsonAsync<LtaLiveCarparkRoot>();

        var liveMallCarparks = results?.value.FindAll(x => x.Agency == "LTA");

        if (liveMallCarparks != null)
        {
            await _updateLiveMallCarparkDataService.UpdateData(mallCarParks, liveMallCarparks);
        }

        return Ok();
    }

    [HttpGet]
    [Route("LiveUpdateUraData")]
    public async Task<ActionResult> LiveUpdateUraData()
    {
        var uraCarParks = await _uraCarparkRepository.GetAllAsync();
        
        var token = await GetUraToken(); 
        var request = new HttpRequestMessage(HttpMethod.Get, _configuration["URA_CARPARK_AVAILABILITY_API"]); 
        request.Headers.Add("AccessKey", _configuration["CarParkWhere:UraAccessKey"]); 
        request.Headers.Add("Token", token); 
        request.Headers.Add("User-Agent", "Mozilla/5.0");
        
        var response = await _client.SendAsync(request);
        var results = await response.Content.ReadFromJsonAsync<UraLiveRoot>();
        
        var uraLiveCarparks = results?.Result;

        if (uraLiveCarparks != null)
        {
            await _updateLiveUraCarparkDataService.UpdateData(uraCarParks, uraLiveCarparks);
        }

        return Ok();
    }
    
    [HttpGet]
    [Route("LiveUpdateHdbData")]
    public async Task<ActionResult> LiveUpdateHdbData()
    {
        var hdbCarParks = await _hdbCarparkRepository.GetAllAsync();
        
        var liveHdbResults = await _client.GetFromJsonAsync<GovLiveRoot>(_configuration["GOV_CARPARK_AVAILABILITY_API"]);
        var liveHdbCarparks = liveHdbResults?.items[0].carpark_data;

        if (liveHdbCarparks != null)
        {
            await _updateLiveHdbCarparkDataService.UpdateData(hdbCarParks, liveHdbCarparks);
        }
        
        return Ok();
    }
    
    private async Task<string?> GetUraToken()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _configuration["URA_GET_DAILY_TOKEN"]);
        request.Headers.Add("AccessKey", _configuration["CarParkWhere:UraAccessKey"]);
        
        var response = await _client.SendAsync(request);
        var results = await response.Content.ReadFromJsonAsync<UraTokenRoot>();
        
        return results?.Result;
    }
}