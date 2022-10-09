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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonBotStart);
            this.Name = "MainForm";
            this.Text = "Configurator";
            this.ResumeLayout(false);

        }

        #endregion

        private Button buttonBotStart;
        private Button button1;
    }
}