using CZ3002_Backend.Models;
using CZ3002_Backend.Repo;
using Microsoft.AspNetCore.Mvc;

namespace CZ3002_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class SampleUserController : ControllerBase
{
    private readonly ILogger<SampleUserController> _logger;
    private readonly ISampleUserRepository _sampleUserRepository;

    public SampleUserController(ILogger<SampleUserController> logger, ISampleUserRepository sampleUserRepository)
    {
        _logger = logger;
        _sampleUserRepository = sampleUserRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<SampleUserModel>>> GetAllUsersAsync()
    {
        return Ok(await _sampleUserRepository.GetAllAsync());
    }
    
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<SampleUserModel>> GetUserAsync(string id)
    {
        var user = new SampleUserModel()
        {
            Id = id
        };

        return Ok(await _sampleUserRepository.GetAsync(user));
    }

    [HttpPost]
    [Route("{id}")]
    public async Task<ActionResult<SampleUserModel>> UpdateUserAsync(string id, SampleUserModel user)
    {
        if (id != user.Id)
        {
            return BadRequest("Id must match.");
        }

        return Ok(await _sampleUserRepository.UpdateAsync(user));
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> DeleteUserAsync(string id, SampleUserModel user)
    {
        if (id != user.Id)
        {
            return BadRequest("Id must match.");
        }

        await _sampleUserRepository.DeleteAsync(user);

        return Ok();
    }


    [HttpPut] 
    public async Task<ActionResult<SampleUserModel>> DeleteUserAsync(SampleUserModel user)
    {  
        return Ok(await _sampleUserRepository.AddAsync(user));
    }

    [HttpGet]
    [Route("city/{city}")]
    public async Task<ActionResult<SampleUserModel>> GetUserWhereCity(string city)
    {  
        return Ok(await _sampleUserRepository.GetUserWhereCity(city));
    }
}