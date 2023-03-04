using CZ3002_Backend.Models;
using CZ3002_Backend.Repo;

namespace CZ3002_Backend.Services;

public class UpdateLiveHdbCarparkDataService : IUpdateLiveCarparkDataService<HdbCarParkModel, GovLiveCarparkDatum>
{
    private IConfiguration _configuration;
    private IHdbCarparkRepository _hdbCarparkRepository;

    public UpdateLiveHdbCarparkDataService(IConfiguration configuration, IHdbCarparkRepository hdbCarparkRepository)
    {
        _configuration = configuration;
        _hdbCarparkRepository = hdbCarparkRepository;
    }

    public async Task UpdateData(List<HdbCarParkModel> staticData, List<GovLiveCarparkDatum> liveData)
    {
        foreach (var hdbCarpark in staticData)
        {
            if (liveData.Exists(cp => cp.carpark_number == hdbCarpark.CarparkCode))
            {
                var liveHdbCarpark = liveData.Find(cp => cp.carpark_number == hdbCarpark.CarparkCode);

                switch (liveHdbCarpark?.carpark_info[0].lot_type)
                {
                    case "C":
                        hdbCarpark.Lots.Car.Available = int.Parse(liveHdbCarpark?.carpark_info[0].lots_available ?? "0");
                        break;
                    case "Y":
                        hdbCarpark.Lots.Motorcycle.Available = int.Parse(liveHdbCarpark?.carpark_info[0].lots_available ?? "0");
                        break;
                    case "H":
                        hdbCarpark.Lots.HeavyVehicle.Available = int.Parse(liveHdbCarpark?.carpark_info[0].lots_available ?? "0");
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
            if (carparkCount + int.Parse(_configuration["GoogleBatchWriteLimit"]) > totalNumOfCarparks - 1)
            {
                partitionedLots = lots.GetRange(carparkCount, totalNumOfCarparks - carparkCount);
                partitionedIds = ids.GetRange(carparkCount, totalNumOfCarparks - carparkCount);
                carparkCount += (totalNumOfCarparks - carparkCount);
            }
            else
            {
                partitionedLots = lots.GetRange(carparkCount, int.Parse(_configuration["GoogleBatchWriteLimit"]));
                partitionedIds = ids.GetRange(carparkCount, int.Parse(_configuration["GoogleBatchWriteLimit"]));
                carparkCount += int.Parse(_configuration["GoogleBatchWriteLimit"]);
            }

            await _hdbCarparkRepository.UpdateMultipleAsync(partitionedLots, partitionedIds, "Lots");
        }
    }
}