using CZ3002_Backend.Models;
using CZ3002_Backend.Repo;

namespace CZ3002_Backend.Services;

public class UpdateLiveUraCarparkDataService : IUpdateLiveCarparkDataService<UraCarparkModel, UraLiveResult>
{
    private readonly IUraCarparkRepository _uraCarparkRepository;
    private const int GoogleBatchWriteLimit = 500;

    public UpdateLiveUraCarparkDataService(IUraCarparkRepository uraCarparkRepository)
    {
        _uraCarparkRepository = uraCarparkRepository;
    }
    
    public async Task UpdateData(List<UraCarparkModel> staticData, List<UraLiveResult> liveData)
    {
        foreach (var uraCarpark in staticData)
        {
            if (liveData.Exists(cp => cp.carparkNo == uraCarpark.CarparkCode))
            {
                var liveUraCarpark = liveData.Find(cp => cp.carparkNo == uraCarpark.CarparkCode);

                switch (liveUraCarpark?.lotType)
                {
                    case "C":
                        uraCarpark.Lots.Car.Available = int.Parse(liveUraCarpark.lotsAvailable);
                        break;
                    case "M":
                        uraCarpark.Lots.Motorcycle.Available = int.Parse(liveUraCarpark.lotsAvailable);
                        break;
                    case "H":
                        uraCarpark.Lots.HeavyVehicle.Available = int.Parse(liveUraCarpark.lotsAvailable);
                        break;
                }
            }
        }
        
        var totalNumOfCarparks = staticData.Count;
        var carparkCount = 0;
        var lots = new List<Lots>();
        var ids = new List<string>();

        foreach (var carpark in staticData)
        {
            lots.Add(carpark.Lots);
            ids.Add(carpark.Id);
        }

        while (carparkCount < totalNumOfCarparks)
        {
            // Console.WriteLine("carpark count: " + carparkCount);
            List<Lots> partitionedLots;
            List<string> partitionedIds;
            if (carparkCount + GoogleBatchWriteLimit > totalNumOfCarparks - 1)
            {
                partitionedLots = lots.GetRange(carparkCount, totalNumOfCarparks - carparkCount);
                partitionedIds = ids.GetRange(carparkCount, totalNumOfCarparks - carparkCount);
                carparkCount += (totalNumOfCarparks - carparkCount);
            }
            else
            {
                partitionedLots = lots.GetRange(carparkCount, GoogleBatchWriteLimit);
                partitionedIds = ids.GetRange(carparkCount, GoogleBatchWriteLimit);
                carparkCount += GoogleBatchWriteLimit;
            }

            await _uraCarparkRepository.UpdateMultipleAsync(partitionedLots, partitionedIds, "Lots");
        }
    }
}