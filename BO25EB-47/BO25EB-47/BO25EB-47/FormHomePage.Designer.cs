namespace BO25EB_47
{
    partial class FormHomePage
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
            btnHomeBot = new Button();
            btnHomeOnline = new Button();
            btnHomeArchives = new Button();
            btnHomeExit = new Button();
            SuspendLayout();
            // 
            // btnHomeBot
            // 
            btnHomeBot.Location = new Point(309, 112);
            btnHomeBot.Name = "btnHomeBot";
            btnHomeBot.Size = new Size(159, 41);
            btnHomeBot.TabIndex = 0;
            btnHomeBot.Text = "Play against bot";
            btnHomeBot.UseVisualStyleBackColor = true;
            btnHomeBot.Click += btnHomeBot_Click;
            // 
            // btnHomeOnline
            // 
            btnHomeOnline.Location = new Point(309, 168);
            btnHomeOnline.Name = "btnHomeOnline";
            btnHomeOnline.Size = new Size(162, 43);
            btnHomeOnline.TabIndex = 1;
            btnHomeOnline.Text = "Play online";
            btnHomeOnline.UseVisualStyleBackColor = true;
            btnHomeOnline.Click += btnHomeOnline_Click;
            // 
            // btnHomeArchives
            // 
            btnHomeArchives.Location = new Point(309, 226);
            btnHomeArchives.Name = "btnHomeArchives";
            btnHomeArchives.Size = new Size(162, 42);
            btnHomeArchives.TabIndex = 2;
            btnHomeArchives.Text = "Archives";
            btnHomeArchives.UseVisualStyleBackColor = true;
            btnHomeArchives.Click += btnHomeArchives_Click;
            // 
            // btnHomeExit
            // 
            btnHomeExit.Location = new Point(309, 292);
            btnHomeExit.Name = "btnHomeExit";
            btnHomeExit.Size = new Size(159, 34);
            btnHomeExit.TabIndex = 3;
            btnHomeExit.Text = "Exit app";
            btnHomeExit.UseVisualStyleBackColor = true;
            btnHomeExit.Click += btnHomeExit_Click;
            // 
            // FormHomePage
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnHomeExit);
            Controls.Add(btnHomeArchives);
            Controls.Add(btnHomeOnline);
            Controls.Add(btnHomeBot);
            Name = "FormHomePage";
            Text = "FormHomePage";
            ResumeLayout(false);
        }

        #endregion

        private Button btnHomeBot;
        private Button btnHomeOnline;
        private Button btnHomeArchives;
        private Button btnHomeExit;
    }
}
