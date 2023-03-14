using CZ3002_Backend.Models;

namespace CZ3002_Backend.Repo;

public interface ISolrRepository
{
    public Task AddField(string name, string type);
    public Task AddCopyField(string source, List<string> destination);
    public Task AddMultipleDocs(List<CarparkSolrIndex> carparkSolrIndices);
    public Task<SolrSearchResults> Search(string? term, int start, int rows, string type);
    public Task DeleteAllDocs();
}