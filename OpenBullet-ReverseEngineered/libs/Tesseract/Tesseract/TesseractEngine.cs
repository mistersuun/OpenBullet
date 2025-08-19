// Decompiled with JetBrains decompiler
// Type: Tesseract.TesseractEngine
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using Tesseract.Internal;
using Tesseract.Interop;

#nullable disable
namespace Tesseract;

public class TesseractEngine : DisposableBase
{
  private const string TesseractVersion = "3.05.02";
  private static readonly TraceSource trace = new TraceSource("Tesseract");
  private HandleRef handle;
  private int processCount;

  public TesseractEngine(string datapath, string language)
    : this(datapath, language, EngineMode.Default, (IEnumerable<string>) new string[0], (IDictionary<string, object>) new Dictionary<string, object>(), false)
  {
  }

  public TesseractEngine(string datapath, string language, string configFile)
  {
    string datapath1 = datapath;
    string language1 = language;
    string[] configFiles;
    if (configFile == null)
      configFiles = new string[0];
    else
      configFiles = new string[1]{ configFile };
    Dictionary<string, object> initialOptions = new Dictionary<string, object>();
    // ISSUE: explicit constructor call
    this.\u002Ector(datapath1, language1, EngineMode.Default, (IEnumerable<string>) configFiles, (IDictionary<string, object>) initialOptions, false);
  }

  public TesseractEngine(string datapath, string language, IEnumerable<string> configFiles)
    : this(datapath, language, EngineMode.Default, configFiles, (IDictionary<string, object>) new Dictionary<string, object>(), false)
  {
  }

  public TesseractEngine(string datapath, string language, EngineMode engineMode)
    : this(datapath, language, engineMode, (IEnumerable<string>) new string[0], (IDictionary<string, object>) new Dictionary<string, object>(), false)
  {
  }

  public TesseractEngine(
    string datapath,
    string language,
    EngineMode engineMode,
    string configFile)
  {
    string datapath1 = datapath;
    string language1 = language;
    int num = (int) engineMode;
    string[] configFiles;
    if (configFile == null)
      configFiles = new string[0];
    else
      configFiles = new string[1]{ configFile };
    Dictionary<string, object> initialOptions = new Dictionary<string, object>();
    // ISSUE: explicit constructor call
    this.\u002Ector(datapath1, language1, (EngineMode) num, (IEnumerable<string>) configFiles, (IDictionary<string, object>) initialOptions, false);
  }

  public TesseractEngine(
    string datapath,
    string language,
    EngineMode engineMode,
    IEnumerable<string> configFiles)
    : this(datapath, language, engineMode, configFiles, (IDictionary<string, object>) new Dictionary<string, object>(), false)
  {
  }

  public TesseractEngine(
    string datapath,
    string language,
    EngineMode engineMode,
    IEnumerable<string> configFiles,
    IDictionary<string, object> initialOptions,
    bool setOnlyNonDebugVariables)
  {
    Guard.RequireNotNullOrEmpty(nameof (language), language);
    this.DefaultPageSegMode = PageSegMode.Auto;
    this.handle = new HandleRef((object) this, TessApi.Native.BaseApiCreate());
    this.Initialise(datapath, language, engineMode, configFiles, initialOptions, setOnlyNonDebugVariables);
  }

  public string Version => "3.05.02";

  internal HandleRef Handle => this.handle;

  public Page Process(Pix image, PageSegMode? pageSegMode = null)
  {
    return this.Process(image, (string) null, new Rect(0, 0, image.Width, image.Height), pageSegMode);
  }

  public Page Process(Pix image, Rect region, PageSegMode? pageSegMode = null)
  {
    return this.Process(image, (string) null, region, pageSegMode);
  }

  public Page Process(Pix image, string inputName, PageSegMode? pageSegMode = null)
  {
    return this.Process(image, inputName, new Rect(0, 0, image.Width, image.Height), pageSegMode);
  }

  public Page Process(Pix image, string inputName, Rect region, PageSegMode? pageSegMode = null)
  {
    if (image == null)
      throw new ArgumentNullException(nameof (image));
    if (region.X1 < 0 || region.Y1 < 0 || region.X2 > image.Width || region.Y2 > image.Height)
      throw new ArgumentException("The image region to be processed must be within the image bounds.", nameof (region));
    if (this.processCount > 0)
      throw new InvalidOperationException("Only one image can be processed at once. Please make sure you dispose of the page once your finished with it.");
    ++this.processCount;
    PageSegMode pageSegMode1 = pageSegMode.HasValue ? pageSegMode.Value : this.DefaultPageSegMode;
    TessApi.Native.BaseAPISetPageSegMode(this.handle, pageSegMode1);
    TessApi.Native.BaseApiSetImage(this.handle, image.Handle);
    if (!string.IsNullOrEmpty(inputName))
      TessApi.Native.BaseApiSetInputName(this.handle, inputName);
    Page page = new Page(this, image, inputName, region, pageSegMode1);
    page.Disposed += new EventHandler<EventArgs>(this.OnIteratorDisposed);
    return page;
  }

  public Page Process(Bitmap image, PageSegMode? pageSegMode = null)
  {
    return this.Process(image, new Rect(0, 0, image.Width, image.Height), pageSegMode);
  }

  public Page Process(Bitmap image, string inputName, PageSegMode? pageSegMode = null)
  {
    return this.Process(image, inputName, new Rect(0, 0, image.Width, image.Height), pageSegMode);
  }

  public Page Process(Bitmap image, Rect region, PageSegMode? pageSegMode = null)
  {
    return this.Process(image, (string) null, region, pageSegMode);
  }

  public Page Process(Bitmap image, string inputName, Rect region, PageSegMode? pageSegMode = null)
  {
    Pix pix = PixConverter.ToPix(image);
    Page page = this.Process(pix, inputName, region, pageSegMode);
    TesseractEngine.PageDisposalHandle pageDisposalHandle = new TesseractEngine.PageDisposalHandle(page, pix);
    return page;
  }

  protected override void Dispose(bool disposing)
  {
    if (!(this.handle.Handle != IntPtr.Zero))
      return;
    TessApi.Native.BaseApiDelete(this.handle);
    this.handle = new HandleRef((object) this, IntPtr.Zero);
  }

  private string GetTessDataPrefix()
  {
    try
    {
      return Environment.GetEnvironmentVariable("TESSDATA_PREFIX");
    }
    catch (SecurityException ex)
    {
      return (string) null;
    }
  }

  private void Initialise(
    string datapath,
    string language,
    EngineMode engineMode,
    IEnumerable<string> configFiles,
    IDictionary<string, object> initialValues,
    bool setOnlyNonDebugVariables)
  {
    Guard.RequireNotNullOrEmpty(nameof (language), language);
    if (!string.IsNullOrEmpty(datapath))
    {
      datapath = datapath.Trim();
      if (datapath.EndsWith("\\", StringComparison.Ordinal) || datapath.EndsWith("/", StringComparison.Ordinal))
        datapath = datapath.Substring(0, datapath.Length - 1);
      if (datapath.EndsWith("tessdata", StringComparison.OrdinalIgnoreCase))
        datapath = datapath.Substring(0, datapath.Length - "tessdata".Length);
    }
    if (TessApi.BaseApiInit(this.handle, datapath, language, (int) engineMode, configFiles ?? (IEnumerable<string>) new List<string>(), initialValues ?? (IDictionary<string, object>) new Dictionary<string, object>(), setOnlyNonDebugVariables) != 0)
    {
      this.handle = new HandleRef((object) this, IntPtr.Zero);
      GC.SuppressFinalize((object) this);
      throw new TesseractException(ErrorMessage.Format(1, "Failed to initialise tesseract engine."));
    }
  }

  public PageSegMode DefaultPageSegMode { get; set; }

  public bool SetDebugVariable(string name, string value)
  {
    return TessApi.BaseApiSetDebugVariable(this.handle, name, value) != 0;
  }

  public bool SetVariable(string name, string value)
  {
    return TessApi.BaseApiSetVariable(this.handle, name, value) != 0;
  }

  public bool SetVariable(string name, bool value)
  {
    string str = value ? "TRUE" : "FALSE";
    return TessApi.BaseApiSetVariable(this.handle, name, str) != 0;
  }

  public bool SetVariable(string name, int value)
  {
    string str = value.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
    return TessApi.BaseApiSetVariable(this.handle, name, str) != 0;
  }

  public bool SetVariable(string name, double value)
  {
    string str = value.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
    return TessApi.BaseApiSetVariable(this.handle, name, str) != 0;
  }

  public bool TryGetBoolVariable(string name, out bool value)
  {
    int num;
    if (TessApi.Native.BaseApiGetBoolVariable(this.handle, name, out num) != 0)
    {
      value = num != 0;
      return true;
    }
    value = false;
    return false;
  }

  public bool TryGetDoubleVariable(string name, out double value)
  {
    return TessApi.Native.BaseApiGetDoubleVariable(this.handle, name, out value) != 0;
  }

  public bool TryGetIntVariable(string name, out int value)
  {
    return TessApi.Native.BaseApiGetIntVariable(this.handle, name, out value) != 0;
  }

  public bool TryGetStringVariable(string name, out string value)
  {
    value = TessApi.BaseApiGetStringVariable(this.handle, name);
    return value != null;
  }

  public bool TryPrintVariablesToFile(string filename)
  {
    return TessApi.Native.BaseApiPrintVariablesToFile(this.handle, filename) != 0;
  }

  private void OnIteratorDisposed(object sender, EventArgs e) => --this.processCount;

  private class PageDisposalHandle
  {
    private readonly Page page;
    private readonly Pix pix;

    public PageDisposalHandle(Page page, Pix pix)
    {
      this.page = page;
      this.pix = pix;
      page.Disposed += new EventHandler<EventArgs>(this.OnPageDisposed);
    }

    private void OnPageDisposed(object sender, EventArgs e)
    {
      this.page.Disposed -= new EventHandler<EventArgs>(this.OnPageDisposed);
      this.pix.Dispose();
    }
  }
}
