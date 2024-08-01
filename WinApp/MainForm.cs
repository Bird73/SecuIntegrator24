using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EnvironmentManager;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.EventLogManager;
using Birdsoft.SecuIntegrator24.BusinessObject;
using Birdsoft.SecuIntegrator24.SystemInfrastructureObject.BackgroundTask;

namespace Birdsoft.SecuIntegrator24.WinUI
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Flag to check if the program is exiting
        /// </summary>
        private bool exitFlag = false;

        public MainForm()
        {
            InitializeComponent();

            CustomInitializeComponent();

            EventLogManager.WriteEventLog(new EventLog
            {
                Type = EventType.Information,
                Message = "Program started",
            });

            EnvironmentManager.LoadConfig();

            if (EventLogManager.ErrorCount > 0)
            {
                ShowErrorCountAndCheckEventLog();
            }

            ResetControlProperties();

            // Load the schedule from the json file
            BusinessObject.TaskScheduler taskScheduler = new BusinessObject.TaskScheduler();
            taskScheduler.LoadScheduleFomrConfig();
            taskScheduler.RegisterAndScheduleTasks();       // Register and schedule a task               

            TaskScheduleManager.StartAllTasks();             // Start all tasks
            TaskScheduleManager.StartDailyReset();
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            trayIcon = new NotifyIcon(components);
            initialYearLabel = new Label();
            initialYearCombo = new ComboBox();
            connectionIntervalLabel = new Label();
            connectionIntervalCombo = new ComboBox();
            secondLabel = new Label();
            AutoRunCheckbox = new CheckBox();
            saveAndReloadButton = new Button();
            exitButton = new Button();
            SuspendLayout();
            // 
            // trayIcon
            // 
            trayIcon.Icon = resources.GetObject("trayIcon.Icon") as Icon;
            trayIcon.Text = "notifyIcon1";
            trayIcon.Visible = true;
            // 
            // initialYearLabel
            // 
            initialYearLabel.AutoSize = true;
            initialYearLabel.Location = new Point(12, 15);
            initialYearLabel.Name = "initialYearLabel";
            initialYearLabel.Size = new Size(157, 30);
            initialYearLabel.TabIndex = 0;
            initialYearLabel.Text = "資料起始年份";
            // 
            // initialYearCombo
            // 
            initialYearCombo.FormattingEnabled = true;
            initialYearCombo.Location = new Point(175, 12);
            initialYearCombo.Name = "initialYearCombo";
            initialYearCombo.Size = new Size(242, 38);
            initialYearCombo.TabIndex = 1;
            // 
            // connectionIntervalLabel
            // 
            connectionIntervalLabel.AutoSize = true;
            connectionIntervalLabel.Location = new Point(12, 65);
            connectionIntervalLabel.Name = "connectionIntervalLabel";
            connectionIntervalLabel.Size = new Size(205, 30);
            connectionIntervalLabel.TabIndex = 2;
            connectionIntervalLabel.Text = "網頁連線間隔時間";
            // 
            // connectionIntervalCombo
            // 
            connectionIntervalCombo.FormattingEnabled = true;
            connectionIntervalCombo.Location = new Point(223, 62);
            connectionIntervalCombo.Name = "connectionIntervalCombo";
            connectionIntervalCombo.Size = new Size(194, 38);
            connectionIntervalCombo.TabIndex = 3;
            // 
            // secondLabel
            // 
            secondLabel.AutoSize = true;
            secondLabel.Location = new Point(433, 65);
            secondLabel.Name = "secondLabel";
            secondLabel.Size = new Size(37, 30);
            secondLabel.TabIndex = 4;
            secondLabel.Text = "秒";
            // 
            // AutoRunCheckbox
            // 
            AutoRunCheckbox.AutoSize = true;
            AutoRunCheckbox.Location = new Point(12, 120);
            AutoRunCheckbox.Name = "AutoRunCheckbox";
            AutoRunCheckbox.Size = new Size(237, 34);
            AutoRunCheckbox.TabIndex = 5;
            AutoRunCheckbox.Text = "啟動自動執行";
            AutoRunCheckbox.UseVisualStyleBackColor = true;
            // 
            // saveAndReloadButton
            // 
            saveAndReloadButton.Location = new Point(366, 346);
            saveAndReloadButton.Name = "saveAndReloadButton";
            saveAndReloadButton.Size = new Size(251, 46);
            saveAndReloadButton.TabIndex = 6;
            saveAndReloadButton.Text = "Save and Reload";
            saveAndReloadButton.UseVisualStyleBackColor = true;
            // 
            // exitButton
            // 
            exitButton.Location = new Point(366, 398);
            exitButton.Name = "exitButton";
            exitButton.Size = new Size(251, 46);
            exitButton.TabIndex = 7;
            exitButton.Text = "Exit Program";
            exitButton.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            ClientSize = new Size(909, 478);
            Controls.Add(exitButton);
            Controls.Add(saveAndReloadButton);
            Controls.Add(AutoRunCheckbox);
            Controls.Add(secondLabel);
            Controls.Add(connectionIntervalCombo);
            Controls.Add(connectionIntervalLabel);
            Controls.Add(initialYearCombo);
            Controls.Add(initialYearLabel);
            Icon = resources.GetObject("$this.Icon") as Icon;
            Name = "MainForm";
            Text = "SecuIntegrator 24";
            Load += new EventHandler(MainForm_Load);
            Resize += new EventHandler(MainForm_Resize);
            Paint += new PaintEventHandler(MainForm_Paint);
            ResumeLayout(false);
            PerformLayout();
        }

        private NotifyIcon trayIcon = default!;
        private System.ComponentModel.IContainer components = default!;
        private Label initialYearLabel = default!;
        private ComboBox initialYearCombo = default!;
        private Label connectionIntervalLabel = default!;
        private ComboBox connectionIntervalCombo = default!;
        private Label secondLabel = default!;
        private CheckBox AutoRunCheckbox = default!;
        private Button saveAndReloadButton = default!;
        private Button exitButton = default!;

        /// <summary>
        /// Custom initialization of components
        /// </summary>
        private void CustomInitializeComponent()
        {
            // Add event handler
            trayIcon.DoubleClick += trayIcon_DoubleClick;
            exitButton.Click += ExitButton_Click;

            saveAndReloadButton.Click += SaveAndReloadButton_Click;
        }

        /// <summary>
        ///   Reset control properties
        /// </summary>
        private void ResetControlProperties()
        {
            // define vertical margin
            int verticalMargin = 20;

            // Reset control properties
            this.initialYearCombo.Location = new Point(this.initialYearLabel.Location.X + this.initialYearLabel.Width + 10, this.initialYearLabel.Location.Y);
            initialYearCombo.Size = new Size(160, initialYearLabel.Height);
            this.connectionIntervalLabel.Location = new Point(this.initialYearLabel.Location.X, this.initialYearLabel.Location.Y + this.initialYearLabel.Height + verticalMargin);
            this.connectionIntervalCombo.Location = new Point(this.connectionIntervalLabel.Location.X + this.connectionIntervalLabel.Width + 10, this.connectionIntervalLabel.Location.Y);
            this.connectionIntervalCombo.Width = this.initialYearCombo.Right - this.connectionIntervalCombo.Left;
            this.secondLabel.Location = new Point(this.connectionIntervalCombo.Location.X + this.connectionIntervalCombo.Width + 10, this.connectionIntervalCombo.Location.Y);
            this.AutoRunCheckbox.Location = new Point(this.initialYearLabel.Location.X, this.connectionIntervalLabel.Location.Y + this.connectionIntervalLabel.Height + verticalMargin);
            
            // saveAndReloadButton 和 exitButton 位置中心對齊, 並且位於視窗底部
            this.saveAndReloadButton.Location = new Point((this.ClientSize.Width - this.saveAndReloadButton.Width - this.exitButton.Width) / 2, this.ClientSize.Height - this.saveAndReloadButton.Height - verticalMargin);
            this.exitButton.Location = new Point(this.saveAndReloadButton.Location.X + this.saveAndReloadButton.Width + 10, this.saveAndReloadButton.Location.Y);

            // combo box list is from 2019 to this this year
            // combo box can't be edited
            this.initialYearCombo.Items.Clear();
            for (int i = 2019; i <= System.DateTime.Now.Year; i++)
            {
                this.initialYearCombo.Items.Add(i);
            }
            this.initialYearCombo.SelectedItem = EnvironmentManager.EnvironmentConfig.InitialYear;

            // Initial connectionDelayTimeCombo with the value from EnvironmentManager.ConnectionDelayTime
            // combo box list is from 1 to 5
            this.connectionIntervalCombo.Items.Clear();
            for (int i = 1; i <= 5; i++)
            {
                this.connectionIntervalCombo.Items.Add(i);
            }
            this.connectionIntervalCombo.SelectedItem = EnvironmentManager.EnvironmentConfig.ConnectionInterval;

            // Initial AutoRunCheckbox with the value from EnvironmentManager.isAutoRunEnabled
            this.AutoRunCheckbox.Checked = EnvironmentManager.EnvironmentConfig.isAutoRunEnabled;
        }   

        /// <summary>
        ///  Reset control properties when the form is loaded, resized or painted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object? sender, EventArgs e)
        {
            // Code to be executed when the form is loaded
            ResetControlProperties();

            // Load the holidays from the database
            HolidayManager.Initialize();
        }

        /// <summary>
        ///   Reset control properties when the form is resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Resize(object? sender, EventArgs e)
        {
            // Code to be executed when the form is resized
            ResetControlProperties();
        }

        /// <summary>
        ///   Reset control properties when the form is painted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Paint(object? sender, PaintEventArgs e)
        {
            // Code to be executed when the form is painted
            //ResetControlProperties();
        }

        /// <summary>
        ///    Hide the form and display the system tray icon
        ///    when the form is closed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!this.exitFlag)
            {
                e.Cancel = true;
                this.Hide();
                this.trayIcon.Visible = true;
            }
            else
            {
                TaskScheduleManager.StopAllTasks();

                EventLogManager.WriteEventLog(new EventLog
                {
                    Type = EventType.Information,
                    Message = "Program exited",
                });
            }
            base.OnFormClosing(e);
        }

        /// <summary>
        ///   Display the form and hide the system tray icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trayIcon_DoubleClick(object? sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.trayIcon.Visible = false;
        }

        /// <summary>
        ///   Exit the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitButton_Click(object? sender, EventArgs e)
        {
            EventLogManager.WriteEventLog(new EventLog
            {
                Type = EventType.Information,
                Message = "Program exited",
            });

            // Exit program
            this.exitFlag = true;
            this.Close();
        }

        private void SaveAndReloadButton_Click(object? sender, EventArgs e)
        {
            List<string> errorMessages = new List<string>();

            // Save and reload
            EnvironmentManager.EnvironmentConfig.InitialYear = this.initialYearCombo.SelectedItem as int? ?? DateTime.Now.Year;
            EnvironmentManager.EnvironmentConfig.ConnectionInterval = this.connectionIntervalCombo.SelectedItem as int? ?? 1;
            EnvironmentManager.EnvironmentConfig.isAutoRunEnabled = this.AutoRunCheckbox.Checked ;

            EnvironmentManager.SaveConfig();
            if (errorMessages.Count > 0)
            {
                ShowErrorCountAndCheckEventLog();
                MessageBox.Show(string.Join("\n", errorMessages), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Save and Reload successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        ///     Show how many error messages in the event log
        /// </summary>
        private void ShowErrorCountAndCheckEventLog()
        {
            // Show how many error messages in the event log and reset error count
            MessageBox.Show($"There are {EventLogManager.ErrorCount} error messages, please check the event log", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            EventLogManager.ResetCount();
        }
    }
}
