using CZ3002_Backend.Enums;
using CZ3002_Backend.Models;
using Geohash;
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

    public async Task<UraCarparkModel> GetAsync(string id) =>
        (UraCarparkModel) await _repository.GetAsyncWithId<UraCarparkModel>(id);

    public async Task<UraCarparkModel> AddAsync(UraCarparkModel entity) => await _repository.AddAsync(entity);

    public async Task<List<UraCarparkModel>> AddMultipleAsync(List<UraCarparkModel> listOfEntities) =>
        await _repository.AddMultipleAsync(listOfEntities);

    public async Task<List<Lots>> UpdateMultipleAsync(List<Lots> listOfEntities, List<string> docReferences, string fieldName)
    {
        return await _repository.UpdateMultipleAsync(listOfEntities, docReferences, fieldName);
    }

    public async Task<UraCarparkModel> UpdateAsync(UraCarparkModel entity) => await _repository.UpdateAsync(entity);

    public async Task DeleteAsync(UraCarparkModel entity) => await _repository.DeleteAsync(entity);

    public async Task<List<UraCarparkModel>> QueryRecordsAsync(Query query) => await _repository.QueryRecordsAsync<UraCarparkModel>(query);
    
    public async Task<long?> GetTotalNumberOfCarparks() => await _repository.GetTotalCountOfDocuments();
    
    public async Task<List<UraCarparkModel>?> GetPaginatedCarparks(int documentsToSkip, int pageSize) => 
        await _repository.QueryPaginatedRecordsAsync<UraCarparkModel>(documentsToSkip, pageSize);
    
    public async Task<List<UraCarparkModel>> GetAllNearbyUraCarParkWithCoords(GeoPoint coordinates, int precision = 6)
    {
        //var query = ((BaseRepository<MallCarparkModel>)_repository)._firestoreDb.Collection(Collection.SampleUsers.ToString()).WhereIn(nameof(SampleUserModel.SampleCityModelFrom), cities);
        var hasher = new Geohasher();
        var hash = hasher.Encode(coordinates.Latitude,
            coordinates.Longitude,precision);
        var query = (_repository)._firestoreDb.Collection(Collection.UraCarparks.ToString()).WhereGreaterThanOrEqualTo(nameof(UraCarparkModel.GeoHash), hash).WhereLessThanOrEqualTo(nameof(UraCarparkModel.GeoHash), hash + "~");
        return await this.QueryRecordsAsync(query);
    }
}