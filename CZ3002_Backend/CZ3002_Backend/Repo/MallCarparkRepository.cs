using CZ3002_Backend.Enums;
using CZ3002_Backend.Models;
using Geohash;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Repo;

public class MallCarparkRepository : IMallCarparkRepository
{
    private readonly BaseRepository<MallCarparkModel> _repository;

    public MallCarparkRepository(IConfiguration configuration)
    {
        _repository = new BaseRepository<MallCarparkModel>(Collection.MallCarparks,configuration);
    }
    
    public async Task<List<MallCarparkModel>> GetAllAsync() => await _repository.GetAllAsync<MallCarparkModel>(); 

    public async Task<MallCarparkModel> GetAsync(MallCarparkModel entity) => (MallCarparkModel) await _repository.GetAsync(entity);

    public async Task<MallCarparkModel> AddAsync(MallCarparkModel entity) => await _repository.AddAsync(entity);

    public async Task<List<MallCarparkModel>> AddMultipleAsync(List<MallCarparkModel> listOfEntities) =>
        await _repository.AddMultipleAsync(listOfEntities);

    public async Task<List<Lots>> UpdateMultipleAsync(List<Lots> listOfEntities, List<string> docReferences, string fieldName) 
        => await _repository.UpdateMultipleAsync(listOfEntities, docReferences, fieldName);

    public async Task<MallCarparkModel> UpdateAsync(MallCarparkModel entity) => await _repository.UpdateAsync(entity);

    public async Task DeleteAsync(MallCarparkModel entity) => await _repository.DeleteAsync(entity);

    public async Task<List<MallCarparkModel>> QueryRecordsAsync(Query query) => await _repository.QueryRecordsAsync<MallCarparkModel>(query);
    
    // This is specific to MallCarParks.
    public async Task<List<MallCarparkModel>> GetAllNearbyMallCarParkWithCoords(GeoPoint coordinates, int precision = 6)
    {
        //var query = ((BaseRepository<MallCarparkModel>)_repository)._firestoreDb.Collection(Collection.SampleUsers.ToString()).WhereIn(nameof(SampleUserModel.SampleCityModelFrom), cities);
        var hasher = new Geohasher();
        var hash = hasher.Encode(coordinates.Latitude,
            coordinates.Longitude,precision);
        var query = (_repository)._firestoreDb.Collection(Collection.MallCarparks.ToString()).WhereGreaterThanOrEqualTo(nameof(MallCarparkModel.GeoHash), hash).WhereLessThanOrEqualTo(nameof(MallCarparkModel.GeoHash), hash + "~");
        return await this.QueryRecordsAsync(query);
    }
}