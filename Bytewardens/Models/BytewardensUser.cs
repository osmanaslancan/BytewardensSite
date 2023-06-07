#nullable disable

using Microsoft.AspNetCore.Identity;

namespace Bytewardens.Models;

public class BytewardensUser : IdentityUser
{
    public string Favorites { get; set; }
}