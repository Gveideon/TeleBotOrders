namespace TeleBotOrders
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonBotStart = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fieldHost = new System.Windows.Forms.TextBox();
            this.fieldPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.fieldDatabase = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.fieldUsername = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.fieldPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonChangeStringConnection = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonBotStart
            // 
            this.buttonBotStart.Location = new System.Drawing.Point(12, 12);
            this.buttonBotStart.Name = "buttonBotStart";
            this.buttonBotStart.Size = new System.Drawing.Size(94, 29);
            this.buttonBotStart.TabIndex = 0;
            this.buttonBotStart.Text = "Start bot";
            this.buttonBotStart.UseVisualStyleBackColor = true;
            this.buttonBotStart.Click += new System.EventHandler(this.buttonBotStart_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(112, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 29);
            this.button1.TabIndex = 1;
            this.button1.Text = "Stop bot";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonChangeStringConnection);
            this.groupBox1.Controls.Add(this.fieldPassword);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.fieldUsername);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.fieldDatabase);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.fieldPort);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.fieldHost);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.groupBox1.Location = new System.Drawing.Point(303, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(485, 290);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Строка подключения  бд";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "Host";
            // 
            // fieldHost
            // 
            this.fieldHost.Location = new System.Drawing.Point(118, 33);
            this.fieldHost.Name = "fieldHost";
            this.fieldHost.Size = new System.Drawing.Size(320, 34);
            this.fieldHost.TabIndex = 1;
            // 
            // fieldPort
            // 
            this.fieldPort.Location = new System.Drawing.Point(118, 73);
            this.fieldPort.Name = "fieldPort";
            this.fieldPort.Size = new System.Drawing.Size(320, 34);
            this.fieldPort.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 28);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port";
            // 
            // fieldDatabase
            // 
            this.fieldDatabase.Location = new System.Drawing.Point(119, 113);
            this.fieldDatabase.Name = "fieldDatabase";
            this.fieldDatabase.Size = new System.Drawing.Size(320, 34);
            this.fieldDatabase.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 28);
            this.label3.TabIndex = 4;
            this.label3.Text = "Database";
            // 
            // fieldUsername
            // 
            this.fieldUsername.Location = new System.Drawing.Point(119, 153);
            this.fieldUsername.Name = "fieldUsername";
            this.fieldUsername.Size = new System.Drawing.Size(320, 34);
            this.fieldUsername.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 156);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 28);
            this.label4.TabIndex = 6;
            this.label4.Text = "Username";
            // 
            // fieldPassword
            // 
            this.fieldPassword.Location = new System.Drawing.Point(119, 193);
            this.fieldPassword.Name = "fieldPassword";
            this.fieldPassword.Size = new System.Drawing.Size(320, 34);
            this.fieldPassword.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 196);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 28);
            this.label5.TabIndex = 8;
            this.label5.Text = "Password";
            // 
            // buttonChangeStringConnection
            // 
            this.buttonChangeStringConnection.Location = new System.Drawing.Point(13, 242);
            this.buttonChangeStringConnection.Name = "buttonChangeStringConnection";
            this.buttonChangeStringConnection.Size = new System.Drawing.Size(136, 42);
            this.buttonChangeStringConnection.TabIndex = 10;
            this.buttonChangeStringConnection.Text = "Задать";
            this.buttonChangeStringConnection.UseVisualStyleBackColor = true;
            this.buttonChangeStringConnection.Click += new System.EventHandler(this.buttonChangeStringConnection_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonBotStart);
            this.Name = "MainForm";
            this.Text = "Configurator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Button buttonBotStart;
        private Button button1;
        private GroupBox groupBox1;
        private Label label1;
        private TextBox fieldUsername;
        private Label label4;
        private TextBox fieldDatabase;
        private Label label3;
        private TextBox fieldPort;
        private Label label2;
        private TextBox fieldHost;
        private TextBox fieldPassword;
        private Label label5;
        private Button buttonChangeStringConnection;
    }
}