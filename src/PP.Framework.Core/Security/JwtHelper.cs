using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PaulPhillips.Framework.Feature.Security
{
    public static class JwtHelper
    {
        public static string GenerateToken(string secretKey, string issuer, string audience, List<Claim> claims, int expiresInMinutes = 60)
        {
            // Create a signing key using HMACSHA256 algorithm and the provided secret key
            var key = new SymmetricSecurityKey(Convert.FromBase64String(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create claims for the token
        //    var claims = new[]
        //    {
        //    new Claim(JwtRegisteredClaimNames.Sub, "example_user"),
        //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //};

            // Create a JWT token with a specified lifetime, claims, and signing credentials
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes), // Token will expire in 15 minutes
                signingCredentials: creds
            );

            // Serialize the token to a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static (bool,List<Claim>) ValidateToken(string token, string secretKey, string issuer, string audience)
        {
            try
            {
                token = token.Replace("Bearer ", "");
                // Create a validation parameters object
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(secretKey))
                };

                // Validate the token using the validation parameters
                var handler = new JwtSecurityTokenHandler();
                
                var principal = handler.ValidateToken(token, validationParameters, out _);
                

                return (true,principal.Claims.ToList()); // Token is valid
            }
            catch (Exception)
            {
                return (false,new List<Claim>()); // Token validation failed
            }
        }
    }

}
