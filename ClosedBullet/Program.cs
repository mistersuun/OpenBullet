using System;
using System.Threading;
using System.Windows.Forms;

namespace ClosedBullet
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Show console info (if console is available)
            Console.WriteLine(@"
================================================
CLOSEDBULLET - MODERN EDITION
Advanced Account Validation Suite
================================================

Features:
✅ EXACT same validation logic as TestBot.cs
🎯 Modern UI with glassmorphism & rounded buttons
📊 Real-time charts and statistics
💾 SQLite database integration
📄 Export to TXT/CSV/JSON
🔧 Proxy support (SOCKS4/5)
🤖 CAPTCHA solving with Tesseract OCR
⚙️ Multiple .anom config support
🎨 Dark theme with accent colors

Starting application...");

            try
            {
                // Show simple splash screen
                using (var splash = new SimpleSplashForm())
                {
                    splash.Show();
                    splash.Refresh();
                    
                    // Simulate loading
                    Thread.Sleep(1500);
                    
                    splash.Hide();
                }
                
                // Show UI selection
                using (var selection = new SimpleUISelection())
                {
                    var result = selection.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        // Show loading screen while UI initializes
                        using (var loading = new FinalLoadingForm(selection.UseModernUI))
                        {
                            loading.Show();
                            loading.Refresh();
                            
                            // Give time for loading screen to appear
                            Thread.Sleep(800);
                            
                            loading.Hide();
                        }
                        
                        if (selection.UseModernUI)
                        {
                            Application.Run(new ModernFormV2());
                        }
                        else
                        {
                            Application.Run(new SimpleForm());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fatal error: {ex.Message}\n\nStack trace:\n{ex.StackTrace}", 
                               "ClosedBullet Error", 
                               MessageBoxButtons.OK, 
                               MessageBoxIcon.Error);
            }
        }
    }
    
    public class SimpleSplashForm : Form
    {
        public SimpleSplashForm()
        {
            this.Text = "ClosedBullet - Loading";
            this.Size = new System.Drawing.Size(400, 200);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.FromArgb(25, 25, 30);
            this.ShowInTaskbar = false;
            
            var titleLabel = new Label
            {
                Text = "ClosedBullet",
                Font = new System.Drawing.Font("Segoe UI", 18, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                AutoSize = true,
                Location = new System.Drawing.Point(100, 60)
            };
            
            var loadingLabel = new Label
            {
                Text = "Loading...",
                Font = new System.Drawing.Font("Segoe UI", 12),
                ForeColor = System.Drawing.Color.FromArgb(100, 149, 237),
                AutoSize = true,
                Location = new System.Drawing.Point(170, 100)
            };
            
            this.Controls.AddRange(new Control[] { titleLabel, loadingLabel });
        }
    }
    
    public class SimpleUISelection : Form
    {
        public bool UseModernUI { get; private set; }
        
        public SimpleUISelection()
        {
            this.Text = "Select UI Version";
            this.Size = new System.Drawing.Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(25, 25, 30);
            
            var titleLabel = new Label
            {
                Text = "Choose Your Interface",
                Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                AutoSize = true,
                Location = new System.Drawing.Point(150, 30)
            };
            
            var modernBtn = new Button
            {
                Text = "🎯 Modern UI\n\n• Glassmorphism design\n• Real-time charts\n• Dark themes\n• Export options",
                Size = new System.Drawing.Size(200, 150),
                Location = new System.Drawing.Point(50, 80),
                BackColor = System.Drawing.Color.FromArgb(100, 149, 237),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 10)
            };
            
            var classicBtn = new Button
            {
                Text = "⚡ Classic UI\n\n• Simple interface\n• Fast performance\n• Minimal design\n• Quick startup",
                Size = new System.Drawing.Size(200, 150),
                Location = new System.Drawing.Point(270, 80),
                BackColor = System.Drawing.Color.FromArgb(255, 193, 7),
                ForeColor = System.Drawing.Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 10)
            };
            
            // Add event handlers after both buttons are declared
            modernBtn.Click += (s, e) => {
                modernBtn.Text = "🔄 Loading Modern UI...";
                modernBtn.Enabled = false;
                classicBtn.Enabled = false;
                this.Refresh();
                System.Threading.Thread.Sleep(500); // Show loading state
                UseModernUI = true; 
                DialogResult = DialogResult.OK; 
            };
            
            classicBtn.Click += (s, e) => {
                classicBtn.Text = "🔄 Loading Classic UI...";
                classicBtn.Enabled = false;
                modernBtn.Enabled = false;
                this.Refresh();
                System.Threading.Thread.Sleep(500); // Show loading state
                UseModernUI = false; 
                DialogResult = DialogResult.OK; 
            };
            
            this.Controls.AddRange(new Control[] { titleLabel, modernBtn, classicBtn });
        }
    }
    
    public class FinalLoadingForm : Form
    {
        public FinalLoadingForm(bool isModernUI)
        {
            this.Text = "Loading UI...";
            this.Size = new System.Drawing.Size(400, 200);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.FromArgb(25, 25, 30);
            this.ShowInTaskbar = false;
            
            var titleLabel = new Label
            {
                Text = isModernUI ? "Loading ClosedBullet Modern UI..." : "Loading ClosedBullet Classic UI...",
                Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                AutoSize = true,
                Location = new System.Drawing.Point(80, 60)
            };
            
            var statusLabel = new Label
            {
                Text = "Initializing components...",
                Font = new System.Drawing.Font("Segoe UI", 11),
                ForeColor = System.Drawing.Color.FromArgb(100, 149, 237),
                AutoSize = true,
                Location = new System.Drawing.Point(130, 100)
            };
            
            // Add border
            this.Paint += (s, e) =>
            {
                using (var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(100, 149, 237), 2))
                {
                    e.Graphics.DrawRectangle(pen, 1, 1, this.Width - 3, this.Height - 3);
                }
            };
            
            this.Controls.AddRange(new Control[] { titleLabel, statusLabel });
        }
    }
}