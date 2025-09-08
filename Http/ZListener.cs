using System.Net;
using System.Text;

namespace Zone_Radio.Http
{
    internal class ZListener
    {
        private HttpListener _listener;
        public TaskCompletionSource<bool> Ready { get; private set; }
        public string RedirectUri { get; }
        public string Code { get; private set; }
        public string State { get; private set; }

        public ZListener(string redirectUri)
        {
            _listener = new HttpListener();
            RedirectUri = redirectUri;
            Ready = new TaskCompletionSource<bool>(false);
        }

        public void Start()
        {
            _listener.Prefixes.Add(RedirectUri);
            _listener.Start();
        }

        public void Stop()
        {
            if (IsListening()) _listener.Stop();
        }

        public bool IsListening()
        {
            return _listener.IsListening;
        }

        public async Task RedirectAsync()
        {
            if (!IsListening()) return;

            Ready.TrySetResult(true); // Let client know we are ready to accept a connection
            var ctx = await _listener.GetContextAsync();
            Code = ctx.Request.QueryString["code"];
            State = ctx.Request.QueryString["state"];

            var resp = Encoding.UTF8.GetBytes("<html><body>Login successful. You can close this page now.</body></html>");
            ctx.Response.ContentLength64 = resp.Length;
            ctx.Response.ContentType = "text/html";
            ctx.Response.StatusCode = 200;
            ctx.Response.OutputStream.Write(resp, 0, resp.Length);

            Stop();
        }
    }
}
