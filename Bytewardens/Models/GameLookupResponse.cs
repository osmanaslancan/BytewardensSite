#nullable disable

using System.Text.Json.Serialization;

namespace Bytewardens.Models;

public class CheapestPriceEver
{
    [JsonPropertyName("price")]
    public string Price { get; set; }

    [JsonPropertyName("date")]
    public int Date { get; set; }
}

public class Deal
{
    [JsonPropertyName("storeID")]
    public string StoreID { get; set; }

    [JsonPropertyName("dealID")]
    public string DealID { get; set; }

    [JsonPropertyName("price")]
    public string Price { get; set; }

    [JsonPropertyName("retailPrice")]
    public string RetailPrice { get; set; }

    [JsonPropertyName("savings")]
    public string Savings { get; set; }
}

public class Info
{
    [JsonPropertyName("gameID")]
    public string GameID { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("steamAppID")]
    public string SteamAppID { get; set; }

    [JsonPropertyName("thumb")]
    public string Thumb { get; set; }
}

public class GameLookupResponse
{
    [JsonPropertyName("info")]
    public Info Info { get; set; }

    [JsonPropertyName("cheapestPriceEver")]
    public CheapestPriceEver CheapestPriceEver { get; set; }

    [JsonPropertyName("deals")]
    public List<Deal> Deals { get; set; }
}

