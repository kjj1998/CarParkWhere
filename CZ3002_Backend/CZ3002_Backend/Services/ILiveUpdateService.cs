namespace CZ3002_Backend.Services;

public interface ILiveUpdateService
{
    Task MallLiveUpdate();
    Task UraLiveUpdate();
    Task HdbLiveUpdate();
}