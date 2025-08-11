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
        query["scope"] = "user-read-playback-state user-modify-playback-state";
        query["redirect_uri"] = Server.RedirectUri;
        query["state"] = _authResponse.State;
        var url = "https://accounts.spotify.com/authorize?" + query;

        Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });

        using var client = new HttpClient();
        _ = await client.GetAsync(url);
    }

    public static async Task RequestAccessTokenAsync(string? state, string? code)
    {
        if (state != _authResponse.State)
            throw new Exception($"State mismatch; expected {_authResponse.State}, received {state}");

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

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization",
            $"Basic {Convert.ToBase64String(authString)}");
        var resp = await client.PostAsync("https://accounts.spotify.com/api/token", body);
        _accessToken = JsonSerializer.Deserialize<TokenResponse>(await resp.Content.ReadAsStreamAsync());
        Console.WriteLine(_accessToken.AccessToken);
        Console.WriteLine(_accessToken.ExpiresIn);
        Console.WriteLine(_accessToken.TokenType);
        Console.WriteLine(_accessToken.RefreshToken);
        Console.WriteLine(_accessToken.Scope);
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

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization",
            $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId))}");
        var resp = await client.PostAsync("https://accounts.spotify.com/api/token", body);
        _accessToken = JsonSerializer.Deserialize<TokenResponse>(await resp.Content.ReadAsStreamAsync());
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
}