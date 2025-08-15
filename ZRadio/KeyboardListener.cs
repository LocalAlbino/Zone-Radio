using SharpHook;
using SharpHook.Data;

namespace ZRadio;

internal class KeyboardListener : IDisposable
{
    private readonly EventLoopGlobalHook _hook = new();
    private bool _isPdaOpen;

    public KeyboardListener()
    {
        _hook.KeyPressed += OnKeyPressed;
    }

    public void Dispose()
    {
        _hook.Dispose();
    }

    ~KeyboardListener()
    {
        Stop();
        Dispose();
    }

    public void Run()
    {
        _hook.Run();
        Console.WriteLine("Keyboard listener started.");
    }

    private void Stop()
    {
        _hook.Stop();
        Console.WriteLine("Keyboard listener stopped.");
    }

    private void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        // Might change this to an enum at some point but
        // these keys are pretty set in stone for my keyboard setup.
        var keyCode = e.Data.KeyCode;
        switch (keyCode)
        {
            case KeyCode.VcTab:
                Console.WriteLine($"Key pressed: {keyCode}");
                _isPdaOpen = !_isPdaOpen;
                Console.WriteLine($"PDA State toggled: {_isPdaOpen}\n");
                break;
            case KeyCode.VcLeft:
                Console.WriteLine($"Key pressed: {keyCode}");
                if (_isPdaOpen) _ = Client.ToggleDevicePlaybackAsync();
                break;
            case KeyCode.VcRight:
                Console.WriteLine($"Key pressed: {keyCode}");
                if (_isPdaOpen) _ = Client.SkipPlaybackAsync();
                break;
        }
    }
}