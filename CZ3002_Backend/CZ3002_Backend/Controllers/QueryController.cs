using CZ3002_Backend.Models;
using CZ3002_Backend.Repo;
using Geohash;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace CZ3002_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class QueryController : ControllerBase
{
    private IMallCarparkRepository _mallCarparkRepository;
    private IHdbCarparkRepository _hdbCarparkRepository;
    private IUraCarparkRepository _uraCarparkRepository;
    private readonly HttpClient _client;
    private ILogger<QueryController> _logger;

    public QueryController(
        ILogger<QueryController> logger,
        IMallCarparkRepository mallCarparkRepository, 
        IHdbCarparkRepository hdbCarparkRepository, 
        IUraCarparkRepository uraCarparkRepository)
    {
        _logger = logger;
        _mallCarparkRepository = mallCarparkRepository;
        _hdbCarparkRepository = hdbCarparkRepository;
        _uraCarparkRepository = uraCarparkRepository;
        _client = new HttpClient();
    }

    [HttpPost]
    [Route("GetCarparkDataBasedOnCurrentLocation")]
    public async Task<ActionResult<FrontendCarparkModel>> GetCarparkDataBasedOnCurrentLocation(LatLong currentLocation)
    {
        try
        {
            var geoPointOfCurrentLocation = new GeoPoint(currentLocation.latitude, currentLocation.longitude);

            var nearbyMallCarparks =
                await _mallCarparkRepository.GetAllNearbyMallCarParkWithCoords(geoPointOfCurrentLocation);
            var nearbyHdbCarparks =
                await _hdbCarparkRepository.GetAllNearbyHdbCarParkWithCoords(geoPointOfCurrentLocation);
            var nearbyUraCarparks =
                await _uraCarparkRepository.GetAllNearbyUraCarParkWithCoords(geoPointOfCurrentLocation);

            var frontendCarparkModel = new FrontendCarparkModel(
                nearbyMallCarparks, nearbyHdbCarparks, nearbyUraCarparks, currentLocation);

            return frontendCarparkModel;
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return BadRequest(e.ToString());
        }
    }

    [HttpGet]
    [Route("GetCarparkDataBasedOnLocationSearchTerm")]
    public async Task<ActionResult<FrontendCarparkModel>> GetCarparkDataBasedOnLocationSearchTerm(string location)
    {
        try
        {
            var oneMapSearchResult = await OneMapLocationSearch(location);

            if (oneMapSearchResult is { found: 0 })
            {
                return NotFound("Location searched does not exist in Singapore");
            }

            var searchedLocationGeoPoint = new GeoPoint(
                double.Parse(oneMapSearchResult?.results?[0].LATITUDE!),
                double.Parse(oneMapSearchResult?.results?[0].LONGITUDE!));

            var searchedLocation = new LatLong
            {
                latitude = double.Parse(oneMapSearchResult?.results?[0].LATITUDE!),
                longitude = double.Parse(oneMapSearchResult?.results?[0].LONGITUDE!)
            };

            var nearbyMallCarparks =
                await _mallCarparkRepository.GetAllNearbyMallCarParkWithCoords(searchedLocationGeoPoint);
            var nearbyHdbCarparks =
                await _hdbCarparkRepository.GetAllNearbyHdbCarParkWithCoords(searchedLocationGeoPoint);
            var nearbyUraCarparks =
                await _uraCarparkRepository.GetAllNearbyUraCarParkWithCoords(searchedLocationGeoPoint);

            var frontendCarparkModel = new FrontendCarparkModel(
                nearbyMallCarparks, nearbyHdbCarparks, nearbyUraCarparks, searchedLocation);

            return frontendCarparkModel;
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return BadRequest(e.ToString());
        }
    }
    
    [HttpGet]
    [Route("GetPaginatedHdbCarparkData")]
    public async Task<ActionResult<PaginatedCarparkModel<HdbCarParkModel>>> GetPaginatedHdbCarparkData(int pageNumber, int pageSize, string? search = null)
    {
        try
        {
            var totalCountOfCarparks = await _hdbCarparkRepository.GetTotalNumberOfCarparks();
            var documentsToSkip = pageSize * (pageNumber - 1);

            var carparksReturned = await _hdbCarparkRepository.GetPaginatedCarparks(documentsToSkip, pageSize);

            var paginatedCarparks = new PaginatedCarparkModel<HdbCarParkModel>
            {
                Carparks = carparksReturned,
                TotalNumOfCarparks = totalCountOfCarparks == null ? 0 : (int) totalCountOfCarparks
            };

            return paginatedCarparks;
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return BadRequest(e.ToString());
        }
    }
    
    [HttpGet]
    [Route("GetPaginatedUraCarparkData")]
    public async Task<ActionResult<PaginatedCarparkModel<UraCarparkModel>>> GetPaginatedUraCarparkData(int pageNumber, int pageSize, string? search = null)
    {
        try
        {
            var totalCountOfCarparks = await _uraCarparkRepository.GetTotalNumberOfCarparks();
            var documentsToSkip = pageSize * (pageNumber - 1);

            var carparksReturned = await _uraCarparkRepository.GetPaginatedCarparks(documentsToSkip, pageSize);

            var paginatedCarparks = new PaginatedCarparkModel<UraCarparkModel>
            {
                Carparks = carparksReturned,
                TotalNumOfCarparks = totalCountOfCarparks == null ? 0 : (int) totalCountOfCarparks
            };

            return paginatedCarparks;
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return BadRequest(e.ToString());
        }
    }
    
    [HttpGet]
    [Route("GetPaginatedMallCarparkData")]
    public async Task<ActionResult<PaginatedCarparkModel<MallCarparkModel>>> GetPaginatedMallCarparkData(int pageNumber, int pageSize, string? search = null)
    {
        try
        {
            var totalCountOfCarparks = await _mallCarparkRepository.GetTotalNumberOfCarparks();
            var documentsToSkip = pageSize * (pageNumber - 1);

            var carparksReturned = await _mallCarparkRepository.GetPaginatedCarparks(documentsToSkip, pageSize);

            var paginatedCarparks = new PaginatedCarparkModel<MallCarparkModel>
            {
                Carparks = carparksReturned,
                TotalNumOfCarparks = totalCountOfCarparks == null ? 0 : (int) totalCountOfCarparks
            };

            return paginatedCarparks;
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return BadRequest(e.ToString());
        }
    }
    
    private async Task<OneMapSearchRootModel?> OneMapLocationSearch(string location)
    {
        var oneMapApiUri = "https://developers.onemap.sg/commonapi/search?searchVal=" + location +
                           "&returnGeom=Y&getAddrDetails=N";
        var response = await _client.GetFromJsonAsync<OneMapSearchRootModel>(oneMapApiUri);

        return response;
    }
}