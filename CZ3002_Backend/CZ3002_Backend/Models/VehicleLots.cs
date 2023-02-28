using Google.Cloud.Firestore;

namespace CZ3002_Backend.Models;

[FirestoreData]
public class Lots : IBaseFirestoreDataModel
{
    public Lots()
    {
        Car = new Availability();
        Motorcycle = new Availability();
        HeavyVehicle = new Availability();
    }

    [FirestoreProperty]
    public Availability Car { get; set; }
    [FirestoreProperty]
    public Availability Motorcycle { get; set; }
    [FirestoreProperty]
    public Availability HeavyVehicle { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
}

[FirestoreData]
public class Availability : IBaseFirestoreDataModel
{
    [FirestoreProperty]
    public int Available { get; set; }
    [FirestoreProperty]
    public int Total { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
}