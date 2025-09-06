namespace Zone_Radio
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            cboxTogglePlayback1.DataSource = Enum.GetValues(typeof(SharpHook.Data.KeyCode));
            cboxTogglePlayback2.DataSource = Enum.GetValues(typeof(SharpHook.Data.KeyCode));
            cboxSkipPlayback1.DataSource = Enum.GetValues(typeof(SharpHook.Data.KeyCode));
            cboxSkipPlayback2.DataSource = Enum.GetValues(typeof(SharpHook.Data.KeyCode));
            cboxTogglePda1.DataSource = Enum.GetValues(typeof(SharpHook.Data.KeyCode));
            cboxTogglePda2.DataSource = Enum.GetValues(typeof(SharpHook.Data.KeyCode));

            lblConnectionStatus.Text = "Not connected to Spotify API.";
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

        }
    }
}
