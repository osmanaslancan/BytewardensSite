#nullable disable

using System.Text.Json.Serialization;

public class CheapestPrice
{
    [JsonPropertyName("price")]
    public string Price { get; set; }

    [JsonPropertyName("date")]
    public int Date { get; set; }
}

public class GameInfo
{
    [JsonPropertyName("storeID")]
    public string StoreID { get; set; }

    [JsonPropertyName("gameID")]
    public string GameID { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("steamAppID")]
    public string SteamAppID { get; set; }

    [JsonPropertyName("salePrice")]
    public string SalePrice { get; set; }

    [JsonPropertyName("retailPrice")]
    public string RetailPrice { get; set; }

    [JsonPropertyName("steamRatingText")]
    public string SteamRatingText { get; set; }

    [JsonPropertyName("steamRatingPercent")]
    public string SteamRatingPercent { get; set; }

    [JsonPropertyName("steamRatingCount")]
    public string SteamRatingCount { get; set; }

    [JsonPropertyName("metacriticScore")]
    public string MetacriticScore { get; set; }

    [JsonPropertyName("metacriticLink")]
    public string MetacriticLink { get; set; }

    [JsonPropertyName("releaseDate")]
    public int ReleaseDate { get; set; }

    [JsonPropertyName("publisher")]
    public string Publisher { get; set; }

    [JsonPropertyName("steamworks")]
    public string Steamworks { get; set; }

    [JsonPropertyName("thumb")]
    public string Thumb { get; set; }
}

public class DealRetriveResponse
{
    [JsonPropertyName("gameInfo")]
    public GameInfo GameInfo { get; set; }

    [JsonPropertyName("cheaperStores")]
    public List<object> CheaperStores { get; set; }

    [JsonPropertyName("cheapestPrice")]
    public CheapestPrice CheapestPrice { get; set; }
}

