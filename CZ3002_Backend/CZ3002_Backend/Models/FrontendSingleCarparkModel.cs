using System.Text.Json.Serialization;

namespace CZ3002_Backend.Models;

public class FrontendSingleCarparkModel
{
    [JsonPropertyName("mall")] 
    public MallCarparkModel Mall { get; set; }
    [JsonPropertyName("hdb")] 
    public HdbCarParkModel Hdb { get; set; }
    [JsonPropertyName("ura")] 
    public UraCarparkModel Ura { get; set; }
}