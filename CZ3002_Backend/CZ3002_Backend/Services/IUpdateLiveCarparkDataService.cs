namespace CZ3002_Backend.Services;

public interface IUpdateLiveCarparkDataService<T1, T2>
{
    Task UpdateData(List<T1> staticData, List<T2> liveData);
}