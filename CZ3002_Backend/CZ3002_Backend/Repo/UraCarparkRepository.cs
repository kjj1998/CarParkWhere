using CZ3002_Backend.Enums;
using CZ3002_Backend.Models;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Repo;

public class UraCarparkRepository : IUraCarparkRepository
{
    public readonly BaseRepository<UraCarparkModel> _repository;
    
    public UraCarparkRepository(IConfiguration configuration)
    {
        _repository = new BaseRepository<UraCarparkModel>(Collection.UraCarparks,configuration);
    }

    public BaseRepository<UraCarparkModel> GetBaseRepository()
    {
        return _repository;
    }

    public async Task<List<UraCarparkModel>> GetAllAsync() => await _repository.GetAllAsync<UraCarparkModel>(); 

    public async Task<UraCarparkModel> GetAsync(UraCarparkModel entity) => (UraCarparkModel) await _repository.GetAsync(entity);

    public async Task<UraCarparkModel> AddAsync(UraCarparkModel entity) => await _repository.AddAsync(entity);

    public async Task<List<UraCarparkModel>> AddMultipleAsync(List<UraCarparkModel> listOfEntities) =>
        await _repository.AddMultipleAsync(listOfEntities);

    public async Task<UraCarparkModel> UpdateAsync(UraCarparkModel entity) => await _repository.UpdateAsync(entity);

    public async Task DeleteAsync(UraCarparkModel entity) => await _repository.DeleteAsync(entity);

    public async Task<List<UraCarparkModel>> QueryRecordsAsync(Query query) => await _repository.QueryRecordsAsync<UraCarparkModel>(query);
}