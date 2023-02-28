using Google.Cloud.Firestore;
namespace CZ3002_Backend.Models;

[FirestoreData]
public class SampleUserModel :IBaseFirestoreDataModel
{
    public string Id { get; set; }
    
    [FirestoreProperty]
    public string Name { get; set; }
    
    [FirestoreProperty]
    public string Surname { get; set; }
    
    [FirestoreProperty]
    public string Gender { get; set; }
    
    [FirestoreProperty]
    public SampleCityModel SampleCityModelFrom { get; set; }
    
    public string NotSavedProperty { get; set; }
}