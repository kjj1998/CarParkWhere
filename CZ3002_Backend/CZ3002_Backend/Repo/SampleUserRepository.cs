using CZ3002_Backend.Enums;
using CZ3002_Backend.Models;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Repo;

public class SampleUserRepository : ISampleUserRepository
{
    private readonly IBaseRepository<SampleUserModel> _repository;

    public SampleUserRepository(IConfiguration configuration)
    {
        _repository = new BaseRepository<SampleUserModel>(Collection.SampleUsers,configuration);
    }
    
    public async Task<List<SampleUserModel>> GetAllAsync() => await _repository.GetAllAsync<SampleUserModel>(); 

    public async Task<SampleUserModel> GetAsync(SampleUserModel entity) => (SampleUserModel) await _repository.GetAsync(entity);

    public async Task<SampleUserModel> AddAsync(SampleUserModel entity) => await _repository.AddAsync(entity);

    public async Task<SampleUserModel> UpdateAsync(SampleUserModel entity) => await _repository.UpdateAsync(entity);

    public async Task DeleteAsync(SampleUserModel entity) => await _repository.DeleteAsync(entity);

    public async Task<List<SampleUserModel>> QueryRecordsAsync(Query query) => await _repository.QueryRecordsAsync<SampleUserModel>(query);
    
    // This is specific to sample users.
    public async Task<List<SampleUserModel>> GetUserWhereCity(string cityName)
    {
        var cities = new List<SampleCityModel>()
        {
            new()
            {
                Name=cityName
            }
        };
        
        var query = ((BaseRepository<SampleUserModel>)_repository)._firestoreDb.Collection(Collection.SampleUsers.ToString()).WhereIn(nameof(SampleUserModel.SampleCityModelFrom), cities);
        return await this.QueryRecordsAsync(query);
    }
}