using System.Text.Json.Serialization;

namespace CZ3002_Backend.Models;

public class AddField
{
    public AddField()
    {
        this.stored = true;
        this.indexed = true;
        this.multiValued = false;
    }

    public string name { get; set; }
    public string type { get; set; }
    public bool indexed { get; set; }
    public bool stored { get; set; }
    public bool multiValued { get; set; }
}

public class SolrAddFieldRoot
{
    [JsonPropertyName("add-field")]
    public AddField Addfield { get; set; }
}

public class AddCopyField
{
    public string source { get; set; }
    public List<string> dest { get; set; }
}

public class SolrAddCopyFieldRoot
{
    [JsonPropertyName("add-copy-field")]
    public AddCopyField? AddCopyfield { get; set; }
}

public class CarparkSolrIndex
{
    public string id { get; set; }
    public string name { get; set; }
    public string? carparkcode { get; set; }
    public string type { get; set; }
}

public class SolrQuery
{
    public string? q { get; set; }
    public string? qOp { get; set; } = "AND";
    public int start { get; set; }
    public int rows { get; set; }
    public string? fq { get; set; }
    
    public IDictionary<string, string?> ToDictionary()
    {
        return new Dictionary<string, string?>
        {
            {"q", q},
            {"q.op", qOp},
            {"start", start.ToString()},
            {"rows", rows.ToString()},
            {"fq", fq}
        };
    }
}

public class SolrDoc
{
    public string id { get; set; }
    public string name { get; set; }
    public string carparkcode { get; set; }
    public string type { get; set; }
    // public string _version_ { get; set; }
}

public class SolrParams
{
    public string q { get; set; }
    public string? start { get; set; }

    [JsonPropertyName("q.op")]
    public string qop { get; set; }
    public string? rows { get; set; }
}

public class SolrResponse
{
    public int numFound { get; set; }
    public int? start { get; set; }
    public bool numFoundExact { get; set; }
    public List<SolrDoc>? docs { get; set; }
}

public class SolrResponseHeader
{
    public int status { get; set; }
    public int QTime { get; set; }
    public SolrParams @params { get; set; }
}

public class SolrResponseRoot
{
    public SolrResponseHeader ResponseHeader { get; set; }
    public SolrResponse Response { get; set; }
}

public class SolrSearchResults
{
    public List<SolrDoc>? RetrievedCarparks { get; set; }
    public int? TotalNumFound { get; set; }
    public int? Start { get; set; }
    public int Rows { get; set; }
    public string Query { get; set; }
}