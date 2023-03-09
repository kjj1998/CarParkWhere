using System.Text.Json.Serialization;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Models;

[FirestoreData]
public class UserModel:IBaseFirestoreDataModel
{
    public string Id { get; set; }
    [FirestoreProperty]
    public string Name { get; set; }

    [FirestoreProperty] 
    
    public List<string> FavouriteHDBCarParkCodes { get; set; }
    [JsonPropertyName("hdb")]
    public List<HdbCarParkModel> Hdb { get; set; }
    
    [FirestoreProperty] 
    public List<string> FavouriteMallCarParkCodes { get; set; }
    [JsonPropertyName("mall")]
    public List<MallCarparkModel> Mall { get; set; }
    
    [FirestoreProperty] 
    public List<string> FavouriteURACarParkCodes { get; set; }
    [JsonPropertyName("ura")]
    public List<UraCarparkModel> Ura { get; set; }

}