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
        static async Task Main(string[] args)
        {

            try
            {
                Configuration config = new Configuration();
                config.BasePath = "https://localhost:7212";
                config.DefaultHeaders.Add("Authorization", "Bearer " + GetToken(1));
                var apiInstance = new SampleApi(config);
                var data = "data_example";  // string? |  (optional) 

                for (int i = 0; i < 2; i++)
                {
                    try
                    {
                        await apiInstance.ApiSampleGetLimitGlobal5PreSecondUser3PreSecondGetAsync(data);
                        Console.WriteLine("OK");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ERROR:{ex.Message}");
                    }
                }
            }
            catch (ApiException e)
            {
                Console.WriteLine("Exception when calling SampleApi.ApiSampleGetNormalGet: " + e.Message);
                Console.WriteLine("Status Code: " + e.ErrorCode);
                Console.WriteLine(e.StackTrace);
            }

            Console.ReadLine();
        }

        static async Task Test1()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"---Test---");
            Console.ResetColor();

            Stopwatch sw = new();

            //TODO: select one..
            var todo = @"
*left to right
seconds: 0---1---2---3---4---5
minutes: 0..
hours:   0..
unknown0:1o--|
          2o--|
           3o--|
            4x--|
unknown1:o
user0:   o o o o o
user1:   o x x x x...

*top to bottom
time      | unknown1 unknown2 user1
     0 sec       1/3      2/3   1/1
   0.5 sec       3/3            2/1
   0.9 sec                4/3
     1 sec       3/3            2/1
   1.1 sec                4/3

*statuses board
global controller rate limit
...
global action rate limit
...
ip rate limit
...
user rate limit
...
";

            Console.WriteLine();
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