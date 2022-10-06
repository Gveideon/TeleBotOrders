using System.Diagnostics;

namespace TeleBotOrders
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            // ���������� ������
            using (ApplicationContext db = new ApplicationContext())
            {
                // ������� ��� ������� User
                User user1 = new User { Name = "Tom", Age = 33 };
                User user2 = new User { Name = "Alice", Age = 26 };

                // ��������� �� � ��
                db.Users.AddRange(user1, user2);
                db.SaveChanges();
            }
            // ��������� ������
            using (ApplicationContext db = new ApplicationContext())
            {
                // �������� ������� �� �� � ������� �� �������
                var users = db.Users.ToList();
                Debug.WriteLine("Users list:");
                foreach (User u in users)
                {
                    Debug.WriteLine($"{u.Id}.{u.Name} - {u.Age}");
                }
            }
        }

        private void buttonBotStart_Click(object sender, EventArgs e)
        {
            TeleBot tele = new TeleBot();
            tele.Start();
        }
    }
}