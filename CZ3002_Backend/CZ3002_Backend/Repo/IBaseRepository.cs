﻿using CZ3002_Backend.Models;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Repo;

public interface IBaseRepository<T>
{
    Task<List<T>> GetAllAsync<T>() where T : IBaseFirestoreDataModel;

    Task<object> GetAsync<T>(T entity) where T : IBaseFirestoreDataModel;

    Task<T> AddAsync<T>(T entity) where T : IBaseFirestoreDataModel;

    Task<T> UpdateAsync<T>(T entity) where T : IBaseFirestoreDataModel;
    
    Task DeleteAsync<T>(T entity) where T : IBaseFirestoreDataModel;
    
    Task<List<T>> QueryRecordsAsync<T>(Query query) where T : IBaseFirestoreDataModel;
    
}