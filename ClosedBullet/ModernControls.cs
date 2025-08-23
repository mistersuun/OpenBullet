using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace ClosedBullet
{
    // Rounded Button with hover effects
    public class RoundedButton : Button
    {
        private int borderRadius = 15;
        private Color hoverColor;
        private Color originalColor;
        private bool isHovering = false;
        
        [Category("Appearance")]
        public int BorderRadius
        {
            get { return borderRadius; }
            set { borderRadius = value; this.Invalidate(); }
        }
        
        public RoundedButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = Color.FromArgb(0, 122, 204);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            this.Cursor = Cursors.Hand;
            
            this.MouseEnter += (s, e) => {
                isHovering = true;
                originalColor = this.BackColor;
                hoverColor = ControlPaint.Light(this.BackColor, 0.2f);
                this.BackColor = hoverColor;
            };
            
            this.MouseLeave += (s, e) => {
                isHovering = false;
                this.BackColor = originalColor;
            };
        }
        
        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            GraphicsPath path = GetRoundedRectPath(rect, borderRadius);
            
            this.Region = new Region(path);
            
            // Draw background
            using (SolidBrush brush = new SolidBrush(this.BackColor))
            {
                pevent.Graphics.FillPath(brush, path);
            }
            
            // Draw shadow effect
            if (isHovering)
            {
                using (Pen pen = new Pen(Color.FromArgb(50, 255, 255, 255), 2))
                {
                    pevent.Graphics.DrawPath(pen, path);
                }
            }
            
            // Draw text
            TextRenderer.DrawText(pevent.Graphics, this.Text, this.Font,
                rect, this.ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
        
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float r = radius;
            
            path.AddArc(rect.X, rect.Y, r, r, 180, 90);
            path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
            path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
            path.CloseFigure();
            
            return path;
        }
    }
    
    // Modern Progress Bar with gradient
    public class ModernProgressBar : Control
    {
        private int value = 0;
        private int maximum = 100;
        private Color gradientStart = Color.FromArgb(0, 122, 204);
        private Color gradientEnd = Color.FromArgb(0, 200, 255);
        
        public int Value
        {
            get { return value; }
            set 
            { 
                this.value = Math.Min(value, maximum);
                this.Invalidate();
            }
        }
        
        public int Maximum
        {
            get { return maximum; }
            set 
            { 
                maximum = value;
                this.Invalidate();
            }
        }
        
        public ModernProgressBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                    ControlStyles.UserPaint | 
                    ControlStyles.DoubleBuffer, true);
            
            this.Size = new Size(300, 30);
            this.BackColor = Color.FromArgb(40, 40, 45);
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw background
            using (SolidBrush bgBrush = new SolidBrush(this.BackColor))
            {
                e.Graphics.FillRectangle(bgBrush, this.ClientRectangle);
            }
            
            // Calculate progress width
            if (maximum > 0 && value > 0)
            {
                int progressWidth = (int)((float)value / maximum * this.Width);
                Rectangle progressRect = new Rectangle(0, 0, progressWidth, this.Height);
                
                // Draw gradient progress
                using (LinearGradientBrush gradientBrush = new LinearGradientBrush(
                    progressRect, gradientStart, gradientEnd, LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(gradientBrush, progressRect);
                }
                
                // Draw percentage text
                string percentText = $"{(value * 100 / maximum)}%";
                using (Font font = new Font("Segoe UI", 10, FontStyle.Bold))
                {
                    SizeF textSize = e.Graphics.MeasureString(percentText, font);
                    float x = (this.Width - textSize.Width) / 2;
                    float y = (this.Height - textSize.Height) / 2;
                    
                    e.Graphics.DrawString(percentText, font, Brushes.White, x, y);
                }
            }
        }
    }
    
    // Statistics Card with animation
    public class StatisticsCard : Panel
    {
        private string title = "TITLE";
        private string value = "0";
        private string icon = "📊";
        private Color accentColor = Color.FromArgb(0, 122, 204);
        private Timer animationTimer;
        private int targetValue = 0;
        private int currentValue = 0;
        
        public string Title
        {
            get { return title; }
            set { title = value; this.Invalidate(); }
        }
        
        public string Value
        {
            get { return value; }
            set 
            { 
                if (int.TryParse(value, out int newTarget))
                {
                    targetValue = newTarget;
                    StartAnimation();
                }
                else
                {
                    this.value = value;
                    this.Invalidate();
                }
            }
        }
        
        public string Icon
        {
            get { return icon; }
            set { icon = value; this.Invalidate(); }
        }
        
        public Color AccentColor
        {
            get { return accentColor; }
            set { accentColor = value; this.Invalidate(); }
        }
        
        public StatisticsCard()
        {
            this.Size = new Size(200, 100);
            this.BackColor = Color.FromArgb(40, 40, 45);
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                    ControlStyles.UserPaint | 
                    ControlStyles.DoubleBuffer, true);
            
            animationTimer = new Timer();
            animationTimer.Interval = 30;
            animationTimer.Tick += AnimationTimer_Tick;
        }
        
        private void StartAnimation()
        {
            animationTimer.Start();
        }
        
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (currentValue < targetValue)
            {
                currentValue = Math.Min(currentValue + Math.Max(1, (targetValue - currentValue) / 10), targetValue);
                this.value = currentValue.ToString();
                this.Invalidate();
            }
            else if (currentValue > targetValue)
            {
                currentValue = Math.Max(currentValue - Math.Max(1, (currentValue - targetValue) / 10), targetValue);
                this.value = currentValue.ToString();
                this.Invalidate();
            }
            else
            {
                animationTimer.Stop();
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw rounded background
            using (GraphicsPath path = GetRoundedRect(this.ClientRectangle, 10))
            {
                using (SolidBrush brush = new SolidBrush(this.BackColor))
                {
                    e.Graphics.FillPath(brush, path);
                }
                
                // Draw accent line
                Rectangle accentRect = new Rectangle(0, 0, 5, this.Height);
                using (SolidBrush accentBrush = new SolidBrush(accentColor))
                {
                    e.Graphics.FillRectangle(accentBrush, accentRect);
                }
            }
            
            // Draw icon
            using (Font iconFont = new Font("Segoe UI", 24))
            {
                e.Graphics.DrawString(icon, iconFont, Brushes.White, 15, 10);
            }
            
            // Draw title
            using (Font titleFont = new Font("Segoe UI", 9))
            {
                e.Graphics.DrawString(title, titleFont, 
                    new SolidBrush(Color.FromArgb(150, 150, 150)), 15, 50);
            }
            
            // Draw value
            using (Font valueFont = new Font("Segoe UI", 20, FontStyle.Bold))
            {
                SizeF textSize = e.Graphics.MeasureString(value, valueFont);
                float x = this.Width - textSize.Width - 15;
                float y = (this.Height - textSize.Height) / 2;
                
                e.Graphics.DrawString(value, valueFont, Brushes.White, x, y);
            }
        }
        
        private GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius - 1, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius - 1, rect.Bottom - radius - 1, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius - 1, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
    
    // Glass Panel with blur effect
    public class GlassPanel : Panel
    {
        private string title = "Panel Title";
        
        public string Title
        {
            get { return title; }
            set { title = value; this.Invalidate(); }
        }
        
        public GlassPanel()
        {
            this.BackColor = Color.FromArgb(30, 30, 35);
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                    ControlStyles.UserPaint | 
                    ControlStyles.DoubleBuffer, true);
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw semi-transparent background
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(200, 30, 30, 35)))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
            
            // Draw border
            using (Pen pen = new Pen(Color.FromArgb(50, 255, 255, 255), 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
            
            // Draw title bar
            Rectangle titleRect = new Rectangle(0, 0, this.Width, 30);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(
                titleRect, 
                Color.FromArgb(40, 40, 45), 
                Color.FromArgb(30, 30, 35), 
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(gradientBrush, titleRect);
            }
            
            // Draw title text
            using (Font font = new Font("Segoe UI", 10, FontStyle.Bold))
            {
                e.Graphics.DrawString(title, font, Brushes.White, 10, 5);
            }
        }
    }
    
    // Live Chart control
    public class LiveChart : Control
    {
        private List<float> dataPoints = new List<float>();
        private int maxPoints = 50;
        private Color lineColor = Color.FromArgb(0, 200, 255);
        private Timer updateTimer;
        
        public Color LineColor
        {
            get { return lineColor; }
            set { lineColor = value; this.Invalidate(); }
        }
        
        public LiveChart()
        {
            this.Size = new Size(400, 200);
            this.BackColor = Color.FromArgb(30, 30, 35);
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                    ControlStyles.UserPaint | 
                    ControlStyles.DoubleBuffer, true);
            
            updateTimer = new Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += (s, e) => AddDataPoint(new Random().Next(0, 100));
            updateTimer.Start();
        }
        
        public void AddDataPoint(float value)
        {
            dataPoints.Add(value);
            if (dataPoints.Count > maxPoints)
            {
                dataPoints.RemoveAt(0);
            }
            this.Invalidate();
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw background
            using (SolidBrush brush = new SolidBrush(this.BackColor))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
            
            // Draw grid lines
            using (Pen gridPen = new Pen(Color.FromArgb(50, 50, 55), 1))
            {
                for (int i = 0; i < 5; i++)
                {
                    int y = (int)(i * this.Height / 4.0f);
                    e.Graphics.DrawLine(gridPen, 0, y, this.Width, y);
                }
            }
            
            // Draw data line
            if (dataPoints.Count > 1)
            {
                using (Pen linePen = new Pen(lineColor, 2))
                {
                    float xStep = (float)this.Width / (maxPoints - 1);
                    
                    for (int i = 1; i < dataPoints.Count; i++)
                    {
                        float x1 = (i - 1) * xStep;
                        float y1 = this.Height - (dataPoints[i - 1] / 100.0f * this.Height);
                        float x2 = i * xStep;
                        float y2 = this.Height - (dataPoints[i] / 100.0f * this.Height);
                        
                        e.Graphics.DrawLine(linePen, x1, y1, x2, y2);
                        
                        // Draw glow effect
                        using (Pen glowPen = new Pen(Color.FromArgb(50, lineColor), 4))
                        {
                            e.Graphics.DrawLine(glowPen, x1, y1, x2, y2);
                        }
                    }
                }
            }
        }
    }
}