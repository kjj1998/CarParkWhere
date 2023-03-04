using CZ3002_Backend.Models;
using CZ3002_Backend.Repo;
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

    public QueryController(
        IMallCarparkRepository mallCarparkRepository, 
        IHdbCarparkRepository hdbCarparkRepository, 
        IUraCarparkRepository uraCarparkRepository)
    {
        _mallCarparkRepository = mallCarparkRepository;
        _hdbCarparkRepository = hdbCarparkRepository;
        _uraCarparkRepository = uraCarparkRepository;
        _client = new HttpClient();
    }

    [HttpPost]
    [Route("GetCarparkDataBasedOnCurrentLocation")]
    public async Task<ActionResult<FrontendCarparkModel>> GetCarparkDataBasedOnCurrentLocation(LatLong currentLocation)
    {
        var geoPointOfCurrentLocation = new GeoPoint(currentLocation.latitude, currentLocation.longitude);

        var nearbyMallCarparks = await _mallCarparkRepository.GetAllNearbyMallCarParkWithCoords(geoPointOfCurrentLocation);
        var nearbyHdbCarparks = await _hdbCarparkRepository.GetAllNearbyHdbCarParkWithCoords(geoPointOfCurrentLocation);
        var nearbyUraCarparks = new List<UraCarparkModel>();
        
        
        
        var frontendCarparkModel = new FrontendCarparkModel(nearbyMallCarparks, nearbyHdbCarparks, nearbyUraCarparks);

        return frontendCarparkModel;
    }
}