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
    private readonly ICarparkRepository _mallCarparkRepository;
    private readonly IUraCarparkRepository _uraCarparkRepository;
    
    private const int GoogleBatchWriteLimit = 500;
    private readonly IConfiguration _configuration;
    private readonly IUpdateLiveCarparkDataService<MallCarparkModel, LtaLiveCarparkValue> _updateLiveMallCarparkDataService;

    public LiveUpdateController(
        ILogger<DataController> logger,
        IConfiguration configuration,
        IHdbCarparkRepository hdbCarparkRepository,
        ICarparkRepository mallCarparkRepository,
        IUraCarparkRepository uraCarparkRepository,
        IUpdateLiveCarparkDataService<MallCarparkModel, LtaLiveCarparkValue> updateLiveMallCarparkDataService)
    {
        _configuration = configuration;
        _logger = logger;
        _client = new HttpClient();
        
        _hdbCarparkRepository = hdbCarparkRepository;
        _mallCarparkRepository = mallCarparkRepository;
        _uraCarparkRepository = uraCarparkRepository;

        _updateLiveMallCarparkDataService = updateLiveMallCarparkDataService;
    }

    [HttpGet]
    [Route("LiveUpdateMallData")]
    public async Task<ActionResult> LiveUpdateMallData()
    {
        var mallCarParks = await _mallCarparkRepository.GetAllAsync<MallCarparkModel>();
        
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
}