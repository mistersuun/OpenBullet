// Decompiled with JetBrains decompiler
// Type: RuriLib.BlockOCR
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Extreme.Net;
using RuriLib.LS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Media;
using Tesseract;

#nullable disable
namespace RuriLib;

public class BlockOCR : BlockBase
{
  private string variableName = "";
  private bool isCapture;
  private string url = "";
  private string userAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
  private string ocrString = "";
  private bool isBase64;
  private float contrast = 1f;
  private float gamma = 1f;
  private float brightness = 1f;
  private int linesMin;
  private int linesMax;
  private bool conGamBri;
  private bool saturate;
  public int saturation;
  private bool grayScale;
  private bool removeLines;
  private bool removeNoise;
  private bool dilate;
  private bool transparent;
  private bool onlyShow;
  public int showDiff;
  public int transDiff;
  private float threshold = 1f;
  private string ocrLang = "eng";

  public string VariableName
  {
    get => this.variableName;
    set
    {
      this.variableName = value;
      this.OnPropertyChanged(nameof (VariableName));
    }
  }

  public bool IsCapture
  {
    get => this.isCapture;
    set
    {
      this.isCapture = value;
      this.OnPropertyChanged(nameof (IsCapture));
    }
  }

  public string Url
  {
    get => this.url;
    set
    {
      this.url = value;
      this.OnPropertyChanged(nameof (Url));
    }
  }

  public string UserAgent
  {
    get => this.userAgent;
    set
    {
      this.userAgent = value;
      this.OnPropertyChanged(nameof (UserAgent));
    }
  }

  public Dictionary<string, string> CustomHeaders { get; set; } = new Dictionary<string, string>()
  {
    {
      "User-Agent",
      "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko"
    },
    {
      "Pragma",
      "no-cache"
    },
    {
      "Accept",
      "*/*"
    }
  };

  public string OCRString
  {
    get => this.ocrString;
    set
    {
      this.ocrString = value;
      this.OnPropertyChanged(nameof (OCRString));
    }
  }

  public bool IsBase64
  {
    get => this.isBase64;
    set
    {
      this.isBase64 = value;
      this.OnPropertyChanged(nameof (IsBase64));
    }
  }

  public float Contrast
  {
    get => this.contrast;
    set
    {
      this.contrast = value;
      this.OnPropertyChanged(nameof (Contrast));
    }
  }

  public float Gamma
  {
    get => this.gamma;
    set
    {
      this.gamma = value;
      this.OnPropertyChanged(nameof (Gamma));
    }
  }

  public float Brightness
  {
    get => this.brightness;
    set
    {
      this.brightness = value;
      this.OnPropertyChanged(nameof (Brightness));
    }
  }

  public int LinesMin
  {
    get => this.linesMin;
    set
    {
      this.linesMin = value;
      this.OnPropertyChanged(nameof (LinesMin));
    }
  }

  public int LinesMax
  {
    get => this.linesMax;
    set
    {
      this.linesMax = value;
      this.OnPropertyChanged(nameof (LinesMax));
    }
  }

  public bool ConGamBri
  {
    get => this.conGamBri;
    set
    {
      this.conGamBri = value;
      this.OnPropertyChanged(nameof (ConGamBri));
    }
  }

  public bool Saturate
  {
    get => this.saturate;
    set
    {
      this.saturate = value;
      this.OnPropertyChanged(nameof (Saturate));
    }
  }

  public int Saturation
  {
    get => this.saturation;
    set
    {
      this.saturation = value;
      this.OnPropertyChanged(nameof (Saturation));
    }
  }

  public bool GrayScale
  {
    get => this.grayScale;
    set
    {
      this.grayScale = value;
      this.OnPropertyChanged(nameof (GrayScale));
    }
  }

  public bool RemoveLines
  {
    get => this.removeLines;
    set
    {
      this.removeLines = value;
      this.OnPropertyChanged(nameof (RemoveLines));
    }
  }

  public bool RemoveNoise
  {
    get => this.removeNoise;
    set
    {
      this.removeNoise = value;
      this.OnPropertyChanged(nameof (RemoveNoise));
    }
  }

  public bool Dilate
  {
    get => this.dilate;
    set
    {
      this.dilate = value;
      this.OnPropertyChanged(nameof (Dilate));
    }
  }

  public bool Transparent
  {
    get => this.transparent;
    set
    {
      this.transparent = value;
      this.OnPropertyChanged(nameof (Transparent));
    }
  }

  public int ShowDiff
  {
    get => this.showDiff;
    set
    {
      this.showDiff = value;
      this.OnPropertyChanged(nameof (ShowDiff));
    }
  }

  public int TransDiff
  {
    get => this.transDiff;
    set
    {
      this.transDiff = value;
      this.OnPropertyChanged(nameof (TransDiff));
    }
  }

  public bool OnlyShow
  {
    get => this.onlyShow;
    set
    {
      this.onlyShow = value;
      this.OnPropertyChanged(nameof (OnlyShow));
    }
  }

  public float Threshold
  {
    get => this.threshold;
    set
    {
      this.threshold = value;
      this.OnPropertyChanged(nameof (Threshold));
    }
  }

  public string OcrLang
  {
    get => this.ocrLang;
    set
    {
      this.ocrLang = value;
      this.OnPropertyChanged(nameof (OcrLang));
    }
  }

  public BlockOCR() => this.Label = "OCR";

  public override void Process(BotData data)
  {
    base.Process(data);
    BlockBase.InsertVariables(data, this.IsCapture, false, this.GetOCR(data), this.VariableName, "", "", false, true);
  }

  public List<string> GetOCR(BotData data)
  {
    return new List<string>()
    {
      new TesseractEngine(".\\tessdata", this.OcrLang, EngineMode.Default).Process(this.GetOCRImage(data)).GetText()
    };
  }

  public static Bitmap CreateNonIndexedImage(Bitmap src)
  {
    Bitmap nonIndexedImage = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
    using (Graphics graphics = Graphics.FromImage((Image) nonIndexedImage))
    {
      graphics.PixelOffsetMode = PixelOffsetMode.None;
      graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
      graphics.DrawImage((Image) src, new Rectangle(0, 0, src.Width, src.Height));
    }
    return nonIndexedImage;
  }

  public Pix GetOCRImage(BotData data)
  {
    string str1 = BlockBase.ReplaceValues(this.Url, data);
    HttpContent content = (HttpContent) null;
    Pix pix;
    if (this.IsBase64)
    {
      byte[] buffer = Convert.FromBase64String(str1);
      using (MemoryStream memoryStream = new MemoryStream(buffer, 0, buffer.Length))
      {
        Bitmap bitmap = BlockOCR.CreateNonIndexedImage((Bitmap) Image.FromStream((Stream) memoryStream));
        if (this.ConGamBri)
          bitmap = this.SetContrastGamma(bitmap);
        if (this.Saturate)
          bitmap = this.SetSaturation(bitmap);
        if (this.RemoveNoise)
          bitmap = this.RemoveNoiseThreshold(bitmap, this.Threshold);
        if (this.Transparent)
          bitmap = this.SetTransparent(bitmap);
        if (this.OnlyShow)
          bitmap = this.SetOnlyShow(bitmap);
        if (this.GrayScale)
          bitmap = this.ToGrayScale(bitmap);
        if (this.RemoveLines)
          bitmap = this.RemoveImageLines(bitmap);
        if (this.Dilate)
          bitmap = this.DilateImage(bitmap);
        pix = PixConverter.ToPix(bitmap);
      }
    }
    else
    {
      WebRequest.Create(str1);
      HttpRequest httpRequest = new HttpRequest();
      httpRequest.Cookies = new CookieDictionary();
      data.Log(new LogEntry("Calling Image URL: " + str1, Colors.MediumTurquoise));
      data.Log(new LogEntry("Sent Cookies:", Colors.MediumTurquoise));
      foreach (KeyValuePair<string, string> cookie in data.Cookies)
      {
        httpRequest.Cookies.Add(cookie.Key, cookie.Value);
        data.Log(new LogEntry($"{cookie.Key} : {cookie.Value}", Colors.MediumTurquoise));
      }
      data.Log(new LogEntry("Sent Headers:", Colors.DarkTurquoise));
      foreach (KeyValuePair<string, string> customHeader in this.CustomHeaders)
      {
        try
        {
          string name = BlockBase.ReplaceValues(customHeader.Key, data);
          string lower = name.Replace("-", "").ToLower();
          string str2 = BlockBase.ReplaceValues(customHeader.Value, data);
          if (lower == "contenttype")
          {
            if (content != null)
              continue;
          }
          httpRequest.AddHeader(name, str2);
          data.Log(new LogEntry($"{name}: {str2}", Colors.MediumTurquoise));
        }
        catch
        {
        }
      }
      HttpResponse httpResponse = httpRequest.Raw(HttpMethod.GET, str1, content);
      data.Log(new LogEntry("Received cookies:", Colors.Goldenrod));
      data.Cookies = (Dictionary<string, string>) httpResponse.Cookies;
      foreach (KeyValuePair<string, string> cookie in (Dictionary<string, string>) httpResponse.Cookies)
        data.Log(new LogEntry($"{cookie.Key}: {cookie.Value}", Colors.LightGoldenrodYellow));
      Bitmap bitmap = BlockOCR.CreateNonIndexedImage((Bitmap) Image.FromStream((Stream) httpResponse.ToMemoryStream()));
      if (this.ConGamBri)
        bitmap = this.SetContrastGamma(bitmap);
      if (this.Saturate)
        bitmap = this.SetSaturation(bitmap);
      if (this.RemoveNoise)
        bitmap = this.RemoveNoiseThreshold(bitmap, this.Threshold);
      if (this.Transparent)
        bitmap = this.SetTransparent(bitmap);
      if (this.OnlyShow)
        bitmap = this.SetOnlyShow(bitmap);
      if (this.GrayScale)
        bitmap = this.ToGrayScale(bitmap);
      if (this.RemoveLines)
        bitmap = this.RemoveImageLines(bitmap);
      if (this.Dilate)
        bitmap = this.DilateImage(bitmap);
      pix = PixConverter.ToPix(bitmap);
    }
    return pix;
  }

  public string GetCustomHeaders()
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (KeyValuePair<string, string> customHeader in this.CustomHeaders)
    {
      stringBuilder.Append($"{customHeader.Key}: {customHeader.Value}");
      if (!customHeader.Equals((object) this.CustomHeaders.Last<KeyValuePair<string, string>>()))
        stringBuilder.Append(Environment.NewLine);
    }
    return stringBuilder.ToString();
  }

  public void SetCustomHeaders(string[] lines)
  {
    this.CustomHeaders.Clear();
    foreach (string line in lines)
    {
      if (line.Contains<char>(':'))
      {
        string[] strArray = line.Split(new char[1]{ ':' }, 2);
        this.CustomHeaders[strArray[0].Trim()] = strArray[1].Trim();
      }
    }
  }

  public Bitmap SetOnlyShow(Bitmap appliedCaptcha)
  {
    System.Drawing.Color color = System.Drawing.Color.FromArgb(0, 0, 0);
    for (int x = 0; x < appliedCaptcha.Width; ++x)
    {
      for (int y = 0; y < appliedCaptcha.Height; ++y)
      {
        if (appliedCaptcha.GetPixel(x, y) != color)
          appliedCaptcha.SetPixel(x, y, System.Drawing.Color.Transparent);
      }
    }
    return appliedCaptcha;
  }

  public Bitmap SetTransparent(Bitmap appliedCaptcha) => appliedCaptcha;

  public override string ToLS(bool indent = true)
  {
    BlockWriter blockWriter = new BlockWriter(this.GetType(), indent, this.Disabled);
    blockWriter.Label(this.Label).Token((object) "OCR").Literal(this.Url);
    if (!blockWriter.CheckDefault((object) this.VariableName, "VariableName"))
      blockWriter.Arrow().Token(this.IsCapture ? (object) "CAP" : (object) "VAR").Literal(this.VariableName).Token((object) this.OcrLang).Indent();
    blockWriter.Boolean(this.IsBase64, "IsBase64").Boolean(this.ConGamBri, "ConGamBri").Boolean(this.Saturate, "Saturate").Boolean(this.RemoveNoise, "RemoveNoise").Boolean(this.Transparent, "Transparent").Boolean(this.OnlyShow, "OnlyShow").Boolean(this.GrayScale, "GrayScale").Boolean(this.RemoveLines, "RemoveLines").Boolean(this.Dilate, "Dilate").Indent();
    foreach (KeyValuePair<string, string> customHeader in this.CustomHeaders)
      blockWriter.Token((object) "HEADER").Literal($"{customHeader.Key}: {customHeader.Value}").Indent();
    if (this.ConGamBri)
      blockWriter.Token((object) "CONTRAST").Float(this.Contrast).Token((object) "GAMMA").Float(this.Gamma).Token((object) "BRIGHTNESS").Float(this.Brightness).Indent();
    if (this.Saturate)
      blockWriter.Token((object) "SATURATION").Integer(this.Saturation).Indent();
    if (this.RemoveNoise)
      blockWriter.Token((object) "THRESHOLD").Float(this.Threshold).Indent();
    if (this.Transparent)
      blockWriter.Token((object) "TRANSDIFF").Integer(this.TransDiff).Indent();
    if (this.OnlyShow)
      blockWriter.Token((object) "SHOWDIFF").Integer(this.ShowDiff).Indent();
    if (this.RemoveLines)
      blockWriter.Token((object) "LINESMIN").Integer(this.LinesMin).Token((object) "LINESMAX").Integer(this.LinesMax).Return();
    return blockWriter.ToString();
  }

  public override BlockBase FromLS(string line)
  {
    string input = line.Trim();
    if (input.StartsWith("#"))
      this.Label = LineParser.ParseLabel(ref input);
    this.Url = LineParser.ParseLiteral(ref input, "Url");
    if (LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Arrow, false) == "")
      return (BlockBase) this;
    try
    {
      string token = LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Parameter, true);
      if (!(token.ToUpper() == "VAR"))
      {
        if (!(token.ToUpper() == "CAP"))
          goto label_8;
      }
      this.IsCapture = token.ToUpper() == "CAP";
    }
    catch
    {
      throw new ArgumentException("Invalid or missing variable type");
    }
label_8:
    try
    {
      this.VariableName = LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Literal, true);
    }
    catch
    {
      throw new ArgumentException("Variable name not specified");
    }
    this.OcrLang = LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Parameter, false);
    while (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
      LineParser.SetBool(ref input, (object) this);
    while (input != "" && LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Parameter)
    {
      switch (LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Parameter, true).ToUpper())
      {
        case "BRIGHTNESS":
          this.Brightness = LineParser.ParseFloat(ref input, "Brightness");
          continue;
        case "CONTRAST":
          this.Contrast = LineParser.ParseFloat(ref input, "Contrast");
          continue;
        case "GAMMA":
          this.Gamma = LineParser.ParseFloat(ref input, "Gamma");
          continue;
        case "HEADER":
          KeyValuePair<string, string> pair = BlockOCR.ParsePair(LineParser.ParseLiteral(ref input, "HEADER VALUE"));
          this.CustomHeaders[pair.Key] = pair.Value;
          continue;
        case "LINESMAX":
          this.LinesMax = LineParser.ParseInt(ref input, "LinesMax");
          continue;
        case "LINESMIN":
          this.LinesMin = LineParser.ParseInt(ref input, "LinesMin");
          continue;
        case "SATURATION":
          this.Saturation = LineParser.ParseInt(ref input, "Saturation");
          continue;
        case "SHOWDIFF":
          this.ShowDiff = LineParser.ParseInt(ref input, "ShowDiff");
          continue;
        case "THRESHOLD":
          this.Threshold = LineParser.ParseFloat(ref input, "Threshold");
          continue;
        case "TRANSDIFF":
          this.TransDiff = LineParser.ParseInt(ref input, "TransDiff");
          continue;
        default:
          continue;
      }
    }
    return (BlockBase) this;
  }

  public static KeyValuePair<string, string> ParsePair(string pair)
  {
    string[] strArray = pair.Split(new char[1]{ ':' }, 2);
    return new KeyValuePair<string, string>(strArray[0].Trim(), strArray[1].Trim());
  }

  public Bitmap SetContrastGamma(Bitmap original)
  {
    Bitmap bitmap = original;
    float num = this.brightness - 1f;
    float[][] newColorMatrix = new float[5][]
    {
      new float[5]{ this.Contrast, 0.0f, 0.0f, 0.0f, 0.0f },
      new float[5]{ 0.0f, this.Contrast, 0.0f, 0.0f, 0.0f },
      new float[5]{ 0.0f, 0.0f, this.Contrast, 0.0f, 0.0f },
      new float[5]{ 0.0f, 0.0f, 0.0f, 1f, 0.0f },
      new float[5]{ num, num, num, 0.0f, 1f }
    };
    ImageAttributes imageAttr = new ImageAttributes();
    imageAttr.ClearColorMatrix();
    imageAttr.SetColorMatrix(new ColorMatrix(newColorMatrix), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
    imageAttr.SetGamma(this.Gamma, ColorAdjustType.Bitmap);
    Graphics.FromImage((Image) bitmap).DrawImage((Image) original, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, imageAttr);
    return bitmap;
  }

  public Bitmap SetSaturation(Bitmap original)
  {
    int saturation = this.Saturation;
    for (int x = 0; x < original.Width; ++x)
    {
      for (int y = 0; y < original.Height; ++y)
      {
        System.Drawing.Color pixel = original.GetPixel(x, y);
        int red;
        if ((int) pixel.R + saturation <= (int) byte.MaxValue)
        {
          pixel = original.GetPixel(x, y);
          red = (int) pixel.R + saturation;
        }
        else
          red = (int) byte.MaxValue;
        pixel = original.GetPixel(x, y);
        int num1;
        if ((int) pixel.G + saturation <= (int) byte.MaxValue)
        {
          pixel = original.GetPixel(x, y);
          num1 = (int) pixel.G + saturation;
        }
        else
          num1 = (int) byte.MaxValue;
        int num2 = num1;
        pixel = original.GetPixel(x, y);
        int num3;
        if ((int) pixel.B + saturation <= (int) byte.MaxValue)
        {
          pixel = original.GetPixel(x, y);
          num3 = (int) pixel.B + saturation;
        }
        else
          num3 = (int) byte.MaxValue;
        int num4 = num3;
        int green = num2;
        int blue = num4;
        System.Drawing.Color color = System.Drawing.Color.FromArgb(red, green, blue);
        original.SetPixel(x, y, color);
      }
    }
    return original;
  }

  public Bitmap ToGrayScale(Bitmap Bmp)
  {
    for (int y = 0; y < Bmp.Height; ++y)
    {
      for (int x = 0; x < Bmp.Width; ++x)
      {
        System.Drawing.Color pixel = Bmp.GetPixel(x, y);
        int num = (int) Math.Round(0.299 * (double) pixel.R + 0.587 * (double) pixel.G + 0.114 * (double) pixel.B);
        Bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(num, num, num));
      }
    }
    return Bmp;
  }

  public Bitmap RemoveImageLines(Bitmap Bmp)
  {
    for (int x = 0; x < Bmp.Width; ++x)
    {
      for (int y = 0; y < Bmp.Height; ++y)
      {
        System.Drawing.Color pixel1 = Bmp.GetPixel(x, y);
        if (x - (this.LinesMax + 1) > 0 && y - (this.LinesMax + 1) > 0)
        {
          System.Drawing.Color pixel2 = Bmp.GetPixel(x - this.LinesMin, y - this.LinesMin);
          System.Drawing.Color pixel3 = Bmp.GetPixel(x - this.LinesMax, y - this.LinesMax);
          if (pixel2 == pixel3 && pixel1 != pixel2)
          {
            if (pixel2 != Bmp.GetPixel(x - (this.LinesMin - 1), y - (this.LinesMin - 1)))
              Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
            if (pixel3 != Bmp.GetPixel(x - (this.LinesMax + 1), y - (this.LinesMax + 1)))
              Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
          }
        }
        if (x + (this.LinesMax + 1) < Bmp.Width && y + (this.LinesMax + 1) < Bmp.Height)
        {
          System.Drawing.Color pixel4 = Bmp.GetPixel(x + this.LinesMin, y + this.LinesMin);
          System.Drawing.Color pixel5 = Bmp.GetPixel(x + this.LinesMax, y + this.LinesMax);
          if (pixel4 == pixel5 && pixel1 != pixel4)
          {
            if (pixel4 != Bmp.GetPixel(x + (this.LinesMin - 1), y + (this.LinesMin - 1)))
              Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
            if (pixel5 != Bmp.GetPixel(x + (this.LinesMax + 1), y + (this.LinesMax + 1)))
              Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
          }
        }
      }
    }
    return Bmp;
  }

  public Bitmap RemoveNoiseThreshold(Bitmap Bmp, float threshold)
  {
    Bitmap Bmp1 = new Bitmap(Bmp.Width, Bmp.Height);
    ImageAttributes imageAttr = new ImageAttributes();
    imageAttr.SetThreshold(threshold);
    Point[] destPoints = new Point[3]
    {
      new Point(0, 0),
      new Point(Bmp.Width, 0),
      new Point(0, Bmp.Height)
    };
    Rectangle srcRect = new Rectangle(0, 0, Bmp.Width, Bmp.Height);
    using (Graphics graphics = Graphics.FromImage((Image) Bmp1))
      graphics.DrawImage((Image) Bmp, destPoints, srcRect, GraphicsUnit.Pixel, imageAttr);
    return this.FillWhitespace(Bmp1);
  }

  public Bitmap FillWhitespace(Bitmap Bmp)
  {
    for (int index = 0; index < 3; ++index)
    {
      for (int x = 0; x < Bmp.Width; ++x)
      {
        for (int y = 0; y < Bmp.Height; ++y)
        {
          if (Bmp.GetPixel(x, y) != System.Drawing.Color.White)
          {
            Bmp.SetPixel(x - 1, y - 1, Bmp.GetPixel(x, y));
            Bmp.SetPixel(x - 1, y + 1, Bmp.GetPixel(x, y));
            Bmp.SetPixel(x + 1, y - 1, Bmp.GetPixel(x, y));
            Bmp.SetPixel(x + 1, y + 1, Bmp.GetPixel(x, y));
            Bmp.SetPixel(x, y - 1, Bmp.GetPixel(x, y));
            Bmp.SetPixel(x - 1, y, Bmp.GetPixel(x, y));
            Bmp.SetPixel(x + 1, y, Bmp.GetPixel(x, y));
            Bmp.SetPixel(x, y + 1, Bmp.GetPixel(x, y));
          }
        }
      }
    }
    return Bmp;
  }

  public unsafe Bitmap DilateImage(Bitmap SrcImage)
  {
    Bitmap bitmap = new Bitmap(SrcImage.Width, SrcImage.Height);
    BitmapData bitmapdata1 = SrcImage.LockBits(new Rectangle(0, 0, SrcImage.Width, SrcImage.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
    BitmapData bitmapdata2 = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
    byte[,] numArray = new byte[5, 5]
    {
      {
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0
      },
      {
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0
      }
    };
    int num1 = 5 / 2;
    for (int index1 = num1; index1 < bitmapdata2.Height - num1; ++index1)
    {
      byte* numPtr1 = (byte*) ((IntPtr) (void*) bitmapdata1.Scan0 + index1 * bitmapdata1.Stride);
      byte* numPtr2 = (byte*) ((IntPtr) (void*) bitmapdata2.Scan0 + index1 * bitmapdata1.Stride);
      for (int index2 = num1; index2 < bitmapdata2.Width - num1; ++index2)
      {
        byte num2 = 0;
        for (int index3 = 0; index3 < 5; ++index3)
        {
          int num3 = index3 - num1;
          byte* numPtr3 = (byte*) ((IntPtr) (void*) bitmapdata1.Scan0 + (index1 + num3) * bitmapdata1.Stride);
          for (int index4 = 0; index4 < 5; ++index4)
          {
            int num4 = index4 - num1;
            byte num5 = (byte) (((int) numPtr3[index2 * 3 + num4] + (int) numPtr3[index2 * 3 + num4 + 1] + (int) numPtr3[index2 * 3 + num4 + 2]) / 3);
            if ((int) num2 < (int) num5 && numArray[index3, index4] != (byte) 0)
              num2 = num5;
          }
        }
        *numPtr2 = (byte) (*(sbyte*) (numPtr2 + 1) = *(sbyte*) (numPtr2 + 2) = (sbyte) num2);
        numPtr1 += 3;
        numPtr2 += 3;
      }
    }
    SrcImage.UnlockBits(bitmapdata1);
    bitmap.UnlockBits(bitmapdata2);
    return bitmap;
  }
}
