using CZ3002_Backend.Models;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Repo;

public interface IUserRepository
{
    Task<List<UserModel>> GetAllAsync();
    Task<UserModel> GetAsync(UserModel entity);
    Task<UserModel> AddAsync(UserModel entity);
    Task<UserModel> UpdateAsync(UserModel entity);
    Task DeleteAsync(UserModel entity);
    Task<List<UserModel>> QueryRecordsAsync(Query query);
    Task<UserModel> GetUserFavouriteCarParks(string user);
    Task UpsertUserFavouriteCarPark(string user, string carParkCode);
    Task DeleteUserFavouriteCarPark(string user, string carParkCode);
}