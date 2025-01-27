namespace DockerWizard.UI
{
    partial class Main
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
            ServersLabel = new Label();
            ServersComboBox = new ComboBox();
            ProjectsLabel = new Label();
            ProjectsComboBox = new ComboBox();
            DeployButton = new Button();
            LogsListBox = new ListBox();
            CloseBtn = new Button();
            SuspendLayout();
            // 
            // ServersLabel
            // 
            ServersLabel.AutoSize = true;
            ServersLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ServersLabel.Location = new Point(12, 6);
            ServersLabel.Name = "ServersLabel";
            ServersLabel.Size = new Size(82, 28);
            ServersLabel.TabIndex = 0;
            ServersLabel.Text = "Servers";
            // 
            // ServersComboBox
            // 
            ServersComboBox.FormattingEnabled = true;
            ServersComboBox.Location = new Point(12, 37);
            ServersComboBox.Name = "ServersComboBox";
            ServersComboBox.Size = new Size(304, 33);
            ServersComboBox.TabIndex = 1;
            ServersComboBox.SelectedIndexChanged += ServersComboBox_SelectedIndexChanged;
            // 
            // ProjectsLabel
            // 
            ProjectsLabel.AutoSize = true;
            ProjectsLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ProjectsLabel.Location = new Point(12, 80);
            ProjectsLabel.Name = "ProjectsLabel";
            ProjectsLabel.Size = new Size(88, 28);
            ProjectsLabel.TabIndex = 2;
            ProjectsLabel.Text = "Projects";
            // 
            // ProjectsComboBox
            // 
            ProjectsComboBox.FormattingEnabled = true;
            ProjectsComboBox.Location = new Point(12, 111);
            ProjectsComboBox.Name = "ProjectsComboBox";
            ProjectsComboBox.Size = new Size(304, 33);
            ProjectsComboBox.TabIndex = 3;
            ProjectsComboBox.SelectedIndexChanged += ProjectsComboBox_SelectedIndexChanged;
            // 
            // DeployButton
            // 
            DeployButton.BackColor = Color.SandyBrown;
            DeployButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            DeployButton.Location = new Point(91, 151);
            DeployButton.Name = "DeployButton";
            DeployButton.Size = new Size(147, 57);
            DeployButton.TabIndex = 4;
            DeployButton.Text = "D E P L O Y";
            DeployButton.UseVisualStyleBackColor = false;
            DeployButton.Click += DeployButton_Click;
            // 
            // LogsListBox
            // 
            LogsListBox.FormattingEnabled = true;
            LogsListBox.ItemHeight = 25;
            LogsListBox.Location = new Point(338, 37);
            LogsListBox.Name = "LogsListBox";
            LogsListBox.Size = new Size(1348, 829);
            LogsListBox.TabIndex = 5;
            // 
            // CloseBtn
            // 
            CloseBtn.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            CloseBtn.Location = new Point(1574, 872);
            CloseBtn.Name = "CloseBtn";
            CloseBtn.Size = new Size(112, 42);
            CloseBtn.TabIndex = 6;
            CloseBtn.Text = "Close";
            CloseBtn.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1698, 920);
            Controls.Add(CloseBtn);
            Controls.Add(LogsListBox);
            Controls.Add(DeployButton);
            Controls.Add(ProjectsComboBox);
            Controls.Add(ProjectsLabel);
            Controls.Add(ServersComboBox);
            Controls.Add(ServersLabel);
            MaximizeBox = false;
            Name = "Main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Docker Wizard";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label ServersLabel;
        private ComboBox ServersComboBox;
        private Label ProjectsLabel;
        private ComboBox ProjectsComboBox;
        private Button DeployButton;
        private ListBox LogsListBox;
        private Button CloseBtn;
    }
}
