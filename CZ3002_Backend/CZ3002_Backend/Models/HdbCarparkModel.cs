using Google.Cloud.Firestore;

namespace CZ3002_Backend.Models;

[FirestoreData]
public class HdbCarParkModel : IBaseFirestoreDataModel
{
    
    public string Id { get; set; }
    
    [FirestoreProperty]
    public string CarparkCode { get; set; }
    
    [FirestoreProperty]
    public string Name { get; set; }
    
    [FirestoreProperty]
    public string System { get; set; }
    
    [FirestoreProperty]
    public Lots Lots { get; set; }
    
    [FirestoreProperty]
    public LatLong? Coordinates { get; set; }
    
    [FirestoreProperty]
    public string ShortTermParking { get; set; }
    
    [FirestoreProperty]
    public string FreeParking { get; set; }
    
    [FirestoreProperty]
    public string NightParking { get; set; }
    
    [FirestoreProperty]
    public int CarParkDecks { get; set; }
    
    [FirestoreProperty]
    public float GantryHeight { get; set; }
    
    [FirestoreProperty]
    public string CarParkBasement { get; set; }
}

public class GovLiveCarparkDatum
{
    public List<GovLiveCarparkInfo> carpark_info { get; set; }
    public string carpark_number { get; set; }
    public DateTime update_datetime { get; set; }
}

public class GovLiveCarparkInfo
{
    public string total_lots { get; set; }
    public string lot_type { get; set; }
    public string lots_available { get; set; }
}

public class GovLiveItem
{
    public DateTime timestamp { get; set; }
    public List<GovLiveCarparkDatum> carpark_data { get; set; }
}

public class GovLiveRoot
{
    public List<GovLiveItem> items { get; set; }
}

public class GovStaticField
{
    public string type { get; set; }
    public string id { get; set; }
}

public class GovStaticLinks
{
    public string start { get; set; }
    public string next { get; set; }
}

public class GovStaticRecord
{
    public string _full_count { get; set; }
    public string short_term_parking { get; set; }
    public string car_park_type { get; set; }
    public string y_coord { get; set; }
    public string x_coord { get; set; }
    public double rank { get; set; }
    public string free_parking { get; set; }
    public string gantry_height { get; set; }
    public string car_park_basement { get; set; }
    public string night_parking { get; set; }
    public string address { get; set; }
    public string car_park_decks { get; set; }
    public int _id { get; set; }
    public string car_park_no { get; set; }
    public string type_of_parking_system { get; set; }
}

public class GovStaticResult
{
    public string resource_id { get; set; }
    public List<GovStaticField> fields { get; set; }
    public string q { get; set; }
    public List<GovStaticRecord> records { get; set; }
    public GovStaticLinks GovStaticLinks { get; set; }
    public int total { get; set; }
}

public class GovStaticRoot
{
    public string help { get; set; }
    public bool success { get; set; }
    public GovStaticResult result { get; set; }
}
