using System.Diagnostics;

namespace TeleBotOrders
{
    public partial class MainForm : Form
    {
        private TeleBot _teleBot;
        public static string StringConnection { get; set; } = "Host=localhost;Port=5432;Database=usersdb;Username=postgres;Password=31428";
        public MainForm()
        {
            InitializeComponent();
           // DBController.CreateTestCafe();
        }

        private void buttonBotStart_Click(object sender, EventArgs e)
        {
            _teleBot = new TeleBot();
            _teleBot.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _teleBot.Stop();
        }

        private void buttonChangeStringConnection_Click(object sender, EventArgs e)
        {
            StringConnection = $"Host={fieldHost.Text};Port={fieldPort.Text};Database={fieldDatabase.Text};Username={fieldUsername};Password={fieldPassword}";
        }
    }
}