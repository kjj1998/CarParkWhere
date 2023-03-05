namespace CZ3002_Backend.Models;
public class Result
{
    public string? SEARCHVAL { get; set; }
    public string? X { get; set; }
    public string? Y { get; set; }
    public string? LATITUDE { get; set; }
    public string? LONGITUDE { get; set; }
    public string? LONGTITUDE { get; set; }
}

public class OneMapSearchRootModel
{
    public int? found { get; set; }
    public int? totalNumPages { get; set; }
    public int? pageNum { get; set; }
    public List<Result>? results { get; set; }
}
