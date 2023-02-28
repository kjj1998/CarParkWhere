using Google.Cloud.Firestore;

namespace CZ3002_Backend.Models;

[FirestoreData]
public class SampleCityModel : IBaseFirestoreDataModel
{
    public string Id { get; set; }
    
    [FirestoreProperty]
    public string Name { get; set; }
}