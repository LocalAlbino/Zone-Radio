using System.Net;
using System.Text;

namespace ZRadio;

internal static class Server
{
    public static string RedirectUri { get; } = "http://127.0.0.1:8888/callback/";
    public static string? State { get; set; }
    public static string? Code { get; set; }

    public static async Task Redirect()
    {
        var server = new HttpListener();
        server.Prefixes.Add(RedirectUri);
        server.Start();
        var ctx = await server.GetContextAsync();
        var buf = Encoding.UTF8.GetBytes("<html><body>Login successful. This page can be closed now.</body></html>");
        ctx.Response.ContentLength64 = buf.Length;
        var queryParams = ctx.Request.QueryString;
        await ctx.Response.OutputStream.WriteAsync(buf);
        ctx.Response.OutputStream.Close();
        server.Stop();
        Code = queryParams["code"];
        State = queryParams["state"];
    }
}