using SharpHook.Data;
using Zone_Radio.Http;
using Zone_Radio.Utility;

namespace Zone_Radio
{
    public partial class MainWindow : Form
    {
        private ZListener _listener;
        private ZHook _hook;
        private System.Timers.Timer _refreshTimer;
        private System.Windows.Forms.Timer _uiTimer;
        private int _uiTimerCurrentTime;

        public MainWindow()
        {
            _listener = new ZListener("http://127.0.0.1:8888/callback/");
            _hook = new ZHook();
            InitializeComponent();

            _uiTimerCurrentTime = 0;
            _uiTimer = new System.Windows.Forms.Timer(); // Connects to UI thread
            _refreshTimer = new System.Timers.Timer(); // Connects to non-UI thread

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

        private void InitializeRefreshTimer(int expiresIn)
        {
            _refreshTimer.Interval = expiresIn * 1000; // Convert seconds to milliseconds
            _refreshTimer.Enabled = true;
            _refreshTimer.Elapsed += async (_, _) =>
            {
                _refreshTimer.Enabled = false;
                _uiTimer.Enabled = false;
                lblRefreshTimer.Text = "Time until refresh: --:--";
                bool reconnected = await TryRefreshConnectionAsync();

                if (reconnected) // Only re-enable if successful
                {
                    _refreshTimer.Enabled = true;
                }
            };
        }

        private void InitializeUiTimer(int expiresIn)
        {
            _uiTimerCurrentTime = expiresIn;

            _uiTimer.Interval = 1000; // 1 second
            _uiTimer.Enabled = true;
            _uiTimer.Tick += async (_, _) =>
            {
                _uiTimerCurrentTime--;
                int minutes = _uiTimerCurrentTime / 60;
                int seconds = _uiTimerCurrentTime % 60;
                if (seconds < 10)
                {
                    lblRefreshTimer.Text = $"Time until refresh: {minutes}:0{seconds}";
                }
                else
                {
                    lblRefreshTimer.Text = $"Time until refresh: {minutes}:{seconds}";
                }
            };
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            lblConnectionStatus.Text = "Connecting to Spotify API.";
            await ConnectAsync();
        }

        private async Task ConnectAsync()
        {
            var env = new ZEnv();
            var client = new ZClient(env.ClientId, env.ClientSecret);
            try
            {
                _listener.Start();
                _ = _listener.RedirectAsync(); // Start listening
                await _listener.Ready.Task; // Wait until listener is ready
                await client.RequestAuthorizationAsync(_listener.RedirectUri);
                // Initial request called once, needs refreshed every hour
                int expiresIn = await client.RequestAccessTokenAsync(_listener.State, _listener.Code,
                    _listener.RedirectUri);
                _listener.Stop();
                bool hasPremium = await client.RequestUserProductAsync(); // Ensure that the user has a premium account
                System.Diagnostics.Debug.WriteLine(hasPremium);
                if (!hasPremium)
                {
                    MessageBox.Show("You must have a premium subscription for Zone Radio to work. Sorry!",
                        "Premium Required!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Close(); // User cannot use the app without premium, so exit
                }

                // Update ui and start refresh timer
                InitializeRefreshTimer(expiresIn);
                InitializeUiTimer(expiresIn);
                btnConnect.Enabled = false;
                lblConnectionStatus.Text = "Connected to Spotify API.";
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                if (_listener.IsListening()) _listener.Stop();

                lblConnectionStatus.Text = "Failed to connect to Spotify API.";
                await ConnectDebounceAsync();
            }
        }

        private async Task<bool> TryRefreshConnectionAsync()
        {
            int maxRetries = 3;
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    await RefreshConnectionAsync();
                    lblConnectionStatus.Text = "Connected to Spotify API.";
                    return true; // Exit if successful
                }
                catch
                {
                    await Task.Delay(2000);
                }
            }

            lblConnectionStatus.Text = "Failed to refresh Spotify API connection.";
            btnConnect.Enabled = true; // Allow user to try getting new connection
            return false; // Unsuccessful
        }

        private async Task RefreshConnectionAsync()
        {
            var env = new ZEnv();
            var client = new ZClient(env.ClientId, env.ClientSecret);

            lblConnectionStatus.Text = "Refreshing Spotify API connection.";
            await client.RequestRefreshTokenAsync();
        }

        private async Task ConnectDebounceAsync()
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
