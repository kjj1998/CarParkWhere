using CZ3002_Backend.Enums;
using CZ3002_Backend.Models;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Repo;

public class BaseRepository<T> : IBaseRepository<T>
{
    private readonly Collection _collection;
    private readonly IConfiguration _configuration;
    public FirestoreDb _firestoreDb;

    public BaseRepository(Collection collection, IConfiguration configuration )
    {
        _collection = collection;
        _configuration = configuration;
        var filepath = _configuration["GOOGLE_APPLICATION_CREDENTIALS"];
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "../CZ3002_Backend/Firebase/carparkwhere-c0ef4-firebase-adminsdk-nvb67-95324885d1.json");
        _firestoreDb = FirestoreDb.Create("carparkwhere-c0ef4");
        
    }
    public async Task<List<T1>> GetAllAsync<T1>() where T1 : IBaseFirestoreDataModel
    {
        Query query = _firestoreDb.Collection(_collection.ToString());
        var querySnapshot = await query.GetSnapshotAsync();
        var list = new List<T1>();
        foreach (var documentSnapshot in querySnapshot.Documents)
        {
            if (!documentSnapshot.Exists)
            {
                continue;
            }

            var data = documentSnapshot.ConvertTo<T1>();
            
            if (data == null)
            {
                continue;
            }

            data.Id = documentSnapshot.Id;
            list.Add(data);
        }

        return list;
    }

    public async Task<object> GetAsync<T1>(T1 entity) where T1 : IBaseFirestoreDataModel
    {
        var documentReference = _firestoreDb.Collection(_collection.ToString()).Document(entity.Id);
        var documentSnapshot = await documentReference.GetSnapshotAsync();

        if (documentSnapshot.Exists)
        {
            var baseFirestoreDataModel = documentSnapshot.ConvertTo<T1>();
            baseFirestoreDataModel.Id = documentSnapshot.Id;
            return baseFirestoreDataModel;
        }

        return null;
    }

    public async Task<T1> AddAsync<T1>(T1 entity) where T1 : IBaseFirestoreDataModel
    {
        var collectionReference = _firestoreDb.Collection(_collection.ToString());
        await collectionReference.AddAsync(entity);

        return entity;
    }

    public async Task<T1> UpdateAsync<T1>(T1 entity) where T1 : IBaseFirestoreDataModel
    {
        var documentReference = _firestoreDb.Collection(_collection.ToString()).Document(entity.Id);
        await documentReference.SetAsync(entity, SetOptions.MergeAll);

        return entity;
    }

    public async Task DeleteAsync<T1>(T1 entity) where T1 : IBaseFirestoreDataModel
    {
        var documentReference = _firestoreDb.Collection(_collection.ToString()).Document(entity.Id);
        await documentReference.DeleteAsync();
    }

    public async Task<List<T1>> QueryRecordsAsync<T1>(Query query) where T1 : IBaseFirestoreDataModel
    {
        var snapshotAsync = await query.GetSnapshotAsync();
        var list = new List<T1>();

        foreach (var documentSnapshot in snapshotAsync.Documents)
        {
            if (!documentSnapshot.Exists)
            {
                continue;
            }

            var baseFirestoreDataModel = documentSnapshot.ConvertTo<T1>();

            if (baseFirestoreDataModel == null)
            {
                continue;
            }
            
            list.Add(baseFirestoreDataModel);
        }

        return list;
    }
}