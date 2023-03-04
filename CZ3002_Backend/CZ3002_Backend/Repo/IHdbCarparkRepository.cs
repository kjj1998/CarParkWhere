using CZ3002_Backend.Models;

namespace CZ3002_Backend.Repo;

using Google.Cloud.Firestore;

public interface IHdbCarparkRepository
{
    BaseRepository<HdbCarParkModel> GetBaseRepository();
    Task<List<HdbCarParkModel>> GetAllAsync();
    Task<HdbCarParkModel> GetAsync(HdbCarParkModel entity);
    Task<HdbCarParkModel> AddAsync(HdbCarParkModel entity);
    Task<List<HdbCarParkModel>> AddMultipleAsync(List<HdbCarParkModel> listOfEntities);
    Task<List<Lots>> UpdateMultipleAsync(List<Lots> listOfEntities, List<string> docReferences, string fieldName);
    Task<HdbCarParkModel> UpdateAsync(HdbCarParkModel entity);
    Task DeleteAsync(HdbCarParkModel entity);
    Task<List<HdbCarParkModel>> QueryRecordsAsync(Query query);
    Task<List<HdbCarParkModel>> GetAllNearbyHDBCarParkWithCoords(GeoPoint coordinates, int precision);
}