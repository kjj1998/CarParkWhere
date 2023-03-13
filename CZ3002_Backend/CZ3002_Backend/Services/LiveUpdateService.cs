using CZ3002_Backend.Models;
using CZ3002_Backend.Repo;

namespace CZ3002_Backend.Services;

public class LiveUpdateService : ILiveUpdateService
{
    private readonly HttpClient _client;
    private readonly IHdbCarparkRepository _hdbCarparkRepository;
    private readonly IMallCarparkRepository _mallCarparkRepository;
    private readonly IUraCarparkRepository _uraCarparkRepository;
    private readonly IConfiguration _configuration;
    private readonly IUpdateLiveCarparkDataService<MallCarparkModel, LtaLiveCarparkValue> _updateLiveMallCarparkDataService;
    private readonly IUpdateLiveCarparkDataService<UraCarparkModel, UraLiveResult> _updateLiveUraCarparkDataService;
    private readonly IUpdateLiveCarparkDataService<HdbCarParkModel, GovLiveCarparkDatum> _updateLiveHdbCarparkDataService;

    public LiveUpdateService(IConfiguration configuration,IHdbCarparkRepository hdbCarparkRepository,
        IMallCarparkRepository mallCarparkRepository,
        IUraCarparkRepository uraCarparkRepository,
        IUpdateLiveCarparkDataService<MallCarparkModel, LtaLiveCarparkValue> updateLiveMallCarparkDataService,
        IUpdateLiveCarparkDataService<UraCarparkModel, UraLiveResult> updateLiveUraCarparkDataService,
        IUpdateLiveCarparkDataService<HdbCarParkModel, GovLiveCarparkDatum> updateLiveHdbCarparkDataService)
    {
        _client = new HttpClient();
        
        _configuration = configuration;
        _hdbCarparkRepository = hdbCarparkRepository;
        _mallCarparkRepository = mallCarparkRepository;
        _uraCarparkRepository = uraCarparkRepository;
        _updateLiveMallCarparkDataService = updateLiveMallCarparkDataService;
        _updateLiveUraCarparkDataService = updateLiveUraCarparkDataService;
        _updateLiveHdbCarparkDataService = updateLiveHdbCarparkDataService;
    }

    public async Task MallLiveUpdate()
    {
        var mallCarParks = await _mallCarparkRepository.GetAllAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, _configuration["LTA_CARPARK_AVAILABILITY_API"]);
        request.Headers.Add("AccountKey", _configuration["LtaAccountKey"]);
        request.Headers.Add("accept", "application/json");

        var response = await _client.SendAsync(request);
        var results = await response.Content.ReadFromJsonAsync<LtaLiveCarparkRoot>();

        var liveMallCarparks = results?.value.FindAll(x => x.Agency == "LTA");

        if (liveMallCarparks != null)
        {
            await _updateLiveMallCarparkDataService.UpdateData(mallCarParks, liveMallCarparks);
        }
    }

    public async Task UraLiveUpdate()
    {
        var uraCarParks = await _uraCarparkRepository.GetAllAsync();

        var token = await GetUraToken();
        var request = new HttpRequestMessage(HttpMethod.Get, _configuration["URA_CARPARK_AVAILABILITY_API"]);
        request.Headers.Add("AccessKey", _configuration["UraAccessKey"]);
        request.Headers.Add((string)"Token", (string?)token);
        request.Headers.Add("User-Agent", "Mozilla/5.0");

        var response = await _client.SendAsync(request);
        var results = await response.Content.ReadFromJsonAsync<UraLiveRoot>();

        var uraLiveCarparks = results?.Result;

        if (uraLiveCarparks != null)
        {
            await _updateLiveUraCarparkDataService.UpdateData(uraCarParks, uraLiveCarparks);
        }
    }

    public async Task HdbLiveUpdate()
    {
        var hdbCarParks = await _hdbCarparkRepository.GetAllAsync();

        var liveHdbResults = await _client.GetFromJsonAsync<GovLiveRoot>(_configuration["GOV_CARPARK_AVAILABILITY_API"]);
        var liveHdbCarparks = liveHdbResults?.items[0].carpark_data;

        if (liveHdbCarparks != null)
        {
            await _updateLiveHdbCarparkDataService.UpdateData(hdbCarParks, liveHdbCarparks);
        }
    }

    private async Task<string?> GetUraToken()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _configuration["URA_GET_DAILY_TOKEN"]);
        request.Headers.Add("AccessKey", _configuration["UraAccessKey"]);
        
        var response = await _client.SendAsync(request);
        var results = await response.Content.ReadFromJsonAsync<UraTokenRoot>();
        
        return results?.Result;
    }
}