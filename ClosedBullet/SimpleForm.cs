using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClosedBullet
{
    public partial class SimpleForm : Form
    {
        // No need for engine instance anymore since TestBotEngine is fully static
        private List<string> phoneNumbers;
        private List<ValidationResult> results;
        private CancellationTokenSource cancellationTokenSource;
        private bool isRunning = false;
        private DateTime startTime;
        private System.Windows.Forms.Timer updateTimer;
        
        // UI Controls
        private Button loadButton;
        private Button startButton;
        private Button stopButton;
        private Button exportButton;
        private Button testSingleButton;
        private TextBox logTextBox;
        private Label statusLabel;
        private Label statsLabel;
        private ProgressBar progressBar;
        private NumericUpDown threadsUpDown;
        private CheckBox soundCheckBox;
        private CheckBox saveHtmlCheckBox;
        private ListBox resultsListBox;
        
        public SimpleForm()
        {
            InitializeComponents();
            InitializeEngine();
        }
        
        private void InitializeComponents()
        {
            this.Text = "🎯 OpenBullet Copy - Amazon Validator (EXACT TestBot Logic)";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
            
            // Initialize controls
            CreateControls();
            
            // Initialize data
            phoneNumbers = new List<string>();
            results = new List<ValidationResult>();
            
            // Initialize timer for statistics updates
            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += UpdateTimer_Tick;
            
            UpdateUI();
        }
        
        private void CreateControls()
        {
            // Load Wordlist Button
            loadButton = new Button();
            loadButton.Text = "📁 Load Wordlist";
            loadButton.Location = new Point(20, 20);
            loadButton.Size = new Size(120, 30);
            loadButton.BackColor = Color.FromArgb(0, 122, 204);
            loadButton.ForeColor = Color.White;
            loadButton.FlatStyle = FlatStyle.Flat;
            loadButton.Click += LoadWordlist_Click;
            this.Controls.Add(loadButton);
            
            // Start Button
            startButton = new Button();
            startButton.Text = "▶️ Start Validation";
            startButton.Location = new Point(150, 20);
            startButton.Size = new Size(120, 30);
            startButton.BackColor = Color.Green;
            startButton.ForeColor = Color.White;
            startButton.FlatStyle = FlatStyle.Flat;
            startButton.Click += StartValidation_Click;
            this.Controls.Add(startButton);
            
            // Stop Button
            stopButton = new Button();
            stopButton.Text = "⏹️ Stop";
            stopButton.Location = new Point(280, 20);
            stopButton.Size = new Size(80, 30);
            stopButton.BackColor = Color.Red;
            stopButton.ForeColor = Color.White;
            stopButton.FlatStyle = FlatStyle.Flat;
            stopButton.Enabled = false;
            stopButton.Click += StopValidation_Click;
            this.Controls.Add(stopButton);
            
            // Export Button
            exportButton = new Button();
            exportButton.Text = "💾 Export";
            exportButton.Location = new Point(370, 20);
            exportButton.Size = new Size(80, 30);
            exportButton.BackColor = Color.FromArgb(0, 122, 204);
            exportButton.ForeColor = Color.White;
            exportButton.FlatStyle = FlatStyle.Flat;
            exportButton.Click += ExportResults_Click;
            this.Controls.Add(exportButton);
            
            // Test Single Button
            testSingleButton = new Button();
            testSingleButton.Text = "🧪 Test Single";
            testSingleButton.Location = new Point(460, 20);
            testSingleButton.Size = new Size(100, 30);
            testSingleButton.BackColor = Color.Orange;
            testSingleButton.ForeColor = Color.White;
            testSingleButton.FlatStyle = FlatStyle.Flat;
            testSingleButton.Click += TestSingle_Click;
            this.Controls.Add(testSingleButton);
            
            // Threads setting
            var threadsLabel = new Label();
            threadsLabel.Text = "Threads:";
            threadsLabel.Location = new Point(600, 25);
            threadsLabel.Size = new Size(60, 20);
            threadsLabel.ForeColor = Color.White;
            this.Controls.Add(threadsLabel);
            
            threadsUpDown = new NumericUpDown();
            threadsUpDown.Minimum = 1;
            threadsUpDown.Maximum = 200;
            threadsUpDown.Value = 10;
            threadsUpDown.Location = new Point(670, 23);
            threadsUpDown.Size = new Size(60, 20);
            threadsUpDown.BackColor = Color.FromArgb(60, 60, 60);
            threadsUpDown.ForeColor = Color.White;
            this.Controls.Add(threadsUpDown);
            
            // Sound checkbox
            soundCheckBox = new CheckBox();
            soundCheckBox.Text = "🔊 Sound";
            soundCheckBox.Location = new Point(750, 25);
            soundCheckBox.Size = new Size(80, 20);
            soundCheckBox.Checked = true;
            soundCheckBox.ForeColor = Color.White;
            this.Controls.Add(soundCheckBox);
            
            // Save HTML checkbox
            saveHtmlCheckBox = new CheckBox();
            saveHtmlCheckBox.Text = "💾 Save HTML";
            saveHtmlCheckBox.Location = new Point(840, 25);
            saveHtmlCheckBox.Size = new Size(100, 20);
            saveHtmlCheckBox.Checked = true;
            saveHtmlCheckBox.ForeColor = Color.White;
            this.Controls.Add(saveHtmlCheckBox);
            
            // Progress bar
            progressBar = new ProgressBar();
            progressBar.Location = new Point(20, 70);
            progressBar.Size = new Size(960, 20);
            progressBar.Style = ProgressBarStyle.Continuous;
            this.Controls.Add(progressBar);
            
            // Statistics label
            statsLabel = new Label();
            statsLabel.Text = "Ready - Using EXACT TestBot validation logic";
            statsLabel.Location = new Point(20, 100);
            statsLabel.Size = new Size(960, 40);
            statsLabel.ForeColor = Color.White;
            statsLabel.Font = new Font("Consolas", 10);
            this.Controls.Add(statsLabel);
            
            // Results list box
            resultsListBox = new ListBox();
            resultsListBox.Location = new Point(20, 150);
            resultsListBox.Size = new Size(480, 400);
            resultsListBox.BackColor = Color.FromArgb(30, 30, 30);
            resultsListBox.ForeColor = Color.White;
            resultsListBox.Font = new Font("Consolas", 9);
            this.Controls.Add(resultsListBox);
            
            // Log text box
            logTextBox = new TextBox();
            logTextBox.Location = new Point(520, 150);
            logTextBox.Size = new Size(460, 400);
            logTextBox.Multiline = true;
            logTextBox.ScrollBars = ScrollBars.Vertical;
            logTextBox.ReadOnly = true;
            logTextBox.BackColor = Color.FromArgb(30, 30, 30);
            logTextBox.ForeColor = Color.Lime;
            logTextBox.Font = new Font("Consolas", 8);
            this.Controls.Add(logTextBox);
            
            // Status label
            statusLabel = new Label();
            statusLabel.Text = "Ready to validate phone numbers";
            statusLabel.Location = new Point(20, 570);
            statusLabel.Size = new Size(960, 20);
            statusLabel.ForeColor = Color.Cyan;
            this.Controls.Add(statusLabel);
        }
        
        private void InitializeEngine()
        {
            AddLog("🚀 OpenBullet Copy Engine initialized");
            AddLog("✅ Using EXACT validation logic from TestBot.cs");
            AddLog("🔊 Sound effects: rifle_reload.wav + rifle_hit.wav");
            AddLog("📊 Real-time statistics with detection patterns");
            AddLog("⚡ High-performance concurrent validation");
            
            results = new List<ValidationResult>();
        }
        
        private void AddLog(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var logEntry = $"[{timestamp}] {message}";
            
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AddLog(message)));
                return;
            }
            
            logTextBox.AppendText(logEntry + Environment.NewLine);
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }
        
        private void LoadWordlist_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.Title = "Select Wordlist File";
                
                // Set initial directory to Wordlists folder
                var wordlistsPath = Path.Combine(Application.StartupPath, "Wordlists");
                if (Directory.Exists(wordlistsPath))
                    openFileDialog.InitialDirectory = wordlistsPath;
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var lines = File.ReadAllLines(openFileDialog.FileName);
                        phoneNumbers = lines.Where(l => !string.IsNullOrWhiteSpace(l))
                            .Select(l => {
                                var trimmed = l.Trim();
                                // Extract phone number only (remove timestamp if present)
                                // Format could be: 15142955315:0000 -> 15142955315
                                var colonIndex = trimmed.IndexOf(':');
                                return colonIndex > 0 ? trimmed.Substring(0, colonIndex) : trimmed;
                            }).ToList();
                        
                        AddLog($"📁 Loaded {phoneNumbers.Count} phone numbers from {Path.GetFileName(openFileDialog.FileName)}");
                        
                        // Log first few numbers for verification
                        if (phoneNumbers.Count > 0)
                        {
                            var sample = string.Join(", ", phoneNumbers.Take(3));
                            AddLog($"🔍 Sample numbers: {sample}...");
                        }
                        
                        statusLabel.Text = $"Loaded {phoneNumbers.Count} phone numbers - Ready to validate";
                        UpdateUI();
                    }
                    catch (Exception ex)
                    {
                        AddLog($"❌ Error loading wordlist: {ex.Message}");
                        MessageBox.Show($"Error loading wordlist: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private async void StartValidation_Click(object sender, EventArgs e)
        {
            if (phoneNumbers.Count == 0)
            {
                MessageBox.Show("Please load a wordlist first.", "No Wordlist", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            isRunning = true;
            cancellationTokenSource = new CancellationTokenSource();
            startTime = DateTime.Now;
            
            // Reset counters
            TestBotEngine.ResetCounters();
            results.Clear();
            resultsListBox.Items.Clear();
            
            // Configure engine
            TestBotEngine.SaveHtmlResponses = saveHtmlCheckBox.Checked;
            
            // Play start sound
            if (soundCheckBox.Checked)
            {
                try 
                { 
                    var startSoundPath = Path.Combine(Application.StartupPath, "Sounds", "rifle_reload.wav");
                    if (File.Exists(startSoundPath))
                        new SoundPlayer(startSoundPath).Play();
                    else
                        SystemSounds.Beep.Play();
                }
                catch { SystemSounds.Beep.Play(); }
            }
            
            // Start timer
            updateTimer.Start();
            
            // Update UI
            UpdateUI();
            AddLog($"🎯 Starting validation of {phoneNumbers.Count} numbers");
            AddLog($"⚙️ Using {threadsUpDown.Value} concurrent threads");
            
            // Start validation
            await RunValidation(cancellationTokenSource.Token);
        }
        
        private void StopValidation_Click(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();
            isRunning = false;
            updateTimer.Stop();
            UpdateUI();
            AddLog("⏹️ Validation stopped by user");
            statusLabel.Text = "Validation stopped";
        }
        
        private async Task RunValidation(CancellationToken cancellationToken)
        {
            var maxThreads = (int)threadsUpDown.Value;
            var semaphore = new SemaphoreSlim(maxThreads, maxThreads);
            var tasks = new List<Task>();
            
            foreach (var phoneNumber in phoneNumbers)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                
                var task = Task.Run(async () =>
                {
                    await semaphore.WaitAsync(cancellationToken);
                    try
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            var result = TestBotEngine.TestNumber(phoneNumber);
                            
                            // Update UI on main thread
                            this.Invoke(new Action(() =>
                            {
                                results.Add(result);
                                var status = result.Status == "VALID" ? "✅" : result.Status == "INVALID" ? "❌" : result.Status == "BLOCKED" ? "🚫" : "❓";
                                resultsListBox.Items.Add($"{status} {result.PhoneNumber} - {result.Status} ({result.DetectionPattern})");
                                resultsListBox.SelectedIndex = resultsListBox.Items.Count - 1;
                                
                                AddLog($"{status} {result.PhoneNumber}: {result.Status} - {result.Reason}");
                                
                                // Play sound for valid results
                                if (soundCheckBox.Checked && result.Status == "VALID")
                                {
                                    try { SystemSounds.Exclamation.Play(); }
                                    catch { }
                                }
                            }));
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, cancellationToken);
                
                tasks.Add(task);
                
                // Add delay to avoid overwhelming Amazon
                await Task.Delay(200, cancellationToken);
            }
            
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelled
            }
            
            // Validation complete
            isRunning = false;
            updateTimer.Stop();
            UpdateUI();
            
            // Play complete sound
            if (soundCheckBox.Checked)
            {
                try 
                { 
                    var completeSoundPath = Path.Combine(Application.StartupPath, "Sounds", "rifle_hit.wav");
                    if (File.Exists(completeSoundPath))
                        new SoundPlayer(completeSoundPath).Play();
                    else
                        SystemSounds.Asterisk.Play();
                }
                catch { SystemSounds.Asterisk.Play(); }
            }
            
            AddLog("✅ Validation complete!");
            AddLog($"📊 Final Results: Valid={TestBotEngine.ValidCount}, Invalid={TestBotEngine.InvalidCount}, Blocked={TestBotEngine.BlockedCount}, Unknown={TestBotEngine.UnknownCount}");
            statusLabel.Text = $"Complete! Valid: {TestBotEngine.ValidCount}, Invalid: {TestBotEngine.InvalidCount}, Blocked: {TestBotEngine.BlockedCount}";
        }
        
        private void ExportResults_Click(object sender, EventArgs e)
        {
            if (results.Count == 0)
            {
                MessageBox.Show("No results to export.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt";
                saveFileDialog.FileName = $"OpenBullet_Results_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var lines = new List<string>
                        {
                            "PhoneNumber,Status,Reason,DetectionPattern,Timestamp,ResponseTimeMs"
                        };
                        
                        lines.AddRange(results.Select(r => 
                            $"{r.PhoneNumber},{r.Status},{r.Reason},{r.DetectionPattern},{r.Timestamp:yyyy-MM-dd HH:mm:ss},{r.ResponseTimeMs}"));
                        
                        File.WriteAllLines(saveFileDialog.FileName, lines);
                        AddLog($"💾 Results exported to {Path.GetFileName(saveFileDialog.FileName)}");
                        statusLabel.Text = $"Exported {results.Count} results to {Path.GetFileName(saveFileDialog.FileName)}";
                    }
                    catch (Exception ex)
                    {
                        AddLog($"❌ Export error: {ex.Message}");
                        MessageBox.Show($"Error exporting results: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private void TestSingle_Click(object sender, EventArgs e)
        {
            var input = Microsoft.VisualBasic.Interaction.InputBox("Enter phone number to test:", "Test Single Number", "11111111111");
            if (!string.IsNullOrEmpty(input))
            {
                AddLog($"🧪 Testing single number: {input.Trim()}");
                statusLabel.Text = $"Testing {input.Trim()}...";
                
                Task.Run(() =>
                {
                    var result = TestBotEngine.TestNumber(input.Trim());
                    
                    this.Invoke(new Action(() =>
                    {
                        var status = result.Status == "VALID" ? "✅" : result.Status == "INVALID" ? "❌" : result.Status == "BLOCKED" ? "🚫" : "❓";
                        AddLog($"{status} {result.PhoneNumber}: {result.Status} - {result.Reason} ({result.ResponseTimeMs}ms)");
                        
                        MessageBox.Show($"Phone Number: {result.PhoneNumber}\n" +
                                       $"Status: {result.Status}\n" +
                                       $"Reason: {result.Reason}\n" +
                                       $"Detection Pattern: {result.DetectionPattern}\n" +
                                       $"Response Time: {result.ResponseTimeMs}ms\n" +
                                       $"Title: {result.ResponseTitle}", 
                                       "Test Result", MessageBoxButtons.OK, 
                                       result.Status == "VALID" ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                        
                        statusLabel.Text = $"Test complete: {result.Status}";
                    }));
                });
            }
        }
        
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (isRunning)
            {
                UpdateStatistics();
            }
        }
        
        private void UpdateStatistics()
        {
            var total = phoneNumbers.Count;
            var checked_count = TestBotEngine.ValidCount + TestBotEngine.InvalidCount + TestBotEngine.BlockedCount + TestBotEngine.UnknownCount;
            var elapsed = DateTime.Now - startTime;
            var cpm = elapsed.TotalMinutes > 0 ? checked_count / elapsed.TotalMinutes : 0;
            
            var stats = $"📊 Progress: {checked_count}/{total} ({(total > 0 ? (double)checked_count / total * 100 : 0):F1}%) | " +
                       $"✅ Valid: {TestBotEngine.ValidCount} | ❌ Invalid: {TestBotEngine.InvalidCount} | " +
                       $"🚫 Blocked: {TestBotEngine.BlockedCount} | ❓ Unknown: {TestBotEngine.UnknownCount}\n" +
                       $"⏱️ Elapsed: {elapsed:hh\\:mm\\:ss} | ⚡ CPM: {cpm:F1} | 🧵 Threads: {threadsUpDown.Value}";
            
            statsLabel.Text = stats;
            
            if (total > 0)
            {
                progressBar.Value = Math.Min(100, (int)((double)checked_count / total * 100));
            }
            
            statusLabel.Text = $"Validating... {checked_count}/{total} completed ({cpm:F1} CPM)";
        }
        
        private void UpdateUI()
        {
            var hasWordlist = phoneNumbers.Count > 0;
            
            loadButton.Enabled = !isRunning;
            startButton.Enabled = hasWordlist && !isRunning;
            stopButton.Enabled = isRunning;
            exportButton.Enabled = results.Count > 0 && !isRunning;
            testSingleButton.Enabled = !isRunning;
            
            threadsUpDown.Enabled = !isRunning;
        }
    }
}