using System.Security.Claims;

namespace Core.Security.Extensions;

public static class ClaimExtensions
{
    public static void AddIEnumerableClaims(this ICollection<Claim> claims, IEnumerable<Claim> listClaims )
    {
        listClaims.ToList().ForEach(item =>
        {
            claims.Add(new Claim(item.Type, item.Value,item.ValueType));
        });
    }
    public static void AddRoles(this ICollection<Claim> claims, ICollection<string> roles)
    {
        foreach (string role in roles)
            claims.AddRole(role);
    }

    public static void AddRole(this ICollection<Claim> claims, string role)
    {
        claims.Add(new Claim(ClaimTypes.Role, role));
    }
}