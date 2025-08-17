using Timer = System.Timers.Timer;

namespace ZRadio;

internal class Program
{
    private static async Task Main()
    {
        Server.Start();
        await Client.RequestAuthAsync();
        Server.Redirect();
        await Client.RequestAccessTokenAsync(Server.State, Server.Code);

        using var timer = new Timer(TimeSpan.FromSeconds(Client.GetRefreshTimer()).TotalMilliseconds);
        timer.AutoReset = true;
        timer.Elapsed += async (_, _) => await Client.RequestRefreshTokenAsync();
        timer.Enabled = true;
        timer.Start();

        using var keyboardListener = new KeyboardListener();
        keyboardListener.Run();

        Application.Run();
    }
}