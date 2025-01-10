using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OfficeFileAccessor.AppUsers.Entities;

namespace OfficeFileAccessor.AppUsers;

public class UserTokens(IConfiguration Config): IUserTokens
{
    public string GenerateToken(ApplicationUser user)
    {
        return new JwtSecurityTokenHandler()
                .WriteToken(new JwtSecurityToken(Config["Jwt:Issuer"],
                    Config["Jwt:Audience"],
                    claims: new []
                    {
                        new Claim(ClaimTypes.Email, user?.Email ?? "")
                    },
                    expires: DateTime.Now.AddSeconds(30),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["Jwt:Key"] ?? "")),
                        SecurityAlgorithms.HmacSha256)));
    }
}