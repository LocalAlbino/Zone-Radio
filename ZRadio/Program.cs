namespace ZRadio;

internal class Program
{
    private static async Task Main()
    {
        _ = Server.Redirect();
        await Client.RequestAuthAsync();
        await Client.RequestAccessTokenAsync(Server.State, Server.Code);
        Application.Run();
    }
}