using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using System.Text;
using System.Diagnostics;

namespace ClosedBullet
{
    public partial class ModernFormV2 : Form
    {
        // UI Components
        private TabControl mainTabControl;
        private Panel headerPanel;
        private Panel statusPanel;
        
        // Tab Pages
        private TabPage dashboardTab;
        private TabPage resultsTab;
        private TabPage configTab;
        private TabPage settingsTab;
        private TabPage monitoringTab;
        
        // Dashboard Components
        private RoundedButton loadWordlistBtn;
        private RoundedButton loadConfigBtn;
        private RoundedButton startBtn;
        private RoundedButton pauseBtn;
        private RoundedButton stopBtn;
        private ModernProgressBar progressBar;
        private StatisticsCard validCard;
        private StatisticsCard invalidCard;
        private StatisticsCard blockedCard;
        private StatisticsCard errorCard;
        private StatisticsCard rateCard;
        private ListBox liveResultsFeed;
        private LiveChart validationChart;
        private Panel chartPanel;
        
        // Results Components
        private DataGridView resultsGrid;
        private RoundedButton exportValidBtn;
        private RoundedButton exportInvalidBtn;
        private RoundedButton exportAllBtn;
        private RoundedButton clearResultsBtn;
        private RoundedButton retryErrorsBtn;
        private TextBox searchBox;
        private ComboBox filterCombo;
        
        // Configuration Components
        private ListBox configListBox;
        private RichTextBox configEditor;
        private RoundedButton loadConfigFileBtn;
        private RoundedButton saveConfigBtn;
        private RoundedButton testConfigBtn;
        private RoundedButton newConfigBtn;
        private TextBox testPhoneBox;
        private ListBox patternsListBox;
        private TextBox patternTextBox;
        private ComboBox patternTypeCombo;
        private RoundedButton addPatternBtn;
        private RoundedButton removePatternBtn;
        
        // Settings Components
        private TrackBar threadsTrackBar;
        private Label threadsLabel;
        private TrackBar delayTrackBar;
        private Label delayLabel;
        private CheckBox proxyEnabledCheckBox;
        private TextBox proxyFileTextBox;
        private RoundedButton loadProxyBtn;
        private RoundedButton testProxiesBtn;
        private CheckBox soundEnabledCheckBox;
        private CheckBox saveHtmlCheckBox;
        private ComboBox themeComboBox;
        private CheckBox autoSaveCheckBox;
        private NumericUpDown autoSaveIntervalBox;
        
        // Monitoring Components
        private Label cpuLabel;
        private Label memoryLabel;
        private Label networkLabel;
        private ProgressBar cpuProgressBar;
        private ProgressBar memoryProgressBar;
        private Label validationRateLabel;
        private Label successRateLabel;
        private Label avgResponseTimeLabel;
        private Label queueSizeLabel;
        private ListBox errorLogListBox;
        private RoundedButton clearErrorsBtn;
        private LiveChart performanceChart;
        private System.Windows.Forms.Timer monitoringTimer;
        
        // Engines and Managers
        private EnhancedValidationEngine validationEngine;
        private ConfigManager configManager;
        private ProxyManager proxyManager;
        
        // Data
        private List<ValidationRecord> validationResults = new List<ValidationRecord>();
        private List<string> phoneNumbers = new List<string>();
        private List<string> errorPhoneNumbers = new List<string>(); // Track failed numbers for retry
        private CancellationTokenSource cancellationTokenSource;
        private bool isRunning = false;
        private string databasePath = Path.Combine(Application.StartupPath, "validation_results.json");
        
        // Statistics
        private int validCount = 0;
        private int invalidCount = 0;
        private int blockedCount = 0;
        private int errorCount = 0;
        private int totalProcessed = 0;
        private DateTime startTime;
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;
        
        // Theme Colors (will be updated based on selected theme)
        private Color backgroundColor = Color.FromArgb(20, 20, 25);
        private Color surfaceColor = Color.FromArgb(30, 30, 35);
        private Color primaryColor = Color.FromArgb(0, 122, 204);
        private Color accentColor = Color.FromArgb(0, 180, 216);
        private Color textColor = Color.White;
        private Color secondaryTextColor = Color.FromArgb(180, 180, 180);
        private Color successColor = Color.FromArgb(46, 204, 113);
        private Color errorColor = Color.FromArgb(231, 76, 60);
        private Color warningColor = Color.FromArgb(241, 196, 15);
        
        // Settings
        private int maxThreads = 10;
        private int requestDelay = 1000;
        private bool soundEnabled = true;
        private bool saveHtmlResponses = true;
        
        public ModernFormV2()
        {
            InitializeComponent();
            ShowLoadingOverlay();
        }
        
        private void ShowLoadingOverlay()
        {
            // Create and show loading overlay immediately
            var loadingOverlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(25, 25, 30)
            };
            
            var titleLabel = new Label
            {
                Text = "ClosedBullet - Modern UI",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true
            };
            
            var statusLabel = new Label
            {
                Text = "Loading components...",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(100, 149, 237),
                AutoSize = true
            };
            
            loadingOverlay.Controls.AddRange(new Control[] { titleLabel, statusLabel });
            this.Controls.Add(loadingOverlay);
            loadingOverlay.BringToFront();
            
            // Center labels when form is shown
            this.Shown += (s, e) =>
            {
                titleLabel.Location = new Point((this.Width - titleLabel.Width) / 2, (this.Height - titleLabel.Height) / 2 - 30);
                statusLabel.Location = new Point((this.Width - statusLabel.Width) / 2, (this.Height - statusLabel.Height) / 2 + 30);
                
                // Initialize in background after form is visible
                var timer = new System.Windows.Forms.Timer { Interval = 100 };
                timer.Tick += (sender, args) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    
                    statusLabel.Text = "Initializing validation engines...";
                    Application.DoEvents();
                    InitializeEngines();
                    
                    statusLabel.Text = "Setting up modern interface...";
                    Application.DoEvents();
                    SetupModernUI();
                    
                    statusLabel.Text = "Initializing monitoring...";
                    Application.DoEvents();
                    InitializeMonitoring();
                    
                    statusLabel.Text = "Loading settings...";
                    Application.DoEvents();
                    LoadSettings();
                    
                    statusLabel.Text = "Loading previous results...";
                    Application.DoEvents();
                    LoadResultsFromDatabase();
                    
                    statusLabel.Text = "Ready!";
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(300);
                    
                    // Remove loading overlay
                    this.Controls.Remove(loadingOverlay);
                    loadingOverlay.Dispose();
                };
                timer.Start();
            };
        }
        
        
        private void InitializeComponent()
        {
            this.Text = "ClosedBullet - Modern Edition v2";
            this.Size = new Size(1400, 900);
            this.MinimumSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = backgroundColor;
            
            // Enable proper resizing with sizable border
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.DoubleBuffered = true;
            
            // Remove the custom drag code since we now have a proper border
            this.MaximizeBox = true;
            this.MinimizeBox = true;
        }
        
        private void InitializeEngines()
        {
            validationEngine = new EnhancedValidationEngine();
            configManager = new ConfigManager();
            proxyManager = new ProxyManager();
            
            // Subscribe to events
            validationEngine.StatusChanged += (s, msg) => UpdateStatus(msg);
            validationEngine.ValidationCompleted += (s, result) => {
                this.BeginInvoke(new Action(() => AddValidationResult(result)));
            };
        }
        
        private void SetupModernUI()
        {
            // Create header panel
            CreateHeaderPanel();
            
            // Create main tab control
            CreateTabControl();
            
            // Create status bar
            CreateStatusBar();
            
            // Setup each tab
            SetupDashboardTab();
            SetupResultsTab();
            SetupConfigurationTab();
            SetupSettingsTab();
            SetupMonitoringTab();
        }
        
        private void CreateHeaderPanel()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(15, 15, 20)
            };
            
            // Title label with icon
            var titleLabel = new Label
            {
                Text = "⚡ ClosedBullet - Modern Edition",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(15, 12),
                AutoSize = true
            };
            
            // Version label
            var versionLabel = new Label
            {
                Text = "v2.0 - Enhanced",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9),
                Location = new Point(15, 32),
                AutoSize = true
            };
            
            headerPanel.Controls.AddRange(new Control[] { titleLabel, versionLabel });
            this.Controls.Add(headerPanel);
        }
        
        private void CreateTabControl()
        {
            mainTabControl = new TabControl
            {
                Location = new Point(0, 50),
                Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 10),
                ItemSize = new Size(180, 40),
                SizeMode = TabSizeMode.Fixed,
                DrawMode = TabDrawMode.OwnerDrawFixed
            };
            
            mainTabControl.DrawItem += TabControl_DrawItem;
            
            // Create tab pages
            dashboardTab = new TabPage("🎯 Dashboard") { BackColor = backgroundColor };
            resultsTab = new TabPage("📊 Results") { BackColor = backgroundColor };
            configTab = new TabPage("⚙️ Configuration") { BackColor = backgroundColor };
            settingsTab = new TabPage("🔧 Settings") { BackColor = backgroundColor };
            monitoringTab = new TabPage("📈 Monitoring") { BackColor = backgroundColor };
            
            mainTabControl.TabPages.AddRange(new TabPage[] { 
                dashboardTab, resultsTab, configTab, settingsTab, monitoringTab 
            });
            
            this.Controls.Add(mainTabControl);
        }
        
        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            TabPage page = tabControl.TabPages[e.Index];
            Rectangle tabBounds = tabControl.GetTabRect(e.Index);
            
            // Fill background
            bool isSelected = e.State == DrawItemState.Selected;
            using (SolidBrush brush = new SolidBrush(
                isSelected ? primaryColor : surfaceColor))
            {
                e.Graphics.FillRectangle(brush, tabBounds);
            }
            
            // Draw text with appropriate color based on theme and selection
            Color tabTextColor;
            if (isSelected)
            {
                // For selected tab, use contrasting color
                tabTextColor = IsLightColor(primaryColor) ? Color.Black : Color.White;
            }
            else
            {
                // For unselected tabs, use the theme's text color
                tabTextColor = textColor;
            }
            
            TextRenderer.DrawText(e.Graphics, page.Text, tabControl.Font,
                tabBounds, tabTextColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
        
        private void CreateStatusBar()
        {
            statusPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                BackColor = Color.FromArgb(15, 15, 20)
            };
            
            var statusLabel = new Label
            {
                Name = "statusLabel",
                Text = "⚡ Ready",
                ForeColor = Color.Lime,
                Font = new Font("Segoe UI", 9),
                Location = new Point(10, 5),
                AutoSize = true
            };
            
            // Add statistics to status bar
            var statsLabel = new Label
            {
                Name = "statsLabel",
                Text = "Valid: 0 | Invalid: 0 | Blocked: 0 | Rate: 0/min",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                Location = new Point(200, 5),
                AutoSize = true
            };
            
            statusPanel.Controls.AddRange(new Control[] { statusLabel, statsLabel });
            this.Controls.Add(statusPanel);
        }
        
        private void SetupDashboardTab()
        {
            // Main container
            var container = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(10)
            };
            
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F));
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            
            // Top controls panel
            var topPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = surfaceColor,
                Padding = new Padding(15)
            };
            
            // Buttons
            loadWordlistBtn = new RoundedButton
            {
                Text = "📁 Load Wordlist",
                Size = new Size(140, 45),
                Location = new Point(15, 15),
                BackColor = primaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            loadWordlistBtn.Click += LoadWordlist_Click;
            
            loadConfigBtn = new RoundedButton
            {
                Text = "⚙️ Load Config",
                Size = new Size(140, 45),
                Location = new Point(165, 15),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            loadConfigBtn.Click += LoadConfig_Click;
            
            startBtn = new RoundedButton
            {
                Text = "▶️ START",
                Size = new Size(100, 45),
                Location = new Point(320, 15),
                BackColor = successColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            startBtn.Click += Start_Click;
            
            pauseBtn = new RoundedButton
            {
                Text = "⏸️ PAUSE",
                Size = new Size(100, 45),
                Location = new Point(430, 15),
                BackColor = warningColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Enabled = false
            };
            pauseBtn.Click += Pause_Click;
            
            stopBtn = new RoundedButton
            {
                Text = "⏹️ STOP",
                Size = new Size(100, 45),
                Location = new Point(540, 15),
                BackColor = errorColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Enabled = false
            };
            stopBtn.Click += Stop_Click;
            
            topPanel.Controls.AddRange(new Control[] { 
                loadWordlistBtn, loadConfigBtn, startBtn, pauseBtn, stopBtn 
            });
            
            // Statistics cards panel
            var statsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(5)
            };
            
            validCard = new StatisticsCard
            {
                Title = "VALID",
                Value = "0",
                Icon = "✅",
                BackColor = Color.FromArgb(40, 40, 45),
                AccentColor = successColor,
                Size = new Size(180, 100),
                Margin = new Padding(5)
            };
            
            invalidCard = new StatisticsCard
            {
                Title = "INVALID",
                Value = "0",
                Icon = "❌",
                BackColor = Color.FromArgb(40, 40, 45),
                AccentColor = errorColor,
                Size = new Size(180, 100),
                Margin = new Padding(5)
            };
            
            blockedCard = new StatisticsCard
            {
                Title = "BLOCKED",
                Value = "0",
                Icon = "⚠️",
                BackColor = Color.FromArgb(40, 40, 45),
                AccentColor = warningColor,
                Size = new Size(150, 100),
                Margin = new Padding(5)
            };
            
            errorCard = new StatisticsCard
            {
                Title = "ERRORS",
                Value = "0",
                Icon = "🟡",
                BackColor = Color.FromArgb(40, 40, 45),
                AccentColor = Color.FromArgb(255, 193, 7),
                Size = new Size(150, 100),
                Margin = new Padding(5)
            };
            
            rateCard = new StatisticsCard
            {
                Title = "RATE/MIN",
                Value = "0",
                Icon = "⚡",
                BackColor = Color.FromArgb(40, 40, 45),
                AccentColor = primaryColor,
                Size = new Size(150, 100),
                Margin = new Padding(5)
            };
            
            statsPanel.Controls.AddRange(new Control[] { 
                validCard, invalidCard, blockedCard, errorCard, rateCard 
            });
            
            // Progress bar
            progressBar = new ModernProgressBar
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Color.FromArgb(40, 40, 45),
                ForeColor = primaryColor,
                Value = 0,
                Maximum = 100,
                Margin = new Padding(10)
            };
            
            // Live feed panel (left side, full width of its column)
            var feedPanel = new GlassPanel
            {
                Dock = DockStyle.Fill,
                Title = "📋 Live Results Feed",
                Margin = new Padding(5)
            };
            
            liveResultsFeed = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(25, 25, 30),
                ForeColor = Color.White,
                Font = new Font("Consolas", 9),
                BorderStyle = BorderStyle.None,
                SelectionMode = SelectionMode.One
            };
            
            feedPanel.Controls.Add(liveResultsFeed);
            
            // Chart panel (right side) - Success/Failure Pie Chart
            chartPanel = new GlassPanel
            {
                Dock = DockStyle.Fill,
                Title = "📊 Success/Failure Distribution",
                Margin = new Padding(5)
            };
            
            // Create custom pie chart panel
            var pieChartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(25, 25, 30)
            };
            pieChartPanel.Paint += PieChartPanel_Paint;
            
            // Add legend below pie chart
            var legendPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10)
            };
            
            var validLegend = new Label
            {
                Text = "● Valid",
                ForeColor = successColor,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(5)
            };
            
            var invalidLegend = new Label
            {
                Text = "● Invalid",
                ForeColor = errorColor,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(5)
            };
            
            var blockedLegend = new Label
            {
                Text = "● Blocked",
                ForeColor = warningColor,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(5)
            };
            
            var errorLegend = new Label
            {
                Text = "● Errors (Retry)",
                ForeColor = Color.FromArgb(255, 193, 7),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(5)
            };
            
            legendPanel.Controls.AddRange(new Control[] { validLegend, invalidLegend, blockedLegend, errorLegend });
            
            chartPanel.Controls.Add(pieChartPanel);
            chartPanel.Controls.Add(legendPanel);
            
            // Add progress bar below stats
            var progressPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 5, 10, 5),
                Height = 50
            };
            
            var progressLabel = new Label
            {
                Text = "Progress:",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Top,
                Height = 20
            };
            
            progressPanel.Controls.Add(progressBar);
            progressPanel.Controls.Add(progressLabel);
            
            // Adjust container layout for progress bar
            container.RowCount = 4;
            container.RowStyles.Clear();
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));  // Top controls
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F)); // Stats cards
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));  // Progress bar
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Feed and chart
            
            // Add to layout
            container.Controls.Add(topPanel, 0, 0);
            container.SetColumnSpan(topPanel, 2);
            
            container.Controls.Add(statsPanel, 0, 1);
            container.SetColumnSpan(statsPanel, 2);
            
            container.Controls.Add(progressPanel, 0, 2);
            container.SetColumnSpan(progressPanel, 2);
            
            container.Controls.Add(feedPanel, 0, 3);
            container.Controls.Add(chartPanel, 1, 3);
            
            dashboardTab.Controls.Add(container);
        }
        
        private void SetupResultsTab()
        {
            var container = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            
            // Export panel
            var exportPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = surfaceColor,
                Padding = new Padding(10)
            };
            
            exportValidBtn = new RoundedButton
            {
                Text = "📄 Export Valid",
                Size = new Size(130, 40),
                Location = new Point(10, 15),
                BackColor = successColor
            };
            exportValidBtn.Click += ExportValid_Click;
            
            exportInvalidBtn = new RoundedButton
            {
                Text = "📄 Export Invalid",
                Size = new Size(130, 40),
                Location = new Point(150, 15),
                BackColor = errorColor
            };
            exportInvalidBtn.Click += ExportInvalid_Click;
            
            exportAllBtn = new RoundedButton
            {
                Text = "📄 Export All",
                Size = new Size(130, 40),
                Location = new Point(290, 15),
                BackColor = primaryColor
            };
            exportAllBtn.Click += ExportAll_Click;
            
            clearResultsBtn = new RoundedButton
            {
                Text = "🗑️ Clear Results",
                Size = new Size(130, 40),
                Location = new Point(430, 15),
                BackColor = Color.FromArgb(80, 80, 85)
            };
            clearResultsBtn.Click += ClearResults_Click;
            
            retryErrorsBtn = new RoundedButton
            {
                Text = "🔄 Retry Errors",
                Size = new Size(130, 40),
                Location = new Point(570, 15),
                BackColor = Color.FromArgb(255, 193, 7)
            };
            retryErrorsBtn.Click += RetryErrors_Click;
            
            // Search and filter
            var searchLabel = new Label
            {
                Text = "Search:",
                ForeColor = Color.White,
                Location = new Point(720, 20),
                AutoSize = true
            };
            
            searchBox = new TextBox
            {
                Size = new Size(200, 25),
                Location = new Point(770, 18),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(40, 40, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            searchBox.TextChanged += SearchBox_TextChanged;
            
            filterCombo = new ComboBox
            {
                Size = new Size(150, 25),
                Location = new Point(980, 18),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(40, 40, 45),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            filterCombo.Items.AddRange(new string[] { 
                "All Results", "Valid Only", "Invalid Only", "Blocked Only" 
            });
            filterCombo.SelectedIndex = 0;
            filterCombo.SelectedIndexChanged += FilterCombo_SelectedIndexChanged;
            
            exportPanel.Controls.AddRange(new Control[] { 
                exportValidBtn, exportInvalidBtn, exportAllBtn, clearResultsBtn, retryErrorsBtn,
                searchLabel, searchBox, filterCombo 
            });
            
            // Results grid
            resultsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(25, 25, 30),
                BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(40, 40, 45),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(30, 30, 35),
                    ForeColor = Color.White,
                    SelectionBackColor = primaryColor,
                    SelectionForeColor = Color.White
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };
            
            // Add columns
            resultsGrid.Columns.Add("Number", "Phone Number");
            resultsGrid.Columns.Add("Status", "Status");
            resultsGrid.Columns.Add("Pattern", "Detection Pattern");
            resultsGrid.Columns.Add("Time", "Response Time");
            resultsGrid.Columns.Add("Timestamp", "Timestamp");
            
            container.Controls.Add(exportPanel, 0, 0);
            container.Controls.Add(resultsGrid, 0, 1);
            
            resultsTab.Controls.Add(container);
        }
        
        private void SetupConfigurationTab()
        {
            var container = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(10)
            };
            
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            
            // Config list panel
            var configListPanel = new GlassPanel
            {
                Dock = DockStyle.Fill,
                Title = "📁 Configuration Files",
                Margin = new Padding(5)
            };
            
            configListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(25, 25, 30),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None
            };
            configListBox.SelectedIndexChanged += ConfigListBox_SelectedIndexChanged;
            
            var configButtonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(5)
            };
            
            loadConfigFileBtn = new RoundedButton
            {
                Text = "📂 Load",
                Size = new Size(80, 35),
                BackColor = primaryColor,
                Margin = new Padding(2)
            };
            loadConfigFileBtn.Click += LoadConfigFile_Click;
            
            newConfigBtn = new RoundedButton
            {
                Text = "➕ New",
                Size = new Size(80, 35),
                BackColor = successColor,
                Margin = new Padding(2)
            };
            newConfigBtn.Click += NewConfig_Click;
            
            configButtonPanel.Controls.AddRange(new Control[] { loadConfigFileBtn, newConfigBtn });
            configListPanel.Controls.Add(configListBox);
            configListPanel.Controls.Add(configButtonPanel);
            
            // Config editor panel
            var editorPanel = new GlassPanel
            {
                Dock = DockStyle.Fill,
                Title = "✏️ Configuration Editor",
                Margin = new Padding(5)
            };
            
            configEditor = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(20, 20, 25),
                ForeColor = Color.White,
                Font = new Font("Consolas", 10),
                BorderStyle = BorderStyle.None,
                WordWrap = false
            };
            
            var editorButtonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(5)
            };
            
            saveConfigBtn = new RoundedButton
            {
                Text = "💾 Save",
                Size = new Size(100, 35),
                BackColor = successColor,
                Margin = new Padding(2)
            };
            saveConfigBtn.Click += SaveConfig_Click;
            
            testConfigBtn = new RoundedButton
            {
                Text = "🧪 Test",
                Size = new Size(100, 35),
                BackColor = warningColor,
                Margin = new Padding(2)
            };
            testConfigBtn.Click += TestConfig_Click;
            
            editorButtonPanel.Controls.AddRange(new Control[] { saveConfigBtn, testConfigBtn });
            editorPanel.Controls.Add(configEditor);
            editorPanel.Controls.Add(editorButtonPanel);
            
            // Pattern manager panel
            var patternPanel = new GlassPanel
            {
                Dock = DockStyle.Fill,
                Title = "🎯 Pattern Manager",
                Margin = new Padding(5)
            };
            
            var patternContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 2
            };
            
            patternContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            patternContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            patternContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            
            patternTypeCombo = new ComboBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(40, 40, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            patternTypeCombo.Items.AddRange(new string[] { 
                "Success Pattern", "Failure Pattern", "Ban Pattern", "Retry Pattern" 
            });
            patternTypeCombo.SelectedIndex = 0;
            
            patternsListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(25, 25, 30),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.None
            };
            
            patternTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(40, 40, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
            
            var patternButtonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight
            };
            
            addPatternBtn = new RoundedButton
            {
                Text = "➕ Add",
                Size = new Size(80, 30),
                BackColor = successColor,
                Margin = new Padding(2)
            };
            addPatternBtn.Click += AddPattern_Click;
            
            removePatternBtn = new RoundedButton
            {
                Text = "➖ Remove",
                Size = new Size(80, 30),
                BackColor = errorColor,
                Margin = new Padding(2)
            };
            removePatternBtn.Click += RemovePattern_Click;
            
            patternButtonPanel.Controls.AddRange(new Control[] { addPatternBtn, removePatternBtn });
            
            patternContainer.Controls.Add(patternTypeCombo, 0, 0);
            patternContainer.SetColumnSpan(patternTypeCombo, 2);
            patternContainer.Controls.Add(patternsListBox, 0, 1);
            patternContainer.SetColumnSpan(patternsListBox, 2);
            patternContainer.Controls.Add(patternTextBox, 0, 2);
            patternContainer.Controls.Add(patternButtonPanel, 1, 2);
            
            patternPanel.Controls.Add(patternContainer);
            
            // Test panel
            var testPanel = new GlassPanel
            {
                Dock = DockStyle.Fill,
                Title = "🧪 Test Configuration",
                Margin = new Padding(5)
            };
            
            var testContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10)
            };
            
            var testLabel = new Label
            {
                Text = "Test Phone Number:",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                AutoSize = true
            };
            
            testPhoneBox = new TextBox
            {
                Size = new Size(200, 30),
                BackColor = Color.FromArgb(40, 40, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11),
                Text = "1234567890"
            };
            
            var testButton = new RoundedButton
            {
                Text = "🚀 Run Test",
                Size = new Size(200, 40),
                BackColor = primaryColor,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            testButton.Click += TestPhoneNumber_Click;
            
            testContainer.Controls.AddRange(new Control[] { testLabel, testPhoneBox, testButton });
            testPanel.Controls.Add(testContainer);
            
            // Add to layout
            container.Controls.Add(configListPanel, 0, 0);
            container.SetRowSpan(configListPanel, 2);
            container.Controls.Add(editorPanel, 1, 0);
            container.Controls.Add(patternPanel, 1, 1);
            
            configTab.Controls.Add(container);
            
            // Load default config
            LoadDefaultConfig();
        }
        
        private void SetupSettingsTab()
        {
            var container = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10)
            };
            
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            
            // Performance settings
            var performancePanel = new GlassPanel
            {
                Dock = DockStyle.Fill,
                Title = "⚡ Performance Settings",
                Margin = new Padding(5)
            };
            
            var perfContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 6,
                ColumnCount = 2,
                Padding = new Padding(10)
            };
            
            // Threads
            var threadsLabelText = new Label
            {
                Text = "Concurrent Threads:",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            threadsTrackBar = new TrackBar
            {
                Minimum = 1,
                Maximum = 100,
                Value = 10,
                Dock = DockStyle.Fill,
                TickFrequency = 10
            };
            
            threadsLabel = new Label
            {
                Text = "10",
                ForeColor = primaryColor,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            threadsTrackBar.ValueChanged += (s, e) => {
                threadsLabel.Text = threadsTrackBar.Value.ToString();
                maxThreads = threadsTrackBar.Value;
            };
            
            // Delay
            var delayLabelText = new Label
            {
                Text = "Request Delay (ms):",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            delayTrackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 10000,
                Value = 1000,
                Dock = DockStyle.Fill,
                TickFrequency = 1000
            };
            
            delayLabel = new Label
            {
                Text = "1000ms",
                ForeColor = primaryColor,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            delayTrackBar.ValueChanged += (s, e) => {
                delayLabel.Text = $"{delayTrackBar.Value}ms";
                requestDelay = delayTrackBar.Value;
            };
            
            // Options
            saveHtmlCheckBox = new CheckBox
            {
                Text = "Save HTML Responses",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Checked = true,
                Dock = DockStyle.Fill
            };
            saveHtmlCheckBox.CheckedChanged += (s, e) => saveHtmlResponses = saveHtmlCheckBox.Checked;
            
            soundEnabledCheckBox = new CheckBox
            {
                Text = "Enable Sound Effects",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Checked = true,
                Dock = DockStyle.Fill
            };
            soundEnabledCheckBox.CheckedChanged += (s, e) => soundEnabled = soundEnabledCheckBox.Checked;
            
            autoSaveCheckBox = new CheckBox
            {
                Text = "Auto-save Results",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Checked = true,
                Dock = DockStyle.Fill
            };
            
            // Theme selection
            var themeLabelText = new Label
            {
                Text = "Theme:",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                Height = 30,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            themeComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(40, 40, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Height = 30,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            themeComboBox.Items.AddRange(new string[] { 
                "Dark Mode (Default)", 
                "Light Mode", 
                "Ocean Blue", 
                "Forest Green", 
                "Royal Purple", 
                "Sunset Orange",
                "Cherry Red",
                "Midnight Blue"
            });
            themeComboBox.SelectedIndex = 0;
            themeComboBox.SelectedIndexChanged += ThemeComboBox_SelectedIndexChanged;
            
            perfContainer.RowCount = 8; // Increase row count for theme selection
            
            perfContainer.Controls.Add(threadsLabelText, 0, 0);
            perfContainer.Controls.Add(threadsTrackBar, 0, 1);
            perfContainer.Controls.Add(threadsLabel, 1, 1);
            perfContainer.Controls.Add(delayLabelText, 0, 2);
            perfContainer.Controls.Add(delayTrackBar, 0, 3);
            perfContainer.Controls.Add(delayLabel, 1, 3);
            perfContainer.Controls.Add(saveHtmlCheckBox, 0, 4);
            perfContainer.SetColumnSpan(saveHtmlCheckBox, 2);
            perfContainer.Controls.Add(soundEnabledCheckBox, 0, 5);
            perfContainer.SetColumnSpan(soundEnabledCheckBox, 2);
            perfContainer.Controls.Add(autoSaveCheckBox, 0, 6);
            perfContainer.SetColumnSpan(autoSaveCheckBox, 2);
            perfContainer.Controls.Add(themeLabelText, 0, 7);
            perfContainer.Controls.Add(themeComboBox, 1, 7);
            
            performancePanel.Controls.Add(perfContainer);
            
            // Proxy settings
            var proxyPanel = new GlassPanel
            {
                Dock = DockStyle.Fill,
                Title = "🌐 Proxy Settings",
                Margin = new Padding(5)
            };
            
            var proxyContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10)
            };
            
            proxyEnabledCheckBox = new CheckBox
            {
                Text = "Enable Proxy Rotation",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11),
                AutoSize = true
            };
            proxyEnabledCheckBox.CheckedChanged += (s, e) => {
                if (proxyManager != null)
                    proxyManager.RotationEnabled = proxyEnabledCheckBox.Checked;
            };
            
            var proxyFileLabel = new Label
            {
                Text = "Proxy List File:",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 5)
            };
            
            proxyFileTextBox = new TextBox
            {
                Size = new Size(300, 30),
                BackColor = Color.FromArgb(40, 40, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                ReadOnly = true
            };
            
            var proxyButtonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 0)
            };
            
            loadProxyBtn = new RoundedButton
            {
                Text = "📂 Load Proxies",
                Size = new Size(140, 40),
                BackColor = primaryColor,
                Margin = new Padding(0, 0, 10, 0)
            };
            loadProxyBtn.Click += LoadProxies_Click;
            
            testProxiesBtn = new RoundedButton
            {
                Text = "🧪 Test Proxies",
                Size = new Size(140, 40),
                BackColor = warningColor
            };
            testProxiesBtn.Click += TestProxies_Click;
            
            var proxyStatsLabel = new Label
            {
                Name = "proxyStatsLabel",
                Text = "No proxies loaded",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 0)
            };
            
            proxyButtonPanel.Controls.AddRange(new Control[] { loadProxyBtn, testProxiesBtn });
            proxyContainer.Controls.AddRange(new Control[] { 
                proxyEnabledCheckBox, proxyFileLabel, proxyFileTextBox, 
                proxyButtonPanel, proxyStatsLabel 
            });
            
            proxyPanel.Controls.Add(proxyContainer);
            
            // Add to layout
            container.Controls.Add(performancePanel, 0, 0);
            container.Controls.Add(proxyPanel, 1, 0);
            
            settingsTab.Controls.Add(container);
        }
        
        private void SetupMonitoringTab()
        {
            var container = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(10)
            };
            
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            
            // System performance panel
            var systemPanel = new GlassPanel
            {
                Dock = DockStyle.Fill,
                Title = "💻 System Performance",
                Margin = new Padding(5)
            };
            
            var sysContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 2,
                Padding = new Padding(10)
            };
            
            cpuLabel = new Label
            {
                Text = "CPU Usage: 0%",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill
            };
            
            cpuProgressBar = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Style = ProgressBarStyle.Continuous,
                Maximum = 100
            };
            
            memoryLabel = new Label
            {
                Text = "Memory: 0 MB",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill
            };
            
            memoryProgressBar = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Style = ProgressBarStyle.Continuous,
                Maximum = 100
            };
            
            networkLabel = new Label
            {
                Text = "Network: 0 KB/s",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill
            };
            
            sysContainer.Controls.Add(cpuLabel, 0, 0);
            sysContainer.Controls.Add(cpuProgressBar, 1, 0);
            sysContainer.Controls.Add(memoryLabel, 0, 1);
            sysContainer.Controls.Add(memoryProgressBar, 1, 1);
            sysContainer.Controls.Add(networkLabel, 0, 2);
            sysContainer.SetColumnSpan(networkLabel, 2);
            
            systemPanel.Controls.Add(sysContainer);
            
            // Validation metrics panel
            var metricsPanel = new GlassPanel
            {
                Dock = DockStyle.Fill,
                Title = "📊 Validation Metrics",
                Margin = new Padding(5)
            };
            
            var metricsContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10)
            };
            
            validationRateLabel = new Label
            {
                Text = "⚡ Validation Rate: 0/min",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11),
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 5)
            };
            
            successRateLabel = new Label
            {
                Text = "✅ Success Rate: 0%",
                ForeColor = successColor,
                Font = new Font("Segoe UI", 11),
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 5)
            };
            
            avgResponseTimeLabel = new Label
            {
                Text = "⏱️ Avg Response: 0ms",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11),
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 5)
            };
            
            queueSizeLabel = new Label
            {
                Text = "📋 Queue Size: 0",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11),
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 5)
            };
            
            metricsContainer.Controls.AddRange(new Control[] { 
                validationRateLabel, successRateLabel, avgResponseTimeLabel, queueSizeLabel 
            });
            
            metricsPanel.Controls.Add(metricsContainer);
            
            // Performance chart
            var chartPanel = new GlassPanel
            {
                Dock = DockStyle.Fill,
                Title = "📈 Performance Chart",
                Margin = new Padding(5)
            };
            
            performanceChart = new LiveChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(25, 25, 30),
                LineColor = successColor
            };
            
            chartPanel.Controls.Add(performanceChart);
            
            // Error log panel
            var errorPanel = new GlassPanel
            {
                Dock = DockStyle.Fill,
                Title = "⚠️ Error Log",
                Margin = new Padding(5)
            };
            
            errorLogListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(25, 25, 30),
                ForeColor = Color.Orange,
                Font = new Font("Consolas", 9),
                BorderStyle = BorderStyle.None
            };
            
            clearErrorsBtn = new RoundedButton
            {
                Text = "🗑️ Clear",
                Size = new Size(100, 30),
                BackColor = errorColor,
                Dock = DockStyle.Bottom
            };
            clearErrorsBtn.Click += (s, e) => errorLogListBox.Items.Clear();
            
            errorPanel.Controls.Add(errorLogListBox);
            errorPanel.Controls.Add(clearErrorsBtn);
            
            // Add to layout
            container.Controls.Add(systemPanel, 0, 0);
            container.Controls.Add(metricsPanel, 1, 0);
            container.Controls.Add(chartPanel, 0, 1);
            container.Controls.Add(errorPanel, 1, 1);
            
            monitoringTab.Controls.Add(container);
        }
        
        private void InitializeMonitoring()
        {
            try
            {
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                
                monitoringTimer = new System.Windows.Forms.Timer();
                monitoringTimer.Interval = 1000; // Update every second
                monitoringTimer.Tick += MonitoringTimer_Tick;
                monitoringTimer.Start();
            }
            catch (Exception ex)
            {
                LogError($"Monitoring initialization failed: {ex.Message}");
            }
        }
        
        private void MonitoringTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Update system metrics
                if (cpuCounter != null)
                {
                    float cpuUsage = cpuCounter.NextValue();
                    cpuLabel.Text = $"CPU Usage: {cpuUsage:F1}%";
                    cpuProgressBar.Value = (int)Math.Min(cpuUsage, 100);
                }
                
                if (ramCounter != null)
                {
                    float availableRam = ramCounter.NextValue();
                    float totalRam = 16384; // Assume 16GB for now
                    float usedRam = totalRam - availableRam;
                    memoryLabel.Text = $"Memory: {usedRam:F0} MB / {totalRam:F0} MB";
                    memoryProgressBar.Value = (int)((usedRam / totalRam) * 100);
                }
                
                // Update validation metrics
                if (isRunning && totalProcessed > 0)
                {
                    var elapsed = DateTime.Now - startTime;
                    var rate = totalProcessed / Math.Max(1, elapsed.TotalMinutes);
                    validationRateLabel.Text = $"⚡ Validation Rate: {rate:F1}/min";
                    
                    var successRate = (validCount * 100.0) / totalProcessed;
                    successRateLabel.Text = $"✅ Success Rate: {successRate:F1}%";
                    
                    queueSizeLabel.Text = $"📋 Queue Size: {phoneNumbers.Count - totalProcessed}";
                    
                    // Update rate card
                    rateCard.Value = rate.ToString("F0");
                    
                    // Add data point to performance chart
                    performanceChart.AddDataPoint((float)rate);
                }
                
                // Update status bar stats
                var statsLabel = statusPanel.Controls.Find("statsLabel", false).FirstOrDefault() as Label;
                if (statsLabel != null)
                {
                    statsLabel.Text = $"Valid: {validCount} | Invalid: {invalidCount} | Blocked: {blockedCount} | Total: {totalProcessed}";
                }
            }
            catch (Exception ex)
            {
                LogError($"Monitoring update error: {ex.Message}");
            }
        }
        
        // Event Handlers
        private async void LoadWordlist_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.Title = "Select Phone Number List";
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    await LoadWordlistAsync(openFileDialog.FileName);
                }
            }
        }
        
        private async Task LoadWordlistAsync(string fileName)
        {
            try
            {
                // Show loading indicator
                loadWordlistBtn.Enabled = false;
                loadWordlistBtn.Text = "🔄 Loading...";
                UpdateStatus("Loading wordlist...");
                
                // Use ConfigureAwait(false) to avoid deadlocks
                var lines = await Task.Run(() => File.ReadAllLines(fileName)).ConfigureAwait(false);
                
                // Process lines in background thread
                var processedNumbers = await Task.Run(() => 
                {
                    return lines.Where(l => !string.IsNullOrWhiteSpace(l))
                        .Select(l => {
                            var trimmed = l.Trim();
                            var colonIndex = trimmed.IndexOf(':');
                            return colonIndex > 0 ? trimmed.Substring(0, colonIndex) : trimmed;
                        }).ToList();
                }).ConfigureAwait(false);
                
                // Update UI on main thread
                this.BeginInvoke(new Action(() =>
                {
                    phoneNumbers = processedNumbers;
                    UpdateStatus($"Loaded {phoneNumbers.Count} phone numbers");
                    loadWordlistBtn.Enabled = true;
                    loadWordlistBtn.Text = "📁 LOAD WORDLIST";
                }));
            }
            catch (Exception ex)
            {
                // Handle exceptions on UI thread
                this.BeginInvoke(new Action(() =>
                {
                    MessageBox.Show($"Error loading file: {ex.Message}");
                    LogError($"Failed to load wordlist: {ex.Message}");
                    loadWordlistBtn.Enabled = true;
                    loadWordlistBtn.Text = "📁 LOAD WORDLIST";
                    UpdateStatus("Failed to load wordlist");
                }));
            }
        }
        
        private void LoadConfig_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Config files (*.anom)|*.anom|All files (*.*)|*.*";
                openFileDialog.Title = "Select Configuration File";
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Load the full file content into the editor
                    try
                    {
                        string fullContent = File.ReadAllText(openFileDialog.FileName);
                        configEditor.Text = fullContent;
                        
                        // Also parse it for the config manager
                        var config = configManager.LoadConfig(openFileDialog.FileName);
                        if (config != null)
                        {
                            configManager.SetActiveConfig(config);
                            UpdateStatus($"Loaded config: {config.Name} ({openFileDialog.FileName})");
                            RefreshConfigList();
                            
                            // Update pattern list from parsed config
                            patternsListBox.Items.Clear();
                            foreach (var pattern in config.SuccessKeys)
                            {
                                patternsListBox.Items.Add($"[SUCCESS] {pattern}");
                            }
                            foreach (var pattern in config.FailureKeys)
                            {
                                patternsListBox.Items.Add($"[FAILURE] {pattern}");
                            }
                            foreach (var pattern in config.BanKeys)
                            {
                                patternsListBox.Items.Add($"[BAN] {pattern}");
                            }
                            foreach (var pattern in config.RetryKeys)
                            {
                                patternsListBox.Items.Add($"[RETRY] {pattern}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading config file: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private async void Start_Click(object sender, EventArgs e)
        {
            if (phoneNumbers.Count == 0)
            {
                MessageBox.Show("Please load a wordlist first!", "No Data", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Reset counters for new run
            totalProcessed = 0;
            progressBar.Value = 0;
            
            isRunning = true;
            startBtn.Enabled = false;
            pauseBtn.Enabled = true;
            stopBtn.Enabled = true;
            startTime = DateTime.Now;
            
            if (soundEnabled)
            {
                PlaySound("rifle_reload.wav");
            }
            
            cancellationTokenSource = new CancellationTokenSource();
            
            // Start validation in background without blocking UI
            _ = ProcessValidationAsync(cancellationTokenSource.Token);
        }
        
        private void Pause_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                isRunning = false;
                pauseBtn.Text = "▶️ RESUME";
            }
            else
            {
                isRunning = true;
                pauseBtn.Text = "⏸️ PAUSE";
            }
        }
        
        private void Stop_Click(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();
            isRunning = false;
            
            startBtn.Enabled = true;
            pauseBtn.Enabled = false;
            stopBtn.Enabled = false;
            pauseBtn.Text = "⏸️ PAUSE";
            
            if (soundEnabled)
            {
                PlaySound("rifle_hit.wav");
            }
            
            UpdateStatus("Validation stopped - saving progress...");
            
            // Save current progress when stopped
            if (autoSaveCheckBox.Checked && validationResults.Count > 0)
            {
                _ = Task.Run(() =>
                {
                    SaveResults();
                    this.BeginInvoke(new Action(() => 
                    {
                        UpdateStatus($"Validation stopped - {totalProcessed} results saved");
                    }));
                });
            }
            else
            {
                UpdateStatus($"Validation stopped - {totalProcessed} results processed");
            }
        }
        
        private async Task ProcessValidationAsync(CancellationToken token)
        {
            try
            {
                var tasks = new List<Task>();
                var semaphore = new SemaphoreSlim(maxThreads);
                var processedCount = 0;
                var totalCount = phoneNumbers.Count;
                
                // Update UI with initial progress
                this.BeginInvoke(new Action(() => 
                {
                    UpdateStatus($"Starting validation of {totalCount} numbers...");
                }));
                
                foreach (var phoneNumber in phoneNumbers)
                {
                    // Check for cancellation before creating task
                    if (token.IsCancellationRequested) break;
                    
                    // Wait while paused with more responsive checking
                    while (!isRunning && !token.IsCancellationRequested)
                    {
                        await Task.Delay(50, token); // Shorter delay for better responsiveness
                    }
                    
                    // Check again after pause
                    if (token.IsCancellationRequested) break;
                    
                    // Yield control to UI thread periodically
                    if (processedCount % 10 == 0)
                    {
                        await Task.Yield();
                    }
                    
                    var task = Task.Run(async () =>
                {
                    try
                    {
                        await semaphore.WaitAsync(token);
                        
                        // Check for cancellation inside task
                        if (token.IsCancellationRequested) return;
                        
                        ValidationResult result;
                        
                        if (proxyManager.RotationEnabled)
                        {
                            result = await validationEngine.ValidateWithFeatures(phoneNumber);
                        }
                        else
                        {
                            result = TestBotEngine.TestNumber(phoneNumber);
                        }
                        
                        // Check for cancellation before updating UI
                        if (!token.IsCancellationRequested)
                        {
                            this.BeginInvoke(new Action(() => 
                            {
                                AddValidationResult(result);
                                
                                // If successful retry, remove from error list
                                if (result.Status != "ERROR" && errorPhoneNumbers.Contains(result.PhoneNumber))
                                {
                                    errorPhoneNumbers.Remove(result.PhoneNumber);
                                }
                            }));
                        }
                        
                        if (requestDelay > 0 && !token.IsCancellationRequested)
                        {
                            await Task.Delay(requestDelay, token);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Task was cancelled, exit gracefully
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, token);
                
                tasks.Add(task);
                processedCount++;
                
                // Update progress periodically without blocking
                if (processedCount % 50 == 0 || processedCount == totalCount)
                {
                    var currentCount = processedCount;
                    this.BeginInvoke(new Action(() => 
                    {
                        UpdateStatus($"Processing {currentCount}/{totalCount} numbers...");
                    }));
                }
            }
            
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                // Tasks were cancelled
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                this.BeginInvoke(new Action(() => 
                {
                    LogError($"Validation error: {ex.Message}");
                    UpdateStatus("Validation failed due to error");
                }));
            }
            
            this.BeginInvoke(new Action(() =>
            {
                isRunning = false;
                startBtn.Enabled = true;
                pauseBtn.Enabled = false;
                stopBtn.Enabled = false;
                pauseBtn.Text = "⏸️ PAUSE";
                
                if (!token.IsCancellationRequested)
                {
                    UpdateStatus($"Validation complete! Valid: {validCount}, Invalid: {invalidCount}, Blocked: {blockedCount}, Errors: {errorCount}");
                    if (soundEnabled)
                    {
                        PlaySound("rifle_hit.wav");
                    }
                }
            }));
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors in the main validation loop
                this.BeginInvoke(new Action(() => 
                {
                    LogError($"Critical validation error: {ex.Message}");
                    UpdateStatus("Validation stopped due to critical error");
                    isRunning = false;
                    startBtn.Enabled = true;
                    pauseBtn.Enabled = false;
                    stopBtn.Enabled = false;
                    pauseBtn.Text = "⏸️ PAUSE";
                    MessageBox.Show($"Validation stopped due to error: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
        }
        
        private void AddValidationResult(ValidationResult result)
        {
            totalProcessed++;
            
            // Fix progress bar to show actual percentage (0-100)
            if (phoneNumbers.Count > 0)
            {
                int percentage = (totalProcessed * 100) / phoneNumbers.Count;
                progressBar.Value = Math.Min(percentage, 100);
            }
            
            // Update statistics
            switch (result.Status)
            {
                case "VALID":
                    validCount++;
                    validCard.Value = validCount.ToString();
                    break;
                case "INVALID":
                    invalidCount++;
                    invalidCard.Value = invalidCount.ToString();
                    break;
                case "BLOCKED":
                    blockedCount++;
                    blockedCard.Value = blockedCount.ToString();
                    break;
                case "ERROR":
                    errorCount++;
                    errorCard.Value = errorCount.ToString();
                    
                    // Track phone numbers that failed for retry
                    if (!errorPhoneNumbers.Contains(result.PhoneNumber))
                    {
                        errorPhoneNumbers.Add(result.PhoneNumber);
                    }
                    break;
            }
            
            // Progress already updated above
            
            // Add to live feed
            var statusIcon = result.Status == "VALID" ? "✅" : 
                           result.Status == "INVALID" ? "❌" : 
                           result.Status == "BLOCKED" ? "⚠️" : "🟡";
            var message = $"{DateTime.Now:HH:mm:ss} {statusIcon} {result.PhoneNumber}: {result.Status}";
            
            liveResultsFeed.Items.Insert(0, message);
            if (liveResultsFeed.Items.Count > 100)
            {
                liveResultsFeed.Items.RemoveAt(liveResultsFeed.Items.Count - 1);
            }
            
            // Add to results grid
            resultsGrid.Rows.Insert(0, 
                result.PhoneNumber, 
                result.Status, 
                result.DetectionPattern ?? "",
                $"{result.ResponseTimeMs}ms",
                result.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
            );
            
            // Color code the row
            var row = resultsGrid.Rows[0];
            if (result.Status == "VALID")
                row.DefaultCellStyle.ForeColor = successColor;
            else if (result.Status == "INVALID")
                row.DefaultCellStyle.ForeColor = errorColor;
            else if (result.Status == "BLOCKED")
                row.DefaultCellStyle.ForeColor = warningColor;
            else if (result.Status == "ERROR")
                row.DefaultCellStyle.ForeColor = Color.FromArgb(255, 193, 7);
            
            // Save to database
            var record = new ValidationRecord
            {
                PhoneNumber = result.PhoneNumber,
                Status = result.Status,
                Pattern = result.DetectionPattern,
                ResponseTime = result.ResponseTimeMs,
                Timestamp = result.Timestamp
            };
            
            validationResults.Add(record);
            
            // Save to JSON database asynchronously to avoid blocking UI
            _ = Task.Run(() => SaveResultToDatabase(result));
            
            // Refresh pie chart
            if (chartPanel != null && chartPanel.Controls.Count > 0)
            {
                chartPanel.Controls[0].Invalidate();
            }
            
            // Auto-save if enabled
            if (autoSaveCheckBox.Checked && totalProcessed % 100 == 0)
            {
                SaveResults();
            }
        }
        
        // Configuration Tab Methods
        private void LoadDefaultConfig()
        {
            var defaultConfig = configManager.CreateDefaultAmazonConfig();
            configManager.SetActiveConfig(defaultConfig);
            RefreshConfigList();
            DisplayConfig(defaultConfig);
        }
        
        private void RefreshConfigList()
        {
            configListBox.Items.Clear();
            foreach (var config in configManager.LoadedConfigs)
            {
                configListBox.Items.Add(config.Name);
            }
            
            if (configManager.ActiveConfig != null)
            {
                configListBox.SelectedItem = configManager.ActiveConfig.Name;
            }
        }
        
        private void DisplayConfig(ConfigManager.AnomConfig config)
        {
            if (config == null) return;
            
            // Load and display the actual full file content if file path exists
            if (!string.IsNullOrEmpty(config.FilePath) && File.Exists(config.FilePath))
            {
                try
                {
                    configEditor.Text = File.ReadAllText(config.FilePath);
                }
                catch
                {
                    // Fall back to reconstructed version if file can't be read
                    DisplayReconstructedConfig(config);
                }
            }
            else
            {
                // If no file path, show reconstructed version
                DisplayReconstructedConfig(config);
            }
            
            // Update pattern list
            patternsListBox.Items.Clear();
            foreach (var pattern in config.SuccessKeys)
            {
                patternsListBox.Items.Add($"[SUCCESS] {pattern}");
            }
            foreach (var pattern in config.FailureKeys)
            {
                patternsListBox.Items.Add($"[FAILURE] {pattern}");
            }
            foreach (var pattern in config.BanKeys)
            {
                patternsListBox.Items.Add($"[BAN] {pattern}");
            }
        }
        
        private void DisplayReconstructedConfig(ConfigManager.AnomConfig config)
        {
            var content = new StringBuilder();
            content.AppendLine("[SETTINGS]");
            content.AppendLine($"Name = {config.Name}");
            content.AppendLine($"Author = {config.Author}");
            content.AppendLine($"Version = {config.Version}");
            content.AppendLine();
            
            content.AppendLine("[REQUEST]");
            content.AppendLine($"Method = {config.Method}");
            content.AppendLine($"URL = {config.Url}");
            content.AppendLine();
            
            content.AppendLine("[PATTERNS]");
            foreach (var pattern in config.SuccessKeys)
            {
                content.AppendLine($"SUCCESS: {pattern}");
            }
            foreach (var pattern in config.FailureKeys)
            {
                content.AppendLine($"FAILURE: {pattern}");
            }
            foreach (var pattern in config.BanKeys)
            {
                content.AppendLine($"BAN: {pattern}");
            }
            
            configEditor.Text = content.ToString();
        }
        
        private void ConfigListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (configListBox.SelectedItem != null)
            {
                var configName = configListBox.SelectedItem.ToString();
                var config = configManager.LoadedConfigs.FirstOrDefault(c => c.Name == configName);
                if (config != null)
                {
                    DisplayConfig(config);
                }
            }
        }
        
        private void LoadConfigFile_Click(object sender, EventArgs e)
        {
            LoadConfig_Click(sender, e);
        }
        
        private void NewConfig_Click(object sender, EventArgs e)
        {
            var newConfig = configManager.CreateDefaultAmazonConfig();
            newConfig.Name = $"New Config {DateTime.Now:HHmmss}";
            configManager.LoadedConfigs.Add(newConfig);
            RefreshConfigList();
            DisplayConfig(newConfig);
        }
        
        private void SaveConfig_Click(object sender, EventArgs e)
        {
            // Parse config from editor and save
            UpdateStatus("Config saved");
        }
        
        private void TestConfig_Click(object sender, EventArgs e)
        {
            if (testPhoneBox != null && !string.IsNullOrEmpty(testPhoneBox.Text))
            {
                TestPhoneNumber_Click(sender, e);
            }
        }
        
        private void AddPattern_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(patternTextBox.Text)) return;
            
            var config = configManager.ActiveConfig;
            if (config == null) return;
            
            var pattern = patternTextBox.Text.Trim();
            var type = patternTypeCombo.SelectedIndex;
            
            switch (type)
            {
                case 0: // Success
                    config.SuccessKeys.Add(pattern);
                    patternsListBox.Items.Add($"[SUCCESS] {pattern}");
                    break;
                case 1: // Failure
                    config.FailureKeys.Add(pattern);
                    patternsListBox.Items.Add($"[FAILURE] {pattern}");
                    break;
                case 2: // Ban
                    config.BanKeys.Add(pattern);
                    patternsListBox.Items.Add($"[BAN] {pattern}");
                    break;
                case 3: // Retry
                    config.RetryKeys.Add(pattern);
                    patternsListBox.Items.Add($"[RETRY] {pattern}");
                    break;
            }
            
            patternTextBox.Clear();
            DisplayConfig(config);
        }
        
        private void ThemeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (themeComboBox == null) return;
            
            switch (themeComboBox.SelectedIndex)
            {
                case 0: // Dark Mode (Default) - Dracula inspired
                    backgroundColor = Color.FromArgb(40, 42, 54);
                    surfaceColor = Color.FromArgb(68, 71, 90);
                    primaryColor = Color.FromArgb(139, 233, 253);
                    accentColor = Color.FromArgb(189, 147, 249);
                    textColor = Color.FromArgb(248, 248, 242);
                    secondaryTextColor = Color.FromArgb(180, 180, 180);
                    break;
                    
                case 1: // Light Mode - Clean and modern
                    backgroundColor = Color.FromArgb(250, 250, 250);
                    surfaceColor = Color.White;
                    primaryColor = Color.FromArgb(25, 118, 210);
                    accentColor = Color.FromArgb(66, 165, 245);
                    textColor = Color.FromArgb(33, 33, 33);
                    secondaryTextColor = Color.FromArgb(117, 117, 117);
                    successColor = Color.FromArgb(76, 175, 80);
                    errorColor = Color.FromArgb(244, 67, 54);
                    warningColor = Color.FromArgb(255, 152, 0);
                    break;
                    
                case 2: // Ocean Blue - Cool and professional
                    backgroundColor = Color.FromArgb(7, 27, 75);
                    surfaceColor = Color.FromArgb(11, 37, 102);
                    primaryColor = Color.FromArgb(0, 180, 216);
                    accentColor = Color.FromArgb(0, 119, 182);
                    textColor = Color.FromArgb(202, 240, 248);
                    secondaryTextColor = Color.FromArgb(144, 224, 239);
                    break;
                    
                case 3: // Forest Green - Natural and calming
                    backgroundColor = Color.FromArgb(27, 38, 44);
                    surfaceColor = Color.FromArgb(52, 73, 85);
                    primaryColor = Color.FromArgb(86, 207, 113);
                    accentColor = Color.FromArgb(29, 185, 84);
                    textColor = Color.FromArgb(213, 245, 227);
                    secondaryTextColor = Color.FromArgb(165, 214, 167);
                    break;
                    
                case 4: // Royal Purple - Elegant and modern
                    backgroundColor = Color.FromArgb(25, 20, 35);
                    surfaceColor = Color.FromArgb(48, 39, 67);
                    primaryColor = Color.FromArgb(156, 39, 176);
                    accentColor = Color.FromArgb(186, 104, 200);
                    textColor = Color.FromArgb(243, 229, 245);
                    secondaryTextColor = Color.FromArgb(206, 147, 216);
                    break;
                    
                case 5: // Sunset Orange - Warm and energetic
                    backgroundColor = Color.FromArgb(38, 20, 5);
                    surfaceColor = Color.FromArgb(62, 39, 35);
                    primaryColor = Color.FromArgb(255, 152, 0);
                    accentColor = Color.FromArgb(255, 193, 7);
                    textColor = Color.FromArgb(255, 249, 196);
                    secondaryTextColor = Color.FromArgb(255, 224, 130);
                    break;
                    
                case 6: // Cherry Red - Bold and striking
                    backgroundColor = Color.FromArgb(25, 15, 15);
                    surfaceColor = Color.FromArgb(51, 28, 28);
                    primaryColor = Color.FromArgb(244, 67, 54);
                    accentColor = Color.FromArgb(229, 57, 53);
                    textColor = Color.FromArgb(255, 235, 238);
                    secondaryTextColor = Color.FromArgb(255, 205, 210);
                    break;
                    
                case 7: // Midnight Blue - Deep and sophisticated
                    backgroundColor = Color.FromArgb(3, 7, 30);
                    surfaceColor = Color.FromArgb(15, 23, 42);
                    primaryColor = Color.FromArgb(59, 130, 246);
                    accentColor = Color.FromArgb(99, 102, 241);
                    textColor = Color.FromArgb(226, 232, 240);
                    secondaryTextColor = Color.FromArgb(148, 163, 184);
                    break;
            }
            
            // Update UI colors
            ApplyTheme();
            UpdateStatus($"Theme changed to {themeComboBox.SelectedItem}");
        }
        
        private void ApplyTheme()
        {
            // Update form background
            this.BackColor = backgroundColor;
            
            // Update all panels
            if (headerPanel != null) headerPanel.BackColor = surfaceColor;
            if (statusPanel != null) statusPanel.BackColor = surfaceColor;
            
            // Update tab control
            if (mainTabControl != null)
            {
                foreach (TabPage tab in mainTabControl.TabPages)
                {
                    tab.BackColor = backgroundColor;
                    tab.ForeColor = textColor;
                }
            }
            
            // Update all controls
            foreach (Control control in this.GetAllControls())
            {
                if (control is Label label)
                {
                    label.ForeColor = textColor;
                }
                else if (control is RoundedButton button)
                {
                    // RoundedButton needs special handling
                    if (button.BackColor != Color.Transparent)
                    {
                        button.ForeColor = IsLightColor(button.BackColor) ? Color.Black : Color.White;
                    }
                }
                else if (control is Button regularButton)
                {
                    regularButton.ForeColor = IsLightColor(regularButton.BackColor) ? Color.Black : Color.White;
                }
                else if (control is Panel panel && !(panel is GlassPanel))
                {
                    panel.BackColor = backgroundColor;
                }
                else if (control is TextBox textBox)
                {
                    textBox.BackColor = surfaceColor;
                    textBox.ForeColor = textColor;
                }
                else if (control is RichTextBox richTextBox)
                {
                    richTextBox.BackColor = surfaceColor;
                    richTextBox.ForeColor = textColor;
                }
                else if (control is ListBox listBox)
                {
                    listBox.BackColor = surfaceColor;
                    listBox.ForeColor = textColor;
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.BackColor = surfaceColor;
                    comboBox.ForeColor = textColor;
                }
                else if (control is DataGridView grid)
                {
                    grid.BackgroundColor = surfaceColor;
                    grid.DefaultCellStyle.BackColor = surfaceColor;
                    grid.DefaultCellStyle.ForeColor = textColor;
                    grid.DefaultCellStyle.SelectionBackColor = primaryColor;
                    grid.DefaultCellStyle.SelectionForeColor = textColor;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = backgroundColor;
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = textColor;
                    grid.GridColor = Color.FromArgb(60, 60, 65);
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.ForeColor = textColor;
                }
                else if (control is TrackBar trackBar)
                {
                    trackBar.BackColor = backgroundColor;
                }
            }
            
            // Update button colors with proper text contrast
            if (startBtn != null) 
            {
                startBtn.BackColor = successColor;
                startBtn.ForeColor = IsLightColor(successColor) ? Color.Black : Color.White;
            }
            if (pauseBtn != null) 
            {
                pauseBtn.BackColor = warningColor;
                pauseBtn.ForeColor = IsLightColor(warningColor) ? Color.Black : Color.White;
            }
            if (stopBtn != null) 
            {
                stopBtn.BackColor = errorColor;
                stopBtn.ForeColor = IsLightColor(errorColor) ? Color.Black : Color.White;
            }
            if (loadWordlistBtn != null) 
            {
                loadWordlistBtn.BackColor = primaryColor;
                loadWordlistBtn.ForeColor = IsLightColor(primaryColor) ? Color.Black : Color.White;
            }
            if (loadConfigBtn != null) 
            {
                loadConfigBtn.BackColor = primaryColor;
                loadConfigBtn.ForeColor = IsLightColor(primaryColor) ? Color.Black : Color.White;
            }
            if (loadConfigFileBtn != null)
            {
                loadConfigFileBtn.BackColor = primaryColor;
                loadConfigFileBtn.ForeColor = IsLightColor(primaryColor) ? Color.Black : Color.White;
            }
            if (exportValidBtn != null)
            {
                exportValidBtn.BackColor = successColor;
                exportValidBtn.ForeColor = IsLightColor(successColor) ? Color.Black : Color.White;
            }
            if (exportInvalidBtn != null)
            {
                exportInvalidBtn.BackColor = errorColor;
                exportInvalidBtn.ForeColor = IsLightColor(errorColor) ? Color.Black : Color.White;
            }
            if (exportAllBtn != null)
            {
                exportAllBtn.BackColor = primaryColor;
                exportAllBtn.ForeColor = IsLightColor(primaryColor) ? Color.Black : Color.White;
            }
            if (clearResultsBtn != null)
            {
                clearResultsBtn.BackColor = Color.FromArgb(80, 80, 85);
                clearResultsBtn.ForeColor = IsLightColor(Color.FromArgb(80, 80, 85)) ? Color.Black : Color.White;
            }
            if (retryErrorsBtn != null)
            {
                retryErrorsBtn.BackColor = Color.FromArgb(255, 193, 7);
                retryErrorsBtn.ForeColor = IsLightColor(Color.FromArgb(255, 193, 7)) ? Color.Black : Color.White;
            }
            
            // Update progress bar color
            if (progressBar != null) progressBar.ForeColor = primaryColor;
            
            // Update chart colors
            if (validationChart != null) validationChart.LineColor = primaryColor;
            if (performanceChart != null) performanceChart.LineColor = primaryColor;
            
            // Update statistics cards
            if (validCard != null) validCard.ForeColor = textColor;
            if (invalidCard != null) invalidCard.ForeColor = textColor;
            if (blockedCard != null) blockedCard.ForeColor = textColor;
            if (errorCard != null) errorCard.ForeColor = textColor;
            if (rateCard != null) rateCard.ForeColor = textColor;
            
            // Refresh the form and all controls
            this.Refresh();
            
            // Refresh pie chart
            if (chartPanel != null && chartPanel.Controls.Count > 0)
            {
                chartPanel.Controls[0].Invalidate();
            }
        }
        
        // Helper method to determine if a color is light
        private bool IsLightColor(Color color)
        {
            // Calculate perceived brightness using the formula:
            // (0.299 * R + 0.587 * G + 0.114 * B)
            double brightness = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B);
            return brightness > 128;
        }
        
        // Helper method to get all controls recursively
        private IEnumerable<Control> GetAllControls()
        {
            var controls = new List<Control>();
            var queue = new Queue<Control>();
            queue.Enqueue(this);
            
            while (queue.Count > 0)
            {
                var control = queue.Dequeue();
                controls.Add(control);
                
                foreach (Control child in control.Controls)
                {
                    queue.Enqueue(child);
                }
            }
            
            return controls;
        }
        
        private void RemovePattern_Click(object sender, EventArgs e)
        {
            if (patternsListBox.SelectedItem == null) return;
            
            var selectedPattern = patternsListBox.SelectedItem.ToString();
            patternsListBox.Items.Remove(selectedPattern);
            
            // Update config
            var config = configManager.ActiveConfig;
            if (config != null)
            {
                // Parse and remove pattern from appropriate list
                DisplayConfig(config);
            }
        }
        
        private async void TestPhoneNumber_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(testPhoneBox.Text)) return;
            
            UpdateStatus($"Testing: {testPhoneBox.Text}");
            
            var result = await Task.Run(() => TestBotEngine.TestNumber(testPhoneBox.Text));
            
            MessageBox.Show(
                $"Phone: {result.PhoneNumber}\n" +
                $"Status: {result.Status}\n" +
                $"Reason: {result.Reason}\n" +
                $"Pattern: {result.DetectionPattern}\n" +
                $"Response Time: {result.ResponseTimeMs}ms",
                "Test Result",
                MessageBoxButtons.OK,
                result.Status == "VALID" ? MessageBoxIcon.Information : MessageBoxIcon.Warning
            );
        }
        
        // Proxy Methods
        private void LoadProxies_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.Title = "Select Proxy List";
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    proxyManager.LoadProxiesFromFile(openFileDialog.FileName);
                    proxyFileTextBox.Text = openFileDialog.FileName;
                    
                    var statsLabel = settingsTab.Controls.Find("proxyStatsLabel", true).FirstOrDefault() as Label;
                    if (statsLabel != null)
                    {
                        statsLabel.Text = $"Loaded {proxyManager.TotalProxies} proxies";
                    }
                    
                    UpdateStatus($"Loaded {proxyManager.TotalProxies} proxies");
                }
            }
        }
        
        private async void TestProxies_Click(object sender, EventArgs e)
        {
            if (proxyManager.TotalProxies == 0)
            {
                MessageBox.Show("Please load proxies first!");
                return;
            }
            
            UpdateStatus("Testing proxies...");
            testProxiesBtn.Enabled = false;
            
            await proxyManager.TestAllProxiesAsync((tested, total) =>
            {
                this.BeginInvoke(new Action(() =>
                {
                    UpdateStatus($"Testing proxies: {tested}/{total}");
                }));
            });
            
            testProxiesBtn.Enabled = true;
            
            var statsLabel = settingsTab.Controls.Find("proxyStatsLabel", true).FirstOrDefault() as Label;
            if (statsLabel != null)
            {
                statsLabel.Text = $"Working: {proxyManager.WorkingProxies} | Dead: {proxyManager.DeadProxies}";
            }
            
            UpdateStatus($"Proxy test complete: {proxyManager.WorkingProxies} working");
        }
        
        // Export Methods
        private void ExportValid_Click(object sender, EventArgs e)
        {
            ExportResults("VALID");
        }
        
        private void ExportInvalid_Click(object sender, EventArgs e)
        {
            ExportResults("INVALID");
        }
        
        private void ExportAll_Click(object sender, EventArgs e)
        {
            ExportResults("ALL");
        }
        
        private void ClearResults_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to clear all results and reset statistics?", 
                "Clear Results", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                // Clear all statistics
                validCount = 0;
                invalidCount = 0;
                blockedCount = 0;
                errorCount = 0;
                totalProcessed = 0;
                
                // Update UI
                validCard.Value = "0";
                invalidCard.Value = "0";
                blockedCard.Value = "0";
                errorCard.Value = "0";
                rateCard.Value = "0/s";
                progressBar.Value = 0;
                
                // Clear lists
                validationResults.Clear();
                errorPhoneNumbers.Clear();
                liveResultsFeed.Items.Clear();
                resultsGrid.Rows.Clear();
                
                // Refresh pie chart
                chartPanel?.Invalidate();
                
                UpdateStatus("Results cleared and statistics reset");
            }
        }
        
        private void RetryErrors_Click(object sender, EventArgs e)
        {
            if (errorPhoneNumbers.Count == 0)
            {
                MessageBox.Show("No error results to retry.", "Retry Errors", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            if (isRunning)
            {
                MessageBox.Show("Please stop the current validation before retrying errors.", "Retry Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            var result = MessageBox.Show(
                $"Retry validation for {errorPhoneNumbers.Count} numbers that had network errors?", 
                "Retry Errors", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                // Remove error results from grid and stats
                var errorResultsToRemove = validationResults.Where(r => r.Status == "ERROR").ToList();
                foreach (var errorResult in errorResultsToRemove)
                {
                    // Find and remove from results grid
                    foreach (DataGridViewRow row in resultsGrid.Rows)
                    {
                        if (row.Cells[0].Value?.ToString() == errorResult.PhoneNumber)
                        {
                            resultsGrid.Rows.Remove(row);
                            break;
                        }
                    }
                    
                    // Remove from live feed
                    for (int i = liveResultsFeed.Items.Count - 1; i >= 0; i--)
                    {
                        string item = liveResultsFeed.Items[i].ToString();
                        if (item.Contains(errorResult.PhoneNumber) && item.Contains("ERROR"))
                        {
                            liveResultsFeed.Items.RemoveAt(i);
                        }
                    }
                }
                
                // Remove from validation results
                validationResults.RemoveAll(r => r.Status == "ERROR");
                
                // Reset error count and total processed
                totalProcessed -= errorCount;
                errorCount = 0;
                errorCard.Value = "0";
                
                // Update progress bar
                if (phoneNumbers.Count > 0)
                {
                    int percentage = (totalProcessed * 100) / phoneNumbers.Count;
                    progressBar.Value = Math.Min(percentage, 100);
                }
                
                // Start retry process
                var retryNumbers = new List<string>(errorPhoneNumbers);
                errorPhoneNumbers.Clear();
                
                // Add retry numbers to current phone numbers for processing
                phoneNumbers.AddRange(retryNumbers);
                
                // Refresh pie chart
                chartPanel?.Invalidate();
                
                UpdateStatus($"Retrying {retryNumbers.Count} failed numbers...");
                
                // Start validation for retry numbers
                Start_Click(this, EventArgs.Empty);
            }
        }
        
        private void ExportResults(string filter)
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text file (*.txt)|*.txt|CSV file (*.csv)|*.csv|JSON file (*.json)|*.json";
                saveFileDialog.Title = $"Export {filter} Results";
                saveFileDialog.FileName = $"results_{filter.ToLower()}_{DateTime.Now:yyyyMMdd_HHmmss}";
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var results = filter == "ALL" ? 
                            validationResults :
                            validationResults.Where(x => x.Status == filter).ToList();
                        
                        if (saveFileDialog.FilterIndex == 1) // TXT
                        {
                            var lines = results.Select(r => r.PhoneNumber);
                            File.WriteAllLines(saveFileDialog.FileName, lines);
                        }
                        else if (saveFileDialog.FilterIndex == 2) // CSV
                        {
                            var csv = new StringBuilder();
                            csv.AppendLine("PhoneNumber,Status,Pattern,ResponseTime,Timestamp");
                            foreach (var r in results)
                            {
                                csv.AppendLine($"{r.PhoneNumber},{r.Status},{r.Pattern},{r.ResponseTime},{r.Timestamp}");
                            }
                            File.WriteAllText(saveFileDialog.FileName, csv.ToString());
                        }
                        else if (saveFileDialog.FilterIndex == 3) // JSON
                        {
                            var json = JsonConvert.SerializeObject(results, Formatting.Indented);
                            File.WriteAllText(saveFileDialog.FileName, json);
                        }
                        
                        UpdateStatus($"Exported {results.Count} results to {Path.GetFileName(saveFileDialog.FileName)}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Export error: {ex.Message}");
                        LogError($"Export failed: {ex.Message}");
                    }
                }
            }
        }
        
        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            FilterResults();
        }
        
        private void FilterCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterResults();
        }
        
        private void FilterResults()
        {
            // Filter implementation
        }
        
        // Helper Methods
        private void UpdateStatus(string message)
        {
            var statusLabel = statusPanel.Controls.Find("statusLabel", false).FirstOrDefault() as Label;
            if (statusLabel != null)
            {
                statusLabel.Text = $"⚡ {message}";
            }
        }
        
        private void LogError(string error)
        {
            if (errorLogListBox != null)
            {
                this.BeginInvoke(new Action(() =>
                {
                    errorLogListBox.Items.Insert(0, $"{DateTime.Now:HH:mm:ss} - {error}");
                    if (errorLogListBox.Items.Count > 100)
                    {
                        errorLogListBox.Items.RemoveAt(errorLogListBox.Items.Count - 1);
                    }
                }));
            }
        }
        
        private void PlaySound(string soundFile)
        {
            try
            {
                var soundPath = Path.Combine(Application.StartupPath, "Sounds", soundFile);
                if (File.Exists(soundPath))
                {
                    using (var player = new System.Media.SoundPlayer(soundPath))
                    {
                        player.Play();
                    }
                }
            }
            catch { }
        }
        
        private void SaveResults()
        {
            try
            {
                var resultsFile = Path.Combine(Application.StartupPath, "results.json");
                var json = JsonConvert.SerializeObject(validationResults, Formatting.Indented);
                File.WriteAllText(resultsFile, json);
            }
            catch (Exception ex)
            {
                LogError($"Auto-save failed: {ex.Message}");
            }
        }
        
        private void LoadSettings()
        {
            // Load user settings from JSON file
            try
            {
                var settingsFile = Path.Combine(Application.StartupPath, "settings.json");
                if (File.Exists(settingsFile))
                {
                    // Load and apply settings
                }
            }
            catch { }
        }
        
        // Database Methods
        private void SaveResultToDatabase(ValidationResult result)
        {
            try
            {
                List<ValidationResult> allResults;
                
                if (File.Exists(databasePath))
                {
                    string json = File.ReadAllText(databasePath);
                    allResults = JsonConvert.DeserializeObject<List<ValidationResult>>(json) ?? new List<ValidationResult>();
                }
                else
                {
                    allResults = new List<ValidationResult>();
                }
                
                allResults.Add(result);
                
                // Keep only last 10000 results to prevent file from growing too large
                if (allResults.Count > 10000)
                {
                    allResults = allResults.Skip(allResults.Count - 10000).ToList();
                }
                
                string updatedJson = JsonConvert.SerializeObject(allResults, Formatting.Indented);
                File.WriteAllText(databasePath, updatedJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to database: {ex.Message}");
            }
        }
        
        private void LoadResultsFromDatabase()
        {
            try
            {
                if (File.Exists(databasePath))
                {
                    string json = File.ReadAllText(databasePath);
                    var savedResults = JsonConvert.DeserializeObject<List<ValidationResult>>(json);
                    
                    if (savedResults != null && savedResults.Count > 0)
                    {
                        // Update statistics from saved results
                        foreach (var result in savedResults)
                        {
                            switch (result.Status)
                            {
                                case "VALID":
                                    validCount++;
                                    break;
                                case "INVALID":
                                    invalidCount++;
                                    break;
                                case "BLOCKED":
                                    blockedCount++;
                                    break;
                            }
                            
                            // Add to results grid if it exists
                            if (resultsGrid != null)
                            {
                                resultsGrid.Rows.Add(
                                    result.PhoneNumber,
                                    result.Status,
                                    result.DetectionPattern ?? "",
                                    $"{result.ResponseTimeMs}ms",
                                    result.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
                                );
                            }
                        }
                        
                        // Update UI
                        if (validCard != null) validCard.Value = validCount.ToString();
                        if (invalidCard != null) invalidCard.Value = invalidCount.ToString();
                        if (blockedCard != null) blockedCard.Value = blockedCount.ToString();
                        UpdateStatus($"Loaded {savedResults.Count} results from previous session");
                        
                        // Refresh pie chart
                        if (chartPanel != null && chartPanel.Controls.Count > 0)
                        {
                            chartPanel.Controls[0].Invalidate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from database: {ex.Message}");
            }
        }
        
        // Pie Chart Paint Method
        private void PieChartPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            var panel = sender as Panel;
            if (panel == null) return;
            
            // Calculate pie chart dimensions
            int size = Math.Min(panel.Width, panel.Height - 60) - 40;
            if (size < 50) return; // Too small to draw
            
            int x = (panel.Width - size) / 2;
            int y = 20;
            
            Rectangle rect = new Rectangle(x, y, size, size);
            
            // Calculate angles based on results
            float total = validCount + invalidCount + blockedCount + errorCount;
            if (total == 0)
            {
                // Draw empty circle when no data
                using (Pen pen = new Pen(Color.FromArgb(60, 60, 65), 2))
                {
                    g.DrawEllipse(pen, rect);
                }
                
                using (Font font = new Font("Segoe UI", 12))
                using (Brush brush = new SolidBrush(Color.Gray))
                {
                    string text = "No data yet";
                    SizeF textSize = g.MeasureString(text, font);
                    g.DrawString(text, font, brush, 
                        x + (size - textSize.Width) / 2,
                        y + (size - textSize.Height) / 2);
                }
                return;
            }
            
            float validAngle = (validCount / total) * 360;
            float invalidAngle = (invalidCount / total) * 360;
            float blockedAngle = (blockedCount / total) * 360;
            float errorAngle = (errorCount / total) * 360;
            
            // Draw pie slices
            float startAngle = 0;
            
            if (validCount > 0)
            {
                using (Brush brush = new SolidBrush(successColor))
                {
                    g.FillPie(brush, rect, startAngle, validAngle);
                }
                startAngle += validAngle;
            }
            
            if (invalidCount > 0)
            {
                using (Brush brush = new SolidBrush(errorColor))
                {
                    g.FillPie(brush, rect, startAngle, invalidAngle);
                }
                startAngle += invalidAngle;
            }
            
            if (blockedCount > 0)
            {
                using (Brush brush = new SolidBrush(warningColor))
                {
                    g.FillPie(brush, rect, startAngle, blockedAngle);
                }
                startAngle += blockedAngle;
            }
            
            if (errorCount > 0)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(255, 193, 7)))
                {
                    g.FillPie(brush, rect, startAngle, errorAngle);
                }
            }
            
            // Draw percentages in center
            using (Font font = new Font("Segoe UI", 14, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.White))
            {
                string percentage = $"{(validCount / total * 100):F1}%";
                SizeF textSize = g.MeasureString(percentage, font);
                g.DrawString(percentage, font, brush,
                    x + (size - textSize.Width) / 2,
                    y + size / 2 - 10);
                    
                using (Font smallFont = new Font("Segoe UI", 10))
                {
                    string successText = "Success Rate";
                    textSize = g.MeasureString(successText, smallFont);
                    g.DrawString(successText, smallFont, brush,
                        x + (size - textSize.Width) / 2,
                        y + size / 2 + 10);
                }
            }
        }
        
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            monitoringTimer?.Stop();
            monitoringTimer?.Dispose();
            cpuCounter?.Dispose();
            ramCounter?.Dispose();
            validationEngine?.CaptchaHandler?.Dispose();
            
            base.OnFormClosed(e);
        }
    }
}