using System.Text.Json.Serialization;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Models;

public class FrontendCarparkModel
{
    [JsonPropertyName("mall")] public List<MallCarparkModel> Mall { get; set; }
    [JsonPropertyName("hdb")] public List<HdbCarParkModel> Hdb { get; set; }
    [JsonPropertyName("ura")] public List<UraCarparkModel> Ura { get; set; }

    public FrontendCarparkModel(List<MallCarparkModel> mall, List<HdbCarParkModel> hdb, List<UraCarparkModel> ura)
    {
        Mall = mall;
        Hdb = hdb;
        Ura = ura;
    }
}

public class FrontendMallCarparkModel
{
    public string? Name { get; set; }
    public string? CarparkCode { get; set; }
    public LatLong? Coordinates { get; set; }
    public string? WeekDayRate1 { get; set; }
    public string? WeekDayRate2 { get; set; }
    public string? SatRate { set; get; }
    public string? SunPhRate { set; get; }
    public Lots? Lots { set; get; }

    // public FrontendMallCarparkModel(string Name, )
    // {
    //     
    // }
}

public class FrontendHdbCarparkModel
{
    public string? CarparkCode { get; set; }
    public string? Name { get; set; }
    public string? System { get; set; }
    public Lots? Lots { get; set; }
    public LatLong? Coordinates { get; set; }
    public string? ShortTermParking { get; set; }
    public string? FreeParking { get; set; }
    public string? NightParking { get; set; }
    public int CarParkDecks { get; set; }
    public float GantryHeight { get; set; }
    public string? CarParkBasement { get; set; }
}

public class FrontendUraCarparkModel
{
    public string? CarparkCode { get; set; }
    public string? Name { get; set; }
    public LatLong? Coordinates { get; set; }
    public string? System { get; set; }
    public Lots? Lots { get; set; }
    public List<CarparkRate>? Rates { get; set; }
}