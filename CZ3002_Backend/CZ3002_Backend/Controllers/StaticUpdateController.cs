using CZ3002_Backend.Models;
using CZ3002_Backend.Repo;
using Microsoft.AspNetCore.Mvc;

namespace CZ3002_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class StaticUpdateController : ControllerBase
{
    private ILogger<StaticUpdateController> _logger;
    private IConfiguration _configuration;
    private IMallCarparkRepository _mallCarparkRepository;
    private readonly HttpClient _client;

    public StaticUpdateController(
        ILogger<StaticUpdateController> logger, 
        IConfiguration configuration,
        IMallCarparkRepository mallCarparkRepository)
    {
        _logger = logger;
        _configuration = configuration;
        _mallCarparkRepository = mallCarparkRepository;
        _client = new HttpClient();
    }

    [HttpGet]
    [Route("StaticUpdateMallData")]
    public async Task<ActionResult> StaticUpdateMallData()
    {
        try
        {
            var mallCarParks = await _mallCarparkRepository.GetAllAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, _configuration["LTA_CARPARK_AVAILABILITY_API"]);
            request.Headers.Add("AccountKey", _configuration["CarParkWhere:LtaAccountKey"]);
            request.Headers.Add("accept", "application/json");

            var response = await _client.SendAsync(request);
            var results = await response.Content.ReadFromJsonAsync<LtaLiveCarparkRoot>();

            var liveMallCarparks = results?.value.FindAll(x => x.Agency == "LTA");


            var existingMallCarParks = new List<MallCarparkModel?>();

            // foreach (var liveMallCarpark in liveMallCarparks)
            // {
            //     if (mallCarParks.Exists(cp => cp.CarparkCode == liveMallCarpark.CarParkID))
            //     {
            //         existingMallCarParks.Add(mallCarParks.Find(cp => cp.CarparkCode == liveMallCarpark.CarParkID));
            //     }
            // }
            
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return BadRequest(e.ToString());
        }
    }
}