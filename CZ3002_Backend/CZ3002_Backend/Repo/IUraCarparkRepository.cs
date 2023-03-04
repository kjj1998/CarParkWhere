using CZ3002_Backend.Models;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Repo;

public interface IUraCarparkRepository
{
    BaseRepository<UraCarparkModel> GetBaseRepository();
    Task<List<UraCarparkModel>> GetAllAsync();
    Task<UraCarparkModel> GetAsync(UraCarparkModel entity);
    Task<UraCarparkModel> AddAsync(UraCarparkModel entity);
    Task<List<UraCarparkModel>> AddMultipleAsync(List<UraCarparkModel> listOfEntities);
    Task<List<Lots>> UpdateMultipleAsync(List<Lots> listOfEntities, List<string> docReferences, string fieldName);
    Task<UraCarparkModel> UpdateAsync(UraCarparkModel entity);
    Task DeleteAsync(UraCarparkModel entity);
    Task<List<UraCarparkModel>> QueryRecordsAsync(Query query);
    Task<List<UraCarparkModel>> GetAllNearbyUraCarParkWithCoords(GeoPoint coordinates, int precision = 6);
}