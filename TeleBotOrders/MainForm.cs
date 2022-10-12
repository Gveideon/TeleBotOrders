using System.Text.Json;
using System.Diagnostics;
using System.Windows.Forms;

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

        private void buttonUpdateByJson_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog.FileName;
            string json = System.IO.File.ReadAllText(filename);
            var dishes = JsonSerializer.Deserialize<List<List<string>>>(json);
            List<Dish> dishList = new List<Dish>(); 
            foreach (var dish in dishes)
            {
                double price = 0;
                string temp = "0";
                if(dish[4] != null)
                    temp =  new string(((string)dish[4]).Select(x => x).Where(x => Char.IsDigit(x)).ToArray());
                double.TryParse(temp, out price);
                dishList.Add(new Dish {Name = (string)dish[0], Description = (string)dish[2] + " " + (string)dish[1], PathImage = (string)dish[3], Price = price});
            }
            DBController.UpdateCafe(dishList, fieldNameCafe.Text);
        }
    }
}