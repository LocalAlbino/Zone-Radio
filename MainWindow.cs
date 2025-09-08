using DotNetEnv;
using SharpHook.Data;
using Zone_Radio.Http;
using Zone_Radio.Utility;

namespace Zone_Radio
{
    public partial class MainWindow : Form
    {
        private ZClient _client;
        private ZListener _listener;
        private ZHook _hook;

        public MainWindow()
        {
            _listener = new ZListener("http://127.0.0.1:8888/callback/");
            _hook = new ZHook();
            InitializeComponent();

            cboxTogglePlayback1.DataSource = Enum.GetValues(typeof(KeyCode));
            cboxTogglePlayback2.DataSource = Enum.GetValues(typeof(KeyCode));
            cboxSkipPlayback1.DataSource = Enum.GetValues(typeof(KeyCode));
            cboxSkipPlayback2.DataSource = Enum.GetValues(typeof(KeyCode));
            cboxTogglePda1.DataSource = Enum.GetValues(typeof(KeyCode));
            cboxTogglePda2.DataSource = Enum.GetValues(typeof(KeyCode));

            // Default values
            cboxTogglePlayback1.SelectedItem = KeyCode.VcLeft;
            cboxTogglePlayback2.SelectedItem = KeyCode.VcUndefined;
            cboxSkipPlayback1.SelectedItem = KeyCode.VcRight;
            cboxSkipPlayback2.SelectedItem = KeyCode.VcUndefined;
            cboxTogglePda1.SelectedItem = KeyCode.VcTab;
            cboxTogglePda2.SelectedItem = KeyCode.VcP;

            // Toggle PDA status checkbox when event is fired, this way we can extend in the future if needed
            _hook.UpdateUiElements += ckboxPdaStatus_ToggleStatus;

            lblConnectionStatus.Text = "Not connected to Spotify API.";
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            lblConnectionStatus.Text = "Connecting to Spotify API.";
            await Connect();
        }

        private async Task Connect()
        {
            Env.TraversePath().Load();
            string clientId = Env.GetString("CLIENT_ID");
            string clientSecret = Env.GetString("CLIENT_SECRET");
            if (string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(clientId))
            {
                lblConnectionStatus.Text = "Failed to connect to Spotify API.";
                await ConnectDebounce();
                return; // Cannot continue if these values aren't both valid
            }

            _client = new ZClient(clientId, clientSecret);
            try
            {
                _listener.Start();
                _ = _listener.RedirectAsync(); // Start listening
                await _listener.Ready.Task; // Wait until listener is ready
                await _client.RequestAuthorizationAsync(_listener.RedirectUri);
                // Initial request called once, needs refreshed every hour
                await _client.RequestAccessTokenAsync(_listener.State, _listener.Code, clientId, clientSecret,
                    _listener.RedirectUri);

                // Update ui
                btnConnect.Enabled = false;
                lblConnectionStatus.Text = "Connected to Spotify API.";
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                if (_listener.IsListening()) _listener.Stop();

                lblConnectionStatus.Text = "Failed to connect to Spotify API.";
                await ConnectDebounce();
            }
        }

        private async Task ConnectDebounce()
        {
            btnConnect.Enabled = false;
            await Task.Delay(2000); // Prevents spamming API requests
            btnConnect.Enabled = true;
        }

        private void ckboxPdaStatus_ToggleStatus(object sender, EventArgs e)
        {
            ckboxPdaStatus.Checked = !ckboxPdaStatus.Checked;
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _hook.Dispose();
        }

        private void ComboBox_SelectionChangesCommitted(object sender, EventArgs e)
        {
            // We only want to update the hook if a key code is updated, we will just ignore
            // any other possible senders
            if (sender is ComboBox cbox && cbox.SelectedItem is KeyCode code) _hook.UpdateKeybind(cbox, code);
        }
    }
}
