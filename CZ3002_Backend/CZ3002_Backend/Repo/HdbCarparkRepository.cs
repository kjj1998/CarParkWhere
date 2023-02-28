using CZ3002_Backend.Models;
using CZ3002_Backend.Enums;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Repo;

public class HdbCarparkRepository : IHdbCarparkRepository
{
    private readonly IBaseRepository<HdbCarParkModel> _repository;

    public HdbCarparkRepository(IConfiguration configuration)
    {
        _repository = new BaseRepository<HdbCarParkModel>(Collection.HdbCarparks,configuration);
    }
    
    public async Task<List<HdbCarParkModel>> GetAllAsync() => await _repository.GetAllAsync<HdbCarParkModel>(); 

    public async Task<HdbCarParkModel> GetAsync(HdbCarParkModel entity) => (HdbCarParkModel) await _repository.GetAsync(entity);

    public async Task<HdbCarParkModel> AddAsync(HdbCarParkModel entity) => await _repository.AddAsync(entity);

    public async Task<List<HdbCarParkModel>> AddMultipleAsync(List<HdbCarParkModel> listOfEntities) =>
        await _repository.AddMultipleAsync(listOfEntities);

    public async Task<HdbCarParkModel> UpdateAsync(HdbCarParkModel entity) => await _repository.UpdateAsync(entity);

    public async Task DeleteAsync(HdbCarParkModel entity) => await _repository.DeleteAsync(entity);

    public async Task<List<HdbCarParkModel>> QueryRecordsAsync(Query query) => await _repository.QueryRecordsAsync<HdbCarParkModel>(query);
}