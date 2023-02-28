using CZ3002_Backend.Models;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Repo;

public interface ISampleUserRepository
{
    Task<List<SampleUserModel>> GetAllAsync();
    Task<SampleUserModel> GetAsync(SampleUserModel entity);
    Task<SampleUserModel> AddAsync(SampleUserModel entity);
    Task<SampleUserModel> UpdateAsync(SampleUserModel entity);
    Task DeleteAsync(SampleUserModel entity);
    Task<List<SampleUserModel>> QueryRecordsAsync(Query query);
    Task<List<SampleUserModel>> GetUserWhereCity(string cityName);
}