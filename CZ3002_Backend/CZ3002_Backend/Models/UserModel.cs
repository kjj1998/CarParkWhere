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
    public List<HdbCarParkModel> FavouriteHDBCarParks { get; set; }
    
    [FirestoreProperty] 
    public List<string> FavouriteMallCarParkCodes { get; set; }
    public List<MallCarparkModel> FavouriteMallCarParks { get; set; }
    
    [FirestoreProperty] 
    public List<string> FavouriteURACarParkCodes { get; set; }
    public List<UraCarparkModel> FavouriteURACarParks { get; set; }

}