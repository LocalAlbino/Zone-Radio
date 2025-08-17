using System.Diagnostics;
using System.Net;
using System.Text;

namespace ZRadio;

internal static class Server
{
    private static readonly HttpListener Listener = new();

    public static string RedirectUri { get; } = "http://127.0.0.1:8888/callback/";
    public static string? State { get; private set; }
    public static string? Code { get; private set; }

    public static void Start()
    {
        Listener.Prefixes.Add(RedirectUri);
        Listener.Start();
        Console.WriteLine($"Listening on {RedirectUri}");
    }

    public static void Redirect()
    {
        Trace.Assert(Listener.IsListening, "Server could not start.");

        var ctx = Listener.GetContext();
        Code = ctx.Request.QueryString["code"];
        State = ctx.Request.QueryString["state"];

        var resp = Encoding.UTF8.GetBytes("<html><body>Login successful. You can close this page now.</body></html>");
        ctx.Response.ContentLength64 = resp.Length;
        ctx.Response.ContentType = "text/html";
        ctx.Response.StatusCode = 200;
        ctx.Response.OutputStream.Write(resp, 0, resp.Length);

        Listener.Stop();
    }

    private static bool IsCodeOrStateNull()
    {
        return Code == null || State == null;
    }
}