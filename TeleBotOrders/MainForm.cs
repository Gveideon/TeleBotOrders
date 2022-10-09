using System.Diagnostics;

namespace TeleBotOrders
{
    public partial class MainForm : Form
    {
        private TeleBot _teleBot;
        public MainForm()
        {
            InitializeComponent();
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
    }
}