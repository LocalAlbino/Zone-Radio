using DotNetEnv;
using SharpHook.Data;
using Zone_Radio.Http;

namespace Zone_Radio
{
    public partial class MainWindow : Form
    {
        private ZClient _client;
        private ZListener _listener;

        public MainWindow()
        {
            _listener = new ZListener("https://127.0.0.1:8080/");
            InitializeComponent();

            cboxTogglePlayback1.DataSource = Enum.GetValues(typeof(KeyCode));
            cboxTogglePlayback2.DataSource = Enum.GetValues(typeof(KeyCode));
            cboxSkipPlayback1.DataSource = Enum.GetValues(typeof(KeyCode));
            cboxSkipPlayback2.DataSource = Enum.GetValues(typeof(KeyCode));
            cboxTogglePda1.DataSource = Enum.GetValues(typeof(KeyCode));
            cboxTogglePda2.DataSource = Enum.GetValues(typeof(KeyCode));

            // Default values
            cboxTogglePlayback1.SelectedValue = KeyCode.VcLeft;
            cboxTogglePlayback2.SelectedValue = KeyCode.VcUndefined;
            cboxSkipPlayback1.SelectedValue = KeyCode.VcRight;
            cboxSkipPlayback2.SelectedValue = KeyCode.VcUndefined;
            cboxTogglePda1.SelectedValue = KeyCode.VcTab;
            cboxTogglePda2.SelectedValue = KeyCode.VcP;

            lblConnectionStatus.Text = "Not connected to Spotify API.";
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            lblConnectionStatus.Text = "Connecting to Spotify API.";
            await Connect();
        }

        private async Task Connect()
        {
            Env.Load();
            string clientId = Env.GetString("CLIENT_ID");
            string clientSecret = Env.GetString("CLIENT_SECRET");
            if (String.IsNullOrEmpty(clientSecret) || String.IsNullOrEmpty(clientId))
            {
                lblConnectionStatus.Text = "Failed to connect to Spotify API.";
                await ConnectDebounce();
                return; // Cannot continue if these values aren't both valid
            }

            _client = new ZClient(clientId, clientSecret);
            try
            {
                _listener.Start();
                await _client.RequestAuthorizationAsync(_listener.RedirectUri);
                await _client.RequestAccessTokenAsync();
            }
            catch
            {
                if (_listener.IsLIstening()) _listener.Stop();

                lblConnectionStatus.Text = "Failed to connect to Spotify API.";
                await ConnectDebounce();
                return;
            }
        }

        private async Task ConnectDebounce()
        {
            btnConnect.Enabled = false;
            await Task.Delay(2000); // Prevents spamming API requests
            btnConnect.Enabled = true;
        }
    }
}
