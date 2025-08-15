using Timer = System.Timers.Timer;

namespace ZRadio;

internal class Program
{
    private static async Task Main()
    {
        _ = Server.Redirect();
        await Client.RequestAuthAsync();
        await Client.RequestAccessTokenAsync(Server.State, Server.Code);

        using var timer = new Timer(TimeSpan.FromSeconds(Client.GetRefreshTimer()).TotalMilliseconds);
        timer.AutoReset = true;
        timer.Elapsed += async (sender, e) => await Client.RequestRefreshTokenAsync();
        timer.Enabled = true;
        timer.Start();

        using var keyboardListener = new KeyboardListener();
        keyboardListener.Run();

        Application.Run();
    }
}