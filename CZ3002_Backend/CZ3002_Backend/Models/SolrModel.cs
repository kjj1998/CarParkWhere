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