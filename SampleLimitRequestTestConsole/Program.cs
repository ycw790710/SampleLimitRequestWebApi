using Microsoft.IdentityModel.Tokens;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SampleLimitRequestTestConsole
{
    internal class Program
    {
        static bool _alive = true;
        static string _basePath = "https://localhost:7212";
        static async Task Main(string[] args)
        {
            await Test1();

            Console.ReadLine();
        }

        private static async Task Test1()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"---{nameof(Test1)}---");
            Console.ResetColor();

            var dateTimePattern = "yyyy/MM/dd hh:mm:ss.fff tt";

            Stopwatch sw = new();
            //TODO: Test1
            try
            {
                Configuration config = new Configuration();
                config.BasePath = _basePath;
                config.DefaultHeaders.Add("Authorization", "Bearer " + GetToken(1));
                var apiInstance = new SampleApi(config);
                var data = "data_example";  // string? |  (optional) 

                Console.WriteLine($"{nameof(apiInstance.ApiSampleGetLimitGlobal5PreSecondUser3PreSecondGetAsync)}");
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        await apiInstance.ApiSampleGetLimitGlobal5PreSecondUser3PreSecondGetAsync(data);
                        Console.WriteLine($"[{DateTime.Now.ToString(dateTimePattern)}] Ok");
                    }
                    catch
                    {
                        Console.WriteLine($"[{DateTime.Now.ToString(dateTimePattern)}] Fail");
                    }
                }
            }
            catch
            {
            }
        }

        static string GetTimeStr(int milliseconds)
        {
            TimeSpan ts = TimeSpan.FromMilliseconds(milliseconds);
            return $"{ts.Hours}:{ts.Minutes:D2}:{ts.Seconds:D2}";
        }

        static string GetToken(int userId)
        {
            var signingKey = new SymmetricSecurityKey(GetSecretKey());

            var claims = CreateClaims(userId);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: "SampleLimitRequest",
                audience: "SampleLimitRequest",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        static byte[] GetSecretKey()
        {
            var bytes = Encoding.UTF8.GetBytes("a98dghmnibqutldimpga08hpm3h;ovihdg;029;vty;d0aest0oiassad9pnyvg39wyh08tyvaote");
            Array.Resize(ref bytes, 64);
            return bytes;
        }

        static IEnumerable<Claim> CreateClaims(int userId)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userId.ToString()),
        };
            return claims;
        }

    }
}