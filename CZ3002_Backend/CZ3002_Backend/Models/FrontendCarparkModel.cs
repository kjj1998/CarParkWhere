using System.Text.Json.Serialization;

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