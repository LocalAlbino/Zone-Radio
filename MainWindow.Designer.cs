namespace Zone_Radio
{
    partial class MainWindow
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
            cboxTogglePlayback1 = new ComboBox();
            lblTogglePLayback = new Label();
            cboxTogglePlayback2 = new ComboBox();
            lblSkipPlayback = new Label();
            cboxSkipPlayback1 = new ComboBox();
            cboxSkipPlayback2 = new ComboBox();
            lblTogglePda = new Label();
            cboxTogglePda1 = new ComboBox();
            cboxTogglePda2 = new ComboBox();
            lblConnectionStatus = new Label();
            btnConnect = new Button();
            SuspendLayout();
            // 
            // cboxTogglePlayback1
            // 
            cboxTogglePlayback1.FormattingEnabled = true;
            cboxTogglePlayback1.Location = new Point(12, 27);
            cboxTogglePlayback1.Name = "cboxTogglePlayback1";
            cboxTogglePlayback1.Size = new Size(171, 23);
            cboxTogglePlayback1.TabIndex = 0;
            // 
            // lblTogglePLayback
            // 
            lblTogglePLayback.AutoSize = true;
            lblTogglePLayback.Location = new Point(12, 9);
            lblTogglePLayback.Name = "lblTogglePLayback";
            lblTogglePLayback.Size = new Size(120, 15);
            lblTogglePLayback.TabIndex = 1;
            lblTogglePLayback.Text = "Toggle Playback Keys";
            // 
            // cboxTogglePlayback2
            // 
            cboxTogglePlayback2.FormattingEnabled = true;
            cboxTogglePlayback2.Location = new Point(12, 56);
            cboxTogglePlayback2.Name = "cboxTogglePlayback2";
            cboxTogglePlayback2.Size = new Size(171, 23);
            cboxTogglePlayback2.TabIndex = 2;
            // 
            // lblSkipPlayback
            // 
            lblSkipPlayback.AutoSize = true;
            lblSkipPlayback.Location = new Point(189, 9);
            lblSkipPlayback.Name = "lblSkipPlayback";
            lblSkipPlayback.Size = new Size(86, 15);
            lblSkipPlayback.TabIndex = 3;
            lblSkipPlayback.Text = "Skip Song Keys";
            // 
            // cboxSkipPlayback1
            // 
            cboxSkipPlayback1.FormattingEnabled = true;
            cboxSkipPlayback1.Location = new Point(189, 27);
            cboxSkipPlayback1.Name = "cboxSkipPlayback1";
            cboxSkipPlayback1.Size = new Size(171, 23);
            cboxSkipPlayback1.TabIndex = 4;
            // 
            // cboxSkipPlayback2
            // 
            cboxSkipPlayback2.FormattingEnabled = true;
            cboxSkipPlayback2.Location = new Point(189, 56);
            cboxSkipPlayback2.Name = "cboxSkipPlayback2";
            cboxSkipPlayback2.Size = new Size(171, 23);
            cboxSkipPlayback2.TabIndex = 5;
            // 
            // lblTogglePda
            // 
            lblTogglePda.AutoSize = true;
            lblTogglePda.Location = new Point(366, 9);
            lblTogglePda.Name = "lblTogglePda";
            lblTogglePda.Size = new Size(96, 15);
            lblTogglePda.TabIndex = 6;
            lblTogglePda.Text = "Toggle PDA Keys";
            // 
            // cboxTogglePda1
            // 
            cboxTogglePda1.FormattingEnabled = true;
            cboxTogglePda1.Location = new Point(366, 27);
            cboxTogglePda1.Name = "cboxTogglePda1";
            cboxTogglePda1.Size = new Size(171, 23);
            cboxTogglePda1.TabIndex = 7;
            // 
            // cboxTogglePda2
            // 
            cboxTogglePda2.FormattingEnabled = true;
            cboxTogglePda2.Location = new Point(366, 56);
            cboxTogglePda2.Name = "cboxTogglePda2";
            cboxTogglePda2.Size = new Size(171, 23);
            cboxTogglePda2.TabIndex = 8;
            // 
            // lblConnectionStatus
            // 
            lblConnectionStatus.AutoSize = true;
            lblConnectionStatus.Location = new Point(12, 100);
            lblConnectionStatus.Name = "lblConnectionStatus";
            lblConnectionStatus.Size = new Size(0, 15);
            lblConnectionStatus.TabIndex = 9;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(462, 96);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(75, 23);
            btnConnect.TabIndex = 10;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(554, 124);
            Controls.Add(btnConnect);
            Controls.Add(lblConnectionStatus);
            Controls.Add(cboxTogglePda2);
            Controls.Add(cboxTogglePda1);
            Controls.Add(lblTogglePda);
            Controls.Add(cboxSkipPlayback2);
            Controls.Add(cboxSkipPlayback1);
            Controls.Add(lblSkipPlayback);
            Controls.Add(cboxTogglePlayback2);
            Controls.Add(lblTogglePLayback);
            Controls.Add(cboxTogglePlayback1);
            Name = "MainWindow";
            Text = "Zone Radio";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cboxTogglePlayback1;
        private Label lblTogglePLayback;
        private ComboBox cboxTogglePlayback2;
        private Label lblSkipPlayback;
        private ComboBox cboxSkipPlayback1;
        private ComboBox cboxSkipPlayback2;
        private Label lblTogglePda;
        private ComboBox cboxTogglePda1;
        private ComboBox cboxTogglePda2;
        private Label lblConnectionStatus;
        private Button btnConnect;
    }
}