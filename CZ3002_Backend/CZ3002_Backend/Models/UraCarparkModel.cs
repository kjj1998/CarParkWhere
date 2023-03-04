using System.Text.Json.Serialization;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Models;

[FirestoreData]
public class UraCarparkModel : IBaseFirestoreDataModel
{
    [JsonIgnore]
    public string Id { get; set; }
    
    [FirestoreProperty]
    public string CarparkCode { get; set; }
    
    [FirestoreProperty]
    public string Name { get; set; }
    
    [FirestoreProperty]
    public GeoPoint? Coordinates { get; set; }
    
    [JsonIgnore]
    [FirestoreProperty]
    public string? GeoHash { get; set; }
    
    [FirestoreProperty]
    public string System { get; set; }
    
    [FirestoreProperty]
    public Lots Lots { get; set; }
    
    [FirestoreProperty]
    public List<CarparkRate> Rates { get; set; }
}

[FirestoreData]
public class CarparkRate : IBaseFirestoreDataModel
{
    [FirestoreProperty]
    public string WeekDayRate { get; set; }
    
    [FirestoreProperty]
    public string SatRate { get; set; }
    
    [FirestoreProperty]
    public string SunPhRate { get; set; }
    
    [FirestoreProperty]
    public string StartTime { get; set; }
    
    [FirestoreProperty]
    public string EndTime { get; set; }

    [FirestoreProperty]
    public string Remarks { get; set; }
    
    [FirestoreProperty]
    public string VehicleCategory { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
}

public class UraLiveGeometry
{
    public string coordinates { get; set; }
}

public class UraLiveResult
{
    public string carparkNo { get; set; }
    public List<UraLiveGeometry> geometries { get; set; }
    public string lotsAvailable { get; set; }
    public string lotType { get; set; }
}

public class UraLiveRoot
{
    public string Status { get; set; }
    public string Message { get; set; }
    public List<UraLiveResult> Result { get; set; }
}

public class UraStaticGeometry
{
    public string coordinates { get; set; }
}

public class UraStaticResult
{
    public string weekdayMin { get; set; }
    public string weekdayRate { get; set; }
    public string ppCode { get; set; }
    public string parkingSystem { get; set; }
    public string ppName { get; set; }
    public string vehCat { get; set; }
    public string satdayMin { get; set; }
    public string satdayRate { get; set; }
    public string sunPHMin { get; set; }
    public string sunPHRate { get; set; }
    public List<UraStaticGeometry> geometries { get; set; }
    public string startTime { get; set; }
    public int parkCapacity { get; set; }
    public string endTime { get; set; }
    public string remarks { get; set; }
}

public class UraStaticRoot
{
    public string Status { get; set; }
    public string Message { get; set; }
    public List<UraStaticResult> Result { get; set; }
}

public class UraTokenRoot
{
    public string Status { get; set; }
    public string Message { get; set; }
    public string? Result { get; set; }
}
