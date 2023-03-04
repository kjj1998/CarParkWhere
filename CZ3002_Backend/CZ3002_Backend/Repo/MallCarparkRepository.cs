using CZ3002_Backend.Enums;
using CZ3002_Backend.Models;
using Geohash;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Repo;

public class MallCarparkRepository : ICarparkRepository
{
    private readonly BaseRepository<MallCarparkModel> _repository;

    public MallCarparkRepository(IConfiguration configuration)
    {
        _repository = new BaseRepository<MallCarparkModel>(Collection.MallCarparks,configuration);
    }
    
    // This is specific to MallCarParks.
    public async Task<List<MallCarparkModel>> GetAllNearbyMallCarParkWithCoords(GeoPoint coordinates, int precision = 6)
    {
        //var query = ((BaseRepository<MallCarparkModel>)_repository)._firestoreDb.Collection(Collection.SampleUsers.ToString()).WhereIn(nameof(SampleUserModel.SampleCityModelFrom), cities);
        var hasher = new Geohasher();
        var hash = hasher.Encode(coordinates.Latitude,
            coordinates.Longitude,precision);
        var query = (_repository)._firestoreDb.Collection(Collection.MallCarparks.ToString()).WhereGreaterThanOrEqualTo(nameof(MallCarparkModel.GeoHash), hash).WhereLessThanOrEqualTo(nameof(MallCarparkModel.GeoHash), hash + "~");
        return await this.QueryRecordsAsync<MallCarparkModel>(query);
    }

    public async Task<List<T>> GetAllAsync<T>() where T : IBaseFirestoreDataModel => await _repository.GetAllAsync<T>();

    public async Task<T> GetAsync<T>(T entity) where T : IBaseFirestoreDataModel
    {
        return (T) await _repository.GetAsync(entity);
    }

    public async Task<T> AddAsync<T>(T entity) where T : IBaseFirestoreDataModel
    {
        return await _repository.AddAsync(entity);
    }

    public async Task<List<T>> AddMultipleAsync<T>(List<T> listOfEntities) where T : IBaseFirestoreDataModel
    {
        return await _repository.AddMultipleAsync(listOfEntities);
    }

    public async Task<T> UpdateAsync<T>(T entity) where T : IBaseFirestoreDataModel
    {
        return await _repository.UpdateAsync(entity);
    }

    public async Task<List<T2>> UpdateMultipleAsync<T2>(List<T2> listOfEntities, List<string> docReferences, string fieldName) where T2 : IBaseFirestoreDataModel
    {
        return await _repository.UpdateMultipleAsync(listOfEntities, docReferences, fieldName);
    }

    public async Task DeleteAsync<T>(T entity) where T : IBaseFirestoreDataModel
    {
        await _repository.DeleteAsync(entity);
    }

    public async Task<List<T>> QueryRecordsAsync<T>(Query query) where T : IBaseFirestoreDataModel
    {
        return await _repository.QueryRecordsAsync<T>(query);
    }
}