namespace BO25EB_47
{
    partial class FormArchivesPage
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
            btnArchivesMenu = new Button();
            btnArchivesSetPos = new Button();
            cbArchivesGames = new ComboBox();
            txtArchivesOpponent = new TextBox();
            txtArchivesPlaysAs = new TextBox();
            txtArchivesMoveCnt = new TextBox();
            txtArchivesDate = new TextBox();
            txtArchivesWinner = new TextBox();
            cbArchivesMoveNr = new ComboBox();
            SuspendLayout();
            // 
            // btnArchivesMenu
            // 
            btnArchivesMenu.Location = new Point(315, 38);
            btnArchivesMenu.Name = "btnArchivesMenu";
            btnArchivesMenu.Size = new Size(118, 34);
            btnArchivesMenu.TabIndex = 0;
            btnArchivesMenu.Text = "Main menu";
            btnArchivesMenu.UseVisualStyleBackColor = true;
            btnArchivesMenu.Click += btnArchivesMenu_Click;
            // 
            // btnArchivesSetPos
            // 
            btnArchivesSetPos.Location = new Point(315, 110);
            btnArchivesSetPos.Name = "btnArchivesSetPos";
            btnArchivesSetPos.Size = new Size(118, 34);
            btnArchivesSetPos.TabIndex = 1;
            btnArchivesSetPos.Text = "Set position";
            btnArchivesSetPos.UseVisualStyleBackColor = true;
            // 
            // cbArchivesGames
            // 
            cbArchivesGames.FormattingEnabled = true;
            cbArchivesGames.Location = new Point(66, 39);
            cbArchivesGames.Name = "cbArchivesGames";
            cbArchivesGames.Size = new Size(182, 33);
            cbArchivesGames.TabIndex = 2;
            cbArchivesGames.SelectedIndexChanged += cbArchivesGames_SelectedIndexChanged;
            // 
            // txtArchivesOpponent
            // 
            txtArchivesOpponent.Location = new Point(83, 166);
            txtArchivesOpponent.Name = "txtArchivesOpponent";
            txtArchivesOpponent.Size = new Size(150, 31);
            txtArchivesOpponent.TabIndex = 3;
            // 
            // txtArchivesPlaysAs
            // 
            txtArchivesPlaysAs.Location = new Point(83, 229);
            txtArchivesPlaysAs.Name = "txtArchivesPlaysAs";
            txtArchivesPlaysAs.Size = new Size(150, 31);
            txtArchivesPlaysAs.TabIndex = 4;
            // 
            // txtArchivesMoveCnt
            // 
            txtArchivesMoveCnt.Location = new Point(83, 292);
            txtArchivesMoveCnt.Name = "txtArchivesMoveCnt";
            txtArchivesMoveCnt.Size = new Size(150, 31);
            txtArchivesMoveCnt.TabIndex = 5;
            // 
            // txtArchivesDate
            // 
            txtArchivesDate.Location = new Point(83, 424);
            txtArchivesDate.Name = "txtArchivesDate";
            txtArchivesDate.Size = new Size(150, 31);
            txtArchivesDate.TabIndex = 6;
            // 
            // txtArchivesWinner
            // 
            txtArchivesWinner.Location = new Point(83, 358);
            txtArchivesWinner.Name = "txtArchivesWinner";
            txtArchivesWinner.Size = new Size(150, 31);
            txtArchivesWinner.TabIndex = 7;
            // 
            // cbArchivesMoveNr
            // 
            cbArchivesMoveNr.FormattingEnabled = true;
            cbArchivesMoveNr.Location = new Point(66, 110);
            cbArchivesMoveNr.Name = "cbArchivesMoveNr";
            cbArchivesMoveNr.Size = new Size(182, 33);
            cbArchivesMoveNr.TabIndex = 8;
            cbArchivesMoveNr.SelectedIndexChanged += cbArchivesMoveNr_SelectedIndexChanged;
            // 
            // FormArchivesPage
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 505);
            Controls.Add(cbArchivesMoveNr);
            Controls.Add(txtArchivesWinner);
            Controls.Add(txtArchivesDate);
            Controls.Add(txtArchivesMoveCnt);
            Controls.Add(txtArchivesPlaysAs);
            Controls.Add(txtArchivesOpponent);
            Controls.Add(cbArchivesGames);
            Controls.Add(btnArchivesSetPos);
            Controls.Add(btnArchivesMenu);
            Name = "FormArchivesPage";
            Text = "FormArchivesPage";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnArchivesMenu;
        private Button btnArchivesSetPos;
        private ComboBox cbArchivesGames;
        private TextBox txtArchivesOpponent;
        private TextBox txtArchivesPlaysAs;
        private TextBox txtArchivesMoveCnt;
        private TextBox txtArchivesDate;
        private TextBox txtArchivesWinner;
        private ComboBox cbArchivesMoveNr;
    }
}