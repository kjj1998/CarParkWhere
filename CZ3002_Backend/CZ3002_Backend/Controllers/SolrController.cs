using CZ3002_Backend.Models;
using CZ3002_Backend.Repo;
using Microsoft.AspNetCore.Mvc;

namespace CZ3002_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class SolrController : ControllerBase
{
    private readonly ILogger<DataController> _logger;

    private readonly IHdbCarparkRepository _hdbCarparkRepository;
    private readonly IMallCarparkRepository _mallCarparkRepository;
    private readonly IUraCarparkRepository _uraCarparkRepository;

    private readonly ISolrRepository _solrRepository;

    public SolrController(
        ILogger<DataController> logger,
        IConfiguration configuration,
        IHdbCarparkRepository hdbCarparkRepository,
        IMallCarparkRepository mallCarparkRepository,
        IUraCarparkRepository uraCarparkRepository,
        ISolrRepository solrRepository)
    {
        _logger = logger;

        _hdbCarparkRepository = hdbCarparkRepository;
        _mallCarparkRepository = mallCarparkRepository;
        _uraCarparkRepository = uraCarparkRepository;

        _solrRepository = solrRepository;
    }
    
    [HttpGet]
    [Route("ConfigureCarparkDataInSolr")]
    public async Task<ActionResult> ConfigureCarparkDataInSolr()
    {
        await _solrRepository.AddField("name", "text_general");
        await _solrRepository.AddField("carparkcode", "text_general");
        await _solrRepository.AddField("type", "text_general");

        var copyFieldDestinations = new List<string> { "_text_" };
        await _solrRepository.AddCopyField("name", copyFieldDestinations);
        await _solrRepository.AddCopyField("carparkcode", copyFieldDestinations);

        return Ok();
    }

    [HttpGet]
    [Route("IndexDataIntoSolr")]
    public async Task<ActionResult> IndexDataIntoSolr()
    {
        var allHdbCarparks = await _hdbCarparkRepository.GetAllAsync();
        
        var hdbSolrCarparks = 
            allHdbCarparks.Select(carpark => 
                new CarparkSolrIndex()
                {
                    id = carpark.Id, carparkcode = carpark.CarparkCode, name = carpark.Name, type = "HDB"
                }).ToList();

        var allMallCarparks = await _mallCarparkRepository.GetAllAsync();
        var mallSolrCarparks = 
            allMallCarparks.Select(carpark => 
                new CarparkSolrIndex()
                {
                    id = carpark.Id, carparkcode = carpark.CarparkCode, name = carpark.Name, type = "Mall"
                }).ToList();
        
        var allUraCarparks = await _uraCarparkRepository.GetAllAsync();
        var uraSolrCarparks = 
            allUraCarparks.Select(carpark => 
                new CarparkSolrIndex()
                {
                    id = carpark.Id, carparkcode = carpark.CarparkCode, name = carpark.Name, type = "URA"
                }).ToList();

        await _solrRepository.AddMultipleDocs(hdbSolrCarparks);
        await _solrRepository.AddMultipleDocs(mallSolrCarparks);
        await _solrRepository.AddMultipleDocs(uraSolrCarparks);
        
        return Ok();
    }

    [HttpGet]
    [Route("DeleteAllDataInSolr")]
    public async Task<ActionResult> DeleteAllDataInSolr()
    {
        await _solrRepository.DeleteAllDocs();

        return Ok();
    }
}