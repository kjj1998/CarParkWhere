using CZ3002_Backend.Models;
using CZ3002_Backend.Repo;
using CZ3002_Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace CZ3002_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class SolrController : ControllerBase
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
    private ISolrRepository _hdbSolrRepository;

    public SolrController(
        ILogger<DataController> logger,
        IConfiguration configuration,
        IHdbCarparkRepository hdbCarparkRepository,
        IMallCarparkRepository mallCarparkRepository,
        IUraCarparkRepository uraCarparkRepository,
        IDataSetUpService<HdbCarParkModel, GovLiveCarparkDatum> hdbDataSetUpService,
        IDataSetUpService<MallCarparkModel, LtaLiveCarparkValue> mallDataSetUpService,
        IDataSetUpService<UraCarparkModel, UraLiveResult> uraDataSetUpService,
        ISolrRepository hdbSolrRepository)
    {
        _configuration = configuration;
        _logger = logger;
        _client = new HttpClient();
        
        _hdbCarparkRepository = hdbCarparkRepository;
        _mallCarparkRepository = mallCarparkRepository;
        _uraCarparkRepository = uraCarparkRepository;

        _hdbSolrRepository = hdbSolrRepository;

        _hdbDataSetUpService = hdbDataSetUpService;
        _mallDataSetUpService = mallDataSetUpService;
        _uraDataSetUpService = uraDataSetUpService;
    }
    
    [HttpGet]
    [Route("ConfigureHdbDataInSolr")]
    public async Task<ActionResult> ConfigureHdbDataInSolr()
    {
        await _hdbSolrRepository.AddField("name", "text_general");
        await _hdbSolrRepository.AddField("carparkcode", "text_general");

        var copyFieldDestinations = new List<string> { "_text_" };
        await _hdbSolrRepository.AddCopyField("name", copyFieldDestinations);
        await _hdbSolrRepository.AddCopyField("carparkcode", copyFieldDestinations);

        return Ok();
    }
    
    
}