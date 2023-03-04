using CZ3002_Backend.Models;
using CZ3002_Backend.Repo;

namespace CZ3002_Backend.Services;

public class UpdateLiveMallCarparkDataService : IUpdateLiveCarparkDataService<MallCarparkModel, LtaLiveCarparkValue>
{
    private readonly ICarparkRepository _mallCarparkRepository;
    private const int GoogleBatchWriteLimit = 500;
    
    public UpdateLiveMallCarparkDataService(ICarparkRepository mallCarparkRepository)
    {
        _mallCarparkRepository = mallCarparkRepository;
    }
    
    public async Task UpdateData(List<MallCarparkModel> staticData, List<LtaLiveCarparkValue> liveData)
    {
        foreach (var mallCarpark in staticData)
        {
            if (liveData.Exists(cp => cp.CarParkID == mallCarpark.CarparkCode))
            {
                var liveMallCarpark = liveData.Find(cp => cp.CarParkID == mallCarpark.CarparkCode);

                switch (liveMallCarpark?.LotType)
                {
                    case "C":
                        mallCarpark.Lots.Car.Available = liveMallCarpark.AvailableLots;
                        break;
                    case "Y":
                        mallCarpark.Lots.Motorcycle.Available = liveMallCarpark.AvailableLots;
                        break;
                    case "H":
                        mallCarpark.Lots.HeavyVehicle.Available = liveMallCarpark.AvailableLots;
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

            await _mallCarparkRepository.UpdateMultipleAsync(partitionedLots, partitionedIds, "Lots");
        }
    }
}