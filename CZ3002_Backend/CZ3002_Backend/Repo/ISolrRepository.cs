namespace CZ3002_Backend.Repo;

public interface ISolrRepository
{
    public Task AddField(string name, string type);
    public Task AddCopyField(string source, List<string> destination);
}