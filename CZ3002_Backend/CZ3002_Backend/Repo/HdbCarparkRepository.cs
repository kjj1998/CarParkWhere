using CZ3002_Backend.Models;
using CZ3002_Backend.Enums;
using Geohash;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Repo;

public class HdbCarparkRepository : IHdbCarparkRepository
{
    private readonly IBaseRepository<HdbCarParkModel> _repository;

    public HdbCarparkRepository(IConfiguration configuration)
    {
        _repository = new BaseRepository<HdbCarParkModel>(Collection.HdbCarparks,configuration);
    }

    public BaseRepository<HdbCarParkModel> GetBaseRepository()
    {
        return (BaseRepository<HdbCarParkModel>)_repository;
    }

    public async Task<List<HdbCarParkModel>> GetAllAsync() => await _repository.GetAllAsync<HdbCarParkModel>(); 

    public async Task<HdbCarParkModel> GetAsync(HdbCarParkModel entity) => (HdbCarParkModel) await _repository.GetAsync(entity);

    public async Task<HdbCarParkModel> GetAsync(string id) => (HdbCarParkModel) await _repository.GetAsyncWithId<HdbCarParkModel>(id);

    public async Task<HdbCarParkModel> AddAsync(HdbCarParkModel entity) => await _repository.AddAsync(entity);

    public async Task<List<HdbCarParkModel>> AddMultipleAsync(List<HdbCarParkModel> listOfEntities) =>
        await _repository.AddMultipleAsync(listOfEntities);

    public async Task<List<Lots>> UpdateMultipleAsync(List<Lots> listOfEntities, List<string> docReferences, string fieldName)
        => await _repository.UpdateMultipleAsync(listOfEntities, docReferences, fieldName);

    public async Task<HdbCarParkModel> UpdateAsync(HdbCarParkModel entity) => await _repository.UpdateAsync(entity);

    public async Task DeleteAsync(HdbCarParkModel entity) => await _repository.DeleteAsync(entity);

    public async Task<List<HdbCarParkModel>> QueryRecordsAsync(Query query) => await _repository.QueryRecordsAsync<HdbCarParkModel>(query);
    public async Task<long?> GetTotalNumberOfCarparks() => await _repository.GetTotalCountOfDocuments();
    
    public async Task<List<HdbCarParkModel>?> GetPaginatedCarparks(int documentsToSkip, int pageSize) => 
        await _repository.QueryPaginatedRecordsAsync<HdbCarParkModel>(documentsToSkip, pageSize);

    // This is specific to HdbCarPark.
    public async Task<List<HdbCarParkModel>> GetAllNearbyHdbCarParkWithCoords(GeoPoint coordinates, int precision = 6)
    {
        var hasher = new Geohasher();
        var hash = hasher.Encode(coordinates.Latitude,
            coordinates.Longitude,precision);
        var query = ((BaseRepository<HdbCarParkModel>)_repository)._firestoreDb.Collection(
            Collection.HdbCarparks.ToString()).WhereGreaterThanOrEqualTo(
            nameof(HdbCarParkModel.GeoHash), hash).WhereLessThanOrEqualTo(
            nameof(HdbCarParkModel.GeoHash), hash + "~");
        return await this.QueryRecordsAsync(query);
    }
}