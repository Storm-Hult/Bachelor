namespace BO25EB_47
{
    partial class FormBotPlayPage
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
            btnBotPlayEval = new Button();
            btnBotPlayBestMove = new Button();
            btnBotPlayExit = new Button();
            txtBotPlayTurn = new TextBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // btnBotPlayEval
            // 
            btnBotPlayEval.Location = new Point(101, 94);
            btnBotPlayEval.Name = "btnBotPlayEval";
            btnBotPlayEval.Size = new Size(152, 34);
            btnBotPlayEval.TabIndex = 1;
            btnBotPlayEval.Text = "Show evaluation";
            btnBotPlayEval.UseVisualStyleBackColor = true;
            btnBotPlayEval.Click += btnBotPlayEval_Click;
            // 
            // btnBotPlayBestMove
            // 
            btnBotPlayBestMove.Location = new Point(101, 148);
            btnBotPlayBestMove.Name = "btnBotPlayBestMove";
            btnBotPlayBestMove.Size = new Size(152, 34);
            btnBotPlayBestMove.TabIndex = 2;
            btnBotPlayBestMove.Text = "Best move";
            btnBotPlayBestMove.UseVisualStyleBackColor = true;
            btnBotPlayBestMove.Click += btnBotPlayBestMove_Click;
            // 
            // btnBotPlayExit
            // 
            btnBotPlayExit.Location = new Point(101, 202);
            btnBotPlayExit.Name = "btnBotPlayExit";
            btnBotPlayExit.Size = new Size(152, 34);
            btnBotPlayExit.TabIndex = 3;
            btnBotPlayExit.Text = "Exit to menu";
            btnBotPlayExit.UseVisualStyleBackColor = true;
            btnBotPlayExit.Click += btnBotPlayExit_Click;
            // 
            // txtBotPlayTurn
            // 
            txtBotPlayTurn.Location = new Point(101, 270);
            txtBotPlayTurn.Name = "txtBotPlayTurn";
            txtBotPlayTurn.Size = new Size(152, 31);
            txtBotPlayTurn.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(118, 55);
            label1.Name = "label1";
            label1.Size = new Size(117, 25);
            label1.TabIndex = 5;
            label1.Text = "Bot playpage";
            // 
            // FormBotPlayPage
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
            Controls.Add(txtBotPlayTurn);
            Controls.Add(btnBotPlayExit);
            Controls.Add(btnBotPlayBestMove);
            Controls.Add(btnBotPlayEval);
            Name = "FormBotPlayPage";
            Text = "FormBotPlayPage";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnBotPlayEval;
        private Button btnBotPlayBestMove;
        private Button btnBotPlayExit;
        private TextBox txtBotPlayTurn;
        private Label label1;
    }
}