using CZ3002_Backend.Models;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Repo;

public interface IMallCarparkRepository
{
    Task<List<MallCarparkModel>> GetAllAsync();
    Task<MallCarparkModel> GetAsync(MallCarparkModel entity);
    Task<MallCarparkModel> AddAsync(MallCarparkModel entity);
    Task<List<MallCarparkModel>> AddMultipleAsync(List<MallCarparkModel> listOfEntities);
    Task<MallCarparkModel> UpdateAsync(MallCarparkModel entity);
    Task DeleteAsync(MallCarparkModel entity);
    Task<List<MallCarparkModel>> QueryRecordsAsync(Query query);

    Task<List<MallCarparkModel>> GetAllNearbyMallCarParkWithCoords(GeoPoint coordinates, int precision);
}