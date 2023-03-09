using CZ3002_Backend.Models;
using CZ3002_Backend.Repo;
using Microsoft.AspNetCore.Mvc;

namespace CZ3002_Backend.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IGeneralRepository _generalRepository;

    public UserController(ILogger<UserController> logger, IGeneralRepository generalRepository)
    {
        _logger = logger;
        _generalRepository = generalRepository;
    }

    [HttpGet]
    [Route("GetFavouriteCarParks")]
    public async Task<IActionResult> GetFavouriteHDBCarParks(string user)
    {
        try
        {
            var result = await _generalRepository.GetUserFavouriteCarParks(user);
            return new JsonResult(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return BadRequest(e.ToString());
        }
    }

    [HttpPut]
    [Route("PutFavouriteCarPark")]
    public async Task<IActionResult> PutFavouriteCarPark(string user, string carParkCode)
    {
        try
        {
            await _generalRepository.UpsertUserFavouriteCarPark(user, carParkCode);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return BadRequest(e.ToString());
        }
    }
    
    [HttpDelete]
    [Route("DeleteFavouriteCarPark")]
    public async Task<IActionResult> DeleteFavouriteCarPark(string user, string carParkCode)
    {
        try
        {
            await _generalRepository.DeleteUserFavouriteCarPark(user, carParkCode);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return BadRequest(e.ToString());
        }
    }
}