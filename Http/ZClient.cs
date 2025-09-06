using System.Diagnostics;
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

            string url = "https://accounts.spotify.com/authorize?" + query;

            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });

            using var client = new HttpClient();
            await client.GetAsync(url);
        }

        private class TokenResponse
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public string scope { get; set; }
        }

        private class AuthorizationResponse
        {
            public string code { get; set; }
            public string state { get; set; }
        }

        public async Task RequestAccessTokenAsync()
        {
            throw new NotImplementedException();
        }
    }
