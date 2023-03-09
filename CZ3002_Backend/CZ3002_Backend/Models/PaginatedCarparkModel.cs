namespace CZ3002_Backend.Models;

public class PaginatedCarparkModel<T1>
{
    public List<T1>? Carparks { get; set; }
    public int TotalNumOfCarparks { get; set; }
}
