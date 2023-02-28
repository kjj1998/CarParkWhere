namespace CZ3002_Backend.Services;

public interface IDataSetUpService<T, TK>
{
    Task<List<T>> SetUp(List<TK>? carparks);
}