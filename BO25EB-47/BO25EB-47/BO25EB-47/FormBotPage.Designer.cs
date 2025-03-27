namespace BO25EB_47
{
    partial class FormBotPage
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
            btnBotPageHome = new Button();
            btnBotNewGame = new Button();
            btnBotCustomPos = new Button();
            cbBotDifficulty = new ComboBox();
            cbBotPlaysAs = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // btnBotPageHome
            // 
            btnBotPageHome.Location = new Point(328, 101);
            btnBotPageHome.Name = "btnBotPageHome";
            btnBotPageHome.Size = new Size(167, 41);
            btnBotPageHome.TabIndex = 0;
            btnBotPageHome.Text = "Go to main menu";
            btnBotPageHome.UseVisualStyleBackColor = true;
            btnBotPageHome.Click += btnBotPageHome_Click;
            // 
            // btnBotNewGame
            // 
            btnBotNewGame.Location = new Point(242, 345);
            btnBotNewGame.Name = "btnBotNewGame";
            btnBotNewGame.Size = new Size(167, 45);
            btnBotNewGame.TabIndex = 1;
            btnBotNewGame.Text = "New game";
            btnBotNewGame.UseVisualStyleBackColor = true;
            btnBotNewGame.Click += btnBotNewGame_Click;
            // 
            // btnBotCustomPos
            // 
            btnBotCustomPos.Location = new Point(415, 345);
            btnBotCustomPos.Name = "btnBotCustomPos";
            btnBotCustomPos.Size = new Size(167, 45);
            btnBotCustomPos.TabIndex = 2;
            btnBotCustomPos.Text = "Custom posision";
            btnBotCustomPos.UseVisualStyleBackColor = true;
            btnBotCustomPos.Click += btnBotCustomPos_Click;
            // 
            // cbBotDifficulty
            // 
            cbBotDifficulty.FormattingEnabled = true;
            cbBotDifficulty.Location = new Point(242, 265);
            cbBotDifficulty.Name = "cbBotDifficulty";
            cbBotDifficulty.Size = new Size(167, 33);
            cbBotDifficulty.TabIndex = 3;
            // 
            // cbBotPlaysAs
            // 
            cbBotPlaysAs.FormattingEnabled = true;
            cbBotPlaysAs.Location = new Point(415, 265);
            cbBotPlaysAs.Name = "cbBotPlaysAs";
            cbBotPlaysAs.Size = new Size(167, 33);
            cbBotPlaysAs.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(242, 228);
            label1.Name = "label1";
            label1.Size = new Size(82, 25);
            label1.TabIndex = 5;
            label1.Text = "Difficulty";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(415, 228);
            label2.Name = "label2";
            label2.Size = new Size(74, 25);
            label2.TabIndex = 6;
            label2.Text = "Plays as";
            // 
            // FormBotPage
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(cbBotPlaysAs);
            Controls.Add(cbBotDifficulty);
            Controls.Add(btnBotCustomPos);
            Controls.Add(btnBotNewGame);
            Controls.Add(btnBotPageHome);
            Name = "FormBotPage";
            Text = "FormBotPage";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnBotPageHome;
        private Button btnBotNewGame;
        private Button btnBotCustomPos;
        private ComboBox cbBotDifficulty;
        private ComboBox cbBotPlaysAs;
        private Label label1;
        private Label label2;
    }
}