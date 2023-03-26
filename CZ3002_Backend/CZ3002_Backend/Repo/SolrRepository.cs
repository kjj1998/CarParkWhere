using Microsoft.AspNetCore.Mvc;
using CZ3002_Backend.Models;
using System.Text.Json;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.WebUtilities;

namespace CZ3002_Backend.Repo;

public class SolrRepository : ISolrRepository
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;
    private readonly string _coreName = "carparks3";

    public SolrRepository(IConfiguration configuration)
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
        
        var uri = _configuration["SOLR_CORE"] + _coreName + "/schema";
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
        
        var uri = _configuration["SOLR_CORE"] + _coreName +"/schema";
        var content = JsonContent.Create(solrAddCopyFieldRoot);
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = content;
        
        await _client.SendAsync(request);
    }

    public async Task AddMultipleDocs(List<CarparkSolrIndex> carparkSolrIndices)
    {
        var uri = _configuration["SOLR_CORE"] + _coreName +"/update/json/docs?commit=true";
        var content = JsonContent.Create(carparkSolrIndices);
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = content;

        await _client.SendAsync(request);
    }

    public async Task<SolrSearchResults> Search(string? term, int start, int rows, string? type)
    {
        var fq = "";
        if (type != null)
        {
            fq = "type:" + type;
        }

        var q = "*:*";
        if (term != null)
        {
            q = term;
        }
        
        var solrQuery = new SolrQuery()
        {
            q = q,
            start = start,
            rows = rows,
            fq = fq
        };

        var queryDict = solrQuery.ToDictionary();
        
        var requestUri = 
            QueryHelpers.AddQueryString(_configuration["SOLR_CORE"] + _coreName +"/select?", queryDict!);
        var response = await _client.GetAsync(requestUri);
        
        var responseBody = await response.Content.ReadFromJsonAsync<SolrResponseRoot>();
        var solrSearchResults = new SolrSearchResults()
        {
            RetrievedCarparks = responseBody?.Response.docs,
            TotalNumFound = responseBody?.Response.numFound,
            Start = responseBody?.Response.start,
            Rows = int.Parse(responseBody?.ResponseHeader.@params.rows),
            Query = responseBody.ResponseHeader.@params.q == "*:*" ? "" : responseBody.ResponseHeader.@params.q
        };

        return solrSearchResults;
    }

    public async Task DeleteAllDocs()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, _configuration["SOLR_CORE"] + _coreName +"/update?commit=true");
        var content = new StringContent("{\r\n    \"delete\": {\r\n        \"query\": \"*:*\"\r\n    }\r\n}", null, "application/json");
        request.Content = content;
        await client.SendAsync(request);
    }
}