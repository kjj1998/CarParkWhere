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
    private readonly IHdbCarparkRepository _hdbCarparkRepository;
    private readonly IDataSetUpService<HdbCarParkModel, GovLiveCarparkDatum> _hdbDataSetUpService;
    private IDataSetUpService<MallCarparkModel, LtaLiveCarparkValue> _mallDataSetUpService;
    private IMallCarparkRepository _mallCarparkRepository;

    private int _googleBatchWriteLimit = 500;
    private IConfiguration _configuration;

    public DataController(
        ILogger<DataController> logger,
        IConfiguration configuration,
        IHdbCarparkRepository hdbCarparkRepository,
        IMallCarparkRepository mallCarparkRepository,
        IDataSetUpService<HdbCarParkModel, GovLiveCarparkDatum> hdbDataSetUpService,
        IDataSetUpService<MallCarparkModel, LtaLiveCarparkValue> mallDataSetUpService)
    {
        _configuration = configuration;
        _logger = logger;
        _client = new HttpClient();
        _hdbCarparkRepository = hdbCarparkRepository;
        _mallCarparkRepository = mallCarparkRepository;

        _hdbDataSetUpService = hdbDataSetUpService;
        _mallDataSetUpService = mallDataSetUpService;
        
    }
    
    [HttpGet]
    [Route("SetUpHdbStaticData")]
    public async Task<ActionResult> SetUpHdbStaticData()
    {
        var liveHdbResults = await _client.GetFromJsonAsync<GovLiveRoot>(_configuration["GOV_CARPARK_AVAILABILITY_API"]);
        
        var carparks = liveHdbResults?.items[0].carpark_data;
        var hdbCarParks = await _hdbDataSetUpService.SetUp(carparks);

        var totalNumOfCarparks = hdbCarParks.Count;
        var carparkCount = 0;

        while (carparkCount < totalNumOfCarparks)
        {
            // Console.WriteLine("carpark count: " + carparkCount);
            List<HdbCarParkModel> partitionedCarparks;
            if (carparkCount + _googleBatchWriteLimit > totalNumOfCarparks - 1)
            {
                partitionedCarparks = hdbCarParks.GetRange(carparkCount, totalNumOfCarparks - carparkCount);
                carparkCount += (totalNumOfCarparks - carparkCount);
            }
            else
            {
                partitionedCarparks = hdbCarParks.GetRange(carparkCount, _googleBatchWriteLimit);
                carparkCount += _googleBatchWriteLimit;
            }
            
            await _hdbCarparkRepository.AddMultipleAsync(partitionedCarparks);
        }
        
        return Ok();
    }

    [HttpGet]
    [Route("SetUpMallStaticData")]
    public async Task<ActionResult> SetUpMallStaticData()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _configuration["LTA_CARPARK_AVAILABILITY_API"]);
        request.Headers.Add("AccountKey", _configuration["LTA_ACCOUNT_KEY"]);
        request.Headers.Add("accept", "application/json");

        var response = await _client.SendAsync(request);
        var results = await response.Content.ReadFromJsonAsync<LtaLiveCarparkRoot>();

        var ltaCarparks = results?.value.FindAll(x => x.Agency == "LTA");
        var mallCarParks = await _mallDataSetUpService.SetUp(ltaCarparks);

        await _mallCarparkRepository.AddMultipleAsync(mallCarParks);

        return Ok();
    }
}