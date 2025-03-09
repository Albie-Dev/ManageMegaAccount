namespace MMA.Domain
{
    using Newtonsoft.Json;

    public class GoogleOAuth2TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; } = string.Empty;

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonProperty("scope")]
        public string Scope { get; set; } = string.Empty;

        [JsonProperty("id_token")]
        public string IdToken { get; set; } = string.Empty;

        [JsonProperty("issued_token_type")]
        public string IssuedTokenType { get; set; } = string.Empty;
    }


    public class GoogleUserInfo
    {
        [JsonProperty("sub")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("picture")]
        public string Picture { get; set; } = string.Empty;

        [JsonProperty("locale")]
        public string Locale { get; set; } = string.Empty;
    }


}