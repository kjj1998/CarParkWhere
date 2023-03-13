using Microsoft.AspNetCore.Mvc;
using CZ3002_Backend.Models;
using System.Text.Json;

namespace CZ3002_Backend.Repo;

public class HdbSolrRepository : ISolrRepository
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;

    public HdbSolrRepository(IConfiguration configuration)
    {
        _client = new HttpClient();
        _configuration = configuration;
    }

    public async Task AddField(string name, string type)
    {
        var solrAddFieldRoot = new SolrAddFieldRoot
        {
            Addfield = new AddField
            {
                name = name,
                type = type
            }
        };
        
        var uri = _configuration["SOLR_CORE"] + "hdbcarparks/schema";
        var content = JsonContent.Create(solrAddFieldRoot); 
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = content;
        
        await _client.SendAsync(request);
    }

    public async Task AddCopyField(string source, List<string> destination)
    {
        var solrAddCopyFieldRoot = new SolrAddCopyFieldRoot()
        {
            AddCopyfield = new AddCopyField()
            {
                source = source,
                dest = destination
            }
        };
        
        var uri = _configuration["SOLR_CORE"] + "hdbcarparks/schema";
        var content = JsonContent.Create(solrAddCopyFieldRoot);
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = content;
        
        await _client.SendAsync(request);
    }
}