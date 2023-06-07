#nullable disable

namespace Bytewardens.Models;

public class HomeViewModel
{
    public List<ListOfDealsResponse> Games { get; set; }
    public List<Store> Stores { get; set; }
    public int? MaxPages { get; set; }
    public bool IsLoggedIn { get; set; }
}