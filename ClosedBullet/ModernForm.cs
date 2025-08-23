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

namespace ClosedBullet
{
    public partial class ModernForm : Form
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
        private LiveChart validationChart;
        private ListBox liveResultsFeed;
        
        // Results Components
        private DataGridView resultsGrid;
        private RoundedButton exportValidBtn;
        private RoundedButton exportInvalidBtn;
        private RoundedButton exportAllBtn;
        private TextBox searchBox;
        private ComboBox filterCombo;
        
        // Database - simplified for now
        private List<ValidationRecord> validationResults = new List<ValidationRecord>();
        
        // Validation Engine
        private List<string> phoneNumbers = new List<string>();
        private CancellationTokenSource cancellationTokenSource;
        private bool isRunning = false;
        private int currentIndex = 0;
        
        // Statistics
        private int validCount = 0;
        private int invalidCount = 0;
        private int blockedCount = 0;
        private int totalProcessed = 0;
        
        // Theme Colors
        private readonly Color backgroundColor = Color.FromArgb(20, 20, 25);
        private readonly Color surfaceColor = Color.FromArgb(30, 30, 35);
        private readonly Color primaryColor = Color.FromArgb(0, 122, 204);
        private readonly Color successColor = Color.FromArgb(46, 204, 113);
        private readonly Color errorColor = Color.FromArgb(231, 76, 60);
        private readonly Color warningColor = Color.FromArgb(241, 196, 15);
        
        public ModernForm()
        {
            InitializeComponent();
            InitializeDatabase();
            SetupModernUI();
            LoadSettings();
        }
        
        private void InitializeComponent()
        {
            this.Text = "OpenBullet Copy - Modern Edition";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = backgroundColor;
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            
            // Enable drag to move
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
        }
        
        private void InitializeDatabase()
        {
            try
            {
                // Load existing results if file exists
                var resultsFile = Path.Combine(Application.StartupPath, "results.json");
                if (File.Exists(resultsFile))
                {
                    var json = File.ReadAllText(resultsFile);
                    validationResults = JsonConvert.DeserializeObject<List<ValidationRecord>>(json) ?? new List<ValidationRecord>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization error: {ex.Message}");
            }
        }
        
        private void SetupModernUI()
        {
            // Create header panel with window controls
            CreateHeaderPanel();
            
            // Create main tab control
            CreateTabControl();
            
            // Create status bar
            CreateStatusBar();
            
            // Setup each tab
            SetupDashboardTab();
            SetupResultsTab();
            SetupConfigTab();
            SetupSettingsTab();
            SetupMonitoringTab();
        }
        
        private void CreateHeaderPanel()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(15, 15, 20)
            };
            
            // Title label
            var titleLabel = new Label
            {
                Text = "⚡ OpenBullet Copy - Modern Edition",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            
            // Window control buttons
            var closeBtn = CreateWindowButton("✕", Color.FromArgb(231, 76, 60));
            closeBtn.Location = new Point(this.Width - 40, 5);
            closeBtn.Click += (s, e) => this.Close();
            
            var maximizeBtn = CreateWindowButton("□", Color.FromArgb(52, 152, 219));
            maximizeBtn.Location = new Point(this.Width - 70, 5);
            maximizeBtn.Click += (s, e) => {
                this.WindowState = this.WindowState == FormWindowState.Maximized ? 
                    FormWindowState.Normal : FormWindowState.Maximized;
            };
            
            var minimizeBtn = CreateWindowButton("—", Color.FromArgb(46, 204, 113));
            minimizeBtn.Location = new Point(this.Width - 100, 5);
            minimizeBtn.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            
            headerPanel.Controls.AddRange(new Control[] { 
                titleLabel, closeBtn, maximizeBtn, minimizeBtn 
            });
            
            this.Controls.Add(headerPanel);
        }
        
        private Button CreateWindowButton(string text, Color hoverColor)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = hoverColor;
            
            return btn;
        }
        
        private void CreateTabControl()
        {
            mainTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                ItemSize = new Size(180, 40),
                SizeMode = TabSizeMode.Fixed,
                DrawMode = TabDrawMode.OwnerDrawFixed
            };
            
            mainTabControl.DrawItem += TabControl_DrawItem;
            
            // Create tab pages
            dashboardTab = new TabPage("🎯 Dashboard") { BackColor = backgroundColor };
            resultsTab = new TabPage("📊 Results & Analytics") { BackColor = backgroundColor };
            configTab = new TabPage("⚙️ Configuration") { BackColor = backgroundColor };
            settingsTab = new TabPage("🔧 Settings") { BackColor = backgroundColor };
            monitoringTab = new TabPage("📈 Monitoring") { BackColor = backgroundColor };
            
            mainTabControl.TabPages.AddRange(new TabPage[] { 
                dashboardTab, resultsTab, configTab, settingsTab, monitoringTab 
            });
            
            // Add padding for header and status bar
            var containerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 40, 0, 30)
            };
            containerPanel.Controls.Add(mainTabControl);
            
            this.Controls.Add(containerPanel);
        }
        
        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            TabPage page = tabControl.TabPages[e.Index];
            Rectangle tabBounds = tabControl.GetTabRect(e.Index);
            
            // Fill background
            using (SolidBrush brush = new SolidBrush(
                e.State == DrawItemState.Selected ? primaryColor : surfaceColor))
            {
                e.Graphics.FillRectangle(brush, tabBounds);
            }
            
            // Draw text
            TextRenderer.DrawText(e.Graphics, page.Text, tabControl.Font,
                tabBounds, Color.White,
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
                Text = "⚡ Ready",
                ForeColor = Color.Lime,
                Font = new Font("Segoe UI", 9),
                Location = new Point(10, 5),
                AutoSize = true
            };
            
            statusPanel.Controls.Add(statusLabel);
            this.Controls.Add(statusPanel);
        }
        
        private void SetupDashboardTab()
        {
            // Main container with padding
            var container = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                AutoScroll = true
            };
            
            // Top controls panel
            var topPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Top,
                BackColor = surfaceColor,
                Padding = new Padding(15)
            };
            
            // Load Wordlist Button
            loadWordlistBtn = new RoundedButton
            {
                Text = "📁 Load Wordlist",
                Size = new Size(150, 45),
                Location = new Point(15, 15),
                BackColor = primaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            loadWordlistBtn.Click += LoadWordlist_Click;
            
            // Load Config Button
            loadConfigBtn = new RoundedButton
            {
                Text = "⚙️ Load Config",
                Size = new Size(150, 45),
                Location = new Point(180, 15),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            loadConfigBtn.Click += LoadConfig_Click;
            
            // Start Button
            startBtn = new RoundedButton
            {
                Text = "▶️ START",
                Size = new Size(120, 45),
                Location = new Point(350, 15),
                BackColor = successColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            startBtn.Click += Start_Click;
            
            // Pause Button
            pauseBtn = new RoundedButton
            {
                Text = "⏸️ PAUSE",
                Size = new Size(120, 45),
                Location = new Point(480, 15),
                BackColor = warningColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Enabled = false
            };
            pauseBtn.Click += Pause_Click;
            
            // Stop Button
            stopBtn = new RoundedButton
            {
                Text = "⏹️ STOP",
                Size = new Size(120, 45),
                Location = new Point(610, 15),
                BackColor = errorColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Enabled = false
            };
            stopBtn.Click += Stop_Click;
            
            topPanel.Controls.AddRange(new Control[] { 
                loadWordlistBtn, loadConfigBtn, startBtn, pauseBtn, stopBtn 
            });
            
            // Statistics Panel
            var statsPanel = new Panel
            {
                Height = 120,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 0, 10)
            };
            
            // Create statistics cards
            validCard = new StatisticsCard
            {
                Title = "VALID",
                Value = "0",
                Icon = "✅",
                BackColor = Color.FromArgb(40, 40, 45),
                AccentColor = successColor,
                Location = new Point(15, 10),
                Size = new Size(200, 100)
            };
            
            invalidCard = new StatisticsCard
            {
                Title = "INVALID",
                Value = "0",
                Icon = "❌",
                BackColor = Color.FromArgb(40, 40, 45),
                AccentColor = errorColor,
                Location = new Point(230, 10),
                Size = new Size(200, 100)
            };
            
            blockedCard = new StatisticsCard
            {
                Title = "BLOCKED",
                Value = "0",
                Icon = "⚠️",
                BackColor = Color.FromArgb(40, 40, 45),
                AccentColor = warningColor,
                Location = new Point(445, 10),
                Size = new Size(200, 100)
            };
            
            statsPanel.Controls.AddRange(new Control[] { 
                validCard, invalidCard, blockedCard 
            });
            
            // Progress Bar
            progressBar = new ModernProgressBar
            {
                Location = new Point(15, 250),
                Size = new Size(container.Width - 50, 30),
                BackColor = Color.FromArgb(40, 40, 45),
                ForeColor = primaryColor,
                Value = 0,
                Maximum = 100
            };
            
            // Live Results Feed
            var feedPanel = new GlassPanel
            {
                Location = new Point(15, 300),
                Size = new Size(container.Width / 2 - 30, 350),
                Title = "📋 Live Results Feed"
            };
            
            liveResultsFeed = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(25, 25, 30),
                ForeColor = Color.White,
                Font = new Font("Consolas", 9),
                BorderStyle = BorderStyle.None
            };
            
            feedPanel.Controls.Add(liveResultsFeed);
            
            // Add all components to dashboard
            container.Controls.Add(feedPanel);
            container.Controls.Add(progressBar);
            container.Controls.Add(statsPanel);
            container.Controls.Add(topPanel);
            
            dashboardTab.Controls.Add(container);
        }
        
        private void SetupResultsTab()
        {
            var container = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            
            // Export panel
            var exportPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = surfaceColor,
                Padding = new Padding(10)
            };
            
            exportValidBtn = new RoundedButton
            {
                Text = "📄 Export Valid",
                Size = new Size(130, 40),
                Location = new Point(10, 10),
                BackColor = successColor
            };
            exportValidBtn.Click += ExportValid_Click;
            
            exportInvalidBtn = new RoundedButton
            {
                Text = "📄 Export Invalid",
                Size = new Size(130, 40),
                Location = new Point(150, 10),
                BackColor = errorColor
            };
            exportInvalidBtn.Click += ExportInvalid_Click;
            
            exportAllBtn = new RoundedButton
            {
                Text = "📄 Export All",
                Size = new Size(130, 40),
                Location = new Point(290, 10),
                BackColor = primaryColor
            };
            exportAllBtn.Click += ExportAll_Click;
            
            // Search box
            searchBox = new TextBox
            {
                Size = new Size(200, 30),
                Location = new Point(450, 15),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(40, 40, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            searchBox.TextChanged += SearchBox_TextChanged;
            
            // Filter combo
            filterCombo = new ComboBox
            {
                Size = new Size(150, 30),
                Location = new Point(660, 15),
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
                exportValidBtn, exportInvalidBtn, exportAllBtn, searchBox, filterCombo 
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
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
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
                AllowUserToAddRows = false
            };
            
            // Add columns
            resultsGrid.Columns.Add("Number", "Phone Number");
            resultsGrid.Columns.Add("Status", "Status");
            resultsGrid.Columns.Add("Pattern", "Detection Pattern");
            resultsGrid.Columns.Add("Time", "Response Time");
            resultsGrid.Columns.Add("Timestamp", "Timestamp");
            
            container.Controls.Add(resultsGrid);
            container.Controls.Add(exportPanel);
            
            resultsTab.Controls.Add(container);
        }
        
        private void SetupConfigTab()
        {
            var container = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                AutoScroll = true
            };
            
            var configLabel = new Label
            {
                Text = "Configuration Manager - Coming Soon",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            
            container.Controls.Add(configLabel);
            configTab.Controls.Add(container);
        }
        
        private void SetupSettingsTab()
        {
            var container = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                AutoScroll = true
            };
            
            var settingsLabel = new Label
            {
                Text = "Advanced Settings - Coming Soon",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            
            container.Controls.Add(settingsLabel);
            settingsTab.Controls.Add(container);
        }
        
        private void SetupMonitoringTab()
        {
            var container = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                AutoScroll = true
            };
            
            var monitoringLabel = new Label
            {
                Text = "Live Monitoring - Coming Soon",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            
            container.Controls.Add(monitoringLabel);
            monitoringTab.Controls.Add(container);
        }
        
        // Event Handlers
        private void LoadWordlist_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.Title = "Select Phone Number List";
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var lines = File.ReadAllLines(openFileDialog.FileName);
                        phoneNumbers = lines.Where(l => !string.IsNullOrWhiteSpace(l))
                            .Select(l => {
                                var trimmed = l.Trim();
                                var colonIndex = trimmed.IndexOf(':');
                                return colonIndex > 0 ? trimmed.Substring(0, colonIndex) : trimmed;
                            }).ToList();
                        
                        UpdateStatus($"Loaded {phoneNumbers.Count} phone numbers");
                        progressBar.Maximum = phoneNumbers.Count;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading file: {ex.Message}");
                    }
                }
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
                    UpdateStatus($"Loaded config: {Path.GetFileName(openFileDialog.FileName)}");
                }
            }
        }
        
        private async void Start_Click(object sender, EventArgs e)
        {
            if (phoneNumbers.Count == 0)
            {
                MessageBox.Show("Please load a wordlist first!");
                return;
            }
            
            isRunning = true;
            startBtn.Enabled = false;
            pauseBtn.Enabled = true;
            stopBtn.Enabled = true;
            
            cancellationTokenSource = new CancellationTokenSource();
            
            await Task.Run(() => ProcessValidation(cancellationTokenSource.Token));
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
            
            UpdateStatus("Validation stopped");
        }
        
        private void ProcessValidation(CancellationToken token)
        {
            var tasks = new List<Task>();
            var semaphore = new SemaphoreSlim(10); // 10 concurrent threads
            
            foreach (var phoneNumber in phoneNumbers)
            {
                if (token.IsCancellationRequested) break;
                
                while (!isRunning && !token.IsCancellationRequested)
                {
                    Thread.Sleep(100);
                }
                
                tasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var result = TestBotEngine.TestNumber(phoneNumber);
                        
                        // Save to database
                        var record = new ValidationRecord
                        {
                            PhoneNumber = phoneNumber,
                            Status = result.Status,
                            Pattern = result.DetectionPattern,
                            ResponseTime = result.ResponseTimeMs,
                            Timestamp = DateTime.Now
                        };
                        
                        validationResults.Add(record);
                        
                        // Save to JSON file
                        var resultsFile = Path.Combine(Application.StartupPath, "results.json");
                        File.WriteAllText(resultsFile, JsonConvert.SerializeObject(validationResults));
                        
                        // Update UI
                        this.BeginInvoke(new Action(() =>
                        {
                            UpdateStatistics(result.Status);
                            AddToLiveFeed(phoneNumber, result.Status);
                            AddToResultsGrid(record);
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                        }));
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, token));
            }
            
            Task.WaitAll(tasks.ToArray());
            
            this.BeginInvoke(new Action(() =>
            {
                Stop_Click(null, null);
                UpdateStatus($"Validation complete! Valid: {validCount}, Invalid: {invalidCount}, Blocked: {blockedCount}");
            }));
        }
        
        private void UpdateStatistics(string status)
        {
            totalProcessed++;
            
            switch (status)
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
            }
        }
        
        private void AddToLiveFeed(string phoneNumber, string status)
        {
            var statusIcon = status == "VALID" ? "✅" : status == "INVALID" ? "❌" : "⚠️";
            var message = $"{DateTime.Now:HH:mm:ss} {statusIcon} {phoneNumber}: {status}";
            
            liveResultsFeed.Items.Insert(0, message);
            
            if (liveResultsFeed.Items.Count > 100)
            {
                liveResultsFeed.Items.RemoveAt(liveResultsFeed.Items.Count - 1);
            }
        }
        
        private void AddToResultsGrid(ValidationRecord record)
        {
            resultsGrid.Rows.Insert(0, 
                record.PhoneNumber, 
                record.Status, 
                record.Pattern,
                $"{record.ResponseTime}ms",
                record.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
            );
            
            // Color code the row
            var row = resultsGrid.Rows[0];
            if (record.Status == "VALID")
                row.DefaultCellStyle.ForeColor = successColor;
            else if (record.Status == "INVALID")
                row.DefaultCellStyle.ForeColor = errorColor;
            else if (record.Status == "BLOCKED")
                row.DefaultCellStyle.ForeColor = warningColor;
        }
        
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
                        
                        UpdateStatus($"Exported {results.Count()} results to {Path.GetFileName(saveFileDialog.FileName)}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Export error: {ex.Message}");
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
            // Implementation for filtering results grid
            // This would filter the DataGridView based on search text and filter combo
        }
        
        private void UpdateStatus(string message)
        {
            if (statusPanel.Controls.Count > 0)
            {
                statusPanel.Controls[0].Text = $"⚡ {message}";
            }
        }
        
        private void LoadSettings()
        {
            // Load user settings from JSON file
        }
        
        // Window dragging
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        
        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }
        
        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(diff));
            }
        }
        
        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
    }
    
    // Database Model
    public class ValidationRecord
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public string Pattern { get; set; }
        public long ResponseTime { get; set; }
        public DateTime Timestamp { get; set; }
    }
}