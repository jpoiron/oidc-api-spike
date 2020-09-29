using System.Text.Json.Serialization;

namespace OidcApi.Web
{
    public class OpenIdUserInfo
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }

        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; }
        
        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
}