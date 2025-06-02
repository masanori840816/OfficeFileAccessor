using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OfficeFileAccessor.AppUsers;

public record ExternalTokenValue(string Key);
public static class TokenValidator
{
    public static bool Validate(ExternalTokenValue tokenValue,  string token)
    {
        try
        {
            JwtSecurityTokenHandler tokenHandler = new ();
            byte[] keyData = Encoding.UTF8.GetBytes(tokenValue.Key);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyData),
                ValidateIssuer = false,
                ValidateAudience = false,
                // Validate expiration times strictly
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            return true;
        }
        catch (SecurityTokenExpiredException)
        {
            return false;
        }
    }
}