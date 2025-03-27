namespace BO25EB_47
{
    partial class FormOnlinePlayPage
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
            txtOnlinePlayWTimer = new TextBox();
            txtOnlinePlayBTimer = new TextBox();
            btnOnlinePlayResign = new Button();
            btnOnlinePlayDraw = new Button();
            btnOnlinePlaySendChat = new Button();
            txtOnlinePlayOpponent = new TextBox();
            txtOnlinePlayPlayer = new TextBox();
            label1 = new Label();
            txtOnlinePlayMsg = new TextBox();
            label2 = new Label();
            txtOnlinePlayOppRating = new TextBox();
            txtOnlinePlayPlayerRating = new TextBox();
            lbOnlinePlayMsgs = new ListBox();
            SuspendLayout();
            // 
            // txtOnlinePlayWTimer
            // 
            txtOnlinePlayWTimer.Location = new Point(488, 12);
            txtOnlinePlayWTimer.Name = "txtOnlinePlayWTimer";
            txtOnlinePlayWTimer.Size = new Size(150, 31);
            txtOnlinePlayWTimer.TabIndex = 0;
            // 
            // txtOnlinePlayBTimer
            // 
            txtOnlinePlayBTimer.Location = new Point(654, 12);
            txtOnlinePlayBTimer.Name = "txtOnlinePlayBTimer";
            txtOnlinePlayBTimer.Size = new Size(150, 31);
            txtOnlinePlayBTimer.TabIndex = 1;
            // 
            // btnOnlinePlayResign
            // 
            btnOnlinePlayResign.Location = new Point(35, 388);
            btnOnlinePlayResign.Name = "btnOnlinePlayResign";
            btnOnlinePlayResign.Size = new Size(112, 34);
            btnOnlinePlayResign.TabIndex = 2;
            btnOnlinePlayResign.Text = "Resign";
            btnOnlinePlayResign.UseVisualStyleBackColor = true;
            btnOnlinePlayResign.Click += this.btnOnlinePlayResign_Click;
            // 
            // btnOnlinePlayDraw
            // 
            btnOnlinePlayDraw.Location = new Point(167, 388);
            btnOnlinePlayDraw.Name = "btnOnlinePlayDraw";
            btnOnlinePlayDraw.Size = new Size(112, 34);
            btnOnlinePlayDraw.TabIndex = 3;
            btnOnlinePlayDraw.Text = "Offer draw";
            btnOnlinePlayDraw.UseVisualStyleBackColor = true;
            // 
            // btnOnlinePlaySendChat
            // 
            btnOnlinePlaySendChat.Location = new Point(298, 388);
            btnOnlinePlaySendChat.Name = "btnOnlinePlaySendChat";
            btnOnlinePlaySendChat.Size = new Size(112, 34);
            btnOnlinePlaySendChat.TabIndex = 4;
            btnOnlinePlaySendChat.Text = "Send chat";
            btnOnlinePlaySendChat.UseVisualStyleBackColor = true;
            // 
            // txtOnlinePlayOpponent
            // 
            txtOnlinePlayOpponent.Location = new Point(22, 12);
            txtOnlinePlayOpponent.Name = "txtOnlinePlayOpponent";
            txtOnlinePlayOpponent.Size = new Size(165, 31);
            txtOnlinePlayOpponent.TabIndex = 5;
            // 
            // txtOnlinePlayPlayer
            // 
            txtOnlinePlayPlayer.Location = new Point(243, 12);
            txtOnlinePlayPlayer.Name = "txtOnlinePlayPlayer";
            txtOnlinePlayPlayer.Size = new Size(150, 31);
            txtOnlinePlayPlayer.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(200, 15);
            label1.Name = "label1";
            label1.Size = new Size(29, 25);
            label1.TabIndex = 7;
            label1.Text = "vs";
            // 
            // txtOnlinePlayMsg
            // 
            txtOnlinePlayMsg.Location = new Point(35, 342);
            txtOnlinePlayMsg.Name = "txtOnlinePlayMsg";
            txtOnlinePlayMsg.Size = new Size(375, 31);
            txtOnlinePlayMsg.TabIndex = 8;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(35, 305);
            label2.Name = "label2";
            label2.Size = new Size(48, 25);
            label2.TabIndex = 9;
            label2.Text = "Chat";
            // 
            // txtOnlinePlayOppRating
            // 
            txtOnlinePlayOppRating.Location = new Point(22, 67);
            txtOnlinePlayOppRating.Name = "txtOnlinePlayOppRating";
            txtOnlinePlayOppRating.Size = new Size(165, 31);
            txtOnlinePlayOppRating.TabIndex = 10;
            // 
            // txtOnlinePlayPlayerRating
            // 
            txtOnlinePlayPlayerRating.Location = new Point(243, 67);
            txtOnlinePlayPlayerRating.Name = "txtOnlinePlayPlayerRating";
            txtOnlinePlayPlayerRating.Size = new Size(150, 31);
            txtOnlinePlayPlayerRating.TabIndex = 11;
            // 
            // lbOnlinePlayMsgs
            // 
            lbOnlinePlayMsgs.FormattingEnabled = true;
            lbOnlinePlayMsgs.ItemHeight = 25;
            lbOnlinePlayMsgs.Location = new Point(22, 137);
            lbOnlinePlayMsgs.Name = "lbOnlinePlayMsgs";
            lbOnlinePlayMsgs.Size = new Size(371, 129);
            lbOnlinePlayMsgs.TabIndex = 12;
            // 
            // FormOnlinePlayPage
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(847, 450);
            Controls.Add(lbOnlinePlayMsgs);
            Controls.Add(txtOnlinePlayPlayerRating);
            Controls.Add(txtOnlinePlayOppRating);
            Controls.Add(label2);
            Controls.Add(txtOnlinePlayMsg);
            Controls.Add(label1);
            Controls.Add(txtOnlinePlayPlayer);
            Controls.Add(txtOnlinePlayOpponent);
            Controls.Add(btnOnlinePlaySendChat);
            Controls.Add(btnOnlinePlayDraw);
            Controls.Add(btnOnlinePlayResign);
            Controls.Add(txtOnlinePlayBTimer);
            Controls.Add(txtOnlinePlayWTimer);
            Name = "FormOnlinePlayPage";
            Text = "Form2";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtOnlinePlayWTimer;
        private TextBox txtOnlinePlayBTimer;
        private Button btnOnlinePlayResign;
        private Button btnOnlinePlayDraw;
        private Button btnOnlinePlaySendChat;
        private TextBox txtOnlinePlayOpponent;
        private TextBox txtOnlinePlayPlayer;
        private Label label1;
        private TextBox txtOnlinePlayMsg;
        private Label label2;
        private TextBox txtOnlinePlayOppRating;
        private TextBox txtOnlinePlayPlayerRating;
        private ListBox lbOnlinePlayMsgs;
    }
}