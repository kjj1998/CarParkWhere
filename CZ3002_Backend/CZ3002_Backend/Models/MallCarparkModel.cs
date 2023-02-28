namespace CZ3002_Backend.Models;

public class MallCarparkModel
{
    private string id { get; set; }
    private string name { get; set; }
    private float[] Coordinates { get; set; }

    private string WeekDayRate1 { get; set; }
    private string WeekDayRate2 { get; set; }
    private string SatRate { set; get; }
    private string SunPhRate { set; get; }
}