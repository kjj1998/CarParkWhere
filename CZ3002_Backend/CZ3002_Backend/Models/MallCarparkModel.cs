using Google.Cloud.Firestore;

namespace CZ3002_Backend.Models;

[FirestoreData]
public class MallCarparkModel : IBaseFirestoreDataModel
{
    public string Id { get; set; }
    [FirestoreProperty]
    public string Name { get; set; }
    [FirestoreProperty]
    public string CarparkCode { get; set; }
    [FirestoreProperty]
    public LatLong? Coordinates { get; set; }
    [FirestoreProperty]
    public string WeekDayRate1 { get; set; }
    [FirestoreProperty]
    public string WeekDayRate2 { get; set; }
    [FirestoreProperty]
    public string SatRate { set; get; }
    [FirestoreProperty]
    public string SunPhRate { set; get; }

    [FirestoreProperty]
    public Lots Lots { set; get; }
}

public class LtaLiveCarparkRoot
{
    // public string odatametadata { get; set; }
    public List<LtaLiveCarparkValue> value { get; set; }
}

public class LtaLiveCarparkValue
{
    public string CarParkID { get; set; }
    public string Area { get; set; }
    public string Development { get; set; }
    public string Location { get; set; }
    public int AvailableLots { get; set; }
    public string LotType { get; set; }
    public string Agency { get; set; }
}

public class GovStaticMallField
{
    public string type { get; set; }
    public string id { get; set; }
}

public class GovStaticMallLinks
{
    public string start { get; set; }
    public string next { get; set; }
}

public class GovStaticMallRecord
{
    public string category { get; set; }
    public string saturday_rate { get; set; }
    public double rank { get; set; }
    public string _full_count { get; set; }
    public string sunday_publicholiday_rate { get; set; }
    public string carpark { get; set; }
    public string weekdays_rate_1 { get; set; }
    public string weekdays_rate_2 { get; set; }
    public int _id { get; set; }
}

public class GovStaticMallResult
{
    public string resource_id { get; set; }
    public List<GovStaticMallField> fields { get; set; }
    public string q { get; set; }
    public List<GovStaticMallRecord>? records { get; set; }
    public GovStaticMallLinks GovStaticMallLinks { get; set; }
    public int total { get; set; }
}

public class GovStaticMallRoot
{
    public string help { get; set; }
    public bool success { get; set; }
    public GovStaticMallResult result { get; set; }
}
