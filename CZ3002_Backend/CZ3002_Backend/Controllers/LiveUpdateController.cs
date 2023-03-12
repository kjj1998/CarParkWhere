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

    private readonly ILiveUpdateService _liveUpdateService;

    public LiveUpdateController(
        ILogger<DataController> logger,
        ILiveUpdateService liveUpdateService
        )
    {
        _logger = logger;
        _liveUpdateService = liveUpdateService;
    }

    [HttpGet]
    [Route("LiveUpdateMallData")]
    public async Task<ActionResult> LiveUpdateMallData()
    {
        try
        {
            await _liveUpdateService.MallLiveUpdate();

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return BadRequest(e.ToString());
        }
    }

    [HttpGet]
    [Route("LiveUpdateUraData")]
    public async Task<ActionResult> LiveUpdateUraData()
    {
        try
        {
            await _liveUpdateService.UraLiveUpdate();

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return BadRequest(e.ToString());
        }
    }

    [HttpGet]
    [Route("LiveUpdateHdbData")]
    public async Task<ActionResult> LiveUpdateHdbData()
    {
        try
        {
            await _liveUpdateService.HdbLiveUpdate();

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return BadRequest(e.ToString());
        }
    }
}