﻿using CZ3002_Backend.Models;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Repo;

public interface IMallCarparkRepository
{
    BaseRepository<MallCarparkModel> GetBaseRepository();
    Task<List<MallCarparkModel>> GetAllAsync();
    Task<MallCarparkModel> GetAsync(MallCarparkModel entity);
    Task<MallCarparkModel> AddAsync(MallCarparkModel entity);
    Task<List<MallCarparkModel>> AddMultipleAsync(List<MallCarparkModel> listOfEntities);
    Task<MallCarparkModel> UpdateAsync(MallCarparkModel entity);
    Task<List<Lots>> UpdateMultipleAsync(List<Lots> listOfEntities, List<string> docReferences, string fieldName);
    Task DeleteAsync(MallCarparkModel entity);
    Task<List<MallCarparkModel>> QueryRecordsAsync(Query query);
    Task<List<MallCarparkModel>> GetAllNearbyMallCarParkWithCoords(GeoPoint coordinates, int precision = 6); 
    Task<long?> GetTotalNumberOfCarparks();
    Task<List<MallCarparkModel>?> GetPaginatedCarparks(int documentsToSkip, int pageSize);
}