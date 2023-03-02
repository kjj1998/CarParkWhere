namespace CZ3002_Backend.Models;

public class UraCarparkModel
{
    private string Id { get; set; }
    private string Name { get; set; }
    private float[] Coordinates { get; set; }
    private CarparkRate[] Availabilities { get; set; }
}

public class CarparkRate
{
    private string WeekDayRate { get; set; }
    private string SatRate { get; set; }
    private string SunPhRate { get; set; }
    private string StartTime { get; set; }
    private string EndTime { get; set; }
    private int AvailableLots { get; set; }
    private string remarks { get; set; }
}