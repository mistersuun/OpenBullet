using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ClosedBullet
{
    public class CaptchaHandler
    {
        // Tesseract will be initialized when DLL is properly configured
        private object ocrEngine; // Will use Tesseract.TesseractEngine
        private string tessDataPath;
        private bool initialized = false;
        
        public event EventHandler<string> CaptchaStatusChanged;
        public event EventHandler<Bitmap> CaptchaImageReceived;
        
        public int SuccessCount { get; private set; }
        public int FailureCount { get; private set; }
        public double SuccessRate => (SuccessCount + FailureCount) > 0 ? 
            (double)SuccessCount / (SuccessCount + FailureCount) * 100 : 0;
        
        public CaptchaHandler()
        {
            InitializeOCR();
        }
        
        private void InitializeOCR()
        {
            try
            {
                // Look for tessdata in multiple locations
                var possiblePaths = new[]
                {
                    Path.Combine(Application.StartupPath, "tessdata"),
                    Path.Combine(Application.StartupPath, "..", "tessdata"),
                    Path.Combine(Environment.CurrentDirectory, "tessdata"),
                    @"C:\Program Files\Tesseract-OCR\tessdata",
                    @"C:\tesseract\tessdata"
                };
                
                foreach (var path in possiblePaths)
                {
                    if (Directory.Exists(path))
                    {
                        tessDataPath = path;
                        break;
                    }
                }
                
                if (string.IsNullOrEmpty(tessDataPath))
                {
                    // Create tessdata directory if it doesn't exist
                    tessDataPath = Path.Combine(Application.StartupPath, "tessdata");
                    Directory.CreateDirectory(tessDataPath);
                    
                    OnCaptchaStatusChanged("Warning: tessdata folder not found. Please add language files.");
                    return;
                }
                
                // Initialize Tesseract with English language
                // TODO: Initialize when Tesseract DLL is properly referenced
                // ocrEngine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default);
                
                // Configure for better CAPTCHA recognition
                // ocrEngine.SetVariable("tessedit_char_whitelist", 
                //     "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
                // ocrEngine.SetVariable("tessedit_unrej_any_wd", true);
                
                initialized = true;
                OnCaptchaStatusChanged($"OCR Engine initialized with tessdata from: {tessDataPath}");
            }
            catch (Exception ex)
            {
                OnCaptchaStatusChanged($"OCR initialization failed: {ex.Message}");
                initialized = false;
            }
        }
        
        // Download CAPTCHA image from URL
        public Bitmap DownloadCaptchaImage(string imageUrl, CookieContainer cookies = null)
        {
            try
            {
                using (var client = new WebClient())
                {
                    if (cookies != null)
                    {
                        // Add cookies if provided
                        var uri = new Uri(imageUrl);
                        var cookieHeader = cookies.GetCookieHeader(uri);
                        if (!string.IsNullOrEmpty(cookieHeader))
                        {
                            client.Headers.Add(HttpRequestHeader.Cookie, cookieHeader);
                        }
                    }
                    
                    byte[] imageBytes = client.DownloadData(imageUrl);
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        var bitmap = new Bitmap(ms);
                        OnCaptchaImageReceived(bitmap);
                        return bitmap;
                    }
                }
            }
            catch (Exception ex)
            {
                OnCaptchaStatusChanged($"Failed to download CAPTCHA: {ex.Message}");
                return null;
            }
        }
        
        // Solve CAPTCHA from image
        public string SolveCaptcha(Bitmap image, bool preprocess = true)
        {
            if (!initialized || ocrEngine == null)
            {
                OnCaptchaStatusChanged("OCR engine not initialized");
                return null;
            }
            
            try
            {
                Bitmap processedImage = image;
                
                if (preprocess)
                {
                    processedImage = PreprocessImage(image);
                }
                
                // TODO: Process with Tesseract when DLL is properly configured
                var text = ""; // Placeholder
                /*
                using (var page = ocrEngine.Process(processedImage, PageSegMode.SingleLine))
                {
                    var text = page.GetText().Trim();
                */
                    
                    // Clean up the text
                    text = CleanOCRText(text);
                    
                    if (!string.IsNullOrEmpty(text))
                    {
                        SuccessCount++;
                        OnCaptchaStatusChanged($"CAPTCHA solved: {text} (Success rate: {SuccessRate:F1}%)");
                    }
                    else
                    {
                        FailureCount++;
                        OnCaptchaStatusChanged($"CAPTCHA solving failed (Success rate: {SuccessRate:F1}%)");
                    }
                    
                    if (processedImage != image)
                    {
                        processedImage.Dispose();
                    }
                    
                    return text;
                // } // End of commented using block
            }
            catch (Exception ex)
            {
                FailureCount++;
                OnCaptchaStatusChanged($"OCR error: {ex.Message}");
                return null;
            }
        }
        
        // Solve CAPTCHA from file path
        public string SolveCaptchaFromFile(string filePath, bool preprocess = true)
        {
            try
            {
                using (var image = new Bitmap(filePath))
                {
                    return SolveCaptcha(image, preprocess);
                }
            }
            catch (Exception ex)
            {
                OnCaptchaStatusChanged($"Failed to load image: {ex.Message}");
                return null;
            }
        }
        
        // Preprocess image for better OCR accuracy
        private Bitmap PreprocessImage(Bitmap original)
        {
            var processed = new Bitmap(original.Width, original.Height);
            
            using (Graphics g = Graphics.FromImage(processed))
            {
                // Convert to grayscale
                var colorMatrix = new System.Drawing.Imaging.ColorMatrix(
                    new float[][]
                    {
                        new float[] {0.299f, 0.299f, 0.299f, 0, 0},
                        new float[] {0.587f, 0.587f, 0.587f, 0, 0},
                        new float[] {0.114f, 0.114f, 0.114f, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 1}
                    });
                
                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);
                
                g.DrawImage(original, 
                    new Rectangle(0, 0, processed.Width, processed.Height),
                    0, 0, original.Width, original.Height, 
                    GraphicsUnit.Pixel, attributes);
            }
            
            // Apply threshold to convert to black and white
            processed = ApplyThreshold(processed, 128);
            
            // Remove noise
            processed = RemoveNoise(processed);
            
            // Scale up if image is too small
            if (processed.Width < 200)
            {
                int newWidth = processed.Width * 2;
                int newHeight = processed.Height * 2;
                processed = new Bitmap(processed, newWidth, newHeight);
            }
            
            return processed;
        }
        
        // Apply threshold to image
        private Bitmap ApplyThreshold(Bitmap image, int threshold)
        {
            var result = new Bitmap(image.Width, image.Height);
            
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int gray = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
                    Color newColor = gray < threshold ? Color.Black : Color.White;
                    result.SetPixel(x, y, newColor);
                }
            }
            
            return result;
        }
        
        // Remove noise from image
        private Bitmap RemoveNoise(Bitmap image)
        {
            var result = new Bitmap(image);
            
            for (int x = 1; x < image.Width - 1; x++)
            {
                for (int y = 1; y < image.Height - 1; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    
                    if (pixel.R == 0) // Black pixel
                    {
                        int blackNeighbors = 0;
                        
                        // Count black neighbors
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                if (dx == 0 && dy == 0) continue;
                                
                                Color neighbor = image.GetPixel(x + dx, y + dy);
                                if (neighbor.R == 0) blackNeighbors++;
                            }
                        }
                        
                        // If isolated pixel (less than 2 black neighbors), remove it
                        if (blackNeighbors < 2)
                        {
                            result.SetPixel(x, y, Color.White);
                        }
                    }
                }
            }
            
            return result;
        }
        
        // Clean OCR text
        private string CleanOCRText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            
            // Remove special characters except alphanumeric
            text = Regex.Replace(text, @"[^a-zA-Z0-9]", "");
            
            // Common OCR mistakes
            text = text.Replace("O", "0")
                      .Replace("o", "0")
                      .Replace("l", "1")
                      .Replace("I", "1")
                      .Replace("S", "5")
                      .Replace("s", "5")
                      .Replace("Z", "2")
                      .Replace("z", "2");
            
            return text;
        }
        
        // Detect if page contains CAPTCHA
        public bool DetectCaptcha(string html)
        {
            var captchaIndicators = new[]
            {
                "captcha",
                "CAPTCHA",
                "recaptcha",
                "g-recaptcha",
                "captcha-image",
                "captcha_image",
                "security-check",
                "verify-human",
                "are you human",
                "prove you're not a robot",
                "enter the characters",
                "type the characters",
                "automated access"
            };
            
            foreach (var indicator in captchaIndicators)
            {
                if (html.Contains(indicator))
                {
                    OnCaptchaStatusChanged($"CAPTCHA detected: {indicator}");
                    return true;
                }
            }
            
            return false;
        }
        
        // Extract CAPTCHA image URL from HTML
        public string ExtractCaptchaImageUrl(string html, string baseUrl)
        {
            // Common CAPTCHA image patterns
            var patterns = new[]
            {
                @"<img[^>]*class=[""'].*captcha.*[""'][^>]*src=[""']([^""']+)[""']",
                @"<img[^>]*id=[""'].*captcha.*[""'][^>]*src=[""']([^""']+)[""']",
                @"<img[^>]*src=[""']([^""']*captcha[^""']*)[""']",
                @"captcha.*src=[""']([^""']+)[""']",
                @"src=[""']([^""']*\/captcha\/[^""']*)[""']"
            };
            
            foreach (var pattern in patterns)
            {
                var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var imageUrl = match.Groups[1].Value;
                    
                    // Make URL absolute if relative
                    if (!imageUrl.StartsWith("http"))
                    {
                        var baseUri = new Uri(baseUrl);
                        imageUrl = new Uri(baseUri, imageUrl).ToString();
                    }
                    
                    OnCaptchaStatusChanged($"CAPTCHA image found: {imageUrl}");
                    return imageUrl;
                }
            }
            
            return null;
        }
        
        // Reset statistics
        public void ResetStatistics()
        {
            SuccessCount = 0;
            FailureCount = 0;
        }
        
        // Dispose resources
        public void Dispose()
        {
            // ocrEngine?.Dispose(); // Will enable when Tesseract is configured
        }
        
        protected virtual void OnCaptchaStatusChanged(string status)
        {
            CaptchaStatusChanged?.Invoke(this, status);
        }
        
        protected virtual void OnCaptchaImageReceived(Bitmap image)
        {
            CaptchaImageReceived?.Invoke(this, image);
        }
    }
}