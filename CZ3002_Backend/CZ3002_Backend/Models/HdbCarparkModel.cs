namespace CZ3002_Backend.Models;

public class HdbCarparkModel
{
    private string Id { get; set; }
    private string Name { get; set; }
    private ParkingSystemEnum SystemEnum { get; set; }
    private VehicleLots[] Availabilities { get; set; }
    private float[] Coordinates { get; set; }
    private string ShortTermParking { get; set; }
    private string FreeParking { get; set; }
    private YesNoEnum NightParking { get; set; }
    private int CarParkDecks { get; set; }
    private float GantryHeight { get; set; }
    private YesNoEnum CarParkBasement { get; set; }
}