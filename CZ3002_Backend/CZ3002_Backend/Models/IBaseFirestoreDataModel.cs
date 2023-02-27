using Google.Cloud.Firestore;

namespace CZ3002_Backend.Models;

[FirestoreData]
public interface IBaseFirestoreDataModel
{
    public string Id { get; set; }
    public string Name { get; set; }
}