using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace MeterReaderLib
{
    public class MeterReaderTokenValidationParameters : TokenValidationParameters
    {
        public MeterReaderTokenValidationParameters(IConfiguration config)
        {
            ValidIssuer = config["Tokens:Issuer"];
            ValidAudience = config["Tokens:Audience"];
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Tokens:Key"]!));
        }
    }
}