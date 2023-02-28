using Google.Cloud.Firestore;

namespace CZ3002_Backend.Models;

[FirestoreData]
public class LatLong : IBaseFirestoreDataModel
{
    public string Id { get; set; }
    public string Name { get; set; }

    [FirestoreProperty]
    public double latitude { get; set; }
    
    [FirestoreProperty]
    public double longitude { get; set; }
}