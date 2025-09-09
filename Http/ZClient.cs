using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Zone_Radio.Http
{
    internal class ZClient
    {
        private TokenResponse _accessToken;
        private AuthorizationResponse _authorizationResponse;
        private string _clientId;
        private string _clientSecret;

        public ZClient(string clientId, string clientSecret)
        {
            _accessToken = new TokenResponse();
            _authorizationResponse = new AuthorizationResponse();
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task RequestAuthorizationAsync(string redirectUri)
        {
            _authorizationResponse.state = Guid.NewGuid().ToString();
            var query = HttpUtility.ParseQueryString(String.Empty);
            query["response_type"] = "code";
            query["client_id"] = _clientId;
            query["scope"] = "user-read-playback-state user-modify-playback-state user-read-private user-read-email";
            query["redirect_uri"] = redirectUri;
            query["state"] = _authorizationResponse.state;

            string url = "https://accounts.spotify.com/authorize?" + query;

            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });

            using var client = new HttpClient();
            _ = await client.GetAsync(url);
        }

        public async Task RequestAccessTokenAsync(string state, string code, string redirectUri)
        {
            if (state != _authorizationResponse.state)
            {
                throw new InvalidOperationException($"State mismatch; expected {_authorizationResponse.state}. Got {state}");
            }

            _authorizationResponse.code = code;
            var body = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", _authorizationResponse.code },
                { "redirect_uri", redirectUri }
            });

            var authString = Encoding.UTF8.GetBytes(_clientId + ":" + _clientSecret);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(authString)}");

            var resp = await client.PostAsync("https://accounts.spotify.com/api/token", body);
            _accessToken = JsonSerializer.Deserialize<TokenResponse>(await resp.Content.ReadAsStreamAsync());
        }

        public async Task RequestRefreshTokenAsync()
        {
            var body = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", _accessToken.refresh_token }
            });

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization",
                $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes(_clientId))}");

            var resp = await client.PostAsync("https://accounts.spotify.com/api/token", body);
            _accessToken = JsonSerializer.Deserialize<TokenResponse>(await resp.Content.ReadAsStreamAsync());
        }

        private async Task<bool> RequestDevicePlaybackStateAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken.access_token}");

            var resp = await client.GetAsync("https://api.spotify.com/v1/me/player");
            var playbackState = JsonSerializer.Deserialize<UserDetails>(await resp.Content.ReadAsStreamAsync());
            if (playbackState == null) throw new NullReferenceException("Failed to fetch playback state.");

            return playbackState.is_playing;
        }

        public async Task<bool> RequestUserProductAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken.access_token}");

            var resp = await client.GetAsync("https://api.spotify.com/v1/me");
            var hasPremium = JsonSerializer.Deserialize<UserDetails>(await resp.Content.ReadAsStreamAsync());
            if (hasPremium == null) throw new NullReferenceException("Failed to fetch user data.");

            return hasPremium.product == "premium";
        }

        public async Task TogglePlaybackAsync()
        {
            string url = await RequestDevicePlaybackStateAsync()
                ? "https://api.spotify.com/v1/me/player/pause"
                : "https://api.spotify.com/v1/me/player/play";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken.access_token}");

            _ = await client.PutAsync(url, null);
        }

        public async Task SkipPlaybackAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken.access_token}");

            _ = await client.PutAsync("https://spotify.com/api/v1/me/player/next", null);
        }

        private class TokenResponse // JSON response for access token
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public string scope { get; set; }
        }

        private class AuthorizationResponse // JSON response for authorization code
        {
            public string code { get; set; }
            public string state { get; set; }
        }

        private class UserDetails // JSON response for user profile request
        {
            public bool is_playing { get; }
            public string product { get; }
        }
    }
}
