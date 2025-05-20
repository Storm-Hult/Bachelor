namespace BO25EB_47
{
    partial class FormOnlinePage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnOnlineHome = new Button();
            btnOnlineStart = new Button();
            Time = new ListBox();
            Requests = new ListBox();
            txtPlayerID = new TextBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // btnOnlineHome
            // 
            btnOnlineHome.Location = new Point(313, 49);
            btnOnlineHome.Name = "btnOnlineHome";
            btnOnlineHome.Size = new Size(112, 34);
            btnOnlineHome.TabIndex = 0;
            btnOnlineHome.Text = "Main menu";
            btnOnlineHome.UseVisualStyleBackColor = true;
            btnOnlineHome.Click += btnOnlineHome_Click;
            // 
            // btnOnlineStart
            // 
            btnOnlineStart.Location = new Point(313, 101);
            btnOnlineStart.Name = "btnOnlineStart";
            btnOnlineStart.Size = new Size(112, 34);
            btnOnlineStart.TabIndex = 1;
            btnOnlineStart.Text = "Start new game";
            btnOnlineStart.UseVisualStyleBackColor = true;
            btnOnlineStart.Click += btnOnlineStart_Click;
            // 
            // Time
            // 
            Time.FormattingEnabled = true;
            Time.ItemHeight = 25;
            Time.Location = new Point(164, 207);
            Time.Name = "Time";
            Time.Size = new Size(180, 129);
            Time.TabIndex = 2;
            // 
            // Requests
            // 
            Requests.FormattingEnabled = true;
            Requests.ItemHeight = 25;
            Requests.Location = new Point(396, 207);
            Requests.Name = "Requests";
            Requests.Size = new Size(180, 129);
            Requests.TabIndex = 3;
            // 
            // txtPlayerID
            // 
            txtPlayerID.Location = new Point(293, 154);
            txtPlayerID.Name = "txtPlayerID";
            txtPlayerID.Size = new Size(150, 31);
            txtPlayerID.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(313, 9);
            label1.Name = "label1";
            label1.Size = new Size(113, 25);
            label1.TabIndex = 5;
            label1.Text = "Online menu";
            // 
            // FormOnlinePage
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
            Controls.Add(txtPlayerID);
            Controls.Add(Requests);
            Controls.Add(Time);
            Controls.Add(btnOnlineStart);
            Controls.Add(btnOnlineHome);
            Name = "FormOnlinePage";
            Text = "FormOnlinePage";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnOnlineHome;
        private Button btnOnlineStart;
        private ListBox Time;
        private ListBox Requests;
        private TextBox txtPlayerID;
        private Label label1;
    }
}