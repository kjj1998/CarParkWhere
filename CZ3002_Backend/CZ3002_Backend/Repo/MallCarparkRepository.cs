using CZ3002_Backend.Enums;
using CZ3002_Backend.Models;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Repo;

public class MallCarparkRepository : IMallCarparkRepository
{
    private readonly BaseRepository<HdbCarParkModel> _repository;

    public MallCarparkRepository(IConfiguration configuration)
    {
        _repository = new BaseRepository<HdbCarParkModel>(Collection.MallCarparks,configuration);
    }
    
    public async Task<List<MallCarparkModel>> GetAllAsync() => await _repository.GetAllAsync<MallCarparkModel>(); 

    public async Task<MallCarparkModel> GetAsync(MallCarparkModel entity) => (MallCarparkModel) await _repository.GetAsync(entity);

    public async Task<MallCarparkModel> AddAsync(MallCarparkModel entity) => await _repository.AddAsync(entity);

    public async Task<List<MallCarparkModel>> AddMultipleAsync(List<MallCarparkModel> listOfEntities) =>
        await _repository.AddMultipleAsync(listOfEntities);

    public async Task<MallCarparkModel> UpdateAsync(MallCarparkModel entity) => await _repository.UpdateAsync(entity);

    public async Task DeleteAsync(MallCarparkModel entity) => await _repository.DeleteAsync(entity);

    public async Task<List<MallCarparkModel>> QueryRecordsAsync(Query query) => await _repository.QueryRecordsAsync<MallCarparkModel>(query);
}