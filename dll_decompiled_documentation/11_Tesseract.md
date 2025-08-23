# Tesseract.dll Documentation

## Overview
Tesseract.dll is a .NET wrapper for the Tesseract OCR (Optical Character Recognition) engine, enabling text extraction from images. It's one of the most accurate open-source OCR engines available, originally developed by HP and now maintained by Google.

## Purpose in OpenBullet
- Extract text from CAPTCHA images
- Read text from screenshots
- Process image-based challenges
- Extract data from image responses
- Solve simple text-based CAPTCHAs
- Read verification codes from images

## Key Components

### Core OCR Engine

#### `TesseractEngine`
- **Purpose**: Main OCR processing engine
- **Key Properties**:
  - `DefaultPageSegMode` - Page segmentation mode
  - `Version` - Tesseract version info
- **Key Methods**:
  - `Process()` - Process image to text
  - `SetVariable()` - Configure OCR parameters
  - `GetVariableAsString()` - Get configuration value
- **Initialization**:
```csharp
// Initialize with language data
var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);

// Multiple languages
var engine = new TesseractEngine(@"./tessdata", "eng+fra+deu", EngineMode.Default);

// With specific mode
var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.LstmOnly);
```

#### `Page`
- **Purpose**: Represents processed page with OCR results
- **Key Methods**:
  - `GetText()` - Extract all text
  - `GetHOCRText()` - Get HTML OCR format
  - `GetMeanConfidence()` - Overall confidence score
  - `GetIterator()` - Iterate through results
  - `AnalyzeLayout()` - Get page layout
- **Properties**:
  - `Text` - Extracted text
  - `Confidence` - Recognition confidence

### Image Processing

#### `Pix`
- **Purpose**: Image representation for Tesseract
- **Creation Methods**:
```csharp
// From file
var image = Pix.LoadFromFile("image.png");

// From memory
var image = Pix.LoadFromMemory(imageBytes);

// From stream
using (var stream = new FileStream("image.jpg", FileMode.Open))
{
    var image = Pix.LoadFromStream(stream);
}

// From bitmap
var bitmap = new Bitmap("image.bmp");
var image = PixConverter.ToPix(bitmap);
```

#### `PixConverter`
- **Purpose**: Convert between image formats
- **Methods**:
  - `ToPix()` - Convert Bitmap to Pix
  - `ToBitmap()` - Convert Pix to Bitmap

## Implementation Examples

### Basic Text Extraction
```csharp
public class SimpleOCR
{
    private TesseractEngine engine;
    
    public SimpleOCR()
    {
        engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
    }
    
    public string ExtractText(string imagePath)
    {
        using (var image = Pix.LoadFromFile(imagePath))
        using (var page = engine.Process(image))
        {
            var text = page.GetText();
            var confidence = page.GetMeanConfidence();
            
            Console.WriteLine($"Confidence: {confidence * 100}%");
            return text.Trim();
        }
    }
    
    public string ExtractTextFromBytes(byte[] imageData)
    {
        using (var image = Pix.LoadFromMemory(imageData))
        using (var page = engine.Process(image))
        {
            return page.GetText().Trim();
        }
    }
    
    public void Dispose()
    {
        engine?.Dispose();
    }
}
```

### CAPTCHA Solving
```csharp
public class CaptchaOCR
{
    private TesseractEngine engine;
    
    public CaptchaOCR()
    {
        engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
        
        // Configure for CAPTCHA recognition
        engine.SetVariable("tessedit_char_whitelist", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        engine.SetVariable("tessedit_char_blacklist", "!@#$%^&*()_+-={}[]|\\:\";<>?,./");
        engine.DefaultPageSegMode = PageSegMode.SingleLine;
    }
    
    public string SolveCaptcha(byte[] captchaImage)
    {
        // Preprocess image
        var processed = PreprocessCaptchaImage(captchaImage);
        
        using (var image = Pix.LoadFromMemory(processed))
        using (var page = engine.Process(image, PageSegMode.SingleWord))
        {
            var text = page.GetText().Trim();
            
            // Clean up result
            text = text.Replace(" ", "")
                      .Replace("\n", "")
                      .ToUpper();
            
            // Validate result
            if (page.GetMeanConfidence() < 0.6)
            {
                throw new Exception("Low confidence OCR result");
            }
            
            return text;
        }
    }
    
    private byte[] PreprocessCaptchaImage(byte[] imageData)
    {
        using (var ms = new MemoryStream(imageData))
        using (var bitmap = new Bitmap(ms))
        {
            // Convert to grayscale
            var grayscale = ConvertToGrayscale(bitmap);
            
            // Apply threshold
            var threshold = ApplyThreshold(grayscale, 128);
            
            // Remove noise
            var denoised = RemoveNoise(threshold);
            
            // Scale up for better recognition
            var scaled = ScaleImage(denoised, 2.0);
            
            // Convert back to bytes
            using (var output = new MemoryStream())
            {
                scaled.Save(output, ImageFormat.Png);
                return output.ToArray();
            }
        }
    }
}
```

### Advanced OCR Configuration
```csharp
public class AdvancedOCR
{
    private TesseractEngine engine;
    
    public enum OCRMode
    {
        Fast,
        Accurate,
        Numeric,
        Alphanumeric,
        Custom
    }
    
    public void ConfigureEngine(OCRMode mode)
    {
        switch (mode)
        {
            case OCRMode.Fast:
                engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.TesseractOnly);
                engine.SetVariable("tessedit_ocr_engine_mode", "0");
                break;
                
            case OCRMode.Accurate:
                engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.LstmOnly);
                engine.SetVariable("tessedit_ocr_engine_mode", "1");
                break;
                
            case OCRMode.Numeric:
                engine.SetVariable("tessedit_char_whitelist", "0123456789");
                engine.DefaultPageSegMode = PageSegMode.SingleWord;
                break;
                
            case OCRMode.Alphanumeric:
                engine.SetVariable("tessedit_char_whitelist", 
                    "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
                break;
        }
    }
    
    public List<TextRegion> ExtractRegions(string imagePath)
    {
        var regions = new List<TextRegion>();
        
        using (var image = Pix.LoadFromFile(imagePath))
        using (var page = engine.Process(image))
        {
            using (var iterator = page.GetIterator())
            {
                iterator.Begin();
                
                do
                {
                    var text = iterator.GetText(PageIteratorLevel.Word);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        var bounds = iterator.BoundingBox(PageIteratorLevel.Word);
                        var confidence = iterator.GetConfidence(PageIteratorLevel.Word);
                        
                        regions.Add(new TextRegion
                        {
                            Text = text,
                            X = bounds.X1,
                            Y = bounds.Y1,
                            Width = bounds.Width,
                            Height = bounds.Height,
                            Confidence = confidence
                        });
                    }
                } while (iterator.Next(PageIteratorLevel.Word));
            }
        }
        
        return regions;
    }
}
```

### Image Preprocessing
```csharp
public class ImagePreprocessor
{
    public Pix PrepareForOCR(Pix input)
    {
        // Convert to grayscale if needed
        var grayscale = input.Depth == 1 ? input : input.ConvertRGBToGray();
        
        // Scale image for better recognition
        var scaled = grayscale.Scale(2.0f, 2.0f);
        
        // Apply adaptive threshold
        var binary = scaled.BinaryThreshold(128);
        
        // Deskew if needed
        var deskewed = Deskew(binary);
        
        // Remove border noise
        var cleaned = RemoveBorder(deskewed);
        
        return cleaned;
    }
    
    private Pix Deskew(Pix image)
    {
        // Detect skew angle
        float angle;
        var deskewed = image.Deskew(out angle);
        
        if (Math.Abs(angle) > 0.01)
        {
            Console.WriteLine($"Deskewed by {angle} degrees");
            return deskewed;
        }
        
        return image;
    }
    
    private Pix RemoveBorder(Pix image)
    {
        // Remove black border
        var noBorder = image.RemoveBorder(10);
        return noBorder;
    }
    
    public Pix EnhanceContrast(Pix image)
    {
        // Enhance contrast for better recognition
        var enhanced = image.UnsharpMasking(5, 1.5f);
        return enhanced;
    }
}
```

## Integration with OpenBullet

### OCR Block Implementation
```csharp
public class OCRBlock : BlockBase
{
    public string ImageSource { get; set; } // File path or variable name
    public string OutputVariable { get; set; }
    public string Language { get; set; } = "eng";
    public bool Preprocess { get; set; } = true;
    
    public override BlockResult Process(BotData data)
    {
        try
        {
            byte[] imageData = GetImageData(data);
            
            if (Preprocess)
            {
                imageData = PreprocessImage(imageData);
            }
            
            using (var engine = new TesseractEngine(@"./tessdata", Language, EngineMode.Default))
            using (var image = Pix.LoadFromMemory(imageData))
            using (var page = engine.Process(image))
            {
                var text = page.GetText().Trim();
                var confidence = page.GetMeanConfidence();
                
                data.Variables[OutputVariable] = text;
                data.Variables[$"{OutputVariable}_CONFIDENCE"] = confidence.ToString();
                
                data.Log.Add($"OCR Result: {text} (Confidence: {confidence:P})");
                
                return confidence > 0.5 ? BlockResult.Continue : BlockResult.Retry;
            }
        }
        catch (Exception ex)
        {
            data.Log.Add($"OCR Error: {ex.Message}");
            return BlockResult.Error;
        }
    }
}
```

### CAPTCHA Integration
```csharp
public class TesseractCaptchaSolver : ICaptchaSolver
{
    private readonly TesseractEngine engine;
    
    public TesseractCaptchaSolver()
    {
        engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
        ConfigureForCaptcha();
    }
    
    private void ConfigureForCaptcha()
    {
        // Optimize for CAPTCHA text
        engine.SetVariable("tessedit_char_whitelist", 
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
        engine.SetVariable("tessedit_unrej_any_wd", "1");
        engine.SetVariable("edges_max_children_per_outline", "40");
        engine.DefaultPageSegMode = PageSegMode.SingleLine;
    }
    
    public string SolveImageCaptcha(byte[] imageData)
    {
        using (var image = Pix.LoadFromMemory(imageData))
        using (var page = engine.Process(image))
        {
            return page.GetText()
                .Trim()
                .Replace(" ", "")
                .Replace("\n", "");
        }
    }
}
```

## Performance Optimization

### Batch Processing
```csharp
public class BatchOCR
{
    private readonly TesseractEngine engine;
    private readonly object lockObj = new object();
    
    public BatchOCR()
    {
        engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
    }
    
    public Dictionary<string, string> ProcessBatch(List<string> imagePaths)
    {
        var results = new ConcurrentDictionary<string, string>();
        
        Parallel.ForEach(imagePaths, new ParallelOptions { MaxDegreeOfParallelism = 4 }, 
            imagePath =>
        {
            lock (lockObj) // Tesseract engine is not thread-safe
            {
                using (var image = Pix.LoadFromFile(imagePath))
                using (var page = engine.Process(image))
                {
                    results[imagePath] = page.GetText().Trim();
                }
            }
        });
        
        return results.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}
```

## Best Practices
1. Always dispose engine and page objects
2. Preprocess images for better accuracy
3. Use appropriate page segmentation mode
4. Configure character whitelist for specific use cases
5. Check confidence scores
6. Handle low-quality images gracefully
7. Use appropriate language data files

## Language Support
- Download additional language data from Tesseract repository
- Common languages: eng, fra, deu, spa, ita, rus, chi_sim, jpn
- Custom training data for specific fonts/styles

## Limitations
- Requires tessdata files for language support
- Not thread-safe (single engine instance)
- Performance depends on image quality
- Limited accuracy on distorted text
- Struggles with complex backgrounds

## Dependencies
- tessdata folder with language files
- Visual C++ Redistributables
- .NET Framework 4.5+
- Leptonica (included)