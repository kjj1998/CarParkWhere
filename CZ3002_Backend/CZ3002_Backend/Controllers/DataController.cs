using Microsoft.AspNetCore.Mvc;
using CZ3002_Backend.Models;
using CZ3002_Backend.Repo;
using CZ3002_Backend.Services;
using RestSharp;

namespace CZ3002_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase
{
    private readonly ILogger<DataController> _logger;
    private readonly HttpClient _client;
    
    private readonly IHdbCarparkRepository _hdbCarparkRepository;
    private readonly IMallCarparkRepository _mallCarparkRepository;
    private readonly IUraCarparkRepository _uraCarparkRepository;
    
    private readonly IDataSetUpService<HdbCarParkModel, GovLiveCarparkDatum> _hdbDataSetUpService;
    private readonly IDataSetUpService<MallCarparkModel, LtaLiveCarparkValue> _mallDataSetUpService;
    private readonly IDataSetUpService<UraCarparkModel, UraLiveResult> _uraDataSetUpService;

    private const int GoogleBatchWriteLimit = 500;
    private readonly IConfiguration _configuration;
    
    public DataController(
        ILogger<DataController> logger,
        IConfiguration configuration,
        IHdbCarparkRepository hdbCarparkRepository,
        IMallCarparkRepository mallCarparkRepository,
        IUraCarparkRepository uraCarparkRepository,
        IDataSetUpService<HdbCarParkModel, GovLiveCarparkDatum> hdbDataSetUpService,
        IDataSetUpService<MallCarparkModel, LtaLiveCarparkValue> mallDataSetUpService,
        IDataSetUpService<UraCarparkModel, UraLiveResult> uraDataSetUpService)
    {
        _configuration = configuration;
        _logger = logger;
        _client = new HttpClient();
        
        _hdbCarparkRepository = hdbCarparkRepository;
        _mallCarparkRepository = mallCarparkRepository;
        _uraCarparkRepository = uraCarparkRepository;

        _hdbDataSetUpService = hdbDataSetUpService;
        _mallDataSetUpService = mallDataSetUpService;
        _uraDataSetUpService = uraDataSetUpService;
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
            if (carparkCount + GoogleBatchWriteLimit > totalNumOfCarparks - 1)
            {
                partitionedCarparks = hdbCarParks.GetRange(carparkCount, totalNumOfCarparks - carparkCount);
                carparkCount += (totalNumOfCarparks - carparkCount);
            }
            else
            {
                partitionedCarparks = hdbCarParks.GetRange(carparkCount, GoogleBatchWriteLimit);
                carparkCount += GoogleBatchWriteLimit;
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
        request.Headers.Add("AccountKey", _configuration["CarParkWhere:LtaAccountKey"]);
        request.Headers.Add("accept", "application/json");

        var response = await _client.SendAsync(request);
        var results = await response.Content.ReadFromJsonAsync<LtaLiveCarparkRoot>();

        var ltaCarparks = results?.value.FindAll(x => x.Agency == "LTA");
        var mallCarParks = await _mallDataSetUpService.SetUp(ltaCarparks);

        await _mallCarparkRepository.AddMultipleAsync(mallCarParks);

        return Ok();
    }

    [HttpGet]
    [Route("SetUpUraStaticData")]
    public async Task<ActionResult> SetUpUraStaticData()
    {
        var token = await GetUraToken(); 
        var request = new HttpRequestMessage(HttpMethod.Get, "https://www.ura.gov.sg/uraDataService/invokeUraDS?service=Car_Park_Availability"); 
        request.Headers.Add("AccessKey", "cf15fba1-2816-4512-9f62-2a8634dc2494"); 
        request.Headers.Add("Token", "f79+gW9-Qfer48cfJ5-n1WXt2gXT-pFaWg55dcA8Cmaa-FVp6QE2zG3T1f6Wfv24uSe57XKD38T6398APaac+9A17d89X-422wqJ"); 
        request.Headers.Add("User-Agent", "Mozilla/5.0");
        
        var response = await _client.SendAsync(request);

        var results = await response.Content.ReadFromJsonAsync<UraLiveRoot>();

        var uraLiveCarparks = results?.Result;
        var uraStaticCarparks = await _uraDataSetUpService.SetUp(uraLiveCarparks);
        
        var totalNumOfCarparks = uraStaticCarparks.Count;
        var carparkCount = 0;
        
        while (carparkCount < totalNumOfCarparks)
        {
            // Console.WriteLine("carpark count: " + carparkCount);
            List<UraCarparkModel> partitionedCarparks;
            if (carparkCount + GoogleBatchWriteLimit > totalNumOfCarparks - 1)
            {
                partitionedCarparks = uraStaticCarparks.GetRange(carparkCount, totalNumOfCarparks - carparkCount);
                carparkCount += (totalNumOfCarparks - carparkCount);
            }
            else
            {
                partitionedCarparks = uraStaticCarparks.GetRange(carparkCount, GoogleBatchWriteLimit);
                carparkCount += GoogleBatchWriteLimit;
            }
            
            await _uraCarparkRepository.AddMultipleAsync(partitionedCarparks);
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