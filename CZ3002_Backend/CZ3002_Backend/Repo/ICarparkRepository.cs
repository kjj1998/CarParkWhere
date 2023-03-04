using CZ3002_Backend.Models;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Repo;

public interface ICarparkRepository
{
    Task<List<T>> GetAllAsync<T>() where T : IBaseFirestoreDataModel;
    Task<T> GetAsync<T>(T entity) where T : IBaseFirestoreDataModel;
    Task<T> AddAsync<T>(T entity) where T : IBaseFirestoreDataModel;
    Task<List<T>> AddMultipleAsync<T>(List<T> listOfEntities) where T : IBaseFirestoreDataModel;
    Task<T> UpdateAsync<T>(T entity) where T : IBaseFirestoreDataModel;
    Task<List<T2>> UpdateMultipleAsync<T2>(List<T2> listOfEntities, List<string> docReferences, string fieldName) where T2 : IBaseFirestoreDataModel;
    Task DeleteAsync<T>(T entity) where T : IBaseFirestoreDataModel;
    Task<List<T>> QueryRecordsAsync<T>(Query query) where T : IBaseFirestoreDataModel;
}