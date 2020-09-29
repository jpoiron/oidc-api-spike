using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace OidcApi.Web
{
    public class JwtBearerEventOverrides : JwtBearerEvents
    {
        public override async Task TokenValidated(TokenValidatedContext context)
        {
            // Add the access_token as a claim, as we may actually need it
            if (context.SecurityToken is JwtSecurityToken accessToken)
            {
                if (context.Principal.Identity is ClaimsIdentity identity)
                {
                    identity.AddClaim(new Claim("access_token", accessToken.RawData));
                    await GetUserProfileAsync(identity);
                }
            }
            
            await base.TokenValidated(context);
        }

        private async Task GetUserProfileAsync(ClaimsIdentity identity)
        {
            var client = new HttpClient();
            var accessToken = identity.FindFirst("access_token");
            
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Value);

            var response = await client.GetAsync("https://local.www.scrum.org/oauth2/UserInfo");

            response.EnsureSuccessStatusCode();

            var responseText = await response.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<OpenIdUserInfo>(responseText);

            if (userInfo != null && !string.IsNullOrEmpty(userInfo.Role))
            {
                var roles = userInfo.Role.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var role in roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }
        }
    }
}