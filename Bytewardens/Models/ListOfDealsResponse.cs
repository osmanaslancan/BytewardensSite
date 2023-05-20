#nullable disable
using System.Text.Json.Serialization;

namespace Bytewardens.Models;

public class ListOfDealsResponse
{
    [JsonPropertyName("internalName")]
    public string InternalName { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("metacriticLink")]
    public string MetacriticLink { get; set; }

    [JsonPropertyName("dealID")]
    public string DealID { get; set; }

    [JsonPropertyName("storeID")]
    public string StoreID { get; set; }

    [JsonPropertyName("gameID")]
    public string GameID { get; set; }

    [JsonPropertyName("salePrice")]
    public string SalePrice { get; set; }

    [JsonPropertyName("normalPrice")]
    public string NormalPrice { get; set; }

    [JsonPropertyName("isOnSale")]
    public string IsOnSale { get; set; }

    [JsonPropertyName("savings")]
    public string Savings { get; set; }

    [JsonPropertyName("metacriticScore")]
    public string MetacriticScore { get; set; }

    [JsonPropertyName("steamRatingText")]
    public string SteamRatingText { get; set; }

    [JsonPropertyName("steamRatingPercent")]
    public string SteamRatingPercent { get; set; }

    [JsonPropertyName("steamRatingCount")]
    public string SteamRatingCount { get; set; }

    [JsonPropertyName("steamAppID")]
    public string SteamAppID { get; set; }

    [JsonPropertyName("releaseDate")]
    public int ReleaseDate { get; set; }

    [JsonPropertyName("lastChange")]
    public int LastChange { get; set; }

    [JsonPropertyName("dealRating")]
    public string DealRating { get; set; }

    [JsonPropertyName("thumb")]
    public string Thumb { get; set; }
}
