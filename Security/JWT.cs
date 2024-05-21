using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Claims;
using HRMS.Models;
using Newtonsoft.Json;
namespace HRMS.Security
{
    public class JWT
    {
        private static string SecretKey = System.Configuration.ConfigurationManager.AppSettings["JWTKey"];
        public static string GenerateJWTToken(object _obj)
        {
            var key = Encoding.ASCII.GetBytes(SecretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            string json = JsonConvert.SerializeObject(_obj);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, json)
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "http://localhost",
                Audience = "http://localhost"
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public static ClaimsPrincipal ValidateToken(string token)
        {
            var key = Encoding.ASCII.GetBytes(SecretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            LogJwtTokenSegments(token);
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "http://localhost",
                    ValidAudience = "http://localhost",
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);
                return principal;
            }
               catch(Exception ex)
            {
                return null;
            }
          
        }


        public static void LogJwtTokenSegments(string token)
        {
            var segments = token.Split('.');
            if (segments.Length == 3)
            {
                var header = Base64UrlDecode(segments[0]);
                var payload = Base64UrlDecode(segments[1]);
                var signature = segments[2];

                System.Diagnostics.Debug.WriteLine($"Header: {header}");
                System.Diagnostics.Debug.WriteLine($"Payload: {payload}");
                System.Diagnostics.Debug.WriteLine($"Signature: {signature}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Invalid JWT Token: {token}");
            }
        }

        public static string Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: throw new Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return Encoding.UTF8.GetString(converted);
        }

        public static JwtSecurityToken DecodeJwtToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(token);
        }
    }
}