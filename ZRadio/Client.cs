using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using DotNetEnv;

namespace ZRadio;

internal static class Client
{
    private static TokenResponse _accessToken;
    private static AuthResponse _authResponse;

    public static int GetRefreshTimer()
    {
        return _accessToken.ExpiresIn;
    }

    public static async Task RequestAuthAsync()
    {
        Env.Load();
        _authResponse.State = Guid.NewGuid().ToString();
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["response_type"] = "code";
        query["client_id"] = Env.GetString("CLIENT_ID");
        query["scope"] = "user-read-playback-state user-modify-playback-state user-read-private user-read-email";
        query["redirect_uri"] = Server.RedirectUri;
        query["state"] = _authResponse.State;
        var url = "https://accounts.spotify.com/authorize?" + query;

        Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });

        Console.WriteLine("Opening authorization...\n");
        using var client = new HttpClient();
        _ = await client.GetAsync(url);
    }

    public static async Task RequestAccessTokenAsync(string? state, string? code)
    {
        Trace.Assert(state == _authResponse.State, $"State mismatch; expected {_authResponse.State}, received {state}");

        Env.Load();
        var apiKey = Env.GetString("SUPER_SECRET_API_KEY");
        var clientId = Env.GetString("CLIENT_ID");

        _authResponse.Code = code;
        var body = new FormUrlEncodedContent(new Dictionary<string, string?>
        {
            { "grant_type", "authorization_code" },
            { "code", _authResponse.Code },
            { "redirect_uri", Server.RedirectUri }
        });

        var authString = Encoding.UTF8.GetBytes(clientId + ":" + apiKey);

        Console.WriteLine("Requesting access token...");
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization",
            $"Basic {Convert.ToBase64String(authString)}");
        var resp = await client.PostAsync("https://accounts.spotify.com/api/token", body);
        _accessToken = JsonSerializer.Deserialize<TokenResponse>(await resp.Content.ReadAsStreamAsync());

        Console.WriteLine("Initial access token recieved.");
        Console.WriteLine($"Access token: {_accessToken.AccessToken}");
        Console.WriteLine($"Refresh Token: {_accessToken.RefreshToken}");
        Console.WriteLine($"Token type: {_accessToken.TokenType}");
        Console.WriteLine($"Expires in: {_accessToken.ExpiresIn}");
        Console.WriteLine($"Scope: {_accessToken.Scope}\n");
    }

    public static async Task RequestRefreshTokenAsync()
    {
        Env.Load();
        var clientId = Env.GetString("CLIENT_ID");

        var body = new FormUrlEncodedContent(new Dictionary<string, string?>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", _accessToken.RefreshToken }
        });

        Console.WriteLine("Requesting refresh token...\n");
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization",
            $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId))}");
        var resp = await client.PostAsync("https://accounts.spotify.com/api/token", body);
        _accessToken = JsonSerializer.Deserialize<TokenResponse>(await resp.Content.ReadAsStreamAsync());

        Console.WriteLine("Refresh token recieved.");
        Console.WriteLine($"Access token: {_accessToken.AccessToken}");
        Console.WriteLine($"Refresh Token: {_accessToken.RefreshToken}");
        Console.WriteLine($"Token type: {_accessToken.TokenType}");
        Console.WriteLine($"Expires in: {_accessToken.ExpiresIn}");
        Console.WriteLine($"Scope: {_accessToken.Scope}\n");
    }

    private static async Task<bool> GetDevicePlaybackStateAsync()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken.AccessToken);

        Console.WriteLine("Getting playback state...");
        var resp = await client.GetAsync("https://api.spotify.com/v1/me/player");
        var playbackState = JsonSerializer.Deserialize<PlaybackState>(await resp.Content.ReadAsStreamAsync());
        return playbackState.IsPlaying;
    }

    public static async Task<bool> IsUserPremiumAsync()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken.AccessToken);

        Console.WriteLine("Checking is user has premium...\n");
        var resp = await client.GetAsync("https://api.spotify.com/v1/me");
        var product = JsonSerializer.Deserialize<HasPremium>(await resp.Content.ReadAsStreamAsync());
        return product.IsPremium.Equals("premium");
    }

    public static async Task ToggleDevicePlaybackAsync()
    {
        Trace.Assert(IsUserPremiumAsync().Result,
            "User is not a premium user. Cannot make API calls for free users.");

        var playbackState = await GetDevicePlaybackStateAsync();
        if (playbackState)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken.AccessToken);

            Console.WriteLine("Pausing playback...\n");
            await client.PutAsync("https://api.spotify.com/v1/me/player/pause", null);
        }
        else
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken.AccessToken);

            Console.WriteLine("Resuming playback...\n");
            await client.PutAsync("https://api.spotify.com/v1/me/player/play", null);
        }
    }

    public static async Task SkipPlaybackAsync()
    {
        Trace.Assert(IsUserPremiumAsync().Result,
            "User is not a premium user. Cannot make API calls for free users.");

        var playbackState = await GetDevicePlaybackStateAsync();
        if (playbackState)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken.AccessToken);

            Console.WriteLine("Skipping playback...\n");
            await client.PostAsync("https://api.spotify.com/v1/me/player/next", null);
        }
    }

    private struct TokenResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; }
        [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
        [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; }
        [JsonPropertyName("token_type")] public string TokenType { get; set; }
        [JsonPropertyName("scope")] public string Scope { get; set; }
    }

    private struct AuthResponse
    {
        public string? Code { get; set; }
        public string State { get; set; }
    }

    private struct PlaybackState
    {
        [JsonPropertyName("is_playing")] public bool IsPlaying { get; set; }
    }

    private struct HasPremium
    {
        [JsonPropertyName("product")] public string IsPremium { get; init; }
    }
}