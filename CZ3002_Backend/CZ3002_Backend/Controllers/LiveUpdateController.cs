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
    
    private readonly IDataSetUpService<HdbCarParkModel, GovLiveCarparkDatum> _hdbDataSetUpService;
    private readonly IDataSetUpService<MallCarparkModel, LtaLiveCarparkValue> _mallDataSetUpService;
    private readonly IDataSetUpService<UraCarparkModel, UraLiveResult> _uraDataSetUpService;

    private const int GoogleBatchWriteLimit = 500;
    private readonly IConfiguration _configuration;
    
    public LiveUpdateController(
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
            foreach (var mallCarpark in mallCarParks)
            {
                if (liveMallCarparks.Exists(cp => cp.CarParkID == mallCarpark.CarparkCode))
                {
                    var liveMallCarpark = liveMallCarparks.Find(cp => cp.CarParkID == mallCarpark.CarparkCode);

                    switch (liveMallCarpark?.LotType)
                    {
                        case "C":
                            mallCarpark.Lots.Car.Available = liveMallCarpark.AvailableLots;
                            break;
                        case "Y":
                            mallCarpark.Lots.Motorcycle.Available = liveMallCarpark.AvailableLots;
                            break;
                        case "H":
                            mallCarpark.Lots.HeavyVehicle.Available = liveMallCarpark.AvailableLots;
                            break;
                    }
                }
            }
            
            var totalNumOfCarparks = mallCarParks.Count;
            var carparkCount = 0;
            List<Lots> lots = new List<Lots>();
            List<string> Ids = new List<string>();
            
            foreach(var carpark in mallCarParks)
            {
                lots.Add(carpark.Lots);
                Ids.Add(carpark.Id);
            }

            while (carparkCount < totalNumOfCarparks)
            {
                // Console.WriteLine("carpark count: " + carparkCount);
                List<Lots> partitionedLots;
                List<string> partitionedIds;
                if (carparkCount + GoogleBatchWriteLimit > totalNumOfCarparks - 1)
                {
                    partitionedLots = lots.GetRange(carparkCount, totalNumOfCarparks - carparkCount);
                    partitionedIds = Ids.GetRange(carparkCount, totalNumOfCarparks - carparkCount);
                    carparkCount += (totalNumOfCarparks - carparkCount);
                }
                else
                {
                    partitionedLots = lots.GetRange(carparkCount, GoogleBatchWriteLimit);
                    partitionedIds = Ids.GetRange(carparkCount, GoogleBatchWriteLimit);
                    carparkCount += GoogleBatchWriteLimit;
                }

                await _mallCarparkRepository.UpdateMultipleAsync(partitionedLots, partitionedIds, "Lots");
            }
        }
        
        return Ok();
    }
}